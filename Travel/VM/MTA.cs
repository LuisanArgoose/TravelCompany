using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Travel.Core;
using Travel.M;

namespace Travel.VM
{
    public class MTA : ViewModelBase, INotifyPropertyChanged
    {
        private BindingList<Tickets> T;
        private BindingList<HotelRooms> H;
        public MTA()
        {
            Initialize();
        }
        private void Initialize()
        {
            T = SC.DB.Tickets.Local.ToBindingList();
            H = SC.DB.HotelRooms.Local.ToBindingList();
            Clients = new BindingList<Clients>();
            UploadCountries();
            temp();
        }
        private void temp()
        {
            for (int i = 0; i < 12; i++)
            {
                H.Add(
                    new HotelRooms()
                    {

                        Companies = SC.DB.Companies.First(x => x.Id == 3),
                        Statuses = SC.DB.Statuses.Single(s => s.Name == "Open"),
                        MaxClient = 2,
                        Cost = 1500,
                        Type = "Eco",
                        Day = new DateTime(2023, 02, 17 + i)
                    });
            }
            SC.DB.SaveChanges();
        }
        private void temp1()
        {
            for (int i = 0; i < 12; i++)
            {
                T.Add(
                    new Tickets()
                    {
                        Departure = new DateTime(2023, 02, 17 + i),
                        Landing = new DateTime(2023, 02, 17 + i),
                        FlightNumber = 123321,
                        StartCountry = "Russia",
                        StartCity = "Saint-Petersburg",
                        EndCountry = "Russia",
                        EndCity = "Kazan",
                        Companies = SC.DB.Companies.First(x => x.Id == 2),
                        Statuses = SC.DB.Statuses.Single(s => s.Name == "Open"),
                        Cost = 3000,

                    });
                T.Add(
                    new Tickets()
                    {
                        Departure = new DateTime(2023, 02, 17 + i),
                        Landing = new DateTime(2023, 02, 17 + i),
                        FlightNumber = 321123,
                        StartCountry = "Russia",
                        StartCity = "Kazan",
                        EndCountry = "Russia",
                        EndCity = "Saint-Petersburg",
                        Companies = SC.DB.Companies.First(x => x.Id == 2),
                        Statuses = SC.DB.Statuses.Single(s => s.Name == "Open"),
                        Cost = 3000,

                    });
            }
            SC.DB.SaveChanges();
        }

        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now;
        private BindingList<Countries> startCountries;
        public BindingList<Countries> StartCountries {
            get => startCountries; 
            set => SetProperty(ref startCountries, value); 
        }
        private Countries selectedStartCountry;
        public Countries SelectedStartCountry 
        { 
            get => selectedStartCountry; 
            set => SetProperty(ref selectedStartCountry, value); 
        }
        public string SelectedStartCity { get; set; }
        private BindingList<Countries> endCountries;
        public BindingList<Countries> EndCountries 
        {
            get => endCountries;
            set => SetProperty(ref endCountries, value);
        }
        private Countries selectedEndCountry;
        public Countries SelectedEndCountry
        {
            get => selectedEndCountry;
            set => SetProperty(ref selectedEndCountry, value);
        }
        public string SelectedEndCity { get; set; }

        private void UploadCountries()
        {
            List<string> countries = new List<string>();
            var querry1 = from Ts in T
                          where Ts.Statuses.Name == "Open"
                          select Ts.StartCountry;
            foreach (var c in querry1)
            {
                countries.Add(c);
            }
            var querry2 = from Ts in T
                          where Ts.Statuses.Name == "Open"
                          select Ts.EndCountry;
            foreach (var c in querry2)
            {
                countries.Add(c);
            }

            var querry3 = from Hs in H
                          where Hs.Statuses.Name == "Open"
                          select Hs.Companies.Country;
            foreach (var c in querry3)
            {
                countries.Add(c);
            }
            List<string> temp = new List<string>();
            foreach (var c in countries)
            {
                if(!temp.Contains(c))
                    temp.Add(c);
                    
            }

            List<Countries> con = new List<Countries>();
            foreach (var c in temp)
            {
                con.Add(new Countries(c));
            }

            StartCountries = new BindingList<Countries>(con);
            EndCountries = new BindingList<Countries>(con);
        }

