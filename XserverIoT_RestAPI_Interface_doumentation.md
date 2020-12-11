# Xserver.IoT Rest API interface documentation

## Service ports:

    ComPort - 8001
    DataPort - 8002
    CorePort - 8003
    OnboardTaskPort - 8006

## Authentication info:

    * Necessary to use UserName and Password
    ** Necessary to use only UserName

## HTTP Response:

    200 OK
    404 Not Found
    401 Unauthorized (Service or User authorization error)

## Xserver.Com API reference:

Xserver.Com manages real-time communication to the field devices (Modbus RTU, TCP/IP and Device Extension IO, Watchdog, RTC, Time synchronize, etc.).

        Gets information about Xserver.Com service status.
        URI: /com/status
        Method: GET
        Response: ServiceStatus object
        HTTP status code: 200 OK

        Gets Xserver.Com service settings.
        URI: /com/settings
        Method: GET
        Response: ComServiceSettings object
        HTTP status code: 200 OK

        Gets list of groups (if response Not_Found then the list of the groups is empty)
        URI: /com/getgroups
        Method: GET
        Response: List<Group> object
        HTTP status code: 200 OK (content is null if SourceGroup or SourceGroup.GroupsList object is null)

        Gets tags of all groups
        URI: /com/getallgroupstags
        Method: GET
        Response: List<string> object
        HTTP status code: 200 OK (content is null if SourceGroup or SourceGroup.GroupsList object is null)

        Gets Groups by filter (Search in Tag of the Group)
        URI: /com/getgroupssbyfilter
        Method: POST
        Request: List<string> (Filters)
        Response: List<Group> object
        HTTP status code: 200 OK, 404 Not Found (request is null)

        Gets list of sources (The list is included SourceId, SourceName, Tag and GroupSettings)
        URI: /com/getsources
        Method: GET
        Response: List<ISourceInfo> object
        HTTP status code: 200 OK

        Gets list of quantites of sources (The list is included SourceId, SourceName and Quantites properties: QuantityId, QuantityName, QuantityTypeId, QuantityTypeName, Unit)
        URI: /com/getquantitesofsources
        Method: GET
        Response: List<ISourceQuantitiesInfo> object
        HTTP status code: 200 OK

        Gets list of quantites of one source (The result is included SourceId, SourceName and Quantites properties: QuantityId, QuantityName, QuantityTypeId, QuantityTypeName, Unit)
        URI: /com/getquantitesofsource
        Method: POST
        Request: ISource (Use only the SourceId)
        Response: ISourceQuantitiesInfo object
        HTTP status code: 200 OK (content is null if not found), 404 Not Found (request is null)

        Gets Sources by filter (Search in Tag of the Source)
        URI: /com/getsourcesbyfilter
        Method: POST
        Request: List<string> (Filters)
        Response: List<ISourceInfo> object
        HTTP status code: 200 OK, 404 Not Found (request is null)

        Gets list of sources of one group (The result is included GroupId, GroupName and Group of Source properties: SourceId, IfMissing, Multiplier)
        URI: /com/getgroupsources
        Method: POST
        Request: Group (Use only the Id)
        Response: IGroupInfo object
        HTTP status code: 200 OK (content is null not found Group)

        Gets tags of all Sources
        URI: /com/getallsourcestags
        Method: GET
        Response: List<string> object
        HTTP status code: 200 OK

        Gets list of quantities value of group
        URI: /com/realtime/getgroupquantitiesvalues
        Method: POST
        Request: List<QuantityRequestItem>
        Response: List<GroupQuantitiesValueItem>
        HTTP status code: 200 OK (content is null if not found Sources), 404 Not Found (Request is null or empty)

        Gets list of quantities value of Sources
        URI: /com/realtime/getsourcequantitiesvalues
        Method: POST
        Request: List<QuantityRequestItem>
        Response: List<QuantityValueItem>
        HTTP status code: 200 OK (content is null if not found Sources), 404 Not Found (Request is null or empty)

        *Writes quantity value of Source
        URI: /com/realtime/writesourcequantityvalue
        Method: POST
        Request: IAuthorizedObject (SerializedObject property is ISourceQuantityWrite)
        Response: QuantityWriteResult
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed or user group hasn't control privilege (CanControl)), 404 Not Found (AuthorizedObj or SerializedObject is null)
        
        ** Gets Active alarms (Alarmgroups of user)
        URI: /com/alarms/getactivealarms
        Method: POST
        Request: IActiveAlarms (Use only UserId.UserName and NumberOfItems parameters)
        Response: IActiveAlarmResponse 
        HTTP status code: 200 OK, 401 Unauthorized (Username is failed), 404 Not Found (Request is null)

        Gets all of Active alarms statistic
        URI: /com/alarms/getallactivealarmsstatistic
        Method: GET
        Response: AlarmList.AlarmsStatistic
        HTTP status code: 200 OK

        ** Gets Active alarms of Source (Alarmgroups of user)
        URI: /com/alarms/getsourceactivealarms
        Method: POST
        Request: ISourceActiveAlarms (Use only UserId.UserName and SourceId parameters)
        Response: IActiveAlarmResponse 
        HTTP status code: 200 OK, 401 Unauthorized (Username is failed), 404 Not Found (Request is null)

        ** Gets Filtered Active alarms (Search in tag of source)
        URI: /com/alarms/getactivealarmsbyfilters
        Method: POST
        Request: IFilteredActiveAlarm (Use only UserId.UserName and Filters parameters)
        Response: IActiveAlarmResponse 
        HTTP status code: 200 OK, 401 Unauthorized (Username is failed), 404 Not Found (Request is null)

