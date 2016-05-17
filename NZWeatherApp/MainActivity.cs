using System;
using System.Net;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;
using System.Threading;
using Android.Graphics.Drawables;

namespace NZWeatherApp
{
    [Activity(Label = "NZWeatherApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ImageView myImageView;
        private string StrMetService;
        public  string URL { get; set; }
        private Button btnGetWeather;
        public string City = "christchurch";
        private string documentsPath;
        private TextView tvTemperature;
        private TextView tvTempChange;
        private Button btnGPS;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //tie in the image view
            myImageView = FindViewById<ImageView>(Resource.Id.Image);
           SpinnerSetup();

            // Get our button from the layout resource,
            // and attach an event to it
            btnGetWeather = FindViewById<Button>(Resource.Id.GetWeatherbtn);
            tvTempChange = FindViewById<TextView>(Resource.Id.tvTempChange);
            tvTemperature = FindViewById<TextView>(Resource.Id.tvTemperature);
            btnGPS = FindViewById<Button>(Resource.Id.btnGPS);
            btnGetWeather.Click += btnGetWeather_Click;
            btnGPS.Click += btnGPS_Click;
        }

        private void btnGPS_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(GPS));
        }

        private void SpinnerSetup()
        { //https://developer.xamarin.com/guides/android/user_interface/spinner/       
        //  //tie in the spinner
        var spinner = FindViewById<Spinner>(Resource.Id.spCity);
        //tie it to the method.
        spinner.ItemSelected += spinner_ItemSelected;
            //The CreateFromResource() method then creates a new ArrayAdapter, which binds each item in the string array to the initial appearance for the Spinner (which is how each item will appear in the spinner when selected).  
            var arrayadapter = ArrayAdapter.CreateFromResource(this, Resource.Array.place_array, Android.Resource.Layout.SimpleSpinnerItem);
        //SetDropDownViewResource is called to define the appearance for each item when the widget is opened (SimpleSpinItem is another standard layout defined by the platform)           
        arrayadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            //Finally, the ArrayAdapter is set to associate all of its items with the Spinner by setting the Adapter property   
            spinner.Adapter = arrayadapter;
        }


    private void btnGetWeather_Click(object sender, EventArgs e)
        {
            //Create the URL - we can change this later for other places 
            // URL = "http://m.metservice.com/towns/christchurch";           
          URL = "http://m.metservice.com/towns/" + City;
            //run the method that dl's the temp            
            ConnectToNetAndDLTemp();
            //change the text on the button, so that you know something has happened            
            btnGetWeather.Text = City.ToUpper(); 
        }

        private void ConnectToNetAndDLTemp()
        {
            //download the website as a string. https://developer.xamarin.com/recipes/ios/network/web_requests/download_a_file/
            try
            {


                //downloads the string and returns it  
                var webaddress = new Uri(URL); //Get the URL change it to a Uniform Resource Identifier 
                var webclient = new WebClient(); //Make a webclient to dl stuff         
                webclient.DownloadStringAsync(webaddress); //dl the website as a string          
                //                                           //Pink color means its an event    
                webclient.DownloadStringCompleted += webclient_DownloadStringCompleted;
                //Connect a method to the run when the DL is finished,   
            }
            catch (Exception e)
            {
                var toast = string.Format("Something went wrong probably no city " + e.Message);
                Toast.MakeText(this, toast, ToastLength.Long).Show();
            }
        }

        //private void webclient_DownloadStringCompleted(Object Sender, DownloadStringCompletedEventArgs e)
        // private void webclient_DownloadStringCompleted(object Sender, DownloadStringCompletedEventArgs e)
        //{ 
        //    //http://stackoverflow.com/questions/30634329/how-to-download-image-from-url-in-xamarin-android               
        //    StrMetService = e.Result; //Result is a property that holds the DL'ed string
        //    StrMetService = e.Result; 


        //    //get rid of single quotes in the string, its a pain otherwise. Always do this first
        //    StrMetService = SearchService.Replace("\"", string.Empty);
        //    StrMetService = StrMetService.Replace("\"", string.Empty);

        //    //get rid of evverything in the header, you don't need it
        //    StrMetService = StrMetService.Remove(0, StrMetService.IndexOf("<body>"));
        //    StrMetService = StrMetService.Remove(0, StrMetService.IndexOf("<body>"));
        //    //get the left hand side of where the temp is, add 30 to get to the end of this string and the beginning of the number
        //    var intTempLeft = StrMetService.IndexOf("summary top><div class=ul><h2>") + 30;
        //    var intTempLeft = StrMetService.IndexOf("summary top><div class=ul><h2>") + 30;
        //    //get the length of the temp string you want.  To do that find the text after the Temp and subtract the length BEFORE the temp from it.
        //    var intTempRight = StrMetService.IndexOf("<span class=temp>") - intTempLeft;
        //    var intTempRight = StrMetService.IndexOf("<span class=temp>") - intTempLeft;

        //    //Pass all the text to the textView in the Scroll bar so you can see the text 
        //    FindViewById<TextView>(Resource.Id.tvAllText).Text = StrMetService;
        //    FindViewById<TextView>(Resource.Id.  AllText).Text = StrMetService;
        //    //Thread.Sleep(500);

        //    ////Pass the Temp to the TempText TextView

        //    var Temp = StrMetService.Substring(intTempLeft, intTempRight);
        //    var Temp = StrMetService.Substring(intTempLeft, intTempRight);

        //    //read in the Old temp if it exists 
        //    string OldTemp = ReadText("Temperature.txt");
        //    string OldTemp = ReadText("Temperature.txt");
        //    //show it on the View
        //    FindViewById<TextView>(Resource.Id.tvTemperature).Text = Temp + " c " + "Old Temp " + OldTemp + " c ";
        //    FindViewById<TextView>(Resource.Id.     TempText).Text = Temp + " c " + "Old Temp " + OldTemp + " c ";
        //    //save the new temp
        //    SaveText("Temperature.txt", Temp);
        //    SaveText("Temperature.txt", Temp);
        //    var imageBitmap = GetImageBitmapFromUrl(ExtractImagePath()); 
        //    var imageBitmap = GetImageBitmapFromUrl(ExtractImagePath());
        //    myImageView.SetImageBitmap(imageBitmap);
        //    myImageView.SetImageBitmap(imageBitmap);
            //}
        private void webclient_DownloadStringCompleted(object Sender, DownloadStringCompletedEventArgs e)
        {
            //http://stackoverflow.com/questions/30634329/how-to-download-image-from-url-in-xamarin-android

            StrMetService = e.Result; //  Result is a property that holds the DL'ed string

            //get rid of single quotes in the string, its a pain otherwise. Always do this first
            StrMetService = StrMetService.Replace("\"", string.Empty);

            //get rid of everything in the header, you don't need it
            StrMetService = StrMetService.Remove(0, StrMetService.IndexOf("<body>"));
            //get the left hand side of where the temp is, add 30 to get to the end of this string and the beginning of the number
            var intTempLeft = StrMetService.IndexOf("summary top><div class=ul><h2>") + 30;
            //get the legth of the temp string you want. To do that find the text after the Temp and subtrack the length BEFORE the temp from it.
            var intTempRight = StrMetService.IndexOf("<span class=temp>") - intTempLeft;

            //Pass all the text to the textView in the Scroll bar so you can see the text
            FindViewById<TextView>(Resource.Id.tvAllText).Text = StrMetService;
            //Pass the Temp to the TempText TextView
            var Temp = StrMetService.Substring(intTempLeft, intTempRight);
            //read in the Old temp if it exists
          string OldTemp = ReadText("Temperature.txt");
            ////show it on the View
            FindViewById<TextView>(Resource.Id.tvTemperature).Text = Temp + " c " + "Old Temp " + OldTemp + " c ";
            ////save the new temp
           SaveText("Temperature.txt", Temp);
            ////convert temperatures from strings to doubles to make comparisons
            double TempNew = Convert.ToDouble(Temp);
            double TempOld = Convert.ToDouble(OldTemp);
            ////if (OldTemp == "")
            ////{
            ////    OldTemp = Temp;
            ////}


            try
            {
                if (TempNew > TempOld)
                {
                    FindViewById<TextView>(Resource.Id.tvTempChange).Text = "It's getting warmer";
                    //make the textview visible (need to have linked the xml to the OnCreate function ie tvTempChange = FindViewById<TextView>(Resource.Id.tvTempChange) for it to use any of the functionality
                    tvTempChange.Visibility = ViewStates.Visible;
                    tvTempChange.Background = new ColorDrawable(Color.Tomato);
                    // tvTemperature.SetBackgroundColor(Color.Red);//SetTextColor(Color.Red); //TextColors.Equals("@android:color/holo_red_light");
                }
                if (TempNew < TempOld)
                {
                    FindViewById<TextView>(Resource.Id.tvTempChange).Text = "It's getting cooler";
                    tvTempChange.Visibility = ViewStates.Visible;
                    tvTempChange.Background = new ColorDrawable(Color.BlueViolet);
                }
                if (TempNew == TempOld)
                {

                    tvTempChange.Visibility = ViewStates.Invisible;

                }
            }
            catch (Exception d)
            {
                var toast = string.Format("It's getting warmer " + d.Message);
                Toast.MakeText(this, toast, ToastLength.Long).Show();
            }
            //Run the Image code, and pass the image to the ImageView
            var imageBitmap = GetImageBitmapFromUrl(ExtractImagePath());
            myImageView.SetImageBitmap(imageBitmap);
             }
            //Welcome to learning from the Internet to DL an image
        private Bitmap GetImageBitmapFromUrl(string url)
        {
            //http://haseeb-ahmed.com/blog/2015/03/image-from-url-in-imageview-xamarin/   
            Bitmap imageBitmap = null;
            using (var webClient = new WebClient())
            { var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            //scale the bitmap to make it bigger
            //https://docs.xamarin.com/api/member/Android.Graphics.Bitmap.CreateScaledBitmap/p/Android.Grap hics.Bitmap/System.Int32/System.Int32/System.Boolean/ and https://forums.xamarin.com/discussion/7153/bitmap-resizing   
            var bitmapScaled = Bitmap.CreateScaledBitmap(imageBitmap, 200, 200, true);
            imageBitmap.Recycle();
            return bitmapScaled;
            }

        public string ExtractImagePath()
        {
            //Return back the path to the image only
            StrMetService = StrMetService.Replace("\"", string.Empty);
            //</div><div class="mob-page" id="forecasts-block"><h2>10 Day Forecast</h2><div class="item"><img src="/sites/all/themes/mobile/images-new/wx-icons/showers_wht.gif" width="32" height="32" title="Showers" alt="Showers" /> 
            var intImageLeft = StrMetService.IndexOf("images-new/wx-icons/") + 20;
            //add 30 to get to the end of this string and the beginning of the number
            var intImageCount = StrMetService.IndexOf("width=32 height=32") - intImageLeft;
            //the text on the right of the number
            var strImage = StrMetService.Substring(intImageLeft, intImageCount);
            return "http://m.metservice.com/sites/all/themes/mobile/images-new/wx-icons/" + strImage;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender; // make a fake spinner and send through the data to it
            City = spinner.GetItemAtPosition(e.Position).ToString();
            City = City.ToLower(); //make it lower case for the URL to work
                                   // make a string to hold the city name so it appears in the toast message
            var toast = string.Format("The city is {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
        }

        public void SaveText(string filename, string text)
        {
            //https://developer.xamarin.com/guides/xamarin-forms/working-with/files/
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, filename);
            System.IO.File.WriteAllText(filePath, text);
        }

        public string ReadText(string filename)
        {
            //if the file exists then read from it
            string text;
            // set up the directory path and combine it with the file name 
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, filename);
            //if the file exists
            if (File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath);

            }
            else
            {
                //otherwise throw a message              
                text = "";
                Toast.MakeText(this, "No File", ToastLength.Short).Show();
            }
            return text;
        }
    }
    }


