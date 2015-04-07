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
using System.Windows.Media.Imaging;

namespace Sokoban.Controls
{
    public partial class ModalPopup : UserControl
    {
        App _Application = (App)Application.Current;

        public ModalPopup()
        {
            InitializeComponent();

            sbContinue.Begin();
        }

        private void btnClosePopup_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed ;
        }

        private void btnClosePopup_MouseEnter(object sender, MouseEventArgs e)
        {
            imgClosePopup.Source = new BitmapImage(new Uri("../images/x2.png",UriKind.Relative ));
        }

        private void btnClosePopup_MouseLeave(object sender, MouseEventArgs e)
        {
            imgClosePopup.Source = new BitmapImage(new Uri("../images/x.png", UriKind.Relative));
        }
    }
}
