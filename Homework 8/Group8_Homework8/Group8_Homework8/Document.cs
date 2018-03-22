using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Group8_Homework8
{
    [Serializable]
    public class Document : ISerializable
    {
        private string fileName;
        private BindingSource bindingSource;
        private BindingList<Text> textBindingList;
        private Color lastColor;
        private Font lastFont;

        public string FileName { get { return fileName; } set { fileName = value; } }
        public BindingSource BindingSource { get { return bindingSource; } set { bindingSource = value; } }
        public BindingList<Text> TextBindingList { get { return textBindingList; } set { textBindingList = value; } }
        public Text LastAddedText { get; set; }

        /* Read-only */
        public Color LastColor { get { return lastColor = LastAddedText.Color; } }
        public Font LastFont { get { return lastFont = new Font(LastAddedText.FontName, LastAddedText.FontSize); } }

        public Document()
        {
            FileName = null;
            BindingSource = new BindingSource();
            BindingSource.AddingNew += new AddingNewEventHandler(BindingSource_AddingNew);
            TextBindingList = new BindingList<Text>();
            BindingSource.DataSource = TextBindingList;
            LastAddedText = new Text();
        }

        public Document(SerializationInfo info, StreamingContext context) 
        {
            fileName = (string)info.GetValue("fileName", typeof(string));
            textBindingList = (BindingList<Text>)info.GetValue("textBindingList", typeof(BindingList<Text>));
            lastColor = (Color)info.GetValue("lastColor", typeof(Color));
            lastFont = (Font)info.GetValue("lastFont", typeof(Font));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("fileName", fileName, typeof(string));
            info.AddValue("textBindingList", TextBindingList, typeof(BindingList<Text>));
            info.AddValue("lastColor", lastColor, typeof(Color));
            info.AddValue("lastFont", lastFont, typeof(Font));
        }

        private void BindingSource_AddingNew(object sender, AddingNewEventArgs e)
        {
            LastAddedText = e.NewObject as Text;
        }

        public void Save(string filename) 
        {
            this.FileName = filename;

            using(Stream stream = new FileStream (filename, FileMode.Create, FileAccess.Write))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
            }
        }

        public static Document Open(string filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                IFormatter formatter = new BinaryFormatter();
                Document doc = (Document)formatter.Deserialize(stream);
                doc.BindingSource = new BindingSource();
                doc.BindingSource.DataSource = doc.TextBindingList;

                return doc;
            }
        }

    }
}