## Xserver.Data API reference:

Xserver.Data service manages settings and forwards historical data to the Cloud or SQL server.

        Gets ServiceStatus object
        URI: /data/status
        Method: GET
        Response: ServiceStatus object
        HTTP status code: 200 OK
   
        *Update Storage
        URI: /data/system/updatestoragedata
        Method: POST
        Request IAuthorizedObject and IStorage (Used properties: Identify, Name, Id) object
        Response: IResult object
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed), 404 Not Found (Request is null or empty)

        *Change Storage access properties
        URI: /data/system/changestorageaccess
        Method: POST
        Request IAuthorizedObject and IStorage (Used properties: Identify, Name, Id, AccessType, AccessNames) object
        Response: IResult object
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed), 404 Not Found (Request is null or empty)

        *Change the Name of the Storage
        URI: /data/system/changestoragename
        Method: POST
        Request IAuthorizedObject and IStorageNewName object
        Response: IResult object
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed), 404 Not Found (Request is null or empty)

        *Add Storage
        URI: /data/system/addstorage
        Method: POST
        Request IAuthorizedObject and IStorage object
        Response: IResult object and int (StorageId) SerializedObject
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed), 404 Not Found (Request is null or empty)

        *Get Storage
        URI: /data/system/getstorage
        Method: POST
        Request IAuthorizedObject and IStorage (Used properties: Identify, Name, Id) object
        Response: IResult object Storage SerializedObject
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed), 404 Not Found (Request is null or empty)

        *Get Storage by DataType
        URI: /data/system/getstorageslistbydatatype
        Method: POST
        Request IAuthorizedObject and IStorage (Used properties: DataType - if DataType value is null, empty or '*' get back all of storages) object
        Response: IResult object List<IStorageInfo> SerializedObject
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed), 404 Not Found (Request is null or empty)

        *Search Storage(s)
        URI: /data/system/searchstorage
        Method: POST
        Request IAuthorizedObject and ISearchStorage object
        Response: IResult object List<IStorageInfo> SerializedObject
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed), 404 Not Found (Request is null or empty)

        *Get all Storages (administrator privilege necessary)
        URI: /data/system/getallstorageslistforadmin
        Method: POST
        Request IAuthorizedObject object
        Response: IResult object List<IStorageInfo> SerializedObject
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed), 404 Not Found (Request is null or empty)

        *Remove Storage
        URI: /data/system/removestorage
        Method: POST
        Request IAuthorizedObject and IStorage (Used properties: Identify, Name, Id) object
        Response: IResult object
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed), 404 Not Found (Request is null or empty)

        *Copy the Storage
        URI: /data/system/copystorage
        Method: POST
        Request IAuthorizedObject and IStorageNewName object
        Response: IResult object and int (StorageId) SerializedObject
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed), 404 Not Found (Request is null or empty)

        *Remove all Storages without the System accesstype of Storages (administrator privilege necessary)
        URI: /data/system/removeallstoragesforadmin
        Method: POST
        Request IAuthorizedObject object
        Response: IResult object
        HTTP status code: 200 OK, 401 Unauthorized (Username or Password is failed), 404 Not Found (Request is null or empty)

        Gets information about Project (IoT Server instalation place, installer, etc.)
        URI: /data/getprojectinfo
        Method: GET
        Response: SystemDB.Model.ProjectInfo object
        HTTP status code: 200 OK, 404 Not Found (table is empty)
