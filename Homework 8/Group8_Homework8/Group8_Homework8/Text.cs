using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group8_Homework8
{
    [Serializable]
    public partial class Text : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string txt;
        private Color color;
        private float x;
        private float y;
        private int rotation;
        private string fontName;
        private float fontSize;

        public string FontName
        {
            get { return fontName; }
            set
            {
                fontName = value;
                OnChange("FontName");
            }
        }

        public float FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                OnChange("FontSize");
            }
        }

        public string Txt
        {
            get { return txt; }
            set
            {
                txt = value;
                OnChange("Txt");
            }
        }

        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                OnChange("Color");
            }
        }

        public float X
        {
            get { return x; }
            set
            {
                x = value;
                OnChange("X");
            }
        }

        public float Y
        {
            get { return y; }
            set
            {
                y = value;
                OnChange("Y");
            }
        }

        public int Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                OnChange("Rotation");
            }
        }

        public Text()
        {
            this.txt = "";
            this.x = 0;
            this.y = 0;
            this.Color = Color.White;
            this.rotation = 0;
            this.fontName = "Consolas";
            this.fontSize = 18;
        }

        public Text(string txt, float x, float y, Font font, Color color, int rotation)
        {
            this.txt = txt;
            this.x = x;
            this.y = y;
            this.color = color;
            this.rotation = rotation;
            this.fontName = font.Name;
            this.fontSize = font.Size;
        }


        protected void OnChange(string propName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public override string ToString()
        {
            int x = (int)X;
            int y = (int)Y;
            return Txt + " \t" + Color.Name + " \t (" + x + ", " + y + ") \t" 
                + Rotation + " \t" + FontName + " \t" + FontSize + "\n";
        }
    

    }
}
