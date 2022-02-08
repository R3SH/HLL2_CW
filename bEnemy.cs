using System;
using System.Collections.Generic;
using System.Text;

namespace CW_HLL2
{
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

        public bEnemy(double hght, double wdth, int mSpeed, int projSpeed)
            : base(hght, wdth, mSpeed, projSpeed)
        {
            hitBox.Tag = "baseEnemy";
            _health = 1;
            _movingLeft = true;
        }

        public bEnemy(double hght, double wdth, int mSpeed, int projSpeed, int hp)
            : base(hght, wdth, mSpeed, projSpeed)
        {
            hitBox.Tag = "baseEnemy";
            Health = hp;
            _movingLeft = true;
        }

        public bool IsMovingLeft()
        {
            return _movingLeft;
        }

        public void changeMovDir()
        {
            _movingLeft = !_movingLeft;
        }
    }
}
