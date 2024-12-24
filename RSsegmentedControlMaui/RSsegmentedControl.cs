using Microsoft.Maui.Controls.Shapes;
using System.Collections;
using System.Linq;

namespace RSsegmentedControlMaui
{
    // All the code in this file is included in all platforms.
    public class RSsegmentedControl : Border
    {
        private StackBase stack;
        private View previousSelectedItemView;
        private View SelectedItemView;
        private IList Items;
        private List<View> ListOfViewsWithoutSeparators;


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

            RSsegmentedControl rSsegmentedControl = bindable as RSsegmentedControl;

            rSsegmentedControl.InitItems();

            if(rSsegmentedControl.Items.Count < 1)
                rSsegmentedControl.SelectedIndex = -1;
            else if((rSsegmentedControl.Items.Count - 1) < rSsegmentedControl.SelectedIndex)
                rSsegmentedControl.SelectedIndex = (rSsegmentedControl.Items.Count - 1);
            
            rSsegmentedControl.SetSelectedItem();
            rSsegmentedControl.HighlightItem(); 
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
            (bindable as RSsegmentedControl).SetSelectedItem();
            (bindable as RSsegmentedControl).HighlightItem();
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
            rSsegmentedControl.HighlightItem();
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

            (bindable as RSsegmentedControl).SetSelectedItem();
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


        public static readonly BindableProperty SelectionModeProperty = BindableProperty.Create(nameof(SelectionMode), typeof(SelectionModeEnum), typeof(RSsegmentedControl), SelectionModeEnum.NonMandatory, BindingMode.TwoWay, propertyChanged: OnSelectionModePropertyChanged);
        public SelectionModeEnum SelectionMode
        {
            get { return (SelectionModeEnum)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        private static void OnSelectionModePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;
        }


        public static readonly BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation), typeof(OrientationEnum), typeof(RSsegmentedControl), OrientationEnum.Horizontal, BindingMode.TwoWay, propertyChanged: OnOrientationPropertyChanged);
        public OrientationEnum Orientation
        {
            get { return (OrientationEnum)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        private static void OnOrientationPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            RSsegmentedControl rSsegmentedControl = bindable as RSsegmentedControl;

            if(newValue != oldValue)
            {
                rSsegmentedControl.stack = (OrientationEnum)newValue == OrientationEnum.Horizontal ? new HorizontalStackLayout() : new VerticalStackLayout();
                rSsegmentedControl.Content = rSsegmentedControl.stack;
            }

            rSsegmentedControl.InitItems();
            rSsegmentedControl.SetSelectedItem();
            rSsegmentedControl.HighlightItem();
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

            stack = new HorizontalStackLayout();
            Content = stack;

            Items = new List<object>();
            ListOfViewsWithoutSeparators = new List<View>();
        }

        private void SetSelectedItem()
        {
            if (SelectedIndex < 0 || SelectedIndex >= Items.Count)
            {
                SelectedItemView = null;
                SelectedItem = null;
                previousSelectedItemView = null;
            }
            else
            {
                SelectedItemView = ListOfViewsWithoutSeparators.ElementAt(SelectedIndex);   
                SelectedItem = Items[SelectedIndex]; ;
                previousSelectedItemView = SelectedItemView;
            }
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

        async Task AnimateBackgroundColorChange(VisualElement element, Color fromColor, Color toColor, uint duration)
        {
            if (element == null)
                return;

            if (fromColor == null)
                fromColor = Colors.Transparent;

            var animation = new Animation(v =>
            {
                element.BackgroundColor = Color.FromRgba(
                    fromColor.Red + (toColor.Red - fromColor.Red) * v,
                    fromColor.Green + (toColor.Green - fromColor.Green) * v,
                    fromColor.Blue + (toColor.Blue - fromColor.Blue) * v,
                    fromColor.Alpha + (toColor.Alpha - fromColor.Alpha) * v);
            });

            animation.Commit(element, "BackgroundColorAnimation", length: duration, easing: Easing.Linear);
        }

        private async void HighlightItem()
        {
            if (SelectedItemView == null && previousSelectedItemView == null)
                return;

            if (previousSelectedItemView != null)
            {
                await AnimateBackgroundColorChange(previousSelectedItemView, previousSelectedItemView.BackgroundColor, Colors.Transparent, 250);
            }

            if (SelectedItemView != null)
            {
                await AnimateBackgroundColorChange(SelectedItemView, SelectedItemView.BackgroundColor, SelectedColor, 250);
            }
        }

        private void TapMehod(View item)
        {
            var currentIndex = ListOfViewsWithoutSeparators.IndexOf(item);

            if (SelectionMode == SelectionModeEnum.Mandatory && SelectedIndex == currentIndex)
                return;

            SelectedIndex = SelectedIndex == currentIndex ? -1 : currentIndex;
        }

        private void InitItems()
        {
            stack.Children.Clear();
            Items.Clear();
            ListOfViewsWithoutSeparators.Clear();

            int i = 0;

            foreach (var item in ItemsSource)
            {
                Items.Add(item);
            }

            foreach (var item in ItemsSource)
            {
                var itemView = ItemTemplate.CreateContent() as View;
                itemView.BindingContext = item;
                itemView.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command<View>(TapMehod),
                    CommandParameter = itemView
                });

                ListOfViewsWithoutSeparators.Add(itemView); 

                BoxView separator = Orientation == OrientationEnum.Horizontal ? new BoxView() { WidthRequest = 1 } :
                                                                                new BoxView() { HeightRequest = 1 };

                Binding binding = new Binding("Stroke", source: this, converter: new BrushToColorConverter(), converterParameter: separator);
                separator.SetBinding(BoxView.ColorProperty, binding);


                stack.Add(itemView);
                if(i < (Items.Count - 1))
                    stack.Add(separator);

                i++;
            }
        }
    }
}
