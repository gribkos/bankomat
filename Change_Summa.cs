using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankomatM
{
    internal class Change_Summa
    {
        client clnt = new();
        Checks chk = new();        
        readonly MySqlConnection conn = DBUtils.GetDBConnection();

        public bool deposit_client_balance(double amount, int card_number)  //Клиент пополняет свой счёт
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            int operation = 2; //Пополнение
            int curr = 643;  //Пополнение только в рублях
            if (chk.check_kratnost(amount, curr))
            {
                double ostatok = chk.check_client_balance(card_number);
                double chng_ostatok;
                chng_ostatok = amount + ostatok;                
                conn.Open();
                string query = "update client_balance set balance = " + chng_ostatok + " where card_number = " + card_number + ";";
                string query_txn = "insert into txn (created_on, card_number, operation, init_amount, txn_rate, to_currency_code, amount_reated) values(sysdate(), " + card_number + ", " + operation + ", " + amount + ", " + 1 + ", " + curr + ", " + amount + " );";
                MySqlCommand comm = new MySqlCommand(query, conn);
                MySqlCommand txn_comm = new MySqlCommand(query_txn, conn);
                comm.ExecuteNonQuery(); 
                if ((chk.check_client_balance(card_number) - amount) == ostatok)
                {
                    txn_comm.ExecuteNonQuery();
                    conn.Close();
                    Console.WriteLine("Операция успешна");
                    return true;
                }
                else
                {
                    conn.Close();
                    Console.WriteLine("Ошибка");
                    return false;
                }                
            }
            else
            {
                conn.Close();
                return false;
            }
        }
        public bool reduce_client_balance(double amount, int card_number, int choice)  //Клиент снимает деньги
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US"); //-----------------*-----------------
            int currency_code = 0;
            int currency_code_ch = choice;
            int operation = 1;  //Снятие
            //1 - RUB;  2 - USD;  3 - EUR;  0 - Выход            
            switch (currency_code_ch)
            {
                case 1:
                    {
                        currency_code = 643;
                        if (chk.check_kratnost(amount, currency_code))
                        {
                            if (chk.check_client_change_ostatok(amount, card_number))
                            {
                                double client_ostatok = chk.check_client_balance(card_number);
                                if (chk.check_bankomat_change_ostatok(amount, currency_code))
                                {
                                    double bank_ostatok = chk.check_bankomat_ostatok(currency_code);                                    
                                    conn.Open();
                                    string query_client = "update client_balance set balance = " + (client_ostatok - amount) + " where card_number = " + card_number + ";";
                                    string query_bankomat = "update bankomat_balance set summa = " + (bank_ostatok - amount) + " where currency_code = " + currency_code + ";";
                                    string query_txn = "insert into txn (created_on, card_number, operation, init_amount, txn_rate, to_currency_code, amount_reated) values(sysdate(), " + card_number + ", " +operation+ ", " + amount + ", " + 1 + ", " + currency_code + ", " + amount + " );";
                                    MySqlCommand client_comm = new MySqlCommand(query_client, conn);
                                    MySqlCommand bankomat_comm = new MySqlCommand(query_bankomat, conn);
                                    MySqlCommand txn_comm = new MySqlCommand(query_txn, conn);
                                    client_comm.ExecuteNonQuery(); 
                                    bankomat_comm.ExecuteNonQuery(); 
                                    if (((chk.check_client_balance(card_number) + amount) == client_ostatok) && ((chk.check_bankomat_ostatok(currency_code) + amount) == bank_ostatok))
                                    {
                                        txn_comm.ExecuteNonQuery();
                                        conn.Close();
                                        Console.WriteLine("Операция успешна");
                                        return true;
                                    }
                                    else
                                    {
                                        conn.Close();
                                        Console.WriteLine("Ошибка");
                                        return false;
                                    }                                    
                                }
                                else
                                {
                                    conn.Close();
                                    return false;
                                }
                            }
                            else
                            {
                                conn.Close();
                                return false;
                            }
                        }
                        else
                        {
                            conn.Close();
                            return false;
                        }
                    }
                case 2:
                    {
                        currency_code = 840;
                        if (chk.check_kratnost(amount, currency_code))
                        {
                            if (chk.check_client_change_ostatok(amount, card_number))
                            {
                                double client_ostatok = chk.check_client_balance(card_number);
                                double clietn_ostatok_before = client_ostatok;
                                if (chk.check_bankomat_change_ostatok(amount, currency_code))
                                {
                                    double bank_ostatok = chk.check_bankomat_ostatok(currency_code);
                                    double txn_rate = get_rate(amount, currency_code, 643);
                                    double amount_reated = amount / txn_rate;  //Всегда из валюты RUB
                                    amount_reated = Math.Round(amount_reated, 2, MidpointRounding.ToPositiveInfinity);                                    
                                    conn.Open();
                                    string query_client = "update client_balance set balance = " + (client_ostatok - amount_reated) + " where card_number = " + card_number + ";";
                                    string query_bankomat = "update bankomat_balance set summa = " + (bank_ostatok - amount) + " where currency_code = " + currency_code + ";";
                                    string query_txn = "insert into txn (created_on, card_number, operation, init_amount, txn_rate, to_currency_code, amount_reated) values(sysdate(), " + card_number +", "+ operation+ ", "+ amount +", "+ txn_rate + ", "+ currency_code +", "+ amount_reated + " );";
                                    MySqlCommand client_comm = new MySqlCommand(query_client, conn);
                                    MySqlCommand bankomat_comm = new MySqlCommand(query_bankomat, conn);
                                    MySqlCommand txn_comm = new MySqlCommand(query_txn, conn);
                                    client_comm.ExecuteNonQuery(); 
                                    bankomat_comm.ExecuteNonQuery(); 
                                    if (txn_rate == txn_rate)
                                    {
                                        txn_comm.ExecuteNonQuery();
                                        conn.Close();
                                        Console.WriteLine("Операция успешна");
                                        return true;
                                    }
                                    else
                                    {
                                        conn.Close();
                                        Console.WriteLine("Ошибка");
                                        return false;
                                    }                                    
                                }
                                else
                                {
                                    conn.Close();
                                    return false;
                                }
                            }
                            else
                            {
                                conn.Close();
                                return false;
                            }
                        }
                        else
                        {
                            conn.Close();
                            return false;
                        }
                    }
                case 3:
                    {
                        currency_code = 978;
                        if (chk.check_kratnost(amount, currency_code))
                        {
                            if (chk.check_client_change_ostatok(amount, card_number))
                            {
                                double client_ostatok = chk.check_client_balance(card_number);
                                double clietn_ostatok_before = client_ostatok;
                                if (chk.check_bankomat_change_ostatok(amount, currency_code))
                                {
                                    double bank_ostatok = chk.check_bankomat_ostatok(currency_code);
                                    double txn_rate = get_rate(amount, currency_code, 643);
                                    double amount_reated = amount / txn_rate;  //Всегда из валюты RUB
                                    amount_reated = Math.Round(amount_reated, 2, MidpointRounding.ToPositiveInfinity);
                                    conn.Open();
                                    string query_client = "update client_balance set balance = " + (client_ostatok - amount_reated) + " where card_number = " + card_number + ";";
                                    string query_bankomat = "update bankomat_balance set summa = " + (bank_ostatok - amount) + " where currency_code = " + currency_code + ";";
                                    string query_txn = "insert into txn (created_on, card_number, operation, init_amount, txn_rate, to_currency_code, amount_reated) values(sysdate(), " + card_number + ", " + operation + ", " + amount + ", " + txn_rate + ", " + currency_code + ", " + amount_reated + " );";
                                    MySqlCommand client_comm = new MySqlCommand(query_client, conn);
                                    MySqlCommand bankomat_comm = new MySqlCommand(query_bankomat, conn);
                                    MySqlCommand txn_comm = new MySqlCommand(query_txn, conn);
                                    client_comm.ExecuteNonQuery(); 
                                    bankomat_comm.ExecuteNonQuery(); 
                                    if (txn_rate==txn_rate)
                                    {
                                        txn_comm.ExecuteNonQuery();
                                        conn.Close();
                                        Console.WriteLine("Операция успешна");
                                        return true;
                                    }
                                    else
                                    {
                                        conn.Close();
                                        Console.WriteLine("Ошибка");
                                        return false;
                                    }                                    
                                }
                                else
                                {
                                    conn.Close();
                                    return false;
                                }
                            }
                            else
                            {
                                conn.Close();
                                return false;
                            }
                        }
                        else
                        {
                            conn.Close();
                            return false;
                        }
                    }
                case 0:
                    return true;                    
                default:
                    return false;
            }
        }
        public double get_rate(double amount, int from_curr_code, int to_curr_code) //Получение курса
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            double summa = amount;            
            double rate = -1;            
            string query = "select rate, id from rates where from_curr = " + from_curr_code + " and to_curr = " + to_curr_code + ";";
            conn.Open();
            MySqlCommand comm = new MySqlCommand(query, conn);
            MySqlDataReader reader = comm.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {                  
                rate = Convert.ToDouble(reader.GetValue(0));               
                reader.Close();
                conn.Close();
                return rate;
            }
            else
            {
                reader.Close();
                conn.Close();
                rate = -1;
                Console.WriteLine("Курс не установлен!");
                return rate;
            }
        }
        public int get_rate_id(int from_curr_code, int to_curr_code) //Получение курса
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            int rate_id;            
            //int id = 0;
            string query = "select id from rates where from_curr = " + from_curr_code + " and to_curr = " + to_curr_code + " order by id;";
            conn.Open();
            MySqlCommand comm = new MySqlCommand(query, conn);
            MySqlDataReader reader = comm.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {                
                rate_id = Convert.ToInt32(reader.GetValue(1));                
                reader.Close();
                conn.Close();
                return rate_id;
            }
            else
            {
                reader.Close();
                conn.Close();
                rate_id = -1;
                Console.WriteLine("Курс не установлен!");
                return rate_id;
            }
        }
    }
}
