namespace RSChartsMaui
{
    public class RSsplineChart : RSBaseChart
    {
        public RSsplineChart()
        {
            Drawable = new RSsplineChartDrawable(this);
        }
    }
}
