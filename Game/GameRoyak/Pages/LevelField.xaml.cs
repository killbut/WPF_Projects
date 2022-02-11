using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GameRoyak.Enums;
using GameRoyak.Logic;
using GameRoyak.Models;
using FilePath = System.IO.Path;

namespace GameRoyak.Pages
{
    public partial class LevelField
    {
        private bool _isAnimation;
        public Player Player { get; set; }

        private readonly Canvas _mainCanvas = new Canvas();
        private Ellipse _torch;

        private double _upDownMove;
        private double _leftRightMove;

        private int _numPreviousPlayerX;
        private int _numPreviousPlayerY;

        public static MediaPlayer SoundSteps = new MediaPlayer();
        public static MediaPlayer SoundBackground = new MediaPlayer();
        
        public LevelField(Player player)
        {
            InitializeComponent();
            Player = player;
            this.DataContext = Player;
            MainGrid.Focus();
            this.Loaded += StartGame;
            MainGrid.KeyDown += OnKeyDown;
        }
        private void StartGame(object sender, RoutedEventArgs e)
        {
            Inventory.Items.Clear();
            foreach (var item in Player.Items)
                Inventory.Items.Add($"{item.Name}: D-{item.Damage}, HP-{item.Hp}, A-{item.Armor}");
            this.DataContext = Player;
            if (!StatePage.IsStartGame) return;
            InitialStartValues();
            FieldProvider.GenerateField();
            PaintField();
            StatePage.IsStartGame = false;
        }
        private void InitialStartValues()
        {
            _leftRightMove = MainGrid.ColumnDefinitions[0].ActualWidth;
            _upDownMove = MainGrid.RowDefinitions[0].ActualHeight;

            SoundSteps.Open(new Uri(FilePath.Combine(Settings.Directory, "Audio/Steps.mp3")));
            SoundBackground.Open(new Uri(FilePath.Combine(Settings.Directory, "Audio/Background.mp3")));
            SoundBackground.Stop();
            SoundBackground.Play();
            SoundBackground.Volume = Settings.Volume;
            SoundBackground.MediaEnded += SoundBackgroundOnMediaEnded;
        }

        private void SoundBackgroundOnMediaEnded(object sender, EventArgs e)
        {
            SoundBackground.Stop();
            SoundBackground.Play();
        }
        
        private void PaintField()
        {
            PlayerIcon.Fill = new ImageBrush
            {
                ImageSource = Player.Icon
            };
            Grid.SetColumnSpan(_mainCanvas, FieldProvider.NumColumn);
            Grid.SetRowSpan(_mainCanvas, FieldProvider.NumRow);
            for (var i = 0; i < FieldProvider.NumColumn; i++)
            {
                for (var j = 0; j < FieldProvider.NumRow; j++)
                {
                    if (!FieldProvider.Field[i][j].IsVisited)
                    {
                        var stateCell = new Image
                        {
                            Stretch = Stretch.Fill
                        };
                        
                        Grid.SetColumn(stateCell, i);
                        Grid.SetRow(stateCell, j);
                        MainGrid.Children.Add(stateCell);
                    }
                    else
                    {
                        var stateCell = new Image
                        {
                            Source = new BitmapImage(new Uri(FilePath.Combine(Settings.Directory, "Images/start.png"))),
                            Stretch = Stretch.Fill
                        };
                        
                        Grid.SetColumn(stateCell, i);
                        Grid.SetRow(stateCell, j);
                        MainGrid.Children.Add(stateCell);
                    }
                }
            }

            _torch = new Ellipse {Width = Settings.TorchWidth, Height = Settings.TorchHeight};
            var brushTorch = new RadialGradientBrush
            {
                Center = new Point(0.5, 0.5),
                GradientOrigin = new Point(0.5, 0.5)
            };
            brushTorch.GradientStops.Add(new GradientStop(Colors.White, 0));
            brushTorch.GradientStops.Add(new GradientStop(Colors.Red, 0.2));
            brushTorch.GradientStops.Add(new GradientStop(Colors.Orange, 0.7));
            brushTorch.GradientStops.Add(new GradientStop(Colors.Yellow, 1));
            
            _torch.Fill = brushTorch;
            _torch.Opacity = Settings.TorchOpacity;
            _torch.Effect = new BlurEffect {Radius = Settings.TorchEffectRadius};
            _mainCanvas.Children.Add(_torch);
            Canvas.SetTop(_torch, Player.Y * _upDownMove + (_upDownMove - _torch.Height) / 2);
            Canvas.SetLeft(_torch, Player.X * _leftRightMove + (_leftRightMove - _torch.Width) / 2);
            _mainCanvas.Children.Add(Player.Image);
            Canvas.SetTop(Player.Image, Player.Y * _upDownMove + (_upDownMove - Player.Image.Height) / 2);
            Canvas.SetLeft(Player.Image, Player.X * _leftRightMove + (_leftRightMove - Player.Image.Width) / 2);
            MainGrid.Children.Add(_mainCanvas);
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W when !_isAnimation && FieldProvider.Field[Player.X][Player.Y].WDirection:
                    Player.Y--;
                    MoveAnimation(Directions.Up);
                    break;
                case Key.A when !_isAnimation && FieldProvider.Field[Player.X][Player.Y].ADirection:
                    Player.X--;
                    MoveAnimation(Directions.Left);
                    break;
                case Key.S when !_isAnimation && FieldProvider.Field[Player.X][Player.Y].SDirection:
                    Player.Y++;
                    MoveAnimation(Directions.Down);
                    break;
                case Key.D when !_isAnimation && FieldProvider.Field[Player.X][Player.Y].DDirection:
                    Player.X++;
                    MoveAnimation(Directions.Right);
                    break;
                case Key.Escape when !_isAnimation:
                    StatePage.State = StatesWindow.Menu;
                    break;
                // case Key.R when !_isAnimation:
                //     StatePage.State = StatesWindow.Restart;
                //     break;
            }
        }

