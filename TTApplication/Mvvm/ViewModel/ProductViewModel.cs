using System;
using System.Collections.Generic;
using TTApplication.Mvvm.ViewModel.Common;

namespace TTApplication.Mvvm.ViewModel
{

    /// <summary>
    /// Product view model
    /// </summary>
    public class ProductViewModel : BindableBase
    {
        public long Order { get; set; }
        private string _bgColor;
        private Uri _logoUri;

        public Uri LogoUri
        {
            get => _logoUri;
            set
            {
                SetProperty(ref _logoUri,  value);
            }
        }

        public List<BenefitViewModel> Benefits { get; } = new List<BenefitViewModel>();

        public string BgColor
        {
            get => _bgColor;
            set => SetProperty(ref _bgColor,  value);
        }

        public Action<ProductViewModel> Clicked { get; set; }
        public Action<ProductViewModel> SelectedAction { get; set; }

        public void Click()
        {
            Clicked?.Invoke(this);
        }
        public void Select()
        {
            SelectedAction?.Invoke(this);
        }
    }
}
