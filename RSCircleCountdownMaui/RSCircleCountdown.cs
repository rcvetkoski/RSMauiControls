using System.Diagnostics;
using System.Windows.Input;

namespace RSCircleCountdownMaui
{
    // All the code in this file is included in all platforms.
    public class RSCircleCountdown : GraphicsView
    {
        CircularCountdownDrawable drawable;
        uint remainingDuration;
        Stopwatch stopwatch;
        bool isRunning;


        public static readonly BindableProperty DurationProperty = BindableProperty.Create(nameof(Duration), typeof(TimeSpan), typeof(RSCircleCountdown), null, propertyChanged: DurationChange);
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        private static void DurationChange(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (bindable as RSCircleCountdown);
            if (control == null)
                return;

            control.remainingDuration = (uint)control.Duration.TotalMilliseconds;
            control.StartCountdown();
        }


        public static readonly BindableProperty ProgressColorProperty = BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(RSCircleCountdown), Colors.SteelBlue, propertyChanged: ProgressColorChanged);
        public Color ProgressColor
        {
            get { return (Color)GetValue(ProgressColorProperty); }
            set { SetValue(ProgressColorProperty, value); }
        }
        private static void ProgressColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (bindable as RSCircleCountdown);
            if (control == null || newValue == null || control.Drawable == null)
                return;

            (control.Drawable as CircularCountdownDrawable).ProgressColor = newValue as Color;
        }

        public static readonly BindableProperty CircleColorProperty = BindableProperty.Create(nameof(CircleColor), typeof(Color), typeof(RSCircleCountdown), Colors.LightGray, propertyChanged: CircleColorChanged);
        public Color CircleColor
        {
            get { return (Color)GetValue(CircleColorProperty); }
            set { SetValue(CircleColorProperty, value); }
        }
        private static void CircleColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (bindable as RSCircleCountdown);
            if (control == null || newValue == null || control.Drawable == null)
                return;

            (control.Drawable as CircularCountdownDrawable).CircleColor = newValue as Color;
        }

        public static readonly BindableProperty StrokeSizeProperty = BindableProperty.Create(nameof(StrokeSize), typeof(float), typeof(RSCircleCountdown), 7f, propertyChanged: StrokeSizeChanged);
        public float StrokeSize
        {
            get { return (float)GetValue(StrokeSizeProperty); }
            set { SetValue(StrokeSizeProperty, value); }
        }
        private static void StrokeSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (bindable as RSCircleCountdown);
            if (control == null || newValue == null || control.Drawable == null)
                return;

            (control.Drawable as CircularCountdownDrawable).StrokeZize = (float)newValue;
        }


        public static readonly BindableProperty IsTextVisibleProperty = BindableProperty.Create(nameof(IsTextVisible), typeof(bool), typeof(RSCircleCountdown), true, propertyChanged: IsTextVisibleChanged);
        public bool IsTextVisible
        {
            get { return (bool)GetValue(IsTextVisibleProperty); }
            set { SetValue(IsTextVisibleProperty, value); }
        }
        private static void IsTextVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (bindable as RSCircleCountdown);
            if (control == null || newValue == null || control.Drawable == null)
                return;

            (control.Drawable as CircularCountdownDrawable).IsTextVisible = (bool)newValue;
        }


        public static readonly BindableProperty AddTimeCommandProperty = BindableProperty.Create(nameof(AddTimeCommand), typeof(ICommand), typeof(RSCircleCountdown), default(ICommand));

        public ICommand AddTimeCommand
        {
            get => (ICommand)GetValue(AddTimeCommandProperty);
            set => SetValue(AddTimeCommandProperty, value);
        }

        public static readonly BindableProperty RemoveTimeCommandProperty = BindableProperty.Create(nameof(RemoveTimeCommand), typeof(ICommand), typeof(RSCircleCountdown), default(ICommand));

        public ICommand RemoveTimeCommand
        {
            get => (ICommand)GetValue(RemoveTimeCommandProperty);
            set => SetValue(RemoveTimeCommandProperty, value);
        }


        public RSCircleCountdown()
        {
            WidthRequest = 100;
            HeightRequest = 100;
            drawable = new CircularCountdownDrawable(ProgressColor, CircleColor, StrokeSize, IsTextVisible);
            Drawable = drawable;
            stopwatch = new Stopwatch();

            // Set the default command to invoke AddTime, RemoveTime
            AddTimeCommand = new Command<string>(AddTime);
            RemoveTimeCommand = new Command<string>(RemoveTime);

        }

        public void StartCountdown()
        {
            if (!isRunning)
            {
                stopwatch.Start();
                isRunning = true;

                var animation = new Animation(v =>
                {
                    // Calculate progress based on remaining time
                    double elapsed = stopwatch.Elapsed.TotalMilliseconds;
                    double progress = Math.Clamp(1.0 - (elapsed / remainingDuration), 0, 1);
                    drawable.Progress = (float)progress;
                    drawable.Time = TimeSpan.FromMilliseconds(progress * remainingDuration);
                    this.Invalidate(); // Redraw the GraphicsView

                    TimerRunning?.Invoke(this, new CircleCountdownEventArgs() { Progress = progress, RemainingDuration = remainingDuration, Time = TimeSpan.FromMilliseconds(progress * remainingDuration)});

                    if (progress <= 0)
                    {
                        // Stop the countdown when time is up
                        stopwatch.Stop();
                        isRunning = false;

                        // Stop the animation explicitly
                        this.AbortAnimation("CountdownAnimation");

                        // Invoke TimerElapsed event
                        TimerElapsed?.Invoke(this, EventArgs.Empty);
                    }
                }, 0, 1);



                animation.Commit(this, "CountdownAnimation", length: remainingDuration, easing: Easing.Linear);
            }
        }

        public void StopCountdown()
        {
            // Stop the countdown 
            stopwatch?.Stop();
            isRunning = false;

            // Stop the animation explicitly
            this.AbortAnimation("CountdownAnimation");
            
            // Invoke TimerElapsed event
            TimerElapsed?.Invoke(this, EventArgs.Empty);
        }

        private void AddTime(string milliseconds)
        {
            // Stop the current animation and stopwatch
            stopwatch.Stop();
            isRunning = false;

            // Add time to remainingDuration
            remainingDuration += (uint)Int32.Parse(milliseconds);

            // Restart the countdown with the adjusted time
            StartCountdown();
        }

        private void RemoveTime(string milliseconds)
        {
            // Stop the current animation and stopwatch
            stopwatch.Stop();
            isRunning = false;
            var millisec = (uint)Int32.Parse(milliseconds);

            // Remove time from remainingDuration, ensuring it doesn't go negative
            remainingDuration = (remainingDuration > millisec) ? remainingDuration - millisec : 0;

            // Restart the countdown with the adjusted time
            StartCountdown();
        }

        public event EventHandler<CircleCountdownEventArgs> TimerRunning;
        public event EventHandler TimerElapsed;
    }

    public class CircleCountdownEventArgs : EventArgs
    {
        public double Progress { get; set; }
        public double RemainingDuration { get; set; }
        public TimeSpan Time { get; set; }
    }
}
