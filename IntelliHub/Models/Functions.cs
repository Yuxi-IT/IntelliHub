using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliHub.Models
{
    public class Functions
    {
        public enum FunctionType
        {
            Web,
            File,
            Shell,
            WebDriver
        }

        public enum WebFunc
        {
            Download,
            Post,
            Get
        }
        public enum ShellFunc
        {
            CmdHidden,
            PSHidden,
            Cmd,
            PS
        }
        public enum WebDriverFunc
        {
            ExecuteJS,
            OpenUrl,
            Bing
        }
        public enum FileFunc
        {
            Write,
            Replace,
            Move,
            Copy,
            Read,
            Del
        }
    }
}
