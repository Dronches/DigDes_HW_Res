using System.Collections.Generic;
using System.ServiceModel;


namespace WCFClientWordCounter
{
    [ServiceContract]
    public interface IService
    {

        [OperationContract]
        Dictionary<string, int> getWordsFromString(string[] info);

    }

}
