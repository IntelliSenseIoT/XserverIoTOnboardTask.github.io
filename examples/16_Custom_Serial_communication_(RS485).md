# Example 16 - Custom Serial communication (RS485)

### Prerequisites:

  - Required Xserver.IoT firmware: 10.2.3
  - IoT Server connects to one serial device on RS485
  - Configure IoT Server with IoT Explorer

![](images/TestMeter.png)
  
## Example:

The following example shows how to send custom message (or protocol) on RS485 interface

## Code:

....
using Driver.Device;
using Driver.Device.Interface;
using System.Threading.Tasks;
using SystemDB.Model;
using System.Diagnostics;

        ....

        #region Helpers
        ....
        //Serial port hardware Id
        private const string SerialPortDeviceId = @"\\?\ACPI#BCM2836#0#{86e0d1e0-8089-11d0-9ce4-08003e301f73}";
        ResultSerialPortSettings SerialSettings = new ResultSerialPortSettings();
        Extension DeviceExt = new Extension();
        //Information about serial communication
        bool ReceivedAnswer;
        bool NotReceived;
        #endregion

        ....
        
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
          
            ....

            //Serial Port Initialize
            SerialSettings = await SerialPort.GetSettings();
            var res = await SerialPortInitialize();
            DeviceExt.SlaveRequestEvent += DeviceExt_ClientRequestEvent;

            .....
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
                NotReceived = false;
                ReceivedAnswer = false;

                Stopwatch stopWatch = new Stopwatch();

                byte[] message = {1,3,0,0,0,2,196,11};  //Your Message
                var res = await DeviceExt.SendComMessage(message, SerialPortDeviceId);  //Send serial message

                if (res.Success == true)
                {
                    stopWatch.Reset();
                    stopWatch.Start();

                    while (stopWatch.Elapsed.TotalMilliseconds < SerialSettings.SerialTimeOut && ReceivedAnswer == false)
                    {
                        await Task.Delay(5);
                    }
                    stopWatch.Stop();

                    if (ReceivedAnswer)
                    {
                        
                    }
                    else
                    {
                        NotReceived = true;
                       
                    }
                }
                else
                {
                    NotReceived = true;
                }
            }
            catch (Exception ex)
            {
                EventLogging.AddLogMessage(MessageType.ExceptionError, this.GetType().Name + " - " + ServiceDisplayName + " - " + "OnboardTask exception error! Error: " + ex.Message);
            }
            OnboardTaskHandler.Run();  //Task continues to run
        }

        ....
        
        //Initialize RS485 Serial Port
        private async Task<bool> SerialPortInitialize(bool reinitialize = false)
        {
            bool result = false;

            try
            {
                CommonFunctions.ComPortSettings findcom = null;

                if (reinitialize == true)
                {
                    //Dispose objects
                    try
                    {
                        DeviceExt.Dispose();
                    }
                    catch (Exception)
                    {
                        //Object disposed
                    }
                    DeviceExt = null;

                    await Task.Delay(5000);
                    //Recrate objects
                    DeviceExt = new Extension();

                    int t = 0;
                    while (t < 10 && findcom == null)
                    {
                        try
                        {
                            findcom = DeviceExt.GetComPortSettings(SerialPortDeviceId);
                        }
                        catch (Exception)
                        {
                            //Error
                        }

                        if (findcom == null)
                        {
                            await Task.Delay(15000);
                        }
                        t = +1;
                    }
                }
                else
                {
                    findcom = DeviceExt.GetComPortSettings(SerialPortDeviceId);
                }

                if (findcom != null)
                {
                    var portopen = findcom.PortOpen;
                    if (portopen == true)
                    {
                        var res = DeviceExt.CloseComPort(SerialPortDeviceId);
                        if (res.Success == false)
                        {
                            EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - " + "It is not able to close serial port! " + res.ErrorMessage);
                        }
                        else
                        {
                            portopen = false;
                        }
                    }
                    if (portopen == false)
                    {
                        if (SerialSettings.SerialBaudRate == SerialBaudRate.BR_9600)
                        {
                            findcom.BaudRate = Driver.Device.Interface.CommonFunctions.SerialBaudRate.BR_9600;
                        }
                        else if (SerialSettings.SerialBaudRate == SerialBaudRate.BR_19200)
                        {
                            findcom.BaudRate = Driver.Device.Interface.CommonFunctions.SerialBaudRate.BR_19200;
                        }
                        else
                        {
                            findcom.BaudRate = Driver.Device.Interface.CommonFunctions.SerialBaudRate.BR_38400;
                        }

                        if (SerialSettings.SerialParity == SerialParity.Even)
                        {
                            findcom.Parity = Driver.Device.Interface.CommonFunctions.SerialParity.Even;
                        }
                        else if (SerialSettings.SerialParity == SerialParity.Odd)
                        {
                            findcom.Parity = Driver.Device.Interface.CommonFunctions.SerialParity.Odd;
                        }
                        else
                        {
                            findcom.Parity = Driver.Device.Interface.CommonFunctions.SerialParity.None;
                        }

                        findcom.ReadTimeout = 20;
                        findcom.WriteTimeout = 20;

                        var res = await DeviceExt.OpenComPort(SerialPortDeviceId);
                        if (res.Success == false)
                        {
                            EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Serial port configuration error! " + res.ErrorMessage);
                        }
                        else
                        {
                            result = true;
                            EventLogging.AddLogMessage(MessageType.Info, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Serial port configuration success.");
                        }
                    }
                }
                else
                {
                    EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Can't find serial port!");
                }
            }
            catch (Exception ex)
            {
                EventLogging.AddLogMessage(MessageType.ExceptionError, this.GetType().Name + " - " + ServiceDisplayName + " - " + "Serial port configuration exception error! " + ex.Message);
            }

            return result;
        }

        //Serial answer
        private void DeviceExt_ClientRequestEvent(object sender, CommonFunctions.SlaveRequestEventArgs e)   
        {
            if (NotReceived == false)
            {
                ReceivedAnswer = true;
                var result = "ClientRequestEvent: " + e.Success.ToString() + " " + e.DeviceId.ToString();

                string received = "";

                for (int i = 0; i < e.ReadBytes.Length - 1; i++)
                {
                    received += " " + e.ReadBytes[i].ToString();
                }

                var Answer = received;
            }
        }
        
