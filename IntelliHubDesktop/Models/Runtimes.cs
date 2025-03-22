using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliHubDesktop.Models
{
    public class Runtimes
    {
        public static string ApiUrl = "";
        public static string Key = "";
        public static List<AISetting> settings = AISettingModel.LoadSettings();
        public static AISetting CurrAI = null;

        public static List<WorkStream> wStreams = WorkStreamModel.LoadSettings();
    }
}
