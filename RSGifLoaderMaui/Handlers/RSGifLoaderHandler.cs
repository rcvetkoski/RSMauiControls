using Microsoft.Maui.Handlers;
#if ANDROID
using Android.Content;
using Android.Content.Res;
using Android.Widget;
#elif IOS
using Foundation;
using ImageIO;
using UIKit;
#endif

namespace RSGifLoaderMaui.Handlers
{
#if ANDROID
    public partial class GifLoaderHandler : ViewHandler<RSGifLoader, ImageView>
    {
        public static IPropertyMapper<RSGifLoader, GifLoaderHandler> PropertyMapper = new PropertyMapper<RSGifLoader, GifLoaderHandler>(ViewHandler.ViewMapper)
        {
            //[nameof(RSGifLoader.IsLoaded)] = MapIsLoaded
        };


        public GifLoaderHandler() : base(PropertyMapper)
        {
        }

        protected override void ConnectHandler(ImageView platformView)
        {
            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(ImageView platformView)
        {
            base.DisconnectHandler(platformView);
        }

        protected override ImageView CreatePlatformView()
        {
            // Create the Android ImageView
            var context = MauiApplication.Current.ApplicationContext;
            var imageView = new ImageView(context);
            var path = this.VirtualView.Path;
            int resourceId = LoadImageFromMauiResources(context, context.Resources, path);

            // Use Glide to load the image
            Bumptech.Glide.Glide.With(context)
            .Load(resourceId)
            .Into(imageView);

            return imageView;
        }

        public int LoadImageFromMauiResources(Context context, Resources resources, string path)
        {
            int resourceId = context.Resources.GetIdentifier(path, "drawable", context.PackageName);
            return resourceId;
        }
    }
#elif IOS
    public partial class GifLoaderHandler : ViewHandler<RSGifLoader, UIImageView>
    {
        public static IPropertyMapper<RSGifLoader, GifLoaderHandler> PropertyMapper = new PropertyMapper<RSGifLoader, GifLoaderHandler>(ViewHandler.ViewMapper)
        {
            //[nameof(RSGifLoader.IsLoaded)] = MapIsLoaded
        };

        public GifLoaderHandler() : base(PropertyMapper)
        {
        }

        protected override void ConnectHandler(UIImageView platformView)
        {
            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(UIImageView platformView)
        {
            base.DisconnectHandler(platformView);
        }

        private UIImage animatedImage;
        private void LoadGif(string path)
        {
            NSData data = NSData.FromFile(path + ".gif");
            CGImageSource source = CGImageSource.FromData(data);

            var frameCount = (int)source.ImageCount;
            var images = new List<UIImage>();
            var totalDuration = 0.0f;

            for (int i = 0; i < frameCount; i++)
            {
                using var cgImage = source.CreateImage(i, null);
                var frameDuration = GetFrameDuration(source, i); // Method to get frame duration
                var image = UIImage.FromImage(cgImage);

                images.Add(image);
                totalDuration += frameDuration;
            }

            animatedImage = UIImage.CreateAnimatedImage(images.ToArray(), totalDuration);
        }

        private CustomUIImageView imageView;

        protected override UIImageView CreatePlatformView()
        {
            LoadGif(VirtualView.Path);
            imageView = new CustomUIImageView();
            imageView.Image = animatedImage;
            imageView.AnimationRepeatCount = 0; // Repeat indefinitely
            imageView.StartAnimating();
            imageView.AnimationSet = true;
            VirtualView.BindingContextChanged += (sender, args) =>
            {
                Update();
            };

            return imageView;
        }

        public void Update()
        {
            if (imageView.AnimationSet)
                return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                //UIImageView image = (sender as RSGifLoader).ToPlatform((sender as RSGifLoader).Handler.MauiContext).Subviews[0] as UIImageView;
                imageView.Image = null; // set ti null to provoke refresh otherwise it may not relaunch the animation
                imageView.Image = animatedImage;
                imageView.AnimationRepeatCount = 0; // Repeat indefinitely
                imageView.AnimationSet = true;
                imageView.StartAnimating();
            });
        }

        private float GetFrameDuration(CGImageSource source, int index)
        {
            return 0.80f;
            // Implement logic to calculate each frame's duration
        }

        public class CustomUIImageView : UIImageView
        {
            public bool AnimationSet { get; set; }

            public override bool IsAnimating
            {
                get
                {
                    if (AnimationSet)
                    {
                        Console.WriteLine("sfew");
                    }

                    AnimationSet = base.IsAnimating;
                    return base.IsAnimating;
                }
            }
        }
    }
#endif
}

