using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using OPC.Common;
using OPC.Data;
using OPC.Data.Interface;

namespace ZDPress.Opc
{
    public class OpcServerManager
    {
        private OpcServer _theSrv;			// root OPCDA object
        private OpcGroup _theGrp;			// the only one OPC-Group in this example
        private int _itmHandleClient;			// 0 if no current item selected
        private int _itmHandleServer;
        private List<int> _itmHandlerListServer;
        private string _rootname = "Root";			// string of Tree root (dummy)
        public TreeNode<string> OpcNamespacesTree { get; set; }
        public bool OpcConnected;		// flag if connected
        public OPCACCESSRIGHTS ItmAccessRights;
        public TypeCode ItmTypeCode;
        public Action<string> OnServerShutdown;
        private void CallOnServerShutdown(string reason)
        {
            if (OnServerShutdown != null)
            {
                OnServerShutdown(reason);
            }
        }

        public List<OpcServerDescription> SelectServer()
        {

            return new SelServer().GetListAllServers();
        }

        public TypeCode Vt2TypeCode(VarEnum vevt)
        {
            switch (vevt)
            {
                case VarEnum.VT_I1:
                    return TypeCode.SByte;
                case VarEnum.VT_I2:
                    return TypeCode.Int16;
                case VarEnum.VT_I4:
                    return TypeCode.Int32;
                case VarEnum.VT_I8:
                    return TypeCode.Int64;

                case VarEnum.VT_UI1:
                    return TypeCode.Byte;
                case VarEnum.VT_UI2:
                    return TypeCode.UInt16;
                case VarEnum.VT_UI4:
                    return TypeCode.UInt32;
                case VarEnum.VT_UI8:
                    return TypeCode.UInt64;

                case VarEnum.VT_R4:
                    return TypeCode.Single;
                case VarEnum.VT_R8:
                    return TypeCode.Double;

                case VarEnum.VT_BSTR:
                    return TypeCode.String;
                case VarEnum.VT_BOOL:
                    return TypeCode.Boolean;
                case VarEnum.VT_DATE:
                    return TypeCode.DateTime;
                case VarEnum.VT_DECIMAL:
                    return TypeCode.Decimal;
                case VarEnum.VT_CY:				// not supported
                    return TypeCode.Double;
            }

            return TypeCode.Object;
        }

        public void Close()
        {
            if (!OpcConnected)
                return;

            if (_theGrp != null)
            {
                _theGrp.DataChanged -= new DataChangeEventHandler(this.theGrp_DataChange);
                _theGrp.WriteCompleted -= new WriteCompleteEventHandler(this.theGrp_WriteComplete);
                RemoveItem();
                _theGrp.Remove(false);
                _theGrp = null;
            }

            if (_theSrv != null)
            {
                _theSrv.Disconnect();				// should clean up
                _theSrv = null;
            }

            OpcConnected = false;
        }


        public bool DoInit(OpcServerDescription opcServerDescription)
        {
            _theSrv = new OpcServer();

            OpcNamespacesTree = new TreeNode<string>(_rootname);

            // ---------------
            OpcServerConnectResult connectResult = DoConnect(opcServerDescription.ProgId);

            if (!connectResult.IsSuccess)
            {
                return false;
            }

            OpcConnected = true;

            // add event handler for server shutdown
            _theSrv.ShutdownRequested += (e, args) =>
            {
                CallOnServerShutdown(args.shutdownReason);
            };

            // precreate the only OPC group in this example
            if (!CreateGroup())
            {
                return false;
            }

            // browse the namespace of the OPC-server
            if (!DoBrowse())
            {
                return false;
            }
            
            return true;
        }


