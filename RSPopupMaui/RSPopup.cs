using Microsoft.Maui.Controls.Shapes;

namespace RSPopupMaui
{
    // All the code in this file is included in all platforms.
    public class RSPopup : ContentPage
    {
        private bool isModal;

        private Color lightBackgroundColor = Colors.White;

        private Color darkBackgroundColor = Color.FromRgba("#212121");

        private RSPopupAnimationTypeEnum rSPopupAnimationTypeEnum;

        private PanGestureRecognizer panGesture;

        private Grid holder { get; set; }

        private Border popup { get; set; }

        public event EventHandler PopupClosed;

        public RSPopup(IView view, RSPopupAnimationTypeEnum rSPopupAnimationTypeEnum, bool isModal)
        {
            base.Loaded += RSPopup_Loaded;
            base.BackgroundColor = Colors.Transparent;
            this.isModal = isModal;
            this.rSPopupAnimationTypeEnum = rSPopupAnimationTypeEnum;
            holder = new Grid
            {
                BackgroundColor = Colors.Transparent
            };
            if (!isModal)
            {
                TapGestureRecognizer item = new TapGestureRecognizer
                {
                    Command = new Command((Action)delegate
                    {
                        ClosePopup();
                    })
                };
                holder.GestureRecognizers.Add(item);
            }

            LayoutOptions horizontalOptions = ((rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect) ? LayoutOptions.Center : LayoutOptions.Fill);
            LayoutOptions verticalOptions = ((rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect) ? LayoutOptions.Center : LayoutOptions.End);
            Thickness margin = ((rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect) ? new Thickness(30.0) : new Thickness(0.0));
            RoundRectangle strokeShape = ((rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect) ? new RoundRectangle
            {
                CornerRadius = new CornerRadius(10.0, 10.0, 10.0, 10.0)
            } : new RoundRectangle
            {
                CornerRadius = new CornerRadius(25.0, 25.0, 0.0, 0.0)
            });
            Thickness padding = ((rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect) ? new Thickness(20.0) : new Thickness(20.0, 10.0, 20.0, 20.0));
            popup = new Border
            {
                StrokeThickness = 0.0,
                Padding = padding,
                StrokeShape = strokeShape,
                VerticalOptions = verticalOptions,
                HorizontalOptions = horizontalOptions,
                Margin = margin
            };
            popup.GestureRecognizers.Add(new TapGestureRecognizer());
            if (rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.BottomToTop)
            {
                panGesture = new PanGestureRecognizer();
                panGesture.PanUpdated += PanGesture_PanUpdated;
                popup.GestureRecognizers.Add(panGesture);
            }

            Grid grid = null;
            if (rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.BottomToTop)
            {
                grid = new Grid
                {
                    RowDefinitions =
                {
                    new RowDefinition
                    {
                        Height = GridLength.Auto
                    },
                    new RowDefinition
                    {
                        Height = GridLength.Auto
                    }
                },
                    ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width = GridLength.Star
                    }
                },
                    RowSpacing = 15.0
                };
                BoxView view2 = new BoxView
                {
                    Color = Colors.Gray,
                    WidthRequest = 40.0,
                    HeightRequest = 4.0,
                    CornerRadius = 20.0
                };
                GridExtensions.Add(grid, view2);
                grid.Add((View)view, 0, 1);
            }

