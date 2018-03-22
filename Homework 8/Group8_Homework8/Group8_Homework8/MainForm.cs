using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using FoundationControlLibrary;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace Group8_Homework8
{

    public partial class MainForm : Form
    {

        public Document document { get; set; }
        private BindingList<Text> savedTextBindingList { get; set; }

        private Form textOptionsDialog;
        private bool isMouseDownOnText;
        private PointF selectedTextMouseDownDiff;
        private Text selectedText;

        //printing
        private bool preview = false;
        private int m_nTotalPage = 0;
        private string stringToPrint = "";
        private bool realPrint = false;
        private string[] pages = new string[1000];
        private int m_nPage = 0;
        private int m_nMaxPage;
        private Font printerFont = null;
        private string textProperties = "Text\tColor\tLocation\tRotation\tFont\tFont Size\n";

        public MainForm()
        {
            InitializeComponent();
            document = new Document();
            document.BindingSource.CurrentChanged += new EventHandler(HighlightSelectedText);
            document.BindingSource.ListChanged += new ListChangedEventHandler(OnDocumentChanged);

            isMouseDownOnText = false;
            selectedTextMouseDownDiff = PointF.Empty;
            selectedText = null;
            printDocument.DefaultPageSettings.Landscape = true;
        }

        #region Misc Events
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (textOptionsDialog != null)
                (textOptionsDialog as ITextOptionsDialog).setBoundaries(0.0F, 0.0F, this.canvasPanel.Width, this.canvasPanel.Height);
        }

        private void removeTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (document.TextBindingList.Count > 0)
            {
                this.document.TextBindingList.Remove(selectedText);
                this.canvasPanel.Invalidate();
            }
        }

        private void OnDocumentChanged(object sender, ListChangedEventArgs e)
        {
            this.canvasPanel.Invalidate();
        }

        #endregion

        #region General Helpers
        private void HighlightSelectedText(object sender, EventArgs e)
        {
            selectedText = document.BindingSource.Current as Text;
            this.canvasPanel.Invalidate();
        }

        private Object DeepClone(Object obj)
        {
            using (MemoryStream buffer = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(buffer, obj);
                buffer.Position = 0;
                object temp = formatter.Deserialize(buffer);
                return temp;
            }
        }

        private void TransformGraphics(Graphics g, Text text, Font textFont)
        {
            SizeF size = g.MeasureString(text.Txt, textFont);

            float diffX = text.X + (size.Width / 2);
            float diffY = text.Y + (size.Height / 2);

            g.TranslateTransform(diffX, diffY);
            g.RotateTransform(text.Rotation);
            g.TranslateTransform(-diffX, -diffY);

        }
        #endregion

        #region File Operations
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MainForm().Show();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "PaintText Files (*.painttxt) | *.painttxt|All Files (*.*)|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    document = Document.Open(dlg.FileName);

                    if (textOptionsDialog != null)
                        (textOptionsDialog as ITextOptionsDialog).BindingSource = document.BindingSource;

                    this.canvasPanel.Invalidate();
                    this.TopLevel = true;
                    this.Text = dlg.FileName;
                }
            }

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (document.FileName == null)
                saveAsToolStripMenuItem_Click(sender, e);
            else
                document.Save(document.FileName);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.DefaultExt = "painttxt";
                dlg.Filter = "PaintText Files (*.painttxt) | *.painttxt|All Files (*.*)|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                    document.Save(dlg.FileName);
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Oath Dialog
        private void oathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OathDialog oath = new OathDialog();
            oath.Show(this);
        }
        #endregion

        #region About Dialog
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutDialog about = new AboutDialog();
            about.Show(this);
        }
        #endregion

        #region Add Text Dialog
        private void addTextToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (Form addTextDialog = new AddTextDialog())
            {
                (addTextDialog as IAddTextDialog).OK += new EventHandler(addTextDialog_OK);
                addTextDialog.ShowDialog();
            }

        }

        private void addTextDialog_OK(object sender, EventArgs e)
        {

            string newString = (sender as IAddTextDialog).Text;
            string[] tokens = newString.Split(new char[] { ' ' });

            float x = 0;
            float y = 0;
            int rotation = 0;

            using (Graphics g = CreateGraphics())
            {

                foreach (string token in tokens)
                {
                    SizeF size = g.MeasureString(token, document.LastFont);
                    document.TextBindingList.Add(document.LastAddedText = new Text(token, x, y, document.LastFont, document.LastColor, rotation));
                    x += size.Width;
                    y += size.Height;
                }

            }

            this.canvasPanel.Invalidate();

        }
        #endregion

        #region Text Options Dialog
        private void ShowTextOptionsDialog()
        {
            savedTextBindingList = DeepClone(document.TextBindingList) as BindingList<Text>;

            textOptionsDialog = new TextOptionsDialog();

            (textOptionsDialog as ITextOptionsDialog).OK += new EventHandler(TextOptionsDialog_OK);
            (textOptionsDialog as ITextOptionsDialog).Apply += new EventHandler(TextOptionsDialog_Apply);
            (textOptionsDialog as ITextOptionsDialog).Cancel += new EventHandler(TextOptionsDialog_Cancel);
            (textOptionsDialog as ITextOptionsDialog).SendToBack += new EventHandler(TextOptionsDialog_SendToBack);
            (textOptionsDialog as ITextOptionsDialog).BringToFront += new EventHandler(TextOptionsDialog_BringToFront);

            (textOptionsDialog as ITextOptionsDialog).BindingSource = document.BindingSource;
            (textOptionsDialog as ITextOptionsDialog).setBoundaries(0.0F, 0.0F,
                this.canvasPanel.Width, this.canvasPanel.Height);

            textOptionsDialog.Show();
            textOptionsDialog = null;
        }

        private void textOptionsDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (document.TextBindingList.Count < 1)
                MessageBox.Show("No text objects on canvas!\nPlease add text using the Add Text Dialog.", "Simply Texty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                ShowTextOptionsDialog();
        }

        public void TextOptionsDialog_OK(object sender, EventArgs e)
        {
            savedTextBindingList = null;
        }

        public void TextOptionsDialog_Apply(object sender, EventArgs e)
        {
            this.canvasPanel.Invalidate();
        }

        public void TextOptionsDialog_Cancel(object sender, EventArgs e)
        {
            document.TextBindingList = DeepClone(savedTextBindingList) as BindingList<Text>;
            document.BindingSource.DataSource = document.TextBindingList;
            savedTextBindingList = null;
            this.canvasPanel.Invalidate();
        }

        public void TextOptionsDialog_SendToBack(object sender, EventArgs e)
        {
            Text savedSelectedText = DeepClone(selectedText) as Text;   // Must save copy because subsequent remove deletes reference to Text object.
            document.TextBindingList.Remove(selectedText);
            document.TextBindingList.Insert(0, savedSelectedText);
            document.BindingSource.Position = 0;
            selectedText = document.TextBindingList[0];
            this.canvasPanel.Invalidate();
        }

        public void TextOptionsDialog_BringToFront(object sender, EventArgs e)
        {
            Text savedSelectedText = DeepClone(selectedText) as Text;
            document.TextBindingList.Remove(selectedText);
            document.TextBindingList.Add(savedSelectedText);
            document.BindingSource.Position = document.TextBindingList.Count - 1;
            selectedText = document.TextBindingList[document.TextBindingList.Count - 1];
            this.canvasPanel.Invalidate();
        }
        #endregion

        #region Draw Text
        private void canvasPanel_Paint(object sender, PaintEventArgs e)
        {

            foreach (Text text in document.TextBindingList)
                drawText(text, e.Graphics);

        }

        private void drawText(Text text, Graphics g, float xDiff = 0.0F,
            float yDiff = 0.0F, bool drawSelectedTextBorder = true)
        {

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Adjust for text objects out-of-bounds.
            if (xDiff < 0)
                g.TranslateTransform(-xDiff, 0.0F);
            if (yDiff < 0)
                g.TranslateTransform(0.0F, -yDiff);

            using (Font textFont = new Font(text.FontName, text.FontSize))
            {
                TransformGraphics(g, text, textFont);
                SizeF size = g.MeasureString(text.Txt, textFont);

                //Color color = Color.FromArgb(230, 229, 235);
                Color color = Color.FromArgb(0, 116, 247);

                if (drawSelectedTextBorder == true && selectedText == text)
                {
                    float thickness = 5;

                    using (Pen pen = new Pen(Color.Red))
                        g.DrawRectangle(pen, text.X - thickness, text.Y - thickness,
                            size.Width + (thickness * 2), size.Height + (thickness * 2));

                    //color = Color.FromArgb(0, 116, 247);
                }

                using (Brush brush = new SolidBrush(color))
                    g.FillRectangle(brush, text.X, text.Y, size.Width, size.Height);

                g.DrawString(text.Txt, textFont, new SolidBrush(text.Color), text.X, text.Y);
                g.ResetTransform();
            }

        }

        #endregion

        #region Mouse Events
        private void canvasPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                using (Graphics g = (sender as Panel).CreateGraphics())
                {

                    for (int i = 0; i < document.TextBindingList.Count; i++)
                    {
                        Text text = document.TextBindingList[i];

                        using (Font textFont = new Font(text.FontName, text.FontSize)) {

                            TransformGraphics(g, text, textFont); // Account for rotated text.

                            // Accurately detect mouse click on text by changing coordinate space.
                            PointF[] ptClick = new PointF[] { new PointF(e.X, e.Y) };
                            g.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, ptClick);
                            SizeF size = g.MeasureString(text.Txt, textFont);
                            RectangleF rect = new RectangleF(text.X, text.Y, size.Width, size.Height);

                            if (rect.Contains(ptClick[0]))
                            {
                                isMouseDownOnText = true;
                                selectedText = document.TextBindingList[i];
                                document.BindingSource.Position = i;
                                selectedTextMouseDownDiff = new PointF(e.X - text.X, e.Y - text.Y);

                                if (e.Button == MouseButtons.Right)
                                    ShowTextOptionsDialog();

                                break;
                            }
                        }

                        g.ResetTransform();

                    }

                }
            }
        }

        private void canvasPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && selectedText != null && isMouseDownOnText == true)
            {

                selectedText.X = e.X - selectedTextMouseDownDiff.X;
                selectedText.Y = e.Y - selectedTextMouseDownDiff.Y;

                canvasPanel.Invalidate();

            }
        }

        private void canvasPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDownOnText = false;
            canvasPanel.Invalidate();
        }
        #endregion

        #region Import Text
        private void importTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.DefaultExt = "txt";
                openFileDialog.Filter = "Text Files (*.txt) | *.txt|All Files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    using (StreamReader streamReader = new StreamReader(openFileDialog.FileName))
                        ImportText(streamReader.ReadToEnd());
            }
        }

        private void ImportText(string text)
        {
            string[] tokens = text.Split(new char[] { ' ', '-', '\t', '\n' });

            float x = 0;
            float y = 0;
            int rotation = 0;

            using (Graphics g = CreateGraphics())
            {

                foreach (string token in tokens)
                {
                    SizeF size = g.MeasureString(token, document.LastFont);
                    document.TextBindingList.Add(document.LastAddedText = new Text(token, x, y, document.LastFont, document.LastColor, rotation));
                    if (x < this.Width)
                    {
                        x += size.Width;
                    }
                    else
                    {
                        x = 0;
                        y += size.Height;
                    }

                }

            }

            this.canvasPanel.Invalidate();
        }

        #endregion

        #region Draw Image
        private float getMin(float[] floats, float start = 0.0F)
        {

            float min = start;

            foreach (float f in floats)
                if (f < min)
                    min = f;

            return min;

        }

        private float getMax(float[] floats, float start = 0.0F)
        {

            float max = start;

            foreach (float f in floats)
                if (f > max)
                    max = f;

            return max;

        }

        private RectangleF GetCanvasBounds(float startMaxX = 0.0F, float startMaxY = 0.0F)
        {

            float minX = 0.0F;
            float maxX = startMaxX;
            float minY = 0.0F;
            float maxY = startMaxY;

            using (Graphics g = this.CreateGraphics())
            {
                foreach (Text text in document.TextBindingList)
                {
                    using (Font textFont = new Font(text.FontName, text.FontSize))
                    {
                        TransformGraphics(g, text, textFont);
                        SizeF size = g.MeasureString(text.Txt, textFont);
                        RectangleF rect = new RectangleF(text.X, text.Y, size.Width, size.Height);

                        minX = getMin(new float[] { minX, rect.Left, rect.Right });
                        maxX = getMax(new float[] { maxX, rect.Left, rect.Right });
                        minY = getMin(new float[] { minY, rect.Top, rect.Bottom });
                        maxY = getMax(new float[] { maxY, rect.Top, rect.Bottom });

                    }

                    g.ResetTransform();

                }
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);


        }

        // Floor x and y and ceiling width and height of bounds.
        private Rectangle AdjustCanvasBounds(RectangleF bounds)
        {
            int x = (int)Math.Floor(bounds.X);
            int y = (int)Math.Floor(bounds.Y);
            int width = (int)Math.Ceiling(bounds.Width);
            int height = (int)Math.Ceiling(bounds.Height);

            return new Rectangle(x, y, width, height);

        }

        private Image DrawImage()
        {
            // Adjust canvas bounds because Bitmap only takes integers (not floats)
            // as constructor args.
            Rectangle bounds = AdjustCanvasBounds(GetCanvasBounds(startMaxX: canvasPanel.Width,
                       startMaxY: canvasPanel.Height));

            Image image = new Bitmap(bounds.Width, bounds.Height);

            using (Graphics g = Graphics.FromImage(image))
            using (Brush brush = new SolidBrush(canvasPanel.BackColor))
            {
                g.FillRectangle(brush, 0, 0, bounds.Width, bounds.Height);
                g.DrawImage(canvasPanel.BackgroundImage, 0, 0, canvasPanel.Width, canvasPanel.Height);
                foreach (Text text in document.TextBindingList)
                    drawText(text, g, xDiff: bounds.X, yDiff: bounds.Y, drawSelectedTextBorder: false);

            }

            return image;
        }

        private ImageFormat GetImageFormat(string ext)
        {
            switch (ext)
            {
                case ".gif":
                    return ImageFormat.Gif;
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
                default:
                    return ImageFormat.Bmp;
            }
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.DefaultExt = "bmp";
                saveFileDialog.Filter = "BMP Files (*.bmp) | *.bmp|GIF Files (*.gif) | *.gif|JPEG Files (*.jpeg) | *.jpeg|PNG Files (*.png) | *.png|All Files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string ext = Path.GetExtension(saveFileDialog.FileName);
                    DrawImage().Save(saveFileDialog.FileName, GetImageFormat(ext));              
                    
                }

                
            }
        }

        private void importImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openImgDialog = new OpenFileDialog())
            {
                openImgDialog.DefaultExt = "bmp";
                openImgDialog.Filter = "BMP Files (*.bmp) | *.bmp|GIF Files (*.gif) | *.gif|JPEG Files (*.jpeg) | *.jpeg|PNG Files (*.png) | *.png|All Files (*.*)|*.*";

                if (openImgDialog.ShowDialog() == DialogResult.OK)
                {
                    //Analysis of colors to be added to color dialog
                    Bitmap bmp = new Bitmap(openImgDialog.FileName);
                    BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                    int[] arr = new int[bmp.Width * bmp.Height - 1];
                    Marshal.Copy(bmpData.Scan0, arr, 0, arr.Length);
                    bmp.UnlockBits(bmpData);
                    var colorCollection = arr.Distinct().ToArray();

                    //img variable is used for scaling image to application size
                    var img = new Bitmap(bmp, canvasPanel.Width, canvasPanel.Height); 
                    //set the image as part of canvas panel
                    canvasPanel.BackgroundImage = img;

                    using (ColorDialog cdlg = new ColorDialog())
                    {
                        cdlg.CustomColors = colorCollection;

                        DialogResult result = cdlg.ShowDialog();

                        if(result == DialogResult.OK)
                        {
                            canvasPanel.BackColor = cdlg.Color;
                                                
                        }
                    }

                    this.canvasPanel.Invalidate();
                    this.TopLevel = true;
                    this.Text = openImgDialog.FileName;
                }
            }

        }

        #endregion

        #region Search Dialog
        private void searchToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SearchDialog search = new SearchDialog();
            search.Accept += Properties_AcceptSearch;
            search.Double_Click += AddFile_DoubleClick;
            search.Show(this);
            searchToolStripMenuItem.Enabled = false;
        }

        void Properties_AcceptSearch(object sender, EventArgs e)
        {
            SearchDialog form = (SearchDialog)sender;
            searchToolStripMenuItem.Enabled = true;
        }

        private void AddFile_DoubleClick(object sender, EventArgs e)
        {

            string text = (sender as SearchDialog).FileText;
            ImportText(text);
        }
        #endregion

        #region Object Properties Dialog
        private void objectPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form objectPropertiesDialog = new ObjectPropertiesDialog())
            {

                BindingList<Text> savedTextBindingList = (BindingList<Text>)DeepClone(this.document.TextBindingList);

                (objectPropertiesDialog as IObjectPropertiesDialog).dataGridView.DataSource =
                    this.document.TextBindingList;

                if (objectPropertiesDialog.ShowDialog() == DialogResult.Cancel)
                {
                    this.document.TextBindingList = savedTextBindingList;
                    this.document.BindingSource.DataSource = savedTextBindingList;
                    this.canvasPanel.Invalidate();
                }
            }
        }
        #endregion

        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle realPageBounds = PrintBounds.GetRealPageBounds(e, preview);
            Rectangle realMarginBounds = PrintBounds.GetRealMarginBounds(e, preview);
            int charactersOnPage = 0;
            int linesPerPage = 0;

            if (!realPrint)
            {
                stringToPrint = textProperties + stringToPrint;
                using (StringFormat format = new StringFormat())
                {
                    PrintHeader(g, printerFont, realPageBounds);
                    PrintFooter(g, printerFont, realPageBounds, m_nPage);
                    format.SetTabStops(0, new float[] { 200, 150, 100, 100, 200, 100 } );
                    g.MeasureString(stringToPrint, printerFont, realMarginBounds.Size, format, out charactersOnPage, out linesPerPage);
                    g.DrawString(stringToPrint, printerFont, Brushes.Black, realMarginBounds, format);
                }
            
                pages[m_nPage] = stringToPrint.Substring(0, charactersOnPage);
                stringToPrint = stringToPrint.Substring(charactersOnPage);
                m_nPage++;
                e.HasMorePages = (stringToPrint.Length > 0);
            }
            else
            {
                using (StringFormat format = new StringFormat())
                {
                    PrintHeader(g, printerFont, realPageBounds);
                    PrintFooter(g, printerFont, realPageBounds, m_nPage);
                    format.SetTabStops(0, new float[] { 200, 150, 100, 100, 200, 100 });
                    g.MeasureString(pages[m_nPage], printerFont, realMarginBounds.Size, format);
                    g.DrawString(pages[m_nPage], printerFont, Brushes.Black, realMarginBounds, format);
                    m_nPage++;
                    e.HasMorePages = (m_nPage + 1) <= m_nMaxPage;
                }
            }
        }

        private void PrintHeader(Graphics g, Font font, Rectangle rect)
        {
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Near;
                g.DrawString("Group 8", font, Brushes.Black, rect, format);

                format.Alignment = StringAlignment.Far;
                format.LineAlignment = StringAlignment.Near;
                g.DrawString("Homework 10", font, Brushes.Black, rect, format);
            }

        }
        private void PrintFooter(Graphics g, Font font, Rectangle rect, int page)
        {
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Far;
                format.LineAlignment = StringAlignment.Far;
                g.DrawString(DateTime.Now.ToString(), font, Brushes.Black, rect, format);

                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Far;
                page++;
                g.DrawString(page.ToString() , font, Brushes.Black, rect, format);
            }
        }

        private void printDocument_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            preview = (e.PrintAction == PrintAction.PrintToPreview);
            printerFont = new Font("Times New Roman", 12);
            foreach (Text text in document.TextBindingList)
                stringToPrint += text.ToString();
        }

        private void printDocument_EndPrint(object sender, PrintEventArgs e)
        {
            printerFont.Dispose();
            printerFont = null;
            stringToPrint = "";
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_nPage = 0;
            this.printPreviewDialog.ShowDialog();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_nTotalPage = PageCountPrintController.GetPageCount(this.printDocument);
            m_nPage = 0;
            realPrint = true;

            printDocument.PrinterSettings.FromPage = 1;
            printDocument.PrinterSettings.ToPage = m_nTotalPage;
            printDocument.PrinterSettings.MinimumPage = 1;
            printDocument.PrinterSettings.MaximumPage = m_nTotalPage;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                if (printDialog.PrinterSettings.PrintRange ==
                    PrintRange.SomePages)
                {
                    m_nPage = printDocument.PrinterSettings.FromPage - 1;
                    m_nMaxPage = printDocument.PrinterSettings.ToPage;
                }
                else
                {
                    m_nPage = 0;
                    m_nMaxPage = m_nTotalPage;
                }
                this.printDocument.Print();
            }

            realPrint = false;
        }

        private void pasteFromClipboard(object sender, EventArgs e)
        {
            ImportText((String)Clipboard.GetData(DataFormats.Text));
        }

        private void Form_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            ImportText((String)e.Data.GetData(typeof(String)));
        }

        #region Color Palette
        private void imageColorOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(canvasPanel.BackgroundImage == null)
            {
                MessageBox.Show("STOP!","This option is for Images only.");                
            }
            else
            {
                Bitmap bmp = new Bitmap(canvasPanel.BackgroundImage);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
                int[] arr = new int[bmp.Width * bmp.Height - 1];
                Marshal.Copy(bmpData.Scan0, arr, 0, arr.Length);
                bmp.UnlockBits(bmpData);
                var colorCollection = arr.Distinct().ToArray();

                //img variable is used for scaling image to application size
                //var img = new Bitmap(bmp, canvasPanel.Width, canvasPanel.Height);
                //set the image as part of canvas panel
                //canvasPanel.BackgroundImage = img;

                using (ColorDialog cdlg = new ColorDialog())
                {
                    cdlg.CustomColors = colorCollection;

                    DialogResult result = cdlg.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        canvasPanel.BackColor = cdlg.Color;
                        Properties.Settings.Default.Save();

                    }
                }
            }
            

            this.canvasPanel.Invalidate();
           // this.TopLevel = true;
            //this.Text = openImgDialog.FileName;
        }
    }
    }

    
    
    #endregion
