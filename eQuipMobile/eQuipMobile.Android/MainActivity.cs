using System;
using Android.App;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Plugin.Permissions;
using Xamarin.Forms;
using Android.Views;
using Com.Honeywell.Aidc;
using System.Collections.Generic;

namespace eQuipMobile.Droid
{
    [Activity(Name = "eisg.eQuipMobileApp.MainActivity",  Label = "eQuipMobile", Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleTask, Theme = "@style/splashscreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new string[] { "eQuipMobileApp.RECVR" }, Categories = new[] { Intent.CategoryDefault })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, BarcodeReader.IBarcodeListener, BarcodeReader.ITriggerListener, AidcManager.ICreatedCallback
    {
        // Field, property, and method for Picture Picker
        public static readonly int PickImageId = 1000;

        // This intent string contains the source of the data as a string  
        private static string SOURCE_TAG = "com.motorolasolutions.emdk.datawedge.source";
        // This intent string contains the captured data as a string  
        // (in the case of MSR this data string contains a concatenation of the track data)  
        private static string DATA_STRING_TAG = "com.motorolasolutions.emdk.datawedge.data_string";
        // Intent Action for our operation
        private static string ourIntentAction = "eQuipMobileApp.RECVR";

        AidcManager manager;
        static BarcodeReader reader;
        bool triggerState = false;

        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);

            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (intent != null))
                {
                    Android.Net.Uri uri = intent.Data;
                    Stream stream = ContentResolver.OpenInputStream(uri);

                    // Set the Stream as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(stream);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
        }

        protected override void OnCreate(Bundle bundle)
        {


            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.Window.RequestFeature(WindowFeatures.ActionBar);
            // Name of the MainActivity theme you had there before.
            // Or you can use global::Android.Resource.Style.ThemeHoloLight
            base.SetTheme(Resource.Style.MainTheme);

            base.OnCreate(bundle);

            //Set up barcode scanner
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            // Set up Raygun:
            Mindscape.Raygun4Net.RaygunClient.Attach("hURdLjAIGA8RoM2KPyBbXw==");

            global::Xamarin.Forms.Forms.Init(this, bundle);

            //test 
            AidcManager.Create(this, this);

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        // Receives the Intent that has barcode info and displays to the user
        private string handleDecodeData(Intent i)
        {
            string result = "";
            // check the intent action is for us  
            if (i.Action.Equals(ourIntentAction))
            {
                // get the source of the data  
                String source = i.GetStringExtra(SOURCE_TAG);
                // save it to use later  
                if (source == null) source = "scanner";
                // get the data from the intent  
                String data = i.GetStringExtra(DATA_STRING_TAG);
                result = data;
            }
            return result;
        }
        // Receives Intent from DataWedge application that has barcode data 
        protected override void OnNewIntent(Intent i)
        {
            var result = handleDecodeData(i);
            MessagingCenter.Send(result, "Datawedge");
        }



        //Ct50 code
        public void OnCreated(AidcManager aidcManager)
        {
            bool issuccessful = false;
            manager = aidcManager;
            // use the manager to create a BarcodeReader with a session
            // associated with the internal imager.
            try
            {
                reader = manager.CreateBarcodeReader();
                issuccessful = true;
            }
            catch (Exception exc)
            {
                issuccessful = false;
            }
            
            if (issuccessful)
            {
                try
                {

                    // apply settings
                    reader.SetProperty(BarcodeReader.PropertyCode39Enabled, true);
                    reader.SetProperty(BarcodeReader.PropertyDatamatrixEnabled, true);

                    reader.SetProperty(BarcodeReader.PropertyCode128Enabled, true);
                    reader.SetProperty(BarcodeReader.PropertyGs1128Enabled, true);
                    reader.SetProperty(BarcodeReader.PropertyQrCodeEnabled, true);
                    reader.SetProperty(BarcodeReader.PropertyUpcAEnable, true);
                    reader.SetProperty(BarcodeReader.PropertyEan13Enabled, true);
                    reader.SetProperty(BarcodeReader.PropertyAztecEnabled, true);
                    reader.SetProperty(BarcodeReader.PropertyCodabarEnabled, true);
                    reader.SetProperty(BarcodeReader.PropertyInterleaved25Enabled, true);
                    reader.SetProperty(BarcodeReader.PropertyPdf417Enabled, true);
                    // Set Max Code 39 barcode length
                    reader.SetProperty(BarcodeReader.PropertyCode39MaximumLength, 10);

                    // Disable bad read response, handle in onFailureEvent
                    reader.SetProperty(BarcodeReader.PropertyNotificationBadReadEnabled, true);

                    // set the trigger mode to client control
                    reader.SetProperty(BarcodeReader.PropertyTriggerControlMode, BarcodeReader.TriggerControlModeClientControl);
                }
                catch (UnsupportedPropertyException)
                {
                    Toast.MakeText(this, "Failed to apply properties", ToastLength.Short).Show();
                }

                // register bar code event listener
                reader.AddBarcodeListener(this as BarcodeReader.IBarcodeListener);

                // register trigger state change listener
                reader.AddTriggerListener(this as BarcodeReader.ITriggerListener);

                reader.Claim();
            }
            
        }
        protected override void OnResume()
        {
            base.OnResume();
            if (reader != null)
            {
                try
                {
                    reader.Claim();
                }
                catch (ScannerUnavailableException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);

                    Toast.MakeText(this, "Scanner unavailable", ToastLength.Short).Show();
                }
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (reader != null)
            {
                // release the scanner claim so we don't get any scanner
                // notifications while paused.
                reader.Release();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (reader != null)
            {
                // unregister barcode event listener
                reader.RemoveBarcodeListener(this);

                // unregister trigger state change listener
                reader.RemoveTriggerListener(this);

                // close BarcodeReader to clean up resources.
                // once closed, the object can no longer be used.
                reader.Close();
            }
            if (manager != null)
            {
                // close AidcManager to disconnect from the scanner service.
                // once closed, the object can no longer be used.
                manager.Close();
            }
        }

        public void OnBarcodeEvent(BarcodeReadEvent @event)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    MessagingCenter.Send(@event.BarcodeData, "Datawedge");
                }
                catch(Exception exc)
                {
                    DependencyService.Get<IError>().SendRaygunError(exc, "Datawedge", "Datawedge", exc.Message);
                }
                
            });

            // reset the trigger state
            triggerState = false;

            
        }

        public void OnFailureEvent(BarcodeFailureEvent @event)
        {
            RunOnUiThread(() =>
                Toast.MakeText(this, "Barcode read failed", ToastLength.Short).Show());
        }

        public void OnTriggerEvent(TriggerStateChangeEvent @event)
        {
            try
            {
                // only handle trigger presses
                if (@event.State)
                {
                    // turn on/off aimer, illumination and decoding
                    reader.Aim(!triggerState);
                    reader.Light(!triggerState);
                    reader.Decode(!triggerState);

                    triggerState = !triggerState;
                }
            }
            catch (ScannerNotClaimedException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);

                Toast.MakeText(this, "Scanner is not claimed", ToastLength.Short).Show();
            }
            catch (ScannerUnavailableException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);

                Toast.MakeText(this, "Scanner unavailable", ToastLength.Short).Show();
            }
        }

    }
}

