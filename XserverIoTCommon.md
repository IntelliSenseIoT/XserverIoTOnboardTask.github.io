# XserverIoTCommon class usage:

## Required UWP Target settings:

    Min version: Windows 10 Fall Creators Update (10.0; Build 16299) 

## Required UWP Capabilities:

    <Capability Name="internetClient" />
    <Capability Name="internetClientServer"/>
    <Capability Name="privateNetworkClientServer"/>

## Authentication class (Connect to Service):

    // Log in to Xserver.IoT
    public static async Task<Result> Login(string UserName, string Password, string ServiceIP = "localhost")

    example: var res = await Authentication.Login("operator", "operator", "192.168.100.1");

    // Gets UserId object for Xserver.Com service object 
    public static Models.Com.Common.IUserId GetComServiceUserId()

    example:

    IActiveAlarms AlarmRequest = new IActiveAlarms();
        
    AlarmRequest.IUserId = Authentication.GetComServiceUserId();
    AlarmRequest.NumberOfItems = 0; //No Limit

    var resultackalarm = await RestAPI.RestClientPOST("/com/alarms/getactivealarms", ServiceName.Com, AlarmRequest);

## RestAPI methods:

    /// GET RestAPI request
    public static async Task<IO.RestClient.RestClient.Result> RestClientGET(string RequestUri, ServiceName Service)    
 
    /// POST RestAPI request
    public static async Task<IO.RestClient.RestClient.Result> RestClientPOST(string RequestUri, ServiceName Service, object objectcontent)

    /// POST RestAPI request with Auth object
    public static async Task<IO.RestClient.RestClient.Result> RestClientPOSTAuthObj(string RequestUri, ServiceName Service, object SerializeObject)

## Realtime class methods and objects:

    public List<ISourceInfo> ListOfSources { get; internal set; } = new List<ISourceInfo>();
    public List<ISourceQuantitiesInfo> ListOfQuantities { get; internal set; } = new List<ISourceQuantitiesInfo>();
   
    /// Uploads ListOfSources and ListOfQuantities objects from Xserver.Com service   
    public async Task<Result> GetSourcesQuantities()
    
    /// Get SourceId and QuantityId (error return value null)
    public QuantityInfo GetIds(string SourceName, string QuantityName)
   
    /// Gets value of the quantity of the Source (error return value null)
    public async Task<QuantityValueItem> GetValue(string SourceName, string QuantityName)
   
    /// Gets values of the quantities of the Sources (error return value null)
    public async Task<List<QuantityValueItem>> GetValues(List<QuantitiesRequestItem> QuantitiesRequestList)
    
    /// Writes value of the quantity of the Source (error return value null)
    public async Task<QuantityWriteResult> WriteValue(string SourceName, string QuantityName, double WriteValue)

### Examples:

    Realtime RealtimeObj = new Realtime();

    ///This must be run first !!!
    private async void Button_Click_5(object sender, RoutedEventArgs e)
    {
        var res = await Authentication.Login("operator", "operator", "192.168.100.1");
        var res1 = await RealtimeObj.GetSourcesQuantities();
    }

    private void Button_Click_6(object sender, RoutedEventArgs e)
    {
        var res  = RealtimeObj.GetIds("Compressor", "Run");
    }

    private async void Button_Click_7(object sender, RoutedEventArgs e)
    {
        var Status = await RealtimeObj.GetValue("Compressor", "Run");
    }

    private async void Button_Click_8(object sender, RoutedEventArgs e)
    {
        List<QuantitiesRequestItem> Reqs = new List<QuantitiesRequestItem>();
        QuantitiesRequestItem oner = new QuantitiesRequestItem();
        QuantitiesRequestItem oner1 = new QuantitiesRequestItem();

        oner.SourceName = "Compressor";
        oner.QuantityName = "Run";

        oner1.SourceName = "Compressor";
        oner1.QuantityName = "Fail";

        Reqs.Add(oner);
        Reqs.Add(oner1);
        var ress = await RealtimeObj.GetValues(Reqs);
    }

    private async void Button_Click_9(object sender, RoutedEventArgs e)
    {
        var writeresult = await RealtimeObj.WriteValue("Main PLC", "Status", 0);


    }
