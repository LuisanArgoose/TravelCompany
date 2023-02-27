using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Travel.Core;
using System.Data.Entity;
using Travel;


namespace Travel.VM
{
    public class MLogin : INotifyPropertyChanged
    {
        public string Login { get; set; } 
        public string WarnMessage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public SingUpCommand SingUpCom { get => new SingUpCommand(SingUp); } 
        
        public MLogin()
        {
            SC.DB.Users.Load();

        }
        private string AutoSingUp()
        {
            Login = "admin";
            return "admin";

        }
        
        private void SingUp(object param)
        {

            string password = (param as PasswordBox).Password;
            //Автовход
            //password = AutoSingUp();


            Users actual;
            try
            {
                actual = SC.DB.Users.Single(u => u.Login == Login && u.Password == password);

            }
            catch
            {
                WarnMessage = "Неверный логин или пароль";
                onPropertyChanged("WarnMessage");
                return;
            }
            SC.ActualUser = actual;
            switch (actual.Modules.Name) {
                case "admin":
                    SC.TWindow.Show();
                    SC.HWindow.Show();
                    SC.AWindow.Show();
                    SC.TrWindow.Show();
                    break;
                case "travel":
                    SC.TWindow.Show();
                    break;
                case "airport":
                    SC.AWindow.Show();
                    break;
                case "hotel":
                    SC.HWindow.Show();
                    break;
                case "transport":
                    SC.TrWindow.Show();
                    break;
            }
            SC.LoginWindow.Close();
                
            
            
        }
    }
}
