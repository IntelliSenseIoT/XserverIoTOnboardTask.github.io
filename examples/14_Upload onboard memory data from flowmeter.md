# Example 14 - Upload onboard data from flow meter

the following properties can be queried from the OnboardTask application: 
  - Source
  - TemplateDevice
  - TemplateDevice Quanities

## Object overview:

     ┌─────────────────────────┐
     │       Source 1          │
     │                         │
     │                         │
     │                         │
     │                         ├─────────┐
     │                         │         │
     └─────────────────────────┘         │
                                         │
     ┌─────────────────────────┐         │
     │       Source 2          │         │
     │                         │         │
     │                         ├───────┐ │
     │                         │       │ │
     │                         │       │ │
     │                         │      ┌┴─┴───────────────────┐
     └─────────────────────────┘      │   Template Device 1  │
                                      │                      │
                                      │       defines        │
                                      │   the device type    │
                                      │ inherits templatedevi│
                                      └─────────────┬────────┘
                                                    │
                                                    │
                                                    │   ┌───────────────────────────┐
                                                    │   │    Template Quaintities   │
                                                    └───┤                           │
                                                        │                           │
                                                        │      Quantity 1           │
                                                        │                           │
                                                        │      Quantity 2           │
                                                        │                           │
                                                        │      ..                   │
                                                        └───────────────────────────┘


## Example:

    Realtime FieldDevices = new Realtime();
    
    //Login to Xserver.IoT Service (User settings can be set with IoT Explorer)
    var res = await Authentication.Login("onboardtask", "onboardtask");
    if (res.Success == false)
    {
        EventLogging.AddLogMessage(MessageType.Error, this.GetType().Name + " - " + ServiceDisplayName + " - Authentication error! " + res.ErrorMessage);
    }
    
    //Get Sources and Quantities list
    var result = await FieldDevices.GetSourcesQuantities();
            
    foreach (var item in FieldDevices.ListOfSources)
    {
        //Gets Source properties
        var tempdev = await FieldDevices.GetSourceProperties(item.SourceId);
        if (tempdev !=null)
        {
            //Gets TemplateDevice properties
            var tempdevprop = await FieldDevices.GetTemplateDeviceProperties(tempdev.TemplateDeviceId);
           
            //Gets TemplateDevice Quantities properties
            var tempdevquans = await FieldDevices.GetTemplateDeviceQuantitiesProperties(tempdev.TemplateDeviceId);
            if (tempdevquans !=null)
            {
                foreach (var tempq in tempdevquans)
                {
                    //my code.........
                }
            }
        }
    }
