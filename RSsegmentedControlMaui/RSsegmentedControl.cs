using Microsoft.Maui.Controls.Shapes;
using System.Collections;

namespace RSsegmentedControlMaui
{
    public enum SegmentedControlStyle
    {
        Underline,
        Outlined
    }

    //public enum SelectionModeEnum
    //{
    //    NonMandatory,
    //    Mandatory
    //}

    public enum SegmentedControlLayoutMode
    {
        Fill,
        Auto
    }

    public class SegmentedControl : ContentView
    {
        private readonly Grid _grid;
        private readonly BoxView _indicator;
        private readonly Border _border;

        private bool _isLoaded;

        public SegmentedControl()
        {
            HorizontalOptions = LayoutOptions.Start;
            VerticalOptions = LayoutOptions.Start;

            _grid = new Grid { ColumnSpacing = 0 };

            _indicator = new BoxView
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start
            };

            _border = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 0 },
                StrokeThickness = 0,
                Padding = 0,
                Content = _grid
            };

            _border.SetBinding(Border.BackgroundProperty, new Binding(nameof(FillColor), source: this));
            _border.SetBinding(Border.PaddingProperty, new Binding(nameof(IndicatorPadding), source: this));

            Content = _border;
        }

        private bool _isSyncingSelection;

        // =========================
        // ItemsSource
        // =========================
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(SegmentedControl),
                propertyChanged: (b, o, n) =>
                {
                    var c = (SegmentedControl)b;
                    c.SyncSelectedItemFromIndex(c.SelectedIndex);
                    c.BuildSegments();
                });

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        // =========================
        // ItemTemplate
        // =========================
        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(SegmentedControl),
                propertyChanged: (b, o, n) => ((SegmentedControl)b).BuildSegments());

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        // =========================
        // SelectionMode
        // =========================
        public static readonly BindableProperty SelectionModeProperty =
            BindableProperty.Create(nameof(SelectionMode), typeof(SelectionModeEnum), typeof(SegmentedControl), SelectionModeEnum.Mandatory);

        public SelectionModeEnum SelectionMode
        {
            get => (SelectionModeEnum)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }

        // =========================
        // SelectedItem
        // =========================
        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SegmentedControl), null,
                BindingMode.TwoWay,
                propertyChanged: (b, o, n) =>
                {
                    var c = (SegmentedControl)b;
                    c.SyncSelectedIndexFromItem(n);
                });

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        // =========================
        // SelectedIndex
        // =========================
        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(SegmentedControl), 0,
                BindingMode.TwoWay,
                propertyChanged: (b, o, n) =>
                {
                    var c = (SegmentedControl)b;
                    c.SyncSelectedItemFromIndex((int)n);
                    c.OnSelectedIndexChanged();
                });

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        private void SyncSelectedItemFromIndex(int index)
        {
            if (_isSyncingSelection) return;
            _isSyncingSelection = true;
            try
            {
                if (index < 0 || ItemsSource == null)
                    SelectedItem = null;
                else
                {
                    var items = ItemsSource.Cast<object>().ToList();
                    SelectedItem = index < items.Count ? items[index] : null;
                }
            }
            finally
            {
                _isSyncingSelection = false;
            }
        }

        private void SyncSelectedIndexFromItem(object item)
        {
            if (_isSyncingSelection) return;
            _isSyncingSelection = true;
            try
            {
                if (item == null || ItemsSource == null)
                    SelectedIndex = -1;
                else
                {
                    var items = ItemsSource.Cast<object>().ToList();
                    SelectedIndex = items.IndexOf(item);
                }
            }
            finally
            {
                _isSyncingSelection = false;
            }
        }

        private async void OnSelectedIndexChanged()
        {
            if (SelectedIndex < 0)
            {
                // Fade out indicator when unselected
                _ = _indicator.FadeTo(0, 150, Easing.CubicInOut);
            }
            else
            {
                if (_indicator.Opacity < 1)
                    _ = _indicator.FadeTo(1, 150, Easing.CubicInOut);

                await MoveIndicator(true);
            }

            UpdateVisualState();
        }

        // =========================
        // StyleMode
        // =========================
        public static readonly BindableProperty StyleModeProperty =
            BindableProperty.Create(nameof(StyleMode), typeof(SegmentedControlStyle), typeof(SegmentedControl),
                SegmentedControlStyle.Underline,
                propertyChanged: (b, o, n) => ((SegmentedControl)b).ApplyStyle());

        public SegmentedControlStyle StyleMode
        {
            get => (SegmentedControlStyle)GetValue(StyleModeProperty);
            set => SetValue(StyleModeProperty, value);
        }

        // =========================
        // LayoutMode
        // =========================
        public static readonly BindableProperty LayoutModeProperty =
            BindableProperty.Create(nameof(LayoutMode), typeof(SegmentedControlLayoutMode), typeof(SegmentedControl),
                SegmentedControlLayoutMode.Fill,
                propertyChanged: (b, o, n) => ((SegmentedControl)b).BuildSegments());

        public SegmentedControlLayoutMode LayoutMode
        {
            get => (SegmentedControlLayoutMode)GetValue(LayoutModeProperty);
            set => SetValue(LayoutModeProperty, value);
        }

        // =========================
        // Orientation
        // =========================
        public static readonly BindableProperty OrientationProperty =
            BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(SegmentedControl),
                StackOrientation.Horizontal,
                propertyChanged: (b, o, n) => ((SegmentedControl)b).BuildSegments());

        public StackOrientation Orientation
        {
            get => (StackOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        // =========================
        // Styling properties
        // =========================
        public static readonly BindableProperty SelectedTextColorProperty =
            BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(SegmentedControl), Colors.White,
                propertyChanged: (b, o, n) => ((SegmentedControl)b).UpdateVisualState());

        public Color SelectedTextColor
        {
            get => (Color)GetValue(SelectedTextColorProperty);
            set => SetValue(SelectedTextColorProperty, value);
        }

        public static readonly BindableProperty UnselectedTextColorProperty =
            BindableProperty.Create(nameof(UnselectedTextColor), typeof(Color), typeof(SegmentedControl), Colors.Gray,
                propertyChanged: (b, o, n) => ((SegmentedControl)b).UpdateVisualState());

        public Color UnselectedTextColor
        {
            get => (Color)GetValue(UnselectedTextColorProperty);
            set => SetValue(UnselectedTextColorProperty, value);
        }

        public static readonly BindableProperty SelectedBackgroundColorProperty =
            BindableProperty.Create(nameof(SelectedBackgroundColor), typeof(Color), typeof(SegmentedControl), Colors.SteelBlue,
                propertyChanged: (b, o, n) =>
                {
                    var c = (SegmentedControl)b;
                    if (c.StyleMode == SegmentedControlStyle.Outlined)
                        c._indicator.BackgroundColor = (Color)n;
                });

        public Color SelectedBackgroundColor
        {
            get => (Color)GetValue(SelectedBackgroundColorProperty);
            set => SetValue(SelectedBackgroundColorProperty, value);
        }

        public static readonly BindableProperty IndicatorColorProperty =
            BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(SegmentedControl), Colors.SteelBlue,
                propertyChanged: (b, o, n) =>
                {
                    var c = (SegmentedControl)b;
                    if (c.StyleMode == SegmentedControlStyle.Underline)
                        c._indicator.BackgroundColor = (Color)n;
                });

        public Color IndicatorColor
        {
            get => (Color)GetValue(IndicatorColorProperty);
            set => SetValue(IndicatorColorProperty, value);
        }

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(SegmentedControl), Colors.LightGray,
                propertyChanged: (b, o, n) => ((SegmentedControl)b).ApplyStyle());

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public static readonly BindableProperty SeparatorColorProperty =
            BindableProperty.Create(nameof(SeparatorColor), typeof(Color), typeof(SegmentedControl), Colors.LightGray);

        public Color SeparatorColor
        {
            get => (Color)GetValue(SeparatorColorProperty);
            set => SetValue(SeparatorColorProperty, value);
        }

        public static readonly BindableProperty SeparatorThicknessProperty =
            BindableProperty.Create(nameof(SeparatorThickness), typeof(double), typeof(SegmentedControl), 1.0);

        public double SeparatorThickness
        {
            get => (double)GetValue(SeparatorThicknessProperty);
            set => SetValue(SeparatorThicknessProperty, value);
        }

        public static readonly BindableProperty SegmentBackgroundColorProperty =
            BindableProperty.Create(nameof(SegmentBackgroundColor), typeof(Color), typeof(SegmentedControl), Colors.Transparent,
                propertyChanged: (b, o, n) =>
                {
                    // Update current background color if already loaded
                    // For thorough application, rebuilding segments is safer
                    ((SegmentedControl)b).BuildSegments();
                });

        public Color SegmentBackgroundColor
        {
            get => (Color)GetValue(SegmentBackgroundColorProperty);
            set => SetValue(SegmentBackgroundColorProperty, value);
        }

        [System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
        public static readonly BindableProperty TextSizeProperty =
            BindableProperty.Create(nameof(TextSize), typeof(double), typeof(SegmentedControl), 14.0d,
                propertyChanged: (b, o, n) => ((SegmentedControl)b).BuildSegments());

        [System.ComponentModel.TypeConverter(typeof(FontSizeConverter))]
        public double TextSize
        {
            get => (double)GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }

        // =========================
        // SegmentHeight
        // =========================
        public static readonly BindableProperty SegmentHeightProperty =
            BindableProperty.Create(nameof(SegmentHeight), typeof(double), typeof(SegmentedControl), -1.0,
                propertyChanged: (b, o, n) => ((SegmentedControl)b).BuildSegments());

        public double SegmentHeight
        {
            get => (double)GetValue(SegmentHeightProperty);
            set => SetValue(SegmentHeightProperty, value);
        }

        // =========================
        // FillColor
        // =========================
        public static readonly BindableProperty FillColorProperty =
            BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(SegmentedControl), Colors.Transparent);

        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        // =========================
        // CornerRadius
        // =========================
        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(SegmentedControl), new CornerRadius(8),
                propertyChanged: (b, o, n) => ((SegmentedControl)b).ApplyStyle());

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        // =========================
        // IndicatorPadding
        // =========================
        public static readonly BindableProperty IndicatorPaddingProperty =
            BindableProperty.Create(nameof(IndicatorPadding), typeof(Thickness), typeof(SegmentedControl), new Thickness(0),
                propertyChanged: (b, o, n) => ((SegmentedControl)b).BuildSegments());

        public Thickness IndicatorPadding
        {
            get => (Thickness)GetValue(IndicatorPaddingProperty);
            set => SetValue(IndicatorPaddingProperty, value);
        }

        // =========================
        // Build UI
        // =========================
        private void BuildSegments()
        {
            _isLoaded = false;

            _grid.Children.Clear();
            _grid.ColumnDefinitions.Clear();
            _grid.RowDefinitions.Clear();

            if (ItemsSource == null)
                return;

            var items = ItemsSource.Cast<object>().ToList();

            // =========================
            // LayoutMode handling (FIX)
            // =========================
            var isFill = LayoutMode == SegmentedControlLayoutMode.Fill;
            var layoutOption = isFill ? LayoutOptions.Fill : LayoutOptions.Start;

            if (Orientation == StackOrientation.Horizontal)
            {
                this.HorizontalOptions = layoutOption;
                this.VerticalOptions = LayoutOptions.Start;

                _grid.HorizontalOptions = LayoutOptions.Fill;
                _border.HorizontalOptions = LayoutOptions.Fill;
                _grid.VerticalOptions = LayoutOptions.Fill;
                _border.VerticalOptions = LayoutOptions.Fill;
            }
            else
            {
                this.VerticalOptions = layoutOption;
                this.HorizontalOptions = LayoutOptions.Start;

                _grid.HorizontalOptions = LayoutOptions.Fill;
                _border.HorizontalOptions = LayoutOptions.Fill;
                _grid.VerticalOptions = LayoutOptions.Fill;
                _border.VerticalOptions = LayoutOptions.Fill;
            }

            // =========================
            // Rows / Columns Setup
            // =========================
            if (Orientation == StackOrientation.Horizontal)
            {
                var rowHeight = SegmentHeight > 0 ? new GridLength(SegmentHeight) : GridLength.Auto;
                if (StyleMode == SegmentedControlStyle.Underline)
                {
                    _grid.RowDefinitions.Add(new RowDefinition(rowHeight));
                    _grid.RowDefinitions.Add(new RowDefinition(2));
                }
                else
                {
                    _grid.RowDefinitions.Add(new RowDefinition(rowHeight));
                }
            }
            else
            {
                if (StyleMode == SegmentedControlStyle.Underline)
                {
                    _grid.ColumnDefinitions.Add(new ColumnDefinition(2));
                    _grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                }
                else
                {
                    _grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                }
            }

            // =========================
            // Indicator Setup
            // =========================
            _indicator.TranslationX = 0;
            _indicator.TranslationY = 0;

            if (StyleMode == SegmentedControlStyle.Underline)
            {
                _indicator.Color = IndicatorColor;
                if (Orientation == StackOrientation.Horizontal)
                {
                    _indicator.HeightRequest = 2;
                    _indicator.WidthRequest = -1;
                    _indicator.HorizontalOptions = LayoutOptions.Start;
                    _indicator.VerticalOptions = LayoutOptions.Start;
                }
                else
                {
                    _indicator.WidthRequest = 2;
                    _indicator.HeightRequest = -1;
                    _indicator.HorizontalOptions = LayoutOptions.Start;
                    _indicator.VerticalOptions = LayoutOptions.Start;
                }
            }
            else
            {
                _indicator.Color = SelectedBackgroundColor;
                if (Orientation == StackOrientation.Horizontal)
                {
                    _indicator.HeightRequest = -1;
                    _indicator.WidthRequest = -1;
                    _indicator.HorizontalOptions = LayoutOptions.Start;
                    _indicator.VerticalOptions = LayoutOptions.Fill;
                }
                else
                {
                    _indicator.WidthRequest = -1;
                    _indicator.HeightRequest = -1;
                    _indicator.HorizontalOptions = LayoutOptions.Fill;
                    _indicator.VerticalOptions = LayoutOptions.Start;
                }
            }

            _indicator.Opacity = SelectedIndex < 0 ? 0 : 1;
            _grid.Add(_indicator);

            if (StyleMode == SegmentedControlStyle.Underline)
            {
                if (Orientation == StackOrientation.Horizontal)
                {
                    Grid.SetRow(_indicator, 1);
                    Grid.SetColumn(_indicator, 0);
                    Grid.SetColumnSpan(_indicator, items.Count);
                    Grid.SetRowSpan(_indicator, 1);
                }
                else
                {
                    Grid.SetColumn(_indicator, 0);
                    Grid.SetRow(_indicator, 0);
                    Grid.SetRowSpan(_indicator, items.Count);
                    Grid.SetColumnSpan(_indicator, 1);
                }
            }
            else
            {
                Grid.SetRow(_indicator, 0);
                Grid.SetColumn(_indicator, 0);
                Grid.SetColumnSpan(_indicator, Orientation == StackOrientation.Horizontal ? items.Count : 1);
                Grid.SetRowSpan(_indicator, Orientation == StackOrientation.Vertical ? items.Count : 1);
            }

            // =========================
            // Items
            // =========================
            for (int i = 0; i < items.Count; i++)
            {
                var length = isFill ? GridLength.Star : GridLength.Auto;

                if (Orientation == StackOrientation.Horizontal)
                {
                    _grid.ColumnDefinitions.Add(new ColumnDefinition(length));
                }
                else
                {
                    var rowHeight = SegmentHeight > 0 ? new GridLength(SegmentHeight) : length;
                    _grid.RowDefinitions.Add(new RowDefinition(rowHeight));
                }

                var container = new Grid();
                container.SetBinding(Grid.BackgroundColorProperty, new Binding(nameof(SegmentBackgroundColor), source: this));

                View content;

                if (ItemTemplate != null)
                {
                    content = (View)ItemTemplate.CreateContent();
                    content.BindingContext = items[i];
                }
                else
                {
                    var textVal = items[i]?.ToString();

                    var ghostLabel = new Label
                    {
                        Text = textVal,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        Padding = new Thickness(10, 5),
                        FontAttributes = FontAttributes.Bold,
                        Opacity = 0
                    };
                    ghostLabel.SetBinding(Label.FontSizeProperty, new Binding(nameof(TextSize), source: this));
                    container.Add(ghostLabel);

                    content = new Label
                    {
                        Text = textVal,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        Padding = new Thickness(10, 5)
                    };
                    content.SetBinding(Label.FontSizeProperty, new Binding(nameof(TextSize), source: this));
                }

                int index = i;
                var tap = new TapGestureRecognizer();
                tap.Tapped += (s, e) =>
                {
                    if (SelectionMode == SelectionModeEnum.NonMandatory && SelectedIndex == index)
                        SelectedIndex = -1;
                    else
                        SelectedIndex = index;
                };
                content.GestureRecognizers.Add(tap);

                container.Add(content);

                // Separator (Outlined mode)
                if (StyleMode == SegmentedControlStyle.Outlined && i < items.Count - 1)
                {
                    var separator = new BoxView();

                    if (Orientation == StackOrientation.Horizontal)
                    {
                        separator.HorizontalOptions = LayoutOptions.End;
                        separator.VerticalOptions = LayoutOptions.Fill;
                        separator.SetBinding(WidthRequestProperty,
                            new Binding(nameof(SeparatorThickness), source: this));
                    }
                    else
                    {
                        separator.HorizontalOptions = LayoutOptions.Fill;
                        separator.VerticalOptions = LayoutOptions.End;
                        separator.SetBinding(HeightRequestProperty,
                            new Binding(nameof(SeparatorThickness), source: this));
                    }

                    separator.SetBinding(BoxView.ColorProperty,
                        new Binding(nameof(SeparatorColor), source: this));

                    // Use property to control visibility from VisualState if needed
                    container.Add(separator);
                }

                _grid.Add(container);

                if (Orientation == StackOrientation.Horizontal)
                {
                    Grid.SetColumn(container, i);
                    Grid.SetRow(container, 0);
                }
                else
                {
                    Grid.SetRow(container, i);
                    Grid.SetColumn(container, StyleMode == SegmentedControlStyle.Underline ? 1 : 0);
                }
            }
        }

        // =========================
        // Style
        // =========================
        private void ApplyStyle()
        {
            if (StyleMode == SegmentedControlStyle.Outlined)
            {
                _border.StrokeThickness = 1;
                _border.Stroke = new SolidColorBrush(BorderColor);
                _border.StrokeShape = new RoundRectangle { CornerRadius = CornerRadius };
                _indicator.CornerRadius = CornerRadius;
            }
            else
            {
                _border.StrokeThickness = 0;
                _border.StrokeShape = new RoundRectangle { CornerRadius = CornerRadius };
                _indicator.CornerRadius = 0;
            }

            BuildSegments();
        }

        // =========================
        // Helper
        // =========================
        private Grid GetSelectedContainer()
        {
            int expectedVertCol = StyleMode == SegmentedControlStyle.Underline ? 1 : 0;
            return _grid.Children.OfType<Grid>().FirstOrDefault(v =>
                (Orientation == StackOrientation.Horizontal && Grid.GetRow(v) == 0 && Grid.GetColumn(v) == SelectedIndex) ||
                (Orientation == StackOrientation.Vertical && Grid.GetColumn(v) == expectedVertCol && Grid.GetRow(v) == SelectedIndex)
            );
        }

        // =========================
        // Indicator
        // =========================
        private async Task MoveIndicator(bool animated)
        {
            var selected = GetSelectedContainer();
            if (selected == null)
                return;

            bool isZeroSize = Orientation == StackOrientation.Horizontal ? selected.Width <= 0 : selected.Height <= 0;
            if (isZeroSize)
            {
                void handler(object s, EventArgs e)
                {
                    selected.SizeChanged -= handler;
                    bool nowHasSize = Orientation == StackOrientation.Horizontal ? selected.Width > 0 : selected.Height > 0;
                    if (nowHasSize)
                        _ = MoveIndicator(animated);
                }
                selected.SizeChanged += handler;
                return;
            }

            double targetSize = Orientation == StackOrientation.Horizontal ? selected.Width : selected.Height;
            if (targetSize <= 0)
            {
                var measure = selected.Measure(double.PositiveInfinity, double.PositiveInfinity);
                targetSize = Orientation == StackOrientation.Horizontal ? measure.Width : measure.Height;
            }

            if (!animated)
            {
                if (Orientation == StackOrientation.Horizontal)
                {
                    _indicator.WidthRequest = targetSize;
                    _indicator.TranslationX = selected.X;
                }
                else
                {
                    _indicator.HeightRequest = targetSize;
                    _indicator.TranslationY = selected.Y;
                }
            }
            else
            {
                var tcs = new TaskCompletionSource<bool>();

                if (Orientation == StackOrientation.Horizontal)
                {
                    double currentWidth = _indicator.Width > 0 ? _indicator.Width : (_indicator.WidthRequest > 0 ? _indicator.WidthRequest : targetSize);
                    new Animation(v => _indicator.WidthRequest = v, currentWidth, targetSize)
                        .Commit(this, "IndicatorSizeAnim", 16, 200, Easing.CubicInOut, (v, c) => tcs.TrySetResult(c));

                    await Task.WhenAll(
                        _indicator.TranslateToAsync(selected.X, 0, 200, Easing.CubicInOut),
                        tcs.Task
                    );
                }
                else
                {
                    double currentHeight = _indicator.Height > 0 ? _indicator.Height : (_indicator.HeightRequest > 0 ? _indicator.HeightRequest : targetSize);
                    new Animation(v => _indicator.HeightRequest = v, currentHeight, targetSize)
                        .Commit(this, "IndicatorSizeAnim", 16, 200, Easing.CubicInOut, (v, c) => tcs.TrySetResult(c));

                    await Task.WhenAll(
                        _indicator.TranslateToAsync(0, selected.Y, 200, Easing.CubicInOut),
                        tcs.Task
                    );
                }
            }
        }

        // =========================
        // Visual state
        // =========================
        private void UpdateVisualState()
        {
            foreach (var child in _grid.Children)
            {
                if (child is Grid container)
                {
                    int index = Orientation == StackOrientation.Horizontal ? Grid.GetColumn(container) : Grid.GetRow(container);
                    bool isSelected = index == SelectedIndex;

                    var label = container.Children.OfType<Label>().FirstOrDefault(l => l.Opacity > 0);
                    if (label != null)
                    {
                        label.TextColor = isSelected ? SelectedTextColor : UnselectedTextColor;
                        label.FontAttributes = isSelected ? FontAttributes.Bold : FontAttributes.None;
                    }
                }
            }
        }

        // =========================
        // Layout
        // =========================
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (!_isLoaded && width > 0)
            {
                _isLoaded = true;
                _ = MoveIndicator(false); // Discard task to avoid CS4014 warning
                UpdateVisualState();
            }
        }

        // Use OnHandlerChanging to clean up ANY external event subscriptions 
        // to avoid memory leaks when the control is removed/destroyed.
        protected override void OnHandlerChanging(HandlerChangingEventArgs args)
        {
            base.OnHandlerChanging(args);

            if (args.OldHandler != null)
            {
                // Control is being disconnected (removed from UI).
                // Unsubscribe from anything that isn't `this`.
                // Example:
                // selected.SizeChanged -= handler;
            }
        }
    }
}
