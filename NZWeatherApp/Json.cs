using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NZWeatherApp
{
    public class Request
    {
        public string type { get; set; }
        public string query { get; set; }
    }

    public class WeatherIconUrl
    {
        public string value { get; set; }
    }

    public class WeatherDesc
    {
        public string value { get; set; }
    }

    public class CurrentCondition
    {
        public string observation_time { get; set; }
        public string temp_C { get; set; }
        public string temp_F { get; set; }
        public string weatherCode { get; set; }
        public List<WeatherIconUrl> weatherIconUrl { get; set; }
        public List<WeatherDesc> weatherDesc { get; set; }
        public string windspeedMiles { get; set; }
        public string windspeedKmph { get; set; }
        public string winddirDegree { get; set; }
        public string winddir16Point { get; set; }
        public string precipMM { get; set; }
        public string humidity { get; set; }
        public string visibility { get; set; }
        public string pressure { get; set; }
        public string cloudcover { get; set; }
    }

    public class WeatherIconUrl2
    {
        public string value { get; set; }
    }

    public class WeatherDesc2
    {
        public string value { get; set; }
    }

    public class Weather
    {
        public string date { get; set; }
        public string tempMaxC { get; set; }
        public string tempMaxF { get; set; }
        public string tempMinC { get; set; }
        public string tempMinF { get; set; }
        public string windspeedMiles { get; set; }
        public string windspeedKmph { get; set; }
        public string winddirection { get; set; }
        public string winddir16Point { get; set; }
        public string winddirDegree { get; set; }
        public string weatherCode { get; set; }
        public List<WeatherIconUrl2> weatherIconUrl { get; set; }
        public List<WeatherDesc2> weatherDesc { get; set; }
        public string precipMM { get; set; }
    }

    public class Data
    {
        public List<Request> request { get; set; }
        public List<CurrentCondition> current_condition { get; set; }
        public List<Weather> weather { get; set; }
    }

    public class RootObject
    {
        public Data data { get; set; }
    }
}

//json2csharp.com