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
    public partial class Player : UserControl, IBlockAnimation 
    {
    

        public Player()
        {
            InitializeComponent();
        }

        private Position _Position = new Position();
        private DoubleAnimation _HorizontalTimeLine = new DoubleAnimation();
        private DoubleAnimation _VerticalTimeLine = new DoubleAnimation();

        #region IBlockPosition Members

        public Position position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }

        public DoubleAnimation HorizontalTimeLine
        {
            get
            {
                return _HorizontalTimeLine;
            }
            set
            {
                _HorizontalTimeLine = value;
            }
        }

        public DoubleAnimation VerticalTimeLine
        {
            get
            {
                return _VerticalTimeLine;
            }
            set
            {
                _VerticalTimeLine  = value;
            }
        }
        #endregion
    }
}
