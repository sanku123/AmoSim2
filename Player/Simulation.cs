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




        private double _playerInicjatywaBase;
        [JsonIgnore]
        public double PlayerInicjatywaBase
        {
            get
            {
                _playerInicjatywaBase = Math.Max(1, Math.Min(5, Math.Round(PlayerViewModel.Player.BattleSpeed / PlayerViewModel.Enemy.BattleSpeed, 3)));
                return _playerInicjatywaBase;
            }
            set
            {
                if (_playerInicjatywaBase != value)
                {
                    _playerInicjatywaBase = value;
                    OnPropertyChanged(nameof(PlayerInicjatywaBase));
                }
            }
        }

        private double _enemyInicjatywaBase;
        [JsonIgnore]
        public double EnemyInicjatywaBase
        {
            get
            {
                _enemyInicjatywaBase = Math.Max(1, Math.Min(5, Math.Round(PlayerViewModel.Enemy.BattleSpeed / PlayerViewModel.Player.BattleSpeed, 3))); 
                return _enemyInicjatywaBase;
            }
            set
            {
                if (_enemyInicjatywaBase != value)
                {
                    _enemyInicjatywaBase = value;
                    OnPropertyChanged(nameof(EnemyInicjatywaBase));
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
    }
}