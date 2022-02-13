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
    class Barrier : baseObj
    {
        private int _health, _initialHealth;

        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                if (value < 0)
                {
                    _health = 0;
                }
                else
                {
                    _health = value;
                }
            }
        }

        public int InitialHealth
        {
            get
            {
                return _initialHealth;
            }
            set
            {
                if (value <= 0)
                {
                    _initialHealth = 1;
                }
                else
                {
                    _initialHealth = value;
                }
            }
        }

        ImageBrush SpriteImg;
        BitmapSource SpritesheetBISrc;
        CroppedBitmap[] SpriteCrop;

        public Barrier(double hght, double wdth)
            : base(hght, wdth, 0)
        {
            _health = 1;
            _initialHealth = 1;
            hitBox.Tag = "Barrier";
        }

        public Barrier(double hght, double wdth, int health, BitmapSource spriteSheet, SpritesheetData ssData)
            : base(hght, wdth, 0)
        {
            InitialHealth = health;
            Health = health;
            SpriteImg = new ImageBrush();
            SpritesheetBISrc = spriteSheet;

            hitBox.Tag = "Barrier";
            SpriteCrop = new CroppedBitmap[ssData.BarrierNumbSprites];
            for (int i = 0; i < ssData.BarrierNumbSprites; ++i)
            {
                SpriteCrop[i] = new CroppedBitmap(SpritesheetBISrc, ssData.BarrierSprites[i]);
            }

            SpriteImg.ImageSource = SpriteCrop[0];
            hitBox.Fill = SpriteImg;
        }

        ~Barrier()
        {

        }

        public void Hit(int damage, SpritesheetData ssData)
        {
            int tmp;

            Health -= damage;

            if (_health > 0)
            {
                tmp = (ssData.BarrierNumbSprites - 1) - Health / (InitialHealth / ssData.BarrierNumbSprites);
                SpriteImg.ImageSource = SpriteCrop[tmp];
                hitBox.Fill = SpriteImg;
            }
        }

        public void RemoveBarrier(ref List<Rectangle> barrierHitboxGC, ref List<Barrier> barrierListGC)
        {
            barrierHitboxGC.Add(this.hitBox);
            barrierListGC.Add(this);
        }
    }
}
