using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace bloomteq.Models
{
    [Table("SHIFT")]
    public class Shift
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int Hours { get; set; }

        [StringLength(100)]
        public string Description{ get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }

}

}
