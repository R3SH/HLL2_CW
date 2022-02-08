using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;

namespace CW_HLL2
{
    public abstract class Entity
    {
        //private double _height, _width;
        //public double Height
        //{
        //    get
        //    {
        //        return _height;
        //    }
        //    set
        //    {
        //        if (value <= 0)
        //            _height = 10;
        //        else
        //            _height = value;
        //    }
        //}
        //public double Width
        //{
        //    get
        //    {
        //        return _width;
        //    }
        //    set
        //    {
        //        if (value <= 0)
        //            _width = 10;
        //        else
        //            _width = value;
        //    }
        //}

        private int _movSpeed, _projectileSpeed;
        public int MovSpeed
        {
            get
            {
                return _movSpeed;
            }
            set
            {
                if (value <= 0)
                    _movSpeed = 1;
                else
                    _movSpeed = value;
            }
        }
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
    }
}
