# Introduction:

- Xserver.IoT devices can easily transfer data from the field devices to Cloud and On-Premises (local SQL server) applications. Field devices include meters, sensors, PLCs, trip units, motor controls, inverters, heat pumps, EV charges and other devices.

- Easy to integrate into your corporate system (SAP, Energy Management Software, Manufacturing Execution Systems, Building Management Software, Smart City software, Power BI, etc.) with flexible connectivity (REST API, Azure IoT HUB, SQL Server, Extendable connectivity via .NET Core SDK)

- This capabilities allow the use of reporting, analysis, and AI software to access information from devices for data collection, trending, alarm/event management, analysis, remote control, and other functions.

# Xserver.IoT OnboardTask Overview:

With the Onboard Task project can be implemented customized tasks (Industrial PC communication, Custom protocol matching, Control tasks, Remote parameter setting from cloud, Control with Artificial Intelligence, etc.).

- More details: https://www.intellisense-iot.com/
- [Technical overview](https://www.youtube.com/watch?v=_fmbNuYwyqE&list=UUcLou6GZjtQRWgN1ukcglpg&index=14)        
- [Nugets](https://www.nuget.org/packages/XserverIoTCommon/)
- [Troubleshootings](https://github.com/IntelliSenseIoT/XserverIoTOnboardTask.github.io/blob/master/Troubleshooting.md)

![](images/SDKOverview.png)

**Xserver.IoT connectivity overview**

![](images/ConnectionTechOverview2.png)

# XserverIoTOnboardTask:

## Required UWP Target settings

    Min version: Windows 10 Fall Creators Update (10.0; Build 16299) 

## Required Xserver.IoT firmware

    Min version: 10.2

## Required UWP Capabilities

    <Capability Name="internetClient" />
    <Capability Name="internetClientServer"/>
    <Capability Name="privateNetworkClientServer"/>

# Examples:

[Example 1 - Real-time values](https://github.com/IntelliSenseIoT/XserverIoTOnboardTask.github.io/blob/master/examples/1_Real-time%20values.md)

[Example 2 - IoT Server and OPCUA Server communication](https://github.com/IntelliSenseIoT/XserverIoTOnboardTask.github.io/blob/master/examples/2_IoT%20Server%20and%20OPCUA%20Server%20communication.md)

[Example 3 - OPC UA Real-time value(s) logging](https://github.com/IntelliSenseIoT/XserverIoTOnboardTask.github.io/blob/master/examples/3_OPC%20UA%20Real-time%20value(s)%20logging.md)

