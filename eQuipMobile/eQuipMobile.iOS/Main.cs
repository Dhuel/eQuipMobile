using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Foundation;
using UIKit;

namespace eQuipMobile.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            try
            {
                Mindscape.Raygun4Net.RaygunClient.Attach("hURdLjAIGA8RoM2KPyBbXw==");
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            
        }
    }
}