            holder.Children.Add(popup);
            popup.Content = ((rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect) ? ((View)view) : grid);
            base.Content = holder;
            ApplyThemeSpecificStyleToPopup(popup);
            if (Application.Current != null)
            {
                Application.Current.RequestedThemeChanged += delegate
                {
                    ApplyThemeSpecificStyleToPopup(popup);
                };
            }
        }

        private void PanGesture_PanUpdated(object? sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    popup.TranslationY = Math.Max(0.0, popup.TranslationY += e.TotalY);
                    break;
                case GestureStatus.Completed:
                    if (popup.TranslationY > 55.0)
                    {
                        ClosePopup();
                    }
                    else
                    {
                        popup.TranslateTo(0.0, 0.0, 250u, Easing.Linear);
                    }

                    break;
                case GestureStatus.Started:
                    break;
            }
        }

        public void ApplyThemeSpecificStyleToPopup(Border border)
        {
            switch (Application.Current.PlatformAppTheme)
            {
                case AppTheme.Light:
                    border.BackgroundColor = lightBackgroundColor;
                    break;
                case AppTheme.Dark:
                    border.BackgroundColor = darkBackgroundColor;
                    break;
            }
        }

        private async void RSPopup_Loaded(object? sender, EventArgs e)
        {
            base.Loaded -= RSPopup_Loaded;
            await OpenAnimatePopup();
        }

        protected override bool OnBackButtonPressed()
        {
            if (!isModal)
            {
                ClosePopup();
            }

            return true;
        }

        public async void ClosePopup()
        {
            await CloseAnimatePopup();
            await Shell.Current.Navigation.PopAsync(animated: false);
            OnPopupClosedInternal(EventArgs.Empty);
        }

        private Task OpenAnimatePopup()
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            if (rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect)
            {
                AnimatePopupPopInEffect(taskCompletionSource);
            }
            else
            {
                AnimatePopupFromBottom(taskCompletionSource);
            }

            return taskCompletionSource.Task;
        }

        public Task CloseAnimatePopup()
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            if (rSPopupAnimationTypeEnum == RSPopupAnimationTypeEnum.PopInEffect)
            {
                DismissPopupPopInEffect(taskCompletionSource);
            }
            else
            {
                DismissPopupFromBottom(taskCompletionSource);
            }

            if (panGesture != null)
            {
                panGesture.PanUpdated -= PanGesture_PanUpdated;
            }

            return taskCompletionSource.Task;
        }

        private void AnimatePopupPopInEffect(TaskCompletionSource<bool> done)
        {
            TaskCompletionSource<bool> done2 = done;
            this.Animate("BackgroundFadeIn", delegate (double v)
            {
                base.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v));
            }, 0.0, 0.4, 16u, 250u, Easing.Linear);
            double num = 0.8;
            double end = 1.0;
            base.Content.Opacity = 0.0;
            base.Content.Scale = num;
            base.Content.Animate("ContentFadeIn", new Animation(delegate (double v)
            {
                base.Content.Opacity = v;
            }, 0.0, 1.0, Easing.CubicInOut));
            base.Content.Animate("ContentScaleIn", new Animation(delegate (double v)
            {
                base.Content.Scale = v;
            }, num, end, Easing.SpringOut), 16u, 250u, null, delegate
            {
                done2.SetResult(result: true);
            });
        }

        private void AnimatePopupFromBottom(TaskCompletionSource<bool> done)
        {
            TaskCompletionSource<bool> done2 = done;
            this.Animate("BackgroundFadeIn", delegate (double v)
            {
                base.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v));
            }, 0.0, 0.4, 16u, 250u, Easing.Linear);
            double height = popup.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins).Request.Height;
            base.Content.TranslationY = height;
            base.Content.Animate("ContentMoveUp", new Animation(delegate (double v)
            {
                base.Content.TranslationY = v;
            }, height, 0.0, Easing.CubicInOut), 16u, 250u, null, delegate
            {
                done2.SetResult(result: true);
            });
        }

        private void DismissPopupFromBottom(TaskCompletionSource<bool> done)
        {
            TaskCompletionSource<bool> done2 = done;
            this.Animate("BackgroundFadeOut", delegate (double v)
            {
                base.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v));
            }, 0.4, 0.0, 16u, 250u, Easing.Linear);
            double height = popup.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins).Request.Height;
            base.Content.TranslationY = height;
            base.Content.Animate("ContentMoveUp", new Animation(delegate (double v)
            {
                base.Content.TranslationY = v;
            }, 0.0, height, Easing.CubicInOut), 16u, 250u, null, delegate
            {
                done2.SetResult(result: true);
            });
        }

        private void DismissPopupPopInEffect(TaskCompletionSource<bool> done)
        {
            TaskCompletionSource<bool> done2 = done;
            this.Animate("BackgroundFadeOut", delegate (double v)
            {
                base.Background = new SolidColorBrush(Colors.Black.WithAlpha((float)v));
            }, 0.4, 0.0, 16u, 250u, Easing.Linear);
            double end = 0.8;
            base.Content.Animate("ContentFadeOut", new Animation(delegate (double v)
            {
                base.Content.Opacity = v;
            }, 1.0, 0.0, Easing.CubicInOut), 16u, 300u);
            base.Content.Animate("ContentScaleOut", new Animation(delegate (double v)
            {
                base.Content.Scale = v;
            }, 1.0, end, Easing.CubicInOut), 16u, 300u, null, delegate
            {
                done2.SetResult(result: true);
            });
        }

        protected void OnPopupClosedInternal(EventArgs e)
        {
            this.PopupClosed?.Invoke(this, e);
        }
    }
}
