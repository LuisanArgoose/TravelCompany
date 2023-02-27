using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Core;

namespace Travel.M
{
    public class Countries: ViewModelBase
    {
        private BindingList<Tickets> T;
        private BindingList<HotelRooms> H;
        public Countries(string country)
        {

            Country = country;
            T = SC.DB.Tickets.Local.ToBindingList();
            H = SC.DB.HotelRooms.Local.ToBindingList();
            Cities = new BindingList<string>();
            SelectCities();
            
        }

        
        private void SelectCities()
        {
            var querry1 = from Ts in T
                         where (Ts.StartCountry == Country || Ts.EndCountry == Country )&
                         Ts.Statuses.Name == "Open"
                          select Ts.StartCity;          
            foreach (var c in querry1)
            {
                if (!Cities.Contains(c))
                    Cities.Add(c);
            }

            var querry2 = from Hs in H
                          where Hs.Companies.Country == Country &
                          Hs.Statuses.Name == "Open"
                          select Hs.Companies.City;
            foreach (var c in querry2)
            {
                if(!Cities.Contains(c))
                    Cities.Add(c);
            }
        }
        public string Country { get; set; }
        private BindingList<string> cities;
        public BindingList<string> Cities {
            get => cities;
            set => SetProperty(ref cities, value);
        }
        public override string ToString()
        {
            return Country;
        }
    }
}
