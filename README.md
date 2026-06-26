# HAPPYCODER - Part 2 to Part 3 Improvements
### Overview of Improvements
Part 3 transforms the chatbot from a simple Q&A bot into a full-featured cybersecurity assistant with task management, interactive learning, and activity tracking. Here's a comprehensive breakdown:

## New Features Added in Part 3
### 1. Task Management System
* Database Integration: Uses SQL Server (localdb) to store tasks

* CRUD Operations: Create, Read, Update, Delete tasks

* Reminders: Set optional reminders for tasks

* Task Status: Mark tasks as completed or pending

* Task Display: View all tasks with their status

### 2. Cybersecurity Quiz Game
* 15+ Questions: Multiple choice and true/false format

* Immediate Feedback: Tells users if they got it right/wrong with explanations

* Score Tracking: Tracks correct answers

* Final Score: Displays percentage and encouraging feedback

* Randomized Questions: Questions appear in random order each time

### 3. Natural Language Processing (NLP) Simulation
* Intent Detection: Recognizes user intent even with different phrasing

* Keyword Matching: Detects commands and topics using keywords

* Fuzzy Matching: Handles slight variations in user input

* Task Extraction: Extracts task details from natural language

* Date Extraction: Recognizes date mentions like "tomorrow", "in 3 days"

### 4. Activity Logging System
* Action Tracking: Records every significant action

* Categories: Organizes logs by type (Task, Quiz, NLP, System, etc.)
  
* Timestamps: Each action is timestamped

* View Log: Users can type "show log" to view recent activities

* Statistics: Tracks activity counts by category


## Feature Comparison: Part 2 vs Part 3
### Feature:	
#### Part 2:
hatbot Responses:	Yes
Topic Detection: Yes
Sentiment Detection:	Yes
Quick Topics:	Yes
Voice Output:	 Yes
Memory Storage: Yes (Text file)
Task Management: No	
Quiz Game: No	
NLP Simulation: No
Activity Logging:	No	
Database Integration:	No
Task Reminders:	No
#### Part 3:
Chatbot Responses: Yes (Enhanced)
Topic Detection: Yes (Enhanced)
Sentiment Detection: Yes
Quick Topics: Yes
Voice Output:	 Yes
Memory Storage: Yes (Text file)
Task Management: Yes (Database)
Quiz Game: Yes
NLP Simulation: Yes
Activity Logging: Yes
Database Integration:Yes (SQL)
Task Reminders: Yes

## How Each Component Works
### 1. QuizGame.cs - Interactive Quiz System
Purpose: Tests users' cybersecurity knowledge through an interactive quiz.

How It Works:

text
User: "start quiz"

1. Initialize Questions (15 questions)
   - Multiple Choice                   -
   - True/False                                                                 
2. Shuffle Questions Randomly          
                                         
3. Display First Question             
   - "Question 1 of 15: What is...?"    
   
4. User Answers (type 1, 2, 3, or 4)  
                                                         
5. Check Answer                       
    - If Correct: Score++              
    - Show Feedback                    
                                        
6. Next Question (repeat until done)  
                                       
7. Show Final Score
    - "8/15 - 53% - Keep Learning!"

Key Methods:

csharp
startQuiz()         
processAnswer()     
GetSummary()        

### 2. NLPSimulator.cs - Natural Language Processing
Purpose: Understands user intent even when phrased differently.

How It Works:

text
User Input: "Remind me to update my password tomorrow"

detectIntention(message)                

1. Convert to lowercase
2. Check against intentPatterns Dictionary                        
3. Find matching keywords:                         
   "remind me" → "set_reminder"                  
4. Retrn intent: "set_reminder" 

extractText(message)
1. Remove "Remind me to" prefix
2. Extract: "Update my password"
3. Return task text

extractDate(message)
1. Check for "Tomorrow"
2. Return: DateTime.Now.AddDays(1)
3. Created Reminder date

ntent Patterns:

Intent	Keywords
- add_task:	"add task", "create task", "new task"
- set_reminder	"remind me", "set reminder", "remind in"
- show_tasks	"show tasks", "view tasks", "my tasks"
- complete_task	"complete task", "mark done", "finish task"
- delete_task	"delete task", "remove task", "clear task"
- start_quiz	"start quiz", "quiz me", "take quiz"
- show_log	"show log", "activity log", "what have you done"
- help	"help", "what can you do", "options"

### 3. ActivityLogger.cs - Activity Tracking
Purpose: Records all significant actions for user review.
ActivityLogger.Log(action, details, category)

Example: Log("Task Added", "Task: 'Enable 2FA'", "Task")

1. Create ActivityEntry with:

- Timestamp: DateTime.Now

- Action: "Task Added"

- Details: "Task: 'Enable 2FA'"

- Category: "Task"

- Id: Auto-incremented

2. Add to activities list

3. If list > 60 entries, remove oldest

4. Raise ActivityLogged event

#### When User Types "show log":

========== ACTIVITY LOG ==========

[14:32:15] System: Application started

Details: CyberAwareness bot launched

