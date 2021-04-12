using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I_SCADA_SERVER.domain
{
    class MD_USER
    {
        public string USER_ID { get; set; }
        public string USER_PW { get; set; }
        public string USER_NAME { get; set; }
        public int USER_STATE { get; set; }
        public string USER_DEPARTMENT { get; set; }
        public int USER_JOB { get; set; }
    }
}
