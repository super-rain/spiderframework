/*
 * Souex.Spider.Framework,基于规则的采集程序基础框架
 * @version:0.1
 * @author:cheeasy@gmail.com
 * @date:2010-06-01
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Souex.Spider.Framework.Helpers.Data
{
    public class MsSqlHelper:DbHelper
    {
        internal MsSqlHelper(string connStr)
            : base(SqlClientFactory.Instance, connStr)
        {
            //
        }

        public override int ExecuteNonQuery(string sql, CommandType cmdType, params DbParameter[] cmdParams)
        {
            throw new NotImplementedException();
        }

        public override object ExecuteScalar(string sql, CommandType cmdType, params DbParameter[] cmdParams)
        {
            throw new NotImplementedException();
        }

        public override DbDataReader ExecuteReader(string sql, CommandType cmdType, CommandBehavior cmdBehavior, params DbParameter[] cmdParams)
        {
            throw new NotImplementedException();
        }
    }
}
