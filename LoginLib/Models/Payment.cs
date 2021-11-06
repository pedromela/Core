
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginLib.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string UserId { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string TxHash { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string ClientAddress { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string ReceiverAddress { get; set; }
        [Column(TypeName = "float")]
        public float Amount { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string Package { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ValidFrom { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ValidTo { get; set; }
    }
}
