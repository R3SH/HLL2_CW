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
        private int _health;
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

        public EnemyTypeList EnemyType;

        public bEnemy(double hght, double wdth, int mSpeed, int projSpeed)
            : base(hght, wdth, mSpeed, projSpeed)
        {
            hitBox.Tag = "baseEnemy";
            hitBox.Fill = Brushes.Yellow;
            _health = 1;
            _movingLeft = true;
        }

        public bEnemy(double hght, double wdth, int mSpeed, int projSpeed, int hp, EnemyTypeList type, ImageBrush enemySkin)
            : base(hght, wdth, mSpeed, projSpeed)
        {
            hitBox.Tag = "baseEnemy";
            hitBox.Fill = enemySkin;
            Health = hp;
            _movingLeft = true;


            //Redundant?
            switch (type)
            {
                case EnemyTypeList.Drone:
                    hitBox.Tag = "Drone";

                    break;
                case EnemyTypeList.Alien:
                    hitBox.Tag = "Alien";

                    break;
                case EnemyTypeList.Enforcer:
                    hitBox.Tag = "Enforcer";

                    break;
                case EnemyTypeList.Overseer:
                    hitBox.Tag = "Overseer";

                    break;
                default:
                    break;
            }
        }

        public bool IsMovingLeft()
        {
            return _movingLeft;
        }

        public void changeMovDir()
        {
            MovSpeed = -MovSpeed;
        }
    }
}
