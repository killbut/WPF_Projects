using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GameRoyak.Logic;
using GameRoyak.Models;

using FilePath = System.IO.Path;
using WpfAnimatedGif;
using Image = System.Windows.Controls.Image;

namespace GameRoyak.Pages
{
    /// <summary>
    /// Логика взаимодействия для View.xaml
    /// </summary>
    public partial class Fight : Page
    {
        private readonly string _directory = FilePath.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static Random _generator = null;
        private string message { get; set; }
        private Player Player { get; set; }
        public int arm { get; set; }
        public Fight(Player PlayerClass)
        {
            InitializeComponent();
            Player = PlayerClass;
            Player.Name = Player.Class;
            Player.ChanceFromEscape = 50;
            arm = Player.Armor;
            BattleModel ViewModel = new BattleModel(Player);
            this.DataContext = ViewModel;
            message = (this.DataContext as BattleModel).Message;
            firstButton.Focus();
            RenderField();
        }

        private void RenderField()
        {
            if (_generator == null)
                _generator = new Random(DateTime.Now.Millisecond);

            var battleground = new Image
            {
                Source = new BitmapImage(new Uri(FilePath.Combine(_directory, $"Images/background{_generator.Next(1, 3)}.png"))),
                Stretch = Stretch.UniformToFill
            };
            Grid.SetRow(battleground, 0);
            MainGrid.Children.Add(battleground);

            Button playerInScreen = new Button();
            playerInScreen.Style = this.Resources["test"] as Style;
            playerInScreen.ApplyTemplate();
            playerInScreen.IsEnabled = false;
            playerInScreen.DataContext = Player;
            var imagePlayer=(Image)playerInScreen.Template.FindName("ImageInStyle", playerInScreen);
            imagePlayer.Source = new BitmapImage(new Uri(Player.PathToImage));
            imagePlayer.Width = _generator.Next(50, 200);
            imagePlayer.Height = _generator.Next(50, 200);
            ImageBehavior.SetAnimatedSource(imagePlayer, imagePlayer.Source);
            Canvas.SetLeft(playerInScreen,60);
            Canvas.SetBottom(playerInScreen,40);

            var playerImage = new Image
            {
                Width = _generator.Next(50, 200),
                Height = _generator.Next(50, 200),
                Source = new BitmapImage(new Uri(Player.PathToImage))
            };
            ImageBehavior.SetAnimatedSource(playerImage, playerImage.Source);

            Button player = new Button();
            player.DataContext = Player;
            player.IsEnabled = false;
            player.Style = this.Resources["PlayerInformation"] as Style;
            
            PlayerGrid.Children.Add(player);
            
            Canvas fieldCanvas = new Canvas();
            //Binding myBinding = new Binding()
            //{
            //    Source = (this.DataContext as BattleModel),
            //    Mode = BindingMode.TwoWay,
            //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            //    Path = new PropertyPath("Message")
            //};

            //MessageTextBlock.VerticalAlignment = VerticalAlignment.Center;
            //MessageTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            //BindingOperations.SetBinding(MessageTextBlock, TextBlock.TextProperty, myBinding);

            
            //Panel.SetZIndex(MessageTextblock,2);
            //Canvas.SetTop(MessageTextblock,50);
            //Canvas.SetLeft(MessageTextblock,500);
           

            MainGrid.Children.Add(fieldCanvas);
            //fieldCanvas.Children.Add(MessageTextblock);
            fieldCanvas.Children.Add(playerInScreen);
            //Canvas.SetLeft(playerImage, 10);
            //Canvas.SetBottom(playerImage, 10);
            //fieldCanvas.Children.Add(playerImage);
            int lengthFromLeft = 10;

            int currentRow = 0;
            
            foreach (var item in (this.DataContext as BattleModel).EnemyObsColl)
            {
                Button enemyImage = new Button();
                enemyImage.Style = this.Resources["test"] as Style;
                enemyImage.IsEnabled = false;
                enemyImage.ToolTip = item.Name;
                enemyImage.DataContext = item;
                enemyImage.ApplyTemplate();

                var enemyImageSource= (Image)enemyImage.Template.FindName("ImageInStyle", enemyImage);
                enemyImageSource.Source = new BitmapImage(new Uri(FilePath.Combine(_directory, item.Image)));
                enemyImageSource.Width = _generator.Next(50, 200);
                enemyImageSource.Height = _generator.Next(50, 200);
                
                Canvas.SetRight(enemyImage, lengthFromLeft);
                lengthFromLeft += 200;
                Canvas.SetBottom(enemyImage, 10);
                fieldCanvas.Children.Add(enemyImage);

                Button enemy = new Button();
                enemy.DataContext = item;
                enemy.Command = (this.DataContext as BattleModel).GetDamageCommand;

                //List<Button> ObjectEnemy = new List<Button>();
                //ObjectEnemy.Add(enemyImage);
                //ObjectEnemy.Add(enemy);
                //Tuple<List<Button>, Grid> parameter = new Tuple<List<Button>,Grid>(ObjectEnemy,EnemiesGrid);
                MultiBinding multiBindingParameter = new MultiBinding();
                multiBindingParameter.Converter = new MultiConverter();
                multiBindingParameter.Bindings.Add(new Binding(){Source = enemyImage});
                multiBindingParameter.Bindings.Add(new Binding(){Source = enemy});
                multiBindingParameter.NotifyOnSourceUpdated = true;
                BindingOperations.SetBinding(enemy,Button.CommandParameterProperty,multiBindingParameter);

                enemy.IsEnabled = false;
                enemy.Focusable = true;
                enemy.Style = this.Resources["HUD"] as Style;
                enemy.Name = item.Name;
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1.0, GridUnitType.Star);

                EnemiesGrid.RowDefinitions.Add(row);
                Grid.SetRow(enemy, currentRow++);
                EnemiesGrid.Children.Add(enemy);
            }
            //todo изображение внизу хп сверху имя
        }

        //private Button AddPlayer()
        //{
        //    Button player = new Button();
        //    player.IsEnabled = false;
        //    Binding myBinding = new Binding();
        //    myBinding.Source = Player;
        //    myBinding.Mode = BindingMode.TwoWay;
        //    myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        //    myBinding.Path = new PropertyPath("CurrentHP");

        //    TextBlock Name = new TextBlock() {Text = Player.Name,Margin =new Thickness(10,0,0,0)};

        //    ProgressBar HP = new ProgressBar()
        //    {
        //        Value = Player.CurrentHP, Maximum = Player.HP, Minimum = 0,
        //        Width = 200,

        //    };
        //    BindingOperations.SetBinding(HP, ProgressBar.ValueProperty, myBinding);
            

        //    TextBlock Damage = new TextBlock() {Text = Player.Damage.ToString(), Margin = new Thickness(20, 0, 0, 0)};

        //    WrapPanel InformationPanel = new WrapPanel();
        //    InformationPanel.Children.Add(Name);
        //    InformationPanel.Children.Add(HP);
        //    InformationPanel.Children.Add(Damage);

        //    player.Content = InformationPanel;
        //    player.IsEnabled = false;
        //    player.Focusable = false;

        //    return player;
        //}

        private void Fight_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(firstButton);
        }
    }
}
