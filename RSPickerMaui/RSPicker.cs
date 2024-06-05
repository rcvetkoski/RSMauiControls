using RSInputViewMaui;
using System.Reflection;
using RSPopupMaui;

namespace RSPickerMaui
{
    // All the code in this file is included in all platforms.
    public class RSPicker<T> : RSInputView
    {
        private RSCollectionView<T> CollectionView { get; set; }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(ICollection<T>), typeof(RSPicker<T>), null, propertyChanged: ItemsSourceChange);
        public ICollection<T> ItemsSource
        {
            get { return (ICollection<T>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void ItemsSourceChange(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSPicker<T> rsPicker = bindable as RSPicker<T>;
            rsPicker.CollectionView.ItemsSource = rsPicker.ItemsSource;
        }



        public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(nameof(SelectedItems), typeof(ICollection<T>), typeof(RSPicker<T>), null, propertyChanged:SelectedItemsChange);
        public ICollection<T> SelectedItems
        {
            get { return (ICollection<T>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        private static void SelectedItemsChange(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSPicker<T> rsPicker = bindable as RSPicker<T>;
            rsPicker.CollectionView.SelectedItems = rsPicker.SelectedItems;
        }


        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(RSPicker<T>), null, propertyChanged:SelectedItemChanged);
        private static void SelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSPicker<T> rsPicker = (bindable as RSPicker<T>);
            rsPicker.PickerText.Text += GetPropValue(rsPicker.SelectedItem, rsPicker.DisplayMemberPath);
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly BindableProperty DisplayMemberPathProperty = BindableProperty.Create(nameof(DisplayMemberPath), typeof(string), typeof(RSPicker<T>), null, propertyChanged: DisplayMemberPathChanged);
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
        private static void DisplayMemberPathChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSPicker<T> rsPicker = (bindable as RSPicker<T>);
            rsPicker.CollectionView.DisplayMemberPath = (string)newValue;
        }
        private Entry PickerText;
        private TapGestureRecognizer tapGestureRecognizer;

        public RSPicker()
        {
            PickerText = new Entry();
            Content = PickerText;
            CollectionView = new RSCollectionView<T>()
            {
                SelectionMode = SelectionMode.Multiple
            };
            CollectionView.SelectionChanged += CollectionView_SelectionChanged;
            tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            Content.GestureRecognizers.Add(tapGestureRecognizer);
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
            //RSpopupManager.ShowPopup(CollectionView);
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
                Command = new Command(()=>
                {
                    CloseButtonPressed?.Invoke(this, EventArgs.Empty);
                    RSpopupManager.ClosePopup();
                })
            };

            grid.Add(CollectionView, 0, 0);
            grid.Add(button, 0, 1);

            return grid;
        }

        public event EventHandler CloseButtonPressed;

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
