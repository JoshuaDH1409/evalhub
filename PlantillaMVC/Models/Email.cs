using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantillaMVC.Models
{
    public class Email
    {
        public string ToName { get; set; }

        public List<string> ToEmail { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public List<string> CC { get; set; }

        public List<string> BCC { get; set; }
    }
}