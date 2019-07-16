namespace PressMachineServices.Press
{
    using System;
    /// <summary>
    /// Пресс.
    /// </summary>
    public class PressDataSelected
    {
        /// <summary>
        /// ИД.
        /// </summary>
        public Int32 Id { get; set; }

        public Guid UniqueId { get; set; }

        /// <summary>
        /// Время.
        /// </summary>
        public string DateInsert { get; set; }
    }
}
