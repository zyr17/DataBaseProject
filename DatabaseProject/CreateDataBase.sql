IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'homework_entity_attachments'))
	DROP TABLE homework_entity_attachments;
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'homework_attachments'))
	DROP TABLE homework_attachments;
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'homework_entities'))
	DROP TABLE homework_entities;
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'students_has_courses'))
	DROP TABLE students_has_courses;
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'homeworks'))
	DROP TABLE homeworks;
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'courses'))
	DROP TABLE courses;
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'students'))
	DROP TABLE students;
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'teachers'))
	DROP TABLE teachers;
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'admins'))
	DROP TABLE admins;
GO
CREATE TABLE [dbo].[admins] (
    [ID]       INT          NOT NULL,
    [password] CHAR (64)    NOT NULL,
    [username] VARCHAR (64) NOT NULL,
    [nickname] NVARCHAR(10) NOT NULL, 
    PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO
CREATE TABLE [dbo].[teachers] (
    [ID]       INT          NOT NULL,
    [password] CHAR (64)    NOT NULL,
    [username] VARCHAR (64) NOT NULL,
    [nickname] NVARCHAR(10) NOT NULL, 
    PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO
CREATE TABLE [dbo].[students] (
    [ID]       INT          NOT NULL,
    [password] CHAR (64)    NOT NULL,
    [username] VARCHAR (64) NOT NULL,
    [nickname] NVARCHAR(10) NOT NULL, 
    PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO
CREATE TABLE [dbo].[courses] (
    [ID]          INT            NOT NULL,
    [name]        NVARCHAR (100) NOT NULL,
    [teachers_ID] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [teachers_ID_in_courses] FOREIGN KEY ([teachers_ID]) REFERENCES [dbo].[teachers] ([ID])
)
GO
CREATE TABLE [dbo].[homeworks] (
    [ID]         INT             NOT NULL,
    [time]       DATETIME        NOT NULL,
    [title]      NVARCHAR (200)  NOT NULL,
    [content]    NVARCHAR (1000) NOT NULL,
    [totalscore] INT             NOT NULL,
    [courses_ID] INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [courses_ID_in_homeworks] FOREIGN KEY ([courses_ID]) REFERENCES [dbo].[courses] ([ID])
)
GO
CREATE TABLE [dbo].[students_has_courses] (
    [students_ID] INT NOT NULL,
    [courses_ID]  INT NOT NULL,
    CONSTRAINT [PK_students_has_courses] PRIMARY KEY CLUSTERED ([courses_ID] ASC, [students_ID] ASC),
    CONSTRAINT [courses_ID_in_students_has_courses] FOREIGN KEY ([courses_ID]) REFERENCES [dbo].[courses] ([ID]),
    CONSTRAINT [students_ID_in_students_has_courses] FOREIGN KEY ([students_ID]) REFERENCES [dbo].[students] ([ID])
)
GO
CREATE TABLE [dbo].[homework_entities] (
    [ID]           INT             NOT NULL,
    [text]         NVARCHAR (1000) NOT NULL,
    [score]        INT             NOT NULL,
    [comment]      NVARCHAR (1000) NOT NULL,
    [is_submitted] BIT             NOT NULL,
    [is_returned]  BIT             NOT NULL,
    [homeworks_ID] INT             NOT NULL,
    [students_ID]  INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [homeworks_ID_in_homework_entities] FOREIGN KEY ([homeworks_ID]) REFERENCES [dbo].[homeworks] ([ID]),
    CONSTRAINT [students_ID_in_homework_entities] FOREIGN KEY ([students_ID]) REFERENCES [dbo].[students] ([ID])
)
GO
CREATE TABLE [dbo].[homework_attachments] (
    [ID]           INT            NOT NULL,
    [attachment]   NVARCHAR (256) NOT NULL,
    [filename]     NVARCHAR (256) NOT NULL,
    [homeworks_ID] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [homeworks_ID_in_homework_attachments] FOREIGN KEY ([homeworks_ID]) REFERENCES [dbo].[homeworks] ([ID])
)
GO
CREATE TABLE [dbo].[homework_entity_attachments] (
    [ID]                   INT            NOT NULL,
    [attachment]           NVARCHAR (256) NOT NULL,
    [filename]             NVARCHAR (256) NOT NULL,
    [homework_entities_ID] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [homework_entities_ID_in_homework_entity_attachments] FOREIGN KEY ([homework_entities_ID]) REFERENCES [dbo].[homework_entities] ([ID])
)
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'drop_all'))
	DROP PROC drop_all
GO
CREATE PROC drop_all 
AS
BEGIN
    DELETE FROM homework_entity_attachments;
    DELETE FROM homework_attachments;
    DELETE FROM homework_entities;
    DELETE FROM homeworks;
    DELETE FROM courses;
    DELETE FROM students_has_courses;
    DELETE FROM teachers;
    DELETE FROM students;
    DELETE FROM admins;
END
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'courses_delete_trigger'))
	DROP TRIGGER courses_delete_trigger
