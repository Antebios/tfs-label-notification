using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace TFSLabelTagNotifcation
{
    public class TFSLabelsDataProvider
    {
        // Set your database connection string here, or put it into a config file to recall it.
        private readonly string connectionString = "Server=sqlserver.name.goes.here\\sqlinstance;Database=DevOps_TFSMetrics; User ID=sql_login; Password=sql_password";

        //public string GetConnection()
        //{
        //    var connection = _configuration.GetSection("ConnectionStrings").GetSection("TFSMetricsDatabase").Value;
        //    return connection;
        //}

        public IEnumerable<RawLabelsModel> GetTFSLabels()
        {
            //var connectionString = this.GetConnection();
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                return sqlConnection.Query<RawLabelsModel>("spcGetLabelQueue", null, commandType: CommandType.StoredProcedure);
            }
        }


        
        public FinalLabelsModel GetLabelRecord(int LabelId)
        {
            //var connectionString = this.GetConnection();
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@LabelId", LabelId);

                return sqlConnection.QuerySingle<FinalLabelsModel>("spGetLabelsByLabelId",
                    dynamicParameters,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public void MarkAsSent(int LabelId)
        {
            //var connectionString = this.GetConnection();
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@LabelId", LabelId);

                sqlConnection.Execute("spcSetLabelToNotified", 
                    dynamicParameters, 
                    commandType: CommandType.StoredProcedure);
            }
        }

    }

}
