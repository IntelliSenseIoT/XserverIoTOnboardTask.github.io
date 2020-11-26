# Required UWP Target settings:

    Min version: Windows 10 Fall Creators Update (10.0; Build 16299)

# Required Xserver.IoT firmware:

    Min version: 10.2

# Required UWP Capabilities:

    <Capability Name="internetClient" />
    <Capability Name="internetClientServer"/>
    <Capability Name="privateNetworkClientServer"/>

# Authentication class:

    // Log in to Xserver.IoT
    public static async Task<Result> Login(string UserName, string Password, string ServiceIP = "localhost")

    example: var res = await Authentication.Login("operator", "operator", "192.168.100.15");

    // Gets UserId object 
    public static Models.Com.Common.IUserId GetComServiceUserId()

    example:

    IActiveAlarms AlarmRequest = new IActiveAlarms();
        
    AlarmRequest.IUserId = Authentication.GetComServiceUserId();
    AlarmRequest.NumberOfItems = 0; //No Limit

    var resultackalarm = await RestAPI.RestClientPOST("/com/alarms/getactivealarms", ServiceName.Com, AlarmRequest);

# RestAPI methods (for Data, Com, Core services):

    Use Xserver.IoT.Connectivity.Interface class REST API methods.
    More technical details are in the Xserver.IoT.Connectivity.Interface documentation.

# RestAPI methods for External services:

Properties:

    /// Authentication Username
    public string Username { get; set; }
    /// Authentication Password
    public string Password { get; set; }
    /// Relative or absolute Uri
    public string uriString { get; set; } 
    /// Connection close (Default value = true)
    public bool ConnectionClose { get; set; }

Methods:

    /// Initialize RestClient
    public void RestClientInitialize()

    /// Send a GET request.
    public async Task<Result> RestClientGET(string RequestURI)

    /// Send a PUT request.
    public async Task<Result> RestClientPUT(string RequestURI, object objectcontent)

    /// Send a POST request.
    public async Task<Result> RestClientPOST(string RequestURI, object objectcontent)

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

# OnboardTask methods:

    /// Gets Onboard Task config    
    public static async Task<Result> GetConfig()
    
    /// Gets Onboard Task properties
    public static async Task<Result> GetProperties()
     
    /// Saves new onboard task config to Onboard Storage
    public static async Task<Result> SaveConfig(string NewConfig)
    
    /// Saves new onboard task properties to Onboard Storage
    public static async Task<Result> SaveProperties(string NewProperties)

# DeviceTwin methods:

    /// Gets Desired properties of Device Twin
    public static async Task<ResultDesiredProperties> GetDesiredProperties()
    
    /// Gets Reported properties of Device Twin
    public static async Task<ResultReportedProperties> GetReportedProperties()
    
    /// Saves new ReportedProperties
    public static async Task<Result> SaveReportedProperties(List<DeviceTwinProperty> NewReportedProperties)
    
