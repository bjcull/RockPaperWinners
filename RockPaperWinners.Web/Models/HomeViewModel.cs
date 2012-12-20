using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RockPaperWinners.Web.Models
{
    public class HomeViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal CurrentFunds { get; set; }
        public string ImageSource { get; set; }
    }
}