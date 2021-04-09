using System.Collections.Generic;
using System.Threading.Tasks;
using TTApplication.TT;

namespace TTApplication.Services.Data
{

    /// <summary>
    /// Data provider
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// List of products
        /// </summary>
        Task<List<IProductInfo>> ProductsInfo { get; }
    }
}
