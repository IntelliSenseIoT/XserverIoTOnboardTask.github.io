# Required UWP Target settings:

    Min version: Windows 10 Fall Creators Update (10.0; Build 16299)

# Required UWP Capabilities:

    <Capability Name="internetClient" />
    <Capability Name="internetClientServer"/>
    <Capability Name="privateNetworkClientServer"/>

# Required UWP Target settings:

    Min version: Windows 10 Fall Creators Update (10.0; Build 16299)

# Required UWP Capabilities:

    <Capability Name="internetClient" />
    <Capability Name="internetClientServer"/>
    <Capability Name="privateNetworkClientServer"/>

# Authentication class:

    // Log in to Xserver.IoT
    public static async Task<Result> Login(string UserName, string Password, string ServiceIP = "localhost")

    example: var res = await Authentication.Login("operator", "operator", "10.29.2.154");

    // Gets UserId object 
    public static Models.Com.Common.IUserId GetComServiceUserId()

    example:

    IActiveAlarms AlarmRequest = new IActiveAlarms();
        
    AlarmRequest.IUserId = Authentication.GetComServiceUserId();
    AlarmRequest.NumberOfItems = 0; //No Limit

    var resultackalarm = await RestAPI.RestClientPOST("/com/alarms/getactivealarms", ServiceName.Com, AlarmRequest);

# RestAPI methods:

   Use Xserver.IoT.Connectivity.Interface class REST API methods.
   More technical details are in the Xserver.IoT.Connectivity.Interface documentation.

# Realtime objects and methods:

    public List<ISourceInfo> ListOfSources { get; internal set; }
    public List<ISourceQuantitiesInfo> ListOfQuantities { get; internal set; }

    //Uploads ListOfSources and ListOfQuantities objects from Xserver.Com service
    public async Task<Result> GetSourcesQuantities()

    //Gets SourceId and QuantityId (error return value null)
    public QuantityInfo GetIds(string SourceName, string QuantityName)

    //Gets value of the quantity of the Source (error return value null)
    public async Task<QuantityValueItem> GetValue(string SourceName, string QuantityName)

    //Gets values of the quantities of the Sources (error return value null)
    public async Task<List<QuantityValueItem>> GetValues(List<QuantitiesRequestItem> QuantitiesRequestList)

    //Writes value of the quantity of the Source (error return value null)
    public async Task<QuantityWriteResult> WriteValue(string SourceName, string QuantityName, double WriteValue)

    /// Adds new values to PeriodLog
    public async Task<Result> PeriodicLogAddNewValues(List<LogItem> LogItems)

    /// Adds new values to DifferenceLog    
    public async Task<Result> DifferenceLogAddNewValues(List<EventItem> LogItems)

# EventLogging methods:

    //Adds a new event into the EventLog
    public static void AddLogMessage(MessageType MessageType, string Message)

# HttpRestServerService methods: 

    /// If true then REST HTTP server is running
    public bool IsStartHttpServer { get; set; }

    /// Start and Initialize Http server
    public async Task<IO.SimpleHttpServer.Result> HttpRESTServerStart()

    /// Stop Http server   
    public async Task<IO.SimpleHttpServer.Result> HttpRESTServerStop()

    /// Send response to client 
    public async Task<IO.SimpleHttpServer.Result> ServerResponse(HTTPStatusCodes HTTPStatus, Windows.Storage.Streams.IOutputStream OStream, string SendData)

# Examples:

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var res = await Authentication.Login("operator", "operator", "10.29.2.154");
    }

    private async void Button_Click_1(object sender, RoutedEventArgs e)
    {
        var result = await RestAPI.RestClientGET("/com/getsources", ServiceName.Com);
    }

    private async void Button_Click_2(object sender, RoutedEventArgs e)
    {
        var result = await RestAPI.RestClientPOSTAuthObj("/data/system/gettemplatedevices", ServiceName.Data, null);
    }

    private async void Button_Click_3(object sender, RoutedEventArgs e)
    {
        var res = await Authentication.Login("admin", "admin", "10.29.2.12");
    }

    private async void Button_Click_4(object sender, RoutedEventArgs e)
    {
        IActiveAlarms AlarmRequest = new IActiveAlarms();
        
        AlarmRequest.IUserId = Authentication.GetComServiceUserId();
        AlarmRequest.NumberOfItems = 0; //No Limit

        var resultackalarm = await RestAPI.RestClientPOST("/com/alarms/getactivealarms", ServiceName.Com, AlarmRequest);
    }

    Realtime RealtimeObj = new Realtime();

    private async void Button_Click_5(object sender, RoutedEventArgs e)
    {
        var res = await Authentication.Login("operator", "operator", "10.29.2.154");

        var res1 = await RealtimeObj.GetSourcesQuantities();
    }

    private void Button_Click_6(object sender, RoutedEventArgs e)
    {
        var res  = RealtimeObj.GetIds("Main PLC", "Light");
    }

    private async void Button_Click_7(object sender, RoutedEventArgs e)
    {
        var Light = await RealtimeObj.GetValue("Main PLC", "Light");
    }

    private async void Button_Click_8(object sender, RoutedEventArgs e)
    {
        List<QuantitiesRequestItem> Reqs = new List<QuantitiesRequestItem>();
        QuantitiesRequestItem oner = new QuantitiesRequestItem();
        QuantitiesRequestItem oner1 = new QuantitiesRequestItem();

        oner.SourceName = "Main PLC";
        oner.QuantityName = "Light";

        oner1.SourceName = "Main PLC";
        oner1.QuantityName = "Status";

        Reqs.Add(oner);
        Reqs.Add(oner1);
        var ress = await RealtimeObj.GetValues(Reqs);
    }

    private async void Button_Click_9(object sender, RoutedEventArgs e)
    {
        var writeresult = await RealtimeObj.WriteValue("Main PLC", "Status", 0);
    }

    private async void Button_Click_10(object sender, RoutedEventArgs e)
    {
        DateTime actualtime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second);

        List<LogItem> LogItems = new List<LogItem>();

        LogItem oneitem = new LogItem();

        oneitem.SourceName = "VirtualDev";
        oneitem.QuantityName = "Test1";
        oneitem.TimestampUTC = null;            //If TimestampUTC is null, the current UTC is used.
        oneitem.Value = DateTime.Now.Second;

        LogItems.Add(oneitem);

        LogItem oneitem1 = new LogItem();

        oneitem1.SourceName = "VirtualDev";
        oneitem1.QuantityName = "Test2";
        oneitem1.TimestampUTC = actualtime;    //If TimestampUTC is null, the current UTC is used.
        oneitem1.Value = DateTime.Now.Second + 10;

        LogItems.Add(oneitem1);

        var result = await RealtimeObj.PeriodicLogAddNewValues(LogItems);
    }

    private async  void Button_Click_11(object sender, RoutedEventArgs e)
    {
        DateTime actualtime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second);

        List<EventItem> EventItems = new List<EventItem>();

        EventItem oneitem = new EventItem();

        oneitem.SourceName = "VirtualDev";
        oneitem.QuantityName = "Test3";
        oneitem.TimestampUTC = actualtime;      //If TimestampUTC is null, the current UTC is used.
        oneitem.Value = DateTime.Now.Second;
        oneitem.ChangePercent = DateTime.Now.Second+10;
        oneitem.TolerancePercentage = DateTime.Now.Second;

        EventItems.Add(oneitem);

        var result = await RealtimeObj.DifferenceLogAddNewValues(EventItems);
    }
