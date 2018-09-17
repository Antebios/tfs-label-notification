# About this project

This project outputs a .NET Core Windows Service that queries a MSSQL Table that is populated from TFS version control labels.  It then sends an email to a list of users with information about the label that was just created.

# Database

## How the data is collected
 In the TFS SQL Server the database has a table that collects TFSVC labels in **tbl\_Labels**.  This table contains a trigger that I created:  trOnLabelDelete and trOnLabelInsert.  As you can see they either trigger on either a record Insert or Delete actions on the table.  They in-turn replicate the data into a table in the database **DevOps_TFSMetrics** and into the table **tblTFSLabels**.

## tblTFSLabels insertion
  When label information is inserted, we cannot insert summary data immediately because join table information is NOT available yet at the time the record is inserted.  If I paused the execution then it would inadvertenly pause the transation, and we want to avoid that.  So, a column called 'IsNotified' is created and a default value of 'False' is set.  It will later be set to 'True' after an email notification has been sent.

## Database source code
  Included in the solution are 5 script files:

  1.tblTFSLabels.sql = creates table in DevOps\_Metrics db.
  
  2.trOnLabelInsert.sql = Trigger in TFS db, on tbl_Labels table that inerts data into DevOps\_Metrics db in tblTFSLabels table.
  
  3.trOnLabelDelete.sql = Trigger in TFS db, on tbl_Labels table that delets data from DevOps\_Metrics db in tblTFSLabels table.
  
  4.spGetLabelQueue.sql = Reads not-notified labels from tblTFSLabels in DevOps\_Metrics db where IsNotified is False.
  
  5.spcGetLabelsByLabelId.sql = Retreives actual Label information from table joins.  Stored in DevOps\_Metrics db.
  
  6.spcSetLabelToNotified.sql = Sets IsNotified to True in table tblTFSLabels in DevOps\_Metrics db.

# Code

## 3rd Party being leveraged

### PeterKottas DotNetCore.WindowsService

[https://github.com/PeterKottas/DotNetCore.WindowsService](https://github.com/PeterKottas/DotNetCore.WindowsService)

This component is very similar to TopShelf.  It allows you to run a Windows Service as either a console or Windows service with the same executable.

### Dapper
 This is used for database communication.

## Hard-coded values
  The database connection string is hard coded in the code file TFSLabelsDataProvider.cs on line #14.  It is assigned to the variable "connectionString".  It is using a MSSQL login.  The account should at least have READ-ONLY access to the TFS database, but dbo to the DevOps\_TFSMetrics database so it can do all the work there.

  The Email Send-To list is in the code file TFSLabelCheck.cs in the method "SendEmail".  In the same code file is the timer for the service, which is set to 30k milliseconds, which is 5-minutes.  So, the service will execute every 5 minutes and check for newly created TFSVC labels.

  TODO:  Create a web-interface so the Send-To list can be dynamically changed via the web-interface and not need to be recompiled.

# To Build

`dotnet build --configuration=release`

`dotnet publish --configuration=release -output dist`

# How to install
  Once you have compiled the source code, you must PUBLISH the project.  Then you take the published output from the "dist" directory that was just created with the published output and copy it to the target server.  Just delete everything in the target directory before copying everything over just to be safe.

  To install the service, open a console and type the command: `dotnet TFSLabelTagNotifcation.dll action:install`.  It will create a service called "Team Foundation Notification Service" (short name is TFSNotificationService), and runs as 'Local System' permission.

# How to monitor
  A log file is created called "log.txt" that looks like this:
```Started
Polling at 2018-09-15T00:46:35.8680911-04:00
********************************************
Starting to process LabelId: 279
Found record for LabelId: 279
Email sent for LabelId: 279
Record marked SENT for LabelId: 279
*********************************************
Polling at 2018-09-15T00:51:36.8014713-04:00
Polling at 2018-09-15T00:56:36.8098830-04:00
```
