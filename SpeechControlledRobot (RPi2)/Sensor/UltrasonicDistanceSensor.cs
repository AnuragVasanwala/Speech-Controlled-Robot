/*
https://github.com/peejster/Rover/blob/master/Rover/UltrasonicDistanceSensor.cs
http://www.guruumeditation.net/en/digital-io-with-windows-10-iot-raspberry-pi-2-and-the-ultrasonic-ranging-module-hc-sr04/
*/
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace SpeechControlledRobot__RPi2_.Sensor
{
    public class UltrasonicDistanceSensor
    {
        private GpioPin Pin_Trig, Pin_Echo;
        private Stopwatch _StopWatch;
        private double? _Distance;
        
        /// <summary>
        /// Available Gpio Pins. Refer: https://ms-iot.github.io/content/en-US/win10/samples/PinMappingsRPi2.htm
        /// </summary>
        public enum AvailableGpioPin : int
        {
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 29
            /// </summary>
            GpioPin_5 = 5,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 31
            /// </summary>
            GpioPin_6 = 6,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 32
            /// </summary>
            GpioPin_12 = 12,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 33
            /// </summary>
            GpioPin_13 = 13,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 36
            /// </summary>
            GpioPin_16 = 16,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 12
            /// </summary>
            GpioPin_18 = 18,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 15
            /// </summary>
            GpioPin_22 = 22,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 16
            /// </summary>
            GpioPin_23 = 23,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 18
            /// </summary>
            GpioPin_24 = 24,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 22
            /// </summary>
            GpioPin_25 = 25,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 37
            /// </summary>
            GpioPin_26 = 26,
            /// <summary>
            /// Raspberry Pi 2 - Header Pin Number : 13
            /// </summary>
            GpioPin_27 = 27
        }


        public UltrasonicDistanceSensor(AvailableGpioPin TrigPin, AvailableGpioPin EchoPin)
        {
            _StopWatch = new Stopwatch();

            var gpio = GpioController.GetDefault();

            Pin_Trig = gpio.OpenPin((int)TrigPin);
            Pin_Trig.SetDriveMode(GpioPinDriveMode.Output);
            Pin_Trig.Write(GpioPinValue.Low);

            Pin_Echo = gpio.OpenPin((int)EchoPin);
            Pin_Echo.SetDriveMode(GpioPinDriveMode.Input);
        }
        
        public double GetDistance()
        {
            var mre = new ManualResetEventSlim(false);


            //Send a 10µs pulse to start the measurement
            Pin_Trig.Write(GpioPinValue.High);
            mre.Wait(TimeSpan.FromMilliseconds(0.01));
            Pin_Trig.Write(GpioPinValue.Low);

            var time = PulseIn(Pin_Echo, GpioPinValue.High,500);

            // multiply by speed of sound in milliseconds (34000) divided by 2 (cause pulse make rountrip)
            var distance = time * 17000;
            return distance;
        }

        private double PulseIn(GpioPin pin, GpioPinValue value, ushort timeout)
        {
            var sw = new Stopwatch();
            var sw_timeout = new Stopwatch();

            sw_timeout.Start();

            // Wait for pulse
            while (pin.Read() != value)
            {
                if (sw_timeout.ElapsedMilliseconds > timeout)
                    return 3.5;
            }
            sw.Start();

            // Wait for pulse end
            while (pin.Read() == value)
            {
                if (sw_timeout.ElapsedMilliseconds > timeout)
                    return 3.4;
            }
            sw.Stop();

            return sw.Elapsed.TotalSeconds;
        }
    }
}
