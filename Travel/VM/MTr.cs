using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Travel.Core;

namespace Travel.VM
{
    public class MTr : ViewModelBase, INotifyPropertyChanged
    {
        BindingList<Transport> Tr;
        //BindingList<Requests> R;
        //BindingList<Clients> C;
        public MTr()
        {
            SC.DB.Transport.Load();
            Tr = SC.DB.Transport.Local.ToBindingList();  
            LoadsAll();


        }
        private Transport awaitingSelectedItem;
        public Transport AwaitingSelectedItem
        {
            get => awaitingSelectedItem;
            set
            {
                SetProperty(ref awaitingSelectedItem, value);
                IsEnableAcceptButton = AwaitingSelectedItem != null;
            }
        }

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
            SC.DB.HotelRooms.Load();
            SC.DB.Requests.Load();
            SC.DB.Statuses.Load();
            SC.DB.Clients.Load();
            AwaitingTransportQuerry();
            AcceptedTransportQuerry();
        }

        private string saveInfo;
        public string SaveInfo { get => saveInfo; set => SetProperty(ref saveInfo, value); }


        #region DataGridVisual


        private BindingList<Transport> awaitingTransport;
        public BindingList<Transport> AwaitingTransport
        {
            get => awaitingTransport;
            set => SetProperty(ref awaitingTransport, value);
        }
        private void AwaitingTransportQuerry()
        {
            var querry = from Trs in Tr
                         where Trs.Statuses.Name == "Awaiting" &
                         Trs.Companies == SC.ActualUser.Companies
                         select Trs;

            AwaitingTransport = new BindingList<Transport>(querry.ToList());

        }
        private BindingList<Transport> acceptedTransport;
        public BindingList<Transport> AcceptedTransport
        {
            get => acceptedTransport;
            set => SetProperty(ref acceptedTransport, value);
        }
        private void AcceptedTransportQuerry()
        {
            var querry = from Trs in Tr
                         where Trs.Statuses.Name == "Accepted" &
                         Trs.Companies == SC.ActualUser.Companies
                         select Trs;

            AcceptedTransport = new BindingList<Transport>(querry.ToList());

        }
        #endregion

        #region ButtonsLogic
        
        public ICommand ReloadButtonCom { get => new ActionCommand(Reload); }
        private void Reload()
        {

            AwaitingTransportQuerry();
            AcceptedTransportQuerry();
        }
        
        private bool isEnableAcceptButton;
        public bool IsEnableAcceptButton { get => isEnableAcceptButton; set => SetProperty(ref isEnableAcceptButton, value); }
        public ICommand AcceptButtonCom { get => new ActionCommand(AcceptRoom); }
        private void AcceptRoom()
        {
            AwaitingSelectedItem.Statuses = SC.DB.Statuses.Single(s => s.Name == "Accepted");


            try
            {

                SC.DB.SaveChanges();
                SaveInfo = "Успешно подтверждено";
                AwaitingTransportQuerry();
                AcceptedTransportQuerry();
            }
            catch
            {
                SaveInfo = "Ошибка подтверждения";

            }

        }
        public ICommand ExitButtonCom { get => new ActionCommand(Exit); }
        private void Exit()
        {
            SC.HWindow.Close();
        }
        #endregion
    }
}
