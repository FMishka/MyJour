﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyJour.Models
{
    public class Parent : User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public ICollection<StudentsByParents> StudentsByParents { get; set; }

    }
}
