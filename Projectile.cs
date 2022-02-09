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
    public class Projectile : baseObj
    {
        private int _damage;
        
        public int Damage
        {
            get
            {
                return _damage;
            }
            set
            {
                if (value <= 0)
                {
                    _damage = 1;
                }
                else
                {
                    _damage = value;
                }
            }
        }

        public bool IsPlayerProjectile;

        public Projectile(double hght, double wdth, int mSpeed)
            : base(hght, wdth, mSpeed)
        {
            _damage = 1;
            IsPlayerProjectile = false;
        }

        public Projectile(double hght, double wdth, int mSpeed, int projDamage, ImageBrush projSkin)
            : base(hght, wdth, mSpeed)
        {
            Damage = projDamage;
            IsPlayerProjectile = false;

            hitBox.Fill = projSkin;
        }

        public Projectile(double hght, double wdth, int mSpeed, int projDamage, bool isPlrProj, ImageBrush projSkin)
            : base(hght, wdth, mSpeed)
        {
            Damage = projDamage;
            IsPlayerProjectile = isPlrProj;
            
            hitBox.Fill = projSkin;
        }
    }
}
