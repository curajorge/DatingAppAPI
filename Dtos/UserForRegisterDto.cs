using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        //To-Do Password
        [Required]
        [DataType(DataType.Password)]
        public string  Password { get; set; }
    }
}
