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
    public class Explosion : baseObj
    {
        private int _currentFrame;

        public int CurrentFrame
        {
            get
            {
                return _currentFrame;
            }
            set
            {
                if (value < 0)
                {
                    _currentFrame = 0;
                }
                else
                {
                    _currentFrame = value;
                }
            }
        }

        ImageBrush SpriteImg;
        BitmapSource SpritesheetBISrc;
        CroppedBitmap[] SpriteCrop;

        public Explosion(double hght, double wdth, BitmapSource spriteSheet, SpritesheetData ssData)
            :base(hght, wdth, 0)
        {
            SpriteImg = new ImageBrush();
            SpritesheetBISrc = spriteSheet;
            SpriteCrop = new CroppedBitmap[ssData.ExplosionNumbSprites];
            for (int i = 0; i < ssData.ExplosionNumbSprites; ++i)
            {
                SpriteCrop[i] = new CroppedBitmap(SpritesheetBISrc, ssData.ExplosionSprites[i]);
            }
            SpriteImg.ImageSource = SpriteCrop[0];
            hitBox.Fill = SpriteImg;
        }

        ~Explosion()
        {

        }

        public bool IsFinished(SpritesheetData ssData)
        {
            ++_currentFrame;
            if (_currentFrame == ssData.ExplosionNumbSprites)
            {
                return true;
            }
            else
            {
                SpriteImg.ImageSource = SpriteCrop[_currentFrame];
                hitBox.Fill = SpriteImg;
                return false;
            }
        }

        public void RemoveExplosion(ref List<Rectangle> hitboxGC, ref List<Explosion> explosionGC)
        {
            hitboxGC.Add(this.hitBox);
            explosionGC.Add(this);
        }
    }
}
