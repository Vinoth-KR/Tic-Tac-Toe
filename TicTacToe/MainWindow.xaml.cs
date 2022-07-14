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
using TicTacToe.Enums;
using TicTacToe.Models;
using System.Windows.Media.Animation;
using NAudio.Wave;
using System.IO;
using System.Windows.Resources;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<Player, ImageSource> imageSources = new Dictionary<Player, ImageSource>()
        {
            { Player.X, new BitmapImage(new Uri("pack://application:,,,/Assets/X15.png")) },
            { Player.O, new BitmapImage(new Uri("pack://application:,,,/Assets/O15.png")) },
            { Player.None, new BitmapImage(new Uri("pack://application:,,,/Assets/Draw.png"))}
        };

        private MediaPlayer audioPlayer = new MediaPlayer();

        private readonly Dictionary<string, Uri> audioSources = new Dictionary<string, Uri>()
        {
            { "MoveMade", new Uri("pack://application:,,,/Assets/MoveMade.wav") },
            { "Winner", new Uri("pack://application:,,,/Assets/Winner.wav") },
            { "Draw", new Uri("pack://application:,,,/Assets/Draw.wav") }
        };

        private readonly Dictionary<Player, string> winnerStrikeLineColorMap = new Dictionary<Player, string>()
        {
            { Player.X, "#545454" },
            { Player.O, "#f2ebd3" }
        };

        private readonly Dictionary<Player, ObjectAnimationUsingKeyFrames> animations = new Dictionary<Player, ObjectAnimationUsingKeyFrames>()
        {
            { Player.X, new ObjectAnimationUsingKeyFrames() },
            { Player.O, new ObjectAnimationUsingKeyFrames() }
        };

        private readonly DoubleAnimation fadeOutAnimation = new DoubleAnimation
        {
            Duration = TimeSpan.FromSeconds(0.5),
            From = 1,
            To = 0
        };

        private readonly DoubleAnimation fadeInAnimation = new DoubleAnimation
        {
            Duration = TimeSpan.FromSeconds(0.5),
            From = 0,
            To = 1
        };

        private Dictionary<Player, int> winningStreakCounter = new Dictionary<Player, int>()
        {
            { Player.X, 0},
            { Player.O, 0}
        };
        private readonly Image[,] imageControls = new Image[3, 3];
        private readonly GameState gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            SetUpGameGrid();
            SetupAnimations();
            UpdateWinningStreakCount();

            gameState.MoveMade += OnMoveMade;
            gameState.GameEnded += OnGameEnded;
            gameState.GameRestarted += OnGameRestarted;
        }

        private void SetUpGameGrid()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    Image imageControl = new Image();
                    GameGrid.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
        }

        private void SetupAnimations()
        {
            animations[Player.X].Duration = TimeSpan.FromSeconds(.25);
            animations[Player.O].Duration = TimeSpan.FromSeconds(.25);

            for (int i = 0; i < 16; i++)
            {
                Uri xUri = new Uri($"pack://application:,,,/Assets/X{i}.png");
                BitmapImage xImg = new BitmapImage(xUri);
                DiscreteObjectKeyFrame xKeyFrame = new DiscreteObjectKeyFrame(xImg);
                animations[Player.X].KeyFrames.Add(xKeyFrame);

                Uri oUri = new Uri($"pack://application:,,,/Assets/O{i}.png");
                BitmapImage oImg = new BitmapImage(oUri);
                DiscreteObjectKeyFrame oKeyFrame = new DiscreteObjectKeyFrame(oImg);
                animations[Player.O].KeyFrames.Add(oKeyFrame);

            }
        }


        private async Task FadeOut(UIElement uiElement)
        {
            uiElement.BeginAnimation(OpacityProperty, fadeOutAnimation);
            await Task.Delay(fadeOutAnimation.Duration.TimeSpan);
            uiElement.Visibility = Visibility.Hidden;
        }

        private async Task FadeIn(UIElement uiElemenet)
        {
            uiElemenet.Visibility = Visibility.Visible;
            uiElemenet.BeginAnimation(OpacityProperty, fadeInAnimation);
            await Task.Delay(fadeInAnimation.Duration.TimeSpan);        
        }
        private void OnMoveMade(int r, int c)
        {
            Player player = gameState.GameGrid[r, c];
            imageControls[r, c].BeginAnimation(Image.SourceProperty, animations[player]);
            PlayerImage.Source = imageSources[gameState.CurrentPlayer];

            PlaySound("MoveMade");
        }

        private void PlaySound(string key)
        {            
            StreamResourceInfo streamInfo = Application.GetResourceStream(audioSources[key]);            
            using (var waveOut = new WaveOutEvent())
            using (var wavReader = new WaveFileReader(streamInfo.Stream))
            {
                waveOut.Init(wavReader);
                waveOut.Play();
            }
        }

        private async Task TransitiontoEndScreen(string text, ImageSource winnerImage)
        {
            await Task.WhenAll(FadeOut(TopPanel), FadeOut(GameCanvas)); 
            ResultText.Text = text;
            WinnerImage.Source = winnerImage;
            await FadeIn(EndScreen);            
        }

        private (Point, Point) FindLinePoints(StrikeInfo strikeInfo)
        {
            double squareSize = GameGrid.Width / 3;
            double margin = squareSize / 2;

            if(strikeInfo.StrikeType ==StrikeOutRule.Row )
            {
                double y = strikeInfo.Index * squareSize + margin;
                return (new Point(0, y), new Point(GameGrid.Width, y)); 
            }

            if(strikeInfo.StrikeType == StrikeOutRule.Column)
            {
                double x = strikeInfo.Index * squareSize + margin;
                return (new Point(x, 0), new Point(x, GameGrid.Height));
            }

            if(strikeInfo.StrikeType == StrikeOutRule.LeftDiagonal)
            {
                return (new Point(0, 0), new Point(GameGrid.Width, GameGrid.Height));
            }

            return (new Point(GameGrid.Width, 0), new Point(0, GameGrid.Height));
        }

        private async Task ShowLine(GameResult gameResult)
        {
            (Point start, Point end) = FindLinePoints(gameResult.StrikeInfo);

            StrikeLine.X1 = start.X;
            StrikeLine.Y1 = start.Y;

            DoubleAnimation xAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.25),
                From = start.X,
                To = end.X
            };

            DoubleAnimation yAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.25),
                From = start.Y,
                To = end.Y
            };

            SolidColorBrush colorBrush = new SolidColorBrush();
            colorBrush.Color = (Color)ColorConverter.ConvertFromString(winnerStrikeLineColorMap[gameResult.Winner]);
            StrikeLine.Stroke = colorBrush;

            StrikeLine.Visibility = Visibility.Visible;
            StrikeLine.BeginAnimation(Line.X2Property, xAnimation);
            StrikeLine.BeginAnimation(Line.Y2Property, yAnimation);

            await Task.Delay(xAnimation.Duration.TimeSpan);

        }

        private async void OnGameEnded(GameResult gameResult)
        {
            await Task.Delay(1000);

            if (gameResult.Winner == Player.None)
            {
                await TransitiontoEndScreen("DRAW !", imageSources[gameResult.Winner]);
                PlaySound("Draw");
            }
            else
            {
               await ShowLine(gameResult);
               await Task.Delay(1000);
               await TransitiontoEndScreen("WINNER ! ", imageSources[gameResult.Winner]);
               PlaySound("Winner");
               winningStreakCounter[gameResult.Winner]++;
                
            }

        }

        private async void OnGameRestarted()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    imageControls[r, c].BeginAnimation(Image.SourceProperty, null);
                    imageControls[r, c].Source = null;
                }
            }

            PlayerImage.Source = imageSources[gameState.CurrentPlayer];
            UpdateWinningStreakCount();
            await TransitionToGameScreen();
        }

        private void UpdateWinningStreakCount()
        {
            xStreakCount.Text = $" X  -  {winningStreakCounter[Player.X]}";
            oStreakCount.Text = $" O  -  {winningStreakCounter[Player.O]}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameState.GameOver)
            {
                gameState.Restart();
            }
        }

        private async Task TransitionToGameScreen()
        {
            await FadeOut(EndScreen);
            StrikeLine.Visibility = Visibility.Hidden;
            await Task.WhenAll(FadeIn(TopPanel), FadeIn(GameCanvas));
        }

        private void GameGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double squareSize = GameGrid.Width / 3;
            Point clickPosition = e.GetPosition(GameGrid);
            int row = (int)(clickPosition.Y / squareSize);
            int col = (int)(clickPosition.X / squareSize);
            gameState.MakeMove(row, col);
        }

    }
}
