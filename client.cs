using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BankomatM
{
    internal class client
    {        
        string client_name = " ";
        int client_status = 0;
        readonly MySqlConnection conn = DBUtils.GetDBConnection();

        public int get_client_status(int card, int pin_code)  //Проверка активности клиента
        {                        
            conn.Open();
            string query = "select status from client where card_number = " + card + " and pin = " + pin_code + ";";
            MySqlCommand comm = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = comm.ExecuteReader())
            {
                while (reader.Read())
                {
                    client_status = Convert.ToInt32(reader.GetValue(0));
                }
                reader.Close();                
            }
            conn.Close();
            return client_status;
        }
        public string get_client_name(int card, int pin_code)  //Получение имени клиента
        {
            conn.Open();
            string query = "select name from client where card_number = " + card + " and pin = " + pin_code + ";";
            MySqlCommand comm = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = comm.ExecuteReader())
            {
                while (reader.Read())
                {
                    client_name = Convert.ToString(reader.GetValue(0));
                }
                reader.Close();
            }
            conn.Close();
            return client_name;
        }
    }
}
