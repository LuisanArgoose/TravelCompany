using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Input;
using Travel.Core;
using Travel.M;

namespace Travel.VM
{
    public class MT : ViewModelBase
    {

        public MT()
        {
            SC.DB.Requests.Load();
            SC.DB.Clients.Load();
            SC.DB.Tickets.Load();
            SC.DB.Companies.Load();
            SC.DB.Transport.Load();
            SC.DB.Statuses.Load();
            SC.DB.HotelRooms.Load();
            IsCanDelete = false;
            Requests = SC.DB.Requests.Local.ToBindingList();
        }
        public BindingList<Requests> Requests { get; private set; }
        private Requests selectedRequest;
        public Requests SelectedRequest
        {
            get => selectedRequest;
            set
            {
                SetProperty(ref selectedRequest, value);
                OnPropertyChanged(AllCost);
                if(selectedRequest != null)
                    FindAllCost();
                IsCanDelete = selectedRequest != null;
            }
        }
        private void FindAllCost()
        {

            decimal tickets = 0;
            foreach(var t in SelectedRequest.Tickets)
            {
                tickets += t.Cost;
            }
            decimal transport = SelectedRequest.Transport.Cost;
            decimal hotel = 0;
            foreach(HotelRooms h in SelectedRequest.HotelRooms)
            {
                hotel += h.Cost;
            }
            decimal allCost = (tickets + transport + hotel + 3000) * 1.75m;
            AllCost = allCost.ToString();
        }
        private string allCost;
        public string AllCost
        {
            get => allCost;
            set => SetProperty(ref allCost, value);
        }
        public ICommand DeleteButtonCom { get => new ActionCommand(DeleteRequest); }
        private void DeleteRequest()
        {
            foreach(var t in SelectedRequest.Tickets)
            {
                t.Statuses = SC.DB.Statuses.Single(s => s.Name == "Open");
            }
            foreach (var t in SelectedRequest.HotelRooms)
            {
                t.Statuses = SC.DB.Statuses.Single(s => s.Name == "Open");
            }
            SelectedRequest.Transport.Statuses = SC.DB.Statuses.Single(s => s.Name == "Open");
            AllCost = "";

            SC.DB.Request_Client.RemoveRange(SelectedRequest.Request_Client);

            
            SC.DB.Requests.Remove(SelectedRequest);
            SC.DB.SaveChanges();
            //SC.TAddRequestWindowReset();
            //SC.TAddRequestWindow.ShowDialog();

        }
        private bool isCanDelete;
        public bool IsCanDelete
        {
            get => isCanDelete;
            set => SetProperty(ref isCanDelete, value);
        }
        public ICommand OpenAddButtonCom { get => new ActionCommand(OpenAddRequest); }
        private void OpenAddRequest()
        {
            SC.TAddRequestWindowReset();
            SC.TAddRequestWindow.ShowDialog();

        }
        public ICommand ExitButtonCom { get => new ActionCommand(Exit); }
        private void Exit()
        {
            SC.TWindow.Close();

        }



    }
}
