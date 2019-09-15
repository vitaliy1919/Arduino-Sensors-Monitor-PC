using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;     
using System.Web.Script;
using System.Web.Script.Serialization;

namespace ArduinoMonitor
{
    class Program
    {
        static bool _continue = true;
        static SerialPort _serialPort;
        static SensorDataMonitor monitor;
        static void Main(string[] args)
        {

            monitor = new SensorDataMonitor();
            monitor.StartMonitoring("COM4", 9600);

            // Event based approach to gather events
            monitor.sensorTypeCallback += eventCallback;

            // Loop based approach to gather events
            // Event based approach is preffered though

            /* while (true)
            {
                sensors_event_t e = monitor.GetLatestSensorReading(0);
                if (e == null)
                    Console.WriteLine("No event");
                else if (e.type == sensors_type_t.SENSOR_TYPE_ACCELEROMETER)
                    Console.WriteLine($"Accelerometer: {e.acceleration.x}, {e.acceleration.y}, {e.acceleration.z}");
                else if (e.type == sensors_type_t.SENSOR_TYPE_GYROSCOPE)
                    Console.WriteLine($"Gyroscope: {e.gyro.x}, {e.gyro.y}, {e.gyro.z}");
                else if (e.type == sensors_type_t.SENSOR_TYPE_AMBIENT_TEMPERATURE)
                    Console.WriteLine($"Temperature: {e.temperature}");
                Thread.Sleep(50);
            } */

            // Use it when using event based approach
            monitor.Wait();
           
        }
        static int count = 0;
        static void eventCallback(sensors_event_t e)
        {
            count++;
            if (e == null)
                Console.WriteLine("No event");
            else if (e.type == sensors_type_t.SENSOR_TYPE_ACCELEROMETER)
                Console.WriteLine($"Accelerometer: {e.acceleration.x}, {e.acceleration.y}, {e.acceleration.z}");
            else if (e.type == sensors_type_t.SENSOR_TYPE_GYROSCOPE)
                Console.WriteLine($"Gyroscope: {e.gyro.x}, {e.gyro.y}, {e.gyro.z}");
            else if (e.type == sensors_type_t.SENSOR_TYPE_AMBIENT_TEMPERATURE)
                Console.WriteLine($"Temperature: {e.temperature}");
            //if (count % 10 == 0)
            //    Console.WriteLine($"count = {count}");
            //if (count >= 100)
            //    monitor.StopMonitoring();


        }
        //public static void Read()
        //{
        //    while (_continue)
        //    {
        //        try
        //        {
        //            string message = _serialPort.ReadLine();
        //            if (message[0] == '{')
        //            {
        //                try
        //                {
        //                    var example2Model = new JavaScriptSerializer().Deserialize<sensors_event_t>(message);
        //                    Console.WriteLine("Event get");
        //                } catch (Exception e)
        //                {
        //                    Console.WriteLine(e.Message);
        //                }
        //            }
        //        }
        //        catch (TimeoutException) { }
        //    }
        //}
    }
}
