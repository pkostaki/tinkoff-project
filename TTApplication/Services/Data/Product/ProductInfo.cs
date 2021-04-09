using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TTApplication.TT
{
    [DataContract]
    public  class ProductInfo : IProductInfo
    {
        [DataMember( Name = "id")]
        public string Id { get; set; }

        [DataMember( Name = "programId")]
        public string ProgramId { get; set; }

        [DataMember( Name = "product")]
        public string Product { get; set; }

        [DataMember( Name = "order")]
        public long Order { get; set; }

        [DataMember( Name = "type")]
        public string Type { get; set; }

        [DataMember( Name = "title")]
        public string Title { get; set; }

        [DataMember( Name = "slogan")]
        public string Slogan { get; set; }

        [DataMember( Name = "multiCurrencies")]
        public bool MultiCurrencies { get; set; }

        [DataMember( Name = "benefits")]
        public List<Benefit> Benefits { get; set; }

        [DataMember( Name = "hrefTariff")]
        public string HrefTariff { get; set; }

        [DataMember( Name = "bgColor")]
        public string BgColor { get; set; }
    }
}