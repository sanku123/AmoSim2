using AmoSim2.Others;
using AmoSim2.Player;
using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using RelayCommand = AmoSim2.Others.RelayCommand;

namespace AmoSim2.ViewModel
{
    public class SimulationViewModel : CommandViewModel
    {
        public PlayerViewModel PlayerViewModel => ServiceLocator.Current.GetInstance<PlayerViewModel>();

        private readonly Random rnd = new Random();
        private DateTime startTime;

        private double _progressValue;
        public double ProgressValue
        {
            get { return _progressValue; }
            set { _progressValue = value; OnPropertyChanged(); }
        }

        private bool _trackCriticalHits;
        public bool TrackCriticalHits
        {
            get => _trackCriticalHits;
            set
            {
                _trackCriticalHits = value;
                OnPropertyChanged(nameof(TrackCriticalHits));
            }
        }

        private ObservableCollection<string> _singleBattleLog = new ObservableCollection<string>();

        public ObservableCollection<string> SingleBattleLog
        {
            get => _singleBattleLog;
            set
            {
                _singleBattleLog = value;
                OnPropertyChanged(nameof(SingleBattleLog));
            }
        }

        private string _battleLogText;
        public string BattleLogText
        {
            get => _battleLogText;
            set
            {
                _battleLogText = value;
                OnPropertyChanged(nameof(BattleLogText));
            }
        }



        private double _timeTakenInSeconds;
        public double TimeTakenInSeconds
        {
            get { return _timeTakenInSeconds; }
            set
            {
                if (_timeTakenInSeconds != value)
                {
                    _timeTakenInSeconds = value;
                    OnPropertyChanged(nameof(TimeTakenInSeconds));
                    UpdateProgressBar(); 
                }
            }
        }
        private void UpdateProgressBar()
        {
            double totalSimulationTime = 100; 
            double progressPercentage = Math.Min((TimeTakenInSeconds / totalSimulationTime) * 100, 100);
            ProgressValue = progressPercentage;
        }
        private void ReportProgress(double value)
        {
            ProgressValue = value;
        }
        private int _averageRounds;
        public int AverageRounds
        {
            get { return _averageRounds; }
            set { _averageRounds = value; OnPropertyChanged(); }
        }

        private int _winCount;
        public int WinCount
        {
            get { return _winCount; }
            set { _winCount = value; OnPropertyChanged(); }
        }

        private int _lostCount;
        public int LostCount
        {
            get { return _lostCount; }
            set { _lostCount = value; OnPropertyChanged(); }
        }

        private int _drawCount;
        public int DrawCount
        {
            get { return _drawCount; }
            set { _drawCount = value; OnPropertyChanged(); }
        }

        private string _winPercentageText;
        public string WinPercentageText
        {
            get { return _winPercentageText; }
            set { _winPercentageText = value; OnPropertyChanged(); }
        }

        private string _lostPercentageText;
        public string LostPercentageText
        {
            get { return _lostPercentageText; }
            set { _lostPercentageText = value; OnPropertyChanged(); }
        }

        private string _drawPercentageText;
        public string DrawPercentageText
        {
            get { return _drawPercentageText; }
            set { _drawPercentageText = value; OnPropertyChanged(); }
        }




        private Dictionary<double, double> _criticalHitPercentages;
        public Dictionary<double, double> CriticalHitPercentages
        {
            get => _criticalHitPercentages;
            private set
            {
                _criticalHitPercentages = value;
                OnPropertyChanged(nameof(CriticalHitPercentages));
            }
        }




        public ICommand StartSingleModeCommand { get; }

        public ICommand StartMultiModeCommand { get; }

        public SimulationViewModel()
        {
            StartMultiModeCommand = new RelayCommand(StartMultiMode);
            StartSingleModeCommand = new RelayCommand(StartSingleMode);

            SingleBattleLog.CollectionChanged += (s, e) =>
            {
                BattleLogText = string.Join(Environment.NewLine, SingleBattleLog);
            };

        }

