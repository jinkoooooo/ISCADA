using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace I_SCADA_SERVER
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private App()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("JingooLogger.xml"));
        }
    }
}
