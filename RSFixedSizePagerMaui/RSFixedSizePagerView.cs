namespace RSFixedSizePagerMaui
{
    public class RSFixedSizePagerView : CarouselView, IDisposable
    {
        public static readonly BindableProperty RSIndicatorProperty = BindableProperty.Create(nameof(RSIndicator), typeof(RSPagerIndicatorView), typeof(RSFixedSizePagerView), null, propertyChanged: OnRSIndicatorPropertyChanged);
        public RSPagerIndicatorView RSIndicator
        {
            get { return (RSPagerIndicatorView)GetValue(RSIndicatorProperty); }
            set { SetValue(RSIndicatorProperty, value); }
        }
        private static void OnRSIndicatorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null && newValue == null)
                return;

            (newValue as RSPagerIndicatorView).RSFixedSizePager = bindable as RSFixedSizePagerView;
        }


        private double pageWidth = 0;
        private double itemWidth = 0;
        private double ratio;


        public RSFixedSizePagerView()
        {
            Loop = false;
            IsBounceEnabled = false;
        }

        protected override void OnScrolled(ItemsViewScrolledEventArgs e)
        {
            base.OnScrolled(e);

            if (RSIndicator == null)
                return;

            pageWidth = this.Width;
            itemWidth = pageWidth / (RSIndicator.Children.Count - 1);

            ratio = itemWidth / pageWidth;
            RSIndicator.Slider.WidthRequest = RSIndicator.Slider.WidthRequest != itemWidth ? itemWidth : RSIndicator.Slider.WidthRequest;
            RSIndicator.Slider.TranslationX += e.HorizontalDelta * ratio;
        }

        protected override void OnPositionChanged(PositionChangedEventArgs args)
        {
            base.OnPositionChanged(args);
            HighLightItem(args.CurrentPosition, args.PreviousPosition);
        }

        //private VisualElement previousSelected;
        private void HighLightItem(int currentIndex, int previousIndex)
        {
            var currentItem = RSIndicator.ElementAt(currentIndex) as VisualElement;
            var previousItem = RSIndicator.ElementAt(previousIndex) as VisualElement;

            VisualStateManager.GoToState(currentItem, "selected");
            if (previousItem != null)
                VisualStateManager.GoToState(previousItem, "normal");

            currentItem?.ScaleTo(1.1);
            previousItem?.ScaleTo(1);
        }

        public void Dispose()
        {
           
        }
    }
}
