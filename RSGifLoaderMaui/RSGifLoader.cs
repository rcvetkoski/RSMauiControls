namespace RSGifLoaderMaui
{
    public class RSGifLoader : Image
    {
        public static readonly BindableProperty PathProperty = BindableProperty.Create(nameof(Path), typeof(string), typeof(RSGifLoader), string.Empty);
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }
    }
}
