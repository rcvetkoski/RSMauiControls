using Microsoft.Maui.Controls.Shapes;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace RSsegmentedControlMaui
{
    // All the code in this file is included in all platforms.
    public class RSsegmentedControl : Border
    {
        private HorizontalStackLayout hStack;
        private View previousSelectedItemView;
        private View SelectedItemView;
        private object previousSelectedItem;
        private IList Items;


        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(RSsegmentedControl), null, propertyChanged: ItemsSourceChange);
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        private static void ItemsSourceChange(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            (bindable as RSsegmentedControl).InitItems();
            (bindable as RSsegmentedControl).SetSelectedIndex();
        }


        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(RSsegmentedControl), SetDefaultDataTemplate(), propertyChanged: OnItemTemplatePropertyChanged);
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        private static void OnItemTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            (bindable as RSsegmentedControl).InitItems();
        }


        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(RSsegmentedControl), null, BindingMode.TwoWay, propertyChanged: OnSelectedItemPropertyChanged);
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        private static void OnSelectedItemPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSsegmentedControl rSsegmentedControl = bindable as RSsegmentedControl;

            var view = rSsegmentedControl.hStack.FirstOrDefault(x => (x as View).BindingContext == newValue) as View;
            rSsegmentedControl.HighlightItem(view);
        }


        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(RSsegmentedControl), -1, BindingMode.TwoWay, propertyChanged: OnSelectedIndexPropertyChanged);
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        private static void OnSelectedIndexPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            (bindable as RSsegmentedControl).SetSelectedIndex();
        }


        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(RSsegmentedControl), Colors.SteelBlue, BindingMode.TwoWay, propertyChanged: OnSelectedColorPropertyChanged);
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }
        private static void OnSelectedColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            (bindable as RSsegmentedControl).SelectedItemView.BackgroundColor = newValue as Color;
        }



        public RSsegmentedControl()
        {
            VerticalOptions = LayoutOptions.Start;
            HorizontalOptions = LayoutOptions.Start;
            Stroke = Colors.Grey;
            this.StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(10)
            };

            hStack = new HorizontalStackLayout()
            {
                
            };

            Content = hStack;
            previousSelectedItem = null;
            Items = new List<object>();
        }

        private void SetSelectedIndex()
        {
            if (SelectedIndex < 0 || SelectedIndex >= Items.Count)
            {
                SelectedItem = null;
            }
            else
            {
                var item = hStack.FirstOrDefault(x => (x as View).BindingContext == Items[SelectedIndex]) as View;
                SelectedItem = item?.BindingContext;
            }

            previousSelectedItem = SelectedItem;
        }

        private static DataTemplate SetDefaultDataTemplate()
        {
            return new DataTemplate(() =>
            {
                Label label = new Label()
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Padding = new Thickness(10)
                };

                label.SetBinding(Label.TextProperty, ".");

                return label;
            });
        }

        private void HighlightItem(View view)
        {
            if (SelectedItem == null && previousSelectedItem == null)
                return;

            if(previousSelectedItem != null)
            {
                previousSelectedItemView = hStack.FirstOrDefault(x => (x as View).BindingContext == previousSelectedItem) as View;
                previousSelectedItemView.BackgroundColor = Colors.Transparent;
            }

            if(SelectedItem != null)
            {
                SelectedItemView = hStack.FirstOrDefault(x => (x as View).BindingContext == SelectedItem) as View;
                SelectedItemView.BackgroundColor = SelectedColor;
            }
        }

        private void TapMehod(object item)
        {
            var currentIndex = Items.IndexOf(item);
            SelectedIndex = SelectedIndex == currentIndex ? -1 : currentIndex;
        }

        private void InitItems()
        {
            hStack.Children.Clear();
            Items.Clear();

            foreach (var item in ItemsSource)
            {
                Items.Add(item);

                var itemView = ItemTemplate.CreateContent() as View;
                itemView.BindingContext = item;
                itemView.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command<object>(TapMehod),
                    CommandParameter = item
                });
                    
                BoxView separator = new BoxView()
                {
                    WidthRequest = 1
                };
                Binding binding = new Binding("Stroke", source: this, converter: new BrushToColorConverter(), converterParameter: separator);
                separator.SetBinding(BoxView.ColorProperty, binding);


                hStack.Add(itemView);
                hStack.Add(separator);
            }
        }
    }
}
