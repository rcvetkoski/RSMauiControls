using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Shapes;

namespace RSPopupMaui;

public class RSPopup : ContentPage
{
    private const uint AnimationLength = 250;
    private const double ScrimOpacity = 0.4;

    private readonly bool isModal;
    private readonly RSPopupAnimationTypeEnum animationType;
    private readonly Color lightBackgroundColor = Colors.White;
    private readonly Color darkBackgroundColor = Color.FromArgb("#212121");
    private readonly BoxView scrim;
    private readonly Border popup;
    private readonly PanGestureRecognizer? panGesture;
    private readonly StatusBarBehavior? statusBarBehavior;
    private bool isClosing;
    private double panStartTranslation;

    public event EventHandler? PopupClosed;

    public RSPopup(IView view, RSPopupAnimationTypeEnum rSPopupAnimationTypeEnum, bool isModal)
    {
        if (view is not View popupContent)
            throw new ArgumentException("Popup content must be a MAUI View.", nameof(view));

        animationType = rSPopupAnimationTypeEnum;
        this.isModal = isModal;
        BackgroundColor = Colors.Transparent;

        var holder = new Grid
        {
            BackgroundColor = Colors.Transparent
        };

        scrim = new BoxView
        {
            Color = Colors.Black,
            Opacity = 0
        };

        if (!isModal)
        {
            scrim.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () => await ClosePopup())
            });
        }

        popup = CreatePopupContainer(popupContent);
        holder.Add(scrim);
        holder.Add(popup);
        Content = holder;

        // Keep the popup invisible until it has its final arranged size. Newer
        // MAUI versions can render a frame before Loaded handlers finish.
        popup.Opacity = 0;

        if (rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.BottomToTop)
        {
            panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            popup.GestureRecognizers.Add(panGesture);
        }
        else
        {
            popup.Scale = 0.9;
        }

        var (backgroundColor, statusBarStyle) = GetThemeAppearance();
#if IOS
        if (OperatingSystem.IsIOSVersionAtLeast(15))
