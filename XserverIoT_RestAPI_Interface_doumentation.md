# Xserver.IoT Rest API interface documentation

## Service ports:

    ComPort - 8001
    DataPort - 8002
    CorePort - 8003
    OnboardTaskPort - 8006

## Authentication info:

    * Necessary to use UserName and Password
    ** Necessary to use only UserName

## HTTP Response:

    200 OK
    404 Not Found
    401 Unauthorized (Service or User authorization error)

## Xserver.Com API reference:

Xserver.Com manages real-time communication to the field devices (Modbus RTU, TCP/IP and Device Extension IO, Watchdog, RTC, Time synchronize).

