using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillarsOfPower.classes
{
    public class ActionResponse
    {
        public bool Outcome { get; set; }
        public GameData gameData { get; set; }
        public string Message { get; set; }
    }
}
