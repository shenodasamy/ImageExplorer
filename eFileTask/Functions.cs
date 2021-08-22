using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eFileTask
{
    public static class Functions
    {
        public static string GetUnitName(this string path)
        {
            string[] parts = path.Split(@"\");
            return parts.Last();
        }
        public static bool IsImage(this string fileName)
        {
            string extension = fileName.Split('.').Last().ToLower();
            string[] allowedExtensions = { "jpg", "jpeg", "png", "tiff", "gif" };
            return allowedExtensions.Contains(extension);
        }
        private static bool IsEmail(this string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            return match.Success;
        }
        public static byte[] ToByte(this Image img)
        {
            byte[] byteArray = new byte[0];
            using MemoryStream stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Close();

            byteArray = stream.ToArray();
            return byteArray;
        }

        // SQL Functions
        public static (int id, int count) GetImageCount_Id(this SqlConnection con, byte[] image)
        {
            int count = 0;
            int id = 0;
            String query = "select id from [dbo].[Images] where image=@image";
            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(new SqlParameter("@image", image));
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                count++;
                if (count == 1)
                    id = int.Parse(dr["Id"].ToString());
            }
            return (id, count);
        }
        public static void InsertData(this SqlConnection con, byte[] image, string name, string address, string email)
        {
            if (!email.IsEmail())
            {
                MessageBox.Show("Type a valid Email");
                return;
            }

            string query = "Insert into [dbo].[Images] (Image, Name, Address, Email) values (@image, @name, @address, @email)";
            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.addParameters(image, name, address, email);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Inserted Successfully");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error While Inserting");
                MessageBox.Show(ex.Message);
            }
        }
        public static void UpdateData(this SqlConnection con, byte[] image, string name, string address, string email)
        {
            if (!email.IsEmail())
            {
                MessageBox.Show("Type a valid Email");
                return;
            }

            string query = "update [dbo].[Images] set Name=@name, Address=@address, Email=@email where image = @image";
            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.addParameters(image, name, address, email);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Updated Successfully");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error While Updating");
                MessageBox.Show(ex.Message);
            }
        }
        public static (string name, string address, string email) GetData(this SqlConnection con, int id)
        {
            (string name, string address, string email) item = new();

            var query = "select Name, Address, Email from [dbo].[Images] where Id = @Id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(new SqlParameter("@id", id));
            SqlDataReader dr = cmd.ExecuteReader();
            int count = 0;
            while (dr.Read())
            {
                count++;
                if (count > 0)
                {
                    item.name = dr["Name"].ToString() ?? String.Empty;
                    item.address = dr["Address"].ToString() ?? String.Empty;
                    item.email = dr["Email"].ToString() ?? string.Empty;
                    return item;
                }
            }
            return item;
        }
        private static void addParameters(this SqlCommand cmd, byte[] image, string name, string address, string email)
        {
            cmd.Parameters.Add(new SqlParameter("@image", image));
            cmd.Parameters.Add(new SqlParameter("@name", name));
            cmd.Parameters.Add(new SqlParameter("@address", address));
            cmd.Parameters.Add(new SqlParameter("@email", email));
        }
    }
}
