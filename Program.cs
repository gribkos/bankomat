using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using BankomatM;
using MySql.Data.MySqlClient;



client clnt = new();
Checks chk = new();
Change_Summa chng_summa = new();
UI u = new();

int card = -1;
int pin = -1;
int vars = -1;
double amount;
UI object_ui = new();
string curr = "RUB";

while (true)
{
    vars = -1;
    card = u.ui_welcome();
    if (card != 0)
    {
        pin = u.ui_enter_pin(card);
        if (pin != 0)
        {
            while (vars != 0)
            {                
                Console.WriteLine(u.show_choose);
                Console.Write(u.show_vars);
                vars = int.Parse(Console.ReadLine());
                Console.Clear();
                switch (vars)
                {
                    case 1: //Снятие                        
                        int currency_code_ch = object_ui.ui_enter_currency_choice();
                        Console.Clear();
                        Console.Write(u.show_enter_amount);
                        amount = int.Parse(Console.ReadLine());
                        Console.Clear(); 
                        chng_summa.reduce_client_balance(amount, card, currency_code_ch);                        
                        amount = 0;
                        break;
                    case 2: //Пополнение
                        Console.Write(u.show_enter_amount);
                        amount = int.Parse(Console.ReadLine());
                        Console.Clear(); 
                        chng_summa.deposit_client_balance(amount, card);
                        amount = 0;
                        break;
                    case 3:
                        Console.Clear(); 
                        Console.WriteLine("\n\nВаш баланс: " + chk.check_client_balance(card) + " " + curr + "\n");
                        break;
                    case 0:
                        Console.Clear(); 
                        vars = 0; //Выход из цикла
                        break;
                    default:
                        Console.Clear(); 
                        Console.WriteLine(u.show_choose_warning);
                        break;
                }
            }
        }
    }
}

