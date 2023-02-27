using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Travel.Core
{
    public class ActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        Action _p;
        public ActionCommand(Action p)
        {
            _p = p;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _p();
        }
    }
}
