using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCRUDwithoutEF.Models
{
    public class KhachHangViewModel
    {
        [Key]
        public string Tendangnhap { get; set; }
        [Required]
        public string Matkhau { get; set; }
        [Required]
        public string Hoten { get; set; }
        [Required]
        public string Diachi { get; set; }
        [Required]
        public int idRole { get; set; }
    }
}
