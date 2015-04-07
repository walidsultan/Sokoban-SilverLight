using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Sokoban
{
    public class Path 
    {
        public List<Direction> Directions=new List<Direction>();

        public Path Clone()
        {
            Path clonedPath = new Path();

            foreach (Direction direction in Directions)
            {
                clonedPath.Directions.Add(direction);
            }

            return clonedPath;
        }
    }
}
