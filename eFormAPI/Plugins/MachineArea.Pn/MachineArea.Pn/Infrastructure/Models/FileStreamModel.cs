using System.IO;

namespace MachineArea.Pn.Infrastructure.Models
{
    public class FileStreamModel
    {
        public string FilePath { get; set; }
        public FileStream FileStream { get; set; }
    }
}
