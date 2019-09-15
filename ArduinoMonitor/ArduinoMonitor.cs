using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Collections.Concurrent;
using System.Web.Script.Serialization;
namespace ArduinoMonitor
{
    delegate void SensorDataArrived(sensors_event_t sensorEvent);
    enum SensorType
    {
        None = 0,
        SENSOR_TYPE_ACCELEROMETER = (1<<1),   /**< Gravity + linear acceleration */
        SENSOR_TYPE_MAGNETIC_FIELD = (1<<2),
        SENSOR_TYPE_ORIENTATION = (1 << 3),
        SENSOR_TYPE_GYROSCOPE = (1 << 4),
        SENSOR_TYPE_LIGHT = (1 << 5),
        SENSOR_TYPE_PRESSURE = (1 << 6),
        SENSOR_TYPE_PROXIMITY = (1 << 8),
        SENSOR_TYPE_GRAVITY = (1 << 9),
        SENSOR_TYPE_LINEAR_ACCELERATION = (1 << 10),  /**< Acceleration not including gravity */
        SENSOR_TYPE_ROTATION_VECTOR = (1 << 11),
        SENSOR_TYPE_RELATIVE_HUMIDITY = (1 << 12),
        SENSOR_TYPE_AMBIENT_TEMPERATURE = (1 << 13),
        SENSOR_TYPE_VOLTAGE = (1 << 15),
        SENSOR_TYPE_CURRENT = (1 << 16),
        SENSOR_TYPE_COLOR = (1 << 17),
        ALL = ~None
    }
    class SensorDataMonitor
    {
        private ReaderWriterLock readWriteLock ;
        private SerialPort serialPort;
        private Thread monitoringThread;
        private SensorType callbackTypes = SensorType.ALL;
        public event SensorDataArrived sensorTypeCallback;
        private volatile bool monitor = true;
        public void StopMonitoring() 
        {
            lock (this)
            { 
                monitor = false;
            }
        }
        public void Wait()
        {
            monitoringThread.Join();
        }
        public void StartMonitoring(string arduinoPort, int baudRate = 9600)
        {
            serialPort = new SerialPort(arduinoPort, baudRate);

            serialPort.Open();
            monitoringThread = new Thread(() =>
            {
                while (monitor)
                {
                    try
                    {
                        string message = serialPort.ReadLine();
                        if (message[0] == '{')
                        {
                            try
                            {
                                var example2Model = new JavaScriptSerializer().Deserialize<sensors_event_t>(message);
                                lastSensorData[example2Model.sensor_id] = example2Model;
                                sensorTypeCallback(example2Model);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                    catch (TimeoutException) { }
                }
               
                
                serialPort.Close();
            });

            monitoringThread.Start();

        }
        public ConcurrentDictionary<Int32, sensors_event_t> lastSensorData = new ConcurrentDictionary<int, sensors_event_t>();
        public sensors_event_t GetLatestSensorReading(Int32 sensorId)
        {

            lastSensorData.TryGetValue(sensorId, out sensors_event_t sensorEvent);
            return sensorEvent;

        }
        public void SetCallbackSensorTypes(SensorType types) { callbackTypes = types; }

    }
    enum sensors_type_t
    {
        SENSOR_TYPE_ACCELEROMETER = (1),   /**< Gravity + linear acceleration */
        SENSOR_TYPE_MAGNETIC_FIELD = (2),
        SENSOR_TYPE_ORIENTATION = (3),
        SENSOR_TYPE_GYROSCOPE = (4),
        SENSOR_TYPE_LIGHT = (5),
        SENSOR_TYPE_PRESSURE = (6),
        SENSOR_TYPE_PROXIMITY = (8),
        SENSOR_TYPE_GRAVITY = (9),
        SENSOR_TYPE_LINEAR_ACCELERATION = (10),  /**< Acceleration not including gravity */
        SENSOR_TYPE_ROTATION_VECTOR = (11),
        SENSOR_TYPE_RELATIVE_HUMIDITY = (12),
        SENSOR_TYPE_AMBIENT_TEMPERATURE = (13),
        SENSOR_TYPE_VOLTAGE = (15),
        SENSOR_TYPE_CURRENT = (16),
        SENSOR_TYPE_COLOR = (17)
    };
    class sensors_color_t
    {
        public float[] c;
        public float r;
        public float g;
        public float b;
        UInt32 rgba;

        sensors_color_t()
        {
            c = new float[3];
            r = g = b = 0f;
            rgba = 0;
        }

    }
    class sensors_vec_t
    {
        public float[] v;
        public float x;
        public float y;
        public float z;
        public float roll;
        public float pitch;
        public float heading;
        public Int16 status;
        public UInt16[] reserved;

      
    }

    class sensors_event_t
    {
        public Int32 version;
        public Int32 sensor_id;
        public sensors_type_t type;
        public Int32 reserved0;
        public float[] data;
        public sensors_vec_t acceleration;         /**< acceleration values are in meter per second per second (m/s^2) */
        public sensors_vec_t magnetic;             /**< magnetic vector values are in micro-Tesla (uT) */
        public sensors_vec_t orientation;          /**< orientation values are in degrees */
        public sensors_vec_t gyro;                 /**< gyroscope values are in rad/s */
        public float temperature;          /**< temperature is in degrees centigrade (Celsius) */
        public float distance;             /**< distance in centimeters */
        public float light;                /**< light in SI lux units */
        public float pressure;             /**< pressure in hectopascal (hPa) */
        public float relative_humidity;    /**< relative humidity in percent */
        public float current;              /**< current in milliamps (mA) */
        public float voltage;              /**< voltage in volts (V) */
        public sensors_color_t color;

      
    }
    class ArduinoMonitor
    {
        public int sensorId;
        public double humidity;
        public double temperature;
    }
}
