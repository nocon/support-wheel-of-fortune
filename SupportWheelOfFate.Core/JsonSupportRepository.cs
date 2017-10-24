using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SupportWheelOfFate.Core
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class JsonSupportRepository : ISupportRepository
    {
        public SupportRota GetRota()
        {
            return JsonConvert.DeserializeObject<SupportRota>(File.ReadAllText(@"Data/supportRota.json"));
        }

        public void UpdateRota(SupportRota rota)
        {
            File.WriteAllText(@"Data/supportRota.json", JsonConvert.SerializeObject(rota));
        }

        public IEnumerable<SupportPerson> GetPeople()
        {
            return JsonConvert.DeserializeObject<List<SupportPerson>>(File.ReadAllText(@"Data/supportPeople.json"));
        }
    }
}