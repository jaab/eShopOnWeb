using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;



namespace Microsoft.eShopWeb.Web.Pages.Shared.Pdf
{

    public class PdfModel : PageModel
    {


        public void OnPost()
        {

            IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();
            Renderer.RenderUrlAsPdf("https://localhost:5001").SaveAs("Catalogo.pdf");
            
            // or System.IO.MemoryStream PdfStream = PDF.GetStream;
            //byte[] PdfBinary = PDF.GetBinary;

    
           

        }

        

    }
}