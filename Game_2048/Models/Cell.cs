using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_2048.Models
{
    class Cell
    {
        public int Value { get; set; }
        public static int Score { get; set; }
        public bool WasAdded { get; set; }

        public static bool operator ==(Cell c1, Cell c2)
        {
            return c1.Value == c2.Value;
        }
        public static bool operator !=(Cell c1, Cell c2)
        {
            return !(c1 == c2);
        }

        public static Cell operator ++(Cell cell)
        {
            Score += cell.Value * 2;
            return new Cell
            {
                Value = cell.Value * 2
            };
        }
    }
}
