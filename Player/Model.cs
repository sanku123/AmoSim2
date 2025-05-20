using AmoSim2.Others;
using AmoSim2.ViewModel;
using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmoSim2.Player
{
    [AddINotifyPropertyChangedInterface]
    public partial class Model : ViewModelBase
    {

        [JsonIgnore]
        public readonly Random random = new Random();

        public bool MageControlsVisibility { get; set; }

        public bool MeleeControlsVisibility { get; set; }

        public string Nickname { get; set; }

        [JsonIgnore]
        public string Build => Race + " " + Class;

        [JsonIgnore]
        public bool Barbarian => Class == "Barbarzyńca";


        public double Level { get; set; }

        public double HP { get; set; }

        public double HPmax { get; set; }

        public double CombatSkill { get; set; }

        public double EvasionSkill { get; set; }

        public double Vampirism { get; set; }

        public double WeaponDMG { get; set; }

        public double WeaponSpeed { get; set; }

        public double Robbery { get; set; }

        public double EQdefence { get; set; }

        public double EQevasion { get; set; }

        [JsonIgnore]
        public double HitAbility
        {
            get
            {
                double value = CombatSkill + HitAbilityBless + Level + (Vampirism * (Level / 250));

                if (Warrior) return value + (double)Math.Round((0.25 + (Level / 5800)) * Strength, 2);
                else if (Archer) return value + (double)Math.Round((0.25 + (Level / 1950)) * Agility, 2);
                else if (Mage && Race == "Gnom") return value + (double)Math.Round((0.25 + (Level / 1420)) * Inteligence + (Level*10), 2);
                else if (Mage) return value + (double)Math.Round((0.25 + (Level / 1420)) * Inteligence, 2);

                return 0;
            }
        }

        [JsonIgnore]
        public double Attack
        {
            get
            {
                if (Warrior) return (Strength + WeaponDMG + DwarfAttackBonus) * ThiefDamagePenalty * RacialEnemyBonus * HumanAttackBonus;
                else if (Mage) return Inteligence * SpellDamage * GnomeAttackBonus;
                else if (Archer) return (((Agility * 0.8) + BonusAbove200Level(2) + WeaponDMG) * ThiefDamagePenalty) * RacialEnemyBonus * ElfAttackBonus;
                return 0;
            }
        }

        [JsonIgnore]
        public double Defence => Class == "Mag" || Class == "Czarnoksiężnik"
                   ? (WillPower * SpellDefence) + BonusAbove200Level(2) + DwarfRobe + Toughness
                   : Toughness + EQdefence + BarbarianDefence + HumanDefenceFromEQ;



        
        private double _EvasionFull;
        [JsonIgnore]
        public double EvasionFull
        {
            get
            {
                _EvasionFull = CalculateEvasionFull();

                return _EvasionFull;
            }
            set
            {
                if (_EvasionFull != value)
                {
                    _EvasionFull = value;
                    OnPropertyChanged(nameof(PlayerHitChance));
                }
            }
        }

        [JsonIgnore]
        public double MP
        {
            get
            {
                if ((Class == "Mag" || Class == "Czarnoksiężnik") && Race == "Gnom") return (Inteligence + WillPower) * 3.3;
                else if ((Class == "Mag" || Class == "Czarnoksiężnik")) return (Inteligence + WillPower) * 3;
                return 0;
            }
        }
        [JsonIgnore]
        public double HP_Bonus
        {
            get
            {
                if (Race == "Krasnolud") return MP * 0.04;
                return MP * 0.02;
            }
        }
        public double CalculateEvasionFull()
        {
            if (Class == "Mag" || Class == "Czarnoksiężnik")
            {
                if (SelectedFirstSpellSlot == "1.1 x Siła Woli 1.4 x Szybkość")
                {
                    return (0.45 * Speed) + EvasionSkill + EvasionBless + HobbyteRobe + MageEvasion + Level + ((BaseSpeed + StaffSpeed + SpeedBless) * 0.2) - (BonusAbove200Level(2) * 0.2);
                }
                return (0.45 * Speed) + EvasionSkill + EvasionBless + HobbyteRobe + MageEvasion + Level;
            }
            else
            {
                return Math.Round((0.45 * Speed) + (0.45 * WeaponSpeed) + EQevasion + Level + EvasionSkill + EvasionBless + ThiefEvasion, 2);
            }
        }

        
        private double _BattleSpeed;

        [JsonIgnore]
        public double BattleSpeed
        {
            get
            {
                _BattleSpeed = CalculateBattleSpeed();

                return _BattleSpeed;
            }
            set
            {
                if (_BattleSpeed != value)
                {
                    _BattleSpeed = value;
                    OnPropertyChanged(nameof(PlayerHitChance));
                }
            }
        }

        public double CalculateBattleSpeed()
        {
            if (Class == "Mag" || Class == "Czarnoksiężnik")
            {
                return Speed * (1 + SpellSpeed_1_4 + SpellSpeed_1_1) + Level;
            }
            else
            {
                return (BaseSpeed - BonusAbove200Level(2)) * ThiefSpeedBonus + BonusAbove200Level(2) + (SpeedBless * ThiefSpeedBonus) + WeaponSpeed + Level;
            }
        }

        //[JsonIgnore]
        //public double BattleSpeed => Class == "Mag" || Class == "Czarnoksiężnik"
        //            ? Speed * (1 + SpellSpeed_1_4 + SpellSpeed_1_1) + Level
        //            : (BaseSpeed - BonusAbove200Level(2)) * ThiefSpeedBonus + BonusAbove200Level(2) + (SpeedBless * ThiefSpeedBonus) + WeaponSpeed + Level;

        [JsonIgnore]
        public bool Archer => new[] { Strength, Agility, Inteligence }.Max() == Agility;


        [JsonIgnore]
        public bool Mage => new[] { Strength, Agility, Inteligence }.Max() == Inteligence;


        [JsonIgnore]
        public bool Warrior => new[] { Strength, Agility, Inteligence }.Max() == Strength;


        [JsonIgnore]
        public double ThiefEvasion
        {
            get
            {
                switch (Class)
                {
                    case "Złodziej" when Race == "Hobbit":
                        return 1 * Robbery;

                    case "Złodziej" when Race == "Wampir":
                        return 1 * Robbery;

                    case "Złodziej" when Race != "Hobbit" && Race != "Wampir":
                        return 1 * Robbery;

                    default:
                        return 0;
                }
            }
        }

        [JsonIgnore]
        public double MageEvasion
        {
            get
            {
                switch (Class)
                {
                    case "Mag":
                        return 5 * Level;

                    case "Czarnoksiężnik":
                        return 3 * Level;

                    default:
                        return 0;
                }
            }
        }

        [JsonIgnore]
        public double RacialEnemyBonus
        {
            get
            {
                if (!RacialEnemyActive)
                    return 1;

                if (Class == "Łowca")
                    return 1.15;

                return 1;
            }
        }


        [JsonIgnore]
        public double BarbarianDefence
        {
            get
            {
                if (Class == "Barbarzyńca")
                {
                    return Level <= 100 ? 6 * Level :
                           Level <= 200 ? 600 + 8 * (Level - 100) :
                           Level <= 300 ? 1400 + 10 * (Level - 200) :
                           2400 + 12 * (Level - 300);
                }
                return 0;
            }
        }

        [JsonIgnore]
        public double ThiefDamagePenalty => Class == "Złodziej" ? 0.85 : 1;

        [JsonIgnore]
        public double ThiefSpeedBonus => Class == "Złodziej" ? 1.1 : 1;

        [JsonIgnore]
        public double HumanDefenceFromEQ => Race == "Człowiek" ? EQdefence * 0.1 : 0;

        [JsonIgnore]
        public double HumanAttackBonus => Race == "Człowiek" && Warrior ? 1.05 : 1;

        [JsonIgnore]
        public double ElfAttackBonus => Race == "Elf" && Archer ? 1.05 : 1;

        [JsonIgnore]
        public double GnomeAttackBonus => Race == "Gnom" && Mage ? 1.05 : 1;

        [JsonIgnore]
        public double DwarfAttackBonus => Race == "Krasnolud" && Class == "Wojownik" ? WeaponDMG * 0.5 : 0;

        public int BonusAbove200Level(int divisor)
        {
            return (int)Math.Max(0, Math.Floor((Level - 200) / divisor) * 15);
        }

        public double Critical()
        {
            int thief = random.Pick(new Dictionary<int, double>
            {
                { 110, 25 },
                { 115, 25 },
                { 120, 15 },
                { 130, 15 },
                { 140, 6 },
                { 150, 6 },
                { 160, 5 },
                { 9999, 3 }
            });

            int nonThief = random.Pick(new Dictionary<int, double>
            {
                { 110, 25 },
                { 115, 20 },
                { 120, 20 },
                { 130, 15 },
                { 140, 8 },
                { 150, 6 },
                { 160, 5 },
                { 9999, 1 }
            });

            if (CritChance >= random.Next(1, 101))
            {
                if (Class == "Złodziej" && Archer) return ((double)thief + 10) / 100;
                if (Class == "Złodziej") return (double)thief / 100;
                if (Archer) return ((double)nonThief + 10) / 100;
                return (double)nonThief / 100;
            }
            return 1;
        }

        [JsonIgnore]
        public double CritChance
        {
            get
            {
                double value = (double)Math.Min(25, Math.Floor((Math.Sqrt((CombatSkill * 0.0655737704918033) + 1) - 1) / 2 * 2) / 2);
                return Class == "Wojownik" ? value + 5 : value;
            }
        }

        [JsonIgnore]
        public int BonusŁowcy => Class == "Łowca" ? (int)(Level * 2 / 3.0) : 0;

        [JsonIgnore]
        public int WarlockPoisonDamage => Class == "Czarnoksiężnik" ? (int)(Level * 0.75) : 0;

       
    }
}