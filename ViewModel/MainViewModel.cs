using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using SSCA.Common;
using SSCA.Model;
using SSCA.Socket;
using SSCA.View;
using SuperSocket.ClientEngine;
using Telerik.Windows;
using Telerik.Windows.Controls;

namespace SSCA.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        //异常or提示信息集合
        private ObservableCollection<ExceptionModel> _exceptionModels = new ObservableCollection<ExceptionModel>();

        //监控点集合 
        private ObservableCollection<YXJK_JKD> _yxjkJkds = new ObservableCollection<YXJK_JKD>();

        //选择的监控点对象
        private YXJK_JKD _selectJkd = new YXJK_JKD();

        //RadMenuItem Click Command
        public DelegateCommand RmcCommand { get; set; }

        //Windows Close Event
        public DelegateCommand ClosedCommand { get; set; }

        //GridView 右键菜单Item Click
        public DelegateCommand GridMenuCommand { get; set; }

        private static bool CanExecute(object o)
        {
            return true;
        }

        private void RadMenuItemClick(object obj)
        {
            if (obj == null)
            {
                return;
            }
            var compara = obj.ToString();
            if (compara.Equals("启动"))
            {
                if (_taskFlag)
                {
                    return;
                }
                _taskFlag = true;
                TaskStart();
                WriteLog("启动", ExEnum.Infor);
            }
            else if (compara.Equals("停止"))
            {
                _taskFlag = false;
                WriteLog("停止", ExEnum.Infor);
            }
        }

        //连接数据库接口
        private IDbConnection _dbc;

        //busy binding
        private bool _isBusy;

        //线程开始结束标识
        private bool _taskFlag;
        //转存redis check binding
        private bool _redisFlag = Settings.RedisFlag;

        public MainViewModel()
        {
            Task.Factory.StartNew(delegate
            {
                try
                {
                    IsBusy = true;
                    InitiCommand();
                    InitiJkd();
                }
                catch (Exception e)
                {
                    WriteLog(e.Message, ExEnum.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        private void TaskStart()
        {
            Parallel.ForEach(YxjkJkds, jkd =>
            {
                Task.Factory.StartNew(async delegate
                {
                    var client = new EasyClient();

                    /***
                     * 初始化socket连接, 接受返回数据处理
                     * HxReceiveFilter为自定义的协议
                     * ***/
                    client.Initialize(new HxReceiveFilter(), (request) =>
                    {
                        try
                        {
                            jkd.JKD_VALUE = request.Key;
                            jkd.CURR_TIME = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                            if (Settings.RedisFlag)
                            {
                                jkd.REDIS_SAVE = RedisManager.SetRedisValue(jkd.JKD_ID, jkd.JKD_VALUE).ToString();
                            }
                            else
                            {
                                jkd.REDIS_SAVE = bool.FalseString;
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteLog(ex.Message, ExEnum.Error);
                        }
                    });
                    // Connect to the server
                    var connected =
                        await client.ConnectAsync(new IPEndPoint(IPAddress.Parse(Settings.RmiIp), Settings.RmiPort));

                    while (_taskFlag)
                    {
                        try
                        {
                            if (connected)
                            {
                                //加密
                                var enStr = DataPacketCodec.Encode($"ri,{jkd.JKD_ID}", Settings.CryptKey) + "#";
                                // Send data to the server
                                client.Send(Encoding.UTF8.GetBytes(enStr));
                            }
                            else
                            {
                                WriteLog($"{jkd.JKD_NAME}Socket连接失败,尝试重新连接...", ExEnum.Error);
                                connected = await client.ConnectAsync(new IPEndPoint(IPAddress.Parse(Settings.RmiIp),
                                    Settings.RmiPort));
                            }
                        }
                        catch (Exception e)
                        {
                            WriteLog(e.Message, ExEnum.Error);
                            // reconnet
                            connected = await client.ConnectAsync(new IPEndPoint(IPAddress.Parse(Settings.RmiIp),
                                Settings.RmiPort));
                        }
                        finally
                        {
                            await Task.Delay(Settings.DelayTime);
                        }
                    }
                    await client.Close();
                    WriteLog($"{jkd.JKD_NAME} socket close", ExEnum.Infor);
                });
            });
        }

        /// <summary>
        /// 初始化Command
        /// </summary>
        private void InitiCommand()
        {
            RmcCommand = new DelegateCommand(RadMenuItemClick, CanExecute);
            ClosedCommand = new DelegateCommand(WindowClosed, CanExecute);
            GridMenuCommand = new DelegateCommand(GridMenuItemClick, CanExecute);
        }

        private void GridMenuItemClick(object obj)
        {
            try
            {
                if (obj == null)
                {
                    return;
                }
                var compara = obj.ToString();
                if (compara.Equals("清空"))
                {
                    ExceptionModels?.Clear();
                }
                else if (compara.Equals("解密"))
                {
                    if (string.IsNullOrEmpty(SelectJkd.JKD_VALUE))
                    {
                        return;
                    }
                    if (SelectJkd.JKD_VALUE.TrimEnd('#').Length == 0)
                    {
                        return;
                    }
                    DeCodeWindow dcw = new DeCodeWindow();
                    var deStr = DataPacketCodec.Decode(SelectJkd.JKD_VALUE.TrimEnd('#'), Settings.CryptKey);
                    dcw.Tb1.Text = deStr;
                    dcw.ShowDialog();
                }
            }
            catch (Exception e)
            {
                WriteLog(e.Message, ExEnum.Error);
            }
        }

        private void WindowClosed(object obj)
        {
            _taskFlag = false;
        }

        /// <summary>
        /// 获取监控点信息
        /// </summary>
        private void InitiJkd()
        {
            try
            {
                _dbc = new OracleConnection(Settings.ConnectStr);
                var jkds = _dbc.Query<YXJK_JKD>(Settings.JkdSql);
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

        public bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        public YXJK_JKD SelectJkd
        {
            get { return _selectJkd; }

            set
            {
                _selectJkd = value;
                OnPropertyChanged("SelectJkd");
            }
        }

        public bool RedisFlag
        {
            get
            {
                return _redisFlag;
            }

            set
            {
                _redisFlag = value;
                Settings.RedisFlag = value;
                OnPropertyChanged("RedisFlag");
                WriteLog($"转存redis参数设置为-{Settings.RedisFlag}", ExEnum.Infor);
            }
        }
    }
}