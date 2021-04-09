using TTApplication.Mvvm.ViewModel.Common;

namespace TTApplication.Mvvm.ViewModel
{
    /// <summary>
    /// Benefit view model
    /// </summary>
    public class BenefitViewModel:BindableBase
    {
        private string _text;

        public string Text
        {
            get => _text;
            set => this.SetProperty(ref _text, value);
        }
    }
}
