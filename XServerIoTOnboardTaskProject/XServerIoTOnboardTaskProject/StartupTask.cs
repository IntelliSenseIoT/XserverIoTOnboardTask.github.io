using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Service.Common;
using File.Log;
using XserverIoTCommon;
using OPCUA.Library;

namespace XServerIoTOnboardTaskProject
{
    /// <summary>
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
        #endregion

        private static BackgroundTaskDeferral _Deferral = null;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _Deferral = taskInstance.GetDeferral();

            EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Start initializing...");

            //Todo: Before use this code, enable loopback in Windows 10 IoT Core: checknetisolation loopbackexempt -a -n='XServerIoTOnboardTaskProject-uwp_39mgpzy4q2jkm'
            //More details about loopback enable: https://github.com/IntelliSenseIoT/XserverIoTOnboardTask.github.io

            //Todo: Write your initial code here

            //Initialize and Start IoT OnboardTask
            OnboardTaskHandler.WaitingTime = TaskHandlerPeriod;
            OnboardTaskHandler.ThresholdReached += OnboardTask;
            OnboardTaskHandler.Run();

            EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Finished initialization.");
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
                EventLogging.AddLogMessage(MessageType.ExceptionError, this.GetType().Name + " - " + ServiceDisplayName + " - " + "OnboardTask exception error! Error: " + ex.Message);
            }
            OnboardTaskHandler.Run();  //Task continues to run
        }
    }
}
