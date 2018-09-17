USE [Tfs_Instance]
GO
/****** Object:  Trigger [dbo].[trOnLabelDelete]    Script Date: 9/14/2018 4:15:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:    Richard Nunez
-- Create date: 9/12/2018
-- Description: Delete the label from this summary table when it is deleted from the TFSVC system.
-- =============================================
ALTER TRIGGER [dbo].[trOnLabelDelete] 
   ON  [dbo].[tbl_Label]
   FOR DELETE
AS 
BEGIN
  -- SET NOCOUNT ON added to prevent extra result sets from
  -- interfering with SELECT statements.
  SET NOCOUNT ON;

    -- Delete statements for trigger here
  delete a
  from DevOps_TFSMetrics.dbo.tblTFSLabels a
    inner join DELETED DLTD ON
      DLTD.PartitionId = a.PartitionId and
    DLTD.LabelId = a.LabelId
  

END

--DROP TRIGGER [dbo].[trOnLabelDelete]
--GO