[14:32:45] Login: User Logged In

Details: User: NTOKOZO

[14:33:10] User: User Input

Details: Message: 'Tell me about password'

[14:34:20] Task: Task Added (NLP)

Details: Task: 'Update password'

[14:35:00] Quiz: Quiz Started

[14:36:00] Quiz: Quiz Completed

Details: Score: 7 out of 10

Showing last 6 actions.
Total activities: 6

### 3. TaskRepository.cs - Database Operations
Purpose: Manages tasks in the SQL database.

Database Schema:

Table: userTasks

taskid INT (IDENTITY) PRIMARY KEY

taskTitle NVARCHAR

taskDescription NVARCHAR 

taskReminderDate DATETIME (NULLABLE) 

isCompleted BIT (0 = Pending, 1 = Completed

CRUD Operations:

AddTask()

INSERT INTO userTasks(taskTitle, taskDescription, taskReminderDate, isCompleted)

VALUES (@title, @description, @reminderDate, 0)

GetAllTasks()

SELECT * FROM userTasks

ORDER BY isCompleted ASC, taskReminderDate ASC

MarkTaskCompleted()

UPDATE userTasks SET isCompleted = 1

WHERE taskid = @taskId

DeleteTask()

DELETE FROM userTasks WHERE taskid = @taskId

### Data Flow: User Interaction Example
Example: Adding a Task

User Types: "add task, Enable 2FA, Set up two-factor authentication, 25/06/2026"

send_question()

Display user message: "NTOKOZO: add task..."

Clear question box

Check for "interested in" → No

NLP Detection: detectIntention() → "add_task"

Switch to "add_task" case

Call AddTaskWithNLP(message)


AddTaskWithNLP()

Extract task text: "Enable 2FA, Set up..."

Extract date: "25/06/2026" → DateTime(2026, 6, 25)

Split into title and description


Title: "Enable 2FA"

Description: "Set up two-factor authentication"

repo.AddTask(title, description, reminderDate)

activityLogger.Log("Task Added (NLP)", ...)

Display success message in chat
│
▼
TasksRepository.AddTask()

Connect to SQL database (CyberBot)

Execute INSERT command

Save task with isCompleted = 0 (pending)
│
▼
Chat Output


HAPPYCODER: Task added successfully!

Title: Enable 2FA

Description: Set up two-factor authentication

Reminder: 25/06/2026

### The Message Flow Process
Here's the complete flow when a user sends a message:

User Input
│
▼
send_question()

Get message from question_box

If empty: Show error message

Display: "{name}: {message}"

Clear question_box

Check for "interested in" → SaveToFile()

NLP Intent Detection:
intent = nlpSimulator.detectIntention(message)

Log User Input:
activityLogger.Log("User Input", message, "User")

Switch on Intent:
- "add_task" → AddTaskWithNLP()
- "show_tasks" → ShowAllTasks()
- "complete_task" → CompleteTask()
- "delete_task" → DeleteTask()
- "start_quiz" → StartQuiz()
- "show_log" → ShowActivityLog()
- "help" → ShowHelp()
- "greeting" → Display greeting
- "how_are_you" → Display response
- "what_is_name" → Display name response
- default → Continue to next step

Check if Quiz is Active:

if (quizGame.IsActive) → Process Quiz Answer

Check for "add task" format (backward compatibility)

Get Bot Response:

botResponse = chatbotResponse(message)

Display Response:

chats_box.AppendText($"HAPPYCODER: {botResponse}")

Voice Output (if enabled)

Scroll to end

## Summary of Improvements
From Part 2 → Part 3
Aspect: 
#### Part 2	
- Purpose:	Basic Q&A Chatbot
- Data Storage:	Text file only
- Interactivity:	Passive responses
- User Actions:	Ask 
- NLP: Basic keyword matching
- Logging:	None
- Task Management:	None
- Learning:	Text file memory
#### Part 3
- Purpose:Full-featured Cybersecurity Assistant
- Data Storage: SQL Database + Text file
- Interactivity: Interactive (Quiz, Tasks)
- User Actions: Ask, Add, Complete, Delete, Quiz
- NLP:Intent detection + fuzzy matching
- Logging: Complete activity logging
- Task Management: Full CRUD with reminders
- Learning:Database persistence

## New User Capabilities
1. Manage Tasks: Add, view, complete, and delete tasks

2. Take Quizzes: Test cybersecurity knowledge

3. Use Natural Language: Phrase requests differently

4. View Activity Log: See what the bot has done

5. Set Reminders: Add dates to tasks

## Why These Improvements Matter
1. Practical Application: Users can actually manage cybersecurity tasks

2. Engagement: Interactive quiz makes learning fun

3. Flexibility: NLP allows natural conversation

4. Transparency: Activity log builds trust

5. Persistence: Database ensures tasks aren't lost

6. Professional: Full CRUD operations show real-world application

# Author
[Ntokozo Happiness Tshabalala] [ST10481997]

# Diploma in Software Development 
[Programming 2A] [Rosebank College Braamfontein]
