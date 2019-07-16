using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace ZDPress.Opc
{
    /// <summary>
    /// Опрашивает ОПЦ сервер с заданным промежутком, результат опроса возвращает в callback.
    /// </summary>
    public class OpcResponder
    {
        private readonly Timer _timer;

        private readonly OpcServerManager _opcServerManager;

        private OpcServerDescription _opcServerDescription;

        private TreeNode<string> _opcNode1;
        private TreeNode<string> _opcNode2;

        public List<string> Parameters1;
        public List<string> Parameters2;
        public List<string> Parameters3;
        public List<string> Parameters4;

        public Action<string> OnServerShutdownAction;

        public Action<List<OpcParameter>> OnReceivedDataAction;


        public void AddParameter(string parameter)
        {
            string par = this.Parameters1.FirstOrDefault(p => p == parameter);

            if (par == null)
            {
                this.Parameters1.Add(parameter);
            }
        }

        public void ClearParameters()
        {
            if (this.Parameters1 != null && this.Parameters1.Any())
            {
                this.Parameters1.Clear();
            }
        }

        public void RemoveParameter(string parameter)
        {
            if (Parameters1 == null || !Parameters1.Any()) return;

            string par = Parameters1.FirstOrDefault(p => p == parameter);

            if (par != null)
            {
                Parameters1.ToList().Remove(parameter);
            }
        }

        public int TimeIntervalInMilliseconds
        {
            get { return Convert.ToInt32(1000); }
            //get { return Convert.ToInt32(ConfigurationManager.AppSettings["OpcRequestInterval"]); }
        }

        public OpcResponder()
        {
            Logger.InitLogger();

            _timer = new Timer(OnTimerTick, null, Timeout.Infinite, Timeout.Infinite);

            _opcServerManager = new OpcServerManager();

            _opcServerManager.OnServerShutdown += OnServerShutdown;
        }


        /// <summary>
        /// Ищет OPC сервер, берет первый найденный сервер.
        /// На сервере берет первый узел. (в дереве) 
        /// </summary>
        /// <returns></returns>
        public void ConfigureProcessor()
        {
            List<OpcServerDescription> servers = _opcServerManager.SelectServer();

            if (servers == null)
            {
                throw new Exception("opc servers is null");
            }

            if (!servers.Any())
            {
                throw new Exception("Не найден OPC сервер");
            }

            _opcServerDescription = servers[0];// TODO: find by name

            _opcServerManager.DoInit(_opcServerDescription);

            if (!_opcServerManager.OpcNamespacesTree.Children.Any())
            {
                throw new Exception("Не найден ни один узел OPC сервера");
            }

            _opcNode1 = _opcServerManager.OpcNamespacesTree.Children.First();// select firt child
            _opcNode2 = _opcServerManager.OpcNamespacesTree.Children[1];
            Parameters1 = new List<string>();
            Parameters1 = _opcServerManager.BrowseToNodeInOpc(_opcNode1.FullPath, "Press2\tApplication\tSCADA").Cast<string>().ToList();

            Parameters2 = _opcServerManager.BrowseToNodeInOpc(_opcNode2.FullPath, "Press1\tApplication\tGVL_TO_SCADA").Cast<string>().ToList();

            Parameters3 = _opcServerManager.BrowseToNodeInOpc(_opcNode2.FullPath, "Press1\tApplication\tPVL").Cast<string>().ToList();
            Parameters4 = _opcServerManager.BrowseToNodeInOpc(_opcNode2.FullPath, "Press1\tApplication\tGVL\tPress").Cast<string>().ToList();

            //if (Parameters1 == null || Parameters1.Count == 0)
            //{
            //    throw new Exception("Не найден ни один параметр");
            //}
            if (Parameters1 != null)
            {
                for (int i = 0; i < Parameters1.Count; i++)
                {
                    Parameters1[i] = string.Format("Press2.Application.SCADA.{0}", Parameters1[i]);
                }  
            }

            if (Parameters2 != null)
            {
                for (int i = 0; i < Parameters2.Count; i++)
                {
                    Parameters2[i] = string.Format("Press1.Application.GVL_TO_SCADA.{0}", Parameters2[i]);
                }

                foreach (string p in Parameters2)
                {
                    Parameters1.Add(p);
                }   
            }

            if (Parameters3 != null)
            {
                foreach (string p in Parameters3.Where(p => OpcConsts.PositionSP1.Contains(p) || OpcConsts.PowerSP1.Contains(p) || OpcConsts.SpeedSP1.Contains(p)))
                {
                    Parameters1.Add(string.Format("Press1.Application.PVL.{0}", p));
                }
            }

            if (Parameters4 != null)
            {
                foreach (string p in Parameters4.Where(p => OpcConsts.Run1.Contains(p)))
                {
                    Parameters1.Add(string.Format("Press1.Application.GVL.Press.{0}", p));
                }   
            }           
        }


        /// <summary>
        /// Вызвать callback и параметром отдать данные с OPC сервера.
        /// </summary>
        /// <param name="parameters"></param>
        private void OnReceivedData(List<OpcParameter> parameters)
        {
            if (OnReceivedDataAction != null)
            {
                OnReceivedDataAction(parameters);
            }
        }
        

        private void OnServerShutdown(string errorText)
        {
            if (OnServerShutdownAction != null)
            {
                OnServerShutdownAction(errorText);
            }
        }


        public void ViewItem(string opcid) 
        {
            _opcServerManager.ViewItem(opcid);
        }

        public void ViewItems(List<string> opcids) 
        {
            _opcServerManager.ViewItems(opcids);
        }



        /// <summary>
        /// Запустить таймер.
        /// </summary>
        public void TimerStart()
        {
            TimerIsRunning = true;

            _timer.Change(TimeIntervalInMilliseconds, Timeout.Infinite);
        }

        /// <summary>
        /// Остановить таймер.
        /// </summary>
        public void TimerStop()
        {
            TimerIsRunning = false;
            OpcServerClose();
            //_timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public bool TimerIsRunning { get; private set; }


        public void OpcServerClose()
        {
            if (_opcServerManager != null)
            {
                _opcServerManager.Close();
            }
        }


        public List<OpcParameter> ProcessParameters(ICollection parameters)
        {
            return _opcServerManager.ProcessOpcNodeParams(new ArrayList(parameters));
        }
                

        private void OnTimerTick(object state)
        {
            try
            {
                DateTime now = DateTime.Now;
               

                int spendToWork = 0;

                _opcServerManager.ViewItems(Parameters1);//обновляем параметры у опц сервера

                List<OpcParameter> parameters = ProcessParameters(Parameters1);//получаем значения параметров

                OnReceivedData(parameters);

                spendToWork = (int)(DateTime.Now - now).TotalMilliseconds;
                
                int nextAfter = TimeIntervalInMilliseconds - spendToWork;

                if (nextAfter < 0)
                {
                   nextAfter = 0;
                }

                if (TimerIsRunning)
                {
                    _timer.Change(nextAfter, Timeout.Infinite);
                }
            }
            catch (Exception ex)
            {
                TimerStop();
                Logger.Log.Error(ex.Message);
                OpcServerClose();
                ConfigureProcessor();
                TimerStart();
                //throw ex;
            }
        }

        private void ViewParametersByOneItem(IEnumerable<string> parameters)
        {
            foreach (string p in parameters)
            {
                _opcServerManager.ViewItem(p);
            }
        }
    }
}
