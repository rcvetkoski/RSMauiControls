namespace ViewPagerMaui
{
    public class ViewPager : CollectionView
    {
        public static readonly BindableProperty ItemBindingPathProperty = BindableProperty.Create(nameof(ItemBindingPath), typeof(string), typeof(ViewPager), ".");
        public string ItemBindingPath
        {
            get { return (string)GetValue(ItemBindingPathProperty); }
            set { SetValue(ItemBindingPathProperty, value); }
        }



        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            set
            {
                currentIndex = value;
            }
        }

        private int currentIndex = 0;

        public double OffsetX { get; set; } = 0;

        public event EventHandler CurrentIndexChanged;

#if ANDROID
        private AndroidX.RecyclerView.Widget.RecyclerView recyclerView;
#endif

#if IOS
        private UIKit.UICollectionView uICollectionView;
#endif

        public ViewPager()
        {
            ItemsLayout = LinearItemsLayout.Horizontal;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
            SetDefaultContentItemTemplate();
        }

        static ViewPager()
        {
            Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("viewPager", (handler, view) =>
            {
                if (view is ViewPager viewPager)
                {
#if ANDROID
                    viewPager.recyclerView = handler.PlatformView;

                    if (viewPager.recyclerView.GetOnFlingListener() == null)
                    {
                        var pagerSnapHelper = new AndroidX.RecyclerView.Widget.PagerSnapHelper();
                        pagerSnapHelper.AttachToRecyclerView(viewPager.recyclerView);
                    }
#endif

#if IOS
                    var platformView = handler.PlatformView;
                    var nativeControl = platformView.Subviews.FirstOrDefault();

                    if (nativeControl != null) 
                    {
                        viewPager.uICollectionView = nativeControl as UIKit.UICollectionView;
                        viewPager.uICollectionView.PagingEnabled = true;
                        viewPager.uICollectionView.Bounces = false;
                        viewPager.uICollectionView.ShowsHorizontalScrollIndicator = false;    
                    }
#endif
                }
            });
        }

        private void SetDefaultContentItemTemplate()
        {
            DataTemplate DefaultContentItemTemplate = new DataTemplate(() =>
            {
                var label = new Label()
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Fill,
                    VerticalTextAlignment = TextAlignment.Center
                };

                label.SetBinding(Label.TextProperty, ItemBindingPath);

                return label;
            });

            this.ItemTemplate = DefaultContentItemTemplate;
        }

        protected override void OnScrolled(ItemsViewScrolledEventArgs e)
        {
            base.OnScrolled(e);

            double leftX = currentIndex > 0 ? (this.Width * currentIndex) - this.Width : -this.Width;
            double rightX = currentIndex > 0 ? (this.Width * (currentIndex + 1)) : this.Width;

            if ((e.HorizontalOffset + OffsetX) >= rightX)
            {
                currentIndex++;
                OnCurrentIndexChanged();
            }
            else if ((e.HorizontalOffset + OffsetX) <= leftX)
            {
                currentIndex--;
                OnCurrentIndexChanged();
            }
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);

            (child as View).SetBinding(View.WidthRequestProperty, new Binding(nameof(ViewPager.Width), source: this));
            //(child as View).SetBinding(View.HeightRequestProperty, new Binding(nameof(ViewPager.Height), source: this));
        }

        /// <summary>
        /// Scrolls the ViewPager to given item position
        /// </summary>
        /// <param name="position"></param>
        public void ScrollTo(int position)
        {
#if ANDROID
            if (recyclerView == null) return;
            var xOffset = recyclerView.ComputeHorizontalScrollOffset();
            var scrollX = (int)(recyclerView.Width * position) - xOffset;
            recyclerView.SmoothScrollBy(scrollX, 0);
#endif
#if IOS
            if (uICollectionView == null) return;
            var scrollX = uICollectionView.Frame.Width * position;
            uICollectionView.SetContentOffset(new CoreGraphics.CGPoint(scrollX, 0), true);
#endif
        }

        public void OnCurrentIndexChanged()
        {
            CurrentIndexChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}