using System;

namespace ZDPress.Opc
{
    public class OpcParameter
    {
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
        public Type ParameterType { get; set; }
    }
}