        private void StartSingleMode(object parameter)
        {
            SingleBattleLog.Clear();

            var player = PlayerViewModel.Player;
            var enemy = PlayerViewModel.Enemy;

            int playerHealthPoints = (int)player.HP + (int)player.HP_Bonus;
            int enemyHealthPoints = (int)enemy.HP + (int)enemy.HP_Bonus;

            bool playerGoesFirst = player.BattleSpeed > enemy.BattleSpeed;

            for (int i = 1; i <= 24; i++)
            {
                SingleBattleLog.Add(Environment.NewLine);
                SingleBattleLog.Add($"--- Runda {i} ---");

                if (playerGoesFirst)
                {
                    enemyHealthPoints = PerformPlayerAttack(enemyHealthPoints, player, enemy, true);
                    if (enemyHealthPoints < 1)
                    {
                        SingleBattleLog.Add($"{player.Nickname} zwycięża!");
                        return;
                    }

                    playerHealthPoints = PerformEnemyAttack(playerHealthPoints, enemy, player, true);
                    if (playerHealthPoints < 1)
                    {
                        SingleBattleLog.Add($"{enemy.Nickname} zwycięża!");
                        return;
                    }
                }
                else
                {
                    playerHealthPoints = PerformEnemyAttack(playerHealthPoints, enemy, player, true);
                    if (playerHealthPoints < 1)
                    {
                        SingleBattleLog.Add($"{enemy.Nickname} zwycięża!");
                        return;
                    }

                    enemyHealthPoints = PerformPlayerAttack(enemyHealthPoints, player, enemy, true);
                    if (enemyHealthPoints < 1)
                    {
                        SingleBattleLog.Add($"{player.Nickname} zwycięża!");
                        return;
                    }
                }

                if (i == 24)
                {
                    SingleBattleLog.Add("Walka zakończyła się remisem.");
                    return;
                }
            }
        }


        private void StartMultiMode(object parameter)
        {
            AverageRounds = 0;
            WinCount = 0;
            LostCount = 0;
            DrawCount = 0;

            criticalValueCounts.Clear(); // Reset critical hit data

            int iterations = 10000;

            ProgressValue = 0;
            startTime = DateTime.Now;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender, e) =>
            {

                for (int i = 0; i < iterations; i++)
                {
                    BeginFastSimulation();
                    double progress = ((double)i / iterations) * 100;
                    worker.ReportProgress((int)progress);
                }
                AverageRounds /= iterations;
            };
            worker.ProgressChanged += (sender, e) =>
            {
                ReportProgress(e.ProgressPercentage);
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                TimeSpan duration = DateTime.Now - startTime;

                double seconds = duration.TotalSeconds;

                TimeTakenInSeconds = seconds;

                double winPercentage = (double)WinCount / iterations * 100;
                double lostPercentage = (double)LostCount / iterations * 100;
                double drawPercentage = (double)DrawCount / iterations * 100;

                WinPercentageText = winPercentage.ToString("0.00") + "%";
                LostPercentageText = lostPercentage.ToString("0.00") + "%";
                DrawPercentageText = drawPercentage.ToString("0.00") + "%";

                //Compute critical hit percentages
                CriticalHitPercentages = GetCriticalHitPercentages();
            };
            worker.RunWorkerAsync();
        }

