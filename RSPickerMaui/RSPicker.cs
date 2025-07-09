using RSInputViewMaui;
using RSPopupMaui;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace RSPickerMaui
{
    public class RSPicker : RSInputView
    {
        private RSCollectionView CollectionView { get; set; }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(RSPicker), null, propertyChanged: ItemsSourcePropertyChanged);

        private static void ItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null || newValue is null)
                return;

            RSPicker picker = bindable as RSPicker;

            if (picker.ItemsSource == null || (picker.SelectedItems == null && picker.SelectedItem == null))
                return;

            // Set text
            if(picker.SelectionMode == SelectionMode.Multiple)
                picker.SetPickerText(picker.SelectedItems);
            else
                picker.SetPickerText(new List<object>() { picker.SelectedItem });
        }

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// In XAML : EnumType="{x:Type enums:DaysOfWeekEnum}"
        /// </summary>
        public static readonly BindableProperty SelectionModeProperty = BindableProperty.Create(nameof(SelectionMode), typeof(SelectionMode), typeof(RSPicker), default);

        public SelectionMode SelectionMode
        {
            get => (SelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }


        public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(nameof(SelectedItems), typeof(IList), typeof(RSPicker), null, propertyChanged: SelectedItemsPropertyChanged);

        private static void SelectedItemsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null || newValue is null)
                return;

            RSPicker picker = bindable as RSPicker;

            if (picker.SelectedItems == null || picker.SelectionMode != SelectionMode.Multiple)
                return;

            // Set text
            picker.SetPickerText(picker.SelectedItems);
        }

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }


        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(RSPicker), null, BindingMode.TwoWay, propertyChanged: SelectedItemPropertyChanged);

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        private static void SelectedItemPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null || newValue is null)
                return;

            RSPicker picker = bindable as RSPicker;

            if (picker.SelectedItem == null || picker.SelectionMode == SelectionMode.Multiple)
                return;

            // Set text
            picker.SetPickerText(new List<object>() { picker.SelectedItem });
        }

        public static readonly BindableProperty ConverterProperty = BindableProperty.Create(nameof(Converter), typeof(IValueConverter), typeof(RSPicker), default(IValueConverter));


        public IValueConverter Converter
        {
            get => (IValueConverter)GetValue(ConverterProperty);
            set => SetValue(ConverterProperty, value);
        }


        public static readonly BindableProperty DisplayMemberPathProperty = BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(RSPicker), null);
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }


        private Label PickerText;
        private TapGestureRecognizer tapGestureRecognizer;


        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(RSPicker), new Label().FontSize, propertyChanged: FontSizeChanged);
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        private static void FontSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSPicker rsPicker = bindable as RSPicker;
            rsPicker.PickerText.FontSize = (double)newValue;
            rsPicker.Graphics.Invalidate();
        }

        public RSPicker()
        {
            PickerText = new Label() { Padding = new Thickness(5, 12.35, 5, 12.35), FontSize = FontSize, LineBreakMode = LineBreakMode.TailTruncation };
            Content = PickerText;
            HasDropDownIcon = true;
            tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            Content.GestureRecognizers.Add(tapGestureRecognizer);
        }

        protected override void ClearText()
        {
            base.ClearText();

            if(SelectionMode == SelectionMode.Multiple)
                SelectedItems?.Clear();
            else
                SelectedItem = null;
        }

        private void CollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // Update picker text here
            SetPickerText(e.CurrentSelection);
        }

        private void SetPickerText(IList list)
        {
            if (list == null)
                return;

            PickerText.Text = string.Empty;

            int i = 0;
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(DisplayMemberPath))
                {
                    if (Converter != null)
                        PickerText.Text += Converter.Convert(GetPropValue(item, DisplayMemberPath), targetType: typeof(object), parameter: null, culture: CultureInfo.CurrentCulture);
                    else
                        PickerText.Text += GetPropValue(item, DisplayMemberPath);
                }
                else
                {
                    if(Converter != null)
                        PickerText.Text += Converter.Convert(item, targetType: typeof(object), parameter: null, culture: CultureInfo.CurrentCulture);
                    else
                        PickerText.Text += item.ToString();
                }


                if (i < list.Count - 1)
                    PickerText.Text += ", ";

                i++;
            }
        }

        private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
        {
            RSpopupManager.ShowPopup(BuildPopup());
        }

        private IView BuildPopup()
        {
            if(CollectionView != null)
            {
                Grid parent = CollectionView.Parent as Grid;

                if (parent != null)
                    parent.Remove(CollectionView);
            }

            CollectionView = new RSCollectionView
            {
                SelectionMode = SelectionMode,
                VerticalScrollBarVisibility = ScrollBarVisibility.Never
            };

            CollectionView.SetBinding(RSCollectionView.SelectedItemsProperty, new Binding("SelectedItems") { Source = this });
            CollectionView.SetBinding(RSCollectionView.SelectedItemProperty, new Binding("SelectedItem", mode: BindingMode.TwoWay) { Source = this });
            CollectionView.SetBinding(RSCollectionView.ItemsSourceProperty, new Binding("ItemsSource") { Source = this });
            CollectionView.SetBinding(RSCollectionView.ConverterProperty, new Binding("Converter") { Source = this });
            CollectionView.DisplayMemberPath = this.DisplayMemberPath;

            CollectionView.SelectionChanged -= CollectionView_SelectionChanged;
            CollectionView.SelectionChanged += CollectionView_SelectionChanged;

            Grid grid = new Grid()
            {
                RowDefinitions = 
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto)
                }
            };
            Button button = new Button() 
            { 
                Text = "Done", 
                HorizontalOptions = LayoutOptions.End, 
                VerticalOptions = LayoutOptions.Center,
                Command = new Command(async ()=>
                {
                    CloseButtonPressed?.Invoke(this, EventArgs.Empty);
                    await RSpopupManager.ClosePopup();
                })
            };

            grid.Add(CollectionView, 0, 0);
            grid.Add(button, 0, 1);

            return grid;
        }

        private void CollectionView_SelectionChanged1(object? sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public event EventHandler? CloseButtonPressed;

        public static string GetPropValue(object src, string propName)
        {
            if(src is null)
                return string.Empty;

            if (string.IsNullOrEmpty(propName))
                return src.ToString();

            object val = null;
            var pty = src.GetType().GetRuntimeProperty(propName);
            if( pty != null )
                val = pty.GetValue(src, null);

            return val != null ? val.ToString() : string.Empty;
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler == null)
            {
                if(CollectionView != null)
                    CollectionView.SelectionChanged -= CollectionView_SelectionChanged;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (CollectionView != null)
                CollectionView.SelectionChanged -= CollectionView_SelectionChanged;
            
            if(tapGestureRecognizer != null)
                tapGestureRecognizer.Tapped -= TapGestureRecognizer_Tapped;
        }
    }
}
