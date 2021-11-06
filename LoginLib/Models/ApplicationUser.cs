using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginLib.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName ="nvarchar(150)")]
        public string FullName { get; set; }
        [Column(TypeName = "bigint")]
        public int MaxAllowedBots { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Package { get; set; }
    }
}
