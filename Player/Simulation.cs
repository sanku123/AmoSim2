using System;
using AmoSim2.ViewModel;
using CommonServiceLocator;
using Newtonsoft.Json;

namespace AmoSim2.Player
{

    public partial class Model : ViewModelBase
    {
        private double _playerHitChance;
        [JsonIgnore]
        public double PlayerHitChance
        {
            get 
            {
                _playerHitChance = CalculateHitChance(PlayerViewModel.Player.HitAbility, PlayerViewModel.Enemy.EvasionFull);
                return _playerHitChance; 
            }
            set
            {
                if (_playerHitChance != value)
                {
                    _playerHitChance = value;
                    OnPropertyChanged(nameof(PlayerHitChance));
                }
            }
        }

        private double _enemyHitChance;
        [JsonIgnore]
        public double EnemyHitChance
        {
            get
            {
                _enemyHitChance = CalculateHitChance(PlayerViewModel.Enemy.HitAbility, PlayerViewModel.Player.EvasionFull);
                return _enemyHitChance;
            }
            set
            {
                if (_enemyHitChance != value)
                {
                    _enemyHitChance = value;
                    OnPropertyChanged(nameof(EnemyHitChance));
                }
            }
        }

        [JsonIgnore]
        public PlayerViewModel PlayerViewModel => ServiceLocator.Current.GetInstance<PlayerViewModel>();

        public double CalculateHitChance(double attackerAbility, double defenderEvasion)
        {
            double val = Math.Round(((Math.Log10(attackerAbility + 200) - Math.Log10(defenderEvasion + 200)) * 500) + 50, 2);
            return Math.Min(val, 98); 
        }
        [JsonIgnore]
        public double PlayerInicjatywaBase => Math.Max(1, Math.Min(5, Math.Round(PlayerViewModel.Player.BattleSpeed / PlayerViewModel.Enemy.BattleSpeed, 3))); 
        
        [JsonIgnore]
        public double EnemyInicjatywaBase => Math.Max(1, Math.Min(5, Math.Round(PlayerViewModel.Enemy.BattleSpeed / PlayerViewModel.Player.BattleSpeed, 3)));




        private double _playerInicjatywa;
        [JsonIgnore]
        public double PlayerInicjatywa
        {
            get
            {
                _playerInicjatywa = GetInicjatywa(PlayerInicjatywaBase);

                return _playerInicjatywa;
            }
            set
            {
                if (_playerInicjatywa != value)
                {
                    _playerInicjatywa = value;
                    OnPropertyChanged(nameof(PlayerHitChance));
                }
            }
        }

   
        private double _enemyInicjatywa;
        [JsonIgnore]
        public double EnemyInicjatywa
        {
            get
            {
                _enemyInicjatywa = GetInicjatywa(EnemyInicjatywaBase);

                return _enemyInicjatywa;
            }
            set
            {
                if (_enemyInicjatywa != value)
                {
                    _enemyInicjatywa = value;
                    OnPropertyChanged(nameof(PlayerHitChance));
                }
            }
        }

        //[JsonIgnore]
        //public double PlayerInicjatywa => GetInicjatywa(PlayerInicjatywaBase);

        //[JsonIgnore]
        //public double EnemyInicjatywa => GetInicjatywa(EnemyInicjatywaBase);

        public double GetInicjatywa(double baseValue)
        {
            return baseValue >= 4.2 ? 5 :
                   baseValue >= 3.2 ? 4 :
                   baseValue >= 2.2 ? 3 :
                   baseValue >= 1.15 ? 2 : 1;
        } 
    }
}