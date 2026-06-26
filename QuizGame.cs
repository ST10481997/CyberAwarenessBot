using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Media;
using System.Text;
using System.IO.Packaging;
using System.Security.Policy;



namespace CyberAwarenessBot
{

    ///Repressent a quiz question
    public class QuizQuestion
    {
        public string Question {  get; set; }
        public string [] Options { get; set; }
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; }
        public bool isTrueFalse => Options?.Length == 2 && (Options[0] == "True" || Options[0] == "False");


        public QuizQuestion(string question,  string [] options,  int correctAnswer,  string explanation)
        {
            Question = question;
            Options = options;
            CorrectAnswer = correctAnswer;
            Explanation = explanation;
        }
    }
    
    /// event arguments for the quiz events 
    public class QuizEventArgs : EventArgs
    {
        public QuizQuestion Question { get; set; }
        public int QuestionNumber { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsFirstQuestion { get; set; }
        public bool IsLastQuestion { get; set; }
        public int Score { get; set; }
        public bool IsComplete { get; set; }
    }

    
    /// result of an answer attempt
    public class QuizResult
    {
        public bool IsCorrect {  get; set; }
        public string CorrectAns {  get; set; }
        public string Explanation { get; set; }
        public int SelectedIndex { get; set; }
    }

    
    /// Summmary of the quiz
    public class QuizSummary
    {
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public double Percentage { get; set; }
        public string Feedback { get; set; }
    }

    //Manages the cybersecurity quiz game functionality
    public class QuizGame
    {
        private List<QuizQuestion> questions;
        private int currentQuestion = 0;
        private int score = 0;
        private bool isActive = false;
        private Random random = new Random();

        //event to notify when quix state changes
        public event EventHandler<QuizEventArgs> QuestionDisplayed;
        public event EventHandler<QuizEventArgs> QuizCompleted;

        public bool IsActive => isActive;
        public int CurrentQuestion => currentQuestion;
        public int Score => score;
        public int TotalQuestions => questions?.Count ?? 0;

        public QuizGame()
        {
            InitialiseQuestions();
        }
         /// <summary>
         /// this method initalises the quiz questions 
         /// </summary>
        private void InitialiseQuestions()
        {
            questions = new List<QuizQuestion>
            {
                //Multiple choice Quescions

                new QuizQuestion(
                    "What is the best way to create a strong password?",
                    new string[] {
                        "Use your birthday and name",
                        "Use a password manager to generate a random 12+ character password",
                        "Use the same password for all accounts",
                        "Use 'password123'"
                    },
                    1,
                    "Using a password manager to generate unique, complex passwords is the most secure approach."
                ),
                new QuizQuestion(
                    "What should you do if you receive an email asking for your password?",
                    new string[] {
                        "Reply with your password",
                        "Delete the email",
                        "Report the email as phishing",
                        "Ignore it"
                    },
                    2,
                    "Reporting phishing emails helps prevent scams and protects others from falling victim."
                ),
                new QuizQuestion(
                    "What does 'https' indicate in a website URL?",
                    new string[] {
                        "The website is hosted in the US",
                        "The connection is encrypted and secure",
                        "The website is fast",
                        "The website is free to use"
                    },
                    1,
                    "'https' indicates that the connection between your browser and the website is encrypted."
                ),
                new QuizQuestion(
                    "What is two-factor authentication (2FA)?",
                    new string[] {
                        "Using two passwords",
                        "An extra layer of security requiring a second verification step",
                        "Having two different accounts",
                        "A type of antivirus software"
                    },
                    1,
                    "2FA adds an extra layer of security by requiring a second verification method beyond your password."
                ),
                new QuizQuestion(
                    "Which of the following is a sign of a phishing attempt?",
                    new string[] {
                        "The email uses your correct name",
                        "The email has spelling and grammar errors",
                        "The email is from a known company",
                        "The email has official logos"
                    },
                    1,
                    "Spelling and grammar errors are common red flags in phishing emails."
                ),
                new QuizQuestion(
                    "What is ransomware?",
                    new string[] {
                        "A type of antivirus software",
                        "Malware that encrypts files and demands payment",
                        "A password manager",
                        "A web browser"
                    },
                    1,
                    "Ransomware is malicious software that encrypts your files and demands payment for their release."
                ),
                new QuizQuestion(
                    "What is social engineering?",
                    new string[] {
                        "Building social media profiles",
                        "Manipulating people to reveal confidential information",
                        "Engineering social networks",
                        "A type of firewall"
                    },
                    1,
                    "Social engineering uses psychological manipulation to trick people into giving up sensitive information."
                ),

                // True/False Questions
                new QuizQuestion(
                    "It's safe to use public Wi-Fi without a VPN.",
                    new string[] { "True", "False" },
                    1,
                    "Public Wi-Fi is often unsecured. Always use a VPN to encrypt your traffic on public networks."
                ),
                new QuizQuestion(
                    "Ransomware can be prevented by keeping regular offline backups.",
                    new string[] { "True", "False" },
                    0,
                    "Offline backups ensure your data is safe even if ransomware encrypts your main system."
                ),
                new QuizQuestion(
                    "You should use the same password for all your online accounts.",
                    new string[] { "True", "False" },
                    1,
                    "Using the same password for all accounts is dangerous. If one account is compromised, all others are at risk."
                ),
                new QuizQuestion(
                    "Antivirus software is the only protection you need against cybersecurity threats.",
                    new string[] { "True", "False" },
                    1,
                    "Antivirus is important, but you also need strong passwords, 2FA, safe browsing habits, and regular updates."
                ),
                new QuizQuestion(
                    "It's safe to click on links from unknown senders if they claim to be from a bank.",
                    new string[] { "True", "False" },
                    1,
                    "Never click links from unknown senders, even if they claim to be legitimate. Always type the URL manually."
                ),
                new QuizQuestion(
                    "Two-factor authentication adds an extra layer of security.",
                    new string[] { "True", "False" },
                    0,
                    "2FA requires a second verification step, making it much harder for attackers to access your accounts."
                ),
                new QuizQuestion(
                    "Using 'password123' as your password is safe if you add a number.",
                    new string[] { "True", "False" },
                    1,
                    "'password123' is one of the most common passwords and is easily cracked. Always use complex, unique passwords.")
            };
        }

        ///Method to start the quiz
        public void startQuiz()
        {
            //shuffle questions for variety
            questions = questions.OrderBy(q => random.Next()).ToList();
            currentQuestion = 0;
            score = 0;
            isActive = true;

            OnQuestionDisplayed(new QuizEventArgs
            {
                Question = questions[currentQuestion],
                QuestionNumber = currentQuestion + 1,
                TotalQuestions = questions.Count,
                IsFirstQuestion = true,
                IsLastQuestion = questions.Count == 1

            });

        }

        /// process the answer to the current question
        public QuizResult processAnswer(int selectedIndex)
        {
            if(!isActive || currentQuestion >= questions.Count)
            {
                return null;
            }

            var question = questions[currentQuestion];
            bool isCorrect = selectedIndex == question.CorrectAnswer;

            if(isCorrect)
            {
                score++;
            }

            var result = new QuizResult
            {
                IsCorrect = isCorrect,
                CorrectAns = question.Options[question.CorrectAnswer],
                Explanation = question.Explanation,
                SelectedIndex = selectedIndex,

            };

            //move on to the next question
            currentQuestion++;

            //check if the quiz is complete
            if(currentQuestion >= questions.Count)
            {
                isActive = false;
                OnQuizCompleted(new QuizEventArgs
                {
                    Score = score,
                    TotalQuestions=questions.Count,
                    IsComplete = true

                });
              
            }
            else
            {
                OnQuestionDisplayed(new QuizEventArgs
                {
                    Question=questions[currentQuestion],
                    QuestionNumber=currentQuestion + 1,
                    TotalQuestions = questions.Count,
                    IsFirstQuestion = false,
                    IsLastQuestion = currentQuestion == questions.Count -1

                });
            }

            return result;
        }

        
        /// gets the current question
        public QuizQuestion getCurrentQuestion()
        {
            if(!isActive || currentQuestion >= questions.Count)
            {
                return null;
            }

            return questions[currentQuestion];
        }

        /// Calculate the final score
        public QuizSummary GetSummary()
        {
            double percentage = (double)score / questions.Count * 100;
            string feedback = percentage >= 80 ? "EXCELLENT! You are a cybersecurity genius" :
                percentage >= 60 ? "GREAT JOB! Keep up the momentum of learning about cybersecurity to stay safe online" :
                "Keep Learning! Cybersecurity is fun, it is important for everyone. Help yourself stay safe@!";

            return new QuizSummary
            {
                Score = score,
                TotalQuestions = (int)questions.Count,
                Percentage = percentage,
                Feedback = feedback
            };
            
        }

        
        /// Method to reset the quiz after user is done
        public void resetQuiz()
        {
            isActive = false;
            currentQuestion = 0;
            score = 0;
        }
        


        protected virtual void OnQuestionDisplayed(QuizEventArgs e)
        {
            QuestionDisplayed?.Invoke(this, e);
        }

        protected virtual void OnQuizCompleted(QuizEventArgs e)
        {
            QuizCompleted?.Invoke(this, e);
        }

        //public int QuestionDisplayed { get; internal set; }
    }
}