using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;
using _91APP.Models;

namespace _91APP.Controllers
{
    public class AccountController : Controller
    {
        string strConnString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Account acc)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var tb = new DataTable();

            using (SqlConnection conn = new SqlConnection(strConnString))
            {
                //SQL injection 改用SQL Parameter避免之
                //SqlCommand com = new SqlCommand("SELECT * FROM [dbo].[Members] WHERE Email='" + acc.Email + "' AND Password='" + acc.Password + "';", conn);
                SqlCommand com = new SqlCommand("SELECT * FROM [dbo].[Members] WHERE Email = @Email AND Password = @Password;", conn);
                com.Parameters.AddWithValue("@Email", acc.Email);
                com.Parameters.AddWithValue("@Password", acc.Password);

                conn.Open();
                SqlDataReader sread = com.ExecuteReader();
                tb.Load(sread);
                conn.Close();
            }

            var user = tb
                        .AsEnumerable()
                        .FirstOrDefault(x => x.Field<string>("Email") == acc.Email);

            //DB無此會員
            if (user == null)
            {
                ModelState.AddModelError("Password", "請輸入正確的帳號或密碼!");
                return View(acc);
            }

            //DB有此會員
            if (user.Field<string>("Password").Equals(acc.Password))
            {
                var now = DateTime.Now;
                var ticket = new FormsAuthenticationTicket(
                    version: 1,
                    name: user.Field<string>("Email"),
                    issueDate: now,
                    expiration: now.AddSeconds(30),
                    isPersistent: true,
                    userData: user.Field<string>("Email").ToString(),
                    cookiePath: FormsAuthentication.FormsCookiePath);

                var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("Password", "請輸入正確的帳號或密碼!");
                return View();
            }
        }
    }
}