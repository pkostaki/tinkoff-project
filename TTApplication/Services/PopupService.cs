using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using TTApplication.Mvvm.ViewModel;

namespace TTApplication.Services
{
    public class PopupService: IPopupService
    {
        public Task ShowProductPopup(ProductViewModel info)
        {
            ContentDialog content = new ContentDialog
            {
                ContentTemplate = (DataTemplate) App.Current.Resources["ContentDialogProductInfo"],
                DataContext = info,
                Background = new SolidColorBrush((Color) XamlBindingHelper.ConvertValue(typeof(Color), info.BgColor)),
                PrimaryButtonText = "Ok",
            };
            return content.ShowAsync().AsTask();
        }
    }

    public interface IPopupService
    {
        /// <summary>
        /// Show product popup
        /// </summary>
        /// <param name="info">product presentation model</param>
        /// <returns></returns>
        Task ShowProductPopup(ProductViewModel info);

    }
}
