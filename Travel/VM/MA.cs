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
    public class MA : ViewModelBase, INotifyPropertyChanged
    {
        BindingList<Tickets> T;
        BindingList<Requests> R;
        BindingList<Clients> C;

        public MA()
        {
            

            T = SC.DB.Tickets.Local.ToBindingList();
            R = SC.DB.Requests.Local.ToBindingList();
            C = SC.DB.Clients.Local.ToBindingList();
            LoadsAll();

            SelectedIndex = 0;

        }
        private Tickets awaitingSelectedTicket;
        public Tickets AwaitingSelectedTicket
        {
            get => awaitingSelectedTicket;
            set
            {
                SetProperty(ref awaitingSelectedTicket, value);
                IsEnableAcceptButton = AwaitingSelectedTicket != null;
            }
        }
        private Tickets openSelectedTicket;
        public Tickets OpenSelectedTicket
        {
            get => openSelectedTicket;
            set
            {
                SetProperty(ref openSelectedTicket, value);
                IsEnableDeleteButton = OpenSelectedTicket != null;
            }
        }
        private string saveInfo;
        public string SaveInfo { get => saveInfo; set => SetProperty(ref saveInfo, value); }

        //public int AwaitingSelectedIndex {  }
        private int selectedIndex;
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                SetProperty(ref selectedIndex, value);
                
                SaveInfo = "";
            }
        }

        private void LoadsAll()
        {
            SC.DB.Tickets.Load();
            SC.DB.Requests.Load();
            SC.DB.Statuses.Load();
            SC.DB.Clients.Load();
            OpenTicketsQuerry();
            AwaitingTicketsQuerry();
            AcceptedTicketsQuerry();
        }
        


        #region DataGridVisual
        private BindingList<Tickets> openTickets;
        public BindingList<Tickets> OpenTickets
        {
            get => openTickets;
            set => SetProperty(ref openTickets, value);
        }
        private void OpenTicketsQuerry()
        {
            var querry = from Ts in T
                         where Ts.Statuses.Name == "Open" &
                         Ts.Companies == SC.ActualUser.Companies
                         select Ts;

            OpenTickets = new BindingList<Tickets>(querry.ToList());
        }

        private BindingList<Tickets> awaitingTickets;
        public BindingList<Tickets> AwaitingTickets
        {
            get => awaitingTickets;
            set => SetProperty(ref awaitingTickets, value);
        }
        private void AwaitingTicketsQuerry()
        {
            var querry = from Ts in T
                         where Ts.Statuses.Name == "Awaiting" &
                         Ts.Companies == SC.ActualUser.Companies
                         select Ts;

            AwaitingTickets = new BindingList<Tickets>(querry.ToList());

        }
        private BindingList<Tickets> acceptedTickets;
        public BindingList<Tickets> AcceptedTickets
        {
            get => acceptedTickets;
            set => SetProperty(ref acceptedTickets, value);
        }
        private void AcceptedTicketsQuerry()
        {
            var querry = from Ts in T
                         where Ts.Statuses.Name == "Accepted" &
                         Ts.Companies == SC.ActualUser.Companies
                         select Ts;

            AcceptedTickets = new BindingList<Tickets>(querry.ToList());

        }
        #endregion

        #region ButtonsLogic
        public ICommand AddButtonCom { get => new ActionCommand(AddTicket); }
        private void AddTicket()
        {
            SC.DB.Tickets.Add(new Tickets()
            {

                Companies = SC.ActualUser.Companies,
                Statuses = SC.DB.Statuses.Single(s => s.Name == "Open"),
                Departure = DateTime.Today,
                Landing = DateTime.Today
            });
            OpenTicketsQuerry();
        }
        public ICommand ReloadButtonCom { get => new ActionCommand(Reload); }
        private void Reload()
        {

            OpenTicketsQuerry();
            AwaitingTicketsQuerry();
            AcceptedTicketsQuerry();
        }
        private bool isEnableDeleteButton;
        public bool IsEnableDeleteButton { get => isEnableDeleteButton; set => SetProperty(ref isEnableDeleteButton, value); }
        public ICommand DeleteButtonCom { get => new ActionCommand(DeleteTicket); }
        private void DeleteTicket()
        {
            SC.DB.Tickets.Remove(OpenSelectedTicket);
            OpenTicketsQuerry();
        }
        public ICommand SaveButtonCom { get => new ActionCommand(SaveTicket); }
        private void SaveTicket()
        {
            try
            {
                SC.DB.SaveChanges();
                SaveInfo = "Успешно сохранено";
            }
            catch
            {
                SaveInfo = "Ошибка сохранения";
            }


        }
        private bool isEnableAcceptButton;
        public bool IsEnableAcceptButton { get => isEnableAcceptButton; set => SetProperty(ref isEnableAcceptButton, value); }
        public ICommand AcceptButtonCom { get => new ActionCommand(AcceptTicket); }
        private void AcceptTicket()
        {
            AwaitingSelectedTicket.Statuses = SC.DB.Statuses.Single(s => s.Name == "Accepted");


            try
            {

                SC.DB.SaveChanges();
                SaveInfo = "Успешно подтверждено";
                AwaitingTicketsQuerry();
                AcceptedTicketsQuerry();
            }
            catch
            {
                SaveInfo = "Ошибка подтверждения";

            }
        }

        public ICommand ExitButtonCom { get => new ActionCommand(Exit); }
        private void Exit()
        {
            SC.AWindow.Close();
        }
        #endregion
    }
}
