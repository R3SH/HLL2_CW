using System;
using System.Collections.Generic;
using System.Linq;
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

using System.IO;
using System.Windows.Threading;

namespace CW_HLL2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Plr PlayerData;

        bool enemyDirChange;
        int enemySpeed = 2;
        int enemyDescSpeed = 10;

        int zakosInWave = 8;
        double zakoW = 30;
        double zakoH = 30;

        bool movUp, movDown, movLeft, movRight;
        const int zakoPts = 50;

        bool onPause;
        //double SCREENWIDTH = 640;
        //double SCREENHEIGHT = 960;

        //Rectangle Player;

        List<Rectangle> gc = new List<Rectangle>();

        DispatcherTimer runTimer = new DispatcherTimer();
        ImageBrush plrSkin = new ImageBrush();
        ImageBrush plrProjectile = new ImageBrush();
        ImageBrush zakoSprite = new ImageBrush();

        public MainWindow()
        {
            InitializeComponent();

            GameInit();
            GameStart();

            runTimer.Tick += GameTick;
            runTimer.Interval = TimeSpan.FromMilliseconds(8);
            runTimer.Start();
            
            if (PlayerData.IsDead())
            {
                runTimer.Stop();
                //add ask to play again or smth
            }
        }

        private void GameInit()
        {
            mCanvas.Focus();

            onPause = false;

            plrSkin.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/res/ship.png"));
            plrProjectile.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/res/projectiles/Shot4/shot4_5f.png"));
            zakoSprite.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/res/zako.png"));
        }

        private void GameStart()
        {
            PlayerData = new Plr(45, 45, 5, 8, 3, 0, plrSkin);

            updateUI(PlayerData.Lives, PlayerData.Score);

            SpawnPlayer(PlayerData.hitBox.Width, PlayerData.hitBox.Height, (mCanvas.Width - PlayerData.hitBox.Width) / 2, (mCanvas.Height - PlayerData.hitBox.Height) / 2);
            SpawnZakoWave(50);
            SpawnZakoWave(40 - zakoH);
        }

        private void GameTick(object sender, EventArgs e)
        {
            if (!PlayerData.IsDead())
            {
                HandlePlayerInput(PlayerData.MovSpeed);
                MoveEnemies();
                CollisionCheck();
                updateUI(PlayerData.Lives, PlayerData.Score);
                gcClear();
            }
            if (onPause)
            {
                //show pause menu
            }
            else
            {
                //Application.Current.Shutdown();
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                onPause = true;
                //Application.Current.Shutdown();
            }
            if(e.Key == Key.Left)
            {
                movLeft = true;
            }
            if (e.Key == Key.Right)
            {
                movRight = true;
            }
            if (e.Key == Key.Up)
            {
                movUp = true;
            }
            if (e.Key == Key.Down)
            {
                movDown = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                movLeft = false;
            }
            if (e.Key == Key.Right)
            {
                movRight = false;
            }
            if (e.Key == Key.Up)
            {
                movUp = false;
            }
            if (e.Key == Key.Down)
            {
                movDown = false;
            }
            if(e.Key == Key.Space)
            {
                SpawnPlayerProjectile();
                PlayerData.ShotsFired++;
            }
        }

        private void CollisionCheck()
        {
            foreach(Rectangle prj in mCanvas.Children.OfType<Rectangle>())
            {
                if((string)prj.Tag == "playerProjectile")
                {
                    Canvas.SetTop(prj, Canvas.GetTop(prj) - PlayerData.ProjectileSpeed);
                    Rect plrPrjHitbox = new Rect(Canvas.GetLeft(prj), Canvas.GetTop(prj), prj.Width, prj.Height);

                    if (Canvas.GetTop(prj) <= -PlayerData.ProjectileSpeed)
                        gc.Add(prj);
                    else
                    {
                        foreach(Rectangle enemy in mCanvas.Children.OfType<Rectangle>())
                        {
                            if((string)enemy.Tag == "zako")
                            {
                                Rect enemyHitbox = new Rect(Canvas.GetLeft(enemy), Canvas.GetTop(enemy), enemy.Width, enemy.Height);

                                if (plrPrjHitbox.IntersectsWith(enemyHitbox))
                                {
                                    gc.Add(enemy);
                                    gc.Add(prj);
                                    PlayerData.AddScore(zakoPts);
                                    PlayerData.EffectiveShots++;
                                }
                            }
                        }
                    }
                }
                else if ((string)prj.Tag == "Player")
                {
                    Rect plrHitbox = new Rect(Canvas.GetLeft(prj), Canvas.GetTop(prj), prj.Width, prj.Height);
                    foreach (Rectangle enemy in mCanvas.Children.OfType<Rectangle>())
                    {
                        if ((string)enemy.Tag == "zako")
                        {
                            Rect enemyHitbox = new Rect(Canvas.GetLeft(enemy), Canvas.GetTop(enemy), enemy.Width, enemy.Height);

                            if (plrHitbox.IntersectsWith(enemyHitbox))
                            {
                                PlayerData.RemoveLife();
                                gc.Add(enemy);
                            }
                        }
                    }
                }
            }
        }

        private void MoveEnemies()
        {
            enemyDirChange = false;

            //Direction check
            foreach(Rectangle tmp in mCanvas.Children.OfType<Rectangle>()) if(!enemyDirChange)
            {
                if ((string)tmp.Tag == "zako" && (((Canvas.GetLeft(tmp) + tmp.Width) >= (mCanvas.Width)) || (Canvas.GetLeft(tmp) <= 5)))
                {
                    enemyDirChange = true;
                    enemySpeed *= -1;
                    MoveEnemiesDown();
                }
            }

            MoveZakos();
        }

        private void MoveEnemiesDown()
        {
            foreach (Rectangle tmp in mCanvas.Children.OfType<Rectangle>())
            {
                if ((string)tmp.Tag == "zako")
                    Canvas.SetTop(tmp, Canvas.GetTop(tmp) + enemyDescSpeed);
            }
        }

        private void MoveZakos()
        {
            foreach(Rectangle tmp in mCanvas.Children.OfType<Rectangle>())
            {
                if ((string)tmp.Tag == "zako")
                    Canvas.SetLeft(tmp, Canvas.GetLeft(tmp) + enemySpeed);
                //TODO: add height check?
            }
        }

        private void EnemiesDive()
        {
            //TODO: Implement diving (by Bezier curves?)
        }

        private void HandlePlayerInput(int plrSpeed)
        {
            if (movLeft && Canvas.GetLeft(PlayerData.hitBox) > 0)
            {
                Canvas.SetLeft(PlayerData.hitBox, Canvas.GetLeft(PlayerData.hitBox) - plrSpeed);
            }
            if (movRight && (Canvas.GetLeft(PlayerData.hitBox) + PlayerData.hitBox.ActualWidth) < mCanvas.ActualWidth)
            {
                Canvas.SetLeft(PlayerData.hitBox, Canvas.GetLeft(PlayerData.hitBox) + plrSpeed);
            }
            if (movDown && (Canvas.GetTop(PlayerData.hitBox) + PlayerData.hitBox.Height) < mCanvas.ActualHeight)
            {
                Canvas.SetTop(PlayerData.hitBox, Canvas.GetTop(PlayerData.hitBox) + plrSpeed);
            }
            if (movUp && Canvas.GetTop(PlayerData.hitBox) > 0)
            {
                Canvas.SetTop(PlayerData.hitBox, Canvas.GetTop(PlayerData.hitBox) - plrSpeed);
            }
        }

        private void SpawnPlayer(double plrW, double plrH, double x, double y)
        {
            //Player = new Rectangle
            //{
            //    Tag = "Player",
            //    Width = plrW,
            //    Height = plrH,
            //    Fill = plrSkin
            //};

            Canvas.SetLeft(PlayerData.hitBox, x);
            Canvas.SetTop(PlayerData.hitBox, y);
            mCanvas.Children.Add(PlayerData.hitBox);
        }

        private void SpawnZako(double x, double y)
        {
            Rectangle newZako = new Rectangle
            {
                Tag = "zako",
                Width = zakoW,
                Height = zakoH,
                Fill = zakoSprite
            };

            Canvas.SetLeft(newZako, x);
            Canvas.SetTop(newZako, y);
            mCanvas.Children.Add(newZako);
        }

        private void SpawnZakoWave(double y)
        {
            for (int i = 0; i < zakosInWave; i++)
            {
                SpawnZako((double)i * (30 + 15) + 10, y);
            }
        }

        private void SpawnPlayerProjectile()
        {
            Rectangle newProjectile = new Rectangle()
            {
                Tag = "playerProjectile",
                Height = 16,
                Width = 8
            };

            newProjectile.Fill = plrProjectile;

            Canvas.SetLeft(newProjectile, Canvas.GetLeft(PlayerData.hitBox) + (PlayerData.hitBox.Width - newProjectile.Width) / 2);
            Canvas.SetTop(newProjectile, Canvas.GetTop(PlayerData.hitBox) - newProjectile.Height);

            mCanvas.Children.Add(newProjectile);
        }

        private void updateUI(int plrLives, int plrScore)
        {
            scoreData.Content = plrScore;
            highscoreData.Content = plrScore;
        }

        private void showPause()
        {
            //print on pause
        }

        private void gcClear()
        {
            foreach (var tmp in gc)
                mCanvas.Children.Remove(tmp);
        }
    }
}