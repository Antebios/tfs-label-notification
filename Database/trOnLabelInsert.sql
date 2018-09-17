USE [Tfs_Instance]
GO

/****** Object:  Trigger [dbo].[trOnLabelInsert]    Script Date: 9/14/2018 4:14:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:    Richard Nunez
-- Create date: 4/4/2018
-- Description: Instead of querying, I need to know when TFS labels were created and be aware of them.
-- =============================================
CREATE TRIGGER [dbo].[trOnLabelInsert] 
   ON  [dbo].[tbl_Label]
   AFTER INSERT
AS 
BEGIN
  -- SET NOCOUNT ON added to prevent extra result sets from
  -- interfering with SELECT statements.
  SET NOCOUNT ON;

    -- Insert statements for trigger here
  Insert into DevOps_TFSMetrics.dbo.tblTFSLabels 
   (PartitionId, LabelScopeDataspaceId, LabelId, LabelName,
            ItemId, OwnerId, Comment, LastModified, SnapshotDT, [Event])
  select PartitionId, LabelScopeDataspaceId, LabelId, LabelName,
            ItemId, OwnerId, Comment, LastModified, GETDATE(), 'Inserted'
    from inserted

END

GO


