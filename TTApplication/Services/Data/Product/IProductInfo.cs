using System.Collections.Generic;

namespace TTApplication.TT
{
    /// <summary>
    /// Represents product 
    /// </summary>
    public interface IProductInfo
    {
        string Id { get; set; }
        string ProgramId { get; set; }
        string Product { get; set; }
        long Order { get; set; }
        string Type { get; set; }

        string Title { get; set; }
        string Slogan { get; set; }
        bool MultiCurrencies { get; set; }
        List<Benefit> Benefits { get; set; }
        string HrefTariff { get; set; }
        string BgColor { get; set; }
    }
}