using System;
using System.Collections.Generic;
using System.Text;

namespace CW_HLL2
{
    public sealed class Game
    {
        public int EnemySpeed, EnemyDescendSpeed, EnemyProjectileSpeed;
        public int DroneDamage, AlienDamage, EnforcerDamage;
        public int DronePts, AlienPts, EnforcerPts, OverseerPts;

        public Game()
        {

        }
        
        public Game(int enSpeed, int enDescSpeed, int enPrjSpeed, int drDmg, int alDmg, int enfDmg, int drPts, int alPts, int enfPts, int ovPts)
        {
            EnemySpeed = enSpeed;
            EnemyDescendSpeed = enDescSpeed;
            EnemyProjectileSpeed = enPrjSpeed;

            DroneDamage = drDmg;
            AlienDamage = alDmg;
            EnforcerDamage = enfDmg;

            DronePts = drPts;
            AlienPts = alPts;
            EnforcerPts = enfPts;
            OverseerPts = ovPts;
        }

        ~Game()
        {

        }
        
    }
}
