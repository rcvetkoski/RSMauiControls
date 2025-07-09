using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RSPickerMaui
{
    public class RSCollectionView : CollectionView
    {
        private bool canUpdateItemsSource = true;

        private ItemSourceHelper previousSelectedItem;

        public string DisplayMemberPath { get; set; }

        public new event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        public static readonly BindableProperty ConverterProperty = BindableProperty.Create(nameof(Converter), typeof(IValueConverter), typeof(RSCollectionView), default(IValueConverter));
        public IValueConverter Converter
        {
            get => (IValueConverter)GetValue(ConverterProperty);
            set => SetValue(ConverterProperty, value);
        }

        public RSCollectionView()
        {
            SetDefultDataTemplate();
        }

        public static new readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(nameof(SelectedItems), typeof(IList), typeof(RSCollectionView), null);
        public new IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);


            if (propertyName == CollectionView.ItemsSourceProperty.PropertyName && canUpdateItemsSource)
            {
                List<object> items = new List<object>();

                foreach (var item in ItemsSource)
                {
                    bool isSelected = false;

                    if (SelectionMode == SelectionMode.Multiple)
                    {
                        if (SelectedItems == null)
                            isSelected = false;
                        else if (SelectedItems.Contains(item))
                            isSelected = true;
                    }
                    else
                    {
                        if (SelectedItem == null)
                            isSelected = false;
                        else if (SelectedItem.Equals(item))
                            isSelected = true;
                    }

                    ItemSourceHelper itemSourceHelper = new ItemSourceHelper()
                    {
                        Item = item,
                        IsSelected = isSelected
                    };

                    items.Add(itemSourceHelper);
                }

                canUpdateItemsSource = false;
                ItemsSource = items;
            }
            else if (propertyName == RSCollectionView.SelectedItemsProperty.PropertyName)
            {
                if (SelectedItems == null || ItemsSource == null || SelectionMode != SelectionMode.Multiple)
                    return;

                SelectItemsInSource(SelectionMode);
            }
            else if (propertyName == RSCollectionView.SelectedItemProperty.PropertyName)
            {
                if (SelectedItem == null || ItemsSource == null || SelectionMode == SelectionMode.Multiple)
                    return;

                SelectItemsInSource(SelectionMode);

            }
            else if (propertyName == RSCollectionView.SelectionModeProperty.PropertyName)
            {
                if (ItemsSource == null || SelectedItem == null || SelectedItems == null)
                    return;

                SetDefultDataTemplate();
                SelectItemsInSource(SelectionMode);
            }
        }

        /// <summary>
        /// Selects the items either in Selected Items if multiple choice or SelectedItem if single
        /// </summary>
        /// <param name="selectionMode"></param>
        private void SelectItemsInSource(SelectionMode selectionMode)
        {
            foreach (ItemSourceHelper item in ItemsSource)
            {
                if (SelectionMode == SelectionMode.Multiple)
                {
                    if (SelectedItems.Contains(item.Item))
                        (item as ItemSourceHelper).IsSelected = true;
                    else
                        (item as ItemSourceHelper).IsSelected = false;
                }
                else
                {
                    if (SelectedItem.Equals(item.Item))
                        (item as ItemSourceHelper).IsSelected = true;
                    else
                        (item as ItemSourceHelper).IsSelected = false;
                }
            }
        }

        private void SetDefultDataTemplate()
        {
            ItemTemplate = new DataTemplate(() =>
            {
                // Grid with 1 row and 2 columns
                var grid = new Grid
                {
                    RowDefinitions = { new RowDefinition { Height = new GridLength(50) } },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Star }
                    }
                };

                // checkButton bound to IsSelected
                View checkButton;
                if (SelectionMode == SelectionMode.Multiple)
                {
                    checkButton = new CheckBox()
                    {
                        VerticalOptions = LayoutOptions.Center
                    };

                    checkButton.SetBinding(CheckBox.IsCheckedProperty, "IsSelected");
                }
                else
                {
                    checkButton = new RadioButton()
                    {
                        VerticalOptions = LayoutOptions.Center
                    };

                    checkButton.SetBinding(RadioButton.IsCheckedProperty, "IsSelected");
                }

                // Add tap gesture to RadioButton
                var rbTapGesture = new TapGestureRecognizer()
                {
                    Command = new Command(() => ItemClicked(grid.BindingContext as ItemSourceHelper))
                };
                checkButton.GestureRecognizers.Add(rbTapGesture);

                // Label bound to Item
                var label = new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Application.Current.RequestedTheme == AppTheme.Dark 
                    ? Colors.White 
                    : Colors.Black
                };

                if(DisplayMemberPath != null)
                    label.SetBinding(Label.TextProperty, $"Item.{DisplayMemberPath}", converter: Converter);
                else
                    label.SetBinding(Label.TextProperty, "Item", converter: Converter);

                // Add tap gesture to Grid itself
                var gridTapGesture = new TapGestureRecognizer()
                    {
                        Command = new Command(() => ItemClicked(grid.BindingContext as ItemSourceHelper))
                    };
                grid.GestureRecognizers.Add(gridTapGesture);

                // Add elements to Grid
                grid.Add(checkButton, 0, 0);
                grid.Add(label, 1, 0);


                // Visual States
                var normalState = new VisualState { Name = "Normal" };
                var selectedState = new VisualState
                {
                    Name = "Selected",
                    Setters =
                    {
                        new Setter
                        {
                            Property = VisualElement.BackgroundColorProperty,
                            Value = Colors.Transparent
                        }
                    }
                };
                var stateGroup = new VisualStateGroup
                {
                    Name = "CommonStates",
                    States = { normalState, selectedState }
                };
                VisualStateManager.SetVisualStateGroups(grid, new VisualStateGroupList { stateGroup });

                return grid;
            });
        }

        private void ItemClicked(ItemSourceHelper itemSourceHelper)
        {
            if (itemSourceHelper == null)
                return;

            object item = itemSourceHelper.Item;

            if (SelectionMode == SelectionMode.Multiple)
            {
                if (SelectedItems == null)
                    return;

                if (SelectedItems.Contains(item))
                {
                    itemSourceHelper.IsSelected = false;
                    SelectedItems.Remove(item);
                }
                else
                {
                    itemSourceHelper.IsSelected = true;
                    SelectedItems.Add(item);
                }


                List<object> list = new List<object>();
                foreach(var item2 in SelectedItems)
                    list.Add(item2); 

                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(list));
            }
            else
            {
                if (SelectedItem != item)
                {
                    SelectedItem = item;
                    itemSourceHelper.IsSelected = true;

                    if (previousSelectedItem != null && previousSelectedItem != itemSourceHelper)
                        previousSelectedItem.IsSelected = false;

                    previousSelectedItem = itemSourceHelper;


                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(new List<object>() { SelectedItem }));
                }
            }
        }
    }

    public class SelectionChangedEventArgs : EventArgs
    {
        public SelectionChangedEventArgs(List<object> list) 
        {
            CurrentSelection = list;    
        }  

        public IList CurrentSelection { get;set; }           
    }
}
