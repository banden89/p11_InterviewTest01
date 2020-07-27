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
    [Authorize]
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

            string comm = "";

            using (SqlConnection conn = new SqlConnection(strConnString))
            {                
                conn.Open();

                //流水編號
                DataTable MaxIdDt = new DataTable();
                long ID_SerialNO = 0;

                comm = "SELECT MAX(CONVERT(BIGINT, SUBSTRING([Id], 3, 12))) AS [Id] FROM [dbo].[ShippingOrder]";
                SqlCommand scom = new SqlCommand(comm, conn);
                SqlDataReader sread = scom.ExecuteReader();
                MaxIdDt.Load(sread);

                if (MaxIdDt.Rows[0]["Id"].ToString() == "")
                { 
                    ID_SerialNO = 0;
                }
                else
                {
                    long maxID = long.Parse(MaxIdDt.Rows[0]["Id"].ToString());
                    if ((maxID / 10000) < Int32.Parse(DateTime.Now.ToString("yyyyMMdd")))
                    {               
                        ID_SerialNO = 1;
                    }
                    else
                    {                              
                        ID_SerialNO = maxID % 10000;
                    }
                }

                comm = "";

                for (int i = 0; i < updateOrders.Length; i++)
                {
                    ID_SerialNO += 1;
                    string str_serialID = ID_SerialNO.ToString("0000.##");

                    comm += "UPDATE [dbo].[Table] SET [Status] = 'To be shipped' WHERE [Id] = '" + updateOrders[i] + "';";
                    comm += "INSERT INTO [dbo].[ShippingOrder] ([Id], [OrderId], [Status], [CreatedDateTime]) VALUES ('SO" + DateTime.Now.ToString("yyyyMMdd") + str_serialID + "', '" + updateOrders[i] + "', 'New', CONVERT(varchar, GETDATE(), 120)); ";
                }

                SqlCommand sscom = new SqlCommand(comm, conn);
                sscom.ExecuteNonQuery();

                conn.Close();
            }

            return null;
        }

        [HttpGet]
        public ActionResult About(string id)
        {
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
    }
}