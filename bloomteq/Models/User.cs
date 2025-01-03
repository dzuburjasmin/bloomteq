using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace bloomteq.Models
{
    [Table("USERS")]
    public class User : IdentityUser
    {
        [StringLength(20)]
        public string Name { get; set; }
        public virtual ICollection<Shift> Shifts { get; set; }
}

}
