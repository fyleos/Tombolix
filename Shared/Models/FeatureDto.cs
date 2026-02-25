using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeUp.Shared.Models
{
    public class FeatureDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "no feature name"; 
        public string ShortDescription { get; set; } = "no short description";
        public string[] EnabledRoles { get; set; } = Array.Empty<string>();
        public bool IsUserAuthorized { get; set; }
    }
}
