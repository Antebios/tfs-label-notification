USE [DevOps_TFSMetrics]
GO
/****** Object:  StoredProcedure [dbo].[spGetLabelsByProject]    Script Date: 9/14/2018 4:32:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spGetLabelsByLabelId]
  @LabelId INT
AS
    SELECT --l.PartitionId, l.LabelScopeDataspaceId, l.ItemId, 
            l.LabelId, l.LabelName,l.Comment,
            --tv.FullPath, 
      --tv.ParentPath,
      --tv.ChildItem,
      max(tv.VersionFrom) as Changeset,
            --v.FullPath as ServerItem, -- v.FullPath --, v.*
            --l.Comment,
            --l.OwnerId AS OwnerId,
            --l.LastModified,
            --l.LabelId
      p.project_name as ProjectName,
      RIGHT(tv.ParentPath,LEN(tv.ParentPath) - charindex('\', tv.ParentPath, 3)) + LEFT(tv.ChildItem, LEN(tv.ChildItem)-1) as Path,
      l.LastModified,
      ADObjects.SamAccountName as [User]
    FROM    [DevOps_TFSMetrics].[dbo].[tblTFSLabels] l
        INNER JOIN    [Tfs_Instance].[dbo].[tbl_Version] v -- WITH (FORCESEEK(IX_tbl_Version_ItemId_VersionFrom(PartitionId, ItemId, ItemDataspaceId, VersionFrom)))
            --on l.PartitionId = v.PartitionId and l.ItemId = v.ItemId and l.LabelScopeDataspaceId = v.ItemDataspaceId and
        ON      v.PartitionId = 1
          AND v.PartitionId = l.PartitionId
            AND v.ItemDataspaceId = l.LabelScopeDataspaceId
            AND v.ItemId = l.ItemId --ItemId of the labelScope.
            --AND @versionLatest BETWEEN v.VersionFrom AND v.VersionTo
    inner join [Tfs_Instance].[dbo].[tbl_LabelEntry] le
      on le.PartitionId = l.PartitionId and le.ItemDataspaceId = l.LabelScopeDataspaceId and le.LabelId = l.LabelId
    inner join [Tfs_Instance].[dbo].[tbl_Version] tv
    on tv.PartitionId=le.PartitionId and tv.ItemDataspaceId=le.ItemDataspaceId and tv.ItemId=le.ItemId
    inner join [Tfs_Instance].[dbo].[tbl_projects] p on p.project_id = REPLACE(REPLACE(LEFT(v.FullPath, LEN(v.FullPath)-1), '$\',''), '"','-')
    inner join [Tfs_Instance].[dbo].[ADObjects] on ADObjects.TeamFoundationId = l.OwnerId

    WHERE   l.PartitionId = 1
            /*
      AND (
                v.FullPath LIKE @labelScope + '%'
                OR @labelScope LIKE v.FullPath + '%'
            )
      */
            --AND l.LabelName LIKE @labelLike
            --AND l.OwnerId = ISNULL(@ownerId, l.OwnerId)
      and tv.VersionFrom <= le.VersionFrom
      and v.FullPath <> '$\'
      and l.LabelId = @LabelId
    
  GROUP BY l.LabelId, l.LabelName,l.Comment, p.project_name, 
           RIGHT(tv.ParentPath,LEN(tv.ParentPath) - charindex('\', tv.ParentPath, 3)) + LEFT(tv.ChildItem, LEN(tv.ChildItem)-1), l.LastModified, ADObjects.SamAccountName
    ORDER BY l.LastModified, p.project_name, LabelName
GO
