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
using MySql.Data.MySqlClient;

namespace Souex.Spider.Framework.Helpers.Data
{
    public class MySqlHelper:DbHelper
    {
        internal MySqlHelper(string connStr)
            : base(MySqlClientFactory.Instance, connStr)
        {
            //
        }

        public override int ExecuteNonQuery(string sql, CommandType cmdType, params DbParameter[] cmdParams)
        {
            using (DbConnection conn = this.CreateConnection())
            using (DbCommand cmd = this.CreateCommand(sql, cmdType, cmdParams))
            {
                this.PrepareCmd(cmd, conn);
                return this.ExecuteNonQuery(cmd);
            }
        }

        public override object ExecuteScalar(string sql, CommandType cmdType, params DbParameter[] cmdParams)
        {
            using (DbConnection conn = this.CreateConnection())
            using (DbCommand cmd = this.CreateCommand(sql, cmdType, cmdParams))
            {
                this.PrepareCmd(cmd, conn);
                return this.ExecuteScalar(cmd);
            }
        }

        public override DbDataReader ExecuteReader(string sql, CommandType cmdType, CommandBehavior cmdBehavior, params DbParameter[] cmdParams)
        {
            DbConnection conn = this.CreateConnection();
            DbCommand cmd = this.CreateCommand(sql, cmdType, cmdParams);
            this.PrepareCmd(cmd, conn);
            return this.ExecuteReader(cmd, cmdBehavior);
        }
    }
}
