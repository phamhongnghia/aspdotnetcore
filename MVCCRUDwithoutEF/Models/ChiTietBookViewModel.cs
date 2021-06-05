using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCRUDwithoutEF.Models
{
    public class ChiTietBookViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int BookID { get; set; }
        [Required]
        public string Nhacungcap { get; set; }
        [Required]
        public string Nhaxuatban { get; set; }
        [Required]
        public string Hinhthuc { get; set; }
        [Required]
        public string Nguoidich { get; set; }
        [Required]
        public string Mota { get; set; }
        [Required]
        public string Noidung { get; set; }
    }
}
