USE [DevOps_TFSMetrics]
GO

/****** Object:  StoredProcedure [dbo].[spcSetLabelToNotified]    Script Date: 9/17/2018 2:14:20 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[spcSetLabelToNotified]
  @LabelId INT
AS
  Update [dbo].[tblTFSLabels]
  Set IsNotified = 1
  WHERE LabelId = @LabelId and IsNotified = 0 
GO


