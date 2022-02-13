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
    public abstract class Entity : baseObj
    {
        private int _projectileSpeed;

        public int ProjectileSpeed
        {
            get
            {
                return _projectileSpeed;
            }
            set
            {
                if (value <= 0)
                    _projectileSpeed = 1;
                else
                    _projectileSpeed = value;
            }
        }

        public Rectangle hitBox;

        public Entity(double hght, double wdth, int mSpeed, int projSpeed)
            : base(hght, wdth, mSpeed)
        {
            //Height = hght;
            //Width = wdth;
            MovSpeed = mSpeed;
            ProjectileSpeed = projSpeed;

            hitBox = new Rectangle()
            {
                Tag = "EntityHitBox",
                Width = wdth,
                Height = hght,
                Fill = Brushes.Red
            };
        }

        ~Entity()
        {

        }
    }
}
