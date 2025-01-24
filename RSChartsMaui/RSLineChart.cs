namespace RSChartsMaui
{
    public class RSLineChart : RSBaseChart
    {
        public static readonly BindableProperty IsCurvedProperty = BindableProperty.Create(
            nameof(IsCurved),
            typeof(bool),
            typeof(RSLineChart),
            false,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                (((RSLineChart)bindable).Drawable as RSLineChartDrawable).ResetPaths();
                ((RSLineChart)bindable).Invalidate();
            });

        public bool IsCurved
        {
            get => (bool)GetValue(IsCurvedProperty);
            set => SetValue(IsCurvedProperty, value);
        }

        public RSLineChart()
        {
            Drawable = new RSLineChartDrawable(this);
        }
    }
}
