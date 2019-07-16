namespace PressMachineServices.Opc
{
    using System;
    using System.Collections.Generic;
    using PressMachineServices.Database;
    using PressMachineServices.Press;
    using ZDPress.Opc;
    using System.Linq;

    /// <summary>
    /// Логика работы с OPC сервером.
    /// </summary>
    public class OpcService : IOpcService
    {
        /// <summary>
        /// Опрашивает OPC сервер.
        /// </summary>
        private OpcResponder _opcResponder;
        private bool lastRun;

        private Guid uniqueID;
        /// <summary>
        /// База данных PressMachineModelContainer.
        /// </summary>
        private readonly PressMachineDbContex _dbContex;

        private Press press1;

        private Press press2;

        /// <summary>
        /// Инициализирует новфй эклземпляр класса <see cref="OpcService"/>
        /// </summary>
        /// <param name="dbContex"></param>
        public OpcService(PressMachineDbContex dbContex)
        {
            this._dbContex = dbContex;

            this._opcResponder = new OpcResponder() ;

            this._opcResponder.OnReceivedDataAction += this.M;

            this.GetPresses();
        }


        private void GetPresses()
        {
           press1 = this._dbContex.Presses.Where(p => p.Id == 1).FirstOrDefault();
           press2 = this._dbContex.Presses.Where(p => p.Id == 2).FirstOrDefault();
        }

        private List<PressOperationData> _batch = new List<PressOperationData>(); // todo подумать как лучше сохранять.

        private void M(List<OpcParameter> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            PressOperationData data = this.ConvertToPressDataItem(parameters);

            if (data.Run && !lastRun)
            {
                uniqueID = Guid.NewGuid();
                
            }

            lastRun = data.Run;
            
            if (data.Run)
            {
                data.UniqueID = uniqueID;
                this._batch.Add(data);

                if (this._batch.Count >= 5) // TODO: можно не сохранить последнюю пачку...
                {
                    this._dbContex.PressOperationDatas.AddRange(this._batch);
                    this._dbContex.SaveChanges();
                    this._batch.Clear();
                }
                
            }
        }


        /// <inheritdoc/>
        public PressOperationData ConvertToPressDataItem(List<OpcParameter> parameters)
        {
            PressOperationData item = new PressOperationData();

            bool run1 = parameters.FirstOrDefault(p => p.ParameterName ==  OpcConsts.Run1) != null ? 
                Convert.ToBoolean(parameters.FirstOrDefault(p => p.ParameterName ==  OpcConsts.Run1).ParameterValue)  : false;
            bool run2 = parameters.FirstOrDefault(p => p.ParameterName == OpcConsts.Run2) != null ?
                Convert.ToBoolean(parameters.FirstOrDefault(p => p.ParameterName == OpcConsts.Run2).ParameterValue) : false;           

            if (run1)
            {
                parameters.ForEach(p => InitInternal1(p, item));
                item.DateInsert = DateTime.Now;
                item.Press = press1;
            }

            if (run2)
            {
                parameters.ForEach(p => InitInternal2(p, item));
                item.DateInsert = DateTime.Now;
                item.Press = press2;                
            }

            item.Run = run1 || run2;

            return item;
        }

          /// <summary>
        /// На основе OpcParameter инициализирует PressOperationData.
        /// </summary>
        /// <param name="parameter">OpcParameter</param>
        /// <param name="item">PressOperationData</param>
        private void InitInternal1(OpcParameter parameter, PressOperationData item)
        {
            if (parameter == null)
            {
                return;
            }

            dynamic val = Convert.ChangeType(parameter.ParameterValue, parameter.ParameterType);

            if (parameter.ParameterName == OpcConsts.Position1)
            {
                item.Position = Convert.ToDecimal(val);
            }
            if (parameter.ParameterName == OpcConsts.Power1)
            {
                item.Power = Convert.ToDecimal(val);
            }
            
            if (parameter.ParameterName == OpcConsts.Speed1)
            {
                item.Speed = Convert.ToDecimal(val);
            }

            if (parameter.ParameterName == OpcConsts.Temperature1)
            {
                item.Temperature = Convert.ToDecimal(val);
            }
            if (parameter.ParameterName == OpcConsts.PositionSP1)
            {
                item.PositionSP = Convert.ToDecimal(val);
            }
            if (parameter.ParameterName == OpcConsts.PowerSP1)
            {
                item.PowerSP = Convert.ToDecimal(val);
            }

            if (parameter.ParameterName == OpcConsts.SpeedSP1)
            {
                item.SpeedSP = Convert.ToDecimal(val);
            }
        }

        /// <summary>
        /// На основе OpcParameter инициализирует PressOperationData.
        /// </summary>
        /// <param name="parameter">OpcParameter</param>
        /// <param name="item">PressOperationData</param>
        private void InitInternal2(OpcParameter parameter, PressOperationData item)
        {
            if (parameter == null)
            {
                return;
            }

            dynamic val = Convert.ChangeType(parameter.ParameterValue, parameter.ParameterType);

            if (parameter.ParameterName == OpcConsts.Position2)
            {
                item.Position = Convert.ToDecimal(val);
            }
            if (parameter.ParameterName == OpcConsts.Power2)
            {
                item.Power = Convert.ToDecimal(val);
            }

            if (parameter.ParameterName == OpcConsts.Speed2)
            {
                item.Speed = Convert.ToDecimal(val);
            }

            if (parameter.ParameterName == OpcConsts.PositionSP2)
            {
                item.PositionSP = Convert.ToDecimal(val);
            }
            if (parameter.ParameterName == OpcConsts.PowerSP2)
            {
                item.PowerSP = Convert.ToDecimal(val);
            }

            if (parameter.ParameterName == OpcConsts.SpeedSP2)
            {
                item.SpeedSP = Convert.ToDecimal(val);
            }

            if (parameter.ParameterName == OpcConsts.Temperature2)
            {
                item.Temperature = Convert.ToDecimal(val);
            }
        }

        public void ConfigureProcessor()
        {
            this._opcResponder.ConfigureProcessor();
        }

        public void Run()
        {
            this._opcResponder.TimerStart();
        }

        public void Stop()
        {
            this._opcResponder.TimerStop();
        }
    }
}
