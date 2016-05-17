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
using Android.Views;
using Android.Widget;

namespace NZWeatherApp
{
    [Activity(Label = "GPS")]
    public class GPS : Activity, ILocationListener
    {// Set our view from the "main" layout resource 
        private Button btnGetGPSWeather;
        private Button btnLocalWeather;
        private EditText txtLat;
        private EditText txtLong;
        private TextView tvLocationlbl;
        private TextView tvTemplbl;
        private TextView tvHumiditylbl;
        private TextView Conditions;
        private TextView FullText;
        private LocationManager locMgr;
        private string Lat;
        private string Long;
        private string locationProvider;
      //Json new Json
        private Location CurrentLocation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.GPS);
            // Get the latitude/longitude EditBox and button resources:
            //Link in all the different components
            EditText Lat = FindViewById<EditText>(Resource.Id.txtLat);
            EditText Long = FindViewById<EditText>(Resource.Id.txtLong);
            TextView location = FindViewById<TextView>(Resource.Id.tvLocationlbl);
            TextView temperature = FindViewById<TextView>(Resource.Id.tvTemplbl);
            TextView conditions = FindViewById<TextView>(Resource.Id.tvConditionslbl);
            TextView humidity = FindViewById<TextView>(Resource.Id.tvHumiditylbl);
            FullText = FindViewById<TextView>(Resource.Id.FullText);
            // LocationManager locMgr;
            btnGetGPSWeather = FindViewById<Button>(Resource.Id.btnGetGPSWeather);
            btnLocalWeather = FindViewById<Button>(Resource.Id.btnLocalWeather);
            // btnLocalWeather.Click += btnLocalWeather_Click;
            btnGetGPSWeather.Click += btnGetGPSWeather_Click;
            InitializeLocationManager();
        }

        private void btnGetGPSWeather_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void InitializeLocationManager()
        {
            //The LocationManager class will listen for GPS updates from the device and notify the application by way of events. In this example we ask Android for the best location provider that matches a given set of Criteria and provide that provider to LocationManager.
            locMgr = (LocationManager)GetSystemService(LocationService);


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

        }  async private void BtnGPS_Click(object sender, EventArgs e)
        {

            if (CurrentLocation == null)
            {
              FullText.Text = "Can't determine the current address. Try again in a few minutes.";
                return;
            }

            Address address = await ReverseGeocodeCurrentLocation();
            DisplayAddress(address);


        }
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
            Toast.MakeText(this, "Lat " + Lat + "Lon " + Long, ToastLength.Long).Show();
            //json  http://api.worldweatheronline.com/free/v1/weather.ashx?q=-43.526429,172.637637&format=json&num_of_days=1&key=4da7nmph2t6yb76hckfbe4ae
            //xml  http://api.worldweatheronline.com/free/v1/weather.ashx?q=" & myGPS.Lat & "," & myGPS.Lon & "&format=xml&num_of_days=1&key=4da7nmph2t6yb76hckfbe4ae
           //txtLat.Text = "Lat " + Lat;
           // txtLong.Text = "Long " + Long; // just so we know it exists
            FullText.Text = "Lat " + Lat + "Long " + Long;
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

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(CurrentLocation.Latitude, CurrentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                tvLocationlbl.Text = deviceAddress.ToString();
            }
            else
            {
                tvLocationlbl.Text = "Unable to determine the address. Try again in a few minutes.";
            }
        }



        protected override void OnPause()
        {
            base.OnPause();
            locMgr.RemoveUpdates(this);

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

    //} private void btnLocalWeather_Click(object sender, EventArgs e)
    //    {
    //        Finish();
    //    }
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
        public void OnLocationChanged(Location location)
        {
            throw new NotImplementedException();
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            throw new NotImplementedException();
        }
    
        }
}