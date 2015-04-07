using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Sokoban.Controls
{
    public partial class AuthenticationMode : UserControl
    {
        public LoginWindow _LoginPopup = new LoginWindow();

        public delegate void AuthenticationModeClosedHandler();
        public event AuthenticationModeClosedHandler AuthenticationModeClosed;

        public AuthenticationMode()
        {
            InitializeComponent();
        }

        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;

            _LoginPopup.Title = "Login";
            _LoginPopup.tbConfirmPassword.Visibility = Visibility.Collapsed;
            _LoginPopup.txtConfirmPassword.Visibility = Visibility.Collapsed;
            _LoginPopup.rowConfirmPassword.Height = new GridLength(0);
            _LoginPopup.Show();
        }

        private void CloseAuthenticationMode()
        {
            if (AuthenticationModeClosed != null)
            {
                AuthenticationModeClosed();
            }
        }

        private void Button_Guest_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;

            CloseAuthenticationMode();
        }
    }
}
