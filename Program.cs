using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace HongshiConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            ServiceMain service = new ServiceMain();
            service.start();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new HSService() //此处是我们的windows服务类名称
            };
            ServiceBase.Run(ServicesToRun);
#endif

        }
    }
}