GO
CREATE TRIGGER courses_delete_trigger
ON courses
	INSTEAD OF DELETE
AS
BEGIN
	DELETE FROM homeworks WHERE courses_ID IN (SELECT ID FROM deleted);
	DELETE FROM students_has_courses WHERE courses_ID IN (SELECT ID FROM deleted);
	DELETE FROM courses WHERE ID IN (SELECT ID FROM deleted);
END
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'teachers_delete_trigger'))
	DROP TRIGGER teachers_delete_trigger
GO
CREATE TRIGGER teachers_delete_trigger
ON teachers
	INSTEAD OF DELETE
AS
BEGIN
	DELETE FROM courses WHERE teachers_ID IN (SELECT ID FROM deleted);
	DELETE FROM teachers WHERE ID IN (SELECT ID FROM deleted);
END
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'students_delete_trigger'))
	DROP TRIGGER students_delete_trigger;
GO
CREATE TRIGGER students_delete_trigger
ON students
	INSTEAD OF DELETE
AS
BEGIN
	DELETE FROM students_has_courses WHERE students_ID IN (SELECT ID FROM deleted);
	DELETE FROM homework_entities WHERE students_ID IN (SELECT ID FROM deleted);
	DELETE FROM students WHERE ID IN (SELECT ID FROM deleted);
END
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'homeworks_delete_trigger'))
	DROP TRIGGER homeworks_delete_trigger;
GO
CREATE TRIGGER homeworks_delete_trigger
ON homeworks
	INSTEAD OF DELETE
AS
BEGIN
	DELETE FROM homework_attachments WHERE homeworks_ID IN (SELECT ID FROM deleted);
	DELETE FROM homework_entities WHERE homeworks_ID IN (SELECT ID FROM deleted);
	DELETE FROM homeworks WHERE ID IN (SELECT ID FROM deleted);
END
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'homework_entities_delete_trigger'))
	DROP TRIGGER homework_entities_delete_trigger;
GO
CREATE TRIGGER homework_entities_delete_trigger
ON homework_entities
	INSTEAD OF DELETE
AS
BEGIN
	DELETE FROM homework_entity_attachments WHERE homework_entities_ID IN (SELECT ID FROM deleted);
	DELETE FROM homework_entities WHERE ID IN (SELECT ID FROM deleted);
END
GO
IF (EXISTS (SELECT * FROM sys.objects WHERE name = 'students_has_courses_delete_trigger'))
	DROP TRIGGER students_has_courses_delete_trigger;
GO
CREATE TRIGGER students_has_courses_delete_trigger
ON students_has_courses
	INSTEAD OF DELETE
AS
BEGIN
	DELETE FROM homework_entities WHERE homeworks_ID IN (SELECT ID FROM homeworks WHERE courses_ID IN (SELECT courses_ID FROM deleted)) AND students_ID IN (SELECT students_ID FROM deleted);
	DELETE FROM students_has_courses WHERE students_ID IN (SELECT students_ID FROM deleted) AND courses_ID IN (SELECT courses_ID FROM deleted);
END
GO
INSERT INTO admins(ID, username, password, nickname) VALUES(1, '111', 'BCB15F821479B4D5772BD0CA866C00AD5F926E3580720659CC80D39C9D09802A', 'null');