# Support Wheel of Fate


First and foremost, I've made some assumptions. Normally I would ask first, but it was weekend... They are as follows:
- It's good to know support shifts in advance, to enable planning around them.
- Monday-Friday work week.
- Friday and Monday are not considered consecutive days.


Front end employs jQuery and Vue to obtain data through the API and render with front-end templates. Saves time if API is already a requirement, and forms a good base for mobile clients (because API exposes everything web app needs to know, so most likely covers most mobile needs as well). Other than that, it's rudimentary. If I had time to afford, using a framework would yield some easy wins (mobile friendliness among them).

Mid tier is MVC (with very basic View). I've picked .NET Core because it's fairly quick to deliver website and API in one project - monolith is a good starting point. I had to cut some corners: no viewmodels, as there is full overlap between domain and API interface for now.

DAL operates on json files included with the project. It's quick and simple, and performance is not an issue here. Of course, it's an awful idea for production app, unless you don't mind losing data stored with app files.

Design wise, I've spent most time figuring out how to reconciliate scheduling ahead with everyday life. Engineer taking a day off, employees being fired or hired all affect the schedule. The simplest usable solution I went with is to keep the schedule reasonably short and rely on employees switching their shifts between them. If this project received more attention, here are possible extension points I accounted for in the design:
- Schedule generator can easily consider employee availability when randomising shifts. Simple IsAvailable property on SupportPerson object can be populated from front end or HR system.
- It's very easy to modify the generator to overwrite the existing schedule with minimal changes to shifts. Implementing this would permit scheduling far ahead (which may be unnecessary).
- It should be fairly easy to extract business rules into their own classes, if they are to be modified or more are added.


I think it's the best thing I could build given the nature of the problem and short time frame.
