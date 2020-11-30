using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_2048.Exceptions
{
    class VictoryException : ApplicationException
    {
        public VictoryException() : base("Вы выиграли!") { }
    }
}
