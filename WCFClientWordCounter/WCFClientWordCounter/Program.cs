using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.ServiceModel;

namespace WCFClientWordCounter
{

    class Program
    {
        /// <summary>
        /// сетевое расположение развертываемого сервиса (в локальной сети) с конечной точкой
        /// </summary>
        private static string ipAddressService = "localhost";
        private static string portService = "5056";
        private static string nameService = "WordCounter";
        /// <summary>
        /// 100 мб передается и принимается в среднем за 5 секунд, а с учетом выполнения за 7 секунд (т.е. 2 секунды выполнения)
        /// однако если будет множество колизий, то время выполнения может сильно возрасти
        /// для среднего случая достаточно около минуты (т.е. default time)
        /// но если для dictionary все слова уникальны, то время возврата клиенту будет совпадать с временем отправки
        /// Допустим, что 5 секунд занимает время отпраки (на самом деле около 4)
        /// тогда худшее время возврата будет больше 5 секунд (т.к. передется словарь из string и int), а не массив строк
        /// будем считать среднее увеличение времени возврата за счет использования словаря и уникальных значений примерно в 2 раза большим
        /// хоть это и не достаточно точно
        /// время выполнения с 2 секунд увеличим до 5 (будем считать, что часто возникают коллизии)
        /// достаточное время отправки: 5*10 = 50 сек 
        /// достаточное общее время (5+2*5+5)*10 -> 4 минуты (с округлением в большую сторону)
        /// соответственно заявленное время удовлетворяет - однако стоит учитывать характеристики компьютера
        /// поэтому время выберем хотя бы в 2 раза большее время (для времени полче
        /// Однако стоит понимать, что часть этого 
        /// </summary>
        private static TimeSpan timeConnection = new TimeSpan(0, 2, 0);
        /// <summary>
        /// объемные характеристики обработки сообщения - максимум 1 ГБ
        /// </summary>
        private static int maxBytes = 1024 * 1024 * 1024;

        static void Main(string[] args)
        {

            // вывод информации о программе
            Console.Write(PathParser.info);
            // считываем путь
            Console.Write("Введите путь к файлу: ");
            string path = Console.ReadLine();

            // получаем данные
            string[] info = FileEditor.GetInfoFromPath(path);
            // выходим, если не получили данные (ошибки записаны внутри)
            if (info == null)
                return;

            string uri = $"net.tcp://{ipAddressService}:{portService}/{nameService}"; // записываем локальный домен (протокол tcp)
            var binding = new NetTcpBinding(SecurityMode.Transport); // создаем бинд с защитой на транспортном уровне

            // изменение характеристик соединения
            binding.OpenTimeout = timeConnection; // default = 1 min 
            binding.CloseTimeout = timeConnection; // default = 1 min
            binding.SendTimeout = timeConnection; // default = 1 min
            //binding.ReceiveTimeout // default = 10 min

            binding.MaxReceivedMessageSize = maxBytes; // увеличиваем количество принимаемых байт
            binding.MaxBufferSize = maxBytes; // увеличиваем количество байт буфера
            
            var channel = new ChannelFactory<IService>(binding); // создаем канал для отправки сообщений
            var endpoint = new EndpointAddress(uri); // записываем адрес для взаимодействия с конечной точкой службы
            var proxy = channel.CreateChannel(endpoint); // открываем канал взаимодействия с интерфейсом серфиса
            try
            {
                //Stopwatch w = new Stopwatch();
                //w.Start();
                Console.WriteLine("Происходит взаимодействие с сервисом...");
                var result = proxy?.getWordsFromString(info);
                //w.Stop();
                //Console.WriteLine(w.ElapsedMilliseconds); // время выполнения в мс для debug
                if (result == null)
                    Console.WriteLine("Произошла ошибка на стороне сервиса...");
                else
                    FileEditor.PrintToFile(path, PathParser.resultName, result as Dictionary<string, int>);
            }
            catch
            {
                Console.WriteLine("Не удалось получить результат от сервиса...");
            }
        }
    }
}
