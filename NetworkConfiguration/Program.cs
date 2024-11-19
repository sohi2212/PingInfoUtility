using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;


namespace NetworkConfiguration
{
    class Programm
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("===================================");
            Console.WriteLine("          Добро пожаловать         ");
            Console.WriteLine("===================================");
            Console.WriteLine();         
            Console.WriteLine("  1.Показать информацию по сетевым интерфейсам");
            Console.WriteLine("  2.Произвести Ping до определенного адреса");
            Console.Write("  Выберите действие: ");
            string choise = Console.ReadLine();

            switch (choise)
            {
                case "1": ShowNetworkInfo(); return;
                case "2":

                    IPAddress globalIP;
                    Console.Write("  Введите IP адресс для ping: ");

                    while (true)
                    {
                        string input = Console.ReadLine();
                        if (IPAddress.TryParse(input, out IPAddress ipAddress))
                        {
                            globalIP = ipAddress;
                            break;
                        }
                        else
                        {
                            Console.Write("  Неверный формат IP адреса, повторите попытку: ");
                        }
                    } 
                    Console.Write("  Введите кол-во запросов: ");
                    int attempts = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine();
                    Console.WriteLine("===================================");
                    Console.Clear();
                        for (int i = 1; i < attempts; i++) // Костыльный цикл для пинга
                        {
                            PingMethod(globalIP);
                        }   
                return;
                default: Console.WriteLine("Нет такого пункта!"); Console.ReadKey(); break;

            }
        }

        public static void ShowNetworkInfo() // Метод для вывода информации по интерфейсам
        {
            IPGlobalProperties pcProperties = IPGlobalProperties.GetIPGlobalProperties(); // метод для получения конфигурации сети на пк(имя и домен в данном случае)
            NetworkInterface[] _interface = NetworkInterface.GetAllNetworkInterfaces(); // получаю информацию по сетевым интерфейсам
            Console.WriteLine(pcProperties.HostName); //Вывожу название имя пк в сети


            if (_interface == null || _interface.Length == 0)
            {
                Console.WriteLine("Не найдено сетевых интерфейсов на {0}", pcProperties.HostName);
                return;
            }

            Console.WriteLine("Кол-во интерфейсов.............. : {0}", _interface.Length); // Вывожу общее кол-во интерфейсов

            foreach (NetworkInterface networkInterface in _interface)
            {
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                Console.WriteLine(); //засчет этого отступа сетевые интерфесы не смешаються в кашу 
                Console.WriteLine(networkInterface.Description); // Выводим все сетевые интерфейсы
                Console.WriteLine(String.Empty.PadLeft(networkInterface.Description.Length, '=')); // Красивое оформление списка интерфейсов!!!Нужно запомнить!!!
                Console.WriteLine("  Тип сетевого интерфейса ............... : {0}", networkInterface.NetworkInterfaceType);
                Console.WriteLine("  MAC-адрес интерфейса................... : {0}", networkInterface.GetPhysicalAddress());
                Console.WriteLine("  Opertional status...................... : {0}", networkInterface.OperationalStatus);

                string versions = "";

                if (networkInterface.Supports(NetworkInterfaceComponent.IPv4)) // Проверка поддерживает ли интерфейс IPv4
                {
                    versions = "IPv4"; // Если да то задаем version IPv4 для дальнейшего вывода его в терминале
                }
                if (networkInterface.Supports(NetworkInterfaceComponent.IPv6)) // Та же проверка с IPv6
                {
                    if (versions.Length > 0)// Если в version уже есть значение то пишем IPv6 через запятую и пробел
                    {
                        versions += ", ";
                    }
                    versions += "IPv6";
                }
                Console.WriteLine("  IP версия.............................. : {0}", versions);

            }
        }

        public static void PingMethod(IPAddress ip)
        {
            Ping pingProcces = new Ping(); // объект для отправки icmp
            PingOptions pingOptions = new PingOptions(); // объект для настройки опций ping

            pingOptions.DontFragment = true; // Меняем значение фрагментации, TTL исходя из документации остаёться 128


            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";// Буфер на 32 байта для отправки
            byte[] buffer = Encoding.ASCII.GetBytes(data); // переводим нашу data в байты
            int timeout = 120; // таймаут ставит на 120
            
                PingReply pingReply = pingProcces.Send(ip, timeout, buffer, pingOptions); // Запускаем пинги

                if (pingReply.Status == IPStatus.Success) // проверка успешен ли ответ
                {
                    Console.WriteLine();
                    Console.WriteLine("===========================================");
                    Console.WriteLine("IP адрес: {0}", pingReply.Address.ToString());
                    Console.WriteLine("Время ответа: {0}", pingReply.RoundtripTime);
                    Console.WriteLine("TTL: {0}", pingReply.Options.Ttl);
                    Console.WriteLine("Фрагментация: {0}", pingReply.Options.DontFragment);
                    Console.WriteLine("Размер буфера: {0}", pingReply.Buffer.Length);
                }
                else
                {
                    Console.WriteLine("Данных хост не отвечает!");
                }
           
           

        }


    }
}
