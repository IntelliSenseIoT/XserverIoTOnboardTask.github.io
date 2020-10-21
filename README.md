# Introduction:

Xserver.IoT.Framework can easily manage data from field devices to Cloud and SQL server applications. Field devices include meters, sensors, PLCs, trip units, motor controls, and other devices.
This capability allows the use of reporting, analysis and AI software (Machine Learning, Power BI, SAP, Energy Management, Smart City software, etc.) to access information from devices for data collection, trending, alarm/event management, analysis, and other functions.

- More details: https://www.intellisense-iot.com/
- [Technical overview](https://www.youtube.com/watch?v=_fmbNuYwyqE&list=UUcLou6GZjtQRWgN1ukcglpg&index=14)        
- [Nugets](https://www.nuget.org/packages/XserverIoTCommon/)

![](images/XserverIoTConnectivity.png)

## Cloning the project with Visual Studio 2019 - No certificate found with the supplied thumbprint

    Solution:
    
    Right click the project -> Properties -> Package Manifest
    On the Package.appxmanifest go to Packaging tab -> Choose Certificate
    In the new window click "Select a Certificate..." if you have one, or create a certificate if you haven't created one

[More details](https://stackoverflow.com/questions/57578299/uwp-no-certificate-found-with-the-supplied-thumbprint)
    
# XserverIoTOnboardTask:

## Required UWP Target settings

    Min version: Windows 10 Fall Creators Update (10.0; Build 16299) 

## Required Xserver.IoT firmware

    Min version: 10.2

## Required UWP Capabilities

    <Capability Name="internetClient" />
    <Capability Name="internetClientServer"/>
    <Capability Name="privateNetworkClientServer"/>

## Before use app, enable loopback on the Windows 10 IoT Core (Before version 10.2)

[More details...](https://github.com/IntelliSenseIoT/XserverIoTOnboardTask.github.io/blob/master/Enable%20loopback%20on%20the%20Windows%2010%20IoT%20Core.md)

# Examples:

[Example 1 - Real-time values](https://github.com/IntelliSenseIoT/XserverIoTOnboardTask.github.io/blob/master/examples/Real-time%20values.md)
[Example 2 - IoT Server and OPCUA Server communication](https://github.com/IntelliSenseIoT/XserverIoTOnboardTask.github.io/blob/master/examples/IoT%20Server%20and%20OPCUA%20Server%20communication.md)
[Example 3 - OPC UA Real-time value(s) logging](https://github.com/IntelliSenseIoT/XserverIoTOnboardTask.github.io/blob/master/examples/OPC%20UA%20Real-time%20value(s)%20logging.md)

