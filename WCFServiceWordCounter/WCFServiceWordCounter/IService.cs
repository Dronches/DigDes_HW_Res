using System.Collections.Generic;
using System.ServiceModel;


namespace WCFServiceWordCounter
{
    /// <summary>
    /// Интерфейс сервиса
    /// </summary>
    [ServiceContract]
    public interface IService
    {
        /// <summary>
        /// Рефлексивно получаем из либы метод
        /// Обрабатываем массив строк
        /// Возвращаем список (результат обработки) 
        /// Записываем список по результирующему пути в удобночитаемом виде
        /// </summary>
        /// <param name="info">данные, передаваемые в функцию</param>
        [OperationContract]
        Dictionary<string, int> getWordsFromString(string[] info);
    }
}
