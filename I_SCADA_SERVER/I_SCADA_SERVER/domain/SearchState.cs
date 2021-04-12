using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I_SCADA_SERVER.domain
{
    class SearchState
    {
        public string PROF_ID { get; set; }
        public string PROF_NAME { get; set; }
        public string STATE { get; set; }
    }

    class AllState
    {
        public string USER_ID { get; set; }
        public string NOW_STATE { get; set; }
        public string CHANGE_TIME { get; set; }
        public string PRO_ADDRESS { get; set; }
    }
}
