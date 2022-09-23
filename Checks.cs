using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BankomatM
{
    internal class Checks
    {
        int client_pin = 0000;
        int client_card_number = 0000;
        
        client clnt = new();
        readonly MySqlConnection conn = DBUtils.GetDBConnection();

        public bool check_card_number(int card_number)  //Проверка номера карты
        {
            conn.Open();
            string query = "select card_number from client where card_number = " + card_number + ";";
            MySqlCommand comm = new MySqlCommand(query, conn);
            MySqlDataReader reader = comm.ExecuteReader();
            reader.Read();
            if (reader.HasRows)  //Проверка на наличие строк
            {
                client_card_number = Convert.ToInt32(reader.GetValue(0));
                if (client_card_number == card_number)
                {
                    reader.Close();
                    conn.Close();
                    return true;
                }
                else
                {
                    reader.Close();
                    conn.Close();
                    Console.Clear();
                    Console.WriteLine("\n\nКарта не читается!\n");
                    return false;
                }                
            }
            else
            {
                reader.Close();
                conn.Close();
                Console.Clear();
                Console.WriteLine("\n\nКарта не читается!\n");
                return false;
            }
        }
        public bool check_pin(int card_number, int pin)  //Проверка Пин-кода
        {
            conn.Open();
            string query = "select pin from client where card_number = " + card_number + ";";
            MySqlCommand comm = new MySqlCommand(query, conn);
            MySqlDataReader reader = comm.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                client_pin = Convert.ToInt32(reader.GetValue(0));
                if (client_pin == pin)
                {
                    reader.Close();
                    conn.Close();
                    return true;
                }
                else
                {
                    reader.Close();
                    conn.Close();
                    Console.Clear();
                    Console.WriteLine("\n\nНе правильный пин-код!\n");
                    return false;
                }                 
            }
            else
            {
                reader.Close();
                conn.Close();
                Console.Clear();
                Console.WriteLine("\n\nНе правильный пин-код!\n");
                return false;
            }
        }

        public bool check_client_status(int card_number, int pin)  //Проверка активен ли клиент
        {            
            if (clnt.get_client_status(card_number, pin) == 1)
            {
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                Console.Clear();
                Console.WriteLine("\n\nКлиент заблокирован. Просьба обратиться в банк.\n");
                return false;
            }
            
        }
        public double check_bankomat_ostatok(int currency_code)  //Проверка доступного баланса банкомата
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            double ostatok = 0;
            conn.Open();
            string query = "select summa from bankomat_balance where currency_code = " + currency_code + ";";
            MySqlCommand comm = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = comm.ExecuteReader())
            {
                while (reader.Read())
                {
                    ostatok = Convert.ToDouble(reader.GetValue(0));
                }
                reader.Close();
            }
            conn.Close();
            return ostatok;
        }
        public bool check_bankomat_change_ostatok(double amount, int currency_code)  //Проверка возможности снятия с остатка банкомата
        {
            if ((check_bankomat_ostatok(currency_code) - amount) > 0)
            {
                return true;
            }
            else
            {
                Console.WriteLine("К сожалению в банкомате недостаточно средств.");
                return false;
            }
        }
        public bool check_kratnost(double summa, int currency_code)  //Проверка на кратность 
        {
            int rub_eur_a = 10;
            int rub_eur_b = 100;
            int rub_eur_c = 500;
            int rub_d = 1000;
            int rub_e = 2000;
            int rub_f = 5000;
            int usd_a = 1;
            int eur_a = 5;
            int eur_b = 20;
            int eur_c = 50;
            int eur_d = 200;

            switch (currency_code)            
            {
                case 643:
                    if ((summa % rub_f == 0) || (summa % rub_e == 0) || (summa % rub_d == 0) || (summa % rub_eur_c == 0) || (summa % rub_eur_b == 0) || (summa % rub_eur_a == 0))
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Сумма должны быть кратна 10 или 100 или 500 или 1000 или 2000 или 5000");
                    return false;
                }
                case 840:
                    {
                        if (summa % usd_a == 0) return true;
                        else
                        {
                            Console.WriteLine("Сумма должна быть кратна 1");
                            return false;
                        } 
                    }
                case 978:
                    {
                        if ((summa % eur_d == 0) || (summa % eur_c == 0) || (summa % eur_b == 0) || 
                            (summa % rub_eur_c == 0) || (summa % rub_eur_b == 0) || (summa % rub_eur_a == 0) || (summa % eur_a == 0))
                        {
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Сумма должна быть кратна 5 или 10 или 20 или 50 или 100 или 200 или 500");
                            return false;
                        }
                    }
                default:
                    {
                        Console.WriteLine("Валюта не поддерживается");
                        return false;
                    }
            }
        }
        public double check_client_balance(int card_number)  //Проверка остатка клиента
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            double ostatok = 0;
            conn.Open();
            string query = "select balance from client_balance where card_number = " + card_number + ";";
            MySqlCommand comm = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = comm.ExecuteReader())
            {
                while (reader.Read())
                {
                    ostatok = Convert.ToDouble(reader.GetValue(0));
                    ostatok = Math.Round(ostatok, 2);
                }
                reader.Close();
            }
            conn.Close();
            return ostatok;
        }
        public bool check_client_change_ostatok(double amount, int card_number)  //Проверка возможности снятия с остатка клиента
        {
            if ((check_client_balance(card_number) - amount) > 0)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Недостаточно средств на Вашем счёте!");
                return false;
            }
        }
    }
}
