# XserverIoTOnboardTask:

## Required UWP Target settings:

    Min version: Windows 10 Fall Creators Update (10.0; Build 16299) 

## Required UWP Capabilities:

    <Capability Name="internetClient" />
    <Capability Name="internetClientServer"/>
    <Capability Name="privateNetworkClientServer"/>

## Before use app, enable loopback in Windows 10 IoT Core::

    checknetisolation loopbackexempt -a -n='XServerIoTOnboardTaskProject-uwp_39mgpzy4q2jkm'


