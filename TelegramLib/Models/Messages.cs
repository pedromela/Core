using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TelegramLib.Models
{
    public class Messages
    {
        [Key]
        public int id { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string Name { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string Text { get; set; }
    }
}
