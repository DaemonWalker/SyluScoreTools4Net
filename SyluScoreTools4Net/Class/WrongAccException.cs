using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyluScoreTools4Net.Class
{
    class WrongAccException : BusinessException
    {
        public WrongAccException() { }
        public WrongAccException(string message) : base(message) { }
    }
}
