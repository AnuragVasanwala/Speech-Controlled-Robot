/*
    Refer: https://www.hackster.io/AnuragVasanwala/speech-controlled-robot
*/
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace SpeechControlledRobot__RPi2_
{
    public sealed partial class MainPage : Page
    {
        private SpeechRecognizer MyRecognizer;
        private bool AvoidObstacle = true;

        public MainPage()
        {
            this.InitializeComponent();

            // Initialize Motor Driver Pins
            MotorDriver.InitializePins(MotorDriver.AvailableGpioPin.GpioPin_5, MotorDriver.AvailableGpioPin.GpioPin_6, MotorDriver.AvailableGpioPin.GpioPin_13, MotorDriver.AvailableGpioPin.GpioPin_26);

            // Initialize MyRecognizer and Load Grammar
            InitializeSpeechRecognizer();

            // Initialize front distance sensor
            Sensor.UltrasonicDistanceSensor Front_DistanceSensor = new Sensor.UltrasonicDistanceSensor(Sensor.UltrasonicDistanceSensor.AvailableGpioPin.GpioPin_12, Sensor.UltrasonicDistanceSensor.AvailableGpioPin.GpioPin_16);

            // Initialize ObstacleDetection object
            Sensor.ObstacleDetection Front_ObstacleDetection = new Sensor.ObstacleDetection(Front_DistanceSensor);
            Front_ObstacleDetection.ObstacleDetected += Front_ObstacleDetection_ObstacleDetected;
        }

        private void Front_ObstacleDetection_ObstacleDetected(Sensor.ObstacleDetection.DetectionState arg1, double arg2)
        {
            // If AvoideObstacle is set, check for the detection state
            if (AvoidObstacle == true)
            {
                // Verify detection state and current driving state, if matched, stop robot
                if (arg1 == Sensor.ObstacleDetection.DetectionState.Detected && MotorDriver.CurrentState == MotorDriver.State.MovingForward)
                {
                    MotorDriver.Stop();
                    Debug.WriteLine("Robot stopped! (Obstacle Detected)");
                }
            }
        }

        /// <summary>
        /// Initializes MyRecognizer and Loads Grammar from File 'Grammar\MyGrammar.xaml'
        /// </summary>
        private async void InitializeSpeechRecognizer()
        {
            // Initialize SpeechRecognizer Object
            MyRecognizer = new SpeechRecognizer();

            // Register Event Handlers
            MyRecognizer.StateChanged += MyRecognizer_StateChanged;
            MyRecognizer.ContinuousRecognitionSession.ResultGenerated += MyRecognizer_ResultGenerated;

            // Create Grammar File Object
            StorageFile GrammarContentFile = await Package.Current.InstalledLocation.GetFileAsync(@"Grammar\MyGrammar.xml");

            // Add Grammar Constraint from Grammar File
            SpeechRecognitionGrammarFileConstraint GrammarConstraint = new SpeechRecognitionGrammarFileConstraint(GrammarContentFile);
            MyRecognizer.Constraints.Add(GrammarConstraint);

            // Compile Grammar
            SpeechRecognitionCompilationResult CompilationResult = await MyRecognizer.CompileConstraintsAsync();

            // Write Debug Information
            Debug.WriteLine("Status: " + CompilationResult.Status.ToString());

            // If Compilation Successful, Start Continuous Recognition Session
            if (CompilationResult.Status == SpeechRecognitionResultStatus.Success)
            {
                await MyRecognizer.ContinuousRecognitionSession.StartAsync();
            }
        }

        /// <summary>
        /// Fires when MyRecognizer successfully parses a speech
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MyRecognizer_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            // Write Debug Information
            Debug.WriteLine(args.Result.Text);

            // Drive robot on recognized speech
            switch (args.Result.Text)
            {
                case "move forward":
                    MotorDriver.MoveForward();
                    break;
                case "move reverse":
                    MotorDriver.MoveReverse();
                    break;
                case "turn right":
                    MotorDriver.TurnRight();
                    break;
                case "turn left":
                    MotorDriver.TurnLeft();
                    break;
                case "stop":
                    MotorDriver.Stop();
                    break;
                case "engage obstacle detection":
                    AvoidObstacle = true;
                    break;
                case "disengage obstacle detection":
                    AvoidObstacle = false;
                    break;
                default:
                    break;
            }

            // Turn on/off obstacle detection

        }

        /// <summary>
        /// Fires when MyRecognizer's state changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MyRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            // Write Debug Information
            Debug.WriteLine(args.State);


        }
    }
}
