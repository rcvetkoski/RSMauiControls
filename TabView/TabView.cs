using System.Collections;
using System.Windows.Input;
using ViewPagerMaui;

namespace TabViewMaui
{
    public class TabView : Grid, IDisposable
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(TabView), null, propertyChanged: OnPropertyChanged);
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView).viewPager.ItemsSource = (IEnumerable)newValue;
            BindableLayout.SetItemsSource((bindable as TabView).tabsContent, (IEnumerable)newValue);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(TabView), null, BindingMode.TwoWay);
        public object SelectedItem
        {
            get { return (IEnumerable)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }


        public static readonly BindableProperty ContentItemTemplateProperty = BindableProperty.Create(nameof(ContentItemTemplate), typeof(DataTemplate), typeof(TabView), propertyChanged: OnContentItemTemplatePropertyChanged);
        public DataTemplate ContentItemTemplate
        {
            get { return (DataTemplate)GetValue(ContentItemTemplateProperty); }
            set { SetValue(ContentItemTemplateProperty, value); }
        }
        private static void OnContentItemTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView).viewPager.ItemTemplate = (DataTemplate)newValue;
        }


        public static readonly BindableProperty TabsItemTemplateProperty = BindableProperty.Create(nameof(TabsItemTemplate), typeof(DataTemplate), typeof(TabView), null, propertyChanged: OnTabsItemTemplatePropertyChanged);
        public DataTemplate TabsItemTemplate
        {
            get { return (DataTemplate)GetValue(TabsItemTemplateProperty); }
            set { SetValue(TabsItemTemplateProperty, value); }
        }
        private static void OnTabsItemTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            BindableLayout.SetItemTemplate((bindable as TabView).tabsContent, (DataTemplate)newValue);
        }


        public static readonly BindableProperty TabTextColorProperty = BindableProperty.Create(nameof(TabTextColor), typeof(Color), typeof(TabView), Colors.Gray);
        public Color TabTextColor
        {
            get { return (Color)GetValue(TabTextColorProperty); }
            set { SetValue(TabTextColorProperty, value); }
        }

        public static readonly BindableProperty SliderColorProperty = BindableProperty.Create(nameof(SliderColor), typeof(Color), typeof(TabView), Colors.Gray);
        public Color SliderColor
        {
            get { return (Color)GetValue(SliderColorProperty); }
            set { SetValue(SliderColorProperty, value); }
        }


        public static readonly BindableProperty SeparatorColorProperty = BindableProperty.Create(nameof(SeparatorColor), typeof(Color), typeof(TabView), Colors.White);
        public Color SeparatorColor
        {
            get { return (Color)GetValue(SeparatorColorProperty); }
            set { SetValue(SeparatorColorProperty, value); }
        }

        public static readonly BindableProperty TabsItemBindingPathProperty = BindableProperty.Create(nameof(TabsItemBindingPath), typeof(string), typeof(TabView), ".");
        public string TabsItemBindingPath
        {
            get { return (string)GetValue(TabsItemBindingPathProperty); }
            set { SetValue(TabsItemBindingPathProperty, value); }
        }

        public static readonly BindableProperty ContentItemBindingPathProperty = BindableProperty.Create(nameof(ContentItemBindingPath), typeof(string), typeof(TabView), ".");
        public string ContentItemBindingPath
        {
            get { return (string)GetValue(ContentItemBindingPathProperty); }
            set { SetValue(ContentItemBindingPathProperty, value); }
        }


        private ScrollView tabsHolder;
        private Grid tabsContent;
        private BoxView slider;
        private BoxView separator;
        private ViewPager viewPager;
        private ICommand TapCommand;
        private VisualElement previousSelected = null;
        private int previousIndex = 0;
        private double scrollRatio = 0;
        private bool IsAutoScroll = false;
        private int sign = 0;
        private double translateX = 0;
        private bool fixIOSScroll = false;
        private ItemsViewScrolledEventArgs itemsViewScrolledEventArgs;


        public TabView()
        {
            SetMainGrid();
            SetTabs();
            SetSlider();
            SetSeparator();
            SetViewPager();

            // Add to main grid
            this.Add(tabsHolder, 0, 0);
            this.Add(separator, 0, 1);
            this.Add(slider, 0, 1);
            this.Add(viewPager, 0, 2);

            TapCommand = new Command<View>(Tap);
        }

        private void SetMainGrid()
        {
            // Main Grid
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        private void SetTabs()
        {
            // Tabs scrollView
            tabsHolder = new ScrollView()
            {
                Orientation = ScrollOrientation.Horizontal,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
            };

            // Disable bounce on ios
            Microsoft.Maui.Handlers.ScrollViewHandler.Mapper.AppendToMapping("Disable_Bounce", (handler, view) =>
            {
#if IOS
            if(view == tabsHolder)
                handler.PlatformView.Bounces = false;
#endif
            });

            // tabs content not recycled
            tabsContent = new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
            };

            //tabsContent.SetBinding(Grid.MinimumWidthRequestProperty, new Binding("Width", source: tabsHolder));
            TabsItemTemplate = SetDefaultTabsItemTemplate();

            // Add content and slider to grid
            tabsHolder.Content = tabsContent;
            tabsHolder.Scrolled += TabsHolder_Scrolled;
            tabsContent.ChildAdded += TabsContent_ChildAdded;
            tabsContent.ChildRemoved += TabsContent_ChildRemoved;
            tabsHolder.MeasureInvalidated += TabsHolder_MeasureInvalidated;
        }

        private void SetSlider()
        {
            // Slider
            slider = new BoxView()
            {
                HeightRequest = 2,
                WidthRequest = 0,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };

            Binding binding = new Binding(nameof(SliderColor), source: this);
            slider.SetBinding(BoxView.ColorProperty, binding);
        }

        private void SetSeparator()
        {
            separator = new BoxView()
            {
                HeightRequest = 1,
                Margin = new Thickness(0, 2, 0, 12),
                BackgroundColor = SeparatorColor,
                IsVisible = false,
                VerticalOptions = LayoutOptions.Start
            };

            Binding binding = new Binding("SeparatorColor", source: this);
            separator.SetBinding(BoxView.BackgroundColorProperty, binding);
        }

        private void SetViewPager()
        {
            viewPager = new ViewPager();
            viewPager.SetBinding(ViewPager.ItemBindingPathProperty, new Binding(nameof(TabView.ContentItemBindingPath), source: this));
            viewPager.Scrolled += ViewPager_Scrolled;
            viewPager.CurrentIndexChanged += ViewPager_CurrentIndexChanged;
        }

        private void SetTabsContentGridColumnDefinitions(View view)
        {
            tabsContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        }

        private void TabsHolder_MeasureInvalidated(object sender, EventArgs e)
        {
            if (!tabsContent.Any())
                return;

            var item = sender as IView;
            var position = viewPager.CurrentIndex < tabsContent.Count ? viewPager.CurrentIndex : tabsContent.Count - 1;
            slider.WidthRequest = tabsContent.ElementAt(position).Measure(double.PositiveInfinity, double.PositiveInfinity).Width;
        }

        private void TabsContent_ChildAdded(object sender, ElementEventArgs e)
        {
            if (!separator.IsVisible)
                separator.IsVisible = true;

            var pos = (sender as Grid).Children.IndexOf(e.Element as View);
            var item = e.Element as View;
            item.GestureRecognizers.Add(new TapGestureRecognizer() { Command = TapCommand, CommandParameter = item });
            ReorderTabs(pos, e.Element as View);
            SetTabsContentGridColumnDefinitions(e.Element as View);


            // Fix horizontal offset for android devices when adding new item at certain position and newItemPosition >= currentPosition 
            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                if (pos < tabsContent.Count - 1 && pos <= viewPager.CurrentIndex)
                    viewPager.offsetX += this.viewPager.Width;
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                if (pos < tabsContent.Count - 1 && pos <= viewPager.CurrentIndex)
                    fixIOSScroll = true;
            }


            //// Used to set slider's width at appearing
            item.SizeChanged += Item_SizeChanged;
        }

        private void TabsContent_ChildRemoved(object sender, ElementEventArgs e)
        {
            int position = 0;
            foreach (var item in ItemsSource)
            {
                if (e.Element.BindingContext == item)
                    return;

                position++;
            }

            tabsContent.ColumnDefinitions.RemoveAt(position);

            if (position >= viewPager.CurrentIndex)
            {
                if (viewPager.CurrentIndex > 0)
                {
                    if (position <= viewPager.CurrentIndex)
                    {
                        viewPager.CurrentIndex--;

                        // fix horizontalScrollOffset on Android
                        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
                            viewPager.offsetX -= viewPager.Width;
                    }
                }
            }

            // Set slider Width to 0 if no items
            if (!tabsContent.Any())
            {
                slider.WidthRequest = 0;
                separator.IsVisible = false;
            }

            // Remove event handler
            (e.Element as View).SizeChanged -= Item_SizeChanged;
        }

        private void Item_SizeChanged(object sender, EventArgs e)
        {
            if (fixIOSScroll)
            {
                IView item = sender as IView;
                var position = tabsContent.Children.IndexOf(item);

                if (position == itemsViewScrolledEventArgs.CenterItemIndex)
                {
                    ViewPager_Scrolled(viewPager, itemsViewScrolledEventArgs);
                    fixIOSScroll = false;
                }
            }
        }

        private void ReorderTabs(int position, IView item)
        {
            int columnIndex = position;

            if (position >= tabsContent.Count - 1)
            {
                tabsContent.SetColumn(item, columnIndex);
                return;
            }

            tabsContent.SetColumn(item, columnIndex);

            int updatedPosition = position + 1;
            for (int i = updatedPosition; i < tabsContent.Count; i++)
            {
                tabsContent.SetColumn(tabsContent[i], i);
            }
        }

        private void Tap(View item)
        {
            var position = tabsContent.Children.IndexOf(item);
            viewPager.ScrollTo(position);
        }

        private void TabsHolder_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (!IsAutoScroll)
                slider.TranslationX = translateX - tabsHolder.ScrollX;

            IsAutoScroll = false;
        }

        private DataTemplate SetDefaultTabsItemTemplate()
        {
            return new DataTemplate(() =>
            {
                var label = new Label()
                {
                    Padding = new Thickness(10, 13, 10, 13),
                    HorizontalOptions = LayoutOptions.Fill,
                    HorizontalTextAlignment = TextAlignment.Center,
                };

                label.SetBinding(Label.TextProperty, TabsItemBindingPath);

                Binding binding = new Binding("TabTextColor", source: this);
                label.SetBinding(Label.TextColorProperty, binding);


                // Visual states
                var visualStateGroup = new VisualStateGroup();
                var visualStateSelected = new VisualState() { Name = "selected" };
                var visualStateNormal = new VisualState() { Name = "normal" };
                visualStateSelected.Setters.Add
                (
                    new Setter()
                    {
                        Property = Label.FontAttributesProperty,
                        Value = FontAttributes.Bold
                    }
                );
                visualStateGroup.States.Add(visualStateSelected);
                visualStateNormal.Setters.Add
                (
                    new Setter()
                    {
                        Property = Label.FontAttributesProperty,
                        Value = FontAttributes.None
                    }
                );
                visualStateGroup.States.Add(visualStateNormal);
                VisualStateManager.GetVisualStateGroups(label).Add(visualStateGroup);

                return label;
            });
        }

        private void HighLightItem()
        {
            var item = tabsContent.ElementAt(viewPager.CurrentIndex) as VisualElement;
            SelectedItem = item.BindingContext;

            VisualStateManager.GoToState(item, "selected");

            if (previousSelected != null)
                VisualStateManager.GoToState(previousSelected, "normal");


            previousSelected = item;
        }

        private void ComputeTabItemsWidth()
        {
            var fullWidth = viewPager.Measure(double.PositiveInfinity, double.PositiveInfinity).Request.Width;

            var widthOccupied = tabsContent.Measure(double.PositiveInfinity, double.PositiveInfinity).Request.Width;
            var widthAvailable = fullWidth - widthOccupied;

            if (!tabsContent.Children.Any())
                return;

            foreach (var child in tabsContent.Children)
            {
                var widthRequested = child.Measure(double.PositiveInfinity, double.PositiveInfinity).Width;
                var ratio = widthRequested / widthOccupied;
                var newWidth = widthAvailable * ratio;
                (child as View).WidthRequest = widthRequested + newWidth;
            }

        }

        private void ViewPager_CurrentIndexChanged(object sender, EventArgs e)
        {
            // For tab item styling purpose
            if (previousIndex != viewPager.CurrentIndex)
            {
                HighLightItem();
                previousIndex = viewPager.CurrentIndex;
            }
        }

        private void ViewPager_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (!tabsContent.Children.Any())
                return;

            itemsViewScrolledEventArgs = e;
            var currentIndex = viewPager.CurrentIndex;
            sign = Math.Sign(e.HorizontalOffset + viewPager.offsetX - viewPager.Width * currentIndex);
            var currentItem = tabsContent.Children.ElementAt(currentIndex) as View;
            View nextItem = tabsContent.Children.Count > (currentIndex + sign) && (currentIndex + sign) >= 0 ? tabsContent.Children.ElementAt(currentIndex + sign) as View : null;

            // scrollRatio
            scrollRatio = (e.HorizontalOffset + viewPager.offsetX - viewPager.Width * currentIndex) / viewPager.Width;

            double currentItemWidth = currentItem.Width < 0 ? currentItem.Measure(double.PositiveInfinity, double.PositiveInfinity).Request.Width : currentItem.Width;
            double nextItemWidth = nextItem != null ? nextItem.Width : currentItem.Width;

            if (sign > 0)
                translateX = currentItem.Bounds.X + scrollRatio * currentItemWidth;
            else
                translateX = currentItem.Bounds.X + scrollRatio * nextItemWidth;


            double maxScrollX = tabsHolder.ContentSize.Width - tabsHolder.Width;
            var tx = sign > 0 ? scrollRatio * currentItemWidth : scrollRatio * nextItemWidth;
            double toScrollX = 0;

            if (currentIndex == 0)
                toScrollX = currentItem.Bounds.X + tx / 2;
            else if (currentIndex > 0)
                toScrollX = currentItem.Bounds.X + tx - (tabsContent.Children.ElementAt(0) as View).Bounds.Width / 2;
            else
                toScrollX = currentItem.Bounds.X + tx;



            if (toScrollX < 0)
                tabsHolder.ScrollToAsync(0, 0, false);
            else if (toScrollX > maxScrollX && sign > 0)
                tabsHolder.ScrollToAsync(maxScrollX, 0, false);
            else if (maxScrollX > 0 && toScrollX < maxScrollX)
                tabsHolder.ScrollToAsync(toScrollX, 0, false);


            //Set slider width
            slider.WidthRequest = currentItemWidth - (currentItemWidth - nextItemWidth) * Math.Abs(scrollRatio);

            //Set slider X position
            slider.TranslationX = translateX - tabsHolder.ScrollX;
        }

        public void Dispose()
        {
            tabsHolder.Scrolled -= TabsHolder_Scrolled;
            tabsHolder.MeasureInvalidated -= TabsHolder_MeasureInvalidated;
            tabsContent.ChildAdded -= TabsContent_ChildAdded;
            tabsContent.ChildRemoved -= TabsContent_ChildRemoved;
            viewPager.Scrolled -= ViewPager_Scrolled;
            viewPager.CurrentIndexChanged -= ViewPager_CurrentIndexChanged;

            foreach (View item in tabsContent.Children)
                item.SizeChanged -= Item_SizeChanged;
        }
    }

}