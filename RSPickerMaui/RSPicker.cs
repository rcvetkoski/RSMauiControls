using Microsoft.Maui.Layouts;
using RSInputViewMaui;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Maui;

namespace RSPickerMaui
{
    // All the code in this file is included in all platforms.
    public class RSPicker : RSInputView
    {
        private CollectionView CollectionView { get; set; }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<object>), typeof(RSPicker), null);
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(nameof(SelectedItems), typeof(IEnumerable<object>), typeof(RSPicker), null, propertyChanged:SelectedItemsChange);

        private static void SelectedItemsChange(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSPicker).PickerText.Text += GetPropValue((bindable as RSPicker).SelectedItem, (bindable as RSPicker).DisplayMemberPath);
        }

        public IEnumerable<object> SelectedItems
        {
            get { return (IEnumerable<object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(RSPicker), null, propertyChanged:SelectedItemChanged);
        private static void SelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as RSPicker).PickerText.Text += GetPropValue((bindable as RSPicker).SelectedItem, (bindable as RSPicker).DisplayMemberPath);
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public string DisplayMemberPath { get; set; }

        private Label PickerText;

        public RSPicker()
        {
            PickerText = new Label();
            PickerText.Padding = new Thickness(PickerText.Padding.Left, 14, PickerText.Padding.Right, 14);
            Content = PickerText;
            CollectionView = new CollectionView();
            CollectionView.SelectionChanged += CollectionView_SelectionChanged;
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
            Content.GestureRecognizers.Add(tapGestureRecognizer);

        }

        private void CollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            
        }

        private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
        {
            CollectionView.ItemsSource = ItemsSource;
        }

        public static string GetPropValue(object src, string propName)
        {
            if (src is null || string.IsNullOrEmpty(propName))
                return string.Empty;

            var val = src.GetType().GetRuntimeProperty(propName).GetValue(src, null);
            return val != null ? val.ToString() : string.Empty;
        }
    }


    public class RSCollectionView<T> : CollectionView
    {
        public RSCollectionView() 
        {
        }

        private List<object>? tempItemsSource;

        new public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(nameof(SelectedItems), typeof(ICollection<T>), typeof(RSCollectionView<T>), null, propertyChanged: SelectedItemsChanged);
        new public ICollection<T> SelectedItems
        {
            get { return (ICollection<T>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        private static void SelectedItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            if (newValue != null)
            {
                RSCollectionView<T> rsCollectionView = (bindable as RSCollectionView<T>);
                List<object> tempList = new List<object>();

                foreach (var item in rsCollectionView.SelectedItems)
                {
                    foreach (var item2 in (bindable as CollectionView).ItemsSource)
                    {
                        if ((item2 as RSItem).Item.Equals(item))
                        {
                            (item2 as RSItem).IsSelected = true;
                            tempList.Add(item2);
                        }
                    }
                }

                rsCollectionView.SelectedItems.Clear();
                (bindable as CollectionView).SelectedItems = tempList;
            }
        }



        new public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable<T>), typeof(RSCollectionView<T>), null, propertyChanged: ItemsSourceChanged);
        new public IEnumerable<T> ItemsSource
        {
            get { return (IEnumerable<T>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            if (newValue != null)
            {
                RSCollectionView<T> rsCollectionView = (bindable as RSCollectionView<T>);
                rsCollectionView.tempItemsSource = new List<object>();

                foreach (var item in rsCollectionView.ItemsSource)
                {
                    RSItem rSItem = new RSItem(item);
                    rsCollectionView.tempItemsSource.Add(rSItem);
                }

                (bindable as CollectionView).ItemsSource = rsCollectionView.tempItemsSource;

                if (rsCollectionView.ItemsSource is INotifyCollectionChanged observableDataSource)
                    rsCollectionView.Add_ItemsSource_ObservableDataSource_CollectionChanged_Event(observableDataSource);
            }
        }

        public void Add_ItemsSource_ObservableDataSource_CollectionChanged_Event(INotifyCollectionChanged source)
        {
            source.CollectionChanged -= ItemsSourceDataSource_CollectionChanged;
            source.CollectionChanged += ItemsSourceDataSource_CollectionChanged;
        }
        private void ItemsSourceDataSource_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(var item in e.NewItems)
                    {
                        RSItem rSItem = new RSItem(item);
                        tempItemsSource.Add(rSItem);
                    }
                    OnPropertyChanged("ItemsSource");
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var itemToRemove = tempItemsSource.First(x => (x as RSItem).Item == item);
                        tempItemsSource.Remove(itemToRemove);
                    }
                    OnPropertyChanged("ItemsSource");
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            if (SelectedItems == null)
                return;

            foreach (var item in args.CurrentSelection)
            {
                if (!args.PreviousSelection.Contains(item))
                {
                    (item as RSItem).IsSelected = true;
                    SelectedItems.Add((T)(item as RSItem).Item);
                }
            }

            foreach (var item in args.PreviousSelection)
            {
                if (!args.CurrentSelection.Contains(item))
                {
                    (item as RSItem).IsSelected = false;
                    SelectedItems.Remove((T)(item as RSItem).Item);
                }
            }
        }
    }

    public class RSItem : INotifyPropertyChanged
    {
        private bool isSelected;
        public bool IsSelected 
        { 
            get
            {
                return isSelected;
            }
            set
            {
                if(isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public object Item { get; set; }

        public RSItem(object item) 
        {
            Item = item;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
