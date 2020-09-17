# Models.Data contains the classes of the Xserver.Data service.

## Interfaces:

    public class IAccessToken                       //Access Token
    {
        public byte[] AESVector { get; set;}
        public byte[] AESKey { get; set; }
    }

    public class IAuthorizedObject                  // Authorized object for RestAPI communication
    {
        public string SerializedObject { get; set; }
        public IUserId IUserId { get; set; }
    }

    public class IDirectMethod                      //Object for Direct method communication
    {
        /// <summary>
        /// Target resource name
        /// </summary>
        public string TargetResource { get; set; }
        /// <summary>
        /// Resoure parameter (optional)
        /// </summary>
        public string Parameter { get; set; }
        /// <summary>
        /// Resource method
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// Message to the target resource
        /// </summary>
        public string Message { get; set; }
        public string Tag { get; set; }
    }

    public class IDirectMethodServiceResponse       // Object for RESTAPI response on Direct method
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Response from target resource
        /// </summary>
        public string Response { get; set; }
        public string Tag { get; set; }
    }

    public class IGetEvent                          // Define event query parameters
    {
        public QueryType QueryType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfEvents { get; set; }
        public bool UseFilter { get; set; }
        public List<string> Filters { get; set; }
    }

    public class IGetUserActivity                   // Define UserActivity query parameters
    {
        public QueryType QueryType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfEvents { get; set; }
        public bool UseFilter { get; set; }
        public List<string> Filters { get; set; }
        public bool UseUserFilter { get; set; }
        public string UserFilter { get; set; }
    }

    public class IGroupQuantity                    // Id of Group and Quantity
    {
        public Int16 GroupId { get; set; }
        public Int16 QuantityId { get; set; }
    }

    public class IMaintenanceParameter              // Parameters for database maintenance
    {
        public RemoveParameter RemoveSetting { get; set; }
        public DateTime RemoveDateTimeLT {get; set;}
    }

    public class INewUserPassword                  //New user password
    {
        public byte[] NewPassword { get; set; }
    }

    public class IRegInfo                           // Get register info 
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int NumberOfRegisters { get; set; }
    }

    public class IResult                            //REST answer
    {
        public string SerializedObject { get; set; }
        public bool Success { get; set; }
        public ErrorCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ISearchStorage                     //Storage parameters
    {
        public string Search { get; set; }
        public bool MatchWhole { get; set; }
    }

    public class ISnapshot                          //Snapshot object
    {
        public string Sender { get; set; }
        public List<string> SerializedObject { get; set; }
    }

    public class ISourceQuantity                    // Id of Source and Quantity
    {
        public Int16 SourceId { get; set; }
        public Int16 QuantityId { get; set; }
    }

    public class ISourceQuantityAggregatedValue : Models.Data.CommonLog.LogItemKey      // Aggregated Value of Sources
    {
        public double Value { get; set; }
        public AggregationTypeId AggregationTypeId { get; set; }
    }

    public class ISourceQuantityMinMaxValue : Models.Data.CommonLog.LogItemKey          // Mininimum and Maximum Value of Sources
    {
        public double Value { get; set; }
        public TypeId TypeId { get; set; }
    }

    public class IStorage
    {
        public Identify Identify { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public TypeOfAccess AccessType { get; set; }
        public List<string> AccessNames { get; set; }
        public string DataType { get; set; }
        public string Data { get; set; }
    }

    public class IStorageInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public TypeOfAccess AccessType { get; set; }
        public List<string> AccessNames { get; set; }
        public string DataType { get; set; }
    }

     public class IStorageNewName
    {
        public Identify Identify { get; set; }
        public int Id { get; set; }
        public string OldName { get; set; }
        public string NewName { get; set; }
    }

    public class ITemplateDeviceDriver                  // Serialized Template Device Driver
    {
        public string TemplateDriverName { get; set; }
        public List<string> SerializedObject { get; set; } = new List<string>();
    }

    public class IImportTemplateDeviceDriver            // Import Template Device Driver
    {
        public bool OverwriteExisting { get; set; }
        public int RemoveId { get; set; }
        public ITemplateDeviceDriver TemplateDriver { get; set; } 
    }

    public class IUserId                            //User Identify
    {
        public string UserName { get; set; }
        public byte[] Password { get; set; }
    }
