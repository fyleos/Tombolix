using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TradeUp.Shared.Models
{
    public class SharedUserOption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        
        public string CurrentTheme { get; set; } = "theme-light";
    }
}
