using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVCCRUDwithoutEF.Data;
using MVCCRUDwithoutEF.Models;
using Newtonsoft.Json;

namespace MVCCRUDwithoutEF.Controllers
{
    public class BookController : Controller
    {
        private readonly IConfiguration _configuration;

        public BookController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        // Page home website
        // GET: Book
        public IActionResult Index()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("BookViewAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }
            return View(dtbl);
        }

        // View list book of website
        // GET: Book/ListBook
        public IActionResult ListBook()
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("BookViewAll", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtbl);
            }
            return View(dtbl);
        }

        // Change view add or edit book
        // GET: Book/AddOrEdit/
        public IActionResult AddOrEdit(int? id)
        {
            BookViewModel bookViewModel = new BookViewModel();
            if (id > 0)
                bookViewModel = FetchBookByID(id);
            return View(bookViewModel);
        }

        // Change view Register
        // GET: Book/Register
        public IActionResult Register()
        {
            return View("Views/Book/Register.cshtml");
        }

        // Add book to website
        // POST: Book/AddOrEdit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(int id, [Bind("BookID,Title,Author,Price,Image,Sale")] BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCmd = new SqlCommand("BookAddOrEdit", sqlConnection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("BookID", bookViewModel.BookID);
                    sqlCmd.Parameters.AddWithValue("Title", bookViewModel.Title);
                    sqlCmd.Parameters.AddWithValue("Author", bookViewModel.Author);
                    sqlCmd.Parameters.AddWithValue("Price", bookViewModel.Price);
                    sqlCmd.Parameters.AddWithValue("Image", bookViewModel.Image);
                    sqlCmd.Parameters.AddWithValue("Sale", bookViewModel.Sale);
                    sqlCmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(ListBook));
            }
            return View(bookViewModel);
        }

        [TempData]
        public string Message { get; set; }
        [TempData]
        public int Dem { get; set; }


        // Register user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(int id, [Bind("Tendangnhap, Matkhau, Hoten, Diachi, IdRole")] KhachHangViewModel khachHangViewModel)
        {
            if (ModelState.IsValid)
            {
                int status = FetchUser(khachHangViewModel.Tendangnhap);
                if (status == 0)
                {
                    using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                    {
                        khachHangViewModel.idRole = 1;
                        sqlConnection.Open();
                        SqlCommand sqlCmd = new SqlCommand("AddUser", sqlConnection);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("Tendangnhap", khachHangViewModel.Tendangnhap);
                        sqlCmd.Parameters.AddWithValue("Matkhau", khachHangViewModel.Matkhau);
                        sqlCmd.Parameters.AddWithValue("Hoten", khachHangViewModel.Hoten);
                        sqlCmd.Parameters.AddWithValue("Diachi", khachHangViewModel.Diachi);
                        sqlCmd.Parameters.AddWithValue("IdRole", khachHangViewModel.idRole);
                        sqlCmd.ExecuteNonQuery();
                    }
                    Message = $"Register success !";
                    return RedirectToAction(nameof(Register));
                }
                else
                {
                    Message = $"Register Fail !";
                    return RedirectToAction(nameof(Register));
                }
            }
            return View(khachHangViewModel);
        }

        // Login website
        public async Task<IActionResult> CheckUser(KhachHangViewModel khachHangViewModel)
        {
            int status = FetchUserLogin(khachHangViewModel.Tendangnhap, khachHangViewModel.Matkhau);
            if(status > 0)
            {
                KhachHangViewModel khachHangViewModel1 = new KhachHangViewModel();
                RoleViewModel roleViewModel = new RoleViewModel();
                khachHangViewModel1 = FetchUser(khachHangViewModel.Tendangnhap, khachHangViewModel.Matkhau);
                roleViewModel = FetchRole(khachHangViewModel1.idRole);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, khachHangViewModel1.Tendangnhap),
                    new Claim(ClaimTypes.GivenName, khachHangViewModel1.Hoten),
                    new Claim(ClaimTypes.Role, roleViewModel.tenRole)
                };
                // create identity
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // create principal
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal
                ,new AuthenticationProperties
                {
                    IsPersistent = true // for 'remember me' feature
                    //ExpiresUtc = DateTime.UtcNow.AddMinutes(1)
                });

                Message = $"Login Success ! ";
                if (roleViewModel.tenRole.Equals("Admin"))
                {
                    return RedirectToAction(nameof(ListBook));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                Message = $"Login Fail !";
                return RedirectToAction(nameof(Register));
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Message = @"Logout success !";
            return RedirectToAction(nameof(Index));
        }

        // Account
        public IActionResult Account(KhachHangViewModel khachHangViewModel)
        {
            KhachHangViewModel khachHang = new KhachHangViewModel();
            khachHang = FetchAccount();
            TempData["username"] = khachHang.Tendangnhap;
            TempData["name"] = khachHang.Hoten;
            TempData["address"] = khachHang.Diachi;
            return View("Views/Book/Account.cshtml");
        }

        // Delete book
        // GET: Book/Delete/5
        public IActionResult Delete(int? id)
        {
            BookViewModel bookViewModel = FetchBookByID(id);
            return View(bookViewModel);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("BookDeteleByID", sqlConnection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("BookID", id);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction(nameof(ListBook));
        }

        // Detail Book
        public IActionResult DetailBook(int? id)
        {
            /*BookViewModel bookViewModel = new BookViewModel();
            bookViewModel = FetchBookByID(id);*/
            dynamic dyModel = new ExpandoObject();
            dyModel.bookViewModel = FetchBookByID(id);
            dyModel.chiTietBookViewModel = FetchDetail(id);
            return View(dyModel);
        }

        // Grid Book
        public IActionResult GridBook(int? id)
        {
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("FetchIdTypeBook", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("ID", id);
                sqlDa.Fill(dtbl);
                return View(dtbl);
            }
        }





        // Fetch book by ID
        [NonAction]
        public BookViewModel FetchBookByID(int? id)
        {
            BookViewModel bookViewModel = new BookViewModel();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("BookViewByID", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("BookID", id);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    bookViewModel.BookID = Convert.ToInt32(dtbl.Rows[0]["BookID"].ToString());
                    bookViewModel.Title = dtbl.Rows[0]["Title"].ToString();
                    bookViewModel.Author = dtbl.Rows[0]["Author"].ToString();
                    bookViewModel.Price = Convert.ToInt32(dtbl.Rows[0]["Price"].ToString());
                    bookViewModel.Image = dtbl.Rows[0]["Image"].ToString();
                    bookViewModel.Sale = Convert.ToInt32(dtbl.Rows[0]["Sale"].ToString());
                    LoaiViewModel loaiViewModel = FetchType(Convert.ToInt32(dtbl.Rows[0]["maloai"].ToString()));
                    bookViewModel.Maloai = loaiViewModel.Maloai;
                    bookViewModel.Tenloai = loaiViewModel.Tenloai;
                }
                return bookViewModel;
            }
        }

        // Fetch user exist with Tendangnhap
        public int FetchUser(string Tendangnhap)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                int status = 0;
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("FetchUser", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Tendangnhap", Tendangnhap);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    status = 1;
                }
                
                return status;
            }
        }

        // Fetch User with Tendangnhap and Matkhau
        public int FetchUserLogin(string Tendangnhap, string Matkhau)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                int status = 0;
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("CheckUser", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Tendangnhap", Tendangnhap);
                sqlDa.SelectCommand.Parameters.AddWithValue("Matkhau", Matkhau);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    status = 1;
                }

                return status;
            }
        }

        //Fetch role user
        public RoleViewModel FetchRole(int idRole)
        {
            RoleViewModel roleViewModel = new RoleViewModel();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("GetRole", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("IdRole", idRole);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    roleViewModel.tenRole = dtbl.Rows[0]["tenRole"].ToString();
                }
                return roleViewModel;
            }
        }

        // Fetch user Login
        public KhachHangViewModel FetchUser(string Tendangnhap, string Matkhau)
        {
            KhachHangViewModel khachHangViewModel = new KhachHangViewModel();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("CheckUser", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Tendangnhap", Tendangnhap);
                sqlDa.SelectCommand.Parameters.AddWithValue("Matkhau", Matkhau);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    khachHangViewModel.Tendangnhap = dtbl.Rows[0]["tendangnhap"].ToString();
                    khachHangViewModel.Hoten = dtbl.Rows[0]["hoten"].ToString();
                    khachHangViewModel.Diachi = dtbl.Rows[0]["diachi"].ToString();
                    khachHangViewModel.idRole = Convert.ToInt32(dtbl.Rows[0]["idRole"].ToString());
                }

                return khachHangViewModel;
            }
        }

        // Fetch detail Book
        public ChiTietBookViewModel FetchDetail(int? id)
        {
            ChiTietBookViewModel chiTietBookViewModel = new ChiTietBookViewModel();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("FetchDetail", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("BookID", id);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    chiTietBookViewModel.Id = Convert.ToInt32(dtbl.Rows[0]["id"].ToString());
                    chiTietBookViewModel.BookID = Convert.ToInt32(dtbl.Rows[0]["BookID"].ToString());
                    chiTietBookViewModel.Nhacungcap = dtbl.Rows[0]["nhacungcap"].ToString();
                    chiTietBookViewModel.Nhaxuatban = dtbl.Rows[0]["nhaxuatban"].ToString();
                    chiTietBookViewModel.Hinhthuc = dtbl.Rows[0]["hinhthuc"].ToString();
                    chiTietBookViewModel.Nguoidich = dtbl.Rows[0]["nguoidich"].ToString();
                    chiTietBookViewModel.Mota = dtbl.Rows[0]["mota"].ToString();
                    chiTietBookViewModel.Noidung = dtbl.Rows[0]["noidung"].ToString();
                }
            }
                return chiTietBookViewModel;
        }


        // Fetch Type Book
        public LoaiViewModel FetchType(int? id)
        {
            LoaiViewModel loaiViewModel = new LoaiViewModel();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("FetchType", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("ID", id);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    loaiViewModel.Maloai = Convert.ToInt32(dtbl.Rows[0]["maloai"].ToString());
                    loaiViewModel.Tenloai = dtbl.Rows[0]["tenloai"].ToString();
                }
            }
            return loaiViewModel;
        }


        // Fetch account
        public KhachHangViewModel FetchAccount()
        {
            KhachHangViewModel khachHangViewModel = new KhachHangViewModel();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                DataTable dtbl = new DataTable();
                sqlConnection.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("FetchUser", sqlConnection);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDa.SelectCommand.Parameters.AddWithValue("Tendangnhap", User.Identity.Name);
                sqlDa.Fill(dtbl);
                if (dtbl.Rows.Count == 1)
                {
                    khachHangViewModel.Tendangnhap = dtbl.Rows[0]["tendangnhap"].ToString();
                    khachHangViewModel.Hoten = dtbl.Rows[0]["hoten"].ToString();
                    khachHangViewModel.Diachi = dtbl.Rows[0]["diachi"].ToString();
                    khachHangViewModel.Matkhau = dtbl.Rows[0]["matkhau"].ToString();
                    khachHangViewModel.idRole = Convert.ToInt32(dtbl.Rows[0]["idRole"].ToString());
                }

                return khachHangViewModel;
            }
        }
        // Change Password
        public async Task<IActionResult> ChangePassword(int id, [Bind("Tendangnhap, Matkhau, Hoten, Diachi, IdRole")] KhachHangViewModel khachHangViewModel)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
            {
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("ChangePassword", sqlConnection);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("Tendangnhap", khachHangViewModel.Tendangnhap);
                sqlCmd.Parameters.AddWithValue("Matkhau", khachHangViewModel.Matkhau);
                sqlCmd.ExecuteNonQuery();
            }
            await HttpContext.SignOutAsync();
            Message = @"Change password success !";
            return RedirectToAction(nameof(Register));
        }
    }
}
