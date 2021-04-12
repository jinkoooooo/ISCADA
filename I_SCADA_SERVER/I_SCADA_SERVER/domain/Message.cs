using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I_SCADA_SERVER.domain
{
    class Message
    {
        public string TO_ID { get; set; }
        public string FROM_ID { get; set; }
        public string MESSAGE { get; set; }
    }
}
