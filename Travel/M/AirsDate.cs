using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.M
{
    public class AirsDate
    {
        public AirsDate() { }
        public string Name { get => Departure.StartCity + " <-> " + Departure.EndCity + " " + Departure.Departure.ToString("dd.MM.yy"); }
        public Tickets Departure { get; set; }
        public Tickets Landing { get; set; }
        public decimal GetCost()
        {
            return Departure.Cost + Landing.Cost;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
