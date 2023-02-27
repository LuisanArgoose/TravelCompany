using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public class MH : ViewModelBase, INotifyPropertyChanged
    {
        BindingList<HotelRooms> HR;
        BindingList<Requests> R;
        BindingList<Clients> C;
        BindingList<Request_Client> RC;
        public MH()
        {
            
            HR = SC.DB.HotelRooms.Local.ToBindingList();
            R = SC.DB.Requests.Local.ToBindingList();
            C = SC.DB.Clients.Local.ToBindingList();
            RC = SC.DB.Request_Client.Local.ToBindingList();
            LoadsAll();

            SelectedIndex = 0;

        }
        private HotelRoomsPassport awaitingSelectedItem;
        public HotelRoomsPassport AwaitingSelectedItem
        { 
            get => awaitingSelectedItem;
            set
            {
                SetProperty(ref awaitingSelectedItem, value);
                IsEnableAcceptButton = AwaitingSelectedItem != null;
            }
        }
        private HotelRooms selectedHotelRoom;
        public HotelRooms SelectedHotelRoom
        {
            get => selectedHotelRoom;
            set
            {
                SetProperty(ref selectedHotelRoom, value);
                IsEnableDeleteButton = SelectedHotelRoom != null;
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
            SC.DB.HotelRooms.Load();
            SC.DB.Request_Client.Load();
            SC.DB.Requests.Load();
            SC.DB.Statuses.Load();
            SC.DB.Clients.Load();
            OpenHotelRoomsQuerry();
            AwaitingHotelRoomsQuerry();
            AcceptedHotelRoomsQuerry();
        }
        
        

        #region DataGridVisual
        private BindingList<HotelRooms> openHotelRooms;
        public BindingList<HotelRooms> OpenHotelRooms
        {
            get => openHotelRooms;
            set => SetProperty(ref openHotelRooms, value);
        }
        private void OpenHotelRoomsQuerry()
        {
            var querry = from HRs in HR 
                         where HRs.Statuses.Name == "Open" &
                         HRs.Companies == SC.ActualUser.Companies
                         select HRs;

            OpenHotelRooms = new BindingList<HotelRooms>(querry.ToList());
        }

        private BindingList<HotelRoomsPassport> awaitingHotelRooms;
        public BindingList<HotelRoomsPassport> AwaitingHotelRooms
        {
            get => awaitingHotelRooms;
            set => SetProperty(ref awaitingHotelRooms, value);
        }
        private void AwaitingHotelRoomsQuerry()
        {
            var querry = from HRs in HR
                         where HRs.Statuses.Name == "Awaiting" &
                         HRs.Companies == SC.ActualUser.Companies
                         join Rs in R on HRs.QuerryId equals Rs.Id
                         join RCs in RC on Rs.Id equals RCs.RequestId
                         join Cs in C on RCs.ClientId equals Cs.Id
                         select new HotelRoomsPassport { HotelRooms = HRs, Passport = Cs.Passport };

            AwaitingHotelRooms = new BindingList<HotelRoomsPassport>(querry.ToList());

        }
        private BindingList<HotelRoomsPassport> acceptedHotelRooms;
        public BindingList<HotelRoomsPassport> AcceptedHotelRooms
        {
            get => acceptedHotelRooms;
            set => SetProperty(ref acceptedHotelRooms, value);
        }
        private void AcceptedHotelRoomsQuerry()
        {
            var querry = from HRs in HR
                         where HRs.Statuses.Name == "Accepted" &
                         HRs.Companies == SC.ActualUser.Companies
                         join Rs in R on HRs.QuerryId equals Rs.Id
                         join RCs in RC on Rs.Id equals RCs.RequestId
                         join Cs in C on RCs.ClientId equals Cs.Id
                         select new HotelRoomsPassport { HotelRooms = HRs, Passport = Cs.Passport };

            AcceptedHotelRooms =new BindingList<HotelRoomsPassport>(querry.ToList());

        }
        #endregion

        #region ButtonsLogic
        public ICommand AddButtonCom { get => new ActionCommand(AddRoom); }
        private void AddRoom()
        {
            SC.DB.HotelRooms.Add(new HotelRooms()
            {

                Companies = SC.ActualUser.Companies,
                Statuses = SC.DB.Statuses.Single(s => s.Name == "Open"),
                Day = DateTime.Today
            });
            OpenHotelRoomsQuerry();
        }

        private bool isEnableDeleteButton;
        public bool IsEnableDeleteButton { get => isEnableDeleteButton; set => SetProperty(ref isEnableDeleteButton, value); }
        public ICommand DeleteButtonCom { get => new ActionCommand(DeleteRoom); }
        private void DeleteRoom()
        {
            SC.DB.HotelRooms.Remove(SelectedHotelRoom);
            SaveInfo = "Успешно удалено";
            OpenHotelRoomsQuerry();
        }
        public ICommand ReloadButtonCom { get => new ActionCommand(Reload); }
        private void Reload()
        {

            OpenHotelRoomsQuerry();
            AwaitingHotelRoomsQuerry();
            AcceptedHotelRoomsQuerry();
        }
        public ICommand SaveButtonCom { get => new ActionCommand(SaveRoom); }
        private void SaveRoom()
        {
            try {
                SaveInfo = "Успешно сохранено";
                OpenHotelRoomsQuerry();
                
            }
            catch
            {
                SaveInfo = "Ошибка сохранения";
                
            }
            


        }
        private bool isEnableAcceptButton;
        public bool IsEnableAcceptButton { get => isEnableAcceptButton; set => SetProperty(ref isEnableAcceptButton, value); }
        public ICommand AcceptButtonCom { get => new ActionCommand(AcceptRoom); }
        private void AcceptRoom()
        {
            AwaitingSelectedItem.HotelRooms.Statuses = SC.DB.Statuses.Single(s => s.Name == "Accepted");


            try { 

                SC.DB.SaveChanges();
                SaveInfo = "Успешно подтверждено";
                AwaitingHotelRoomsQuerry();
                AcceptedHotelRoomsQuerry();
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
