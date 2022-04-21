## IoTServerInsertionAPI helps to easily create custom interfaces to IoT Server for various services (Google Cloud, AWS, IBM Cloud, My SQL, Oracle, REST API services, etc.).

# Getting Started

## IoTServerInsertionAPI.LogHelpers

### Logging class

#### Properties:

    /// Prefetch realtime data (seconds) Default value: 45, Range: 30-55
    public int PreReadSecond

    /// Maximum reading density (minutes) Default value: 1, Range: 1-60 - Do not use more frequent data.
    public int DensityMaxFreq

    /// One communication package size Default value: 100, Range: 1-10000 - Specifies the number of items during data transmission.
    public int PackageSize

#### Methods:

    /// Reads Realtime values to LogPuffer
    public async Task<Result> ReadRealtimeValues(Realtime FieldDevices)

    /// Returns the number of items in the LogPuffer.
    public int LogPufferCount()

    ///Deletes all items.
    public void ClearLogPuffer()

    /// Gets LogPuffer cloneobject
    public List<LogPuffer.LogItem> GetLogPuffer()

    /// Deletes items from LogPuffer  
    public void DeleteLogPuffer(List<LogPuffer.LogItem> LogItems)

    /// LogPuffer to Onboard storage - Copies the item number specified in PackageSize from the LogPuffer to Onboard storage. LogPuffer is in memory, OnboardStorage is in storage.
    public async Task<Result> LogToOnboardStorage()

    /// Gets the Onboard Storage log file for sending (if error return null)  
    public async Task<IStorage> GetOnboardStorageLogFile()

    /// Gets OnboardStorage logfile number  
    public async Task<int> GetOnboardStorageLogFileNumber()

    /// Deletes a logfile from Onboard storage   
    public async Task<Result> DeleteOnboardStorage(string StorageName)

#### Example:

##### Requirements: 

IoTServerInsertionAPI nuget
XServerIoTOnboardTaskProject from GitHub

##### Code:

    ...
    //Task Handler Period (ms)
    private const int TaskHandlerPeriod = 1000;
    private const int PufferTaskHandlerPeriod = 5000;
    ...

    TaskHandler OnboardTaskHandler = new TaskHandler();
    TaskHandler PufferTaskHandler = new TaskHandler();
    IoTServerInsertionAPI.LogHelpers.Logging InsertionAPI = new IoTServerInsertionAPI.LogHelpers.Logging();
    ...

    public async void Run(IBackgroundTaskInstance taskInstance)
    {
        _Deferral = taskInstance.GetDeferral();

        await EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Start initializing...");

        #region Check the running of services (Data,Com), after starting the IoT device
        bool exit = false;
        while (exit == false)
        {
            var com = await Services.ComIsInitialized();
            var data = await Services.DataIsInitialized();
            if (com.Initialized == true && data.Initialized == true)
            {
                exit = true;
            }
            await Task.Delay(5000);
        }
        #endregion

        #region Login to Xserver.IoT Service
        var res = await Authentication.Login("onboardtask", "onboardtask");
        if (res.Success == false)
        {
            await EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Authentication error! " + res.ErrorMessage);
        }
        #endregion

        #region OnboardTask Config
        var onboardconfig = await XserverIoTCommon.OnboardTask.GetConfig();
        if (string.IsNullOrEmpty(onboardconfig.Content) == false)
        {
            DexConf = JsonConvert.DeserializeObject<DexmaConfig>(onboardconfig.Content);
        }
        else
        {
            string content = JsonConvert.SerializeObject(DexConf);
            await XserverIoTCommon.OnboardTask.SaveConfig(content);
        }
        #endregion

        #region Set Insertion API
        InsertionAPI.DensityMaxFreq = 5; //Minutes
        #endregion
        
        ...

        //Initialize and Start IoT OnboardTask
        OnboardTaskHandler.WaitingTime = TaskHandlerPeriod;
        OnboardTaskHandler.ThresholdReached += OnboardTask;
        OnboardTaskHandler.Run();

        //Initialize and Start LogPuffer Task
        PufferTaskHandler.WaitingTime = PufferTaskHandlerPeriod;
        PufferTaskHandler.ThresholdReached += PufferTask;
        PufferTaskHandler.Run();

        await EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Finished initialization.");
    }

    bool FailedTogle = false;
    private async void OnboardTask(object sender, EventArgs e)
    {
        try
        {
            var result = await InsertionAPI.ReadRealtimeValues(FieldDevices);
            if (result.Success == false && FailedTogle == false)
            {
                FailedTogle = true;
                await EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - " + "InsertionAPI.ReadRealtimeValues: " + result.ErrorMessage);
            }
            else
            {
                FailedTogle = false;
            }
        }
        catch (Exception ex)
        {
            await EventLogging.AddLogMessage(MessageType.ExceptionError, this.GetType().Name + " - " + ServiceDisplayName + " - " + "OnboardTask exception error! Error: " + ex.Message);
        }
        OnboardTaskHandler.Run();  //Task continues to run
    }

    private async void PufferTask(object sender, EventArgs e)
    {
        try
        {
            #region Send LogPuffer
            if (InsertionAPI.LogPufferCount() != 0)
            {
                List<IoTServerInsertionAPI.LogPuffer.LogItem> SentItemsFromLogPuffer = new List<IoTServerInsertionAPI.LogPuffer.LogItem>();

                var LogPufferClone = InsertionAPI.GetLogPuffer();

                ....

                //After sent items delete from LogPuffer
                InsertionAPI.DeleteLogPuffer(SentItemsFromLogPuffer);
            }
            #endregion

            #region Send Onboard Storage
            if (ComErrFlag == false)
            {
                var onestorge = await InsertionAPI.GetOnboardStorageLogFile();
                if (onestorge != null && onestorge.Name !=null)
                {
                    var datafromstorage = JsonConvert.DeserializeObject<List<IoTServerInsertionAPI.LogPuffer.LogItem>>(onestorge.Data);

                    ...

                    //After sent items delete from OnboardStorage
                    await InsertionAPI.DeleteOnboardStorage(onestorge.Name);
                }
            }
            #endregion

            #region Log to Onboard Storage
            if (OnboardStopWatch.IsRunning == false && ComErrFlag == true)
            {
                OnboardStopWatch.Reset();
                OnboardStopWatch.Start();
            }
            if (ComErrFlag == false)
            {
                OnboardStopWatch.Stop();
                OnboardStopWatch.Reset();
            }
            else if (OnboardStopWatch.Elapsed.TotalSeconds > OnboardStorageDelay)
            {
                OnboardStopWatch.Stop();
                OnboardStopWatch.Reset();
                await InsertionAPI.LogToOnboardStorage();
            }
            #endregion
        }
        catch (Exception ex)
        {
            await EventLogging.AddLogMessage(MessageType.ExceptionError, this.GetType().Name + " - " + ServiceDisplayName + " - " + "PufferTask exception error! Error: " + ex.Message);
        }
        PufferTaskHandler.Run();
    }
