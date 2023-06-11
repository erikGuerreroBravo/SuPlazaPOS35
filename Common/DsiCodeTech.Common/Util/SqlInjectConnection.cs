using System;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using DsiCodeTech.Common.Constant;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Common.Security;

using static DsiCodeTech.Common.Constant.DsiCodeConst;

namespace DsiCodeTech.Common.Util
{
    public sealed class SqlInjectConnection
    {
        private static AesCrypto aesManager = new AesCrypto();

        private string Database { get; set; }

        private string Metadata { get; set; }

        public SqlInjectConnection() { }


        public SqlInjectConnection WithNameDatabase(string database)
        {
            this.Database = database;
            return this;
        }

        public SqlInjectConnection WithMetadata(string metadata)
        {
            this.Metadata = metadata;
            return this;
        }

        public string Build()
        {
            //Ver fluent interface validation
            if (DsiCodeUtil.IsNull(this.Database, this.Metadata))
                throw new BusinessException(DsiCodeConst.HANDLE_ERROR_MESSAGE_ID, DsiCodeConst.HANDLE_ERROR_MESSAGE);

            SqlConnectionStringBuilder SqlConnectionStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = @Environment.GetEnvironmentVariable(SYSTEM_ENVIRONMENT_ACCESS_SQL_SERVER, EnvironmentVariableTarget.Machine),
                InitialCatalog = this.Database,
                UserID = aesManager.Decrypt(Environment.GetEnvironmentVariable(SYSTEM_ENVIRONMENT_ACCESS_SQL_USER, EnvironmentVariableTarget.Machine)),
                Password = aesManager.Decrypt(Environment.GetEnvironmentVariable(SYSTEM_ENVIRONMENT_ACCESS_SQL_PASSWORD, EnvironmentVariableTarget.Machine)),
                MultipleActiveResultSets = true,
            };

            EntityConnectionStringBuilder entityConnectionStringBuilder = new EntityConnectionStringBuilder()
            {
                Provider = "System.Data.SqlClient",
                Metadata = this.Metadata,
                ProviderConnectionString = SqlConnectionStringBuilder.ToString()
            };

            return entityConnectionStringBuilder.ConnectionString;
        }
    }
}
