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

using galEngine.ViewModels;

namespace CW_HLL2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Entity Player;

        private GameSession _gameSession;

        int plrSpeed = 10;
        bool movUp, movDown, movLeft, movRight;
        const int gameWidth = 240;
        //readonly double plAreaH = 960;
        readonly double plAreaW = 480;
        double SCREENHEIGHT = 960;
        double SCREENWIDTH = 640;

        DispatcherTimer runTimer = new DispatcherTimer();
        ImageBrush plrSkin = new ImageBrush();

        public MainWindow()
        {
            InitializeComponent();

            runTimer.Tick += GameLoop;
            runTimer.Interval = TimeSpan.FromMilliseconds(16);
            runTimer.Start();

            plrSkin.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/res/ship.png"));
            //plrSkin.ImageSource = new BitmapImage(new Uri("C:/Users/R3SH/source/repos/HLL2/CW_HLL2/res/ship.png"));
            Player.Fill = plrSkin;
            
            mCanvas.Focus();
            //SCREENHEIGHT = System.Windows.SystemParameters.PrimaryScreenHeight;
            //SCREENWIDTH = this.Width;

            Canvas.SetTop(Player, SCREENHEIGHT / 2);
            Canvas.SetLeft(Player, SCREENWIDTH / 2);
            
            //_gameSession = new GameSession();
            //DataContext = _gameSession;

            //SCREENHEIGHT = ((Panel)Application.Current.MainWindow.Content).ActualHeight;
            //SCREENWIDTH = ((Panel)Application.Current.MainWindow.Content).ActualWidth;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            //INPUT HANDLING
            if(movLeft && Canvas.GetLeft(Player) > 0)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - plrSpeed);
            }
            //if (movRight && Canvas.GetLeft(Player) < (SCREENWIDTH - Player.Width))
            if(movRight && (Canvas.GetLeft(Player) + Player.ActualWidth + 10) < mCanvas.ActualWidth)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + plrSpeed);
            }
            //if(movDown && Canvas.GetTop(Player) < (SCREENHEIGHT - Player.Height))
            if(movDown && (Canvas.GetTop(Player) + Player.Height + 10) < mCanvas.ActualHeight)
            {
                Canvas.SetTop(Player, Canvas.GetTop(Player) + plrSpeed);
            }
            if(movUp && Canvas.GetTop(Player) > 0)
            {
                Canvas.SetTop(Player, Canvas.GetTop(Player) - plrSpeed);
            }

            SpawnZako();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current.Shutdown();
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
                Rectangle newProjectile = new Rectangle()
                {
                    Tag = "projectile",
                    Height = 20,
                    Width = 4,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red,
                };

                Canvas.SetLeft(newProjectile, Canvas.GetLeft(Player) + Player.Width / 2 - 2);
                Canvas.SetTop(newProjectile, Canvas.GetTop(Player) - newProjectile.Height);

                mCanvas.Children.Add(newProjectile);
            }
        }

        private void SpawnZako()
        {
            ImageBrush zakoSprite = new ImageBrush();
            zakoSprite.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/res/zako.png"));

            Rectangle newZako = new Rectangle
            {
                Tag = "zako",
                Height = 40,
                Width = 40,
                Fill = zakoSprite
            };

            Canvas.SetTop(newZako, 100);
            Canvas.SetLeft(newZako, 100);

            mCanvas.Children.Add(newZako);
        }
    }
}
