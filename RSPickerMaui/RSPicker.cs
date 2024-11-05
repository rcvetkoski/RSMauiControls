using RSInputViewMaui;
using System.Reflection;
using RSPopupMaui;
using System.Collections;

namespace RSPickerMaui
{
    // All the code in this file is included in all platforms.
    public class RSPicker : RSInputView
    {
        private RSCollectionView CollectionView { get; set; }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(RSPicker), null, propertyChanged: ItemsSourceChange);
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void ItemsSourceChange(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSPicker rsPicker = bindable as RSPicker;
            rsPicker.CollectionView.ItemsSource = rsPicker.ItemsSource;
        }

        /// <summary>
        /// In XAML : EnumType="{x:Type enums:DaysOfWeekEnum}"
        /// </summary>
        public static readonly BindableProperty EnumTypeProperty = BindableProperty.Create(nameof(EnumType), typeof(Type), typeof(RSPicker), null, propertyChanged: OnEnumTypeChanged);

        public Type EnumType
        {
            get => (Type)GetValue(EnumTypeProperty);
            set => SetValue(EnumTypeProperty, value);
        }

        private static void OnEnumTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is RSPicker rsPicker && newValue is Type enumType && enumType.IsEnum)
            {
                var list = EnumHelper.GetEnumValues(enumType);
                rsPicker.ItemsSource = list.ToList();
            }
        }



        public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(nameof(SelectedItems), typeof(IList), typeof(RSPicker), null, propertyChanged:SelectedItemsChange);
        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        private static void SelectedItemsChange(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSPicker rsPicker = bindable as RSPicker;
            rsPicker.CollectionView.SelectedItems = rsPicker.SelectedItems;
        }


        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(RSPicker), null, propertyChanged:SelectedItemChanged);
        private static void SelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSPicker rsPicker = (bindable as RSPicker);
            rsPicker.PickerText.Text += GetPropValue(rsPicker.SelectedItem, rsPicker.DisplayMemberPath);
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly BindableProperty DisplayMemberPathProperty = BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(RSPicker), null, propertyChanged: DisplayMemberPathChanged);
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
        private static void DisplayMemberPathChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSPicker rsPicker = (bindable as RSPicker);
            rsPicker.CollectionView.DisplayMemberPath = (string)newValue;
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
            CollectionView = new RSCollectionView()
            {
                SelectionMode = SelectionMode.Multiple
            };
            CollectionView.SelectionChanged += CollectionView_SelectionChanged;
            tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            Content.GestureRecognizers.Add(tapGestureRecognizer);
        }

        protected override void ClearText()
        {
            base.ClearText();

            CollectionView.SelectedItem = null;
            CollectionView.ClearSelectedItems();
        }


        private void CollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // Update picker text here
            PickerText.Text = string.Empty;

            int i = 0;
            foreach (var item in e.CurrentSelection)
            {
                PickerText.Text += GetPropValue((item as RSItem).Item, DisplayMemberPath);

                if (i < e.CurrentSelection.Count - 1)
                    PickerText.Text += ", ";

                i++;
            }
        }

        private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
        {
            Grid parent = CollectionView.Parent as Grid;

            if(parent != null)
                parent.Remove(CollectionView);


            RSpopupManager.ShowPopup(BuildPopup());
        }

        private IView BuildPopup()
        {
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
                Text = "Cancel", 
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

        public event EventHandler? CloseButtonPressed;

        public static string GetPropValue(object src, string propName)
        {
            if(src is null)
                return string.Empty;

            if (string.IsNullOrEmpty(propName))
                return src.ToString();

            var val = src.GetType().GetRuntimeProperty(propName).GetValue(src, null);
            return val != null ? val.ToString() : string.Empty;
        }

        public override void Dispose()
        {
            base.Dispose();

            CollectionView.SelectionChanged -= CollectionView_SelectionChanged;
            tapGestureRecognizer.Tapped -= TapGestureRecognizer_Tapped;
        }
    }
}
