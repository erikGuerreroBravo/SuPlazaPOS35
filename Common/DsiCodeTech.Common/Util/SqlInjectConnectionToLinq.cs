using System;
using System.Data.SqlClient;
using DsiCodeTech.Common.Constant;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Common.Security;

using static DsiCodeTech.Common.Constant.DsiCodeConst;

namespace DsiCodeTech.Common.Util
{
    public sealed class SqlInjectConnectionToLinq
    {
        private static AesCrypto aesManager = new AesCrypto();

        private string Database { get; set; }

        public SqlInjectConnectionToLinq() { }

        public SqlInjectConnectionToLinq WithNameDatabase(string database)
        {
            this.Database = database;
            return this;
        }

        public string Build() 
        {
            //Ver fluent interface validation
            if (DsiCodeUtil.IsNull(this.Database))
                throw new BusinessException(DsiCodeConst.HANDLE_ERROR_MESSAGE_ID, DsiCodeConst.HANDLE_ERROR_MESSAGE);

            SqlConnectionStringBuilder SqlConnectionStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = @Environment.GetEnvironmentVariable(SYSTEM_ENVIRONMENT_ACCESS_SQL_SERVER, EnvironmentVariableTarget.Machine),
                InitialCatalog = this.Database,
                UserID = aesManager.Decrypt(Environment.GetEnvironmentVariable(SYSTEM_ENVIRONMENT_ACCESS_SQL_USER, EnvironmentVariableTarget.Machine)),
                Password = aesManager.Decrypt(Environment.GetEnvironmentVariable(SYSTEM_ENVIRONMENT_ACCESS_SQL_PASSWORD, EnvironmentVariableTarget.Machine)),
                MultipleActiveResultSets = true,
            };

            return SqlConnectionStringBuilder.ToString();
        }
    }
}
