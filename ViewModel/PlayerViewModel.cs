using AmoSim2.Others;
using AmoSim2.ViewModel;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace AmoSim2.Player
{
    public class PlayerViewModel : CommandViewModel
    {
        public Model Player { get; set; }
        public Model Enemy { get; set; }

        
        public PlayerViewModel()
        {
            Player = InitializeCharacter("Gracz");
            Enemy = InitializeCharacter("Przeciwnik");

            Enemy.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Enemy.EvasionFull))
                {
                    UpdatePlayerHitChance();
                }
            };

            Player.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Player.EvasionFull))
                {
                    UpdateEnemyHitChance();
                }
            };


            Enemy.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Enemy.BattleSpeed))
                {
                    UpdatePlayerInicjatywa();
                }
            };

            Player.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Player.BattleSpeed))
                {
                    UpdateEnemyInicjatywa();
                }
            };
        }


        private void UpdatePlayerHitChance()
        {
            Player.PlayerHitChance = Player.CalculateHitChance(Player.HitAbility, Enemy.EvasionFull);
        }
        private void UpdateEnemyHitChance()
        {
            Enemy.EnemyHitChance = Player.CalculateHitChance(Enemy.EvasionFull, Player.HitAbility);
        }

        private void UpdatePlayerInicjatywa()
        {
            Player.PlayerInicjatywaBase = Math.Max(1, Math.Min(5, Math.Round(Player.BattleSpeed / Enemy.BattleSpeed, 3)));

        }
        private void UpdateEnemyInicjatywa()
        {
            Enemy.EnemyInicjatywaBase = Math.Max(1, Math.Min(5, Math.Round(Enemy.BattleSpeed / Player.BattleSpeed, 3)));
        }
        private Model InitializeCharacter(string nickname)
        {
            return new Model
            {
                Nickname = nickname,
                Race = "",
                Class = "",
                Level = 1,
                HP = 100,
                BaseStrength = 4,
                BaseAgility = 4,
                BaseInteligence = 4,
                BaseSpeed = 4,
                BaseWillPower = 4,
                BaseToughness = 4,
                CombatSkill = 0,
                EvasionSkill = 0,
                Vampirism = 0,
                WeaponSpeed = 0,
                WeaponDMG = 0,
                Robbery = 0,
                EQdefence = 0,
                EQevasion = 0,
                StaffDefence = 0,
                StaffDamage = 0,
                StaffSpeed = 0,
                SpellDamage = 1,
                SpellDefence = 1,
                SpellSpeed_1_1 = 0,
                SpellSpeed_1_4 = 0,
                StrengthBlessActive = false,
                ToughnessBlessActive = false,
                AgilityBlessActive = false,
                SpeedBlessActive = false,
                InteligenceBlessActive = false,
                WillPowerBlessActive = false,
                HitAbilityBlessActive = false,
                EvasionBlessActive = false,
                MageControlsVisibility = false,
                MeleeControlsVisibility = false
            };
        }

        protected override void Save(object profile)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            string filter = "Json files (*.json)|*.json";
            saveFileDialog1.Filter = filter;

            if (saveFileDialog1.ShowDialog() == true)
            {
                string fileName = saveFileDialog1.FileName;
                Session.SavePlayerData(fileName, (Model)profile);
            }
        }
        protected override void Load(object profile)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            string filter = "Json files (*.json)|*.json";
            openFileDialog1.Filter = filter;

            if (openFileDialog1.ShowDialog() == true)
            {
                string fileName = openFileDialog1.FileName;
                Session.LoadPlayerData(fileName, (Model)profile);
            }
        }
    }
}