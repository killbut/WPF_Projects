using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace GameRoyak.Models
{
    [DataContract]
    public class Enemy : INotifyPropertyChanged
    {
        private int _id;
        private int _hp;
        private int _currentHP;
        private string _name;
        private int _damage;
        private bool _stun;
        private bool _freeze;
        private int _cooldown;
        private string _status;
        public int Cooldown
        {
            get => _cooldown;
            set
            {
                if (_cooldown < 0)
                    _cooldown = 0;
                else
                {
                    _cooldown = value;
                }
            }
        }

        public string Status
        {
            get => _status;
            set
            {

                switch (value)
                {
                    case "Stun":
                        _status = value;
                        _stun = true;
                        Cooldown = 1;
                        _freeze = false;
                        break;
                    case "Common":
                        _status = value;
                        _stun = false;
                        _freeze = false;
                        break;
                    case "Freeze":
                        _status = value;
                        Cooldown = 2;
                        _stun = false;
                        _freeze = true;
                        break;
                }
            }
        }  
        [DataMember]
        public int ID
        {
            get => _id; set
            {
                _id = value;
            }
        }
        [DataMember]
        public int HP
        {
            get => _hp; set
            {
                _hp = value;
            }
        }

        [DataMember]
        public int CurrentHP
        {
            get => _currentHP;
            set
            {
                if (value > 0)
                {
                    _currentHP = value;
                    OnPropertyChanged();
                }
                else if (value <= 0)
                {
                    _currentHP = 0;
                    OnPropertyChanged();
                }
            }
        }
        [DataMember]
        public string Name
        {
            get => _name; set => _name = value;
        }

        [DataMember]
        public int Damage
        {
            get => _damage; set
            {
                _damage = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public string Image
        {
            get; set;
        }

        public void GetDamage(int Damage)
        {
            CurrentHP -= Damage;
        }
        public int DealDamage(int HP)
        {
            return CurrentHP -= Damage;
        }
        public bool IDead()
        {
            if (CurrentHP <= 0)
                return true;
            else
                return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
