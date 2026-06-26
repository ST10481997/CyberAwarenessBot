SET IDENTITY_INSERT [dbo].[userTasks] ON
INSERT INTO [dbo].[userTasks] ([taskid], [taskTitle], [taskDescription], [taskReminderDate], [isCompleted]) VALUES (1, N'cyberlesson', N'it will be online', NULL, 0)
INSERT INTO [dbo].[userTasks] ([taskid], [taskTitle], [taskDescription], [taskReminderDate], [isCompleted]) VALUES (2, N'cyberlesson', N'it will be online', NULL, 0)
INSERT INTO [dbo].[userTasks] ([taskid], [taskTitle], [taskDescription], [taskReminderDate], [isCompleted]) VALUES (3, N'cyberlesson', N'will be online', NULL, 0)
INSERT INTO [dbo].[userTasks] ([taskid], [taskTitle], [taskDescription], [taskReminderDate], [isCompleted]) VALUES (4, N'cyberlesson', N'will be online', N'2026-01-22 00:10:00', 0)
SET IDENTITY_INSERT [dbo].[userTasks] OFF
