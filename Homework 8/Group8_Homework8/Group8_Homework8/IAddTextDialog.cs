using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group8_Homework8
{
    interface IAddTextDialog
    {
        event EventHandler OK;
        event EventHandler Cancel;
        string Text { get; set; }
    }
}
