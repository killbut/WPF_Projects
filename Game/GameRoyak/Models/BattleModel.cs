using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using GameRoyak.Enums;
using GameRoyak.Logic;
using GameRoyak.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FilePath = System.IO.Path;

namespace GameRoyak.Models
{
    public class BattleModel : INotifyPropertyChanged
    {
        private RelayCommand _showSecondPanelCommand;
        private RelayCommand _returnPreviousPanelCommand;
        private RelayCommand _showThirdPanelCommand;
        private RelayCommand _attackCommand;
        private RelayCommand _getDamageCommand;
        private RelayCommand _escapeFromBattleCommand;
        private Player _player;
        private string _message;
        private Grid EnemiesGrid;
        private StackPanel SecondPanel;
        private Random _random;
        private int TypeReturn = 0;
        private int _cooldownEscapeButton;
        
        private ObservableCollection<Enemy> _enemyObsColl;
        private Button StunButton;
        private Button FreezeButton;
        private Button EscapeButton;
        private static Random _generator = null;
        private string attackType
        {
            get; set;
        }
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public Player Player
        {
            get => _player;
            set
            {
                _player = value;
                OnPropertyChanged();
            }
        }

        private int CooldownEscapeButton
        {
            get => _cooldownEscapeButton;
            set
            {
                if(value<=0)
                    _cooldownEscapeButton = 0;
                else
                    _cooldownEscapeButton=value;
            }
        }
        public RelayCommand EscapeFromBattleCommand
        {
            get
            {
                return _escapeFromBattleCommand ?? (_escapeFromBattleCommand = new RelayCommand(obj =>
                {
                    if (EnemyObsColl.Count > 0)
                    {
                        FieldProvider.Field[_player.X][_player.Y].IsVisited = false;
                        FieldProvider.Field[_player.X][_player.Y].IsWorking = true;
                    }

                    EscapeButton=obj as Button;
                    if (CooldownEscapeButton == 0)
                    {
                        SplitAtRandom(Player.ChanceFromEscape,
                            () => { StatePage.State = StatesWindow.LevelField; },
                            () => { Message += "Вы не смогли избежать боя\n"; });
                        Message += "Побег будет доступен через 2 хода\n";
                        CooldownEscapeButton = 2;
                        (obj as Button).IsEnabled = false;
                        EnemiesDealDamage();
                    }
                }));
            }
        }
        private void SplitAtRandom(int chanceOfSuccess, Action onSuccess, Action onFailure)
        {
            // Seed
            if (_generator == null)
                _generator = new Random(DateTime.Now.Millisecond);

            // By chance
            if (_generator.Next(100) < chanceOfSuccess)
            {
                if (onSuccess != null)
                    onSuccess();
            }
            else
            {
                if (onFailure != null)
                {
                    onFailure();
                }
            }
        }
        public ObservableCollection<Enemy> EnemyObsColl
        {
            get => _enemyObsColl;
            set
            {
                _enemyObsColl = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand ReturnPreviousPanelCommand
        {
            get
            {
                return _returnPreviousPanelCommand ?? (_returnPreviousPanelCommand = new RelayCommand(obj =>
                {
                    if (TypeReturn == 0)
                    {
                        var listStackPanel = ((obj as List<object>)[0] as Grid).Children.OfType<StackPanel>().ToList();
                        var currentPanel = listStackPanel.Find(x => x.Visibility == Visibility.Visible);
                        switch (currentPanel.Name)
                        {
                            case "FirstPanel":
                                {
                                    listStackPanel[0].Children.OfType<Button>().First().Focus();
                                    break;
                                }
                            case "SecondPanel":
                                {
                                    currentPanel.Visibility = Visibility.Hidden;
                                    listStackPanel[0].Visibility = Visibility.Visible;
                                    listStackPanel[0].Children.OfType<Button>().First().Focus();
                                    break;
                                }
                            case "ThirdPanel":
                                {
                                    currentPanel.Visibility = Visibility.Hidden;
                                    listStackPanel[1].Visibility = Visibility.Visible;
                                    listStackPanel[1].Children.OfType<Button>().First().Focus();
                                    break;
                                }
                        }
                    }
                    else if (TypeReturn == 1)
                    {
                        var listStackPanel = ((obj as List<object>)[0] as Grid).Children.OfType<StackPanel>().ToList();
                        listStackPanel[1].IsEnabled = true;
                        ((obj as List<object>)[1] as Grid).IsEnabled = false;
                        TypeReturn = 0;
                    }
                }));
            }
        }
        public RelayCommand ShowSecondPanelCommand
        {
            get
            {
                return _showSecondPanelCommand ?? (_showSecondPanelCommand = new RelayCommand(obj =>
                {
                    var listStackPanel = (obj as Grid).Children.OfType<StackPanel>().ToList();
                    listStackPanel[0].Visibility = Visibility.Hidden;
                    listStackPanel[1].Visibility = Visibility.Visible;
                    listStackPanel[1].Children.OfType<Button>().First().Focus();
                }
                ));
            }
        }

        public RelayCommand ShowThirdPanelCommand
        {
            get
            {
                return _showThirdPanelCommand ?? (_showThirdPanelCommand = new RelayCommand(obj =>
                {
                    var listStackPanel = (obj as Grid).Children.OfType<StackPanel>().ToList();

                    listStackPanel[1].Visibility = Visibility.Hidden;
                    listStackPanel[2].Visibility = Visibility.Visible;
                    listStackPanel[2].Children.OfType<Button>().First().Focus();
                }));
            }
        }

        public RelayCommand AttackCommand
        {
            get
            {
                return _attackCommand ?? (_attackCommand = new RelayCommand(obj =>
                {
                    var inputParams = (List<object>)obj;
                    switch (inputParams[2])
                    {
                        case "Stun":
                            {
                                attackType = inputParams[2].ToString();
                                StunButton = inputParams[3] as Button;
                                break;
                            }
                        case "Common":
                            {
                                attackType = inputParams[2].ToString();
                                break;
                            }
                        case "Freeze":
                            {
                                attackType = inputParams[2].ToString();
                                FreezeButton=inputParams[3] as Button;
                                break;
                            }
                    }
                    (inputParams[0] as StackPanel).IsEnabled = false;
                    (inputParams[1] as Grid).IsEnabled = true;
                    TypeReturn = 1;
                    SecondPanel = inputParams[0] as StackPanel;
                    EnemiesGrid = inputParams[1] as Grid;
                    var listChildren = (inputParams[1] as Grid).Children.OfType<Button>().ToList();
                    foreach (var item in listChildren)
                    {
                        if (item.Name != Player.Name)
                        {
                            item.IsEnabled = true;
                            item.Focus();
                        }
                    }

                    //var tuple = (Tuple<object, object>)obj;
                    //(tuple.Item1 as StackPanel).IsEnabled = false;
                    //(tuple.Item2 as Grid).IsEnabled = true;
                    //EnemiesGrid = tuple.Item2 as Grid;
                    //SecondPanel = tuple.Item1 as StackPanel;
                    //TypeReturn = 1;
                    //var list = (tuple.Item2 as Grid).Children.OfType<Button>().ToList();
                    //foreach (var item in list)
                    //{
                    //    if (item.Name != Player.Name)
                    //    {
                    //        item.IsEnabled = true;
                    //        item.Focus();
                    //    }
                    //}
                }));
            }
        }

        public RelayCommand GetDamageCommand
        {
            get
            {
                return _getDamageCommand ?? (_getDamageCommand = new RelayCommand(obj =>
                {
                    CooldownEscapeButton--;
                    if (CooldownEscapeButton == 0 && EscapeButton !=null)
                        EscapeButton.IsEnabled = true;
                    var inputParams = (List<object>)obj;
                    var enemy = (inputParams[1] as Button).DataContext;
                    var enemyImage = (inputParams[0] as Button);
                    switch (attackType)
                    {
                        case "Stun":
                            (enemy as Enemy).Status = attackType;
                            (enemy as Enemy).GetDamage(Player.Damage/2);
                            Message += $"Вы оглушили {(enemy as Enemy).Name} и нанесли половину урона = {Player.Damage / 2}\n";
                            StunButton.IsEnabled = false;
                            Task.Delay(1000);
                            break;
                        case "Common":
                            EnemyObsColl.Single(x => x.ID == (enemy as Enemy).ID).Status = "Common";
                            (enemy as Enemy).Status = "Common";
                            (enemy as Enemy).GetDamage(Player.Damage);
                            Message += $"Вы ударили {(enemy as Enemy).Name} и нанесели {Player.Damage}\n";
                            Task.Delay(1000);
                            break;
                        case "Freeze":
                            (enemy as Enemy).Status = attackType;
                            Message += $"Вы заморозили {(enemy as Enemy).Name}\n";
                            FreezeButton.IsEnabled = false;
                            Task.Delay(1000);
                            break;
                    }

                    if ((enemy as Enemy).CurrentHP <= 0)
                    {
                        FieldProvider.Field[_player.X][_player.Y].IsVisited = true;
                        FieldProvider.Field[_player.X][_player.Y].IsWorking = false;

                        if (FieldProvider.IsBossBattle)
                        {
                            DoubleAnimation buttonAnimation = new DoubleAnimation();
                            buttonAnimation.From = (inputParams[0] as Button).Opacity;
                            buttonAnimation.To = 0;
                            buttonAnimation.Duration = TimeSpan.FromSeconds(3);
                            (inputParams[0] as Button).BeginAnimation(Button.OpacityProperty, buttonAnimation);
                            
               
                            FieldProvider.IsBossBattle = false;
                            StatePage.State = StatesWindow.Main;
                        }
                        else
                        {
                            if (EnemiesGrid.Children.Contains((inputParams[1] as Button)))
                                EnemiesGrid.Children.Remove((inputParams[1] as Button));                
                            DoubleAnimation buttonAnimation = new DoubleAnimation();
                            buttonAnimation.From = (inputParams[0] as Button).Opacity;
                            buttonAnimation.To = 0;
                            buttonAnimation.Duration = TimeSpan.FromSeconds(1);
                            (inputParams[0] as Button).BeginAnimation(Button.OpacityProperty, buttonAnimation);
                            if ((EnemiesGrid).Children.Count == 0)
                            {
                                Player.Coins += 10;
                                StatePage.State = StatesWindow.LevelField;
                            }

                            //if ((EnemiesGrid).Children.Count == 0)
                            //    StatePage.State = StatesWindow.LevelField;
                        }

                    }
                    EnemiesGrid.IsEnabled = false;
                    EnemiesDealDamage();
                }));
            }
        }

        private async void EnemiesDealDamage()
        {
            foreach (var enemy in EnemyObsColl)
            {
                if (enemy.CurrentHP > 0 && Player.CurrentHP > 0)
                {
                    switch (enemy.Status)
                    {
                        case "Common":
                            Message += $"Вы получили {enemy.Damage} урона от {(enemy as Enemy).Name}\n";
                            Player.GetDamage(enemy.Damage);
                            Task.Delay(1000);
                            break;
                        case "Stun" :
                            Message += $"{(enemy as Enemy).Name} пропускает ход\n";
                            enemy.Status = "Common";
                            Task.Delay(1000);
                            break;
                        case "Freeze":
                            Message += $"{(enemy as Enemy).Name} заморожен \n";
                            Task.Delay(1000);
                            break;
                    }

                }
            }

            if (SecondPanel != null)
            {
                SecondPanel.IsEnabled = true;
                (SecondPanel.Children.OfType<Button>().First() as Button).Focus();
            }
        }

        public BattleModel(Player PlayerClass)
        {
            EnemyObsColl = new ObservableCollection<Enemy>();
            _random = new Random();
            Message = "Бой\n";
            var arrayEnemies = JsonConvert.DeserializeObject<ObservableCollection<Enemy>>(
                    File.ReadAllText(FilePath.Combine(Settings.Directory, "Enemy.json")));
            if (FieldProvider.IsBossBattle)
            {
                arrayEnemies = JsonConvert.DeserializeObject<ObservableCollection<Enemy>>(
                    File.ReadAllText(FilePath.Combine(Settings.Directory, "Boss.json")));
                EnemyObsColl.Add(arrayEnemies[0]);
            }
            else
            {
                for (var i = 0; i < _random.Next(1, 4); i++)
                {
                    if (arrayEnemies == null) break;
                    var numEnemy = _random.Next(0, arrayEnemies.Count);
                    EnemyObsColl.Add(arrayEnemies[numEnemy]);
                    arrayEnemies.RemoveAt(numEnemy);
                }
            }

            foreach (var item in EnemyObsColl)
                item.Status = "Common";

            Player = PlayerClass;
        }

        public BattleModel()
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}