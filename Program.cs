using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDBUtil
{
    class User
    {
        private int id;
        private string username;
        private string password;
        private string fullname;

        public User(string username, string password, string fullname)
        {
            this.username = username;
            this.password = password;
            this.fullname = fullname;
        }
        public string Fullname
        {
            get { return fullname; }
            set { fullname = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

    }

    class Program
    {

        //hàm dùng để truy xuất dữ liệu
        public static List<User> QueryUsers(SqlConnection con)
        {
            List<User> result = new List<User>();
            string sql = "SELECT * " +
                            "From tblUser";

            //tạo 1 đối tượng SQLCommand
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = sql;

            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //Lấy chỉ số của cột ID trong câu lệnh sql
                        int idIndex = reader.GetOrdinal("id"); //cột id đứng đầu nên có index = 0
                        int id = Convert.ToInt32(reader.GetValue(idIndex)); //lấy id từ trong cột id
                        //Lấy username trong cột username có index = 1
                        string username = reader.GetString(1);
                        string password = reader.GetString(2);
                        string fullname = reader.GetString(3);
                        User newUser = new User(username, password, fullname);
                        newUser.ID = id;

                        result.Add(newUser);
                    }
                }
            }
            return result;
        }

        public static User QueryUserByID(SqlConnection con, int id)
        {
            User result = null;
            try
            {
                SqlCommand cmd = con.CreateCommand();
                string sql = "SELECT * " +
                                "FROM tblUser " +
                                "WHERE id = @id";
                cmd.CommandText = sql;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            //Lấy id
                            int userID = reader.GetInt32(0);
                            //lấy username
                            string username = reader.GetString(1);
                            string password = reader.GetString(2);
                            string fullname = reader.GetString(3);

                            result = new User(username, password, fullname);
                            result.ID = userID;
                        }
                    }
                }
                

            } catch (Exception e)
            {
                Console.WriteLine("Error at QueryUserByID: " + e.ToString());
                Console.WriteLine(e.StackTrace);
            }
            return result;
        }

        public static bool InsertUser(SqlConnection con, User user)
        {
            bool result = false;
            //Câu lệnh dùng để insert
            string sql = "INSERT INTO tblUser (username, password, fullname) " +
                            "VALUES(@username, @password, @fullname)";

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = sql;

            //Thêm tham số cho từng cái values được truyền đi
            //cmd.Parameters.Add("@id", SqlDbType.Int).Value = user.ID;
            cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = user.Username;
            cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = user.Password;
            cmd.Parameters.Add("@fullname", SqlDbType.NVarChar).Value = user.Fullname;
            //câu lệnh thực thi add dữ liệu vào database
            int rowCount = cmd.ExecuteNonQuery();
            if (rowCount >= 1)
            {
                result = true;
            }
            return result;
        }

        public static bool UpdateUser(SqlConnection con, User user)
        {
            return false;
        }
        //kết nối theo kiểu hướng đối tượng
        static void Main(string[] args)
        {
            SqlConnection con = null;
            try
            {
                con = DBUtils.GetConnection();
                if (con != null)
                {
                    Console.WriteLine("Connecting to database... succeeded");
                    con.Open();
                    Console.WriteLine("Opened dabatase");

                    List<User> users = QueryUsers(con);
                    if (users.Count == 0)
                    {
                        Console.WriteLine("The list is empty");
                    }
                    else
                    {
                        foreach (User user in users)
                        {
                            Console.WriteLine("--------------------------");
                            Console.WriteLine("User ID: {0}", user.ID);
                            Console.WriteLine("Username: {0}", user.Username);
                            Console.WriteLine("Password: {0}", user.Password);
                            Console.WriteLine("Full name: {0}", user.Fullname);
                        }
                    }
                    Console.WriteLine("----------INSERTION------------");
                    Console.WriteLine("Add a new user");
                    bool AddResult = InsertUser(con, new User("huychi", "123asdweg12", "Phùng Chí Huy"));
                    AddResult = InsertUser(con, new User("ahihidongok", "5321351324", "Nguyễn Nhựt Hào"));
                    if (AddResult)
                    {
                        users = QueryUsers(con);
                        foreach (User user in users)
                        {
                            Console.WriteLine("--------------------------");
                            Console.WriteLine("User ID: {0}", user.ID);
                            Console.WriteLine("Username: {0}", user.Username);
                            Console.WriteLine("Password: {0}", user.Password);
                            Console.WriteLine("Full name: {0}", user.Fullname);
                        }
                    } else
                    {
                        Console.WriteLine("Failed to add");
                    }
                    Console.WriteLine("------------------------------------------------------------");
                    Console.WriteLine("Input the id you want to update: ");
                    int uID = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Update a record which id is {0}", uID);
                    User uUser = QueryUserByID(con, uID);
                    if (uUser != null)
                    {
                        Console.WriteLine("Update menu");
                    } else
                    {
                        Console.WriteLine("You don't have that id in your database");
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine("Error at Main: " + e.Message);
                Console.WriteLine(e.StackTrace);
            } finally
            {
                try
                {
                    if (con != null)
                    {
                        con.Close(); //đóng kết nối
                        con.Dispose(); //hủy đối tượng để giải phóng tài nguyên
                    }
                } catch (Exception e)
                {
                    Console.WriteLine("Error while closing connection");
                }
                
            }
            
        }



        //kết nối theo kiểu cũ
        //static void Main(string[] args)
        //{
        //    SqlConnection con;
        //    DataTable tblUser = new DataTable("tblUser");
        //    SqlDataAdapter da = new SqlDataAdapter();

        //    //copy data source vào chuỗi
        //    string strConnection = @"Data Source=SE140675\SQLEXPRESS;Initial Catalog=DemoCsharp;User ID=sa;password=123";
        //    try
        //    {
        //        con = new SqlConnection(strConnection);
        //        if (con != null)
        //        {
        //            //mở cổng kết nối tới SQL Server
        //            con.Open();
        //            Console.WriteLine("Connect to database successfully");
        //        }


        //        con.Close();
        //        con.Dispose();
        //        con = null;
        //    } catch (Exception e)
        //    {
        //        Console.WriteLine("Error at DemoDBUtil: " + e.Message);
        //    }

        //}
    }
}
