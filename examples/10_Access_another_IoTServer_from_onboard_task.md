# Example 10 - Access another IoT server from onboard task

In this example below, we connected to the services of another IoT server.

More details about OnboardTask REST API functions: 

[ - XserverIoTCommon class doumentation](https://github.com/IntelliSenseIoT/XserverIoTOnboardTask.github.io/blob/master/XserverIoTCommon.md)

[ - REST API interface documentation](https://github.com/IntelliSenseIoT/XserverIoTOnboardTask.github.io/blob/master/XserverIoT_RestAPI_Interface_doumentation.md)


## Code:

        ....
       
        //The request is redirected to the other IoT server
        XserverIoTConnectivityInterface.LastIP = "10.29.2.12";
        XserverIoTConnectivityInterface.Reinitialize = true;

        var resultfromotherIoTServer = await XserverIoTConnectivityInterface.RestClientGET("/com/status", ServiceName.Com);

        //The request back to the local server
        XserverIoTConnectivityInterface.LastIP = "localhost";
        XserverIoTConnectivityInterface.Reinitialize = true;

        .......

            
        
