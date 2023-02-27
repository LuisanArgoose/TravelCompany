using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.M
{
    public class HotelRoomsDate
    {
        public HotelRoomsDate() { }
        private DateTime MinDate()
        {
            var querry = from Hs in HotelRooms
                         select Hs.Day;
            return querry.Min();
        }
        private DateTime MaxDate()
        {
            var querry = from Hs in HotelRooms
                         select Hs.Day;
            return querry.Max();
        }
        public string Name { get => MinDate().ToString("dd") + " - " + MaxDate().ToString("dd.MM.yy") + " " + HotelRooms[0].MaxClient + " чел. " + HotelRooms[0].Type; }
        public BindingList<HotelRooms> HotelRooms { get; set; }
        public decimal GetCost()
        {
            decimal cost = 0;
            foreach(var h in HotelRooms)
            {
                cost += h.Cost;
            }
            return cost;
        }
        public int ClientCount()
        {
            if(HotelRooms != null)
            {
                return HotelRooms[0].MaxClient;
            }
            else { return 0; }
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
