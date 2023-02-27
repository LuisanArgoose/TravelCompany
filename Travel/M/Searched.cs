using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Travel.Core;

namespace Travel.M
{
    public class Searched : ViewModelBase
    {
        private BindingList<Clients> C;
        private BindingList<Tickets> T;
        private BindingList<HotelRooms> H;
        private DateTime startTime;
        private DateTime endTime;
        private string selectedStartCity;
        private string selectedEndCity;
        public Searched(DateTime startTime,
            DateTime endTime,
            string selectedStartCity,
            string selectedEndCity)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.selectedStartCity = selectedStartCity;
            this.selectedEndCity = selectedEndCity;

            Initialize();
        }

        private void Initialize()
        {
            C = SC.DB.Clients.Local.ToBindingList();
            T = SC.DB.Tickets.Local.ToBindingList();
            H = SC.DB.HotelRooms.Local.ToBindingList();
            OpenTickets = new BindingList<AirsDate>();
            SelectedTickets = new BindingList<AirsDate>();
            OpenHotelRooms = new BindingList<HotelRoomsDate>();
            SelectedHotelRooms = new BindingList<HotelRoomsDate>();
            UploadAirs();
            UploadHotels();
        }
        private BindingList<string> airs;
        public BindingList<string> Airs
        {
            get => airs;
            set => SetProperty(ref airs, value);
        }
        private void UploadAirs()
        {
            Airs = new BindingList<string>();
            var querry1 = from Ts in T
                          where Ts.StartCity == this.selectedStartCity &
                          Ts.EndCity == this.selectedEndCity &
                          Ts.Departure.Date == startTime.Date &
                         Ts.Statuses.Name == "Open"
                          select Ts;
            var querry2 = from Ts in T
                          where Ts.EndCity == this.selectedEndCity &
                          Ts.StartCity == this.selectedStartCity &
                          Ts.Departure.Date == startTime.Date &
                         Ts.Statuses.Name == "Open"
                          select Ts;
            foreach(var q in querry1)
            {
                if (!Airs.Contains(q.Companies.Name)) 
                    Airs.Add(q.Companies.Name);
            }
            foreach (var q in querry2)
            {
                if (!Airs.Contains(q.Companies.Name))
                    Airs.Add(q.Companies.Name);
            }
        }
        private string selectedAir;
        public string SelectedAir 
        { 
            get => selectedAir; 
            set
            {
                SetProperty(ref selectedAir, value);
                UploadTickets();
            }

        }
        private void UploadTickets()
        {
            OpenTickets = new BindingList<AirsDate>();
            SelectedTickets = new BindingList<AirsDate>();
            var querry1 = from Ts in T
                         where Ts.StartCity == this.selectedStartCity &
                         Ts.EndCity == this.selectedEndCity &
                         Ts.Departure.Date == startTime.Date & 
                         Ts.Companies.Name == SelectedAir &
                         Ts.Statuses.Name == "Open"
                          select Ts;

            var querry2 = from Ts in T
                         where Ts.StartCity == this.selectedEndCity &
                         Ts.EndCity == this.selectedStartCity &
                         Ts.Departure.Date == endTime.Date &
                         Ts.Companies.Name == SelectedAir &
                         Ts.Statuses.Name == "Open"
                          select Ts;
            List<Tickets> Departure = querry1.ToList();
            List<Tickets> Landing = querry2.ToList();
            for (int i = 0; i < Math.Min(Departure.Count, Landing.Count); i++)
            {
                OpenTickets.Add(
                    new AirsDate()
                    {
                        Departure = Departure[i],
                        Landing = Landing[i]
                    });
            }
        }
        private BindingList<AirsDate> openTickets;
        public BindingList<AirsDate> OpenTickets
        {
            get => openTickets;
            set => SetProperty(ref openTickets, value);
        }
        private BindingList<AirsDate> selectedTickets;
        public BindingList<AirsDate> SelectedTickets 
        { 
            get => selectedTickets; 
            set
            {
                SetProperty(ref selectedTickets, value);

            }
        }
        public ICommand AddTicketCom { get => new ActionCommand(AddTicket); }
        private void AddTicket()
        {
            if (SelectedTicket == null)
                return;
            if (!SelectedTickets.Contains(SelectedTicket))
                SelectedTickets.Add(SelectedTicket);
            if (OpenTickets.Contains(SelectedTicket))
                OpenTickets.Remove(SelectedTicket);
            
        }
        public ICommand DeleteTicketCom { get => new ActionCommand(DeleteTicket); }
        private void DeleteTicket()
        {
            
            if (SelectedTicket == null)
                return;
            if (!OpenTickets.Contains(SelectedTicket))
                OpenTickets.Add(SelectedTicket);
            if (SelectedTickets.Contains(SelectedTicket))
                SelectedTickets.Remove(SelectedTicket);
        }
        public AirsDate SelectedTicket { get; set; }

