using System.Collections;

namespace RSFixedSizePagerMaui
{
    public class RSPagerIndicatorView : Grid, IDisposable
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(RSPagerIndicatorView), null, propertyChanged: OnPropertyChanged);
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null && newValue == null)
                return;

            RSPagerIndicatorView grid = bindable as RSPagerIndicatorView;

            if (grid.TabsItemTemplate != null)
                grid.InitTabs(newValue as IEnumerable, grid.TabsItemTemplate);
            else
                grid.InitTabs(newValue as IEnumerable, grid.SetDefaultTabsItemTemplate());
        }


        public static readonly BindableProperty TabsItemTemplateProperty = BindableProperty.Create(nameof(RSPagerIndicatorView), typeof(DataTemplate), typeof(RSFixedSizePagerView), null, propertyChanged: OnTabsItemTemplatePropertyChanged);
        public DataTemplate TabsItemTemplate
        {
            get { return (DataTemplate)GetValue(TabsItemTemplateProperty); }
            set { SetValue(TabsItemTemplateProperty, value); }
        }
        private static void OnTabsItemTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSPagerIndicatorView grid = bindable as RSPagerIndicatorView;
            if (grid.ItemsSource == null)
                return;


            if(newValue != null)
                grid.InitTabs(grid.ItemsSource, newValue as DataTemplate);
            else
                grid.InitTabs(grid.ItemsSource, grid.SetDefaultTabsItemTemplate());
        }



        public static readonly BindableProperty TabsItemBindingPathProperty = BindableProperty.Create(nameof(RSPagerIndicatorView), typeof(string), typeof(RSFixedSizePagerView), ".");
        public string TabsItemBindingPath
        {
            get { return (string)GetValue(TabsItemBindingPathProperty); }
            set { SetValue(TabsItemBindingPathProperty, value); }
        }


        public static readonly BindableProperty TabTextColorProperty = BindableProperty.Create(nameof(TabTextColor), typeof(Color), typeof(RSPagerIndicatorView), default);
        public Color TabTextColor
        {
            get { return (Color)GetValue(TabTextColorProperty); }
            set { SetValue(TabTextColorProperty, value); }
        }

        public static readonly BindableProperty TabFontSizeProperty = BindableProperty.Create(nameof(TabFontSize), typeof(double), typeof(RSPagerIndicatorView), (double)16);
        public double TabFontSize
        {
            get { return (double)GetValue(TabFontSizeProperty); }
            set { SetValue(TabFontSizeProperty, value); }
        }


        private void InitTabs(IEnumerable itemsSource, DataTemplate dataTemplate)
        {
            if (this.Children.Any())
            {
                this.ColumnDefinitions.Clear();
                this.Children.Clear();  
            }


            if (itemsSource is IEnumerable newItems)
            {
                int columnIndex = 0;

                foreach (var item in newItems)
                {
                    var view = dataTemplate.CreateContent() as View;
                    view.BindingContext = item;
                    TapGestureRecognizer = new TapGestureRecognizer()
                    {
                        Command = new Command<int>(TabItemClick),
                        CommandParameter = columnIndex
                    };
                    view.GestureRecognizers.Add(TapGestureRecognizer);

                    this.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
                    this.Add(view, columnIndex, 0);
                    columnIndex++;
                }

                this.Add(this.Slider, 0, 0);
                this.SetColumnSpan(this.Slider, columnIndex);
            }
        }

        public RSFixedSizePagerView RSFixedSizePager { get;set; }   
        private void TabItemClick(int position)
        {
            if (RSFixedSizePager == null)
                return;

            ApplyRippleEffect(this.Children.ElementAt(position) as View);
            RSFixedSizePager.ScrollTo(position);    
        }
        private TapGestureRecognizer TapGestureRecognizer { get; set; }
        private async void ApplyRippleEffect(View targetView)
        {
            if (targetView == null)
                return;

            // Store the original scale of the view
            var originalScale = targetView.Scale;

            // Apply a "press" effect by shrinking the view slightly
            await targetView.ScaleTo(0.9, 100, Easing.CubicOut);

            // Restore the view to its original size
            await targetView.ScaleTo(originalScale, 100, Easing.CubicIn);
        }

        private DataTemplate SetDefaultTabsItemTemplate()
        {
            return new DataTemplate(() =>
            {
                var label = new Label()
                {
                    Padding = new Thickness(0, 10),
                    HorizontalOptions = LayoutOptions.Fill,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                label.SetBinding(Label.TextProperty, TabsItemBindingPath);

                Binding TabTextColorBinding = new Binding("TabTextColor", source: this);
                label.SetBinding(Label.TextColorProperty, TabTextColorBinding);

                Binding TabFontSizeBinding = new Binding("TabFontSize", source: this);
                label.SetBinding(Label.FontSizeProperty, TabFontSizeBinding);


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

        public BoxView Slider { get; set; } 

        public RSPagerIndicatorView()
        {
            this.AddRowDefinition(new RowDefinition(GridLength.Auto));

            Slider = new BoxView()
            {
                HeightRequest = 2,
                Color = Colors.White,
                HorizontalOptions = LayoutOptions.Start,  
                VerticalOptions = LayoutOptions.End
            };
        }

        public void Dispose()
        {
            
        }
    }
}
