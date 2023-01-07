using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soneta.Business;

namespace PracaDyplomowaNT.Shipx
{
    public class ShipmentParameters : ContextBase
    {
        public ShipmentParameters(Context cx) : base(cx)
        {
        }

        public bool SmsNotification { get; set; }
        public bool EmailNotification { get; set; }
        public bool SaturdayDelivery { get; set; }
    }
}
