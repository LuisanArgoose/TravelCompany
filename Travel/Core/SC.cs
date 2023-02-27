using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.V;

namespace Travel.Core
{
    static public class SC
    {
        static public Users ActualUser { get; set; }
        static public NazarovTravelEntities3 DB { get; private set; } 
        static SC()
        {
            DB = new NazarovTravelEntities3();
        }
        
        //static public User ActualUser { get; set;}
        static private LoginWindow loginWindow;
        static public LoginWindow LoginWindow 
        {
            get {
                if(loginWindow == null)
                    loginWindow = new LoginWindow();
                return loginWindow;
            }
            set
            {
                if (loginWindow == null)
                    loginWindow = value;
                return;
            }
        }
        static private TWindow tWindow;
        static public TWindow TWindow
        {
            get
            {
                if (tWindow == null)
                    tWindow = new TWindow();
                return tWindow;
            }
        }
        static private TrWindow trWindow;
        static public TrWindow TrWindow
        {
            get
            {
                if (trWindow == null)
                    trWindow = new TrWindow();
                return trWindow;
            }
        }
        static private AWindow aWindow;
        static public AWindow AWindow
        {
            get
            {
                if (aWindow == null)
                    aWindow = new AWindow();
                return aWindow;
            }
        }
        static private HWindow hWindow;
        static public HWindow HWindow
        {
            get
            {
                if (hWindow == null)
                    hWindow = new HWindow();
                return hWindow;
            }
        }
        static private TAddRequestWindow tAddRequestWindow;
        static public TAddRequestWindow TAddRequestWindow
        {
            get
            {
                if (tAddRequestWindow == null)
                    tAddRequestWindow = new TAddRequestWindow();
                return tAddRequestWindow;
            }
        }
        static public void TAddRequestWindowReset()
        {
            tAddRequestWindow = new TAddRequestWindow();
        }

    }
}
