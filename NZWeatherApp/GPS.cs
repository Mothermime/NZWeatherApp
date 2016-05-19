using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Test.Suitebuilder.Annotation;
using Android.Views;
using Android.Widget;
using Java.Net;
using Newtonsoft.Json;
using Android.Text;

namespace NZWeatherApp
{
    [Activity(Label = "GPS")]
    public class GPS : Activity, ILocationListener
    {
// Set our view from the "main" layout resource 
        private Button btnGetGPSWeather;
        private Button btnLocalWeather;
        private EditText txtLat;
        private EditText txtLong;
       
        private TextView tvLocationlbl;
        private TextView tvTemplbl;
        private TextView tvHumiditylbl;
        private TextView tvConditionslbl;
        private TextView FullText;
        private LocationManager locMgr;
        private string Lat;
        private string Long;
        private string locationProvider;
        
        public string URL { get; set; }

        //Json new Json
        private Location CurrentLocation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.GPS);
            // Get the latitude/longitude EditBox and button resources:
            //Link in all the different components
            //var root = JsonConvert.DeserializeObject<RootObject>(TempDataJson);
           txtLat = FindViewById<EditText>(Resource.Id.txtLat);
             txtLong = FindViewById<EditText>(Resource.Id.txtLong);
           
            tvLocationlbl = FindViewById<TextView>(Resource.Id.tvLocationlbl);
            tvTemplbl = FindViewById<TextView>(Resource.Id.tvTemplbl);
            tvConditionslbl = FindViewById<TextView>(Resource.Id.tvConditionslbl);
            tvHumiditylbl = FindViewById<TextView>(Resource.Id.tvHumiditylbl);
            FullText = FindViewById<TextView>(Resource.Id.FullText);

