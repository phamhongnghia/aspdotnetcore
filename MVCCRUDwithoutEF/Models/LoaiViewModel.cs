using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCRUDwithoutEF.Models
{
    public class LoaiViewModel
    {
        [Key]
        public int Maloai { get; set; }
        [Required]
        public string Tenloai { get; set; }
    }
}
