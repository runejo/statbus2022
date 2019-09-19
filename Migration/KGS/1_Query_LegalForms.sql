DELETE FROM [dbo].[LegalForms]
GO
DBCC CHECKIDENT ('dbo.LegalForms',RESEED, 1)
GO

INSERT INTO [dbo].[LegalForms]
  ([Code]
  ,[IsDeleted]
  ,[Name]
  ,[ParentId])
SELECT 
  [K_OPF]
  ,0
  ,[N_OPF]
  ,NULL
FROM [statcom].[dbo].[SPROPF]
GO