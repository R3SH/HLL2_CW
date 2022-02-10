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
    public class SpritesheetData
    {
        public readonly int DroneNumbSprites;
        public readonly Int32Rect[] DroneSprites;
        public readonly int AlienNumbSprites;
        public readonly Int32Rect[] AlienSprites;
        public readonly int EnforcerNumbSprites;
        public readonly Int32Rect[] EnforcerSprites;
        public readonly int DrProjectileNumbSprites;
        public readonly Int32Rect[] DrProjectileSprites;
        public readonly int AlProjectileNumbSprites;
        public readonly Int32Rect[] AlProjectileSprites;
        public readonly int EnforProjectileNumbSprites;
        public readonly Int32Rect[] EnforProjectileSprites;
        public readonly int ExplosionNumbSprites;
        public readonly Int32Rect[] ExplosionSprites;
        public readonly int BarrierNumbSprites;
        public readonly Int32Rect[] BarrierSprites;

        public SpritesheetData()
        {
            DroneNumbSprites = 2;
            AlienNumbSprites = 2;
            EnforcerNumbSprites = 2;
            DrProjectileNumbSprites = 4;
            AlProjectileNumbSprites = 3;
            EnforProjectileNumbSprites = 4;
            ExplosionNumbSprites = 2;
            BarrierNumbSprites = 3;

            DroneSprites = new Int32Rect[DroneNumbSprites];
            AlienSprites = new Int32Rect[AlienNumbSprites];
            EnforcerSprites = new Int32Rect[EnforcerNumbSprites];
            DrProjectileSprites = new Int32Rect[DrProjectileNumbSprites];
            AlProjectileSprites = new Int32Rect[AlProjectileNumbSprites];
            EnforProjectileSprites = new Int32Rect[EnforProjectileNumbSprites];
            ExplosionSprites = new Int32Rect[ExplosionNumbSprites];
            BarrierSprites = new Int32Rect[BarrierNumbSprites];

            for (int i = 0; i < DroneNumbSprites; ++i)
            {
                DroneSprites[i].X = 5 * (i + 1) + 36 * i;
                DroneSprites[i].Y = 5;
                DroneSprites[i].Width = 36;
                DroneSprites[i].Height = 24;
            }
            for (int i = 0; i < AlienNumbSprites; ++i)
            {
                AlienSprites[i].X = 5 * (i + 1) + 33 * i;
                AlienSprites[i].Y = 34;
                AlienSprites[i].Width = 33;
                AlienSprites[i].Height = 24;
            }
            for (int i = 0; i < EnforcerNumbSprites; ++i)
            {
                EnforcerSprites[i].X = 5 * (i + 1) + 24 * i;
                EnforcerSprites[i].Y = 63;
                EnforcerSprites[i].Width = 24;
                EnforcerSprites[i].Height = 24;
            }
            for (int i = 0; i < DrProjectileNumbSprites; ++i)
            {
                DrProjectileSprites[i].X = 93 + 5 * i + 9 * i;
                DrProjectileSprites[i].Y = 37;
                DrProjectileSprites[i].Width = 9;
                DrProjectileSprites[i].Height = 18;
            }
            for (int i = 0; i < AlProjectileNumbSprites; ++i)
            {
                AlProjectileSprites[i].X = 149 + 5 * i + 9 * i;
                AlProjectileSprites[i].Y = 37;
                AlProjectileSprites[i].Width = 9;
                AlProjectileSprites[i].Height = 21;
            }
            for (int i = 0; i < EnforProjectileNumbSprites; ++i)
            {
                EnforProjectileSprites[i].X = 190 + 5 * i + 9 * i;
                EnforProjectileSprites[i].Y = 37;
                EnforProjectileSprites[i].Width = 9;
                EnforProjectileSprites[i].Height = 21;
            }
            for (int i = 0; i < ExplosionNumbSprites; ++i)
            {
                ExplosionSprites[i].X = 131 + (5 + 39) * i;
                ExplosionSprites[i].Y = 5;
                ExplosionSprites[i].Width = 39;
                ExplosionSprites[i].Height = 24;
            }
            for (int i = 0; i < BarrierNumbSprites; ++i)
            {
                BarrierSprites[i].X = 293 + (7 + 66) * i;
                BarrierSprites[i].Y = 5;
                BarrierSprites[i].Width = 66;
                BarrierSprites[i].Height = 48;
            }
        }


    }
}