        private OpcServerConnectResult DoConnect(string progid)
        {
            OpcServerConnectResult result = new OpcServerConnectResult {IsSuccess = false};
            try
            {
                _theSrv.Connect(progid);
                Thread.Sleep(100);
                _theSrv.SetClientName("DirectOPC " + Process.GetCurrentProcess().Id);	// set my client name (exe+process no)

                SERVERSTATUS sts;
                _theSrv.GetStatus(out sts);

                // get infos about OPC server
                StringBuilder sb = new StringBuilder(sts.szVendorInfo, 200);
                sb.AppendFormat(" ver:{0}.{1}.{2}", sts.wMajorVersion, sts.wMinorVersion, sts.wBuildNumber);
                result.ServerInfo = sb.ToString();

                // set status to show server state
                result.SbpTimeStart = DateTime.FromFileTime(sts.ftStartTime);
                result.SbpStatus = sts.eServerState.ToString();
                result.IsSuccess = true;
                return result;
            }
            catch (COMException ex)
            {
                result.ErrorText = ex.Message;
                return result;
            }
        }


        private bool CreateGroup()
        {
            try
            {
                // add our only working group
                _theGrp = _theSrv.AddGroup("OPCdotNET-Group", true, 500);

                //При создании группы тут читаем данные по всем тегам с сервера.
                //OPCItemDef[] i = new OPCItemDef[1];
                //i[0] = new OPCItemDef();
                //i[1] = new OPCItemDef();
                //i[2] = new OPCItemDef();
                //i[3] = new OPCItemDef();
                //i[4] = new OPCItemDef();
                //i[5] = new OPCItemDef();
                //i[6] = new OPCItemDef();
                //i[7] = new OPCItemDef();
                ////i[8] = new OPCItemDef();
               

                //i[0].ItemID = OpcConsts.Position;
                //i[1].ItemID = OpcConsts.PositionSP;
                //i[2].ItemID = OpcConsts.Speed;
                //i[3].ItemID = OpcConsts.SpeedSP;
                //i[4].ItemID = OpcConsts.Power;
                //i[5].ItemID = OpcConsts.PowerSP;
                //i[6].ItemID = OpcConsts.Temperature;
                //i[7].ItemID = OpcConsts.Run;
               
          

                //OPCItemResult[] resI = new OPCItemResult[1];
                //_theGrp.AddItems(i, out resI);

                // add event handler for data changes
                _theGrp.DataChanged += theGrp_DataChange;
                _theGrp.WriteCompleted += theGrp_WriteComplete;
            }
            catch (COMException e)
            {
                throw e;
            }
            return true;
        }



        // event handler: called if any item in group has changed values
        protected void theGrp_DataChange(object sender, DataChangeEventArgs e)
        {
            //Trace.WriteLine("theGrp_DataChange  id=" + e.transactionID + " me=0x" + e.masterError.ToString("X"));

            foreach (OPCItemState s in e.sts.Where(s => s.HandleClient == _itmHandleClient))
            {
                Trace.WriteLine("  item error=0x" + s.Error.ToString("X"));

                if (HRESULTS.Succeeded(s.Error))
                {
                    Trace.WriteLine("  val=" + s.DataValue);
                }
            }
        }

        // event handler: called if asynch write finished
        protected void theGrp_WriteComplete(object sender, WriteCompleteEventArgs e)
        {
            //foreach (OPCWriteResult w in e.res)
            //{
            //    if (w.HandleClient != _itmHandleClient)		// only one client handle
            //        continue;
               
            //}
        }


        private bool DoBrowse()
        {
            try
            {
                OPCNAMESPACETYPE opcorgi = _theSrv.QueryOrganization();
                // fill Tree with all
                OpcNamespacesTree.Clear();
                if (opcorgi == OPCNAMESPACETYPE.OPC_NS_HIERARCHIAL)
                {
                    _theSrv.ChangeBrowsePosition(OPCBROWSEDIRECTION.OPC_BROWSE_TO, "");	// to root
                    RecurBrowse(OpcNamespacesTree, 1);
                }
            }
            catch (COMException ex)
            {
                throw ex;
            }
            return true;
        }


