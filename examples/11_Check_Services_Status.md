# Example 11 - Check the running of services (Data,Com,Core)

After starting the IoT device, the first step is to check the service you want to access. 

  var com = await Services.ComIsInitialized();
  var data = await Services.DataIsInitialized();
  var core = await Services.CoreIsInitialized();

Example:

     .....

     public async void Run(IBackgroundTaskInstance taskInstance)
     {
          _Deferral = taskInstance.GetDeferral();

          EventLogging.Initialize();
          EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Start initializing...");

          bool exit = false;
          while (exit == false)
          {
              var com = await Services.ComIsInitialized();
              var data = await Services.DataIsInitialized();
              if (com.Initialized == true && data.Initialized==true)
              {
                  exit = true;
              }
              await Task.Delay(5000);
          }

         //.....

         EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Finished initialization.");
    }

