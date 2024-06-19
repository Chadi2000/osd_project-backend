using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend_app.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public DateTime DueDate { get; set; }
        public string Estimate { get; set; }
        public string Importance { get; set; }
        public string Email { get; set; }
    }
}