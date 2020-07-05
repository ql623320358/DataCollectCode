using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;

namespace LYSWGuardData
{
    class DbHelperPostgreDB
    {
        public static string pgsqlConnection = "PORT=5432;DATABASE=security_db;HOST=127.0.0.1;PASSWORD=sa123;USER ID=root";
        /// <summary>
        /// select查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet PostgreTable(string sql)
        {
            DataSet ds = new DataSet();
            using (NpgsqlConnection conn = new NpgsqlConnection(pgsqlConnection))
            {

                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
                NpgsqlDataAdapter sda = new NpgsqlDataAdapter(cmd);

                sda.Fill(ds);

            }

            return ds;
        }

        /// <summary>
        /// 执行sql返回是否成功
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static bool PostgreExecuQuery(string StrText)
        {
            bool bools = false;

            using (NpgsqlConnection conn = new NpgsqlConnection(pgsqlConnection))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(StrText, conn);
                int n = cmd.ExecuteNonQuery();
                conn.Close();
                if (n > 0) bools = true;
                else bools = false;
            }

            return bools;
        }

        /// <summary>
        /// 执行sql返回字符
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="pgsqlConnection"></param>
        /// <returns></returns>
        public static string GetpostgreExecuteScalar(string strText, string StrConnection)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(StrConnection))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(strText, conn);
                string PostgreScalar = cmd.ExecuteScalar().ToString();
                if (PostgreScalar != null || !string.IsNullOrEmpty(PostgreScalar))
                    return PostgreScalar;
                else return null;
            }
        }

    }
}
