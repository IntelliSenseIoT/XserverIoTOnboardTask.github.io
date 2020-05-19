# XserverIoTCommon class usage:

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
