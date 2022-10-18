using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.ServiceModel;
using WordCounterLib;

namespace WCFServiceWordCounter
{
    /// <summary>
    /// Класс, реализующий интерфейс сервиса
    /// Делаем невозможным осуществление одновременных действий по нескольким клиентам (иначе придется пересчитывать время) - 1 экземпляр
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : IService
    {
        private static string nameFunc = "ParseInfoToDictionaryParallel";
        /// <summary>
        /// Рефлексивно получаем из либы метод
        /// Обрабатываем массив строк
        /// Возвращаем список (результат обработки) 
        /// Записываем список по результирующему пути в удобночитаемом виде
        /// </summary>
        /// <param name="info">данные, передаваемые в функцию</param>
        public Dictionary<string, int> getWordsFromString(string[] info)
        {
            Console.WriteLine("Данные получены, происходит их обработка...");
            // получаем нужный нам метод
            var searchMethod = typeof(LineParser).GetMethod(nameFunc, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            // если полученный объект метода пустой 
            if (searchMethod is null)
            {
                Console.WriteLine("Ошибка. Не удалось получить объект библиотеки, проверьте наличие библиотек...");
                return null;
            }
            else
            {
                // Создаем объект класса (он доступен в стандартном варианте, в отличие от метода)
                var lineParser = new LineParser();
                //Stopwatch w = new Stopwatch();
                //w.Start();
                // выполняем извлеченный метод - передаем данные
                var objDictionary = searchMethod?.Invoke(lineParser, parameters: new object[] { info }); // parameters - поясняющая мнемоника, может быть опущена
                //w.Stop();
                //Console.WriteLine(w.ElapsedMilliseconds); // время выполнения в мс для debug       

                // если объект пустой 
                if (!(objDictionary is null))
                    if (objDictionary.GetType() == typeof(Dictionary<string, int>))
                    {
                        Console.WriteLine("Обработка данных была успешно выполнена!");
                        return objDictionary as Dictionary<string, int>;
                    }
                    // если объект не пустой -> не тот тип
                    else
                        Console.WriteLine("Не удалось распознать тип результата - системная ошибка...");
                // иначе объект пустой - будет выведена ошибка, втроенная в lib
                return null;
            }
        }
    }
}
