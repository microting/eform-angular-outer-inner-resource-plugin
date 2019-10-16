using System.IO;

namespace OuterInnerResource.Pn.Infrastructure.Models
{
    public class FileStreamModel
    {
        public string FilePath { get; set; }
        public FileStream FileStream { get; set; }
    }
}
