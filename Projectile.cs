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
        private int _damage, _currentFrame;
        
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

        public bool IsPlayerProjectile;

        EnemyTypeList EnemyType;

        ImageBrush SpriteImg;
        BitmapSource SpritesheetBISrc;
        CroppedBitmap[] SpriteCrop;

        public Projectile(double hght, double wdth, int mSpeed)
            : base(hght, wdth, mSpeed)
        {
            _damage = 1;
            _currentFrame = 0;
            IsPlayerProjectile = false;
        }

        public Projectile(double hght, double wdth, int mSpeed, int projDamage, EnemyTypeList enType, BitmapSource spriteSheet, SpritesheetData ssData)
            : base(hght, wdth, mSpeed)
        {
            Damage = projDamage;
            _currentFrame = 0;
            IsPlayerProjectile = false;
            EnemyType = enType;

            SpriteImg = new ImageBrush();
            SpritesheetBISrc = spriteSheet;

            switch (EnemyType)
            {
                case EnemyTypeList.Drone:
                    hitBox.Tag = "DrProjectile";
                    //SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.DrProjectileSprites[0]);
                    SpriteCrop = new CroppedBitmap[ssData.DrProjectileNumbSprites];
                    for (int i = 0; i < ssData.DrProjectileNumbSprites; ++i)
                    {
                        SpriteCrop[i] = new CroppedBitmap(SpritesheetBISrc, ssData.DrProjectileSprites[i]);
                    }
                    break;
                case EnemyTypeList.Alien:
                    hitBox.Tag = "AlProjectile";
                    //SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.AlProjectileSprites[0]);
                    SpriteCrop = new CroppedBitmap[ssData.AlProjectileNumbSprites];
                    for (int i = 0; i < ssData.AlProjectileNumbSprites; ++i)
                    {
                        SpriteCrop[i] = new CroppedBitmap(SpritesheetBISrc, ssData.AlProjectileSprites[i]);
                    }
                    break;
                case EnemyTypeList.Enforcer:
                    hitBox.Tag = "EnforProjectile";
                    //SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.EnforProjectileSprites[0]);
                    SpriteCrop = new CroppedBitmap[ssData.EnforProjectileNumbSprites];
                    for (int i = 0; i < ssData.EnforProjectileNumbSprites; ++i)
                    {
                        SpriteCrop[i] = new CroppedBitmap(SpritesheetBISrc, ssData.EnforProjectileSprites[i]);
                    }
                    break;
                default:
                    hitBox.Tag = "UndefinedProjectile";
                    hitBox.Fill = Brushes.Red;
                    break;
            }

            SpriteImg.ImageSource = SpriteCrop[0];
            hitBox.Fill = SpriteImg;
        }

        public Projectile(double hght, double wdth, int mSpeed, int projDamage, bool isPlrProj, ImageBrush projSkin)
            : base(hght, wdth, mSpeed)
        {
            Damage = projDamage;
            _currentFrame = 0;
            IsPlayerProjectile = isPlrProj;
            
            hitBox.Fill = projSkin;
        }

        ~Projectile()
        {

        }

        public void UpdateFrame(SpritesheetData ssData)
        {
            ++_currentFrame;

            switch (EnemyType)
            {
                case EnemyTypeList.Drone:
                    if (_currentFrame >= ssData.DrProjectileNumbSprites)
                        _currentFrame = 0;
                    //SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.DrProjectileSprites[_currentFrame]);
                    break;
                case EnemyTypeList.Alien:
                    if (_currentFrame >= ssData.AlProjectileNumbSprites)
                        _currentFrame = 0;
                    //SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.AlProjectileSprites[_currentFrame]);
                    break;
                case EnemyTypeList.Enforcer:
                    if (_currentFrame >= ssData.EnforProjectileNumbSprites)
                        _currentFrame = 0;
                    //SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.EnforProjectileSprites[_currentFrame]);
                    break;
                default:
                    break;
            }

            SpriteImg.ImageSource = SpriteCrop[_currentFrame];
            hitBox.Fill = SpriteImg;
        }

        public void RemoveProjectile(ref List<Rectangle> projectileGC, ref List<Projectile> projectileList)
        {
            projectileGC.Add(this.hitBox);
            projectileList.Add(this);
        }
    }
}
