using System.Text;
using Markdig;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PrayerTimes.Web.Pages
{
    public class DocumentationModel : PageModel
    {
        private readonly ILogger<DocumentationModel> _logger;

       
        public string Documentation { get; set; } = string.Empty;

        public DocumentationModel(ILogger<DocumentationModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            string documentation;
            using (var streamReader = new StreamReader(@"Documentation/Documentation.html", Encoding.UTF8))
            {
                documentation = streamReader.ReadToEnd();
            }

            
            Documentation = documentation;
        }
    }
}
