using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace PruebaTextToSpeech
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        //SpeechSynthesizer Class Provides access to the functionality of an installed a speech synthesis engine.   

        //TTS 
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MediaElement mediaplayer = new MediaElement();
            using (var speech = new SpeechSynthesizer())
            {
                speech.Voice = SpeechSynthesizer.AllVoices.First(gender => gender.Gender == VoiceGender.Female);
                string ssml = @"<speak version='1.0' " + "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='es-ES'>" + txtBox.Text + "</speak>";
                SpeechSynthesisStream stream = await speech.SynthesizeSsmlToStreamAsync(ssml);
                mediaplayer.SetSource(stream, stream.ContentType);

            }
        }

        //MICROFONO
        private async void Button_Click2(object sender, RoutedEventArgs e)
        {            
            // Create an instance of SpeechRecognizer.
            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();
            // Compile the dictation grammar by default.
            await speechRecognizer.CompileConstraintsAsync();
            // Start recognition.
            try
            {
                SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeWithUIAsync();
                // Do something with the recognition result.
                var messageDialog = new Windows.UI.Popups.MessageDialog(speechRecognitionResult.Text, "Text spoken");
                await messageDialog.ShowAsync();
            }
            catch (Exception){
                RequestMicrophonePermission();
            }
        }
        private static int NoCaptureDevicesHResult = -1072845856;

        public async void RequestMicrophonePermission()
        {
                ContentDialog noWifiDialog = new ContentDialog()
                {
                    Title = "No tienes activados los permisos del micrófono",
                    Content ="¿Deseas activarlos?",
                    PrimaryButtonText= "Sí",
                    CloseButtonText = "No"
                };

            ContentDialogResult result=await noWifiDialog.ShowAsync();

            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-speech"));
                //await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-microphone")); quizás pueda servir
                // Request access to the audio capture device.
                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
                settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                settings.MediaCategory = MediaCategory.Speech;
                MediaCapture capture = new MediaCapture();

                await capture.InitializeAsync(settings);
            }
            catch (TypeLoadException)
            {
                // Thrown when a media player is not available.
                var messageDialog = new Windows.UI.Popups.MessageDialog("Media player components are unavailable.");
                await messageDialog.ShowAsync();
            }
            catch (UnauthorizedAccessException)
            {
                // Thrown when permission to use the audio capture device is denied.
                // If this occurs, show an error or disable recognition functionality.

            }
            catch (Exception exception)
            {
                // Thrown when an audio capture device is not present.
                if (exception.HResult == NoCaptureDevicesHResult)
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog("No Audio Capture devices are present on this system.");
                    await messageDialog.ShowAsync();
                }
                else
                {
                    throw;
                }
            }
        }
    }
}


