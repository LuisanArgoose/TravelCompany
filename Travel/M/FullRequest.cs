using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.M
{
    public class FullRequest
    {
        public int Request { get; set; }
        public List<Tickets> Tickets { get; set; }
        public List<HotelRooms> HotelRooms { get; set; }
        public List<Clients> Clients { get; set; }
        public string AllCost { get; set; }
    }
}
