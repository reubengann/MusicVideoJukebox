using MusicVideoJukebox.Core.UserInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MusicVideoJukebox
{
    public class InterfaceFader : IFadesWhenInactive
    {
        private readonly List<FrameworkElement> elements;
        private readonly DependencyProperty opacityProperty;
        private readonly DispatcherTimer _timer;
        private bool fadingOut = false;
        private bool fadingIn = false;
        private bool enabled = true;
        private bool sidebarShown = false;

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
            _timer.Stop();
            if (fadingOut) return;
            var fadeOutAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.5) // Adjust the duration as needed
            };

            fadeOutAnimation.Completed += (s, e) => { 
                fadingOut = false; 
                VisibilityChanged?.Invoke(this, new VisibilityChangedEventArgs(false)); 
                sidebarShown = false;
                foreach (var element in elements)
                {
                    element.Visibility = Visibility.Collapsed;
                }
            };
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
            if (fadingOut)
            {
                //Debug.WriteLine("MaybeFadeIn and currently fading out");
                fadingOut = false;
            }
            if (fadingIn)
            {
                //Debug.WriteLine("Fading in already so ignoring input");
                return;
            }
            if (sidebarShown)
            {
                //Debug.WriteLine("Sidebar already shown so ignoring input");
                return;
            };

            sidebarShown = true;

            _timer.Stop();
            _timer.Start();

            foreach (var element in elements)
            {
                element.Visibility = Visibility.Visible;
            }
            var fadeInAnimation = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(0.25)
            };

            VisibilityChanged?.Invoke(this, new VisibilityChangedEventArgs(true));

            fadeInAnimation.Completed += (s, e) =>
            {
                //Debug.WriteLine("Completed fade out of sidebar");
                fadingIn = false;
            };
            fadingIn = true;
            foreach (var element in elements)
            {
                element.BeginAnimation(opacityProperty, fadeInAnimation);
            }
        }
    }
}
