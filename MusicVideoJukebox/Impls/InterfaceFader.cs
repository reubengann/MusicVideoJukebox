using MusicVideoJukebox.Core.UserInterface;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MusicVideoJukebox
{
    public class InterfaceFader : IFadesWhenInactive
    {
        private readonly List<FrameworkElement> elements;
        private readonly DependencyProperty opacityProperty;
        DispatcherTimer _timer;
        private bool fadingOut = false;
        private bool fadingIn = false;
        private bool enabled = true;

        public event EventHandler<VisibilityChangedEventArgs>? VisibilityChanged;

        public InterfaceFader(List<FrameworkElement> elements, DependencyProperty opacityProperty)
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            _timer.Tick += _timer_Tick;
            this.elements = elements;
            this.opacityProperty = opacityProperty;
            _timer.Start();
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            FadeButtonsOut();
        }

        private void FadeButtonsOut()
        {
            if (fadingOut) return;
            var fadeOutAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5) // Adjust the duration as needed
            };

            fadeOutAnimation.Completed += (s, e) => { fadingOut = false; VisibilityChanged?.Invoke(this, new VisibilityChangedEventArgs(false)); };
            fadingOut = true;
            foreach (var element in elements)
            {
                element.BeginAnimation(opacityProperty, fadeOutAnimation);
            }
        }

        public void DisableFading()
        {
            enabled = false;
            _timer.Stop();
        }

        public void EnableFading()
        {
            enabled = true;
            _timer.Stop();
            _timer.Start();
        }

        public void UserInteracted()
        {
            if (!enabled) return;
            MaybeFadeButtonsIn();
        }

        private void MaybeFadeButtonsIn()
        {
            if (fadingOut) fadingOut = false;
            if (fadingIn) return;

            var fadeInAnimation = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(0.25)
            };

            VisibilityChanged?.Invoke(this, new VisibilityChangedEventArgs(true));

            fadeInAnimation.Completed += (s, e) => fadingIn = false;
            fadingIn = true;
            foreach (var element in elements)
            {
                element.BeginAnimation(opacityProperty, fadeInAnimation);
            }
        }
    }
}
