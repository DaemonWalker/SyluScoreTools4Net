using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyluScoreTools4Net.Class
{
    class ServerErrorException : BusinessException
    {
        public ServerErrorException() { }
        public ServerErrorException(string message) : base(message) { }
    }
}
