using System.Windows;
using Travel.Core;

namespace Travel.V
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window 
    {
        public LoginWindow()
        {
            InitializeComponent();
            SC.LoginWindow = this; 
        }
    }
}
