using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Tracing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace CyberAwarenessBot
{
    ///UserTask class to represent a task
    public class UserTasks
    {
        public int TaskID { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public DateTime? TaskReminderDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class TasksRepository
    {

        public readonly string connection =
            @"Data Source=(localdb)\MSSQLLocalDB; 
              Initial Catalog=CyberBot;
              Integrated Security = True;";

        /// <summary>
        /// Add task
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="reminderDate"></param>
        public void AddTask(string title, string description, DateTime? reminderDate)
        {

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();

                //Adding into a table
                string query = @" INSERT INTO userTasks(taskTitle, taskDescription, taskReminderDate)
                                   VALUES (@title, @description, @reminderDate)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("title", title);
                    cmd.Parameters.AddWithValue("description", description);

                    if (reminderDate == null)
                    {
                        cmd.Parameters.AddWithValue("@reminderDate", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@reminderDate", reminderDate);
                    }

                    cmd.ExecuteNonQuery();



                }

            }
        }

        ///Retrieve all tasks from database
        public List<UserTasks> GetAllTasks()
        {
            List<UserTasks> tasks = new List<UserTasks>();
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = @"SELECT * FROM userTasks 
                                 ORDER BY isCompleted ASC, taskReminderDate ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new UserTasks
                        {
                            TaskID = reader.GetInt32(reader.GetOrdinal("taskid")),
                            TaskTitle = reader.GetString(reader.GetOrdinal("taskTitle")),
                            TaskDescription = reader.GetString(reader.GetOrdinal("taskDescription")),
                            TaskReminderDate = reader.IsDBNull(reader.GetOrdinal("taskReminderDate")) ?
                            (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("taskReminderDate")),
                            IsCompleted = reader.GetBoolean(reader.GetOrdinal("isCompleted"))
                        });
                    }
                }


            }
            return tasks;
        }

        ///Mark task as completed
        public bool MarkTaskCompleted(int taskId)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "UPDATE userTasks SET isCompleted = 1 WHERE taskid = @taskId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return (rowsAffected > 0);
                }
            }
        }

        /// Delete a task from the database
        public bool DeleteTask(int taskId)
        {
            using (SqlConnection conn =new SqlConnection(connection))
            {
                conn.Open();
                string query = "DELETE FROM userTasks WHERE taskid = @taskId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return (rowsAffected > 0);
                }

            }
        }

        ///Delete all completed tasks
        public int DeleteCompletedTasks()
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "DELETE FROM userTasks WHERE isCompleted = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public bool markTaskPending(int taskId)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "UPDATE userTasks SET isCompleted = 0 WHERE taskid = @taskId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }

        }

        //Method to view all the pendinf uncompleted tasks
        public List<UserTasks> viewPendingTasks()
        {
            List<UserTasks> tasks = new List<UserTasks>();

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = @"SELECT * FROM userTasks WHERE isCompleted = 0 
                                ORDER BY taskReminderDate ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader()) 
                {
                    while(reader.Read())
                    {
                        tasks.Add(new UserTasks
                        {

                            TaskID = reader.GetInt32(reader.GetOrdinal("taskid")),
                            TaskTitle = reader.GetString(reader.GetOrdinal("taskTitle")),
                            TaskDescription = reader.GetString(reader.GetOrdinal("taskDescription")),
                            TaskReminderDate = reader.IsDBNull(reader.GetOrdinal("taskReminderDate")) ?
                            (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("taskReminderDate")),
                            IsCompleted = reader.GetBoolean(reader.GetOrdinal("isCompleted"))

                        });
                    }

                }
            }
            return tasks;
        }

        ///GET TASK BY IS ITS ID
        public UserTasks getTaskById(int taskId)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT * FROM userTasks WHERE taskId = @taskId";

                using (SqlCommand cmd = new SqlCommand(@query, conn))
                {
                    cmd.Parameters.AddWithValue("@taskId", taskId);
                    using (SqlDataReader reader = cmd.ExecuteReader()) 
                    {
                        if(reader.Read())
                        {
                            return new UserTasks
                            {

                                TaskID = reader.GetInt32(reader.GetOrdinal("taskid")),
                                TaskTitle = reader.GetString(reader.GetOrdinal("taskTitle")),
                                TaskDescription = reader.GetString(reader.GetOrdinal("taskDescription")),
                                TaskReminderDate = reader.IsDBNull(reader.GetOrdinal("taskReminderDate")) ?
                                    (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("taskReminderDate")),
                                IsCompleted = reader.GetBoolean(reader.GetOrdinal("isCompleted"))

                            };
                        }
                    }
                }
            }

            return null;
        }

        
    }
}
