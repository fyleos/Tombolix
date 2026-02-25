using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeUp.Shared.Models
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; } = false;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
