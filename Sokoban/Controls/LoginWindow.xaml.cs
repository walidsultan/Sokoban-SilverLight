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
using Sokoban.Web.Services;
using System.ServiceModel.DomainServices.Client;
using Sokoban.DataTypes;
using Sokoban.Classes;
namespace Sokoban
{
    public partial class LoginWindow : ChildWindow
    {
        App _Application = (App)Application.Current;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            activityAuthentication.IsActive = true;

            if (btnOk.Content.ToString() == "Sign Up")
            {
                InsertPlayerAsynchronous();
            }
            else if (btnOk.Content.ToString() == "Sign In")
            {
                LoginPlayerAsynchronous();
            }
        }

        private void LoginPlayerAsynchronous()
        {
            StaticFields.SokobanContext.Users.Clear();
            LoadOperation loadLoginOperation = StaticFields.SokobanContext.Load(StaticFields.SokobanContext.GetUsersWithUsernameQuery(txtUsername.Text));
            loadLoginOperation.Completed += new EventHandler(loadLoginOperation_Completed);
        }

        void loadLoginOperation_Completed(object sender, EventArgs e)
        {
            activityAuthentication.IsActive = false;

            validationSummary.Errors.Clear();

            if (StaticFields.SokobanContext.Users.Count > 0)
            {
                User  currentPlayer = StaticFields.SokobanContext.Users.SingleOrDefault();
               if (currentPlayer.Password == txtPassword.Password)
               {
                   MainPage.CurrentPlayer = currentPlayer;
                   GetPlayerProgressAsynchronous();
                   return;
               }
            }

            validationSummary.Errors.Add(new ValidationSummaryItem("Wrong username or password."));
        }

        private void GetPlayerProgressAsynchronous()
        {
            StaticFields.SokobanContext.Progresses.Clear();
            LoadOperation loadPlayerProgressOperation = StaticFields.SokobanContext.Load(StaticFields.SokobanContext.GetProgressByUserIdQuery(MainPage.CurrentPlayer.Id));
            loadPlayerProgressOperation.Completed += new EventHandler(loadPlayerProgressOperation_Completed);
        }

        void loadPlayerProgressOperation_Completed(object sender, EventArgs e)
        {
            MainPage.PlayerProgress =StaticFields.SokobanContext.Progresses;
            this.Close();
        }

        void submitOperation_Completed(object sender, EventArgs e)
        {
            activityAuthentication.IsActive = false;
            this.Close();
        }

        private void InsertPlayerAsynchronous()
        {
            StaticFields.SokobanContext.Users.Clear();
            LoadOperation loadSignUpOperation = StaticFields.SokobanContext.Load(StaticFields.SokobanContext.GetUsersWithUsernameQuery(txtUsername.Text));
            loadSignUpOperation.Completed += new EventHandler(loadSignUpOperation_Completed);
        }

        void loadSignUpOperation_Completed(object sender, EventArgs e)
        {
            //Validate user input
            validationSummary.Errors.Clear();
            bool isValid = true;

            if (StaticFields.SokobanContext.Users.Count > 0)
            {
                validationSummary.Errors.Add(new ValidationSummaryItem("The username is already used."));
                isValid = false;
            }

            if (txtUsername.Text.Trim()==string.Empty )
            {
                validationSummary.Errors.Add(new ValidationSummaryItem("Username is required."));
                isValid = false;
            }

            if (txtPassword .Password.Trim()== string.Empty)
            {
                validationSummary.Errors.Add(new ValidationSummaryItem("Password is required."));
                isValid = false;
            }

            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                validationSummary.Errors.Add(new ValidationSummaryItem("The passwords doesn't match."));
                isValid = false;
            }

            //Insert player
            if (isValid)
            {
                User  newPlayer = new User ();
                newPlayer.Username = txtUsername.Text;
                newPlayer.Password = txtPassword.Password;
                if (Application.Current.IsRunningOutOfBrowser)
                {
                    newPlayer.IpAddress = "OOB";
                }
                else
                {
                    newPlayer.IpAddress = _Application.DeploymentConfigurations["IpAddress"];
                }
                newPlayer.Score = 0;

                StaticFields.SokobanContext = new SokobanContext();
                StaticFields.SokobanContext.Users.Add(newPlayer);
                SubmitOperation submitOperation = StaticFields.SokobanContext.SubmitChanges();
                submitOperation.Completed += new EventHandler(submitOperation_Completed);

                MainPage.CurrentPlayer = newPlayer;
            }
            else
            {
                activityAuthentication.IsActive = false;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void hlnkSignUp_Click(object sender, RoutedEventArgs e)
        {
            sbSignUp.Begin();

            ResetAllFields();

            if (hlnkSignUp.Content.ToString() == "Sign Up")
            {
                tbConfirmPassword.Visibility = Visibility.Visible;
                txtConfirmPassword.Visibility = Visibility.Visible;
                rowConfirmPassword.Height = GridLength.Auto;
                btnOk.Content = "Sign Up";
                hlnkSignUp.Content = "Login";
                this.Title = "Sign Up";
            }
            else
            {
                tbConfirmPassword.Visibility = Visibility.Collapsed;
                txtConfirmPassword.Visibility = Visibility.Collapsed;
                rowConfirmPassword.Height = new GridLength(0);
                btnOk.Content = "Sign In";
                hlnkSignUp.Content = "Sign Up";
                this.Title = "Login";
            }
        }

        private void ResetAllFields()
        {
            txtConfirmPassword.Password = string.Empty;
            txtPassword.Password = string.Empty;
            txtUsername.Text = string.Empty;
            validationSummary.Errors.Clear();
        }
    }
}

