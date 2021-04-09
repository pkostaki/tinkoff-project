using TTApplication.Services.Data;

namespace TTApplication.Services
{
    /// <summary>
    /// Service provider
    /// </summary>
    public class ServiceProvider
    {
        private IDataProvider _dataProvider;
        public IDataProvider DataProvider => _dataProvider ?? (_dataProvider = new DataProvider("https://config.tinkoff.ru/resources?name=products_info"));

        private IPopupService _popupService;
        public IPopupService PopupService => _popupService ?? (_popupService = new PopupService());
    }
}
