using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Extensions
{
    /// <summary>
    /// Provide image with progress ring to point up progress of loading
    /// </summary>
    public class ImageProgressLoadingBehaviour:Microsoft.Xaml.Interactivity.Behavior<Image>
    {
        private Panel ParentPanel => AssociatedObject.Parent as Panel;
        private ProgressRing _progressRing;
        private double _imageWidth;
        private double _imageHeight;
        private long _eventIdX;
        private long _eventIdY;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnImageLoaded;
         }


        private void OnImageLoaded(object sender, RoutedEventArgs args)
        {
            if (ParentPanel == null)
            {
                return;
            }

            DefineSize();
            AddProgressRing();
            UpdateProgressRingPosition();
            AssociatedObject.ImageOpened += OnImageOpened;
            AssociatedObject.LayoutUpdated += OnLayoutUpdated;
            var transform = (AssociatedObject.RenderTransform as TranslateTransform);
            if(transform!=null)
            {
               _eventIdX = transform.RegisterPropertyChangedCallback(TranslateTransform.XProperty, OnPositionChanged);
               _eventIdY = transform.RegisterPropertyChangedCallback(TranslateTransform.YProperty, OnPositionChanged);
            }
        }

        private void DefineSize()
        {
            var bi = AssociatedObject.Source as BitmapImage;
            if (!double.IsNaN(AssociatedObject.Width) && !double.IsInfinity(AssociatedObject.Width))
            {
                _imageWidth = AssociatedObject.Width;
            }
            else if (bi != null)
            {
                _imageWidth = bi.DecodePixelWidth;
            }

            if (!double.IsNaN(AssociatedObject.Height) && !double.IsInfinity(AssociatedObject.Height))
            {
                _imageHeight = AssociatedObject.Height;
            }
            else if (bi != null)
            {
                _imageHeight = bi.DecodePixelHeight;
            }
        }

        private void AddProgressRing()
        {
            _progressRing = new ProgressRing
            {
                Width = 20,
                Height = 20,
                IsActive = true,
                RenderTransform = new TranslateTransform(),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            ParentPanel.Children.Add(_progressRing);
        }

        private void OnPositionChanged(DependencyObject sender, DependencyProperty dp)
        {
            UpdateProgressRingPosition();
        }

        private void OnLayoutUpdated(object sender, object e)
        {
            UpdateProgressRingPosition();
        }

        private void UpdateProgressRingPosition()
        {
            var bounds = AssociatedObject.TransformToVisual(ParentPanel).TransformBounds(new Rect(0, 0, _imageWidth, _imageHeight));

            TranslateTransform translateTransform = _progressRing.RenderTransform as TranslateTransform;
            translateTransform.X = bounds.Left + (bounds.Width - _progressRing.Width) * 0.5;
            translateTransform.Y = bounds.Top + (bounds.Height - _progressRing.Height) * 0.5;
        }

        private void OnImageOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Dispose();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            Dispose();
        }

        private void Dispose()
        {
            AssociatedObject.Loaded -= OnImageLoaded;
            AssociatedObject.ImageOpened -= OnImageOpened;
            AssociatedObject.LayoutUpdated -= OnLayoutUpdated;
            var transform = (AssociatedObject.RenderTransform as TranslateTransform);
            if (transform != null)
            {
                transform.UnregisterPropertyChangedCallback(TranslateTransform.XProperty, _eventIdX);
                transform.UnregisterPropertyChangedCallback(TranslateTransform.YProperty, _eventIdY);
            }
            RemoveProgressRing();
        }

        private void RemoveProgressRing()
        {
            if (_progressRing == null)
            {
                return;
            }
            _progressRing.IsActive = false;
            
            ParentPanel?.Children?.Remove(_progressRing);
            _progressRing = null;
        }
    }
}
