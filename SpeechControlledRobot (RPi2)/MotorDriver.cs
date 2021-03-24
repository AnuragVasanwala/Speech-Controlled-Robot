using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace SpeechControlledRobot__RPi2_
{
    /// <summary>
    /// Provides abstraction layer to drive robot
    /// </summary>
    public static class MotorDriver
    {
        private static GpioPin MotorLeft_A, MotorLeft_B;
        private static GpioPin MotorRight_A, MotorRight_B;
        private static State _CurrentState;
        
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

        /// <summary>
        /// Available driving states
        /// </summary>
        public enum State : byte
        {
            MovingForward,
            MovingReverse,
            RotatingRight,
            RotatingLeft,
            TurningRight,
            TurningLeft,
            Stopped
        }

        public static State CurrentState
        {
            get
            {
                return _CurrentState;
            }
        }

        /// <summary>
        /// Initializes four pins for motor driver
        /// </summary>
        /// <param name="Pin_MotorLeft_A">Left motor's Pin A.</param>
        /// <param name="Pin_MotorLeft_B">Left motor's Pin B.</param>
        /// <param name="Pin_MotorRight_A">Right motor's Pin A.</param>
        /// <param name="Pin_MotorRight_B">Right motor's Pin B.</param>
        public static void InitializePins(AvailableGpioPin Pin_MotorLeft_A, AvailableGpioPin Pin_MotorLeft_B, AvailableGpioPin Pin_MotorRight_A, AvailableGpioPin Pin_MotorRight_B)
        {
            // Initialize Gpio Controller
            GpioController MyGpioConroller = GpioController.GetDefault();

            // If GpioPins are not null then make them first null to release old pins.
            // Check only one pin against null and if it is not then null all.
            if (MotorLeft_A != null)
            {
                MotorLeft_A = null;
                MotorLeft_B = null;
                MotorRight_A = null;
                MotorRight_B = null;
            }

            // Initialize Pins
            MotorLeft_A = MyGpioConroller.OpenPin((int)Pin_MotorLeft_A);
            MotorLeft_B = MyGpioConroller.OpenPin((int)Pin_MotorLeft_B);

            MotorRight_A = MyGpioConroller.OpenPin((int)Pin_MotorRight_A);
            MotorRight_B = MyGpioConroller.OpenPin((int)Pin_MotorRight_B);

            // Set Drive Mode
            MotorLeft_A.SetDriveMode(GpioPinDriveMode.Output);
            MotorLeft_B.SetDriveMode(GpioPinDriveMode.Output);
            MotorRight_A.SetDriveMode(GpioPinDriveMode.Output);
            MotorRight_B.SetDriveMode(GpioPinDriveMode.Output);

            // Set Current State
            _CurrentState = State.Stopped;
        }

        public static void MoveForward()
        {
            // Return if pins are not initialized
            if (MotorLeft_A == null)
                return;

            // Left Motor
            MotorLeft_A.Write(GpioPinValue.Low);
            MotorLeft_B.Write(GpioPinValue.High);

            // Right Motor
            MotorRight_A.Write(GpioPinValue.Low);
            MotorRight_B.Write(GpioPinValue.High);

            // Set current state
            _CurrentState = State.MovingForward;
        }

        public static void MoveReverse()
        {
            // Return if pins are not initialized
            if (MotorLeft_A == null)
                return;
            
            // Left Motor
            MotorLeft_A.Write(GpioPinValue.High);
            MotorLeft_B.Write(GpioPinValue.Low);

            // Right Motor
            MotorRight_A.Write(GpioPinValue.High);
            MotorRight_B.Write(GpioPinValue.Low);

            // Set current state
            _CurrentState = State.MovingReverse;
        }

        public static void TurnRight()
        {
            // Return if pins are not initialized
            if (MotorLeft_A == null)
                return;

            // Left Motor
            MotorLeft_A.Write(GpioPinValue.Low);
            MotorLeft_B.Write(GpioPinValue.High);

            // Right Motor
            MotorRight_A.Write(GpioPinValue.Low);
            MotorRight_B.Write(GpioPinValue.Low);

            // Set current state
            _CurrentState = State.TurningRight;
        }

        public static void TurnLeft()
        {
            // Return if pins are not initialized
            if (MotorLeft_A == null)
                return;

            // Left Motor
            MotorLeft_A.Write(GpioPinValue.Low);
            MotorLeft_B.Write(GpioPinValue.Low);

            // Right Motor
            MotorRight_A.Write(GpioPinValue.Low);
            MotorRight_B.Write(GpioPinValue.High);

            // Set current state
            _CurrentState = State.TurningLeft;
        }

        public static void RotateRight()
        {
            // Return if pins are not initialized
            if (MotorLeft_A == null)
                return;

            // Left Motor
            MotorLeft_A.Write(GpioPinValue.High);
            MotorLeft_B.Write(GpioPinValue.Low);

            // Right Motor
            MotorRight_A.Write(GpioPinValue.Low);
            MotorRight_B.Write(GpioPinValue.High);

            // Set current state
            _CurrentState = State.RotatingRight;
        }

        public static void RotateLeft()
        {
            // Return if pins are not initialized
            if (MotorLeft_A == null)
                return;

            // Left Motor
            MotorLeft_A.Write(GpioPinValue.Low);
            MotorLeft_B.Write(GpioPinValue.High);

            // Right Motor
            MotorRight_A.Write(GpioPinValue.High);
            MotorRight_B.Write(GpioPinValue.Low);

            // Set current state
            _CurrentState = State.RotatingLeft;
        }

        public static void Stop()
        {
            // Return if pins are not initialized
            if (MotorLeft_A == null)
                return;

            // Left Motor
            MotorLeft_A.Write(GpioPinValue.Low);
            MotorLeft_B.Write(GpioPinValue.Low);

            // Right Motor
            MotorRight_A.Write(GpioPinValue.Low);
            MotorRight_B.Write(GpioPinValue.Low);

            // Set current state
            _CurrentState = State.Stopped;
        }
    }
}
