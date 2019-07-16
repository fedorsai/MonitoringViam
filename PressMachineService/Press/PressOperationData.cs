using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineServices.Press
{
    /// <summary>
    /// 
    /// </summary>
    public class PressOperationData
    {
        /// <summary>
        /// Ид
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Ид
        /// </summary>
        public Guid UniqueID { get; set; }

        /// <summary>
        /// Положение (высота).
        /// </summary>
        public decimal Position { get; set; }

        /// <summary>
        /// Усилие.
        /// </summary>
        public decimal Power { get; set; }

        /// <summary>
        /// Скорость.
        /// </summary>
        public decimal Speed { get; set; }

        /// <summary>
        /// Положение уставка (высота).
        /// </summary>
        public decimal PositionSP { get; set; }

        /// <summary>
        /// Усилие уставка.
        /// </summary>
        public decimal PowerSP { get; set; }

        /// <summary>
        /// Скорость уставка.
        /// </summary>
        public decimal SpeedSP { get; set; }

        /// <summary>
        /// Темпиратура.
        /// </summary>
        public decimal Temperature { get; set; }

        /// <summary>
        /// Дата вставки.
        /// </summary>
        public DateTime DateInsert { get; set; }

        /// <summary>
        /// Работает пресс 1.
        /// </summary>
        public bool Run { get; set; }

        /// <summary>
        /// Press Id
        /// </summary>
        public Press Press { get; set; }
    }
}
