using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.IO.Packaging;


namespace CyberAwarenessBot
{

    /// <summary>
    /// this cclas simulates language processin for the chatbot (HAPPYCODER)
    /// </summary>
    public class NLPSimulator
    {
        private Dictionary<string, string[]> intentionPatterns = new Dictionary<string, string[]>()
        {
            //Intention patterns wuith associated words
            { "add_task", new string[] { "add task", "create task", "new task", "add a task", "task add", "create a task" } },
            { "set_reminder", new string[] { "set reminder", "remind me", "add reminder", "create reminder", "remind in", "remind on" } },
            { "show_tasks", new string[] { "show tasks", "view tasks", "list tasks", "my tasks", "display tasks", "get tasks" } },
            { "complete_task", new string[] { "complete task", "mark done", "finish task", "task done", "done task", "complete" } },
            { "delete_task", new string[] { "delete task", "remove task", "clear task", "erase task", "delete", "remove" } },
            { "start_quiz", new string[] { "start quiz", "begin quiz", "play quiz", "quiz me", "take quiz", "do quiz" } },
            { "show_log", new string[] { "show log", "activity log", "what have you done", "show activity", "view log", "history" } },
            { "help", new string[] { "help", "what can you do", "options", "commands", "assistance" } },
            { "interested_in", new string[] { "interested in", "i like", "i want to learn", "teach me about", "tell me about" } },
            { "greeting", new string[] { "hello", "hi", "hey", "good morning", "good afternoon", "good evening", "greetings" } },
            { "how_are_you", new string[] { "how are you", "how are you doing", "what's up", "how goes it" } },
            { "what_is_name", new string[] { "what is your name", "who are you", "your name", "name" } }

        };

        //date patterns
        private Dictionary<string, Func<string, DateTime?>> dateExtractor = new Dictionary<string, Func<string, DateTime?>>()
        {

            { @"tomorrow", s => DateTime.Now.AddDays(1) },
            { @"today", s => DateTime.Now },
            { @"now", s => DateTime.Now },
            { @"in (\d+) days?", s => DateTime.Now.AddDays(int.Parse(Regex.Match(s, @"in (\d+) days?").Groups[1].Value)) },
            { @"in (\d+) weeks?", s => DateTime.Now.AddDays(int.Parse(Regex.Match(s, @"in (\d+) weeks?").Groups[1].Value) * 7) },
            { @"in (\d+) months?", s => DateTime.Now.AddMonths(int.Parse(Regex.Match(s, @"in (\d+) months?").Groups[1].Value)) },
            { @"on (\d{2})/(\d{2})/(\d{4})", s => {
                var match = Regex.Match(s, @"on (\d{2})/(\d{2})/(\d{4})");
                return new DateTime(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[1].Value));
            }},
            { @"(\d{2})/(\d{2})/(\d{4})", s => {
                var match = Regex.Match(s, @"(\d{2})/(\d{2})/(\d{4})");
                return new DateTime(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[1].Value));
            }}

        };

        ///Detect a user's intention from the message types
        public string detectIntention(string message)
        {
            string imessage = message.ToLower().Trim();

            foreach(var pattern in intentionPatterns)
            {
                foreach(var keyword in pattern.Value)
                {
                    if (imessage.Contains(keyword))
                        return pattern.Key;
                }

            }
            //if there is no intetion found in the message, try fuzzy matching
            return detectIntentionFuzzy(imessage);
        }

        ///Fuzzy intention dectection for slight variations
       
        private string detectIntentionFuzzy(string message)
        {
            string[] words = message.Split(new char[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach(var pattern in intentionPatterns)
            {
                foreach(var keyword in pattern.Value)
                {
                    string[] keywords = keyword.Split(' ');
                    int matches = 0;

                    foreach(string word in words)
                    {
                        if(keywords.Any(kw=> kw.Length>2 && (word.Contains(kw) || kw.Contains(word))))
                        {
                            matches++;
                        }
                    }

                    if(matches >= keywords.Length*0.7)
                    {
                        return pattern.Key;
                    }    
                }  
                
            }
            return "unknown";

        }

        ///Extract Task from a message 
        public string extractText(string message)
        {
            string[] tasks = { "add task", "add a task", "create task", "new task",
                               "set reminder", "remind me", "add reminder", "create reminder",
                               "remind me to", "set a reminder"            
            
            };

            string tmessage = message.ToLower();
            string extractedText = message.Trim();

            foreach(string task in tasks)
            {
                if(tmessage.Contains(task))
                {
                    int index = tmessage.IndexOf(task) + task.Length;
                    
                    if(index < message.Length)
                    {
                        extractedText = message.Substring(index).Trim();
                        break;
                    }
                    else
                    {
                        extractedText = "Untitles Task";
                        break;
                    }
                }

            }

            return string.IsNullOrEmpty(extractedText) ? "Untitles Task" : extractedText;
        }
        
        ///Extract a date from the given message
        public DateTime? extractDate(string message)
        {

            string tmessage = message;

            foreach(var extractor in dateExtractor)
            {
                var regex = new Regex(extractor.Key, RegexOptions.IgnoreCase);

                if(regex.IsMatch(tmessage))
                {
                    try
                    {
                        return extractor.Value(tmessage);
                    }
                    catch
                    {
                        //skip invalid date formats
                    }
                    
                }
            }  
            return null;
        }


        ///Extract the topic from the message 
        public string extractTopic(string message)
        {
            string tmessage = message;

            if(tmessage.Contains("interested in"))
            {
                int idx = tmessage.IndexOf("interested in") + "interested in".Length;
                string topic = tmessage.Substring(idx).Trim();

                topic = Regex.Replace(topic, @"[.,!?;:]$", "");
                return topic;
            }
            return null;
        }

        ///check if the message contains any of the gicen keywords
        public bool containsKeyword(string message, string[] keywords)
        {
            string tmessage = message;
            return keywords.Any(k=> tmessage.Contains(k));
        }

        ///receieve all available commands as a string 
        public string getAvailableCommands()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========== AVAILABLE COMMANDS ==========");
            sb.AppendLine("TASK MANAGEMENT:");
            sb.AppendLine("  * 'add task' or 'add task, Title, Description' - Add a new task");
            sb.AppendLine("  * 'remind me to [task] [time]' - Set a reminder");
            sb.AppendLine("  * 'show tasks' or 'view tasks' - See all your tasks");
            sb.AppendLine("  * 'complete task [id]' - Mark a task as completed");
            sb.AppendLine("  * 'delete task [id]' - Delete a task");
            sb.AppendLine();
            sb.AppendLine("QUIZ:");
            sb.AppendLine("  * 'start quiz' or 'quiz me' - Start the cybersecurity quiz");
            sb.AppendLine();
            sb.AppendLine("ACTIVITY LOG:");
            sb.AppendLine("  * 'show log' or 'activity log' - View recent bot activities");
            sb.AppendLine();
            sb.AppendLine(" CYBERSECURITY TOPICS:");
            sb.AppendLine("  * Ask about: passwords, phishing, scams, privacy, malware, ransomware,");
            sb.AppendLine("    2FA, encryption, safe browsing, firewalls, social engineering, data breaches");
            sb.AppendLine();
            sb.AppendLine("OTHER:");
            sb.AppendLine("  * 'I'm interested in [topic]' - Saves your favourite topic");
            sb.AppendLine("  * 'memory recap' - Shows your saved preferences");
            sb.AppendLine("  * 'clear chat' - Clears the chat display");
            sb.AppendLine("  * 'voice toggle' - Toggles voice output");

            return sb.ToString();
        }
    

    }

}

