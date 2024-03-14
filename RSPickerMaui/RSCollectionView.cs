using System.Collections.Specialized;

namespace RSPickerMaui
{
    public class RSCollectionView<T> : CollectionView
    {
        public RSCollectionView()
        {
            ItemTemplate = new DataTemplate(() =>
            {
                // Create the grid and define its columns
                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Star }
                    }
                };

                // Create the CheckBox and set its IsChecked binding
                var checkBox = new CheckBox();
                checkBox.SetBinding(CheckBox.IsCheckedProperty, "IsSelected");
                Grid.SetColumn(checkBox, 0); // Set CheckBox to the first column

                // Create the Label and set its Text binding
                var label = new Label { VerticalOptions = LayoutOptions.Center };
                label.SetBinding(Label.TextProperty, "Item");
                Grid.SetColumn(label, 1); // Set Label to the second column

                // Add the CheckBox and Label to the Grid
                grid.Children.Add(checkBox);
                grid.Children.Add(label);

                return grid;
            });
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

            RSCollectionView<T> rsCollectionView = (bindable as RSCollectionView<T>);

            if (newValue != null && rsCollectionView.ItemsSource != null)
                rsCollectionView.FillOriginalItemsSource();
        }

        private void FillOriginalItemsSource()
        {
            List<object> tempList = new List<object>();

            foreach (var item in SelectedItems)
            {
                foreach (var item2 in (this as CollectionView).ItemsSource)
                {
                    if ((item2 as RSItem).Item.Equals(item))
                    {
                        (item2 as RSItem).IsSelected = true;
                        tempList.Add(item2);
                    }
                }
            }

            SelectedItems.Clear();
            (this as CollectionView).SelectedItems = tempList;
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


                // Fill SelectedItems
                if (rsCollectionView.SelectedItems != null)
                    rsCollectionView.FillOriginalItemsSource();


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
                    foreach (var item in e.NewItems)
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
}
