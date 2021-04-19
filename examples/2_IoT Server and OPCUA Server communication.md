## Example 2 (OPCUA communication):

The IoT server is able to communicate with the OPCUA server using OnboardTask. 

### Prerequisites:

  - OPCUA.Library from nuget.org
  - Test OPCUA server (in example: OPC UA Simulator Server (www.prosysopc.com))

### Code:
    
        //First step add OPCUA.Library nuget to your project

        using OPCUA.Library;

        #region Helpers
        //.....
        Realtime RObj = new Realtime();
        OPCUAClient OPCUAClient = new OPCUAClient();
        #endregion

        private static BackgroundTaskDeferral _Deferral = null;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _Deferral = taskInstance.GetDeferral();

            EventLogging.Initialize();
            EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Start initializing...");
            
            Init();
        }
        private async void Init()      //Initialize service
        {
            bool error = false;

            #region Login to Xserver.IoT Service
            var res = await Authentication.Login("operator", "operator");
            if (res.Success == false)
            {
                EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - " + res.ErrorMessage);
                error = true;
            }
            #endregion

            #region Gets List of Sources and Quantities
            var result = await RObj.GetSourcesQuantities();
            if (result.Success == false)
            {
                EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - " + result.ErrorMessage);
                error = true;
            }
            #endregion

            #region Connect to the OPCUA Server
            var certificateFile = await Package.Current.InstalledLocation.GetFileAsync(@"Client.Uwp.pfx");
            OPCUAClient.CertificateFilePath = certificateFile.Path;
            OPCUAClient.ServerAddress = "opc.tcp://COMPUTERNAME:53530/OPCUA/SimulationServer";
            var resopcua = OPCUAClient.Connect();
            if (resopcua.Success == false)
            {
                EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - " + resopcua.ErrorMessage);
                error = true;
            }
            #endregion

            #region Initialize and Start IoT OnboardTask
            OnboardTaskHandler.WaitingTime = TaskHandlerPeriod;
            OnboardTaskHandler.ThresholdReached += OnboardTask;
            OnboardTaskHandler.Run();
            #endregion

            EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Finished initialization.");
        }
       
        /// IoT Onboard Task
        private async void OnboardTask(object sender, EventArgs e)
        {
            try
            {
                //Todo: Type your onboard task code here
                
                //Reads OPCUA nodes example
                List<OPCReadNode> OPCNodes = new List<OPCReadNode>();

                OPCReadNode onenode = new OPCReadNode();

                onenode.Name = "Counter";
                onenode.NodeId = "ns=3;s=Counter";
                OPCNodes.Add(onenode);

                var result = await OPCUAClient.ReadValues(OPCNodes);
            }
            catch (Exception ex)
            {
                EventLogging.AddLogMessage(MessageType.ExceptionError, this.GetType().Name + " - " + ServiceDisplayName + " - " + "OnboardTask exception error! Error: " + ex.Message);
            }
            OnboardTaskHandler.Run();  //Task continues to run
        }
        