#endif
        {
#pragma warning disable CA1416 // Guarded above on iOS; always supported on Android.
            statusBarBehavior = new StatusBarBehavior
            {
                ApplyOn = StatusBarApplyOn.OnPageNavigatedTo,
                StatusBarColor = backgroundColor,
                StatusBarStyle = statusBarStyle
            };
            Behaviors.Add(statusBarBehavior);
#pragma warning restore CA1416
        }

        popup.BackgroundColor = backgroundColor;
        Loaded += OnLoaded;

        if (Application.Current is not null)
            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
    }

    private Border CreatePopupContainer(View popupContent)
    {
        var isBottomSheet = animationType == RSPopupAnimationTypeEnum.BottomToTop;
        var container = new Border
        {
            StrokeThickness = 0,
            Padding = isBottomSheet ? new Thickness(20, 10, 20, 20) : new Thickness(20),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = isBottomSheet
                    ? new CornerRadius(25, 25, 0, 0)
                    : new CornerRadius(10)
            },
            VerticalOptions = isBottomSheet ? LayoutOptions.End : LayoutOptions.Center,
            HorizontalOptions = isBottomSheet ? LayoutOptions.Fill : LayoutOptions.Center,
            Margin = isBottomSheet ? new Thickness(0) : new Thickness(30)
        };

        // Consume taps inside the popup so they do not dismiss it.
        container.GestureRecognizers.Add(new TapGestureRecognizer());

        if (!isBottomSheet)
        {
            container.Content = popupContent;
            return container;
        }

        var bottomSheetContent = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            RowSpacing = 15
        };

        bottomSheetContent.Add(new BoxView
        {
            Color = Colors.Gray,
            WidthRequest = 40,
            HeightRequest = 4,
            CornerRadius = 2,
            HorizontalOptions = LayoutOptions.Center
        }, 0, 0);
        bottomSheetContent.Add(popupContent, 0, 1);
        container.Content = bottomSheetContent;

        return container;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        Loaded -= OnLoaded;

        // Let MAUI finish arranging the modal page before reading Height.
        await Task.Yield();

        if (animationType == RSPopupAnimationTypeEnum.BottomToTop)
        {
            popup.TranslationY = GetPopupHeight();
            popup.Opacity = 1;

            await Task.WhenAll(
                scrim.FadeToAsync(ScrimOpacity, AnimationLength, Easing.Linear),
                popup.TranslateToAsync(0, 0, AnimationLength, Easing.CubicOut));
        }
        else
        {
            await Task.WhenAll(
                scrim.FadeToAsync(ScrimOpacity, AnimationLength, Easing.Linear),
                popup.FadeToAsync(1, AnimationLength, Easing.CubicOut),
                popup.ScaleToAsync(1, AnimationLength, Easing.CubicOut));
        }
    }

    private async void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (isClosing)
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                panStartTranslation = popup.TranslationY;
                break;

            case GestureStatus.Running:
                // TotalY is cumulative from the gesture start; do not add it on
                // every event or the sheet accelerates and jumps.
                popup.TranslationY = Math.Max(0, panStartTranslation + e.TotalY);
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                if (popup.TranslationY > 55)
                    await ClosePopup();
                else
                    await popup.TranslateToAsync(0, 0, AnimationLength, Easing.CubicOut);
                break;
        }
    }

    public void ApplyThemeSpecificStyleToPopup(Border border)
    {
        var (backgroundColor, statusBarStyle) = GetThemeAppearance();
        border.BackgroundColor = backgroundColor;
#if IOS
        if (OperatingSystem.IsIOSVersionAtLeast(15) && statusBarBehavior is not null)
#else
        if (statusBarBehavior is not null)
#endif
        {
#pragma warning disable CA1416 // Guarded above on iOS; always supported on Android.
            statusBarBehavior.StatusBarColor = backgroundColor;
            statusBarBehavior.StatusBarStyle = statusBarStyle;
#pragma warning restore CA1416
        }
    }

    public async Task ClosePopup()
    {
        if (isClosing)
            return;

        isClosing = true;

        await CloseAnimatePopup();

        // The popup is pushed modally, so it must be removed from the modal stack.
        if (Navigation.ModalStack.Contains(this))
            await Navigation.PopModalAsync(animated: false);

        OnPopupClosedInternal(EventArgs.Empty);
    }

    public async Task CloseAnimatePopup()
    {
        if (animationType == RSPopupAnimationTypeEnum.BottomToTop)
        {
            await Task.WhenAll(
                scrim.FadeToAsync(0, AnimationLength, Easing.Linear),
                popup.TranslateToAsync(0, GetPopupHeight(), AnimationLength, Easing.CubicIn));
        }
        else
        {
            await Task.WhenAll(
                scrim.FadeToAsync(0, AnimationLength, Easing.Linear),
                popup.FadeToAsync(0, AnimationLength, Easing.CubicIn),
                popup.ScaleToAsync(0.9, AnimationLength, Easing.CubicIn));
        }

        if (panGesture is not null)
            panGesture.PanUpdated -= OnPanUpdated;
    }

    protected override bool OnBackButtonPressed()
    {
        if (!isModal)
            _ = ClosePopup();

        return true;
    }

    protected override void OnDisappearing()
    {
        if (Application.Current is not null)
            Application.Current.RequestedThemeChanged -= OnRequestedThemeChanged;

        if (panGesture is not null)
            panGesture.PanUpdated -= OnPanUpdated;

        base.OnDisappearing();
    }

    private double GetPopupHeight()
    {
        if (popup.Height > 0)
            return popup.Height;

        var availableWidth = Width > 0 ? Width : double.PositiveInfinity;
        return Math.Max(1, popup.Measure(availableWidth, double.PositiveInfinity).Height);
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        Dispatcher.Dispatch(() => ApplyThemeSpecificStyleToPopup(popup));
    }

    private (Color BackgroundColor, StatusBarStyle StatusBarStyle) GetThemeAppearance()
    {
        var application = Application.Current;
        var effectiveTheme = application?.UserAppTheme == AppTheme.Unspecified
            ? application.RequestedTheme
            : application?.UserAppTheme ?? AppTheme.Light;

        var isDarkTheme = effectiveTheme == AppTheme.Dark;
        var resourceKey = isDarkTheme ? "CardBackgroundDark" : "CardBackground";
        var fallbackColor = isDarkTheme ? darkBackgroundColor : lightBackgroundColor;
        var backgroundColor = application?.Resources.TryGetValue(resourceKey, out var resource) == true
                              && resource is Color resourceColor
            ? resourceColor
            : fallbackColor;

        return (
            backgroundColor,
            isDarkTheme ? StatusBarStyle.LightContent : StatusBarStyle.DarkContent);
    }

    protected void OnPopupClosedInternal(EventArgs e)
    {
        PopupClosed?.Invoke(this, e);
    }
}
