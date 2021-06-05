using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCRUDwithoutEF.Models
{
    public class RoleViewModel
    {
        [Key]
        public int idRole { get; set; }
        [Required]
        public string tenRole { get; set; }

    }
}
