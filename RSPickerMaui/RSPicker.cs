using RSInputViewMaui;
using System.Reflection;
using RSPopupMaui;
using Microsoft.Maui.Controls;

namespace RSPickerMaui
{
    // All the code in this file is included in all platforms.
    public class RSPicker<T> : RSInputView
    {
        private RSCollectionView<T> CollectionView { get; set; }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(ICollection<T>), typeof(RSPicker<T>), null);
        public ICollection<T> ItemsSource
        {
            get { return (ICollection<T>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(nameof(SelectedItems), typeof(ICollection<T>), typeof(RSPicker<T>), null, propertyChanged:SelectedItemsChange);
        private static void SelectedItemsChange(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSPicker<T>).PickerText.Text += GetPropValue((bindable as RSPicker<T>).SelectedItem, (bindable as RSPicker<T>).DisplayMemberPath);
        }
        public ICollection<T> SelectedItems
        {
            get { return (ICollection<T>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(RSPicker<T>), null, propertyChanged:SelectedItemChanged);
        private static void SelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSPicker<T>).PickerText.Text += GetPropValue((bindable as RSPicker<T>).SelectedItem, (bindable as RSPicker<T>).DisplayMemberPath);
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public string DisplayMemberPath { get; set; }

        private Entry PickerText;


        public RSPicker()
        {
            PickerText = new Entry();
            //PickerText.Padding = new Thickness(PickerText.Padding.Left, 14, PickerText.Padding.Right, 14);
            Content = PickerText;
            CollectionView = new RSCollectionView<T>()
            {
                SelectionMode = SelectionMode.Multiple
            };
            CollectionView.SelectionChanged += CollectionView_SelectionChanged;
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            //Content.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void CollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            // Update picker text here
            PickerText.Unfocus();
            PickerText.Text = "troll";
            PickerText.Focus();
        }

        private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
        {
            CollectionView.ItemsSource = ItemsSource;
            CollectionView.SelectedItems = SelectedItems;

            RSpopupManager.ShowPopup(CollectionView);
        }

        public static string GetPropValue(object src, string propName)
        {
            if (src is null || string.IsNullOrEmpty(propName))
                return string.Empty;

            var val = src.GetType().GetRuntimeProperty(propName).GetValue(src, null);
            return val != null ? val.ToString() : string.Empty;
        }
    }
}
