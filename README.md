# Arduino Sensors Monitor PC
## Overview

This library gives access to the data from sensors send by custom [Arduino firmware](https://github.com/vitaliy1919/Arduino-Universal-Sensor-Firmware).

## Usage

Using this library is pretty simple (see `Program.cs` for an example).

Just add `AndroidMonitor.cs` to your program.

Then create monitor:
``` csharp
var monitor = new SensorDataMonitor();
monitor.StartMonitoring("<port arduino connected to>", 9600);
```

Connect your callback to it:
``` csharp
monitor.sensorTypeCallback += eventCallback;
```

The callback has the following signature:
``` csharp 
static void eventCallback(sensors_event_t e)
```

See [Adafruit universal sensor](https://github.com/adafruit/Adafruit_Sensor) for the documentation on `sensors_event_t`

If you're using this library in console application, you have to also add
``` csharp
monitor.Wait();
```
Otherwise, the program finishes preliminary and the callback doesn't work.