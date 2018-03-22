using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Group8_Homework8
{
    interface ITextOptionsDialog
    {
        event EventHandler OK;
        event EventHandler Apply;
        event EventHandler Cancel;
        event EventHandler SendToBack;
        event EventHandler BringToFront;

        BindingSource BindingSource { get; set; }      

        void setBoundaries(float minX, float minY, float maxX, float maxY);
    }
}