        /// <summary>
        /// recursively call the OPC namespace tree
        /// </summary>
        /// <param name="tnParent">tnParent</param>
        /// <param name="depth">depth</param>
        private void RecurBrowse(TreeNode<string> tnParent, int depth)
        {
            ArrayList lst;

            _theSrv.Browse(OPCBROWSETYPE.OPC_BRANCH, out lst);

            if (lst == null)
            {
                return;
            }

            if (lst.Count < 1)
            {
                return;
            }

            foreach (string s in lst)
            {
                TreeNode<string> tnNext = new TreeNode<string>(s);

                _theSrv.ChangeBrowsePosition(OPCBROWSEDIRECTION.OPC_BROWSE_DOWN, s);

                RecurBrowse(tnNext, depth + 1);

                _theSrv.ChangeBrowsePosition(OPCBROWSEDIRECTION.OPC_BROWSE_UP, "");

                tnParent.AddChild(tnNext);
            }
        }

        //private void treeOpcItems_AfterSelect(string )
        public ArrayList BrowseToNodeInOpc(string nodeFullPath, string targetPath)
        {
            RemoveItem(); // remove item from group
            try
            {
                _theSrv.ChangeBrowsePosition(OPCBROWSEDIRECTION.OPC_BROWSE_TO, "");	// to root                
                if (nodeFullPath.Length > _rootname.Length)		// check if it's only the dummy root
                {
                    
                    //string[] splitpath = "Press2".Split(new char[] { '\t' });	// convert path-string to string-array (separator)
                    string[] splitpath = targetPath.Split(new char[] { '\t' });

                    foreach (string n in splitpath)
                    {
                        _theSrv.ChangeBrowsePosition(OPCBROWSEDIRECTION.OPC_BROWSE_DOWN, n);	// browse to node in OPC namespace
                    }
                }
                // get all items at this node level
                ArrayList lst;
                _theSrv.Browse(OPCBROWSETYPE.OPC_LEAF, out lst);
                return lst;
            }
            catch (Exception ex)
            {
                return new ArrayList();
                //throw ex;// exceptions MUST be handled
            }
        }

         public List<OpcParameter> ProcessOpcNodeParams(ArrayList lst)
         {
             if (lst == null)
             {
                 throw new ArgumentNullException("lst");
             }

             List<OpcParameter> result = new List<OpcParameter>();

             //_theSrv.Browse(OPCBROWSETYPE.OPC_LEAF, out lst);

             if (lst.Count < 1)
             {
                 return result;
             }

             string[] istrs = new string[5];

             OPCProperty[] props;

             OPCPropertyData[] propdata;

             int[] propertyIDs = new int[1];

             //for (int i = 0; i < lst.Count; i++)
             //{
             //    lst[i] = string.Format("PLC.PLC.{0}", lst[i]);
             //}

             // enum+add all item names to ListView
             //string[] itemstrings = new string[2];
             //перебор списка имён переменных
             foreach (string item in lst)
             {
                 OpcParameter parameterData = new OpcParameter();
                 string i = item;
                 parameterData.ParameterName = i;
                 //itemstrings[0] = item;
                 //получение списка свойств переменной
                 _theSrv.QueryAvailableProperties(i, out props);

                 if (props == null)
                 {
                     continue;
                 }

                 // перебираем все свойства
                 foreach (OPCProperty p in props)
                 {
                     // PropertyID == 2 - это свойство в котором хранится значение переменной
                     if (p.PropertyID == 2)
                     {
                         propertyIDs[0] = p.PropertyID;
                         //получение свойства
                         _theSrv.GetItemProperties(i, propertyIDs, out propdata);
                         parameterData.ParameterType = propdata[0].Data.GetType();
                         parameterData.ParameterValue = propdata[0].Data;
                     }
                 }

                 result.Add(parameterData);
             }

             return result;
         }

