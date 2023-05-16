using System.ComponentModel.DataAnnotations;

namespace CapcomFitness.Data
{
    public class JSONUploadModel
    {
        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileType { get; set; }

        [Required]
        public byte[] FileContent { get; set; }

    }
}
