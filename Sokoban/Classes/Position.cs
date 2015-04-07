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

namespace Sokoban
{
    public class Position : IComparable
    {
        public double Left;
        public double Top;

        public Position Clone()
        {
            Position position = new Position();
            position.Left = Left;
            position.Top = Top;
            return position;
        }

        #region IComparable Members

        /// <summary>
        /// Return 0 if not equal otherwise return 1
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Position otherPosition = (Position)obj;
            if (otherPosition != null)
            {
                if (otherPosition.Left == this.Left && otherPosition.Top == this.Top)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                throw new ArgumentException("Object is not a Position");
            }
        }

        #endregion
    }
}
