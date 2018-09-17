USE [DevOps_TFSMetrics]
GO

/****** Object:  Table [dbo].[tblTFSLabels]    Script Date: 9/14/2018 4:16:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblTFSLabels](
  [PartitionId] [int] NOT NULL,
  [LabelScopeDataspaceId] [int] NOT NULL,
  [LabelId] [int] NOT NULL,
  [LabelName] [nvarchar](64) NOT NULL,
  [ItemId] [int] NOT NULL,
  [OwnerId] [uniqueidentifier] NOT NULL,
  [Comment] [nvarchar](2048) NOT NULL,
  [LastModified] [datetime] NOT NULL,
  [SnapshotDT] [datetime] NOT NULL,
  [Event] [nvarchar](50) NOT NULL,
  [IsNotified] [bit] NULL CONSTRAINT [DF_tblTFSLabels_IsNotified]  DEFAULT ((0))
) ON [PRIMARY]

GO


