using System;
using GameRoyak.Enums;

namespace GameRoyak.Logic
{
    public static class StatePage
    {
        public static bool IsStartGame = true;
        public static event EventHandler OnChangedNumber;
        private static StatesWindow _state;
        public static StatesWindow State
        {
            get => _state;
            set
            {
                if(value != _state)
                {
                    _state = value;
                    if (OnChangedNumber != null) 
                        OnChangedNumber(null, EventArgs.Empty);
                }
            }
        }
    }
}