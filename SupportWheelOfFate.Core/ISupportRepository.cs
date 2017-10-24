using System.Collections.Generic;

namespace SupportWheelOfFate.Core
{
    public interface ISupportRepository
    {
        SupportRota GetRota();

        void UpdateRota(SupportRota rota);

        IEnumerable<SupportPerson> GetPeople();
    }
}