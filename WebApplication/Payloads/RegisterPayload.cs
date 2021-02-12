using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Payloads
{
    public class RegisterPayload
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Pass { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
