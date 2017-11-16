using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using SSCA.Model;
using Telerik.Windows.Controls;

namespace SSCA.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        //异常or提示信息集合
        private ObservableCollection<ExceptionModel> _exceptionModels = new ObservableCollection<ExceptionModel>();

        //监控点集合 
        private ObservableCollection<YXJK_JKD> _yxjkJkds = new ObservableCollection<YXJK_JKD>();

        //连接数据库接口
        private IDbConnection _dbc;

        //监控点查询sql
        private const string JkdSql = "SELECT JKD_ID, JKD_NAME, RMI_ID FROM yxjk_jkd where ISRUN = 1";

        public MainViewModel()
        {
            InitiJkd();
            TaskStart();
            WriteLog("启动", ExEnum.Infor);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        private void TaskStart()
        {
            Parallel.ForEach(YxjkJkds, jkd => { Task.Factory.StartNew(delegate { }); });
        }

        /// <summary>
        /// 获取监控点信息
        /// </summary>
        private void InitiJkd()
        {
            try
            {
                _dbc = new OracleConnection(ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString);
                var jkds = _dbc.Query<YXJK_JKD>(JkdSql);
                var yxjkJkds = jkds as IList<YXJK_JKD> ?? jkds.ToList();
                if (!yxjkJkds.Any())
                {
                    MessageBox.Show("无换热站");
                    return;
                }
                YxjkJkds = new ObservableCollection<YXJK_JKD>(yxjkJkds);

                #region 测试用

                //                SoureJkds.Clear();
                //                for (int i = 0; i < 100; i++)
                //                {
                //                    SoureJkds.Add(new YXJK_JKD() {JKD_ID = "tyzx"+i, JKD_NAME = "体验中心"+i});
                //                }

                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, ExEnum.Error);
            }
        }

        /// <summary>
        /// 添加log到集合中显示
        /// </summary>
        /// <param name="paramStr">log信息</param>
        /// <param name="paramLevel">log级别</param>
        public void WriteLog(string paramStr, ExEnum paramLevel)
        {
            string level = string.Empty;
            if (paramLevel == ExEnum.Infor)
            {
                level = "提示";
            }
            if (paramLevel == ExEnum.Error)
            {
                level = "异常";
            }
            if (!string.IsNullOrEmpty(level))
            {
                ExceptionModels.Add(new ExceptionModel()
                {
                    ExTime = DateTime.Now,
                    ExLevel = level,
                    ExMessage = paramStr
                });
            }
        }

        //异常or提示信息集合
        public ObservableCollection<ExceptionModel> ExceptionModels
        {
            get { return _exceptionModels; }

            set
            {
                _exceptionModels = value;
                OnPropertyChanged("ExceptionModels");
            }
        }

        //监控点集合 
        public ObservableCollection<YXJK_JKD> YxjkJkds
        {
            get { return _yxjkJkds; }

            set
            {
                _yxjkJkds = value;
                OnPropertyChanged("YxjkJkds");
            }
        }
    }
}