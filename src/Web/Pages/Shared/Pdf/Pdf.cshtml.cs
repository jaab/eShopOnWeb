using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components.RenderTree;
using System;
using Microsoft.AspNetCore.Http;

namespace Microsoft.eShopWeb.Web.Pages.Shared.Pdf
{

    public class PdfModel : PageModel
    {

        public IActionResult OnPost(int id)
        {

           IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();
           Renderer.RenderUrlAsPdf("https://localhost:5001").SaveAs("Catalogo.pdf");

           /*MemoryStream rend=Renderer.RenderUrlAsPdf("https://localhost:5001").Stream;
            HttpContext.Current.Response.AddHeader("Content-Type", "application/pdf");
            HttpContext.Current.Response.AddHeader("Content-Disposition", String.Format("{0}; filename=Print.pdf; size={1}",
              "attachment", rend.Length.ToString()));
            HttpContext.Current.Response.BinaryWrite(rend.ToArray());
            HttpContext.Current.Response.End();*/

             string filePath = System.IO.Path.GetFullPath("Catalogo.pdf");
              var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
               return File(stream, "application/pdf", "Catalogo.pdf");
               

        }

     /*   public IActionResult Get(int id)
        { 
          var stream = new FileStream(@"C:\Users\Utilizador\Google Drive\javanet\netcore\eShopOnWeb\src\Web\Catalogo.pdf", FileMode.Open, FileAccess.Read);
           return File(stream, "application/pdf", "Catalogo.pdf");
        }*/

        

    }
}