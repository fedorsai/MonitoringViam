using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OPC.Common;

namespace ZDPress.Opc
{
    public class SelServer
    {
        public List<OpcServerDescription> GetListAllServers()
        {
            List<OpcServerDescription> result = new List<OpcServerDescription>();

            OpcServerList lst = new OpcServerList();

            OpcServers[] svs = null;

            lst.ListAllData20(out svs);

            if (svs == null)
            {
                return result;
            }

            string[] itemstrings = new string[3];
            foreach (OpcServers l in svs)
            {
                itemstrings[0] = l.ServerName;
                itemstrings[1] = l.ProgID;
                itemstrings[2] = l.ClsID.ToString();
                result.Add(new OpcServerDescription
                {
                    ServerName = l.ServerName,
                    ProgId = l.ProgID,
                    ClsId = l.ClsID
                });
            }
            return result;
        }

    }
}
