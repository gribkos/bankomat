using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BankomatM
{
    internal class UI 
    {
        int card_number = 0;
        int client_pin = 0;
        string pin_code_string = " ";
        string card_number_string = " ";

        Checks chk = new();
        client cl = new();

        private static string welcome_message = "\t\tДобро пожаловать\n\t\t в мой банкомат!\n------------------------------------------------";
        private static string hello_message = "\t\nЗдравствуйте ";
        private static string end_message = "\tДо свидания!\n";
        private static string enter_card = "Введите номер Вашей карты: ";
        private static string enter_pin = "Введите пин-код Вашей карты: ";
        private static string enter_amount = "Введите сумму: ";
        private static string choose = "Возможные операции: ";
        private static string choose_warning = "Необходимо выбрать номер необходимой Вам операции! ";
        private static string razdelitel = "------------------------------------------------";
        private static string vars = " 1 - Снятие наличных; \n 2 - Пополнение счёта; \n 3 - Запрос баланса; \n 0 - Выход \n\n Введите номер нужной операции: ";
        private static string vars_currency = "Выберитю валюту снятия:\n 1 - RUB; \n 2 - USD; \n 3 - EUR; \n 0 - Выход \n\n Введите номер нужной операции: ";



        public int ui_welcome() //Приглашение и ввод карты
        {            
            Console.WriteLine(show_razdelitel); 
            Console.WriteLine(welcome_message);
            Console.Write(enter_card);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);


                if (key.Key != ConsoleKey.Backspace)
                {
                    card_number_string += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
                else
                {
                    if ((card_number_string.Length - 1) != -1)
                    {
                        card_number_string = card_number_string.Remove(card_number_string.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            } 
            while (key.Key != ConsoleKey.Enter);    // Прекратить когда Enter          
            if ((card_number_string != " \r") || (card_number_string != "\r"))
            {
                card_number = int.Parse(card_number_string);
                card_number_string = " ";                
                if (chk.check_card_number(card_number))
                {
                    Console.Clear();
                    return card_number;
                }
                else
                {                    
                    return 0;
                }
            }
            else
            {
                Console.WriteLine("\n\nНужно ввести номер Вашей карты!\n");
                return 0;
            }            
        }
        public int ui_enter_pin(int card_number) //Ввод пин-кода
        {
            Console.Write(enter_pin); 
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                  
                if (key.Key != ConsoleKey.Backspace)
                {
                    pin_code_string += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if ((pin_code_string.Length - 1) != -1)
                    {
                        pin_code_string = pin_code_string.Remove(pin_code_string.Length - 1);
                        Console.Write("\b \b");
                    }
                }                
            } // Прекратить когда Enter  
            while (key.Key != ConsoleKey.Enter);
            if ((pin_code_string != " \r") || (pin_code_string != "\r"))
            {
                client_pin = int.Parse(pin_code_string);
                pin_code_string = " ";
                //client_pin = int.Parse(Console.ReadLine());
                if (chk.check_pin(card_number, client_pin))
                {
                    if (chk.check_client_status(card_number, client_pin))
                    {
                        Console.Clear();
                        Console.WriteLine(show_hello_message + cl.get_client_name(card_number, client_pin) + " \n");
                        return client_pin;
                    }
                    else return 0;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                Console.WriteLine("\n\nНужно ввести пин-код!\n");
                return 0;
            }
        }
        public int ui_enter_currency_choice()  //Выбор валюты
        {
            int var_currency_choice = -1;
            string choice = " ";
            Console.Write(vars_currency);
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);


                if (key.Key != ConsoleKey.Backspace)
                {
                    choice += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
                else
                {
                    if ((choice.Length - 1) != -1)
                    {
                        choice = choice.Remove(choice.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            } // Прекратить когда Enter  
            while (key.Key != ConsoleKey.Enter);
            if ((choice != " \r") || (choice != "\r"))
            {
                var_currency_choice = int.Parse(choice);
                choice = " ";
                return var_currency_choice;
            }
            else
            {
                Console.Write("Необходимо выбрать необходимый Вам код валюты или код возврата");
                var_currency_choice = -1;
                return var_currency_choice;
            }
        }

        public string show_welcome_message
        {
            get { return welcome_message; }            
        }
        public string show_end_message
        {
            get { return end_message; }
        }
        public string show_enter_amount
        {
            get { return enter_amount; }
        }
        public string show_choose
        {
            get { return choose; }            
        }
        public string show_razdelitel
        {
            get { return razdelitel; }            
        }
        public string show_vars
        {
            get { return vars; }
        }
        public string show_choose_warning
        {
            get { return choose_warning; }
        }
        public string show_hello_message
        {
            get { return hello_message; }
        }
        public string show_vars_currency
        {
            get { return vars_currency; }
        }
    }
}
