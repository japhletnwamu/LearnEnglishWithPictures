using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LearnEnglishWithPictures
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Input your Bing Image Search API Subscription Key in the section below
        const string SubscriptionKey = "input your subscription key here";
        const string UriBase = "https://api.cognitive.microsoft.com/bing/v7.0/images/search";

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            // If the Enter key is pressed by the user, the app checks if the content of the textbox is empty, if not empty it searches for the word and finds an image
            if (e.Key == Windows.System.VirtualKey.Enter && inputTermsTextBox.Text.Trim().Length > 0)
            {
                // The app is prompted to search for an image using the Bing Image Search API.
                string imageUrl = FindUrlofImage(inputTermsTextBox.Text);
                // The app then displays the first image it finds.
                foundObjectImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(imageUrl, UriKind.Absolute));
            }
        }

        struct searchResult
        {
            public String jsonResult;
            public Dictionary<string, string> relevantHeaders;
        }

        private string FindUrlofImage(string targetString)
        {
            // Call the method that does the search
            searchResult result = PerformBingImageSearch(targetString);
            // Once the JSON response from the Bing Image Search API has been processed, the URL of the first image is returned
            Windows.Data.Json.JsonObject jsonObject = JsonObject.Parse(result.jsonResult);
            JsonArray results = jsonObject.GetNamedArray("value");
            if (results.Count > 0)
            {
                JsonObject firstResult = results.GetObjectAt(0);
                string imageUrl = firstResult.GetNamedString("contentUrl");
                return imageUrl;
            }
            else
                return "Error 404!";
        }

        static searchResult PerformBingImageSearch(string inputTerms)
        {
            // Create the web-based query that talks to the Bing API
            string uriQuery = UriBase + "?q=" + Uri.EscapeDataString(inputTerms);
            WebRequest request = WebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = SubscriptionKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // Create the result object for the return value.
            var searchResult = new searchResult()
            {
                jsonResult = json,
                relevantHeaders = new Dictionary<String, String>()
            };
            // Extract the Bing HTTP headers
            foreach (string header in response.Headers)
            {
                if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
                {
                    searchResult.relevantHeaders[header] = response.Headers[header];
                }       
            }
            return searchResult;

        }

        private void inputTermsTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
