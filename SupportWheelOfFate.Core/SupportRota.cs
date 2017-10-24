using System;
using System.Collections.Generic;
using System.Linq;

namespace SupportWheelOfFate.Core
{
    public class SupportRota
    {
        public const byte PeoplePerShift = 2;
        
        //TODO: Replacing DateTime with a pure Date struct could eliminate potential bugs
        //It may be beneficial to turn this into a SortedDictionary if there are more operations on time periods.
        public Dictionary<DateTime, List<SupportPerson>> ShiftPlan { get; }

        public SupportRota()
        {
            ShiftPlan = new Dictionary<DateTime, List<SupportPerson>>();    
        }
        
        /// <summary>
        /// Plans shifts in the given time period. Doesn't overwrite existing shifts.
        /// </summary>
        public void PlanShifts(DateTime from, DateTime to, List<SupportPerson> supportPeople, int randomSeed = 1)
        {
            from = from.Date;
            to = to.Date;
            
            if (from > to)
                throw new ArgumentException("To must be the same as or after from", "to");

            if (supportPeople == null)
                throw new ArgumentNullException("supportPeople");
            
            if (supportPeople.Count < PeoplePerShift)
                throw new ArgumentException(
                    $"At least {PeoplePerShift} support people must be provided", 
                    "supportPeople");
            
            var random = new Random(randomSeed);
            
            for (var day = from; day <= to; day = day.AddDays(1))
            {
                if (!IsWeekend(day) || ShiftPlan.ContainsKey(day))
                    continue;
                
                // Rule: No person should be assigned two days in a row
                var availablePeople = supportPeople.Except(GetYesterdaysShift(day));
                
                ShiftPlan.Add(day, GetTwoRandomDistinctPeople(availablePeople, random));
            }
        }
        
        //TODO: Move into helper class
        private static bool IsWeekend(DateTime day) => 
            day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday;

        private IEnumerable<SupportPerson> GetYesterdaysShift(DateTime day) =>
            ShiftPlan.ContainsKey(day.AddDays(-1)) ? ShiftPlan[day.AddDays(-1)] : new List<SupportPerson>();
        
        // Rule: No person should be assigned two shifts on the same day
        //TODO: This could be made quicker
        private List<SupportPerson> GetTwoRandomDistinctPeople(IEnumerable<SupportPerson> people, Random random) => 
             people
                .OrderBy(user => random.Next())
                .Take(PeoplePerShift).ToList()
                .ToList();
        
    }
}