        private void MoveAnimation(Directions moveDirection)
        {
            SoundSteps.Play();
            _isAnimation = true;
            var animation = new ThicknessAnimation {From = Player.Image.Margin};
            switch (moveDirection)
            {
                case Directions.Up:
                    animation.To = new Thickness(Player.Image.Margin.Left, Player.Image.Margin.Top - _upDownMove,
                        0, 0);
                    break;
                case Directions.Left:
                    animation.To = new Thickness(Player.Image.Margin.Left - _leftRightMove, Player.Image.Margin.Top,
                        0, 0);
                    break;
                case Directions.Down:
                    animation.To = new Thickness(Player.Image.Margin.Left, Player.Image.Margin.Top + _upDownMove,
                        0, 0);
                    break;
                case Directions.Right:
                    animation.To = new Thickness(Player.Image.Margin.Left + _leftRightMove, Player.Image.Margin.Top,
                        0, 0);
                    break;
            }
            animation.Duration = TimeSpan.FromMilliseconds(Settings.SpeedAnimation);
            animation.Completed += AnimationOnCompleted;
            _torch.BeginAnimation(MarginProperty, animation);
            Player.Image.BeginAnimation(MarginProperty, animation);
        }
        private void PaintCurrentAndPreviousCell()
        {
            var cellInMatrix = FieldProvider.Field[Player.X][Player.Y];
            var currentCell = (Image)MainGrid.Children[Player.Y + (Player.X * FieldProvider.NumRow)];
            var savePreviousCell = (Image)MainGrid.Children[_numPreviousPlayerY + (_numPreviousPlayerX * FieldProvider.NumRow)];
            var previousCell = (Image)MainGrid.Children[_numPreviousPlayerY + (_numPreviousPlayerX * FieldProvider.NumRow)];
            FieldProvider.Field[_numPreviousPlayerX][_numPreviousPlayerY].OpacityCell = Settings.OpacityDarkRoom;
            cellInMatrix.OpacityCell = 1;
            previousCell.Opacity = FieldProvider.Field[_numPreviousPlayerX][_numPreviousPlayerY].OpacityCell;
            if (FieldProvider.Field[_numPreviousPlayerX][_numPreviousPlayerY].IsWorking)
                previousCell.Effect = new BlurEffect
                {
                    Radius = 10
                };
            else
            {
                previousCell.Effect = new BlurEffect
                {
                    Radius = 0
                };
            }

            _numPreviousPlayerX = Player.X;
            _numPreviousPlayerY = Player.Y;
            currentCell.Opacity = cellInMatrix.OpacityCell;
            if (cellInMatrix.CellNum == Settings.NumRoomShop && !cellInMatrix.IsVisited)
                StatePage.State = StatesWindow.Shop;
            if (cellInMatrix.CellNum == Settings.NumRoomChest && !cellInMatrix.IsVisited)
                StatePage.State = StatesWindow.Chest;
            if (cellInMatrix.CellNum == Settings.NumRoomHeal && !cellInMatrix.IsVisited)
                StatePage.State = StatesWindow.Heal;
            if (cellInMatrix.CellNum == Settings.NumRoomFight && !cellInMatrix.IsVisited)
                StatePage.State = StatesWindow.Fight;
            if (cellInMatrix.CellNum == Settings.NumRoomBoss && !cellInMatrix.IsVisited)
            {
                FieldProvider.IsBossBattle = true;
                StatePage.State = StatesWindow.Fight;
            }

            if (cellInMatrix.IsVisited) return;
            currentCell.Source = new BitmapImage(
                new Uri(FilePath.Combine(Settings.Directory,
                    $"Images/cell{cellInMatrix.CellNum}/" +
                    $"{ConvertBoolToInt(cellInMatrix.WDirection)}" +
                    $"_{ConvertBoolToInt(cellInMatrix.ADirection)}" +
                    $"_{ConvertBoolToInt(cellInMatrix.SDirection)}" +
                    $"_{ConvertBoolToInt(cellInMatrix.DDirection)}.png")));

            cellInMatrix.IsVisited = true;
        }
        private void AnimationOnCompleted(object sender, EventArgs e)
        {
            _isAnimation = false;
            SoundSteps.Stop();
            PaintCurrentAndPreviousCell();
        }

        private int ConvertBoolToInt(bool boolToConvert) => boolToConvert ? 1 : 0;
    }
}