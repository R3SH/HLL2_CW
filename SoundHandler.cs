using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace CW_HLL2
{
    public enum SoundList : ushort
    {
        MenuPress,
        PlrUp,
        PlrShoot,
        PlrHit,
        PlrDth,
        DroneDth,
        EnforDth,
        EnemyShoot,
        EnemySpawn,
        BarrierHit,
        BarrierDestroyed
    }

    public class SoundHandler
    {
        MediaPlayer[] SoundsArr;

        public SoundHandler()
        {
            InitSounds();
        }

        private void InitSounds()
        {
            SoundsArr = new MediaPlayer[11];
            for (int i = 0; i < 11; ++i)
                SoundsArr[i] = new MediaPlayer();

            SoundsArr[0].Open(new Uri(Environment.CurrentDirectory + "/res/sounds/menuPress.wav"));
            SoundsArr[1].Open(new Uri(Environment.CurrentDirectory + "/res/sounds/plrLiveUp.wav"));
            SoundsArr[2].Open(new Uri(Environment.CurrentDirectory + "/res/sounds/plrShoot.wav"));
            SoundsArr[3].Open(new Uri(Environment.CurrentDirectory + "/res/sounds/plrHit.wav"));
            SoundsArr[4].Open(new Uri(Environment.CurrentDirectory + "/res/sounds/plrDth.wav"));
            SoundsArr[5].Open(new Uri(Environment.CurrentDirectory + "/res/sounds/drnDth.wav"));
            SoundsArr[6].Open(new Uri(Environment.CurrentDirectory + "/res/sounds/enforDth.wav"));
            SoundsArr[7].Open(new Uri(Environment.CurrentDirectory + "/res/sounds/enemyShoot.wav"));
            SoundsArr[8].Open(new Uri(Environment.CurrentDirectory + "/res/sounds/enemySpawn.wav"));
            SoundsArr[9].Open(new Uri(Environment.CurrentDirectory + "/res/sounds/barrierHit.wav"));
            SoundsArr[10].Open(new Uri(Environment.CurrentDirectory + "/res/sounds/barrierDestr.wav"));
        }

        public void Play(SoundList sType)
        {
            SoundsArr[(int)sType].Position = TimeSpan.Zero;
            SoundsArr[(int)sType].Play();
        }
    }
}
