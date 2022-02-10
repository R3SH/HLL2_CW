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
    public enum EnemyTypeList
    {
        Drone,
        Alien,
        Enforcer,
        Overseer
    }

    public class bEnemy : Entity
    {
        private int _health, _currentFrame;
        private bool _movingLeft;

        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                if (value <= 0)
                {
                    _health = 1;
                }
                else
                {
                    _health = value;
                }
            }
        }

        //private int _numberOfFrames;
        //public int NumberOfFrames
        //{
        //    get
        //    {
        //        return _numberOfFrames;
        //    }
        //    set
        //    {
        //        if (value <= 0)
        //        {
        //            _numberOfFrames = 1;
        //        }
        //        else
        //        {
        //            _numberOfFrames = value;
        //        }
        //    }
        //}

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

        public EnemyTypeList EnemyType;

        ImageBrush SpriteImg;
        BitmapSource SpritesheetBISrc;
        CroppedBitmap SpriteCrop;

        public bEnemy(double hght, double wdth, int mSpeed, int projSpeed)
            : base(hght, wdth, mSpeed, projSpeed)
        {
            hitBox.Tag = "baseEnemy";
            hitBox.Fill = Brushes.Yellow;
            _health = 1;
            //_numberOfFrames = 1;
            _movingLeft = true;
            _currentFrame = 0;
        }

        public bEnemy(double hght, double wdth, int mSpeed, int projSpeed, int hp, EnemyTypeList type, BitmapSource spriteSheet, SpritesheetData ssData)
            : base(hght, wdth, mSpeed, projSpeed)
        {
            hitBox.Tag = "baseEnemy";
            //hitBox.Fill = enemySkin;
            Health = hp;
            _movingLeft = true;
            _currentFrame = 0;
            //NumberOfFrames = frameN;
            EnemyType = type;

            SpriteImg = new ImageBrush();
            //SpriteImg.Width = wdth;
            SpritesheetBISrc = spriteSheet;

            switch (EnemyType)
            {
                case EnemyTypeList.Drone:
                    hitBox.Tag = "Drone";
                    SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.DroneSprites[0]);
                    break;
                case EnemyTypeList.Alien:
                    hitBox.Tag = "Alien";
                    SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.AlienSprites[0]);
                    break;
                case EnemyTypeList.Enforcer:
                    hitBox.Tag = "Enforcer";
                    SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.EnforcerSprites[0]);
                    break;
                case EnemyTypeList.Overseer:
                    hitBox.Tag = "Overseer";
                    //SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.DroneSprites[0]);
                    break;
                default:
                    break;
            }

            SpriteImg.ImageSource = SpriteCrop;
            hitBox.Fill = SpriteImg;
        }

        public bool IsMovingLeft()
        {
            return _movingLeft;
        }

        public void changeMovDir()
        {
            MovSpeed = -MovSpeed;
        }

        public void UpdateFrame(SpritesheetData ssData)
        {
            ++_currentFrame;

            switch (EnemyType)
            {
                case EnemyTypeList.Drone:
                    if (_currentFrame >= ssData.DroneNumbSprites)
                        _currentFrame = 0;
                    SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.DroneSprites[_currentFrame]);
                    break;
                case EnemyTypeList.Alien:
                    if (_currentFrame >= ssData.AlienNumbSprites)
                        _currentFrame = 0;
                    SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.AlienSprites[_currentFrame]);
                    break;
                case EnemyTypeList.Enforcer:
                    if (_currentFrame >= ssData.EnforcerNumbSprites)
                        _currentFrame = 0;
                    SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.EnforcerSprites[_currentFrame]);
                    break;
                case EnemyTypeList.Overseer:
                    //if (_currentFrame >= ssData.Ov)
                    //    _currentFrame = 0;
                    //SpriteCrop = new CroppedBitmap(SpritesheetBISrc, ssData.DroneSprites[_currentFrame]);
                    break;
                default:
                    break;
            }

            SpriteImg.ImageSource = SpriteCrop;
            hitBox.Fill = SpriteImg;
        }
    }
}