        private void BeginFastSimulation()
        {
            var player = PlayerViewModel.Player;
            var enemy = PlayerViewModel.Enemy;

            int playerHealthPoints = (int)player.HP + (int)player.HP_Bonus;
            int enemyHealthPoints = (int)enemy.HP + (int)enemy.HP_Bonus;

            PlayerViewModel.Player.PlayerHitChance = Math.Max(PlayerViewModel.Player.PlayerHitChance, 2);

            bool playerGoesFirst = player.BattleSpeed > enemy.BattleSpeed;

            for (int i = 1; i <= 24; i++)
            {
                AverageRounds++;
                if (playerGoesFirst)
                {
                    enemyHealthPoints = PerformPlayerAttack(enemyHealthPoints, player, enemy);
                    if (enemyHealthPoints < 1)
                    {
                        WinCount += 1;
                        return;
                    }

                    playerHealthPoints = PerformEnemyAttack(playerHealthPoints, enemy, player);
                    if (playerHealthPoints < 1)
                    {
                        LostCount += 1;
                        return;
                    }
                }
                else
                {
                    playerHealthPoints = PerformEnemyAttack(playerHealthPoints, enemy, player);
                    if (playerHealthPoints < 1)
                    {
                        LostCount += 1;
                        return;
                    }

                    enemyHealthPoints = PerformPlayerAttack(enemyHealthPoints, player, enemy);
                    if (enemyHealthPoints < 1)
                    {
                        WinCount += 1;
                        return;
                    }
                }

                if (i == 24)
                {
                    DrawCount += 1;
                    return;
                }
            }
        }

        private int PerformPlayerAttack(int targetHP, Model player, Model enemy, bool log = false)
        {
            double hitChance = Convert.ToInt32(Math.Max(player.PlayerHitChance, 2));

            int fullAttacks = (int)Math.Floor(player.PlayerInicjatywaBase);
            double chanceForExtraAttack = (int)Math.Round((player.PlayerInicjatywaBase - fullAttacks)*100);

            for (int i = 0; i < fullAttacks && (targetHP > 0); i++)
            {
                if (hitChance < rnd.Next(1, 101))
                {
                    if (log) SingleBattleLog.Add($"{enemy.Nickname} uniknął ataku {player.Nickname}.");
                    continue;
                }

                if (enemy.BlockChance >= rnd.Next(1, 101))
                {
                    if (log) SingleBattleLog.Add($"{enemy.Nickname} zablokował atak {player.Nickname}.");
                    continue;
                }

                double crit = player.Critical();
                int damage = CalculateDamage(player, enemy, crit);

                if (damage > 0 && player.Class == "Czarnoksiężnik" || player.Class == "Mag" && enemy.Race == "Jaszczuroczłek")
                    damage = (int)(damage * 0.95);

                targetHP -= damage;

                if (log)
                {
                    if (crit > 1)
                        SingleBattleLog.Add($"{player.Nickname} w przypływie szału walki atakuje {enemy.Nickname} i zadaje {damage} obrażeń! ({targetHP} zostało).(CRIT x{crit})");
                    else
                        SingleBattleLog.Add($"{player.Nickname} atakuje {enemy.Nickname} i zadaje {damage} obrażeń! ({targetHP} zostało)");
                }
            }

            if (fullAttacks >= 1 && chanceForExtraAttack < rnd.Next(1, 101) && (targetHP > 0) && hitChance < rnd.Next(1, 101) && enemy.BlockChance < rnd.Next(1, 101))
            {
                double crit = player.Critical();
                int damage = CalculateDamage(player, enemy, crit);

                if (damage > 0 && player.Class == "Czarnoksiężnik" || player.Class == "Mag" && enemy.Race == "Jaszczuroczłek")
                    damage = (int)(damage * 0.95);

                targetHP -= damage;

                if (log)
                {
                    if (crit > 1)
                        SingleBattleLog.Add($"{player.Nickname} w przypływie szału walki atakuje {enemy.Nickname} i zadaje {damage} obrażeń! ({targetHP} zostało).(CRIT x{crit})");
                    else
                        SingleBattleLog.Add($"{player.Nickname} atakuje {enemy.Nickname} i zadaje {damage} obrażeń! ({targetHP} zostało)");
                }
            }

            return targetHP;
        }

