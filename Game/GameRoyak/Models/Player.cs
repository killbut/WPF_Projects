using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GameRoyak.Enums;
using GameRoyak.Logic;

namespace GameRoyak.Models
{
    [DataContract]
    public class Player : INotifyPropertyChanged
    {
        private int _hp;
        private int _currentHP;
        private string _name;
        private int _damage;
        private int _coins;
        private int _armor;
        private int _chanceFromEscape;

        public int ChanceFromEscape
        {
            get => _chanceFromEscape;
            set
            {
                _chanceFromEscape = value;
                OnPropertyChanged();
            }
        }
        public string PathToImage { get; set; }
        public Image Image { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
        public string Class { get; set; }
        public BitmapImage Icon { get; set; } = new BitmapImage();
        public int X { get; set; }
        public int Y { get; set; }
        public int MaxArmor { get; set; }
        [DataMember] public int Coins { get => _coins; set { _coins = value; OnPropertyChanged(); } }

        [DataMember]
        public int Armor
        {
            get => _armor;
            set
            {
                if (value <= 0)
                {
                    _armor = 0;
                    OnPropertyChanged();
                }
                else
                {
                    _armor = value;
                    OnPropertyChanged();
                }
                
            }
        }

        [DataMember]
        public int HP
        {
            get => _hp;
            set
            {
                if (_currentHP > _hp)
                {
                    _hp = value;
                    OnPropertyChanged();
                }
                _hp = value; 
                OnPropertyChanged();
            }
        }
        [DataMember] public int CurrentHP 
        {
            get => _currentHP;
            set
            {
                if (value > 0)
                {
                    _currentHP = value;
                    OnPropertyChanged();
                }
                else if (value <= 0 && _currentHP != 0)
                {
                    _currentHP = 0;
                    StatePage.State = StatesWindow.Death;
                }
            }
        }
        [DataMember] public string Name { get => _name; set { _name = Class;OnPropertyChanged(); } }

        [DataMember] public int Damage { get => _damage; set { _damage = value; OnPropertyChanged();} }
        
        public void GetDamage(int Damage)
        {
            if (Armor <= 0)
                CurrentHP -= Damage;
            else if (Armor > 0)
            {
                Armor -=  Damage;
               
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}