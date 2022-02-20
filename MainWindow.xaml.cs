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
using System.Xml.Serialization;
using System.Collections.ObjectModel;

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

        int dronesInWave = 8;

        List<Barrier> barrierList;
        List<Projectile> projectileList;
        List<bEnemy> enemyList;
        List<Explosion> explosionList;

        const int MaxHighscoreListEntryCount = 10;
        const int shotDelay = 20;
        int AnimUpdateTimer, plrShootDelay;
        int CurrentHighScore = 0;

        bool movLeft, movRight;

        bool OnStart, OnPause, HardMode;

        Game game;
        SoundHandler SndHandler;

        List<Rectangle> hitBoxGC;
        List<Barrier> barrierGC;
        List<bEnemy> enemyGC;
        List<Projectile> projectileGC;
        List<Explosion> explosionGC;

        DispatcherTimer runTimer = new DispatcherTimer();
        ImageBrush plrSkin = new ImageBrush();
        ImageBrush plrProjectile = new ImageBrush();
        BitmapImage spritesheetBI = new BitmapImage();
        SpritesheetData spritesheetData;

        public ObservableCollection<HighScoreNode> HighscoreList
        {
            get; set;
        } = new ObservableCollection<HighScoreNode>();

        public MainWindow()
        {
            InitializeComponent();

            GameInit();

            runTimer.Tick += GameTick;
            runTimer.Interval = TimeSpan.FromMilliseconds(16);
            runTimer.Start();
        }

        private void GameInit()
        {
            mCanvas.Focus();

            OnStart = true;
            OnPause = false;

            plrShootDelay = shotDelay;

            SndHandler = new SoundHandler();

            hitBoxGC = new List<Rectangle>();
            barrierGC = new List<Barrier>();
            enemyGC = new List<bEnemy>();
            projectileGC = new List<Projectile>();
            explosionGC = new List<Explosion>();

            pause.Visibility = Visibility.Collapsed;
            bdrNewHighscore.Visibility = Visibility.Collapsed;
            playAgain.Visibility = Visibility.Collapsed;

            plrSkin.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/res/ship.png"));
            plrProjectile.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/res/projectiles/Shot4/shot4_5f.png"));
            spritesheetBI.BeginInit();
            spritesheetBI.UriSource = new Uri(Environment.CurrentDirectory + "/res/spritesheet.png");
            spritesheetBI.EndInit();
            spritesheetData = new SpritesheetData();

            LoadHighscoreList();
        }

        private void GameStart()
        {
            runTimer.IsEnabled = true;

            if (game != null)
                game = null;

            game = new Game(2, 3, 4, 1, 2, 3, 10, 20, 30, 100);

            if (HardMode)
            {
                PlayerData = new Plr(45, 45, 5, 8, 3, 0, plrSkin);
                game.DronePts *= 3;
                game.AlienPts *= 3;
                game.EnforcerPts *= 3;
                game.EnemySpeed = 3;
                game.EnemyDescendSpeed = 5;
            }
            else
            {
                PlayerData = new Plr(45, 45, 5, 8, 9, 0, plrSkin);
            }

            if (barrierList != null)
                barrierList.Clear();
            if(projectileList != null)
                projectileList.Clear();
            if(enemyList != null)
                enemyList.Clear();
            if (explosionList != null)
                explosionList.Clear();

            ClearLists();

            barrierList = new List<Barrier>();
            projectileList = new List<Projectile>();
            enemyList = new List<bEnemy>();
            explosionList = new List<Explosion>();

            highscoreData.Content = CurrentHighScore;
            updateUI(PlayerData.Lives, PlayerData.Score);

            SpawnPlayer((mCanvas.Width - PlayerData.hitBox.Width) / 2, mCanvas.Height - PlayerData.hitBox.Height);
            //SPAWN FROM DOWN TO TOP
            SpawnBarrierWall(4, 600, 9);
            SpawnEnemyWave();
        }

        private void GameTick(object sender, EventArgs e)
        {
            if (OnStart)
            {
                HandlePlayerInput(0);
            }
            else
            {
                if (OnPause)
                {
                    //show pause menu
                }
                else if (!PlayerData.IsDead())
                {
                    ++AnimUpdateTimer;

                    if (enemyList.Count == 0)
                    {
                        game.EnemySpeed++;
                        SpawnEnemyWave();
                    }

                    ShootDelay();
                    HandlePlayerInput(PlayerData.MovSpeed);
                    MoveEnemies();
                    CollisionCheck();
                    updateUI(PlayerData.Lives, PlayerData.Score);
                    gcHitBoxes();
                    gcProjectiles();
                    gcBarriers();
                    gcEnemies();
                    gcExplosions();

                    if (AnimUpdateTimer % 10 == 0)
                    {
                        if (AnimUpdateTimer % 20 == 0)
                        {
                            AnimUpdate();
                        }

                        ExplosionsUpdate();
                    }
                    if (AnimUpdateTimer == 60)
                    {
                        EnemyShoot();
                        AnimUpdateTimer = 0;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                    }
                }
                else
                {
                    SndHandler.Play(SoundList.PlrDth);
                    hitBoxGC.Add(PlayerData.hitBox);
                    ClearLists();
                    EndGame();
                }

            }

        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (OnPause)
                {
                    Application.Current.Shutdown();
                }
                //game.OnPause = !game.OnPause;
            }
            if (e.Key == Key.Left)
            {
                movLeft = true;
            }
            if (e.Key == Key.Right)
            {
                movRight = true;
            }
            if (e.Key == Key.Space)
            {
                if (plrShootDelay == shotDelay)
                {
                    plrShootDelay = 0;
                    SpawnPlayerProjectile();
                    PlayerData.ShotsFired++;
                }
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
            if (e.Key == Key.Escape)
            {
                if (!OnPause && !OnStart)
                {
                    OnPause = true;
                    pause.Visibility = Visibility.Visible;
                    SndHandler.Play(SoundList.MenuPress);
                }
            }
            if (e.Key == Key.Space)
            {
                if (OnStart)
                {
                    OnStart = false;
                    menu.Visibility = Visibility.Collapsed;
                    GameStart();
                }
                else if (OnPause)
                {
                    OnPause = false;
                    pause.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (plrShootDelay == shotDelay)
                    {
                        plrShootDelay = 0;
                        SpawnPlayerProjectile();
                        PlayerData.ShotsFired++;
                    }
                }
            }
        }

        private void CollisionCheck()
        {
            //Check Projectiles
            foreach (Projectile prj in projectileList)
            {
                if (prj.IsPlayerProjectile)
                {
                    Canvas.SetTop(prj.hitBox, Canvas.GetTop(prj.hitBox) - prj.MovSpeed);
                    Rect plrPrjHitbox = new Rect(Canvas.GetLeft(prj.hitBox), Canvas.GetTop(prj.hitBox), prj.hitBox.Width, prj.hitBox.Height);

                    if (Canvas.GetTop(prj.hitBox) <= -prj.MovSpeed)
                    {
                        prj.RemoveProjectile(ref hitBoxGC, ref projectileGC);
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
                                    switch (enemy.EnemyType)
                                    {
                                        case EnemyTypeList.Drone:
                                            PlayerData.AddScore(game.DronePts);
                                            SndHandler.Play(SoundList.DroneDth);
                                            break;
                                        case EnemyTypeList.Alien:
                                            PlayerData.AddScore(game.AlienPts);
                                            SndHandler.Play(SoundList.DroneDth);
                                            break;
                                        case EnemyTypeList.Enforcer:
                                            PlayerData.AddScore(game.EnforcerPts);
                                            SndHandler.Play(SoundList.EnforDth);
                                            break;
                                        case EnemyTypeList.Overseer:
                                            PlayerData.AddScore(game.OverseerPts);
                                            break;
                                    }

                                    PlayerData.EffectiveShots++;

                                    explosionList.Add(SpawnExplosion(Canvas.GetLeft(enemy.hitBox), Canvas.GetTop(enemy.hitBox), enemy.hitBox.Height, enemy.hitBox.Width));
                                    enemy.RemoveEnemy(ref hitBoxGC, ref enemyGC);
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
                                    SndHandler.Play(SoundList.BarrierDestroyed);
                                }
                                else
                                {
                                    SndHandler.Play(SoundList.BarrierHit);
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
                            SndHandler.Play(SoundList.PlrHit);
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
                                    SndHandler.Play(SoundList.BarrierDestroyed);
                                }
                                else
                                {
                                    SndHandler.Play(SoundList.BarrierHit);
                                }

                                prj.RemoveProjectile(ref hitBoxGC, ref projectileGC);
                            }
                        }
                    }
                }
            }
        }

        private void MoveEnemies()
        {
            enemyDirChange = false;
            double maxY = 0;

            //Direction check and y check
            foreach (bEnemy tmp in enemyList) if (!enemyDirChange)
                {
                    if (maxY < (Canvas.GetTop(tmp.hitBox) + tmp.hitBox.Height))
                    {
                        maxY = Canvas.GetTop(tmp.hitBox) + tmp.hitBox.Height;
                    }

                    if (tmp.EnemyType != EnemyTypeList.Overseer && (((Canvas.GetLeft(tmp.hitBox) + tmp.hitBox.Width) >= (mCanvas.Width)) || (Canvas.GetLeft(tmp.hitBox) <= 5)))
                    {
                        enemyDirChange = true;
                        foreach (bEnemy ench in enemyList)
                        {
                            ench.changeMovDir();
                        }

                        if (maxY < 500)
                        {
                            MoveEnemiesDown();
                        }
                    }
                }

            foreach (bEnemy tmp in enemyList)
            {
                if (tmp.EnemyType != EnemyTypeList.Overseer)
                    Canvas.SetLeft(tmp.hitBox, Canvas.GetLeft(tmp.hitBox) + tmp.MovSpeed);
            }
        }

        private void EnemyShoot()
        {
            int closestDr = -1;
            double minDist = Double.MaxValue;
            double shootPosX, shootPosY;
            int enemyNumber = -1;
            bool freeLine;

            foreach (bEnemy tmpEnemy in enemyList)
            {
                ++enemyNumber;
                shootPosX = Canvas.GetLeft(tmpEnemy.hitBox) + (tmpEnemy.hitBox.Width / 2) - 3;
                shootPosY = Canvas.GetTop(tmpEnemy.hitBox) + tmpEnemy.hitBox.Height;

                freeLine = true;

                foreach (bEnemy lineEnemy in enemyList)
                {

                    if (shootPosY < Canvas.GetTop(lineEnemy.hitBox))
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
                Projectile newProjectile;

                switch (enemyList[closestDr].EnemyType)
                {
                    case EnemyTypeList.Drone:
                        newProjectile = new Projectile(16, 8, enemyList[closestDr].ProjectileSpeed, game.DroneDamage,
                    enemyList[closestDr].EnemyType, spritesheetBI, spritesheetData);
                        break;
                    case EnemyTypeList.Alien:
                        newProjectile = new Projectile(16, 8, enemyList[closestDr].ProjectileSpeed, game.AlienDamage,
                    enemyList[closestDr].EnemyType, spritesheetBI, spritesheetData);
                        break;
                    case EnemyTypeList.Enforcer:
                        newProjectile = new Projectile(16, 8, enemyList[closestDr].ProjectileSpeed, game.EnforcerDamage,
                    enemyList[closestDr].EnemyType, spritesheetBI, spritesheetData);
                        break;
                    default:
                        newProjectile = new Projectile(16, 8, enemyList[closestDr].ProjectileSpeed, 1,
                    enemyList[closestDr].EnemyType, spritesheetBI, spritesheetData);
                        break;
                }

                Canvas.SetLeft(newProjectile.hitBox, Canvas.GetLeft(enemyList[closestDr].hitBox) +
                    (enemyList[closestDr].hitBox.Width - newProjectile.hitBox.Width) / 2);
                Canvas.SetTop(newProjectile.hitBox, Canvas.GetTop(enemyList[closestDr].hitBox) + enemyList[closestDr].hitBox.Height
                    - newProjectile.hitBox.Height);

                //EnemyLightUp
                enemyList[closestDr].hitBox.Fill = Brushes.Red;

                mCanvas.Children.Add(newProjectile.hitBox);
                projectileList.Add(newProjectile);

                SndHandler.Play(SoundList.EnemyShoot);
            }
        }

        private void MoveEnemiesDown()
        {
            foreach (bEnemy tmp in enemyList)
            {
                if (tmp.EnemyType != EnemyTypeList.Overseer)
                    Canvas.SetTop(tmp.hitBox, Canvas.GetTop(tmp.hitBox) + game.EnemyDescendSpeed);
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
            SndHandler.Play(SoundList.PlrShoot);

            Projectile newProjectile = new Projectile(16, 8, PlayerData.ProjectileSpeed, 1, true, plrProjectile);

            Canvas.SetLeft(newProjectile.hitBox, Canvas.GetLeft(PlayerData.hitBox) + (PlayerData.hitBox.Width - newProjectile.hitBox.Width) / 2);
            Canvas.SetTop(newProjectile.hitBox, Canvas.GetTop(PlayerData.hitBox) - newProjectile.hitBox.Height);

            mCanvas.Children.Add(newProjectile.hitBox);
            projectileList.Add(newProjectile);
        }

        private void SpawnEnemy(double x, double y, EnemyTypeList enType)
        {
            bEnemy newDrone = new bEnemy(30, 30, game.EnemySpeed, game.EnemyProjectileSpeed, 1, enType, spritesheetBI, spritesheetData);

            Canvas.SetLeft(newDrone.hitBox, x);
            Canvas.SetTop(newDrone.hitBox, y);
            mCanvas.Children.Add(newDrone.hitBox);
            enemyList.Add(newDrone);
        }

        private Explosion SpawnExplosion(double x, double y, double hght, double width)
        {
            Explosion newExpl = new Explosion(hght, width, spritesheetBI, spritesheetData);

            Canvas.SetLeft(newExpl.hitBox, x);
            Canvas.SetTop(newExpl.hitBox, y);
            mCanvas.Children.Add(newExpl.hitBox);

            return newExpl;
        }

        private void SpawnBarrierWall(int barrierNumb, double y, int barrierHealth)
        {
            //double xOffset = mCanvas.Width / barrierNumb - 20;
            double xOffset = (mCanvas.Width - (barrierNumb * 66)) / (barrierNumb + 1);

            for (int i = 0; i < barrierNumb; i++)
            {
                //SpawnBarrier((double)i * (48 + 15), y, barrierHealth);
                //SpawnBarrier((double)(i+1) * xOffset + (66 * i), y, barrierHealth);       //FIX
                SpawnBarrier(xOffset * (i + 1) + (66 * i), y, barrierHealth);       //FIX
            }
        }

        private void SpawnEnemyRow(double y, EnemyTypeList enType)
        {
            for (int i = 0; i < dronesInWave; i++)
            {
                SpawnEnemy((double)i * (30 + 15) + 10, y, enType);
            }
        }

        private void SpawnEnemyWave()
        {
            //SPAWN FROM DOWN TO TOP
            SpawnEnemyRow(235, EnemyTypeList.Drone);
            SpawnEnemyRow(200, EnemyTypeList.Drone);
            SpawnEnemyRow(165, EnemyTypeList.Alien);
            SpawnEnemyRow(130, EnemyTypeList.Alien);
            SpawnEnemyRow(95, EnemyTypeList.Enforcer);
            SpawnEnemyRow(60, EnemyTypeList.Enforcer);
            SndHandler.Play(SoundList.EnemySpawn);
        }

        private void updateUI(int plrLives, int plrScore)
        {
            scoreData.Content = plrScore;
            if (plrScore > CurrentHighScore)
            {
                highscoreData.Content = plrScore;
                CurrentHighScore = plrScore;
            }

            remainingHealthData.Content = plrLives;
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

        private void ExplosionsUpdate()
        {
            foreach (Explosion tmp in explosionList)
            {
                if (tmp.IsFinished(spritesheetData))
                {
                    tmp.RemoveExplosion(ref hitBoxGC, ref explosionGC);
                }
            }
        }

        private void ShootDelay()
        {
            if (plrShootDelay < shotDelay)
            {
                ++plrShootDelay;
            }
        }

        private void EndGame()
        {
            bool isNewHighscore = false;
            if (PlayerData.Score > 0)
            {
                int lowestHighscore = this.HighscoreList.Count > 0 ? this.HighscoreList.Min(x => x.Score) : 0;
                if ((PlayerData.Score > lowestHighscore) || (this.HighscoreList.Count < MaxHighscoreListEntryCount))
                {
                    bdrNewHighscore.Visibility = Visibility.Visible;
                    txtPlayerName.Focus();
                    isNewHighscore = true;
                }
            }
            if (!isNewHighscore)
            {
                endScreenFinalScore.Text = PlayerData.Score.ToString();
                EndGameScreen.Visibility = Visibility.Visible;
            }

            runTimer.IsEnabled = false;
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

        private void gcExplosions()
        {
            List<Explosion> toRemove = new List<Explosion>();

            foreach (var tmp in explosionGC)
            {
                explosionList.Remove(tmp);
                toRemove.Add(tmp);
            }

            foreach (var tmp in toRemove)
            {
                explosionGC.Remove(tmp);
            }

            toRemove.Clear();
        }

        private void ClearLists()
        {
            if (barrierList != null)
            {
                foreach (var tmp in barrierList)
                    tmp.RemoveBarrier(ref hitBoxGC, ref barrierGC);
                barrierList.Clear();
            }
            if (projectileList != null)
            {
                foreach (var tmp in projectileList)
                    tmp.RemoveProjectile(ref hitBoxGC, ref projectileGC);
                projectileList.Clear();
            }
            if (enemyList != null)
            {
                foreach (var tmp in enemyList)
                    tmp.RemoveEnemy(ref hitBoxGC, ref enemyGC);
                enemyList.Clear();
            }
            if (explosionList != null)
            {
                foreach (var tmp in explosionList)
                    tmp.RemoveExplosion(ref hitBoxGC, ref explosionGC);
                explosionList.Clear();
            }
        }

        private void ButtonShowScoreList(object sender, RoutedEventArgs e)
        {
            menu.Visibility = Visibility.Collapsed;
            HighScoreList.Visibility = Visibility.Visible;
            SndHandler.Play(SoundList.MenuPress);
        }

        private void GameModeSwitch(object sender, RoutedEventArgs e)
        {
            HardMode = !HardMode;

            if (HardMode)
            {
                gameModeSwitch.Content = "Повышенная сложность";
            }
            else
            {
                gameModeSwitch.Content = "Нормальнная сложность";
            }
            
            SndHandler.Play(SoundList.MenuPress);
        }

        private void StartButton(object sender, RoutedEventArgs e)
        {
            OnStart = false;
            menu.Visibility = Visibility.Collapsed;
            HighScoreList.Visibility = Visibility.Collapsed;
            EndGameScreen.Visibility = Visibility.Collapsed;
            SndHandler.Play(SoundList.MenuPress);
            GameStart();
        }

        private void LoadHighscoreList()
        {
            if (File.Exists("spcInvdrsHSList.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<HighScoreNode>));
                using (Stream reader = new FileStream("spcInvdrsHSList.xml", FileMode.Open))
                {
                    List<HighScoreNode> tempList = (List<HighScoreNode>)serializer.Deserialize(reader);
                    this.HighscoreList.Clear();
                    foreach (var item in tempList.OrderByDescending(x => x.Score))
                        this.HighscoreList.Add(item);

                    CurrentHighScore = this.HighscoreList[0].Score;
                }
            }
        }

        private void SaveHighscoreList()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<HighScoreNode>));

            using (Stream writer = new FileStream("spcInvdrsHSList.xml", FileMode.Create))
            {
                serializer.Serialize(writer, this.HighscoreList);
            }
        }

        private void BtnAddToHighscoreList_Click(object sender, RoutedEventArgs e)
        {
            SndHandler.Play(SoundList.MenuPress);

            int newIndex = 0;
            // Where should the new entry be inserted?
            if ((this.HighscoreList.Count > 0) && (PlayerData.Score < this.HighscoreList.Max(x => x.Score)))
            {
                HighScoreNode justAbove = this.HighscoreList.OrderByDescending(x => x.Score).First(x => x.Score >= PlayerData.Score);
                if (justAbove != null)
                    newIndex = this.HighscoreList.IndexOf(justAbove) + 1;
            }
            // Create & insert the new entry
            this.HighscoreList.Insert(newIndex, new HighScoreNode()
            {
                PlayerName = txtPlayerName.Text,
                Score = PlayerData.Score
            }); ;
            // Make sure that the amount of entries does not exceed the maximum
            while (this.HighscoreList.Count > MaxHighscoreListEntryCount)
                this.HighscoreList.RemoveAt(MaxHighscoreListEntryCount);

            SaveHighscoreList();

            bdrNewHighscore.Visibility = Visibility.Collapsed;
            HighScoreList.Visibility = Visibility.Visible;
        }

        private void BtnPlayAgainClck(object sender, RoutedEventArgs e)
        {
            SndHandler.Play(SoundList.MenuPress);
            GameStart();
        }

        private void BtnExitClck(object sender, RoutedEventArgs e)
        {
            SndHandler.Play(SoundList.MenuPress);
            Application.Current.Shutdown();
        }

        private void BtnBackToMainClck(object sender, RoutedEventArgs e)
        {
            SndHandler.Play(SoundList.MenuPress);

            HighScoreList.Visibility = Visibility.Collapsed;
            EndGameScreen.Visibility = Visibility.Collapsed;
            menu.Visibility = Visibility.Visible;
        }
    }
}