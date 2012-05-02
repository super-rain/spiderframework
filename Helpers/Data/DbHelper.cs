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

namespace Souex.Spider.Framework.Helpers.Data
{
    /// <summary>
    /// 为数据库访问提供抽象基类
    /// </summary>
    public abstract class DbHelper
    {
        protected DbProviderFactory DbFactory;
        protected string ConnectionString;

        private DbHelper()
        {
            this.ConnectionString = "";
        }

        protected DbHelper(DbProviderFactory factory, string connStr)
            : this()
        {
            this.DbFactory = factory;
            this.ConnectionString = connStr;
        }

        #region ExecuteNonQuery
        protected int ExecuteNonQuery(DbCommand cmd)
        {
            int n = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return n;
        }

        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, CommandType.Text, null);
        }

        public int ExecuteNonQuery(string sql, CommandType cmdType)
        {
            return ExecuteNonQuery(sql, cmdType, null);
        }

        /// <summary>
        /// 在派生类中重写此方法，实现ExecuteNonQuery
        /// </summary>
        /// <param name="sql">SQL命令</param>
        /// <param name="cmdType">CommandType枚举值之一</param>
        /// <param name="cmdParams">SQL命令参数</param>
        /// <returns>执行SQL命令所影响的行数</returns>
        public abstract int ExecuteNonQuery(string sql, CommandType cmdType, params DbParameter[] cmdParams);
        #endregion


        #region ExecuteScalar
        protected object ExecuteScalar(DbCommand cmd)
        {
            object obj = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return obj;
        }
        
        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, CommandType.Text, null);
        }

        public object ExecuteScalar(string sql, CommandType cmdType)
        {
            return ExecuteScalar(sql, cmdType, null);
        }

        /// <summary>
        /// 在派生类中重写此方法，实现ExecuteScalar
        /// </summary>
        /// <param name="sql">SQL命令</param>
        /// <param name="cmdType">CommandType枚举值之一</param>
        /// <param name="cmdParams">SQL命令参数</param>
        /// <returns>返回第一条记录，第一个字段的值</returns>
        public abstract object ExecuteScalar(string sql, CommandType cmdType, params DbParameter[] cmdParams);
        #endregion


        #region ExecuteReader
        protected DbDataReader ExecuteReader(DbCommand cmd, CommandBehavior cmdBehavior)
        {
            DbDataReader reader = cmd.ExecuteReader(cmdBehavior);
            cmd.Parameters.Clear();
            return reader;
        }
        public DbDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, CommandType.Text, CommandBehavior.Default, null);
        }
        public DbDataReader ExecuteReader(string sql, CommandType cmdType, params DbParameter[] cmdParams)
        {
            return ExecuteReader(sql, cmdType, CommandBehavior.Default, cmdParams);
        }

        public DbDataReader ExecuteReader(string sql, CommandType cmdType)
        {
            return ExecuteReader(sql, cmdType, CommandBehavior.Default, null);
        }

        public DbDataReader ExecuteReader(string sql, CommandBehavior cmdBehavior)
        {
            return ExecuteReader(sql, CommandType.Text, cmdBehavior, null);
        }

        public DbDataReader ExecuteReader(string sql, CommandBehavior cmdBehavior, params DbParameter[] cmdParams)
        {
            return ExecuteReader(sql, CommandType.Text, cmdBehavior, cmdParams);
        }

        /// <summary>
        /// 在派生类中重写此方法，实现ExecuteReader
        /// </summary>
        /// <param name="sql">SQL命令</param>
        /// <param name="cmdType">CommandType枚举值之一</param>
        /// <param name="cmdBehavior">指示CommandBehavior,CommandBehavior枚举值之一</param>
        /// <param name="cmdParams">SQL命令参数</param>
        /// <returns>DbDataReader实例或NULL</returns>
        public abstract DbDataReader ExecuteReader(string sql, CommandType cmdType, CommandBehavior cmdBehavior, params DbParameter[] cmdParams);
        #endregion

        /// <summary>
        /// 创建连接对象
        /// </summary>
        /// <returns>DbConnection</returns>
        protected DbConnection CreateConnection()
        {
            DbConnection conn= this.DbFactory.CreateConnection();
            conn.ConnectionString = this.ConnectionString;
            return conn;
        }

        /// <summary>
        /// 根据给定cmdText,创建DbCommand实例
        /// </summary>
        /// <param name="cmdText">命令语句</param>
        /// <returns>DbCommand实例</returns>
        public DbCommand CreateCommand(string cmdText)
        {
            return CreateCommand(cmdText, CommandType.Text, null);
        }

        /// <summary>
        /// 根据给定cmdText和cmdType,创建DbCommand实例
        /// </summary>
        /// <param name="cmdText">命令语句</param>
        /// <param name="cmdType">命令类型,CommandType枚举值之一</param>
        /// <returns>DbCommand实例</returns>
        public DbCommand CreateCommand(string cmdText, CommandType cmdType)
        {
            return CreateCommand(cmdText, cmdType, null);
        }

        /// <summary>
        /// 根据给定cmdText和cmdType，以及cmdParams所指定的参数,创建DbCommand实例
        /// </summary>
        /// <param name="cmdText">命令语句</param>
        /// <param name="cmdType">命令类型,CommandType枚举值之一</param>
        /// <param name="cmdParams">命令参数</param>
        /// <returns>DbCommand实例</returns>
        public DbCommand CreateCommand(string cmdText, CommandType cmdType, params DbParameter[] cmdParams)
        {
            DbCommand cmd = this.DbFactory.CreateCommand();
            if (null != cmdText && cmdText != "")
            {
                cmd.CommandText = cmdText;
            }
            cmd.CommandType = cmdType;
            if (null != cmdParams)
            {
                for (int i = 0; i < cmdParams.Length; i++)
                {
                    if (cmdParams[i] != null)
                    {
                        cmd.Parameters.Add(cmdParams[i]);
                    }
                }
            }
            return cmd;
        }

        /// <summary>
        /// 预处理DbCommand，将DbCommand的Connection实例设置为conn参数所表示的实例，并且打开此连接
        /// </summary>
        /// <param name="cmd">DbCommand实例</param>
        /// <param name="conn">DbConnection实例</param>
        protected void PrepareCmd(DbCommand cmd, DbConnection conn)
        {
            PrepareCmd(cmd, conn, null);
        }

        protected void PrepareCmd(DbCommand cmd, DbConnection conn, DbTransaction trans)
        {
            if (null == cmd || null == conn)
            {
                throw new ArgumentNullException(Properties.DbResource.DbCmdNull);
            }

            if (null == cmd.Connection)
            {
                cmd.Connection = conn;
            }

            if (null == cmd.Connection)
            {
                throw new ArgumentNullException(Properties.DbResource.DbConnNull);
            }

            if (cmd.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }

            if (null != trans)
            {
                cmd.Transaction = trans;
            }

            cmd.Prepare();
        }

        /// <summary>
        /// 创建MySqlHelper实例,如 MySqlHelper mysql=DbHelper.CreateMySqlHelper('...') as MySqlHelper;
        /// </summary>
        /// <param name="connectionString">ConnectionString</param>
        /// <returns>MySqlHelper</returns>
        public static DbHelper CreateMySqlHelper(string connectionString)
        {
            return new MySqlHelper(connectionString);
        }

        /// <summary>
        /// 创建MsSqlHelper实例,如 MsSqlHelper mssql=DbHelper.CreateMsSqlHelper('...') as MsSqlHelper;
        /// </summary>
        /// <param name="connectionString">ConnectionString</param>
        /// <returns>MsSqlHelper</returns>
        public static DbHelper CreateMsSqlHelper(string connectionString)
        {
            return new MsSqlHelper(connectionString);
        }
    }
}
