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
    public class Plr : Entity
    {
        private int _lives, _score, _shotsFired, _effectiveShots;

        public int Lives
        {
            get 
            { 
                return _lives; 
            }
            set
            {
                if (value < 0)
                {
                    _lives = 0;
                }
                else
                    _lives = value;
            }
        }

        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                if (value < 0)
                {
                    _score = 0;
                }
                else
                    _score = value;
            }
        }

        public int ShotsFired
        {
            get
            {
                return _shotsFired;
            }
            set
            {
                if (value < 0)
                {
                    _shotsFired = 0;
                }
                else
                    _shotsFired = value;
            }
        }

        public int EffectiveShots
        {
            get
            {
                return _effectiveShots;
            }
            set
            {
                if (value < 0)
                {
                    _effectiveShots = 0;
                }
                else
                    _effectiveShots = value;
            }
        }

        public Plr(double hght, double wdth, int mSpeed, int projSpeed)
            : base(hght, wdth, mSpeed, projSpeed)
        {
            hitBox.Tag = "Player";
            hitBox.Fill = Brushes.Green;
            _lives = 0;
            _score = 0;
            _shotsFired = 0;
        }

        public Plr(double hght, double wdth, int mSpeed, int projSpeed, int lives, int score, ImageBrush plrSkin)
            : base(hght, wdth, mSpeed, projSpeed)
        {
            hitBox.Tag = "Player";
            hitBox.Fill = plrSkin;
            Lives = lives;
            Score = score;
            _shotsFired = 0;
        }

        ~Plr()
        {

        }

        public void AddLife()
        {
            _lives++;
        }

        public void RemoveLife()
        {
            _lives--;
        }

        public void AddScore(int tmp)
        {
            _score += tmp;
        }

        public bool IsDead()
        {
            if (_lives <= 0)
                return true;
            else
                return false;
        }
    }
}