            // LocationManager locMgr;
            btnGetGPSWeather = FindViewById<Button>(Resource.Id.btnGetGPSWeather);
            btnLocalWeather = FindViewById<Button>(Resource.Id.btnLocalWeather);
             btnLocalWeather.Click += btnLocalWeather_Click;
            btnGetGPSWeather.Click += btnGetGPSWeather_Click;
            InitializeLocationManager();
          
    }
        private void btnLocalWeather_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private async void btnGetGPSWeather_Click(object sender, EventArgs e)
        {
            if (CurrentLocation == null)
            {
                FullText.Text = "Can't determine the current address.  Try again in a few minutes.";
                return;
            }
            Address address = await ReverseGeocodeCurrentLocation();
            DisplayAddress(address);
            DownloadGPSTemp();
           
        }
        

        private void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                tvLocationlbl.Text = deviceAddress.ToString();
                FullText.TextSize = 20;
            }
            else
            {
                FullText.Text = "unable to determine the address. Try again in a few minutes.";
            }
        }

        private async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            //get a list of addresses from the current location, get a max of 10 adddresses   
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(CurrentLocation.Latitude, CurrentLocation.Longitude, 10);
            //get the first address, yet its not necessarily the correct one…           
            Address address = addressList.FirstOrDefault();
            return address;
        }

        //display the address that you have got
        private void InitializeLocationManager()
        {
            //The LocationManager class will listen for GPS updates from the device and notify the application by way of events. In this example we ask Android for the best location provider that matches a given set of Criteria and provide that provider to LocationManager.
            locMgr = (LocationManager) GetSystemService(LocationService);


            //Define a Criteria for the best location provider we don’t just use GPS but also WiFi and Cell Towers 
            Criteria criteriaForLocationService = new Criteria
            {
                //A constant indicating an approximate accuracy
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Medium
            };
            //gets the best providor
            locationProvider = locMgr.GetBestProvider(criteriaForLocationService, true);
            Toast.MakeText(this, "Using " + locationProvider, ToastLength.Short).Show();

        }

        //private async void BtnGPS_Click(object sender, EventArgs e)
        //{
        //    if (CurrentLocation == null)
        //    {
        //        FullText.Text = "Can't determine the current address. Try again in a few minutes.";
        //        return;
        //    }

        //    Address address = await ReverseGeocodeCurrentLocation();
        //    DisplayAddress(address);
        //}

        protected override void OnResume()
        {
            base.OnResume();

            // initialize location manager again 
            locMgr.RequestLocationUpdates(locationProvider, 0, 0, this);
            locMgr = GetSystemService(Context.LocationService) as LocationManager;
        }

        //https://developer.xamarin.com/recipes/android/os_device_resources/gps/get_current_device_location/
        //http://developer.android.com/guide/topics/location/strategies.html


        //ILocationListener methods
        void ILocationListener.OnLocationChanged(Location location)
        {
            CurrentLocation = location;
            UpdateGPSLocation();
        }

        private void UpdateGPSLocation()
        {
            Lat = CurrentLocation.Latitude.ToString();
            Long = CurrentLocation.Longitude.ToString();
           // Toast.MakeText(this, "Lat " + Lat + "Lon " + Long, ToastLength.Long).Show();
            //json  http://api.worldweatheronline.com/free/v1/weather.ashx?q=-43.526429,172.637637&format=json&num_of_days=1&key=4da7nmph2t6yb76hckfbe4ae
            //xml  http://api.worldweatheronline.com/free/v1/weather.ashx?q=" & myGPS.Lat & "," & myGPS.Lon & "&format=xml&num_of_days=1&key=4da7nmph2t6yb76hckfbe4ae
            txtLat.Text =  Lat;
            txtLong.Text =  Long; // just so we know it exists
            //txtLocation.Text = "Lat " + Lat + "Long " + Long;
            //FullText.TextSize = 30;

        }

        //Turn off GPS?
        void ILocationListener.OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        void ILocationListener.OnProviderEnabled(string provider)
        {
            Toast.MakeText(this, "Provider Enabled", ToastLength.Short).Show();
        }

        void ILocationListener.OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }

        //async Task<Address> ReverseGeocodeCurrentLocation()
        //{
        //    Geocoder geocoder = new Geocoder(this);
        //    IList<Address> addressList =
        //        await geocoder.GetFromLocationAsync(CurrentLocation.Latitude, CurrentLocation.Longitude, 10);

        //    Address address = addressList.FirstOrDefault();
        //    return address;
        //}





        protected override void OnPause()
        {
            base.OnPause();
            locMgr.RemoveUpdates(this);

        }

        public void DownloadGPSTemp()
        {
            try
            {
           //here is out URL with our Lat and Long stuck into it
             URL =  "http://api.worldweatheronline.com/free/v1/weather.ashx?q=" + CurrentLocation.Latitude + "," + CurrentLocation.Longitude + "&format=json&num_of_days=1&key=4da7nmph2t6yb76hckfbe4ae";
                    //downloads the string and returns it 
                    var webaddress = new Uri(URL);
                    //Get the URL change it to a Uniform Resource Identifier
                    var webclient = new WebClient(); //Make a webclient to dl stuff  
                    webclient.DownloadStringAsync(webaddress); //dl the website 
                   webclient.DownloadStringCompleted += webclient_DownloadJSONCompleted; //Connect a method to the run when the DL is finished,              
                } catch (Exception e) {  //Run an error message if it doesn’t work  
                var toast = string.Format("DL not working? " + e.Message); Toast.MakeText(this, toast, ToastLength.Long).Show();
            }          }
        private void webclient_DownloadJSONCompleted(object Sender, DownloadStringCompletedEventArgs e)
        {
            string TempDataJSON = e.Result;
            //We need to parse the class that has Data in it public Data data { get; set; } in this case its called Root, but it might not be. 
            var root = JsonConvert.DeserializeObject<RootObject>(TempDataJSON);
            //then we pass out the data into the classes we want to use. Its coming out as a list, so we get the first entry [0]
            CurrentCondition currentCondition = root.data.current_condition[0];
            Weather weather = root.data.weather[0];
           
            //then we can do whatever we like with it. 
            FullText.Text =  " Wind = " + currentCondition.windspeedKmph + "kph" + "  Cloud cover = " + currentCondition.cloudcover + "   Wind direction = " + currentCondition.winddir16Point + "  Rainfall = " + currentCondition.precipMM;
            tvHumiditylbl.Text = "Humidity = " + currentCondition.humidity;
            tvTemplbl.Text = "Current temperature = " + currentCondition.temp_C  + "     Min " + weather.tempMinC + "  Max " + weather.tempMaxC;
            tvConditionslbl.Text = "Visibility = " + currentCondition.visibility;
            //Can we put HTML in a label?  https://forums.xamarin.com/discussion/56484/need-to-put-htmlinto-a-label           
            //  AllText.TextFormatted = Html.FromHtml("<ul>Current Temp = " + currentCondition.temp_C + "</ul>");        

            string text = "<bold>Current Temp = " + currentCondition.temp_C + "</bold>";
             
            //FullText.SetText(Html.FromHtml(text), TextView.BufferType.Normal);
            FullText.TextSize = 20;
        }  

        }
}

    //// When the user clicks the button ... 
        //btnGetWeather.Click += async (sender, e) =>
        //    {
        //      //  Get the latitude and longitude entered by the user and create a query.
        //            string url = "http://api.geonames.org/findNearByWeatherJSON?lat=" + latitude.Text + "&lng=" + longitude.Text + "&username=demo";
        //     //   Fetch the weather information asynchronously,  
        //         //    parse the results, then update the screen:  
        //            JsonValue json = await FetchWeatherAsync(url);
        //        ParseAndDisplay(json);
        //    };
        // //   Gets weather data from the passed URL.
        //        private async Task<JsonValue> FetchWeatherAsync(string url)
        //{     // Create an HTTP web request using the URL:    
        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url)); request.ContentType = "application/json"; request.Method = "GET";
        //  //  Send the request to the server and wait for the response: 
        //    using (WebResponse response = await request.GetResponseAsync())
        //        {         // Get a stream representation of the HTTP web response:  
        //            using (Stream stream = response.GetResponseStream())
        //            {             // Use this stream to build a JSON document object: 
        //                JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream)); Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
        //                Return the JSON document:            
        //            return jsonDoc;
        //            }
        //        }
        //}

    
 
