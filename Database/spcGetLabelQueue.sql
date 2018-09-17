USE [DevOps_TFSMetrics]
GO
/****** Object:  StoredProcedure [dbo].[spcGetLabelQueue]    Script Date: 9/13/2018 5:19:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE spcGetLabelQueue
AS
  SELECT PartitionId, LabelId, LabelName, IsNotified
  FROM [dbo].[tblTFSLabels]
  WHERE IsQueued = 0 and IsNotified = 0
GO
