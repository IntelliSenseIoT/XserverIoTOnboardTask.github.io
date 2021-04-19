# Example 8 - Read and Save config and properties to Onboard storage of the IoT Server

We have the ability to store properties (parameters, values, etc.) and settings on the onboard storage of the IoT Server.

## Code:

    namespace XServerIoTOnboardTaskProject
    {
            ....

            private static BackgroundTaskDeferral _Deferral = null;

            public async void Run(IBackgroundTaskInstance taskInstance)
            {
                _Deferral = taskInstance.GetDeferral();

                EventLogging.Initialize();
                EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Start initializing...");

                ... 
                
                //Read OnboardTask Config and Properties from Onboard storage of the IoT Server 
                var onboardconfig = await XserverIoTCommon.OnboardTask.GetConfig();
                var onboardproperties = await XserverIoTCommon.OnboardTask.GetProperties();

                //Save to Onboard storage
                //Save Config
                Message NewMessage = new Message();
                NewMessage.DateTimeUTC = DateTime.UtcNow;
                NewMessage.MessageStr = "New Message from OnboardTask";
                string contentconf = JsonConvert.SerializeObject(NewMessage);
                await XserverIoTCommon.OnboardTask.SaveConfig(contentconf);

                //Save properties
                Parameters NewParam = new Parameters();
                NewParam.Light = "ON";
                NewParam.SetTemperature = 30;
                string content = JsonConvert.SerializeObject(NewParam);
                await XserverIoTCommon.OnboardTask.SaveProperties(content);

                ....

                EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Finished initialization.");
            }

            private class Parameters
            {
                public string Light { get; set; }
                public double SetTemperature { get; set; }
            }

            private class Message
            {
                public string MessageStr { get; set; }
                public DateTime DateTimeUTC { get; set; }
            }

          ....
    }
