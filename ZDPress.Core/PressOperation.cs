using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZDPress.Core
{
    /// <summary>
    /// Операция пресования.
    /// </summary>
    public class PressOperation
    {
        /// <summary>
        /// Ид.
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Номер завода.
        /// </summary>
        public string FactoryNumber { get; set; }
        /// <summary>
        /// дата и время начала пресования
        /// </summary>
        public DateTime OperationStart { get; set; }
        /// <summary>
        /// дата и время конца пресования
        /// </summary>
        public DateTime? OperationStop { get; set; }
        /// <summary>
        /// Номер колеса вводится пользователем
        /// </summary>
        public string WheelNumber { get; set; }
        /// <summary>
        /// Номер диаграммы	вводится пользователем
        /// </summary>
        public string DiagramNumber { get; set; }
        /// <summary>
        /// Номер оси вводится пользователем
        /// </summary>
        public string AxisNumber { get; set; }
        /// <summary>
        /// Тип колёсной пары вводится пользователем
        /// </summary>
        public string WheelType { get; set; }
        /// <summary>
        /// Сторона вводится пользователем
        /// </summary>
        public string Side { get; set; }
        /// <summary>
        /// Диаметр подст. Части вводится пользователем
        /// </summary>
        public decimal DWheel { get; set; }
        /// <summary>
        /// Диаметр отв. Ступицы. вводится пользователем
        /// </summary>
        public decimal DAxis { get; set; }
        /// <summary>
        /// Длина ступицы вводится пользователем
        /// </summary>
        public int LengthStup { get; set; }
        /// <summary>
        /// Натяг (рассчитывается)
        /// </summary>
        public decimal Natiag { get; set; }
        /// <summary>
        /// Длина сопряжения (рассчитывается)
        /// </summary>
        public int LengthSopriazh { get; set; }
        /// <summary>
        /// Усилие запрессовки на 100мм	(рассчитывается)
        /// </summary>
        public decimal Power100Mm { get; set; }
        /// <summary>
        /// Макс. Усилие запрессовки (рассчитывается)
        /// </summary>
        public decimal MaxPower { get; set; }
        /// <summary>
        /// Длина прямых участков рассчитывается
        /// </summary>
        public int LengthLines { get; set; }


        public int TotalRows { get; set; }

    }
}
