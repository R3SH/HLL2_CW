using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CW_HLL2
{
    public abstract class baseObj
    {
        private int _movSpeed;
        public int MovSpeed
        {
            get
            {
                return _movSpeed;
            }
            set
            {
                _movSpeed = value;
            }
        }

        public Rectangle hitBox;

        public baseObj(double hght, double wdth, int mSpeed)
        {
            MovSpeed = mSpeed;

            hitBox = new Rectangle()
            {
                Tag = "baseObjHitBox",
                Width = wdth,
                Height = hght,
                Fill = Brushes.White
            };
        }
    }
}
