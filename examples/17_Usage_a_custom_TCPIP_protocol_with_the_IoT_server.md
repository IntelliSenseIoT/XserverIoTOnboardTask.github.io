# Example 17 - Usage a custom TCPIP protocol with the IoT server

### Prerequisites:

  - Required Xserver.IoT firmware: 10.2.x
  - IoT Server connects to one TCP/IP device on Ethernet
  - Configure IoT Server with IoT Explorer

## Communication specifications

  LAN side protocol: TCP / IP or UDP / IP (depending on Xport setting)
  Specify by IP address (***. ***. ***. ***)
  Local port number: 10001
  Remote port number: 10002
  ‑> Put the command in an IP packet and perform LAN communication.

## Example:

The following example shows how to send custom TCP/IP message (or protocol) on Ethernet interface with OnboardTask

## Code:

    ....
    using IO.TCPIPClient;

        ....

        #region Helpers
        ....
        TCPIPClient TCPClient = new TCPIPClient();
        #endregion

        ....
        
        /// <summary>
        /// IoT Onboard Task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnboardTask(object sender, EventArgs e)
        {
            try
            {
                byte[] SendMessage = { 
                    0x02,       // 02H:STX(Start Mark) Boiler ControlNumber
                    0x01,       // 0x01‑0xFF:HEX Number
                    0x30,       // Refer to "command code table'Refer
                    0x32,       // to" sub command code table
                    0x30,       // Reserve 30H Everytime
                    0x30,       // Reserve 30H Everytime
                    0xFA,       // SUM value (lower 8 bits when adding data up to the previous time)
                    0x03};      // 03H:ETX(Separator)

                var result = await TCPClient.SendClientRequest("10.29.2.126", "8007", SendMessage);

                var answer = result.ReadBytes; 
            }
            catch (Exception ex)
            {
                EventLogging.AddLogMessage(MessageType.ExceptionError, this.GetType().Name + " - " + ServiceDisplayName + " - " + "OnboardTask exception error! Error: " + ex.Message);
            }
            OnboardTaskHandler.Run();  //Task continues to run
        }

   ....
        
      
        
