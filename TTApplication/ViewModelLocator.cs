using TTApplication.Mvvm.ViewModel;
using TTApplication.Services;

namespace TTApplication
{
    public class ViewModelLocator
    {
        private readonly ServiceProvider _services = new ServiceProvider();
        public MainPageViewModel MainPageViewModel => new MainPageViewModel(_services.DataProvider, _services.PopupService);
    }
}
