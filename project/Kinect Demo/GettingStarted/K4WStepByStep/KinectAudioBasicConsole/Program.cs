using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Speech;
using Microsoft.Speech.Recognition;
using System.IO;
using Microsoft.Speech.AudioFormat;
using System.Threading;

namespace KinectAudioBasicConsole
{
    class Program
    {
      static  KinectSensor sensor;

      const string RecognizerId = "SR_MS_en-US_Kinect_11.0";

        static void Main(string[] args)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                sensor = KinectSensor.KinectSensors[0];
                sensor.Start();
                startAudio();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No Device Connected !");
            }
        }

        /// <summary>
        /// Starts the audio.
        /// </summary>
        private static void startAudio()
        {
            if (sensor == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No Kinect sensors are attached to this computer");
                return;
            }

            // Get the Kinect Audio Source
            KinectAudioSource audioSource = sensor.AudioSource;

            audioSource.AutomaticGainControlEnabled = false;
            audioSource.NoiseSuppression = true;

            RecognizerInfo ri = GetKinectRecognizer();

            if (ri == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not find Kinect speech recognizer. Please refer to the sample requirements.");
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Using: {0}", ri.Name);

            // NOTE: Need to wait 4 seconds for device to be ready right after initialization
            int wait = 4;
            while (wait > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Device will be ready for speech recognition in {0} second(s).\r", wait--);
                Thread.Sleep(1000);
            }

            using (var sre = new SpeechRecognitionEngine(ri.Id))
            {
                var options = new Choices();
                options.Add("Red");
                options.Add("Green");
                options.Add("Blue");
                options.Add("Yellow");


                var gb = new GrammarBuilder { Culture = ri.Culture };

                // Specify the culture to match the recognizer in case we are running in a different culture.                                 
                gb.Append(options);

                // Create the actual Grammar instance, and then load it into the speech recognizer.
                var g = new Grammar(gb);

                sre.LoadGrammar(g);
                sre.SpeechRecognized += SreSpeechRecognized;
                sre.SpeechHypothesized += SreSpeechHypothesized;
                sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;


                using (Stream s = audioSource.Start())
                {
                    sre.SetInputToAudioStream(
                        s, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Recognizing speech. Read: 'Red', 'Green', 'Blue', 'Yellow'");
                    sre.RecognizeAsync(RecognizeMode.Multiple);
                    Console.ReadLine();
                    Console.WriteLine("Stopping recognizer ...");
                    sre.RecognizeAsyncStop();
                }

            }
           
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            RecognizerInfo ri = SpeechRecognitionEngine.InstalledRecognizers().Where(r => r.Id == RecognizerId).FirstOrDefault();

            return ri;
        }

        private static void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speech Rejected");
            if (e.Result != null)
            {
                // do operation
            }
        }

        private static void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            //  MessageBox.Show(string.Format("\rSpeech Hypothesized: \t{0}", e.Result.Text));
        }

        private static void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence >= 0.5)
            {
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), e.Result.Text);
                Console.WriteLine(e.Result.Text + "  Confidence : " + e.Result.Confidence);


            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\nSpeech Recognized but confidence was too low: \t{0}  - {1}", e.Result.Text, e.Result.Confidence);
            }
        }
    }
}
