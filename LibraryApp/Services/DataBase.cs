using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace LibraryApp
{
    public class DataBase
    {
        private DataBase()
        {
        }

        private static readonly Lazy<DataBase> instance = new Lazy<DataBase>(() => new DataBase(), true);
        public static DataBase Instance { get { return instance.Value; } }

        public SqlConnection sqlConnection = new SqlConnection("server=.;database=Library;Integrated Security=SSPI;");

        private void ShowUsers()
        {
            SqlDataAdapter oleDbDataAdapter = new SqlDataAdapter("select * from User", sqlConnection);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);
        }

        private void ShowUserBookRecords()
        {
            SqlDataAdapter oleDbDataAdapter = new SqlDataAdapter("select * from User u inner join Book b on u.UserId=b.BookId", sqlConnection);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);
        }

        public void SaveBook(string bookName, string isbn)
        {
            SqlCommand sqlCommand = new SqlCommand("insert Book values(@BookName,@ISBN)", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@BookName", bookName);
            sqlCommand.Parameters.AddWithValue("@ISBN", isbn);

            sqlConnection.Open();
            int etkilenen = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (etkilenen > 0)
            {
                Console.WriteLine("Kayıt eklendi");
                ShowUsers();
            }
            else
                Console.WriteLine("Kayıt ekleneMEdi!");
        }

        public void SaveUserBook(string userId, string bookId)
        {
            SqlCommand sqlCommand = new SqlCommand("insert UserBook values(@UserId,@BookId)", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@UserId", userId);
            sqlCommand.Parameters.AddWithValue("@BookId", bookId);

            sqlConnection.Open();
            int etkilenen = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (etkilenen > 0)
            {
                Console.WriteLine("Kayıt eklendi");
                ShowUsers();
            }
            else
                Console.WriteLine("Kayıt ekleneMEdi!");
        }

        public void KaydetFisBilgilerini(int isletmeId, string fisNo, string tarih, string urunlerKdvlerFiyatlar, string toplamFiyat)
        {
            SqlCommand sqlCommand = new SqlCommand("insert Fis values(@isletmeId,@fisNo,@tarih,@urunlerKdvlerFiyatlar,@toplamFiyat)", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@isletmeId", isletmeId);
            sqlCommand.Parameters.AddWithValue("@fisNo", fisNo);
            sqlCommand.Parameters.AddWithValue("@tarih", tarih);
            sqlCommand.Parameters.AddWithValue("@urunlerKdvlerFiyatlar", urunlerKdvlerFiyatlar);
            sqlCommand.Parameters.AddWithValue("@toplamFiyat", toplamFiyat);

            sqlConnection.Open();
            int etkilenen = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (etkilenen > 0)
            {
                Console.WriteLine("Kayıt eklendi");
                ShowUsers();
            }
            else
                Console.WriteLine("Kayıt ekleneMEdi!");
        }

        public void KaydetIsletmeFisBilgilerini(string isletmeAdi, string isletmeAdresi, string tarih, string urunlerKdvlerFiyatlar, string toplamFiyat)
        {
            SqlCommand sqlCommand = new SqlCommand("insert IsletmeFis values(@isletmeAdi,@isletmeAdresi,@tarih,@urunlerKdvlerFiyatlar,@toplamFiyat)", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@isletmeAdi", isletmeAdi);
            sqlCommand.Parameters.AddWithValue("@isletmeAdresi", isletmeAdresi);
            sqlCommand.Parameters.AddWithValue("@tarih", tarih);
            sqlCommand.Parameters.AddWithValue("@urunlerKdvlerFiyatlar", urunlerKdvlerFiyatlar);
            sqlCommand.Parameters.AddWithValue("@toplamFiyat", toplamFiyat);

            sqlConnection.Open();
            int etkilenen = sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();

            if (etkilenen > 0)
            {
                Console.WriteLine("Kayıt eklendi");
                ShowUsers();
            }
            else
                Console.WriteLine("Kayıt ekleneMEdi!");
        }

        //private void textBox1_TextChanged(object sender, EventArgs e)
        //{
        //    SqlDataAdapter oleDbDataAdapter = new SqlDataAdapter("select * from IsletmeFis where IsletmeAdi like '%" + textBox1.Text + "%'", sqlConnection);
        //    DataTable dataTable = new DataTable();
        //    oleDbDataAdapter.Fill(dataTable);
        //}

        //private void textBox2_TextChanged(object sender, EventArgs e)
        //{
        //    SqlDataAdapter oleDbDataAdapter = new SqlDataAdapter("select * from IsletmeFis where Tarih like '%" + textBox2.Text + "%'", sqlConnection);
        //    DataTable dataTable = new DataTable();
        //    oleDbDataAdapter.Fill(dataTable);
        //}
    }
}