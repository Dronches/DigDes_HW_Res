using System;
using System.ServiceModel;

namespace WCFServiceWordCounter
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
        /// Допустим, что 5 секунд занимает время отпраки от клиента (на самом деле около 4)
        /// тогда худшее время возврата будет больше 5 секунд (т.к. передется словарь из string и int), а не массив строк
        /// будем считать среднее увеличение времени возврата клиенту за счет использования словаря и уникальных значений примерно в 2 раза большим
        /// хоть это и не достаточно точно
        /// время выполнения с 2 секунд увеличим до 5 (будем считать, что часто возникают коллизии)
        /// достаточное время отправки: 2*5*10 -> 2 мин 
        /// достаточное общее время (5+2*5+5)*10 -> 4 минуты (с округлением в большую сторону)
        /// соответственно заявленное время удовлетворяет - однако стоит учитывать характеристики компьютера
        /// поэтому время выберем хотя бы в 2 раза большее время (для времени полче
        /// Однако стоит понимать, что часть этого 
        /// </summary>
        private static TimeSpan timeConnection = new TimeSpan(0, 4, 0);
        /// <summary>
        /// объемные характеристики обработки сообщения - максимум 1 ГБ
        /// </summary>
        private static int maxBytes = 1024 * 1024 * 1024;

        static void Main(string[] args)
        {
            Console.WriteLine("Происходит запуск сервера...");
            try
            {
                var uris = new Uri[1];
                uris[0] = new Uri($"net.tcp://{ipAddressService}:{portService}");
                IService wordCounterService = new Service();
                ServiceHost host = new ServiceHost(wordCounterService, uris); // создание хоста с привязкой к сетевому адресу и релизации интерфейса команд
                var binding = new NetTcpBinding(SecurityMode.Transport); // создаем бинд с защитой на транспортном уровне
                // изменение характеристик соединения
                binding.OpenTimeout = timeConnection; // default = 1 min 
                binding.CloseTimeout = timeConnection; // default = 1 min
                binding.SendTimeout = timeConnection; // default = 1 min
                //binding.ReceiveTimeout // default = 10 min

                binding.MaxReceivedMessageSize = maxBytes; // увеличиваем количество принимаемых байт
                binding.MaxBufferSize = maxBytes; // увеличиваем количество байт буфера

                host.AddServiceEndpoint(typeof(IService), binding, nameService); // конечная точка сетевого расположения ервиса
                host.Opened += OpenService; // добавляем функцию, выполняющуюся при запуске
                host.Open(); 
            }
            catch
            {
                Console.WriteLine("Возникла ошибка запуска сервиса!");
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Хэндлер на запуск сервера - функция, выполняющаяся при запуске сервиса
        /// </summary>
        private static void OpenService(object sender, EventArgs e)
        {
            Console.WriteLine("Сервер запущен!");
        }
    }
}
