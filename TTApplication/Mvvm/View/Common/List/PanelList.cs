using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Uwp.UI.Extensions;


namespace TTApplication.Mvvm.View.Common.List
{
    /// <summary>
    /// Organize items as cycle list with one item at center and others by sides.
    /// </summary>
    public class PanelList : Panel
    {
        /// <summary>
        /// Set space around center item between edges of the list
        /// </summary>
        public int SideSpace
        {
            get => (int) GetValue(SideSpaceProperty);
            set => SetValue(SideSpaceProperty, value);
        }

        // Using a DependencyProperty as the backing store for SideSpace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SideSpaceProperty =
            DependencyProperty.Register("SideSpace", typeof(int), typeof(PanelList), new PropertyMetadata(100, (o, args) => (o as PanelList).ImmediatelyOrder(true)));

        private CycleSequence _sequence;
        private double _offsetSeparator;
        private ValueAnimator _animator;
        private bool _isManipulationInProgress;

        private double ItemWidth => ActualWidth == 0 ? 0 : ActualWidth - SideSpace * 2;
        private int ItemsCount => Children?.Count ?? 0;

        public PanelList()
        {
            Loaded += OnLoaded;
            Unloaded+=OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            _animator = new ValueAnimator();
            _animator.ValueChanged += SliderOnValueChanged;

            ManipulationMode = (ManipulationModes.TranslateX);
            ManipulationDelta += OnManipulationDelta;
            ManipulationCompleted += OnManipulationCompleted;
            ManipulationStarted += OnManipulationStarted;

            SizeChanged += OnSizeChanged;
            PointerWheelChanged += OnPointerWheelChanged;
            Tapped += OnTapped;
        }

        private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _isManipulationInProgress = true;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnloaded;

