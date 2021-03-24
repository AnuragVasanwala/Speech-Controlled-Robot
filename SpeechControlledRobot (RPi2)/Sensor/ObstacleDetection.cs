using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechControlledRobot__RPi2_.Sensor
{
    public class ObstacleDetection
    {
        public enum DetectionState
        {
            /// <summary>
            /// Minimum distance breached, obstacle ahead
            /// </summary>
            Detected,
            /// <summary>
            /// There is no obstacle for up-to specified MinDistance
            /// </summary>
            Clear
        }

        /// <summary>
        /// Raises when obstacle detected or it is clear
        /// </summary>
        public event Action<DetectionState, double> ObstacleDetected;

        /// <summary>
        /// Get or set minimum distance(in cm) between obstacle and sensor to fire ObstacleDetected event. Default is 30cm
        /// </summary>
        public double MinDistance = 30;

        private DetectionState CurrentState = DetectionState.Clear;

        public ObstacleDetection(UltrasonicDistanceSensor _Sensor, int ScanResolutionInMs = 100)
        {
            // initialize CurrentDistance to zero
            double CurrentDistanceInCm = 0;

            // Create a separate Task to scan distance between obstacle and sensor
            Task.Factory.StartNew(async () =>
            {
                // Loop infinity until the power is down
                while (true)
                {
                    // Verify current distance ahead and raise event if MinDistance distance is breached
                    if ((CurrentDistanceInCm = _Sensor.GetDistance()) <= MinDistance && CurrentState == DetectionState.Clear)
                    {
                        // Raise event only if it is not null
                        if (ObstacleDetected != null)
                        {
                            ObstacleDetected(DetectionState.Detected, CurrentDistanceInCm);
                        }
                        // Set current state
                        CurrentState = DetectionState.Detected;
                    }
                    else if (CurrentDistanceInCm > MinDistance && CurrentState == DetectionState.Detected) // Raise event if distance between object and sensor is greater than MinDistance
                    {
                        // Raise event only if it is not null
                        if (ObstacleDetected != null)
                        {
                            ObstacleDetected(DetectionState.Clear, CurrentDistanceInCm);
                        }
                        // Set current state
                        CurrentState = DetectionState.Clear;
                    }

                    // Wait for the next scan time
                    await Task.Delay(ScanResolutionInMs);
                }
            });
        }
    }
}
