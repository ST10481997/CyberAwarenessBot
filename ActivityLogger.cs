using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace CyberAwarenessBot
{
    
    /// this class manages the activity logging for the HAPPYCODER chatbot
    
    
    public class ActivityLogger
    {

        private List<ActivityEntry> activities = new List<ActivityEntry>();
        private int maxEntries = 60;
        private int displayEntries = 20;

        //when a new activity is logged

        public event EventHandler<ActivityEventArgs> ActivityLogged;
        public int TotalActivities => activities.Count;
        public int DisplayEntries => displayEntries;

        

        public void logActivity(string action, string details, string category)
        {
            var entry = new ActivityEntry
            {
                Id = activities.Count + 1,
                Timestamp = DateTime.Now,
                Action = action,
                Details = details,
                Category = category
            };

            activities.Add(entry);

            //keep the last max entries
            if(activities.Count > maxEntries)
            {
                activities.RemoveAt(0);
                
                //re-index ids
                for(int i=0; i<activities.Count; i++) 
                {
                    activities[i].Id = i + 1;
                }
            }

            OnActivityLogged(new ActivityEventArgs { Entry = entry });
        }

        /// <summary>
        /// get all the activities 
        /// </summary>
        /// <returns></returns>
        public List<ActivityEntry> getALlActivities()
        {
            return new List<ActivityEntry>(activities);
        }

        ///get all the last activited
       public List<ActivityEntry> getRecentActivities (int count = 20)
        {
            if(activities.Count == 0)
            {
                return new List<ActivityEntry>();
            }

            int startIndex = Math.Max(0, activities.Count - count);
            return activities.Skip(startIndex).Take(count).ToList();
        }

        //get the activities by their category
        public List<ActivityEntry> getActivitiesbyCategory(string category)
        {
            return activities.Where(a=>a.Category == category).ToList();  

        }


        /// <summary>
        /// Clears all activities
        /// </summary>
        public void Clear()
        {
            activities.Clear();
            logActivity("Log Cleared", "Activity log was cleared", "System");
        }

        /// <summary>
        /// Gets a formatted summary of recent activities
        /// </summary>
        public string GetFormattedSummary(int count = 10)
        {
            if (activities.Count == 0)
                return "No activities have been logged yet.";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========== ACTIVITY LOG ==========");

            var recent = getRecentActivities(count);
            foreach (var entry in recent)
            {
                sb.AppendLine($"  [{entry.Timestamp.ToString("HH:mm:ss")}] {entry.Category}: {entry.Action}");
                if (!string.IsNullOrEmpty(entry.Details))
                {
                    sb.AppendLine($"    Details: {entry.Details}");
                }
            }

            sb.AppendLine($"\nShowing last {recent.Count} actions.");
            sb.AppendLine($"Total activities: {activities.Count}");

            return sb.ToString();
        }

        
        /// Gets a summary of statistics
        
        public ActivityStatistics GetStatistics()
        {
            var stats = new ActivityStatistics
            {
                TotalActivities = activities.Count,
                Categories = activities.GroupBy(a => a.Category)
                                       .ToDictionary(g => g.Key, g => g.Count())
            };

            if (activities.Count > 0)
            {
                stats.firstActivity = activities.First().Timestamp;
                stats.lastActivity = activities.Last().Timestamp;
                stats.averageActivityPerDay = CalculateAveragePerDay();
            }

            return stats;
        }

        /// <summary>
        /// Calculates the average number of activities per day
        /// </summary>
        private double CalculateAveragePerDay()
        {
            if (activities.Count < 2)
                return activities.Count;

            var first = activities.First().Timestamp;
            var last = activities.Last().Timestamp;
            var days = (last - first).TotalDays;

            if (days < 1)
                return activities.Count;

            return activities.Count / days;
        }

        // Event raising method
        protected virtual void OnActivityLogged(ActivityEventArgs e)
        {
            ActivityLogged?.Invoke(this, e);
        }

    }

    //log a new activitiy
    


    /// <summary>
    /// activity category constants
    /// </summary>
    public class ActivityCategories
    {
        public const string Task = "Task";
        public const string Quiz = "Quiz";
        public const string NLP = "NLP";
        public const string System = "System";
        public const string Chat = "Chat";
        public const string User = "User";
        public const string Error = "Error";
        public const string Reminder = "Reminder";
        public const string General = "General";
        public const string Login = "Login";

        
    }


    /// <summary>
    /// statistics
    /// </summary>
    public class ActivityStatistics
    {
        public int TotalActivities {  get; set; }
        public Dictionary<string, int> Categories { get; set; }
        public DateTime firstActivity { get; set; }
        public DateTime lastActivity { get; set; }
        public double averageActivityPerDay { get; set; }
    }


    /// <summary>
    /// A single activity log entry
    /// </summary>
    public class ActivityEntry
    {
        public int Id {  get; set; }
        public DateTime Timestamp {  get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public string Category { get; set; }
    }

    /// <summary>
    /// arguments for activity logging 
    /// </summary>
    public class ActivityEventArgs : EventArgs
    {
        public ActivityEntry Entry { get; set; }

    }


    






}