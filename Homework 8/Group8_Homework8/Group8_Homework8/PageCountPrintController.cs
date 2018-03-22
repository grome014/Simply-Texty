using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;

namespace Group8_Homework8
{
    class PageCountPrintController : PreviewPrintController
    {
        int pageCount = 0;

        public override void OnStartPrint(
        PrintDocument document, PrintEventArgs e)
        {
            base.OnStartPrint(document, e);
            this.pageCount = 0;
        }

        public override System.Drawing.Graphics OnStartPage(
        PrintDocument document, PrintPageEventArgs e)
        {
            // Increment page count
            ++this.pageCount;
            return base.OnStartPage(document, e);
        }

        public int PageCount
        {
            get { return this.pageCount; }
        }

        // Helper method to simplify client code
        public static int GetPageCount(PrintDocument document)
        {
            // Must have a print document to generate page count
            if (document == null)
                throw new ArgumentNullException("PrintDocument must be set.");

            // Substitute this PrintController to cause a Print to initiate the
            // count, which means that OnStartPrint and OnStartPage are called
            // as the PrintDocument prints
            PrintController existingController = document.PrintController;
            PageCountPrintController controller =
            new PageCountPrintController();
            document.PrintController = controller;
            document.Print();
            document.PrintController = existingController;
            return controller.PageCount;
        }
    }
}
