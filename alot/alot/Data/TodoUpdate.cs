using System.ComponentModel.DataAnnotations;

namespace alot.Data
{
    public class TodoUpdate
    {
        [Required]
        public required DateTime DateTime { get; set; }

        [Required]
        public required string Todo { get; set; }
    }
}
