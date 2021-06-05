using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCRUDwithoutEF.Models
{
    public class BookViewModel
    {
        /*[Key]
        public int id { get; set; }
        [Required]
        public string tensp { get; set; }
        [Required]
        public string tacgia { get; set; }
        [Required]

        public int giagoc { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Should be greated than or equal to 1")]
        public int giamgia { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Should be greated than or equal to 1")]
        public string hinhanh { get; set; }
        [Required]*/


        [Key]
        public int BookID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Range(1,int.MaxValue,ErrorMessage ="Should be greated than or equal to 1")]
        public int Price { get; set; }
        [Required]
        public string Image { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Should be greated than or equal to 1")]
        public int Sale { get; set; }
        [Required]
        public int Maloai { get; set; }
        [Required]
        public string Tenloai { get; set; }
    }
}
