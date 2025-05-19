using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmoSim2.Player
{
    public partial class Model
    {
        [JsonIgnore]
        public List<string> Robes => new List<String> 
        {   "", 
            "Krasnoludzka", 
            "Hobbicka" 
        };

        private string _selectedRobe;
        public string SelectedRobe
        {
            get => _selectedRobe;
            set
            {
                if (value != _selectedRobe)
                {
                    _selectedRobe = value;
                    OnPropertyChanged();
                }
            }
        }

        [JsonIgnore]
        public double HobbyteRobe
        {
            get
            {
                switch (SelectedRobe)
                {
                    case "Hobbicka" when Race == "Hobbit" && Class == "Mag":
                        return 3000;
                    case "Hobbicka":
                        return 2000;
                    default:
                        return 0;
                }
            }
        }
        [JsonIgnore]
        public double DwarfRobe
        {
            get
            {
                switch (SelectedRobe)
                {
                    case "Krasnoludzka" when Race == "Krasnolud" && Class == "Mag":
                        return 4000;
                    case "Krasnoludzka":
                        return 2000;
                    default:
                        return 0;
                } 
            }
        }
    }
}