            PointerWheelChanged-=OnPointerWheelChanged;
            ManipulationStarted -= OnManipulationStarted;
            ManipulationDelta -= OnManipulationDelta;
            ManipulationCompleted -= OnManipulationCompleted;
            _animator.ValueChanged -= SliderOnValueChanged;
            _animator.Dispose();
            SizeChanged -= OnSizeChanged;
            Tapped -= OnTapped;
        }


        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //when user maximize/minimize window ArrangeOverride doesn't executes by framework, so call manually
            ArrangeOverride(e.NewSize);
        }

        #region Input manipulations
        private void OnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            if(_isManipulationInProgress)
            {
                //sometimes fires tap event during manipulation
                return;
            }

            var point = tappedRoutedEventArgs.GetPosition(this);
            UIElement selected = null;
            foreach (var child in Children)
            {
                var containsPoint = child.TransformToVisual(this)
                    .TransformBounds(new Rect(0, 0, child.RenderSize.Width, child.RenderSize.Height)).Contains(point);
                if (!containsPoint)
                {
                    continue;
                }

                selected = child;
                break;
            }


            if (selected == null)
            {
                return;
            }

            tappedRoutedEventArgs.Handled = true;
            _animator.Complete();
            if (selected == FindCenterItem())
            {
                NotifyItemSelected(selected);
                return;
            }
            
            ScrollToElement(selected);
        }


        private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;

            var delta = e.GetCurrentPoint(this).Properties.MouseWheelDelta;
            ScrollTo(delta > 0 ? ItemWidth : -ItemWidth);
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            _isManipulationInProgress = false;
            _animator.Complete();
            var currentItem = FindCenterItem();
            if (currentItem == null)
            {
                return;
            }

            e.Handled = true;
            ScrollToElement(currentItem);

        }
        #endregion

        private void NotifyItemSelected(UIElement item)
        {
            var panelItem = item.FindDescendant<PanelItemPresentationAbstract>();
            panelItem?.NotifySelected();
        }

        private UIElement FindCenterItem()
        {
            UIElement result = null;
            double min = double.MaxValue;
            var containerCenterX = ActualWidth * 0.5d;
            foreach (var child in Children)
            {
                var transform = child.TransformToVisual(this).TransformBounds(new Rect(0, 0, child.RenderSize.Width, child.RenderSize.Height));
                var p = new Vector2((float) containerCenterX, 0) - new Vector2((float) (transform.X + transform.Width * 0.5), 0);
                var distance = p.LengthSquared();
                if (distance < min)
                {
                    min = distance;
                    result = child;
                }
            }

            return result;
        }

        private void ScrollToElement(UIElement uiElement)
        {
            _animator.Complete();
            var offset = GetOffsetToReachCenter(uiElement);
            var compositeTransform = (TranslateTransform) uiElement.RenderTransform;
            _animator.Run(compositeTransform.X, offset);
        }      
        
        private void ScrollTo(double offset)
        {
            _animator.Complete();
            _animator.Run(_offsetSeparator, offset);
        }

        private double GetOffsetToReachCenter(UIElement uiElement)
        {
            var rect = uiElement.TransformToVisual(this).TransformBounds(new Rect(0, 0, uiElement.RenderSize.Width * 0.5, uiElement.RenderSize.Height));
            var targetX = ActualWidth * 0.5;
            return  targetX - rect.X - rect.Width;
        }

        private void SliderOnValueChanged(object sender, double offset)
        {
            if (Math.Abs(offset) < double.Epsilon)
            {
                return;
            }

            UpdatePositions(offset);
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            _animator.Complete();
            UpdatePositions(e.Delta.Translation.X);
        }
        
        private void UpdatePositions(double offsetDelta)
        {
            if (ItemsCount == 1)
            {
                PlaceOneItem();
                return;
            }

            var maxLogicalWidth = ItemsCount * ItemWidth;

            _offsetSeparator = (_offsetSeparator + offsetDelta) % maxLogicalWidth;

            var itemNumberSeparator = (int)(Math.Abs(_offsetSeparator) / ItemWidth);
            int itemIndexChanging;
            double offsetAfter;
            double offsetBefore;

            if (_offsetSeparator > 0)
            {
                //scroll right direction
                itemIndexChanging = ItemsCount - itemNumberSeparator - 1;
                offsetAfter = _offsetSeparator;
                offsetBefore = offsetAfter - maxLogicalWidth;
            }
            else
            {
                //scroll left direction
                itemIndexChanging = itemNumberSeparator;
                offsetBefore = _offsetSeparator;
                offsetAfter = maxLogicalWidth + offsetBefore;
            }

            // items that must be before
            UpdatePosition(itemIndexChanging, this.ItemsCount, offsetBefore);

            // items that must be after
            UpdatePosition(0, itemIndexChanging, offsetAfter);
        }

        private void PlaceOneItem()
        {
            var child = Children.FirstOrDefault();
            var translateTransform = child.RenderTransform as TranslateTransform;
            translateTransform.X = 0;
            NotifyElementAboutMovingProcess(child);
        }

        private void UpdatePosition(int startIndex, int endIndex, double offset)
        {
            for (var i = startIndex; i < endIndex; i++)
            {
                var item = Children[_sequence.Total[i]];

                PlaceToPosition(item, offset, false);
                NotifyElementAboutMovingProcess(item);
            }
        }

        private void NotifyElementAboutMovingProcess(UIElement item)
        {
            var panelItem = item.FindDescendant<PanelItemPresentationAbstract>();
            if ( panelItem == null)
            {
                return;
            }

            var bounds = item.TransformToVisual(this).TransformBounds(new Rect(0, 0, item.RenderSize.Width*0.5, item.RenderSize.Height));
            var itemCenter = bounds.X + bounds.Width;
            var containerCenterX = ActualWidth * 0.5d;
            if (Math.Abs(containerCenterX - itemCenter) <= 10)
            {
                panelItem.ProcessVisibleState(0);
                panelItem.NotifyPlacedCenter();
                return;
            }
            
            var distance = itemCenter - containerCenterX;
            var ratio =  distance / ItemWidth;
            if (Math.Abs(ratio) > 2)
            {
                //ignore any updates of items which is out of the screen and notify the item about it 
                panelItem.ProcessHiddenState(ratio);
                return;
            }

            var isRightFromCenter = distance > 0;
            panelItem.ProcessVisibleState(isRightFromCenter ? Math.Min(1, ratio) : Math.Max(-1, ratio));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var itemWidth = 0d;
            var itemHeight = 0d;
            foreach (var child in Children)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                itemWidth = child.DesiredSize.Width;
                itemHeight = child.DesiredSize.Height;
            }

            var result = new Size(double.IsPositiveInfinity(availableSize.Width) ? itemWidth : availableSize.Width,
                double.IsPositiveInfinity(availableSize.Height) ? itemHeight : availableSize.Height);
            Clip = new RectangleGeometry { Rect = new Rect(0, 0, result.Width, result.Height) };
            return result;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ImmediatelyOrder(true);
            return finalSize;
        }


        private void ImmediatelyOrder(bool callArrange)
        {
            if (ItemsCount == 0)
            {
                return;
            }

            _animator.Complete();
            var currentItem = FindCenterItem();
            var currentIndex = Children.IndexOf(currentItem);
            
            var half = (int) Math.Floor((ItemsCount - 1) * 0.5);
            var offset = -ItemWidth * half + SideSpace;
            _sequence = new CycleSequence(currentIndex, ItemsCount);
            var indexes = _sequence.Total;
            for (var i = 0; i < indexes.Count; i++)
            {
                var index = indexes[i];
                PlaceToPosition(Children[index], offset, callArrange);
                NotifyElementAboutMovingProcess(Children[index]);
                offset += ItemWidth;
            }

            _offsetSeparator = 0;
        }

        private void PlaceToPosition(UIElement item, double offset, bool callArrange)
        {
            var itemRenderTransform =  item.RenderTransform as TranslateTransform;
            if (itemRenderTransform == null)
            {
                item.RenderTransform = new TranslateTransform();
                itemRenderTransform =  item.RenderTransform as TranslateTransform;
            }

            if (callArrange)
            {
                item.Arrange(new Rect(offset, 0, ItemWidth, item.DesiredSize.Height));
                itemRenderTransform.X = 0;
            }
            else
            {
                itemRenderTransform.X = offset;    
            }
        }

        /// <summary>
        /// Realize smooth changing of value from-to
        /// </summary>
        private class ValueAnimator
        {
            /// <summary>
            /// Value changed event. Pass offset as param
            /// </summary>
            public event EventHandler<double> ValueChanged;
            private readonly Storyboard _storyboard;
            private bool _lockEventFiring;
            private TranslateTransform _render;
            private readonly long _eventId;
            private double _value;

            public double Value
            {
                private set
                {
                    var offset = value - _value;
                    _value = value;
                    ValueChanged?.Invoke(this, offset);
                }
                get => _value;
            }

            public ValueAnimator()
            {
                _render = new TranslateTransform();
                _eventId = _render.RegisterPropertyChangedCallback(TranslateTransform.XProperty, OnPropertyChanged);

                _storyboard = new Storyboard();
                Storyboard.SetTarget(_storyboard, _render);
                Storyboard.SetTargetProperty(_storyboard, nameof(_render.X));
            }

            private void OnPropertyChanged(DependencyObject sender, DependencyProperty dp)
            {
                if(_lockEventFiring)
                {
                    return;
                }

                Value = (sender as TranslateTransform).X;
            }

            public void Run(double start, double offset)
            {
                _storyboard.Children.Add(new DoubleAnimation
                {
                    AutoReverse = false,
                    From = start,
                    To = start + offset,
                    EnableDependentAnimation = true,
                    EasingFunction = new ExponentialEase {EasingMode = EasingMode.EaseInOut},
                    Duration = new Duration(TimeSpan.FromMilliseconds(300))
                });

                _value = start;
                _storyboard.Begin();
            }

            public void Complete()
            {
                var state = _storyboard.GetCurrentState();
                if (state == ClockState.Active)
                {
                    if (_storyboard.Children.FirstOrDefault() is DoubleAnimation animation)
                    {
                        Value = (double) animation.To;
                    }
                }

                _lockEventFiring = true;
               _storyboard.Stop();
               _storyboard.Children.Clear();
               _lockEventFiring = false;
            }

            internal void Dispose()
            {
                _render.UnregisterPropertyChangedCallback(TranslateTransform.XProperty, _eventId);
            }
        }

        /// <summary>
        /// Represent cycle sequence numbers
        /// </summary>
        private class CycleSequence
        {
            public List<int> Total { get; } = new List<int>();
            public List<int> LeftPart{ get; } =new List<int>();
            public List<int> RightPart { get; } = new List<int>();
            public int Center { get; }

            private int _startIndex;
            private readonly int _itemsCount;

            /// <summary>
            /// ctor
            /// </summary>
            /// <param name="center">center element</param>
            /// <param name="count">total items at sequence</param>
            public CycleSequence(int center, int count)
            {
                Center = center;
                _itemsCount = count;

                Process();
            }
            
            private int ProcessNextIndex()
            {
                return _startIndex < _itemsCount - 1 ? ++_startIndex : _startIndex = 0;
            }
        
            private int ProcessPreviousIndex()
            {
                return _startIndex > 0 ? --_startIndex: _startIndex = _itemsCount - 1;
            }

            private void Process()
            {
                _startIndex = Center;
                int half = (int)Math.Floor((_itemsCount - 1) * 0.5);

                while (LeftPart.Count< half)
                {
                    LeftPart.Add(ProcessPreviousIndex());
                }

                LeftPart.Reverse();
                Total.AddRange(LeftPart);
                Total.Add(Center);
            
                _startIndex = Center;
            
                while (RightPart.Count + LeftPart.Count< _itemsCount-1)
                {
                    RightPart.Add(ProcessNextIndex());
                }
                Total.AddRange(RightPart);
            }

            public override string ToString()
            {
                return $"left: {string.Join("|", LeftPart)} right: {string.Join("|", RightPart)} " ;
            }
        }
    }
}