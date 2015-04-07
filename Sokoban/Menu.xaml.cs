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
using Sokoban.Controls;

namespace Sokoban
{
    public partial class Menu : UserControl
    {
        AuthenticationMode _AuthenticationMode = new AuthenticationMode();

        public Menu(bool showAuthenticationMode)
        {
            InitializeComponent();

            try
            {
                //Adjust menu BackGround
                SetBackGroundDimensions();

                if (showAuthenticationMode)
                {
                    //Show Authentication type
                    _AuthenticationMode.SetValue(Grid.RowSpanProperty, 3);
                    _AuthenticationMode.SetValue(Grid.ColumnSpanProperty, 3);
                    _AuthenticationMode.lblModalPopupTitle.Content = "Choose Authentication Mode";
                    grdLayoutRoot.Children.Add(_AuthenticationMode);

                    _AuthenticationMode._LoginPopup.Closed += new EventHandler(_LoginPopup_Closed);
                    _AuthenticationMode.AuthenticationModeClosed += new AuthenticationMode.AuthenticationModeClosedHandler(authenticationMode_AuthenticationModeClosed);
                }
                else
                {
                    CheckPlayerProgress();
                    SetWelcomeMessage();
                }

                //Remove install button if running out of the browser
                if (Application.Current.IsRunningOutOfBrowser)
                {
                    btnInstall.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetBackGroundDimensions()
        {
            if (BrowserScreenInformation.ClientHeight <= BrowserScreenInformation.ClientWidth)
            {
                grdLayoutRoot.Width = BrowserScreenInformation.ClientHeight / 1024 * 1280;
                grdLayoutRoot.Height = BrowserScreenInformation.ClientHeight;
            }
            else
            {
                grdLayoutRoot.Height = BrowserScreenInformation.ClientWidth / 1280 * 1024;
                grdLayoutRoot.Width = BrowserScreenInformation.ClientWidth;
            }
        }

        void authenticationMode_AuthenticationModeClosed()
        {
            SetWelcomeMessage(); 
        }

        private void SetWelcomeMessage()
        {
            if (MainPage.CurrentPlayer != null)
            {
                tbLoggedUser.Text = "Welcome " + MainPage.CurrentPlayer.Username;
                hlnkSignUp.Visibility = Visibility.Collapsed;
            }
            else
            {
                hlnkSignUp.Visibility = Visibility.Visible;
            }
        }

        void _LoginPopup_Closed(object sender, EventArgs e)
        {
            CheckPlayerProgress();
            SetWelcomeMessage();
        }

        private void CheckPlayerProgress()
        {
            if (MainPage.PlayerProgress != null && MainPage.PlayerProgress.Count > 0)
            {
                btnContinue.IsEnabled = true;
            }
        }


        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            PageSwitcher pageSwticher = (PageSwitcher)this.Parent;
            pageSwticher.Navigate(new MainPage(0));
         }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            PageSwitcher pageSwticher = (PageSwitcher)this.Parent;
            int levelIndex=MainPage.PlayerProgress.Last().LevelIndex+1;
            MainPage mainPage = new MainPage(levelIndex);
            pageSwticher.Navigate(mainPage );
        }

        private void btnRankings_Click(object sender, RoutedEventArgs e)
        {
            PageSwitcher pageSwticher = (PageSwitcher)this.Parent;
            pageSwticher.Navigate(new Ranking());
        }

        private void hlnkSignUp_Click(object sender, RoutedEventArgs e)
        {
            ResetSignInForm();
            _AuthenticationMode._LoginPopup.Title = "Login";
            _AuthenticationMode._LoginPopup.tbConfirmPassword.Visibility = Visibility.Collapsed;
            _AuthenticationMode._LoginPopup.txtConfirmPassword.Visibility = Visibility.Collapsed;
            _AuthenticationMode._LoginPopup.rowConfirmPassword.Height = new GridLength(0);
            _AuthenticationMode._LoginPopup.Show();
            _AuthenticationMode._LoginPopup.Closed += new EventHandler(_LoginPopup_Closed);
        }

        private void ResetSignInForm()
        {
            _AuthenticationMode._LoginPopup.txtUsername  .Text  = string.Empty;
            _AuthenticationMode._LoginPopup.txtPassword.Password  = string.Empty;
            _AuthenticationMode._LoginPopup.txtConfirmPassword.Password = string.Empty;
            _AuthenticationMode._LoginPopup.validationSummary.Errors.Clear();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetBackGroundDimensions();
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.InstallState == InstallState.Installed)
            {
                ModalPopup  applicationInstalled = new ModalPopup();
                applicationInstalled.lblModalPopupTitle.Content   = "Power Sweeper";
                applicationInstalled.lblModalPopoupContent .Content  = "Application is already installed";
                applicationInstalled.Visibility = Visibility.Visible;
            }
            else
            {
                Application.Current.Install();
            }
        }
    }
}
