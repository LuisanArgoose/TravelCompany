using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Travel.Core
{
    public class SingUpCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<object> p;

        public SingUpCommand(Action<object> p)
        {
            this.p = p;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            p(parameter);
        }
    }
}
