using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SupportWheelOfFate.Core.Tests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SupportRotaTests
    {
        public class ThePlanShiftsMethod
        {
            private readonly SupportRota _unit = new SupportRota();
            
            private readonly List<SupportPerson> _supportPeople = new List<SupportPerson>()
            {
                new SupportPerson {Name = "1", Id = 1},
                new SupportPerson {Name = "2", Id = 2},
                new SupportPerson {Name = "3", Id = 3},
                new SupportPerson {Name = "4", Id = 4},
                new SupportPerson {Name = "5", Id = 5},
                new SupportPerson {Name = "6", Id = 6},
                new SupportPerson {Name = "7", Id = 7},
                new SupportPerson {Name = "8", Id = 8},
                new SupportPerson {Name = "9", Id = 9},
                new SupportPerson {Name = "10", Id = 10}
            };
            
            //TODO: Refactor into dedicated class
            #region test data generators

            private const uint TestRepetitions = 10;
            
            private static IEnumerable<object[]> GetRandomNumbers()
            {
                var random = new Random();
                for (var i = 0; i < TestRepetitions; i++)
                {
                    yield return new object[] { random.Next(1000) };
                }
            }
            
            private static IEnumerable<object[]> GetRandomFromsAndTos()
            {
                var random = new Random();
                for (var i = 0; i < TestRepetitions; i++)
                {
                    var from = new DateTime(2010, 10, 5).AddDays(random.Next(10000));
                    yield return new object[] { from, from.AddDays(random.Next(1000)) };
                }
            }
            #endregion
            
            [Theory]
            [MemberData(nameof(GetRandomNumbers))]
            public void WhenFromAfterTo_ThrowsArgumentException(int random)
            {
                var before = new DateTime(2010, 10, 5).AddDays(random);
                var after = before.AddDays(random + 1);
                
                Assert.Throws<ArgumentException>(() => _unit.PlanShifts(after, before, _supportPeople));
            }

            [Theory]
            [MemberData(nameof(GetRandomFromsAndTos))]
            public void WithNullListOfPeople_ThrowsArgumentNullException(DateTime from, DateTime to)
            {
                Assert.Throws<ArgumentNullException>(() => _unit.PlanShifts(from, to, null));
            }

            [Theory]
            [MemberData(nameof(GetRandomFromsAndTos))]
            public void WithNotEnoughPeople_ThrowsArgumentException(DateTime from, DateTime to)
            {
                var noPeople = new List<SupportPerson>();
                Assert.Throws<ArgumentException>(() => _unit.PlanShifts(from, to, noPeople));

                var onePerson = new List<SupportPerson> { new SupportPerson()};
                Assert.Throws<ArgumentException>(() => _unit.PlanShifts(from, to, onePerson));
            }

            [Theory]
            [MemberData(nameof(GetRandomFromsAndTos))]
            public void PlansTheRightNumberOfShifts(DateTime from, DateTime to)
            {
                _unit.PlanShifts(from, to, _supportPeople);
                Assert.Equal(GetCountOfWorkdaysInPeriod(from, to), _unit.ShiftPlan.Count);
            }

            //TODO: This is prime helper material
            private int GetCountOfWorkdaysInPeriod(DateTime from, DateTime to) =>
                Enumerable
                    .Range(0, (to - from).Days + 1)
                    .Select(day => from.AddDays(day))
                    .Count(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);

            [Theory]
            [MemberData(nameof(GetRandomFromsAndTos))]
            public void WhenSomeShiftsAreAlreadyPlanned_DoesntOverwriteThem(DateTime from, DateTime to)
            {
                var random = new Random();
                _unit.PlanShifts(from, to, _supportPeople);
                
                var firstShiftPlan = new Dictionary<DateTime, List<SupportPerson>>(_unit.ShiftPlan);
                _unit.PlanShifts(from, to.AddDays(random.Next(1000)), _supportPeople);
                
                foreach (var key in firstShiftPlan.Keys)
                {
                    Assert.Equal(firstShiftPlan[key], _unit.ShiftPlan[key]);
                }
            }

            [Theory]
            [MemberData(nameof(GetRandomFromsAndTos))]
            public void DistributesShiftsRandomly(DateTime from, DateTime to)
            {
                _unit.PlanShifts(from, to.AddDays(1000), _supportPeople);
                foreach (var person in _supportPeople)
                {
                    //TODO: This could be based on proper statistics
                    float countOfShiftsWithThatPerson = _unit.ShiftPlan.Values.Count(s => s.Contains(person));
                    var expectedCount = (float)SupportRota.PeoplePerShift / _supportPeople.Count * _unit.ShiftPlan.Count;
                    var deviation = Math.Abs(countOfShiftsWithThatPerson - expectedCount) / expectedCount;
                    Assert.True(deviation < 0.2);
                }
            }

            [Theory]
            [MemberData(nameof(GetRandomFromsAndTos))]
            public void PlansOnlyWorkDayShifts(DateTime from, DateTime to)
            {
                _unit.PlanShifts(from, to, _supportPeople);
                Assert.Empty(_unit.ShiftPlan.Keys.Where(s => 
                    s.Date.DayOfWeek == DayOfWeek.Saturday || 
                    s.Date.DayOfWeek == DayOfWeek.Sunday));
            }

            [Theory]
            [MemberData(nameof(GetRandomFromsAndTos))]
            public void NeverAssignsAnyoneMoreToThanOneShiftADay(DateTime from, DateTime to)
            {
                _unit.PlanShifts(from, to, _supportPeople);
                Assert.Empty(_unit.ShiftPlan.Values.Where(s =>
                    s.Distinct().Count() != s.Count
                ));
            }

            [Theory]
            [MemberData(nameof(GetRandomFromsAndTos))]
            public void NeverAssignsAnyoneToTwoShiftsInARow(DateTime from, DateTime to)
            {
                //ASSUMPTION: Friday and Monday are NOT two consecutive days
                _unit.PlanShifts(from, to, _supportPeople);
                Assert.False(CheckIfAnyoneIsAssignedTwoDaysInARow(_unit.ShiftPlan));
            }
            
            private static bool CheckIfAnyoneIsAssignedTwoDaysInARow(Dictionary<DateTime, List<SupportPerson>> shifts)
            {
                //TODO: This test is quite similiar to implementation. Should find another way to test this.
                foreach (var date in shifts.Keys)
                {
                    if (shifts.ContainsKey(date.AddDays(-1)) &&
                        shifts[date].Intersect(shifts[date.AddDays(-1)]).Any())
                        return true;
                }
                return false;
            }
            
            //TODO: Implement the last rule: Each engineer should have completed one whole day of support in any 2 week period.
        }
    }
}