# Models.Com contains the classes of the Xserver.Com service.

## Interfaces:

    public class IQuantitiesOfSource        //IQuantitiesOfSource object is included all of the quantities of one Source (for REST communication)
    {
        public IQuantitiesOfSourceResult Result { get; set; }
        public Int16 SourceId { get; set; }
        public string SourceName { get; set; }
        public List<QuantityItem> Quantities = new List<QuantityItem>();
    }

    public class ISource                    //Define Source by Id and Name (for REST communication)
    {
        public Int16 SourceId { get; set; }
        public string SourceName { get; set; }
    }

    public class ISourceInfo                //Source information
    {
        public Int16 SourceId { get; set; }
        public string SourceName { get; set; }
        public List<SourceUseGroup> GroupSettings { get; set; }
        public string Tag { get; set; }
    }

    public class ISourceQuantitiesInfo      //Source quantites information 
    {
        public Int16 SourceId { get; set; }
        public string SourceName { get; set; }
        public List<IQuantityInfo> Quantities = new List<IQuantityInfo>();
    }

    public class IGroupInfo                 //Group Information
    {
        public Int16 GroupId { get; set; }
        public string GroupName { get; set; }
        public List<SourceGroupSettings> SourcesOfGroup = new List<SourceGroupSettings>();
    }

    public class IActiveAlarms              // Active alarm request
    {
        // if NumberOfItems = 0 then no size limit
        public int NumberOfItems { get; set; }
        public IUserId IUserId { get; set; }
    }

    public class ISourceActiveAlarms        // One source active alarm request
    {
        public Int16 SourceId { get; set; }
        public IUserId IUserId { get; set; }
    }

    public class IFilteredActiveAlarm       // Filtered active alarm request
    {
        public List<string> Filters { get; set; }
        public IUserId IUserId { get; set; }
    }

    public class IActiveAlarmResponse       // Answer for request
    {
        public List<AlarmList.AlarmItem> ActiveAlarms { get; set; }
        public int Count { get; set; }
        public int AckedAlarmsCount { get; set; }
        public int NotAckedAlarmsCount { get; set; }
    }
    
    public class IAckAlarm    // Acknowledge one alarm
    {
        // For historical alarm
        public int GUIDforHis { get; set; }
        // For active alarm
        public Int16 SourceIdforAct { get; set; }
        // For active alarm
        public Int16 QuantityIdforAct { get; set; }
        public string AckNote { get; set; }
        public IUserId IUserId { get; set; }
    }

    public class IAckAllAlarms   // Acknowledge all alarms
    {
        public string AckNote { get; set; }
        public IUserId IUserId { get; set; }
    }

    public class IUserId                    // User Identify
    {
        public string UserName { get; set; }
        public byte[] Password { get; set; }
    }

    // Acknowledge one alarm
    public class IAckAlarm
    {
        public int GUID { get; set; }
        public string AckNote { get; set; }
        public IUserId IUserId { get; set; }
    }

    // Acknowledge all alarms
    public class IAckAllAlarms
    {
        public string AckNote { get; set; }
        public IUserId IUserId { get; set; }
    }

    // Authorized object (for REST request, for example: /com/deviceextension/writertcclock)
    public class IAuthorizedObject
    {
            public string SerializedObject { get; set; }
            public IUserId IUserId { get; set; }
    }
    
    //Define Source and Quantity for write
    public class ISourceQuantityWrite
    {
        public Int16 SourceId { get; set; }
        public Int16 QuantityId { get; set; }
        public double WriteValue { get; set; }
    }
