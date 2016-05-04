using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaperScissorStone1.Models
{
    public class HomeViewModel
    {
        public bool IsLogin { get; set; }

        [Required(ErrorMessage = "The name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        public IEnumerable<string> Errors { get; set; }
    }
}