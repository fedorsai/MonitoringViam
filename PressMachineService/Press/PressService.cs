namespace PressMachineServices.Press
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PressMachineServices.Database;
    using ZDPress.Opc;


    /// <inheritdoc>
    public class PressService : IPressService
    {
        /// <summary>
        /// База данных PressMachineModelContainer.
        /// </summary>
        private readonly PressMachineDbContex _dbContex;

        /// <summary>
        ///     Инициализирует новый экземпляр класса <see cref="ConfidantService" />.
        /// </summary>
        /// <param name="docflowDatabase">База данных Docflow.</param>
        public PressService(PressMachineDbContex pressMachineModelContainer)
        {
            this._dbContex = pressMachineModelContainer;
        }


        /// <inheritdoc/>
        public List<Press> GetPresses()
        {
            return this._dbContex.Presses.ToList();
        }

        /// <inheritdoc/>
        public void SavePressOperationDataList(List<PressOperationData> data)
        {
            if (data != null)
            {
                this._dbContex.PressOperationDatas.AddRange(data);

                this._dbContex.SaveChanges();
            }
        }

        /// <inheritdoc/>
        public List<PressOperationData> GetPressOperationData(PressOperationDataFilter filter)
        {
            if (filter == null)
            {
                return this._dbContex.PressOperationDatas.ToList();
            }

            if (filter.Press == null)
            {
                throw new ArgumentException("filter.Press == null");
            }

            return this._dbContex.PressOperationDatas.Where(op => op.DateInsert >= filter.From && op.DateInsert < filter.To && op.Press.Id == filter.Press.Id).ToList();
        }
    }
}
