﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend_app.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}