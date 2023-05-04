using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyJour.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