        private int PerformEnemyAttack(int targetHP, Model enemy, Model player, bool log = false)
        {
            double hitChance = Convert.ToInt32(Math.Max(enemy.EnemyHitChance, 2));

            int fullAttacks = (int)Math.Floor(enemy.EnemyInicjatywaBase);
            double chanceForExtraAttack = (int)Math.Round((enemy.EnemyInicjatywaBase - fullAttacks) * 100);

            for (int i = 0; i < fullAttacks && (targetHP > 0); i++)
            {
                if (hitChance < rnd.Next(1, 101))
                {
                    if (log) SingleBattleLog.Add($"{player.Nickname} uniknął ataku {enemy.Nickname} .");
                    continue;
                }

                if (player.BlockChance >= rnd.Next(1, 101))
                {
                    if (log) SingleBattleLog.Add($"{player.Nickname} zablokował atak {enemy.Nickname}.");
                    continue;
                }

                double crit = enemy.Critical();
                int damage = CalculateDamage(enemy, player, crit);

                if (damage > 0 && enemy.Class == "Czarnoksiężnik" || enemy.Class == "Mag" && player.Race == "Jaszczuroczłek")
                    damage = (int)(damage * 0.95);

                targetHP -= damage;

                if (log)
                {
                    if (crit > 1)
                        SingleBattleLog.Add($"{enemy.Nickname} w przypływie szału walki atakuje {player.Nickname} i zadaje {damage} obrażeń! ({targetHP} zostało).(CRIT x{crit})");
                    else
                        SingleBattleLog.Add($"{enemy.Nickname} atakuje {player.Nickname} i zadaje {damage} obrażeń! ({targetHP} zostało)");
                }
            }

            if (fullAttacks >= 1 && chanceForExtraAttack < rnd.Next(1, 101) && (targetHP > 0) && hitChance < rnd.Next(1, 101) && player.BlockChance < rnd.Next(1, 101))
            {
                double crit = enemy.Critical();
                int damage = CalculateDamage(enemy, player, crit);

                if (damage > 0 && enemy.Class == "Czarnoksiężnik" || enemy.Class == "Mag" && player.Race == "Jaszczuroczłek")
                    damage = (int)(damage * 0.95);

                targetHP -= damage;

                if (log)
                {
                    if (crit > 1)
                        SingleBattleLog.Add($"{enemy.Nickname} w przypływie szału walki atakuje {player.Nickname} i zadaje {damage} obrażeń! ({targetHP} zostało).(CRIT x{crit})");
                    else
                        SingleBattleLog.Add($"{enemy.Nickname} atakuje {player.Nickname} i zadaje {damage} obrażeń! ({targetHP} zostało)");
                }
            }

            return targetHP;
        }

        private Dictionary<double, int> criticalValueCounts = new Dictionary<double, int>();

        private int CalculateDamage(Model attacker, Model defender, double criticalMultiplier)
        {
            int baseDamage = (int)(attacker.Attack + rnd.Next(1, (int)(5 * attacker.Level)));
            double warlockDefenceBreak = 0;
            int bonusDamage = 0;

            if (attacker.Class == "Łowca" && defender.Race != "Jaszczuroczłek")
                bonusDamage = attacker.BonusŁowcy;
            else if (attacker.Class == "Czarnoksiężnik")
                bonusDamage = attacker.WarlockPoisonDamage;

            if (attacker.Class == "Czarnoksiężnik")
                warlockDefenceBreak = defender.Defence - (int)(defender.Defence * (1 - (Math.Floor(attacker.Level/40)/100)));

            if (TrackCriticalHits && criticalMultiplier != 1)
            {
                if (!criticalValueCounts.TryGetValue(criticalMultiplier, out var count))
                    criticalValueCounts[criticalMultiplier] = 1;
                else
                    criticalValueCounts[criticalMultiplier] = count + 1;
            }

            int calculatedDamage = (int)Math.Max(0, baseDamage * criticalMultiplier * attacker.ThiefDamagePenalty - defender.Defence - warlockDefenceBreak);

            return calculatedDamage + bonusDamage;
        }

        public Dictionary<double, double> GetCriticalHitPercentages()
        {
            int totalCriticalHits = criticalValueCounts.Values.Sum();
            return criticalValueCounts
                .OrderBy(kvp => kvp.Key) // Sort by the critical hit multiplier
                .ToDictionary(
                    kvp => kvp.Key,       // Critical multiplier
                    kvp => (double)kvp.Value / totalCriticalHits * 100 // Percentage
                );
        }

    }

}
