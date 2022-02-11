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
        int enemyDescSpeed = 10;

        int zakosInWave = 8;

        List<bEnemy> enemyList;

        List<Projectile> projectileList;

        int AnimUpdateTimer;

        bool movUp, movDown, movLeft, movRight;
        const int zakoPts = 50;

        bool onPause;
        //double SCREENWIDTH = 640;
        //double SCREENHEIGHT = 960;

        //Rectangle Player;

        List<Rectangle> hitBoxGC;
        List<bEnemy> enemyGC;
        List<Projectile> projectileGC;

        DispatcherTimer runTimer = new DispatcherTimer();
        ImageBrush plrSkin = new ImageBrush();
        ImageBrush plrProjectile = new ImageBrush();
        ImageBrush zakoSprite = new ImageBrush();
        BitmapImage spritesheetBI = new BitmapImage();
        SpritesheetData spritesheetData;

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

            hitBoxGC = new List<Rectangle>();
            enemyGC = new List<bEnemy>();
            projectileGC = new List<Projectile>();

            onPause = false;

            plrSkin.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/res/ship.png"));
            plrProjectile.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/res/projectiles/Shot4/shot4_5f.png"));
            zakoSprite.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/res/zako.png"));
            spritesheetBI.BeginInit();
            spritesheetBI.UriSource = new Uri(Environment.CurrentDirectory + "/res/spritesheet.png");
            spritesheetBI.EndInit();
            spritesheetData = new SpritesheetData();
        }

        private void GameStart()
        {
            PlayerData = new Plr(45, 45, 5, 8, 3, 0, plrSkin);
            enemyList = new List<bEnemy>();
            projectileList = new List<Projectile>();

            updateUI(PlayerData.Lives, PlayerData.Score);

            SpawnPlayer((mCanvas.Width - PlayerData.hitBox.Width) / 2, (mCanvas.Height - PlayerData.hitBox.Height) / 2);
            SpawnDroneWave(50);
            SpawnDroneWave(40 - 30);
        }

        private void GameTick(object sender, EventArgs e)
        {
            if (onPause)
            {
                //show pause menu
            }
            else if (!PlayerData.IsDead())
            {
                ++AnimUpdateTimer;
                if (AnimUpdateTimer == 40)
                    AnimUpdateTimer = 0;

                HandlePlayerInput(PlayerData.MovSpeed);
                MoveEnemies();
                CollisionCheck();
                updateUI(PlayerData.Lives, PlayerData.Score);
                gcHitBoxes();
                gcEnemies();
                gcProjectiles();
                if (AnimUpdateTimer == 0)
                {
                    AnimUpdate();
                    EnemyShoot();
                }
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
                onPause = !onPause;
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
            //Check Projectiles
            foreach(Projectile prj in projectileList)
            {
                if(prj.IsPlayerProjectile)
                {
                    Canvas.SetTop(prj.hitBox, Canvas.GetTop(prj.hitBox) - PlayerData.ProjectileSpeed);
                    Rect plrPrjHitbox = new Rect(Canvas.GetLeft(prj.hitBox), Canvas.GetTop(prj.hitBox), prj.hitBox.Width, prj.hitBox.Height);

                    if (Canvas.GetTop(prj.hitBox) <= -PlayerData.ProjectileSpeed)
                        hitBoxGC.Add(prj.hitBox);
                    else
                    {
                        foreach (bEnemy enemy in enemyList)
                        {
                            Rect enemyHitbox = new Rect(Canvas.GetLeft(enemy.hitBox), Canvas.GetTop(enemy.hitBox), enemy.hitBox.Width, enemy.hitBox.Height);

                            if (plrPrjHitbox.IntersectsWith(enemyHitbox))
                            {
                                if (enemy.Health > 1)
                                {
                                    enemy.Health--;
                                }
                                else
                                {
                                    if ((string)enemy.hitBox.Tag == "Drone")
                                    {
                                        PlayerData.AddScore(zakoPts);
                                    }
                                    PlayerData.EffectiveShots++;
                                    hitBoxGC.Add(enemy.hitBox);
                                    enemyGC.Add(enemy);
                                }

                                hitBoxGC.Add(prj.hitBox);
                                projectileGC.Add(prj);
                            }
                        }
                    }
                }
                else    //if it's an enemy projectile
                {
                    //!!!!!!!!!CHANGE SPEED TO ENEMIES PROJECTILE SPEED!!!!!!!!!
                    Canvas.SetTop(prj.hitBox, Canvas.GetTop(prj.hitBox) + PlayerData.ProjectileSpeed);
                    Rect enemyPrjHitbox = new Rect(Canvas.GetLeft(prj.hitBox), Canvas.GetTop(prj.hitBox), prj.hitBox.Width, prj.hitBox.Height);

                    if (Canvas.GetTop(prj.hitBox) >= mCanvas.Height + PlayerData.ProjectileSpeed)
                        hitBoxGC.Add(prj.hitBox);
                    else
                    {
                        Rect playerHitbox = new Rect(Canvas.GetLeft(PlayerData.hitBox), Canvas.GetTop(PlayerData.hitBox), PlayerData.hitBox.Width, PlayerData.hitBox.Height);

                        if (enemyPrjHitbox.IntersectsWith(playerHitbox))
                        {
                            PlayerData.RemoveLife();
                            hitBoxGC.Add(prj.hitBox);
                            projectileGC.Add(prj);
                        }
                    }
                }
            }

            //Check player collision with enemies
            Rect plrHitbox = new Rect(Canvas.GetLeft(PlayerData.hitBox), Canvas.GetTop(PlayerData.hitBox), PlayerData.hitBox.Width, PlayerData.hitBox.Height);

            foreach (bEnemy enemy in enemyList)
            {
                Rect enemyHitbox = new Rect(Canvas.GetLeft(enemy.hitBox), Canvas.GetTop(enemy.hitBox), enemy.hitBox.Width, enemy.hitBox.Height);

                if (plrHitbox.IntersectsWith(enemyHitbox))
                {
                    PlayerData.RemoveLife();
                    hitBoxGC.Add(enemy.hitBox);
                    enemyGC.Add(enemy);
                }
            }
        }

        private void MoveEnemies()
        {
            enemyDirChange = false;

            //Direction check
            foreach (bEnemy tmp in enemyList) if (!enemyDirChange)
            {
                if (tmp.EnemyType != EnemyTypeList.Overseer && (((Canvas.GetLeft(tmp.hitBox) + tmp.hitBox.Width) >= (mCanvas.Width)) || (Canvas.GetLeft(tmp.hitBox) <= 5)))
                {
                    enemyDirChange = true;
                    foreach (bEnemy ench in enemyList)
                    {
                        ench.changeMovDir();
                    }

                    MoveEnemiesDown();
                }
            }

            foreach (bEnemy tmp in enemyList)
            {
                if (tmp.EnemyType != EnemyTypeList.Overseer)
                    Canvas.SetLeft(tmp.hitBox, Canvas.GetLeft(tmp.hitBox) + tmp.MovSpeed);
                //TODO: add height check?
            }

            //MoveOverseers();
        }

        private void EnemyShoot()
        {
            int closestDr = -1;
            //double minDist = Double.MaxValue;
            double minDist = 100000;
            double shootPos = 0;
            int enemyNumber = -1;
            bool freeLine = true;

            foreach (bEnemy tmpEnemy in enemyList)
            {
                ++enemyNumber;
                freeLine = true;
                shootPos = Canvas.GetLeft(tmpEnemy.hitBox) + (tmpEnemy.hitBox.Width / 2) - 3;

                foreach (bEnemy lineEnemy in enemyList)
                {
                    if ((shootPos >= Canvas.GetLeft(lineEnemy.hitBox)) && (shootPos <= Canvas.GetLeft(lineEnemy.hitBox) + lineEnemy.hitBox.Width) ||
                        ((shootPos + 3) >= Canvas.GetLeft(lineEnemy.hitBox)) && ((shootPos + 3) <= Canvas.GetLeft(lineEnemy.hitBox) + lineEnemy.hitBox.Width))
                    {
                        //freeLine = false;
                    }
                }

                if (minDist > Math.Abs(Canvas.GetLeft(tmpEnemy.hitBox) - Canvas.GetLeft(PlayerData.hitBox)) && freeLine)
                {
                    minDist = Math.Abs(Canvas.GetLeft(tmpEnemy.hitBox) - Canvas.GetLeft(PlayerData.hitBox));
                    closestDr = enemyNumber;
                }
            }

            //spawning enemy projectile
            if (closestDr != -1)
            {
                Projectile newProjectile = new Projectile(16, 8, enemyList[closestDr].ProjectileSpeed, 1, 
                    enemyList[closestDr].EnemyType, spritesheetBI, spritesheetData);

                Canvas.SetLeft(newProjectile.hitBox, Canvas.GetLeft(enemyList[closestDr].hitBox) +
                    (enemyList[closestDr].hitBox.Width - newProjectile.hitBox.Width) / 2);
                Canvas.SetTop(newProjectile.hitBox, Canvas.GetTop(enemyList[closestDr].hitBox) - newProjectile.hitBox.Height);

                mCanvas.Children.Add(newProjectile.hitBox);
                projectileList.Add(newProjectile);
            }
        }

        private void MoveEnemiesDown()
        {
            foreach (bEnemy tmp in enemyList)
            {
                if (tmp.EnemyType != EnemyTypeList.Overseer)
                    Canvas.SetTop(tmp.hitBox, Canvas.GetTop(tmp.hitBox) + enemyDescSpeed);
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

        private void SpawnPlayer(double x, double y)
        {
            Canvas.SetLeft(PlayerData.hitBox, x);
            Canvas.SetTop(PlayerData.hitBox, y);
            mCanvas.Children.Add(PlayerData.hitBox);
        }

        private void SpawnDrone(double x, double y)
        {
            bEnemy newDrone = new bEnemy(30, 30, 2, 2, 1, EnemyTypeList.Drone, spritesheetBI, spritesheetData);

            Canvas.SetLeft(newDrone.hitBox, x);
            Canvas.SetTop(newDrone.hitBox, y);
            mCanvas.Children.Add(newDrone.hitBox);
            enemyList.Add(newDrone);
        }

        private void SpawnDroneWave(double y)
        {
            for (int i = 0; i < zakosInWave; i++)
            {
                SpawnDrone((double)i * (30 + 15) + 10, y);
            }
        }

        private void SpawnPlayerProjectile()
        {
            Projectile newProjectile = new Projectile(16, 8, PlayerData.ProjectileSpeed, 1, true, plrProjectile);

            Canvas.SetLeft(newProjectile.hitBox, Canvas.GetLeft(PlayerData.hitBox) + (PlayerData.hitBox.Width - newProjectile.hitBox.Width) / 2);
            Canvas.SetTop(newProjectile.hitBox, Canvas.GetTop(PlayerData.hitBox) - newProjectile.hitBox.Height);

            mCanvas.Children.Add(newProjectile.hitBox);
            projectileList.Add(newProjectile);
        }

        private void updateUI(int plrLives, int plrScore)
        {
            scoreData.Content = plrScore;
            highscoreData.Content = plrScore;
        }

        private void AnimUpdate()
        {
            foreach (bEnemy tmp in enemyList)
            {
                if (tmp.EnemyType != EnemyTypeList.Overseer)
                    tmp.UpdateFrame(spritesheetData);
            }
            foreach (Projectile tmp in projectileList)
            {
                if (!tmp.IsPlayerProjectile)
                    tmp.UpdateFrame(spritesheetData);
            }

        }

        private void showPause()
        {
            //print on pause
        }

        private void gcHitBoxes()
        {
            foreach (var tmp in hitBoxGC)
                mCanvas.Children.Remove(tmp);
        }

        private void gcEnemies()
        {
            foreach (var tmp in enemyGC)
                enemyList.Remove(tmp);
        }

        private void gcProjectiles()
        {
            foreach (var tmp in projectileGC)
                projectileList.Remove(tmp);
        }
    }
}