        public BindingList<Clients> Clients { get; set; }
        public ICommand SearchCom { get => new ActionCommand(Search); }
        public Searched searched;
        public Searched Searched
        {
            get => searched;
            set => SetProperty(ref searched, value);
        }
        private decimal cost;
        public decimal Cost
        {
            get => cost;
            set => SetProperty(ref cost , value);
        }
        public ICommand GetCostCom { get => new ActionCommand(GetCost); }
        private void GetCost()
        {
            if (Searched != null)
            {
                Cost = Searched.GetCost();
                IsAccept = IsCorrect() & Searched.IsCorrect(Clients) & IsClientsCorrect();
            }   
            else
            {
                Cost = 0;
                IsAccept = false;
            }
        }
        private bool isAccept;
        public bool IsAccept { get=> isAccept; set => SetProperty(ref isAccept,value); }
        public ICommand AcceptCom { get => new ActionCommand(Accept); }
        private void Search()
        {
            if ( !IsCorrect()
                ) { return; }
            Searched = new Searched(StartTime, EndTime, SelectedStartCity, SelectedEndCity);
        }
        private bool IsCorrect()
        {
            return StartTime <= EndTime &
                SelectedStartCity != null &
                SelectedEndCity != null &
                SelectedEndCity != SelectedStartCity &
                Clients.Count >= 1;
        }
        private bool IsClientsCorrect()
        {
            foreach (var c in Clients)
            {
                if (c.Name == "" ||
                    c.Surename == "" ||
                    c.Age == 0 ||
                    c.Passport == "")
                {
                    return false;
                }
                    
            }
            return true;
        }
        private void Accept()
        {
            Requests requests = new Requests();
            SC.DB.Requests.Add(requests);
            // Запрос на транспорт
            var trans = SC.DB.Transport.Local.Where(x => x.Departure == Searched.SelectedTickets[0].Departure.Departure &
                                     x.Landing == Searched.SelectedTickets[0].Landing.Departure);
            if (trans.Count() > 0)
            {
                var exemp = trans.First();
                exemp.Statuses = SC.DB.Statuses.Single(s => s.Name == "Awaiting");
                requests.Transport = exemp;

            }
            else
            {
                var ex = new Transport()
                {
                    Departure = Searched.SelectedTickets[0].Departure.Departure,
                    Landing = Searched.SelectedTickets[0].Landing.Departure,
                    Statuses = SC.DB.Statuses.Single(s => s.Name == "Awaiting"),
                    Companies = SC.DB.Companies.Single(s => s.Name == "TransRussia"),
                    Cost = 3000
                };
                requests.Transport = ex;
            }
            //SC.DB.SaveChanges();
            // Создание клиентов
            foreach (var c in Clients)
            {
                
                var temp = c;
                var clien = SC.DB.Clients.Local.Where(x => x.Passport == c.Passport);
                if (clien.Count() > 0)
                {
                    temp = clien.First();
                    //
                }
                SC.DB.Request_Client.Add(new Request_Client()
                {
                    Requests = requests,
                    Clients = temp
                });
                

            }

            foreach (var c in Clients)
            {
                
                //SC.DB.Clients.Add(c);
            }
            int k = 0;
            // Изменение статуса билетов
            foreach (var t in Searched.SelectedTickets)
            {
                
                tick(t.Departure, k);
                tick(t.Landing, k);
                k++;
                
            }
            void tick(Tickets a, int res)
            {
                a.Clients = Clients[res];
                a.Statuses = SC.DB.Statuses.Single(s => s.Name == "Awaiting");
                a.Requests = requests;
            }

            // Изменение статусов номеров
            foreach (var t in Searched.SelectedHotelRooms)
            {
                foreach(var h in t.HotelRooms)
                {
                    h.Statuses = SC.DB.Statuses.Single(s => s.Name == "Awaiting");
                    requests.HotelRooms.Add(h);
                }
            }
            SC.DB.SaveChanges();
            SC.TAddRequestWindow.Close();
        }
    }
}
