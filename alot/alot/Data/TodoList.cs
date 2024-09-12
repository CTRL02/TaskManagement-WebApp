using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alot.Data
{
    public class TodoList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required DateTime DateTime { get; set; }

        [Required]
        public required string Todo { get; set; }

        public bool IsCompleted { get; set; }

        public required string UserId { get; set; }
        [ForeignKey("UserId")]

        public User User { get; set; }
    }
}
