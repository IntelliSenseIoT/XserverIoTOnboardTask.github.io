# Example 14 - Upload onboard data from flow meter and Log to SQL database

### The following example shows how to upload data from the onboard memory of an flow meter:

  - Upload historical data to the IoT server with onboardtask
  - Historical data log to SQL Server PeriodLog table
  - If the SQL server is not available, the data is stored in the onboard storage of the IoT Server
  - When the SQL server is reconnected, the data is uploaded from the onboard storage to the SQL server.

### Prerequisites:

  - Installed SQL Server 2019 Express (Create empty IoT Server database)
  - IoT Server connects to Flow meter on RS485 serial cable
  - Configure IoT Server with IoT Explorer
  
## Example:

In the example below, we upload data (hourly, 8 hours, daily, monthly) from a Modbus flow meter (connected to IoT Server with RS485) and log the data to an SQL server (we gets from connection string from IoT Explorer settings). If the connection to the SQL server is lost, the data is temporarily stored in the Onboard storage.

    using File.Log;
    using Models.Com.Common;
    using Models.Data;
    using Newtonsoft.Json;
    using Service.Common;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.ApplicationModel.Background;
    using Xserver.IoT.Connectivity.Interface;
    using XserverIoTCommon;

    namespace XServerIoTOnboardTaskProject
    {
        //Project: Flow onboard data reader & uploader
        public sealed class StartupTask : IBackgroundTask
        {
            #region XServerIoTOnboardTask service settings
            //Service display name
            private const string ServiceDisplayName = "Xserver.OnboardTask";
            //Task Handler Period (ms)
            private const int TaskHandlerPeriod = 15000;
            //Onboard Logtimes
            private int HourlyMinuteOffset = 5;
            private DateTime[] EightHourLogTime = new DateTime[3] { new DateTime(2000, 1, 1, 6, 0, 0), new DateTime(2000, 1, 1, 14, 0, 0), new DateTime(2000, 1, 1, 22, 0, 0) };
            private int EightMinuteOffset = 5;
            private int DailyMonthlyMinuteOffset = 5;
            private int SQLItemsOneQuery = 500;
            private int SQLLoggingPeriod = 60; //Seconds
            private int OnboardStorageDelay = 300; //Seconds
            private int LogInfoChangedDelay = 120; //Seconds
            #endregion

            #region Helpers
            //Log service events and errors
            TaskHandler OnboardTaskHandler = new TaskHandler();
            Realtime FieldDevices = new Realtime();
            TimeCalculation Tcalc = new TimeCalculation();
            //SQL Server
            string PeriodTableName;
            SQL SQLlogger = new SQL();
            bool SQLLogErrorFlag = false;
            //Project Info
            private string Namespace = null;
            //Timers
            Stopwatch SQLLoggingStopWatch = new Stopwatch();
            Stopwatch OnboardStopWatch = new Stopwatch();
            Stopwatch LongInfoStopWatch = new Stopwatch();
            #endregion

            private static BackgroundTaskDeferral _Deferral = null;

            public async void Run(IBackgroundTaskInstance taskInstance)
            {
                _Deferral = taskInstance.GetDeferral();

                EventLogging.Initialize();
                EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Start initializing...");

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
                    EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - Authentication error! " + res.ErrorMessage);
                }
                #endregion

                #region Get SQL connection info
                var sqlres = await SQLInfo.GetConnectionInfo();
                if (sqlres.Success == true)
                {
                    SQLlogger.ConnectionString = sqlres.SQLConnectionString;
                    PeriodTableName = sqlres.SQLPeriodLogsTableName;
                }
                else
                {
                    EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - SQL Server connection info error! " + sqlres.ErrorMessage);
                }
                #endregion

                #region Get ProjectInfo
                var projres = await ProjectInfo.GetProjectInfo();
                if (projres.Success == true)
                {
                    Namespace = ProjectInfo.MyProject.Namespace;
                }
                else
                {
                    EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - Project info error! " + projres.ErrorMessage);
                }
                #endregion

                #region Get LogInfo config from OnboardStorage
                var onboardconfig = await XserverIoTCommon.OnboardTask.GetConfig();
                if (onboardconfig.Success== true && string.IsNullOrEmpty(onboardconfig.Content) ==false)
                {
                    LoginInfoList = JsonConvert.DeserializeObject<List<LogInfo>>(onboardconfig.Content);
                }
                else if (onboardconfig.Success == false)
                {
                    EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - LogInfo config error! " + onboardconfig.ErrorMessage);
                }
                #endregion

                //Initialize and Start IoT OnboardTask
                OnboardTaskHandler.WaitingTime = TaskHandlerPeriod;
                OnboardTaskHandler.ThresholdReached += OnboardTask;
                OnboardTaskHandler.Run();

                EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Finished initialization.");
            }

            private async void OnboardTask(object sender, EventArgs e)
            {
                try
                {
                    var result = await FieldDevices.GetSourcesQuantities();
                    if (result.Success == true)
                    {
                        var acttime = DateTime.Now.ToLocalTime();

                        foreach (var item in FieldDevices.ListOfSources)
                        {    
                            #region Hourly 
                            var findh = (LoginInfoList).Where(p => (p.SourceId.Equals(item.SourceId) && p.LogType.Equals(LogType.Hourly))).FirstOrDefault();
                            if (findh==null)
                            {
                                AddNewSource(item.SourceId, LogType.Hourly);
                                LogInfoChanged = true;
                            }
                            else
                            {
                                if (Tcalc.DateDiff(TimeCalculation.DateInterval.Minute, findh.TimeStampLT, acttime) > 60 && acttime.Minute >= HourlyMinuteOffset)
                                {
                                    var SourceProps = await FieldDevices.GetSourceProperties(item.SourceId);
                                    var findq = await GetQuantitiesList(item.SourceId, SourceProps.TemplateDeviceId, LogType.Hourly);
                                    if (findq != null && findq.Count() != 0)
                                    {
                                        var timestamplt = new DateTime(acttime.Year, acttime.Month, acttime.Day, acttime.Hour, 0, 0);
                                        var resl = await AddLogPuffer(SourceProps, findq, timestamplt);
                                        if (resl == true)
                                        {
                                            findh.LastLogTimeLT = new DateTime(acttime.Year, acttime.Month, acttime.Day, acttime.Hour, acttime.Minute, 0);
                                            findh.TimeStampLT = timestamplt;
                                            LogInfoChanged = true;
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region EightHour
                            var finde = (LoginInfoList).Where(p => (p.SourceId.Equals(item.SourceId) && p.LogType.Equals(LogType.EightHour))).FirstOrDefault();
                            if (finde == null)
                            {
                                AddNewSource(item.SourceId, LogType.EightHour);
                                LogInfoChanged = true;
                            }
                            else
                            {
                                var actshift = GetShift(acttime);

                                if (Tcalc.DateDiff(TimeCalculation.DateInterval.Minute,finde.TimeStampLT,acttime)>480 && acttime.Minute > EightMinuteOffset)
                                {
                                    var SourceProps = await FieldDevices.GetSourceProperties(item.SourceId);
                                    var findq = await GetQuantitiesList(item.SourceId,SourceProps.TemplateDeviceId, LogType.EightHour);
                                    if (findq != null && findq.Count() != 0)
                                    {
                                        DateTime timestamplt = new DateTime();
                                        if (actshift == 2 && acttime.Hour < EightHourLogTime[0].Hour)
                                        {
                                            var prevday = acttime.AddDays(-1);
                                            timestamplt = new DateTime(prevday.Year, prevday.Month, prevday.Day, EightHourLogTime[actshift].Hour, 0, 0);
                                        }
                                        else
                                        {
                                            timestamplt = new DateTime(acttime.Year, acttime.Month, acttime.Day, EightHourLogTime[actshift].Hour, 0, 0);
                                        }
                                        var resl = await AddLogPuffer(SourceProps, findq, timestamplt);
                                        if (resl == true)
                                        {       
                                            finde.LastLogTimeLT = new DateTime(acttime.Year, acttime.Month, acttime.Day, acttime.Hour, acttime.Minute, 0);
                                            finde.TimeStampLT = timestamplt;  
                                            LogInfoChanged = true;
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region Daily
                            var findd = (LoginInfoList).Where(p => (p.SourceId.Equals(item.SourceId) && p.LogType.Equals(LogType.Daily))).FirstOrDefault();
                            if (findd == null)
                            {
                                AddNewSource(item.SourceId, LogType.Daily);
                                LogInfoChanged = true;
                            }
                            else
                            {
                                if (findd.TimeStampLT.Day != acttime.Day && acttime.Minute >= DailyMonthlyMinuteOffset)
                                {
                                    var SourceProps = await FieldDevices.GetSourceProperties(item.SourceId);
                                    var findq = await GetQuantitiesList(item.SourceId,SourceProps.TemplateDeviceId, LogType.Daily);
                                    if (findq != null && findq.Count() != 0)
                                    {
                                        var timestamplt = new DateTime(acttime.Year, acttime.Month, acttime.Day, 0, 0, 0);
                                        var resl = await AddLogPuffer(SourceProps, findq, timestamplt);
                                        if (resl == true)
                                        {
                                            findd.LastLogTimeLT = new DateTime(acttime.Year, acttime.Month, acttime.Day, acttime.Hour, acttime.Minute, 0);
                                            findd.TimeStampLT = timestamplt;
                                            LogInfoChanged = true;
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region Monthly
                            var findm = (LoginInfoList).Where(p => (p.SourceId.Equals(item.SourceId) && p.LogType.Equals(LogType.Monthly))).FirstOrDefault();
                            if (findm == null)
                            {
                                AddNewSource(item.SourceId, LogType.Monthly);
                                LogInfoChanged = true;
                            }
                            else
                            {
                                if (findm.TimeStampLT.Month != acttime.Month && acttime.Minute >= DailyMonthlyMinuteOffset)
                                {
                                    var SourceProps = await FieldDevices.GetSourceProperties(item.SourceId);
                                    var findq = await GetQuantitiesList(item.SourceId, SourceProps.TemplateDeviceId, LogType.Monthly);
                                    if (findq != null && findq.Count() != 0)
                                    {
                                        var timestamplt = new DateTime(acttime.Year, acttime.Month, 1, 0, 0, 0);
                                        var resl = await AddLogPuffer(SourceProps, findq, timestamplt);
                                        if (resl == true)
                                        {
                                            findm.LastLogTimeLT = new DateTime(acttime.Year, acttime.Month, acttime.Day, acttime.Hour, acttime.Minute, 0);
                                            findm.TimeStampLT = timestamplt;
                                            LogInfoChanged = true;
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }

                    if (SQLLoggingStopWatch.IsRunning == false)
                    {
                        SQLLoggingStopWatch.Reset();
                        SQLLoggingStopWatch.Start();
                    }
                    if (SQLLoggingStopWatch.Elapsed.TotalSeconds > SQLLoggingPeriod)
                    {
                        SQLLoggingStopWatch.Stop();
                        SQLLoggingStopWatch.Reset();
                        await LogPufferToSQLServer();
                        await LogOnboardStoragetoSQL();
                    }

                    if (OnboardStopWatch.IsRunning == false && SQLLogErrorFlag == true)
                    {
                        OnboardStopWatch.Reset();
                        OnboardStopWatch.Start();
                    }
                    if (SQLLogErrorFlag == false)
                    {
                        OnboardStopWatch.Stop();
                        OnboardStopWatch.Reset();
                    }
                    else if (OnboardStopWatch.Elapsed.TotalSeconds > OnboardStorageDelay)
                    {
                        OnboardStopWatch.Stop();
                        OnboardStopWatch.Reset();
                        await LogToOnboardStorage();
                    }

                    if (LongInfoStopWatch.IsRunning == false && LogInfoChanged == true)
                    {
                        LongInfoStopWatch.Reset();
                        LongInfoStopWatch.Start();
                    }
                    else if (LongInfoStopWatch.Elapsed.TotalSeconds > LogInfoChangedDelay)
                    {
                        LongInfoStopWatch.Stop();
                        LongInfoStopWatch.Reset();
                        string content = JsonConvert.SerializeObject(LoginInfoList);
                        await XserverIoTCommon.OnboardTask.SaveConfig(content);
                        LogInfoChanged = false;
                    }
                }
                catch (Exception ex)
                {
                    EventLogging.AddLogMessage(MessageType.ExceptionError, this.GetType().Name + " - " + ServiceDisplayName + " - " + "OnboardTask exception error! Error: " + ex.Message);
                }
                OnboardTaskHandler.Run();  //Task continues to run
            }

            #region LogInfo functions
            private List<LogInfo> LoginInfoList = new List<LogInfo>();
            private bool LogInfoChanged = false;

            private class LogInfo
            {
                public Int16 SourceId { get; set; }
                public LogType LogType{ get; set; }
                public DateTime LastLogTimeLT { get; set; }
                public DateTime TimeStampLT { get; set; }
            }

            private enum LogType
            {
                Hourly,
                EightHour,
                Daily,
                Monthly
            }

            private void AddNewSource(short SourceId, LogType LogType)
            {
                LogInfo newinfo = new LogInfo();
                newinfo.SourceId = SourceId;
                newinfo.LogType = LogType;
                newinfo.LastLogTimeLT = new DateTime(2000, 1, 1, 0, 0, 0);
                LoginInfoList.Add(newinfo);
            }
            #endregion

            private async Task<List<SystemDB.Model.TemplateQuantity>> GetQuantitiesList(short SourceId,int TemplateDeviceId, LogType logType)
            {
                IEnumerable<SystemDB.Model.TemplateQuantity> findq = null;

                var tempdevquans = await FieldDevices.GetTemplateDeviceQuantitiesProperties(TemplateDeviceId);
                if (tempdevquans != null)
                {
                    if (logType == LogType.Hourly)
                    {
                        findq = (tempdevquans).Where(p => (p.QuantityTypeId.Equals(SystemDB.Model.QuantityTypeId.Undefined) && p.DensityType.Equals(SystemDB.Model.LogInterval.Minute) && p.Density.Equals(60)));
                    }
                    else if (logType == LogType.EightHour)
                    {
                        findq = (tempdevquans).Where(p => (p.QuantityTypeId.Equals(SystemDB.Model.QuantityTypeId.Undefined) && p.DensityType.Equals(SystemDB.Model.LogInterval.Minute) && p.Density.Equals(480)));
                    }
                    else if (logType == LogType.Daily)
                    {
                        findq = (tempdevquans).Where(p => (p.QuantityTypeId.Equals(SystemDB.Model.QuantityTypeId.Undefined) && p.DensityType.Equals(SystemDB.Model.LogInterval.Minute) && p.Density.Equals(1440)));
                    }
                    else if (logType == LogType.Monthly)
                    {
                        findq = (tempdevquans).Where(p => (p.QuantityTypeId.Equals(SystemDB.Model.QuantityTypeId.Undefined) && p.DensityType.Equals(SystemDB.Model.LogInterval.Minute) && p.Density.Equals(44640)));
                    }
                }
                if (tempdevquans == null || findq == null)
                {
                    return null;
                }
                else
                {
                    return findq.ToList();
                }
            }

            private int GetShift(DateTime ActTimeLT)
            {
                if (ActTimeLT.Hour >= EightHourLogTime[0].Hour && ActTimeLT.Hour< EightHourLogTime[1].Hour)
                {
                    return 0;
                }
                else if(ActTimeLT.Hour >= EightHourLogTime[1].Hour && ActTimeLT.Hour < EightHourLogTime[2].Hour)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }

            #region LogPuffer functions
            private List<LogItem> LogPuffer = new List<LogItem>();
            private class LogItem
            {
                public string SourceName { get; set; }
                public string Classification { get; set; }
                public string QuantityName { get; set; }
                public string QuantityTypeName { get; set; }
                public string Unit { get; set; }
                public double Value { get; set; }
                public DateTime TimestampUTC { get; set; }
                public DateTime TimeStampLT { get; set; }
            }

            private async Task<bool> AddLogPuffer(SystemDB.Model.Source SourceProperties, List<SystemDB.Model.TemplateQuantity> TempQuantities, DateTime TimeStampLT)
            {
                bool OK = false;

                var tempdevprop = await FieldDevices.GetTemplateDeviceProperties(SourceProperties.TemplateDeviceId);
                if (tempdevprop != null)
                {
                    List<QuantitiesRequestItem> requestlist = new List<QuantitiesRequestItem>();
                    foreach (var itemq in TempQuantities)
                    {
                        QuantitiesRequestItem onereq = new QuantitiesRequestItem();
                        onereq.SourceName = SourceProperties.SourceName;
                        onereq.QuantityName = itemq.QuantityName;
                        requestlist.Add(onereq);
                    }
                    var Quantities = await FieldDevices.GetValues(requestlist);
                    if (Quantities != null && Quantities.Count != 0)
                    {
                        var find = (Quantities).Where(p => (p.Quality.Equals(Quality.None))).FirstOrDefault();
                        if (find == null)
                        {
                            foreach (var item in Quantities)
                            {
                                LogItem newitem = new LogItem();
                                newitem.SourceName = item.SourceName;
                                newitem.Classification = tempdevprop.Classification;
                                newitem.QuantityName = item.QuantityName;
                                newitem.QuantityTypeName = item.QuantityTypeName;
                                newitem.Unit = item.Unit;
                                newitem.Value = item.Value;
                                newitem.TimeStampLT = TimeStampLT;
                                newitem.TimestampUTC = TimeStampLT.ToUniversalTime();
                                LogPuffer.Add(newitem);
                                OK = true;
                            }
                        }
                    }
                }
                return OK;
            }
            #endregion

            #region Log to SQLServer
            private async Task<bool> LogPufferToSQLServer()
            {
                if (LogPuffer !=null && LogPuffer.Count !=0)
                {
                    var findres = (LogPuffer).Take(SQLItemsOneQuery).ToList();
                    if (findres !=null)
                    {
                        var sqlres = await LogToSQLServer(findres);
                        if (sqlres == true)
                        {
                            foreach (var item in findres)
                            {
                                LogPuffer.Remove(item);
                            }
                            SQLLogErrorFlag = false;
                        }
                        else
                        {
                            SQLLogErrorFlag = true;
                        }
                    }
                }
                return SQLLogErrorFlag;
            }

            private async Task<bool> LogOnboardStoragetoSQL()
            {
                bool OK = false;
                IStorage sf = new IStorage();

                sf.DataType = "*";

                var result = await XserverIoTConnectivityInterface.RestClientPOSTAuthObj("/data/system/getstorageslistbydatatype", ServiceName.Data, sf);

                if (result.Success == true)
                {
                    var answer = JsonConvert.DeserializeObject<Models.Data.IResult>(result.ResultContent);
                    if (answer.Success == true)
                    {
                        var storageslist = JsonConvert.DeserializeObject<List<IStorageInfo>>(answer.SerializedObject);
                        if (storageslist != null && storageslist.Count() != 0)
                        {
                            IStorage s = new IStorage();
                            s.Identify = Identify.Name;
                            s.Name = storageslist.FirstOrDefault().Name;

                            var results = await XserverIoTConnectivityInterface.RestClientPOSTAuthObj("/data/system/getstorage", ServiceName.Data, s);
                            var answers = JsonConvert.DeserializeObject<Models.Data.IResult>(results.ResultContent);
                            if (answers.Success == true)
                            {
                                var storage = JsonConvert.DeserializeObject<IStorage>(answers.SerializedObject);
                                var logitems = JsonConvert.DeserializeObject<List<LogItem>>(storage.Data);
                                var sqlres = await LogToSQLServer(logitems);
                                if (sqlres == true)
                                {
                                    IStorage sd = new IStorage();
                                    sd.Identify = Identify.Name;
                                    sd.Name = storageslist.FirstOrDefault().Name;

                                    var resultd = await XserverIoTConnectivityInterface.RestClientPOSTAuthObj("/data/system/removestorage", ServiceName.Data, sd);
                                    SQLLogErrorFlag = false;
                                    OK = true;
                                }
                                else
                                {
                                    SQLLogErrorFlag = true;
                                }
                            }
                        }
                    }
                }
                return OK;
            }
            private async Task<bool> LogToSQLServer(List<LogItem> LogItems)
            {
                if (LogItems != null)
                {
                    var res = await ProjectInfo.GetProjectInfo();
                    if (res.Success == true)
                    {
                        Namespace = ProjectInfo.MyProject.Namespace;
                    }

                    SQLTableInfo.PeriodData Period = new SQLTableInfo.PeriodData();
                    foreach (var item in LogItems)
                    {
                        SQLTableInfo.Common.PeriodDataTableRow NewRow = new SQLTableInfo.Common.PeriodDataTableRow();

                        NewRow.TimestampUTC = item.TimestampUTC;
                        NewRow.TimestampLT = item.TimeStampLT;
                        NewRow.Namespace = Namespace;
                        NewRow.SourceName = item.SourceName;
                        NewRow.Classification = item.Classification;
                        NewRow.QuantityName = item.QuantityName;
                        NewRow.QuantityTypeName = item.QuantityTypeName;
                        NewRow.Unit = item.Unit;
                        NewRow.Value = item.Value;
                        NewRow.Calculated = false;

                        Period.Add(NewRow);
                    }

                    var sqlres = SQLlogger.SQLBulkInsert(PeriodTableName, Period.Get());

                    return sqlres.Success;
                }
                else
                {
                    return false;
                }
            }
            #endregion

            #region LogPuffer to Onboard Storage
            private async Task<bool> LogToOnboardStorage()
            {
                bool OK = false;
                if (LogPuffer != null && LogPuffer.Count != 0)
                {
                    var findres = (LogPuffer).Take(SQLItemsOneQuery).ToList();
                    if (findres != null && findres.Count() !=0)
                    {
                        IStorage newstorage = new IStorage();

                        newstorage.Name = "File"+ DateTime.UtcNow.Ticks.ToString();
                        newstorage.DataType = "Data";
                        newstorage.AccessType = Models.Data.TypeOfAccess.EveryBody;
                        newstorage.Data = JsonConvert.SerializeObject(findres);

                        var result = await XserverIoTConnectivityInterface.RestClientPOSTAuthObj("/data/system/addstorage", ServiceName.Data, newstorage);
                        if (result.Success == true)
                        {
                            var answer = JsonConvert.DeserializeObject<Models.Data.IResult>(result.ResultContent);
                            if (answer.Success == true)
                            {
                                foreach (var item in findres)
                                {
                                    LogPuffer.Remove(item);
                                }
                                OK = true;
                            }
                        }
                    }
                }
                return OK;
            }
            #endregion
        }
    }
  
## highlighted parts:

### Gets ProjectInfo of the IOT Server:

** var projres = await ProjectInfo.GetProjectInfo();
** Namespace = ProjectInfo.MyProject.Namespace;
    


