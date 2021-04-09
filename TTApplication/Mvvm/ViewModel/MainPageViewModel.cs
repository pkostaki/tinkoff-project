using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TTApplication.Mvvm.ViewModel.Common;
using TTApplication.Services;
using TTApplication.Services.Data;
using TTApplication.TT;
using Windows.UI;

namespace TTApplication.Mvvm.ViewModel
{
    /// <summary>
    /// Mainpage view model
    /// </summary>
    public class MainPageViewModel:BindableBase
    {
        public ObservableCollection<ProductViewModel> Products;
        private readonly IDataProvider _dataProvider;
        private bool _activatePageLoadingProgressRing;
        private readonly IPopupService _popupService;

        public bool ActivatePageLoadingProgressRing
        {
            get => _activatePageLoadingProgressRing;
            set => SetProperty(ref _activatePageLoadingProgressRing ,  value);
        }
        
        private string _bgColor = Colors.Transparent.ToString();

        public string BgColor
        {
            get => _bgColor;
            set => SetProperty(ref _bgColor,  value);
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="dataProvider">dataprovider</param>
        /// <param name="popupService">popupservice</param>
        public MainPageViewModel(IDataProvider dataProvider, IPopupService popupService)
        {
            _popupService = popupService;
            _dataProvider = dataProvider;
#pragma warning disable 4014
            CreateProductList();
#pragma warning restore 4014
        }

        private async Task CreateProductList()
        {
            ActivatePageLoadingProgressRing = true;
            Products = new ObservableCollection<ProductViewModel>();
            
            var products = await _dataProvider.ProductsInfo;
            foreach (var productInfo in products)
            {
                Products.Add(CreateProductItem(productInfo));
            }

            ActivatePageLoadingProgressRing = false;
        }

        private ProductViewModel CreateProductItem(IProductInfo productInfo)
        {
            var result = new ProductViewModel
            {
                LogoUri = new Uri(
                    $"https://static.tcsbank.ru/icons/new-products/windows/{productInfo.Type}/400/{productInfo.ProgramId}.png"),
                BgColor = productInfo.BgColor,
                Order = productInfo.Order,
                SelectedAction = model => BgColor = model.BgColor,
                Clicked = model => _popupService.ShowProductPopup(model)
            };
            foreach (var productInfoBenefit in productInfo.Benefits)
            {
                result.Benefits.Add(new BenefitViewModel{Text = productInfoBenefit.Text});
            }
            
            return result;
        }
    }
}
