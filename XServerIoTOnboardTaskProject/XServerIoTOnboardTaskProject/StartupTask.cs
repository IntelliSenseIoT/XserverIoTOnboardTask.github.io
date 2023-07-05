using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Service.Common;
using File.Log;
using XserverIoTCommon;
using IO.SimpleHttpServer;
using Newtonsoft.Json;
using Xserver.IoT.Connectivity.Interface;

namespace XServerIoTOnboardTaskProject
{
    /// <summary>
    /// Xserver.IoT.100 OnboardTask
    /// Use the code below to easily write a Windows IoT Core OnboardTask to Xserver.IoT framework.
    /// Created by IntelliSense Ltd.
    /// Website: http://www.intellisense-iot.com/
    /// </summary>

    public sealed class StartupTask : IBackgroundTask
    {
        #region XServerIoTOnboardTask service settings
        //Service display name
        private const string ServiceDisplayName = "Xserver.OnboardTask";
        //Task Handler Period (ms)
        private const int TaskHandlerPeriod = 1000;
        #endregion

        #region Helpers
        //Log service events and errors
        TaskHandler OnboardTaskHandler = new TaskHandler();
        HttpRestServerService RestServer = new HttpRestServerService();
        #endregion

        private static BackgroundTaskDeferral _Deferral = null;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _Deferral = taskInstance.GetDeferral();

            await EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Start initializing...");

            //Todo: Write your initial code here

            //Initialize Http REST server
            await RestServer.HttpRESTServerStart();
            RestServer.ClientEvent += HttpRestServer_ClientRequestEvent;

            //Initialize and Start IoT OnboardTask
            OnboardTaskHandler.WaitingTime = TaskHandlerPeriod;
            OnboardTaskHandler.ThresholdReached += OnboardTask;
            OnboardTaskHandler.Run();

            await EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Finished initialization.");
        }

        /// <summary>
        /// IoT Onboard Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnboardTask(object sender, EventArgs e)
        {
            try
            {
                //Todo: Type your onboard task code here
            }
            catch (Exception ex)
            {
                await EventLogging.AddLogMessage(MessageType.ExceptionError, this.GetType().Name + " - " + ServiceDisplayName + " - " + "OnboardTask exception error! Error: " + ex.Message);
            }
            OnboardTaskHandler.Run();  //Task continues to run
        }

        private async void HttpRestServer_ClientRequestEvent(object sender, HttpRestServerService.ClientRequestEventArgs e)
        {
            IO.SimpleHttpServer.Result res = new IO.SimpleHttpServer.Result();

            try
            {
                if (e.RequestMethod == RequestMethodType.GET)
                {
                    //Todo: Type your code here
                    // Example:
                    //if (e.uriString.ToLower() == "/onboardtask/examplegeturi")
                    //{
                    //    string content = JsonConvert.SerializeObject(YourObject);
                    //    res = await RestServer.ServerResponse(HTTPStatusCodes.OK, e.OStream, content);
                    //}
                }
                else if (e.RequestMethod == RequestMethodType.POST)
                {
                    //Todo: Type your code here
                    // Example:
                    //if (e.uriString.ToLower() == "/onboardtask/exampleposturi")
                    //{  
                    //    YourObject MyObj = JsonConvert.DeserializeObject<YourObject>(e.HttpContent);
                    //    ....
                    //    string content = JsonConvert.SerializeObject(answer);
                    //    res = await RestServer.ServerResponse(HTTPStatusCodes.OK, e.OStream, content);
                    //}
                }
                else
                {
                    res = await RestServer.ServerResponse(HTTPStatusCodes.Not_Found, e.OStream, null);
                }
            }
            catch (Exception ex)
            {
                await EventLogging.AddLogMessage(MessageType.ExceptionError, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Http REST server exception error! Error: " + ex.Message);
            }
        }
    }
}
