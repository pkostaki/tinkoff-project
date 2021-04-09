using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using TTApplication.Mvvm.ViewModel;



namespace TTApplication.Mvvm.View
{
    public sealed partial class ProductControl
    {
        public bool IsVerticalEmbeddedView
        {
            get { return (bool)GetValue(IsVerticalEmbeddedViewProperty); }
            set { SetValue(IsVerticalEmbeddedViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsVerticalEmbeddedView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsVerticalEmbeddedViewProperty =
            DependencyProperty.Register("IsVerticalEmbeddedView", typeof(bool), typeof(ProductControl),
                new PropertyMetadata(false, (o, args) =>
                {
                    VisualStateManager.GoToState((Control)o, (bool)args.NewValue ? "VerticalLayout" : "DefaultLayout", false);
                }));


        private double _progress = double.PositiveInfinity;
        private ProductViewModel ViewModel => DataContext as ProductViewModel;

        public ProductControl()
        {
            this.InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, IsVerticalEmbeddedView? "VerticalLayout" : "DefaultLayout", false);
            InvalidateVisualState();
        }

        private void Image_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateVisualState();
        }

        public override void ProcessVisibleState(double progress)
        {
            _progress = progress;
            InvalidateVisualState();
        }

        private void InvalidateVisualState()
        {
            if(double.IsPositiveInfinity(_progress))
            {
                return;
            }
            FindName(nameof(Image));
            if (IsVerticalEmbeddedView)
            {
                UpdateTranslateTransform(Image,0);
                UpdateTranslateTransform(Benefits,0);
                Benefits.Opacity = 1;
                return;
            }

            UpdateTranslateTransform(Image, FuncImage(_progress));

            Benefits.Opacity = _progress < 0 ? 1 + _progress : 1 - _progress;
            UpdateTranslateTransform(Benefits, ActualWidth*0.5);
        }

        private void UpdateTranslateTransform(UIElement uiElement, double value)
        {
            (uiElement.RenderTransform as TranslateTransform).X = value;
        }

        public override void NotifyPlacedCenter()
        {
            ViewModel.Select();
        }

        public override void NotifySelected()
        {
            ViewModel.Click();
        }

        public override void ProcessHiddenState(double progress)
        {
            if (Math.Floor(Math.Abs(progress)) < 4)
            {
                FindName(nameof(Image));
            }
        }

        private double FuncImage(double progress)
        {
            const double keepProgressPoint = 0.3;
            var startX = ActualWidth * 0.5 - Image.ActualWidth;
            if (Math.Abs(progress) < keepProgressPoint)
            {
                return startX;
            }

            if (progress < 0)
            {
                return startX -(ActualWidth * 0.5) * (progress + keepProgressPoint) / (1 - keepProgressPoint);
            }

            return startX * (1 - (progress - keepProgressPoint) / (1 - keepProgressPoint));

        }

    }
}
