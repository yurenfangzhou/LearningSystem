--2019-04-24
/*���ӹ�������*/
alter table Organization add Org_GonganBeian  [nvarchar](255) NULL
go
/*ɾ��������΢����ʹ�õ��ֶΣ��ù��ܲ���������ʽ���֣�*/
ALTER TABLE Organization DROP COLUMN Org_IsOnlyWeixin 
go
--alter table [Student_Course] add Stc_IsTry  float not NULL
/*���ѧԱ������ϰ�÷ּ�¼*/
alter table [Student_Course] add Stc_QuesScore float NULL
go
UPDATE [Student_Course]  SET Stc_QuesScore = 0
GO
alter table [Student_Course] ALTER COLUMN Stc_QuesScore float NOT NULL
go
/*���ѧԱ��Ƶѧϰ���ȼ�¼*/
alter table [Student_Course] add Stc_StudyScore float NULL
go
UPDATE [Student_Course]  SET Stc_StudyScore = 0
GO
alter table [Student_Course] ALTER COLUMN Stc_StudyScore float NOT NULL
go
/*���ѧԱ��ο��Գɼ���¼*/
alter table [Student_Course] add Stc_ExamScore float NULL
go
UPDATE [Student_Course]  SET Stc_ExamScore = 0
GO
alter table [Student_Course] ALTER COLUMN Stc_ExamScore float NOT NULL
go