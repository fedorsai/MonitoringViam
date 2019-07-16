using System;

namespace ZDPress.Opc
{
    public class OpcServerConnectResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorText { get; set; }
        public string ServerInfo { get; set; }
        public DateTime SbpTimeStart { get; set; }
        public string SbpStatus { get; set; }
    }
}
