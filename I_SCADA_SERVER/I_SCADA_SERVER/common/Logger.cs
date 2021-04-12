using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace I_SCADA_SERVER.common
{
    public class Logger {
        readonly static Dictionary<string, ILog> _logs = new Dictionary<string, ILog>() 
        {
            {"All", LogManager.GetLogger("All") }
        };

        public static ILog All { get => _logs["All"]; }
    }
}
