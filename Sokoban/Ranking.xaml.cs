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
using System.Windows.Navigation;
using Sokoban.Web.Services;
using System.ServiceModel.DomainServices.Client;
using Sokoban.DataTypes;
using Sokoban.Classes;
namespace Sokoban
{
    public partial class Ranking : Page
    {
        public Ranking()
        {
            InitializeComponent();

            SetLevelDimensions();

            activityLoadingUsers.IsActive = true;

            Loaded += new RoutedEventHandler(Ranking_Loaded);

        }

        private void SetLevelDimensions()
        {
            //Adjust menu BackGround
            if (BrowserScreenInformation.ClientHeight <= BrowserScreenInformation.ClientWidth)
            {
                grdLayout.Width = BrowserScreenInformation.ClientHeight / 1024 * 1280;
                grdLayout.Height = BrowserScreenInformation.ClientHeight;
            }
            else
            {
                grdLayout.Height = BrowserScreenInformation.ClientWidth / 1280 * 1024;
                grdLayout.Width = BrowserScreenInformation.ClientWidth;
            }
        }

        void Ranking_Loaded(object sender, RoutedEventArgs e)
        {
            grdRanking.ItemsSource = StaticFields.SokobanContext.UserRankings;
            LoadOperation  loadTopTenUsersOperation= StaticFields.SokobanContext.Load(StaticFields.SokobanContext.GetUsersRankingQuery ());
            loadTopTenUsersOperation.Completed += new EventHandler(loadTopTenUsersOperation_Completed);
        }

        void loadTopTenUsersOperation_Completed(object sender, EventArgs e)
        {
            activityLoadingUsers.IsActive = false ;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            PageSwitcher pageSwticher = (PageSwitcher)this.Parent;
            pageSwticher.Navigate(new Menu (false ));
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetLevelDimensions();
        }

    }
}
