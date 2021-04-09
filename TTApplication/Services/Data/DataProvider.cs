using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTApplication.Helpers;
using TTApplication.TT;
using HttpCompletionOption = Windows.Web.Http.HttpCompletionOption;

namespace TTApplication.Services.Data
{
    public class DataProvider:IDataProvider
    {
        private readonly Lazy<Task<List<IProductInfo>>> _loading;
        private readonly string _uriPath;
        

        public DataProvider(string uriPath)
        {
            _uriPath = uriPath;
            _loading = new Lazy<Task<List<IProductInfo>>>(LoadData);
        }

        
        private async Task<List<IProductInfo>> LoadData()
        {
            var ret = new List<IProductInfo>();
            using (var httpClient = new Windows.Web.Http.HttpClient())
            {
                try
                {
                    var result = await httpClient.GetAsync(new Uri(_uriPath), HttpCompletionOption.ResponseHeadersRead);
                    if (result.IsSuccessStatusCode)
                    {
                        var json = await result.Content.ReadAsStringAsync();
                        var parsedObject = JsonHelper.Deserialize<RequestResult<RequestDetails<ProductInfo>>>(json);

                        ret = parsedObject.Result.Value.Select(info => info as IProductInfo).ToList();
                    }
                }
                catch (Exception ex)
                {
                    // Details in ex.Message and ex.HResult.
                }
            }

            return ret;

        }

        public Task<List<IProductInfo>> ProductsInfo => _loading.Value;
    }
}
