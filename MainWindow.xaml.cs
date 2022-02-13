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
        //int enemyDescSpeed = 10;
        int enemyDescSpeed = 1;

        int dronesInWave = 8;

        List<Barrier> barrierList;
        List<Projectile> projectileList;
        List<bEnemy> enemyList;

        int AnimUpdateTimer;

        bool movUp, movDown, movLeft, movRight;
        const int dronePts = 50;

        bool onPause;
        //double SCREENWIDTH = 640;
        //double SCREENHEIGHT = 960;

        //Rectangle Player;

        List<Rectangle> hitBoxGC;
        List<Barrier> barrierGC;
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
            runTimer.Interval = TimeSpan.FromMilliseconds(16);
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
            barrierGC = new List<Barrier>();
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
            //PlayerData = new Plr(45, 45, 5, 8, 3, 0, plrSkin);
            PlayerData = new Plr(45, 45, 5, 8, 3333, 0, plrSkin);
            barrierList = new List<Barrier>();
            projectileList = new List<Projectile>();
            enemyList = new List<bEnemy>();

            updateUI(PlayerData.Lives, PlayerData.Score);

            SpawnPlayer((mCanvas.Width - PlayerData.hitBox.Width) / 2, mCanvas.Height - PlayerData.hitBox.Height);
            //SPAWN FROM DOWN TO TOP
            SpawnBarrierWall(4, 600, 9);
            SpawnDroneWave(130, EnemyTypeList.Drone);
            SpawnDroneWave(95, EnemyTypeList.Drone);
            SpawnDroneWave(60, EnemyTypeList.Alien);
            SpawnDroneWave(25, EnemyTypeList.Enforcer);
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
                if (AnimUpdateTimer == 60)
                {
                    AnimUpdateTimer = 0;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }

                HandlePlayerInput(PlayerData.MovSpeed);
                MoveEnemies();
                CollisionCheck();
                updateUI(PlayerData.Lives, PlayerData.Score);
                gcHitBoxes();
                gcProjectiles();
                gcBarriers();
                gcEnemies();
                if (AnimUpdateTimer % 20 == 0)
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
                if (!onPause)
                {
                    SpawnPlayerProjectile();
                    PlayerData.ShotsFired++;
                }
            }
        }

        private void CollisionCheck()
        {
            //Check Projectiles
            foreach(Projectile prj in projectileList)
            {
                if(prj.IsPlayerProjectile)
                {
                    Canvas.SetTop(prj.hitBox, Canvas.GetTop(prj.hitBox) - prj.MovSpeed);
                    Rect plrPrjHitbox = new Rect(Canvas.GetLeft(prj.hitBox), Canvas.GetTop(prj.hitBox), prj.hitBox.Width, prj.hitBox.Height);

                    if (Canvas.GetTop(prj.hitBox) <= -prj.MovSpeed)
                    {
                        prj.RemoveProjectile(ref hitBoxGC, ref projectileGC);
                        //hitBoxGC.Add(prj.hitBox);
                    }
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
                                        PlayerData.AddScore(dronePts);
                                    }
                                    PlayerData.EffectiveShots++;

                                    enemy.RemoveEnemy(ref hitBoxGC, ref enemyGC);
                                    //hitBoxGC.Add(enemy.hitBox);
                                    //enemyGC.Add(enemy);
                                }

                                prj.RemoveProjectile(ref hitBoxGC, ref projectileGC);
                            }
                        }

                        foreach (Barrier tmpBar in barrierList)
                        {
                            Rect barrierHitBox = new Rect(Canvas.GetLeft(tmpBar.hitBox), Canvas.GetTop(tmpBar.hitBox), tmpBar.hitBox.Width, tmpBar.hitBox.Height);

                            if (plrPrjHitbox.IntersectsWith(barrierHitBox))
                            {
                                tmpBar.Hit(prj.Damage, spritesheetData);
                                if (tmpBar.Health == 0)
                                {
                                     tmpBar.RemoveBarrier(ref hitBoxGC, ref barrierGC);
                                }
                                prj.RemoveProjectile(ref hitBoxGC, ref projectileGC);
                            }
                        }
                    }
                }
                else    //if it's an enemy projectile
                {
                    Canvas.SetTop(prj.hitBox, Canvas.GetTop(prj.hitBox) + prj.MovSpeed);
                    Rect enemyPrjHitbox = new Rect(Canvas.GetLeft(prj.hitBox), Canvas.GetTop(prj.hitBox), prj.hitBox.Width, prj.hitBox.Height);

                    if (Canvas.GetTop(prj.hitBox) >= mCanvas.Height + PlayerData.ProjectileSpeed)
                    {
                        prj.RemoveProjectile(ref hitBoxGC, ref projectileGC);
                    }
                    else
                    {
                        Rect playerHitbox = new Rect(Canvas.GetLeft(PlayerData.hitBox), Canvas.GetTop(PlayerData.hitBox), PlayerData.hitBox.Width, PlayerData.hitBox.Height);

                        if (enemyPrjHitbox.IntersectsWith(playerHitbox))
                        {
                            PlayerData.RemoveLife();
                            prj.RemoveProjectile(ref hitBoxGC, ref projectileGC);
                        }

                        foreach (Barrier tmpBar in barrierList)
                        {
                            Rect barrierHitBox = new Rect(Canvas.GetLeft(tmpBar.hitBox), Canvas.GetTop(tmpBar.hitBox), tmpBar.hitBox.Width, tmpBar.hitBox.Height);
                            
                            if (enemyPrjHitbox.IntersectsWith(barrierHitBox))
                            {
                                tmpBar.Hit(prj.Damage, spritesheetData);
                                if (tmpBar.Health == 0)
                                {
                                    tmpBar.RemoveBarrier(ref hitBoxGC, ref barrierGC);
                                }
                                prj.RemoveProjectile(ref hitBoxGC, ref projectileGC);
                            }
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

                    enemy.RemoveEnemy(ref hitBoxGC, ref enemyGC);
                    //hitBoxGC.Add(enemy.hitBox);
                    //enemyGC.Add(enemy);
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
            double minDist = Double.MaxValue;
            //double minDist = 100000;
            double shootPosX, shootPosY;
            int enemyNumber = -1;
            bool freeLine = true;

            foreach (bEnemy tmpEnemy in enemyList)
            {
                ++enemyNumber;
                shootPosX = Canvas.GetLeft(tmpEnemy.hitBox) + (tmpEnemy.hitBox.Width / 2) - 3;
                shootPosY = Canvas.GetTop(tmpEnemy.hitBox) + tmpEnemy.hitBox.Height;

                freeLine = true;

                foreach (bEnemy lineEnemy in enemyList)
                {

                    if(shootPosY < Canvas.GetTop(lineEnemy.hitBox))
                    {
                        if ((shootPosX >= Canvas.GetLeft(lineEnemy.hitBox)) && (shootPosX <= Canvas.GetLeft(lineEnemy.hitBox) + lineEnemy.hitBox.Width) ||
                            ((shootPosX + 3) >= Canvas.GetLeft(lineEnemy.hitBox)) && ((shootPosX + 3) <= Canvas.GetLeft(lineEnemy.hitBox) + lineEnemy.hitBox.Width))
                        {
                            freeLine = false;
                        }
                    }
                    else
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
                Canvas.SetTop(newProjectile.hitBox, Canvas.GetTop(enemyList[closestDr].hitBox) + enemyList[closestDr].hitBox.Height
                    - newProjectile.hitBox.Height);

                //EnemyLightUp
                enemyList[closestDr].hitBox.Fill = Brushes.Red;

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

        private void SpawnBarrier(double x, double y, int barHealth)
        {
            Barrier newBarrier = new Barrier(48, 66, barHealth, spritesheetBI, spritesheetData);

            Canvas.SetLeft(newBarrier.hitBox, x);
            Canvas.SetTop(newBarrier.hitBox, y);
            mCanvas.Children.Add(newBarrier.hitBox);
            barrierList.Add(newBarrier);
        }
        
        private void SpawnPlayerProjectile()
        {
            Projectile newProjectile = new Projectile(16, 8, PlayerData.ProjectileSpeed, 1, true, plrProjectile);

            Canvas.SetLeft(newProjectile.hitBox, Canvas.GetLeft(PlayerData.hitBox) + (PlayerData.hitBox.Width - newProjectile.hitBox.Width) / 2);
            Canvas.SetTop(newProjectile.hitBox, Canvas.GetTop(PlayerData.hitBox) - newProjectile.hitBox.Height);

            mCanvas.Children.Add(newProjectile.hitBox);
            projectileList.Add(newProjectile);
        }

        private void SpawnEnemy(double x, double y, EnemyTypeList enType)
        {
            bEnemy newDrone = new bEnemy(30, 30, 2, 2, 1, enType, spritesheetBI, spritesheetData);

            Canvas.SetLeft(newDrone.hitBox, x);
            Canvas.SetTop(newDrone.hitBox, y);
            mCanvas.Children.Add(newDrone.hitBox);
            enemyList.Add(newDrone);
        }

        private void SpawnBarrierWall(int barrierNumb, double y, int barrierHealth)
        {
            //double xOffset = mCanvas.Width / barrierNumb - 20;
            double xOffset = (mCanvas.Width - (barrierNumb * 66)) / (barrierNumb + 1);

            for (int i = 0; i < barrierNumb; i++)
            {
                //SpawnBarrier((double)i * (48 + 15), y, barrierHealth);
                //SpawnBarrier((double)(i+1) * xOffset + (66 * i), y, barrierHealth);       //FIX
                SpawnBarrier(xOffset * (i+1) + (66*i), y, barrierHealth);       //FIX
            }
        }

        private void SpawnDroneWave(double y, EnemyTypeList enType)
        {
            for (int i = 0; i < dronesInWave; i++)
            {
                SpawnEnemy((double)i * (30 + 15) + 10, y, enType);
            }
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
            List<Rectangle> toRemove = new List<Rectangle>();

            foreach (var tmp in hitBoxGC)
            {
                mCanvas.Children.Remove(tmp);
                toRemove.Add(tmp);
            }

            foreach (var tmp in toRemove)
            {
                hitBoxGC.Remove(tmp);
            }

            toRemove.Clear();
        }

        private void gcProjectiles()
        {
            List<Projectile> toRemove = new List<Projectile>();
            
            foreach (var tmp in projectileGC)
            {
                projectileList.Remove(tmp);
                toRemove.Add(tmp);
            }

            foreach (var tmp in toRemove)
            {
                projectileGC.Remove(tmp);
            }

            toRemove.Clear();
        }

        private void gcBarriers()
        {
            List<Barrier> toRemove = new List<Barrier>();

            foreach (var tmp in barrierGC)
            {
                barrierList.Remove(tmp);
                toRemove.Add(tmp);
            }

            foreach (var tmp in toRemove)
            {
                barrierGC.Remove(tmp);
            }

            toRemove.Clear();
        }

        private void gcEnemies()
        {
            List<bEnemy> toRemove = new List<bEnemy>();
            
            foreach (var tmp in enemyGC)
            {
                enemyList.Remove(tmp);
                toRemove.Add(tmp);
            }

            foreach (var tmp in toRemove)
            {
                enemyGC.Remove(tmp);
            }

            toRemove.Clear();
        }

    }
}