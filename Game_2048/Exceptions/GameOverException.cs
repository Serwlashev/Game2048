using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_2048.Exceptions
{
    class GameOverException : ApplicationException
    {
        public GameOverException() : base("Вы проиграли!") { }
    }
}
