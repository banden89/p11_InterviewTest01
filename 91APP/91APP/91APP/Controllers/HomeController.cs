using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using _91APP.Models;

namespace _91APP.Controllers
{
    public class HomeController : Controller
    {
        string strConnString = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;

        public ActionResult Index()
        {
            List<Order> OrderLists = new List<Order>();
            DataTable tableReader = new DataTable();      

            using (SqlConnection conn = new SqlConnection(strConnString))
            {
                conn.Open();
                SqlCommand scom = new SqlCommand("SELECT * FROM [dbo].[Table]", conn);

                SqlDataReader sread = scom.ExecuteReader(CommandBehavior.CloseConnection);

                tableReader.Load(sread);

                for(int i = 0; i < tableReader.Rows.Count; i++)
                {
                    Order order = new Order();
                    order.Id = tableReader.Rows[i]["Id"].ToString();
                    order.Item = tableReader.Rows[i]["Item"].ToString();
                    order.Price = Convert.ToInt32(tableReader.Rows[i]["Price"]);
                    order.Cost = Convert.ToInt32(tableReader.Rows[i]["Cost"]);
                    order.Status = tableReader.Rows[i]["Status"].ToString();
                    OrderLists.Add(order);
                }

                conn.Close();
            }

            ViewBag.orders = OrderLists;

            return View();
        }

        [HttpPost]
        public ActionResult Index(string[] updateOrders)
        {
            if (updateOrders == null)
            {
                return null;
            }

            string updatecomm = "";

            using (SqlConnection conn = new SqlConnection(strConnString))
            {
                conn.Open();                

                for(int i = 0; i < updateOrders.Length; i++)
                {
                    updatecomm += "UPDATE [dbo].[Table] SET [Status] = 'To be shipped' WHERE [Id] = '" + updateOrders[i] + "';";
                }

                SqlCommand scom = new SqlCommand(updatecomm, conn);

                scom.ExecuteNonQuery();

                conn.Close();
            }

            return null;
        }

        public ActionResult About(string id)
        {
            //ViewBag.Message = "Your application description page.";

            List<Order> OrderLists = new List<Order>();
            DataTable tableReader = new DataTable();

            using (SqlConnection conn = new SqlConnection(strConnString))
            {
                conn.Open();
                SqlCommand scom = new SqlCommand("SELECT * FROM [dbo].[Table] WHERE [Id] = '" + id + "';", conn);

                SqlDataReader sread = scom.ExecuteReader(CommandBehavior.CloseConnection);

                tableReader.Load(sread);

                Order order = new Order();
                order.Id = tableReader.Rows[0]["Id"].ToString();
                order.Item = tableReader.Rows[0]["Item"].ToString();
                order.Price = Convert.ToInt32(tableReader.Rows[0]["Price"]);
                order.Cost = Convert.ToInt32(tableReader.Rows[0]["Cost"]);
                order.Status = tableReader.Rows[0]["Status"].ToString();
                OrderLists.Add(order);
                

                conn.Close();
            }

            ViewBag.orders = OrderLists;

            return View();
        }

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}