        public void WriteToOpc(string value) 
        {
            // convert the user text to OPC data type of item
            object[] arrVal = new Object[1];
            arrVal[0] = Convert.ChangeType(value, ItmTypeCode);

            int[] serverhandles = new int[1] { _itmHandleServer };
            int cancelId;
            int[] arrErr;
            _theGrp.Write(serverhandles, arrVal, 9988, out cancelId, out arrErr);
        }

        public void ViewItems(List<string> opcids)
        {
           		// first remove previous item if any

            _itmHandleClient = 1234;  //или 

            RemoveItems();

            List<OPCItemDef> aD = new List<OPCItemDef>();

            int i = 1;
            foreach (string opcid in opcids)
            {
                i++;
                aD.Add(new OPCItemDef(opcid, true, _itmHandleClient + i, VarEnum.VT_EMPTY));  
            }

            OPCItemResult[] arrRes;

            _theGrp.AddItems(aD.ToArray(), out arrRes);

            if (arrRes == null)
            {
                return;
            }

            if (arrRes.Any(ar => ar.Error != HRESULTS.S_OK))
            {
                throw new Exception("arrRes[0].Error != HRESULTS.S_OK");
            }

            _itmHandlerListServer = new List<int>();
            foreach (var item in arrRes)
            {
                _itmHandlerListServer.Add(Convert.ToInt32(item.HandleServer));
            }
            //_itmHandlerListServer = arrRes.OfType<int>().ToList();


            int cancelId;

            _theGrp.Refresh2(OPCDATASOURCE.OPC_DS_DEVICE, 7788, out cancelId);
        }

        public void ViewItem(string opcid)
        {
            RemoveItem();		// first remove previous item if any

            _itmHandleClient = 1234;

            OPCItemDef[] aD = new OPCItemDef[1];

            aD[0] = new OPCItemDef(opcid, true, _itmHandleClient, VarEnum.VT_EMPTY);

            OPCItemResult[] arrRes;

            _theGrp.AddItems(aD, out arrRes);

            if (arrRes == null)
            {
                throw new Exception(string.Format("arrRes == null  opcid:{0}", opcid));
            }

            if (arrRes[0].Error != HRESULTS.S_OK)
            {
                throw new Exception(string.Format("arrRes[0].Error != HRESULTS.S_OK  opcid:{0}", opcid));
            }

            _itmHandleServer = arrRes[0].HandleServer;

            ItmAccessRights = arrRes[0].AccessRights;

            ItmTypeCode = Vt2TypeCode(arrRes[0].CanonicalDataType);


            GrpRefresh2();

            

            
            //else
            //    //txtItemValue.Text = "no read access";

            //    if (ItmTypeCode != TypeCode.Object)				// Object=failed!
            //    {
            //        // check if write is premitted
            //        if ((ItmAccessRights & OPCACCESSRIGHTS.OPC_WRITEABLE) != 0)
            //        { }
            //        //btnItemWrite.Enabled = true;
            //    }
           
        }

        public void GrpRefresh2()
        {
            if ((ItmAccessRights & OPCACCESSRIGHTS.OPC_READABLE) == 0) return;

            int cancelId;

            _theGrp.Refresh2(OPCDATASOURCE.OPC_DS_DEVICE, 7788, out cancelId);
        }


        // remove previous OPC item if any
        private void RemoveItem()
        {
            if (_itmHandleClient == 0) return;

            _itmHandleClient = 0;

            int[] serverhandles = new int[1] { _itmHandleServer };

            int[] remerrors;

            _theGrp.RemoveItems(serverhandles, out remerrors);

            _itmHandleServer = 0;
           
        }

        private void RemoveItems()
        {
            if (_itmHandleClient == 0) return;

            if (_itmHandlerListServer == null || !_itmHandlerListServer.Any()) return;

            _itmHandleClient = 0;

            int[] remerrors;

            _theGrp.RemoveItems(_itmHandlerListServer.ToArray(), out remerrors);

            _itmHandlerListServer = new List<int>();
        }
    }
}
