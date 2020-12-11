# Example 10 - Access another IoT server from onboard task

In this example below, we will connect to the services of another IoT server.

##Code:

        ....
       
        //The request is redirected to the other IoT server
        XserverIoTConnectivityInterface.LastIP = "10.29.2.12";
        XserverIoTConnectivityInterface.Reinitialize = true;

        var resultfromotherIoTServer = await XserverIoTConnectivityInterface.RestClientGET("/com/status", ServiceName.Com);

        //The request to the local server
        XserverIoTConnectivityInterface.LastIP = "localhost";
        XserverIoTConnectivityInterface.Reinitialize = true;

        .......

            
        