        private BindingList<string> hotels;
        public BindingList<string> Hotels {
            get => hotels;
            set => SetProperty(ref hotels, value);
        }
        private void UploadHotels()
        {
            Hotels = new BindingList<string>();
            var querry1 = from Hs in H
                          where Hs.Companies.City == this.selectedEndCity &
                          Hs.Day >= startTime  & Hs.Day <= endTime &
                          Hs.Statuses.Name == "Open"
                          select Hs;
            foreach (var q in querry1)
            {
                if (!Hotels.Contains(q.Companies.Name))
                    Hotels.Add(q.Companies.Name);
            }
        }
        private string selectedHotel;
        public string SelectedHotel
        {
            get => selectedHotel;
            set
            {
                SetProperty(ref selectedHotel, value);
                UploadHotelRooms();
            }
        }
        private void UploadHotelRooms()
        {
            OpenHotelRooms = new BindingList<HotelRoomsDate>();
            SelectedHotelRooms = new BindingList<HotelRoomsDate>();
            var querry1 = from Hs in H
                          where Hs.Companies.City == this.selectedEndCity &
                          Hs.Day >= startTime & Hs.Day <= endTime &
                          Hs.Companies.Name == SelectedHotel &
                          Hs.Statuses.Name == "Open"
                          select Hs;
            var querry2 = from Hs in querry1
                          select Hs.Type;

            List<string> Types = new List<string>();
            foreach(var t in querry2)
            {
                if (!Types.Contains(t))
                    Types.Add(t);
            }

            foreach (var t in Types)
            {
                OpenHotelRooms.Add(new HotelRoomsDate()
                {
                    HotelRooms = new BindingList<HotelRooms>(querry1.Where(x => x.Type == t).ToList())
                });
            }
            
        }
        private BindingList<HotelRoomsDate> openHotelRooms;
        public BindingList<HotelRoomsDate> OpenHotelRooms
        {
            get => openHotelRooms;
            set => SetProperty(ref openHotelRooms, value);
        }
        private BindingList<HotelRoomsDate> selectedHotelRooms;
        public BindingList<HotelRoomsDate> SelectedHotelRooms
        {
            get => selectedHotelRooms;
            set => SetProperty(ref selectedHotelRooms, value);
        }
        public ICommand AddHotelRoomCom { get => new ActionCommand(AddHotelRoom); }
        private void AddHotelRoom()
        {
            if (SelectedHotelRoom == null)
                return;
            if (!SelectedHotelRooms.Contains(SelectedHotelRoom))
                SelectedHotelRooms.Add(SelectedHotelRoom);
            if (OpenHotelRooms.Contains(SelectedHotelRoom))
                OpenHotelRooms.Remove(SelectedHotelRoom);

        }
        public ICommand DeleteHotelRoomCom { get => new ActionCommand(DeleteHotelRoom); }
        private void DeleteHotelRoom()
        {

            if (SelectedHotelRoom == null)
                return;
            if (!OpenHotelRooms.Contains(SelectedHotelRoom))
                OpenHotelRooms.Add(SelectedHotelRoom);
            if (SelectedHotelRooms.Contains(SelectedHotelRoom))
                SelectedHotelRooms.Remove(SelectedHotelRoom);
        }
        public HotelRoomsDate SelectedHotelRoom { get; set; }

        public decimal GetCost()
        {
            decimal cost = 0;
            foreach(var t in SelectedTickets)
            {
                cost += t.GetCost();
            }
            foreach (var t in SelectedHotelRooms)
            {
                cost += t.GetCost();
            }
            return (cost + 5000)*1.75m;
        }

        public bool IsCorrect(BindingList<Clients> clients)
        {
            int count = 0;
            foreach (var t in SelectedHotelRooms)
            {
                count += t.ClientCount();
            }

            bool result =
                count >= clients.Count &
                SelectedTickets.Count == clients.Count &
                SelectedHotelRooms.Count > 0 &
                SelectedTickets.Count > 0;
            return result;
        }
    }
}