//potentially useful code
//[Activity(Label = "GPSApp", MainLauncher = true, Icon = "@drawable/icon")]
//public class MainActivity : Activity, ILocationListener
//{
//    Location _currentLocation;
//    LocationManager _locationManager;

//    TextView _locationText;
//    TextView _addressText;
//    TextView _remarksText;
//    string _locationProvider;
//    const string _sourceAddress = "The Link, Cebu IT Park, Jose Maria del Mar St,Lahug, Cebu City, 6000 Cebu";

//    GPSServiceBinder binder;
//    GPSServiceConnection gpsServiceConnection;
//    Intent gpsServiceIntent;
//    protected override void OnCreate(Bundle bundle)
//    {
//        base.OnCreate(bundle);
//        SetContentView(Resource.Layout.Main);

//        _addressText = FindViewById<TextView>(Resource.Id.txtAddress);
//        _locationText = FindViewById<TextView>(Resource.Id.txtLocation);
//        _remarksText = FindViewById<TextView>(Resource.Id.txtRemarks);

//        //Initialising the LocationManager to provide access to the system location services.
//        //The LocationManager class will listen for GPS updates from the device and notify the application by way of events. 
//        _locationManager = (LocationManager)GetSystemService(LocationService);

//        //Define a Criteria for the best location provider
//        Criteria criteriaForLocationService = new Criteria
//        {
//            //A constant indicating an approximate accuracy
//            Accuracy = Accuracy.Coarse,
//            PowerRequirement = Power.Medium



