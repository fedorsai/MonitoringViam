using System;

namespace PressMachineServices.Press
{
    public class PressOperationDataFilter
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public Press Press { get; set; }
    }
}