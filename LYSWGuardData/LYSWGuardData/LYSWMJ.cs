using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.Threading;

namespace LYSWGuardData
{
    public partial class LYSWMJForm : Form
    {
        Thread dbThread;
        bool start = false;
        DbHelperPostgreDB postgredb;
        string strMysqlQuery = "SELECT MAX(event_time) FROM `guard`";
        string strALLMysql = "SELECT * FROM `guard`";
        string strPostsqlQuery = "SELECT dev_alias,event_time,pin,name,dept_name,event_name,event_point_name FROM acc_transaction WHERE event_name like '%开门'";
        
        DataSet dsMySql;
        DataSet dsPostSql;


        public LYSWMJForm()
        {
            InitializeComponent();
            this.tsmiStart.Enabled = true;
            this.tsmiStop.Enabled = true;
        }

        private void tsmiStart_Click(object sender, EventArgs e)
        {
            this.tsmiStart.Enabled = false;
            this.tsmiStop.Enabled = true;
            if (dbThread != null)
            {
                try
                {
                    dbThread.Abort();
                }
                catch { }

                dbThread = null;
            }

            dbThread = new Thread(new ThreadStart(ReadGuardData));
            dbThread.IsBackground = true;
            dbThread.Start();
            start = true;
            MessageBox.Show("开始");
        }

        private void tsmiStop_Click(object sender, EventArgs e)
        {
            
            start = false;
            this.tsmiStart.Enabled = true;
            this.tsmiStop.Enabled = false;
            if (dbThread != null)
            {
                try
                {
                    dbThread.Abort();
                }
                catch { }
                dbThread = null;
            }

            MessageBox.Show("停止");
        }

        private void tsmiRefresh_Click(object sender, EventArgs e)
        {
            DataSet ds=DbHelperMySQL.Query(strALLMysql);

            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    this.dgvGuard.DataSource = ds.Tables[0];
                }
            }
        }


        private void ReadGuardData()
        {
            while (start)
            {

                dsMySql = DbHelperMySQL.Query(strMysqlQuery);//查询mysql中门禁记录中最大时间

                if (dsMySql != null)
                {
                    if (dsMySql.Tables.Count > 0)
                    {
                        if ((dsMySql.Tables[0].Rows.Count > 0) && (!String.IsNullOrEmpty(dsMySql.Tables[0].Rows[0][0].ToString())))//判断是否有数据
                        {
                            string strPostsqlQueryPara = "SELECT dev_alias,event_time,pin,name,dept_name,event_name,event_point_name FROM acc_transaction WHERE event_name like '%开门'  and event_time >'" + dsMySql.Tables[0].Rows[0][0].ToString() + "'";
                            dsPostSql = DbHelperPostgreDB.PostgreTable(strPostsqlQueryPara);//根据mysql表中门禁记录最大时间，查询postgresql中的门禁数据

                            if (dsPostSql != null)
                            {
                                if (dsPostSql.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < dsPostSql.Tables[0].Rows.Count; i++)
                                    {
                                        DbHelperMySQL.ExecuteSql(string.Format("INSERT INTO guard (event_time,person_code,card_code,device_name,status,event_type,verify) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", dsPostSql.Tables[0].Rows[i][1].ToString(), dsPostSql.Tables[0].Rows[i][2].ToString(), dsPostSql.Tables[0].Rows[i][3].ToString(), dsPostSql.Tables[0].Rows[i][0].ToString(), dsPostSql.Tables[0].Rows[i][4].ToString(), dsPostSql.Tables[0].Rows[i][5].ToString(), dsPostSql.Tables[0].Rows[i][6].ToString()));
                                    }

                                }
                            }
                        }
                        else //对门禁表数据为空进行处理
                        {
                            dsPostSql = DbHelperPostgreDB.PostgreTable(strPostsqlQuery);//查询postgresql中的门禁所有数据

                            if (dsPostSql.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < dsPostSql.Tables[0].Rows.Count; i++)
                                {
                                    DbHelperMySQL.ExecuteSql(string.Format("INSERT INTO guard (event_time,person_code,card_code,device_name,status,event_type,verify) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", dsPostSql.Tables[0].Rows[i][1].ToString(), dsPostSql.Tables[0].Rows[i][2].ToString(), dsPostSql.Tables[0].Rows[i][3].ToString(), dsPostSql.Tables[0].Rows[i][0].ToString(), dsPostSql.Tables[0].Rows[i][4].ToString(), dsPostSql.Tables[0].Rows[i][5].ToString(), dsPostSql.Tables[0].Rows[i][6].ToString()));
                                }

                            }
                        }
                    }
                    
                }
               

                Thread.Sleep(15000);
            }
        }




    }
}
