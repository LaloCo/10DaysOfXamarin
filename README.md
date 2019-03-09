We made significant progress yesterday, getting the current location and responding to updates so that every time the location changes by a certain margin, we get the new coordinates. This means that we are ready to ask Foursquare for venues that are nearby.

# Moving on

Foursquare is a company that started up as a social network oriented to visited places: people would "check in" and become majors of places, it is fantastic. A few years back it split into Swarm -the social network part- and Foursquare -which now exists as a place recommendations platform.

[This branch](https://github.com/LaloCo/10DaysOfXamarin/tree/Day7/initial)

# Challenge

Our goal today is to use the public Foursquare API to find venues that exist close to the coordinates that we have retrieved. You need to familiarize yourself with the **Foursquare search API**, create an account so you can create an app that has access to that API, and use the API to search for nearby venues.

Users will be able to select the venue from a list, which has to be assigned to the Experience and saved along with the title and experience into the SQLite table.

## Problem

You will need to familiarize yourself with the format of the requests to the Foursquare search API, and the format of the responses that it returns. Once you have a clear idea of what the requests and responses look like, you should use HttpClient to send requests to the API and catch the response from the service.

Making requests to the internet is easy, but it comes with its challenges. For instance, you won't be receiving C# objects from the Foursquare API. Instead, you receive a JSON string. You need the Newtonsoft.Json NuGet package to deserialize that JSON string into C# objects that you can then list into your UI.

The model for our Experience table has to change. The table has to contain some more columns, with information about the selected venue. You don't have to save all of the information that Foursquare returns, but name, category, and coordinates should be in the new entries.

## App description

The app’s interface changes a bit. In the MainPage, you will have

- An entry for the user to search for nearby venues
- A ListView to show potential matches to the search the user is performing
- A label to display the name of the selected venue
- A label to display the category of the selected venue
- A label to display the coordinates of the selected venue

With these five new elements, users can select an item from the list view, at which point, the labels are updated with the information from that selected venue. When saving, the Experience object should now contain the name, category, and coordinates of that same venue.

## Constraints

Once a venue is selected from the list, the "search entry" should be cleaned and the list hidden.

While no venue is selected, you should hide the labels.

When the users press the save button, the labels, the new entry, and the list view should also be cleared, just like the title entry and the content editor already do.

# Solution

So there are a few steps that we need to perform today. Let's start with what we already know by preparing the UI.

## Defining the UI

We need to add a few items to the MainPage. Notice that I have added all the labels inside of a second StackLayout. This doesn't affect the way the UI looks but makes it easier for me to hide them all since I will only need to hide one element instead of three.

``` xml
<Entry x:Name="searchEntry"
       Style="{StaticResource mainEntryStyle}"
       Placeholder="Search a nearby venue..."
       TextChanged="SearchEntry_TextChanged"/>
<ListView x:Name="venuesListView"
          HeightRequest="20"
          Margin="-16,0">
    <ListView.ItemTemplate>
        <DataTemplate>
            <TextCell/>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
<StackLayout x:Name="selectedVenueStackLayout"
             IsVisible="false">
    <Label x:Name="venueNameLabel"
           Style="{StaticResource titleLabelStyle}"/>
    <Label x:Name="venueCategoryLabel"
           Style="{StaticResource subtitleLabelStyle}"/>
    <Label x:Name="venueCoordinatesLabel"
           Style="{StaticResource detailLabelStyle}"/>
</StackLayout>
```

I've also already set the item template for the ListView to a TextCell, just like we did with the experiences list. This time, however, I haven't set the bindings for the Text or Detail, since I don't yet know the type of element that will be bound to these cells. I know that it will be a venue and that I will probably want to bind a name and a category, or the coordinates, but I don't yet have this class.

Also, very important, the new entry has an event handler. From this event handler I perform the requests to the Foursquare API: every time the text in this search entry changes, I want to perform a request with the updated query.

Finally, notice that the StackLayout both has a name, and is hidden by default. Once the user selects a venue, we shall set some text values to all those labels and change the IsVisible property back to true.

## Exploring the Foursquare API

Now that we have the UI quite ready, time to explore the API. How are requests supposed to be made? What will the API return? Additionally, this JSON response, how do I model it into a C# class, similar to the way we modeled the database table?

### Creating an app

So, of course, the first step is to create a Foursquare developer account. Head over to

[Foursquare Developer](https://developer.foursquare.com/)

and click on the top-right button to create an account. Fill in the required information, and click on create.

Now, you are navigated to your apps. Of course, there are no apps listed yet, but here you can create an app for free, which is enough for us to implement search into our apps.

Go ahead and create a new application. You need to provide a name, a URL, and information about whom you are building the app for. In the features section, make sure you select the "search for points of interest" option. Agree to the license and policy and click on continue -the free version of the API is more than enough to get started. Optionally, you can verify your account to access 100x more API calls, although this requires you to add some credit card information.

Once you have created the app though, you see a client id and a client secret, both of which are necessary when making the requests to the search API.

### The Search API

Navigate over to the "[Search for Venues](https://developer.foursquare.com/docs/api/venues/search)" documentation to check the format of the request, the parameters that you can pass, and a sample response.

> Authentication: Note that the authentication for this endpoint can be "user-less," which means that users don't need to login to their Foursquare account. As developers, however, we need to authenticate ourselves in some way. Fortunately, we can do so with the client id and client secret that we got when creating the app, remember? This is all in the authentication documentation under the "user-less auth" section.

So back to the search for venues documentation. You find in that page that the request must be a GET request. This is important once we start making the requests from our Xamarin app. You also find the endpoint, which is the URL to which you must make the requests:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day7-001.png)

Then, you find a long list of parameters. Some of these parameters are required and must be added to the request URL; most are optional. You have to select the parameters that best match your application. For our example, these are the parameters that we need:

- ll. This way we pass the coordinates that we have already retrieved back in our application.
- radius. We limit the search to 500 meters (about 1500 feet).
- query. The search query that the users type in the search entry is passed to this parameter.
- limit. We want only the top three results, so we can limit what the endpoint returns to three right away by setting a value to this parameter.
- client_id. This is not listed, but if you check the authentication documentation, you will notice it is required. Here we pass the client id that we created when we created our Foursquare app.
- client_secret. Also in the authentication documentation, not this parameters list. This value was also created along with our app.

Finally, at the bottom of the page, you see a sample response. While we can use this sample response to create the C# classes (more on that in a minute), this is just a sample. Let's make a real request and get an actual response!

## Making a request

We already know the endpoint that we have to use, and the parameters that we have to pass. This means that we are ready to make requests! Before making the requests from our code though, let's make a request directly from Chrome (or whatever browser you are using).

Following what we know about the endpoint and the parameters, let's type the following in the address bar:

    https://api.foursquare.com/v2/venues/search?ll=40.7,-74&radius=500&query=sushi&limit=3&client_id=YOUR_CLIENT_ID&client_secret=YOUR_CLIENT_SECRET&v=20190226

Make sure you substitute the date and add your client id and client secret. You should see a response to your first request! This JSON now contains the three venues that are close to the coordinates you passed:

``` javascript
{
   "meta":{
      "code":200,
      "requestId":"5c75e85b4c1f67634b1dc168"
   },
   "response":{
      "venues":[
         {
            "id":"55a1985c498e60f3d254cbbf",
            "name":"Gasolinera \"Las Palomas\"",
            "location":{
               "lat":20.085283530439852,
               "lng":-98.71836225350707,
               "labeledLatLngs":[
                  {
                     "label":"display",
                     "lat":20.085283530439852,
                     "lng":-98.71836225350707
                  }
               ],
               "distance":57,
               "cc":"MX",
               "country":"Mexico",
               "formattedAddress":[
                  "Mexico"
               ]
            },
            "categories":[
               {
                  "id":"4bf58dd8d48988d113951735",
                  "name":"Gas Station",
                  "pluralName":"Gas Stations",
                  "shortName":"Gas Station",
                  "icon":{
                     "prefix":"https:\/\/ss3.4sqi.net\/img\/categories_v2\/shops\/gas_",
                     "suffix":".png"
                  },
                  "primary":true
               }
            ],
            "referralId":"v-1551231067",
            "hasPerk":false
         },
         {
            "id":"551d60f4498e3ebc8f30216e",
            "name":"Pachuca",
            "location":{
               "lat":20.08608537427932,
               "lng":-98.71822507515328,
               "labeledLatLngs":[
                  {
                     "label":"display",
                     "lat":20.08608537427932,
                     "lng":-98.71822507515328
                  }
               ],
               "distance":33,
               "cc":"MX",
               "country":"Mexico",
               "formattedAddress":[
                  "Mexico"
               ]
            },
            "categories":[
               {
                  "id":"52e81612bcbc57f1066b7a13",
                  "name":"Nature Preserve",
                  "pluralName":"Nature Preserves",
                  "shortName":"Preserve",
                  "icon":{
                     "prefix":"https:\/\/ss3.4sqi.net\/img\/categories_v2\/parks_outdoors\/naturepreserve_",
                     "suffix":".png"
                  },
                  "primary":true
               }
            ],
            "referralId":"v-1551231067",
            "hasPerk":false
         },
         {
            "id":"4f777102e4b087957aaddc63",
            "name":"Oxxo Santa Ana",
            "location":{
               "lat":20.085300571122577,
               "lng":-98.71837559304696,
               "labeledLatLngs":[
                  {
                     "label":"display",
                     "lat":20.085300571122577,
                     "lng":-98.71837559304696
                  }
               ],
               "distance":56,
               "cc":"MX",
               "country":"Mexico",
               "formattedAddress":[
                  "Mexico"
               ]
            },
            "categories":[
               {
                  "id":"4d954b0ea243a5684a65b473",
                  "name":"Convenience Store",
                  "pluralName":"Convenience Stores",
                  "shortName":"Convenience Store",
                  "icon":{
                     "prefix":"https:\/\/ss3.4sqi.net\/img\/categories_v2\/shops\/conveniencestore_",
                     "suffix":".png"
                  },
                  "primary":true
               }
            ],
            "referralId":"v-1551231067",
            "hasPerk":false
         }
      ],
      "confident":false
   }
}
```

### The JSON structure

If you evaluate this JSON string, you can notice many different blocks of code. You can identify that the whole string exists within a blog of curly brackets. This means that the entire string represents one single principal object. The principal object itself contains more objects: the meta and the response objects. And so on. The meta object doesn't have anything that interests us, but the response does: it contains a venues object! Also, that venues object is a list (you can identify a list because it starts and ends with square brackets). The list itself contains three objects (in this case), all of which contain the same properties, which means that they all represent a single type (for example a venue) but have different values for each property.

Don't worry if you don't fully understand this JSON, but I hope you get a sense of how it is structured.

### Generating C# classes

Understanding how the JSON is structured is crucial to generating the C# classes that represent each object in this JSON. For example, take a look at just the first part of this JSON:

``` javascript
{
   "meta":{
      "code":200,
      "requestId":"5c75e85b4c1f67634b1dc168"
   },
	 ...
}
```

For the principal object that I previously mentioned, we could create a Search class that contains a meta property (ignoring for a second the response object). This meta property will itself be a complex type so that we can create a Meta class. Finally, this Meta class will have an integer code property, and a string requestId property. Considering only this small part of our JSON, we would end up with this C# classes:

``` csharp
class Search
{
    public Meta Meta { get; set; }
}

class Meta
{
    public int Code {get; set; }
    public string RequestId { get; set; }
}
```

Continuing with the response object, we would need a Response class, and so on. However, this JSON is a bit long; it may take a few minutes to come up with all the necessary classes. Enter [jsonutils.com](http://jsonutils.com/)

[Jsonutils.com](http://jsonutils.com/) is a great website that automates the process of generating C# (or other languages) classes based on a JSON string. So navigate over to that website, paste your JSON, rename the class to be Search, and it creates all the classes for you:

``` csharp
public class Meta
    {
        public int code { get; set; }
        public string requestId { get; set; }
    }

    public class LabeledLatLng
    {
        public string label { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public IList<LabeledLatLng> labeledLatLngs { get; set; }
        public int distance { get; set; }
        public string cc { get; set; }
        public string country { get; set; }
        public IList<string> formattedAddress { get; set; }
    }

    public class Icon
    {
        public string prefix { get; set; }
        public string suffix { get; set; }
    }

    public class Category
    {
        public string id { get; set; }
        public string name { get; set; }
        public string pluralName { get; set; }
        public string shortName { get; set; }
        public Icon icon { get; set; }
        public bool primary { get; set; }
    }

    public class Venue
    {
        public string id { get; set; }
        public string name { get; set; }
        public Location location { get; set; }
        public IList<Category> categories { get; set; }
        public string referralId { get; set; }
        public bool hasPerk { get; set; }
    }

    public class Response
    {
        public IList<Venue> venues { get; set; }
        public bool confident { get; set; }
    }

    public class Search
    {
        public Meta meta { get; set; }
        public Response response { get; set; }
    }
```

Notice the Search and Meta classes; we were on the right track! Now, let's move on and paste these classes in a new Search.cs file that we need to create inside the Model folder.

By the way, to be able to use the IList interface which is used in those properties that are arrays in the JSON string, you need to add a using directive to the System.Collections.Generic namespace

Having this classes model the JSON objects is crucial since they are used to deserialize the JSON string into C# objects. In this example, when we deserialize all the JSON as a Search object, all the properties are assigned to the corresponding type, and then we can use those values.

## Making the request from the app

We have made an example request directly from our browser, received a JSON and created the classes that model that string.

Finally, it is time to make the request from our code, get the JSON and deserialize it into C# objects:

### Making the request

We need to make the request -with the same format as we did from Chrome- from the event handler for when the text in the search entry changes. First, of course, we need the URL. We can create the URL like this:

``` csharp
string url = $"https://api.foursquare.com/v2/venues/search?ll={position.Latitude},{position.Longitude}&radius=500&query={searchEntry.Text}&limit=3&client_id={YOUR_CLIENT_ID}&client_secret={YOUR_CLIENT_SECRET}&v={DateTime.Now.ToString("yyyyMMdd")}";
```

A lot is going on in that string, but notice that it is the same string that we created before, except now the values that we assign to the parameters come from local variables:

- The ll come from both the latitude and longitude that exist in the position variable
- The radius is a static 500
- The query is whatever the user wrote in the search entry
- The limit is a static 3
- The client_id and client_secret are, well, your client id and client secret
- The v is the current date formatted as yyyyMMdd, so, for example, April 26th 2019 is converted to 20190426.

Now that the URL is ready, it is time to make the request. For this, we use the HttpClient class. Notice that I create an instance of the HttpClient class inside of a using statement, for reasons similar to why we created the SQLiteConnection inside a using statement too.

``` csharp
// added using System.Net.Http;
using (HttpClient client = new HttpClient())
{
    // made the method async
    string json = await client.GetStringAsync(url);
}
```

Just like that we already have the JSON in a C# variable.

### Deserializing the JSON

With the JSON in a string variable, the next step is to deserialize that into C# objects. For this, we need to add a third-party NuGet package to the .NET Standard Library: [the Newtonsoft.Json package](https://github.com/JamesNK/Newtonsoft.Json).

With the package added, deserializing that string is effortless:

``` csharp
// added using Newtonsoft.Json;
Search searchResult = JsonConvert.DeserializeObject<Search>(json);
```

The searchResult variable now contains all the information that we retrieved from the Foursquare search API.

![](https://10daysofxamarin.files.wordpress.com/2019/03/day7-002.png)

Notice the venues list? That is the list that we must set as the items source for our list view! So we are ready to update the TextCell that we defined earlier to display a couple of properties from the Venue class (that venues list contains Venue objects). My TextCell template now looks like this:

``` xml
<TextCell Text="{Binding name}"
          Detail="{Binding location.distance, StringFormat='{0} meters away'}"/>
```

Don't forget to set the items source for the ListView, right after deserializing the JSON:

``` csharp
venuesListView.ItemsSource = searchResult.response.venues;
```

Now, every time you type in the search entry, the ListView is updated with the venues that were found:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day7-003.png)

Optionally, you can wrap all of this code inside of an if statement, so the search only happens when the search has some text. Notice that I'm also hiding/showing the list view accordingly:

``` csharp
async void SearchEntry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
{
    if (!string.IsNullOrWhiteSpace(searchEntry.Text))
    {
        string url = $"https://api.foursquare.com/v2/venues/search?ll={position.Latitude},{position.Longitude}&radius=500&query={searchEntry.Text}&limit=3&client_id={Helpers.Constants.FOURSQR_CLIENT_ID}&client_secret={Helpers.Constants.FOURSQR_CLIENT_SECRET}&v={DateTime.Now.ToString("yyyyMMdd")}";

        // added using System.Net.Http;
        using (HttpClient client = new HttpClient())
        {
            // made the method async
            string json = await client.GetStringAsync(url);

            // added using Newtonsoft.Json;
            Search searchResult = JsonConvert.DeserializeObject<Search>(json);
            venuesListView.IsVisible = true;
            venuesListView.ItemsSource = searchResult.response.venues;
        }
    }
    else
    {
        venuesListView.IsVisible = false;
    }
}
```

## Handling the selection of a venue

The only thing that our app is missing is displaying the information from the selected venue into the hidden labels. For this, we add some code to a new event handler. This event handler handles the ItemSelected event for the ListView. I assume you already know how to create that event handler form XAML and show you the code that you must add inside:

``` csharp
void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
{
    if(venuesListView.SelectedItem != null)
    {
        selectedVenueStackLayout.IsVisible = true;
        searchEntry.Text = string.Empty;
        venuesListView.IsVisible = false;

        Venue selectedVenue = venuesListView.SelectedItem as Venue;
        venueNameLabel.Text = selectedVenue.name;
        venueCategoryLabel.Text = selectedVenue.categories.FirstOrDefault()?.name;
        venueCoordinatesLabel.Text = $"{selectedVenue.location.lat:0.000}, {selectedVenue.location.lng:0.000}";
    }
    else
    {
        selectedVenueStackLayout.IsVisible = false;
    }
}
```

Hopefully, there is nothing new here; I am merely hiding or showing elements accordingly, and setting the text for the labels that I defined in the MainPage.

I do want you to notice the question mark right after the FirstOrDefault method call. The method gets the first element in a list (the categories list in this case), but if there are no elements in the list, it returns null. The question mark makes sure that, if the value is null, the code does not return an exception when trying to access the name variable.

Finally, we have to make sure that the labels are cleaned when an experience is saved. So in the event handler for the click of the save of a button, right after evaluating if the insertion was successful and after cleaning the text in the title entry and the content editor, also clean the text in these labels:

``` csharp
venueNameLabel.Text = string.Empty;
venueCategoryLabel.Text = string.Empty;
venueCoordinatesLabel.Text = string.Empty;
```

## Updating the Experience model

Now that we display all this information, the last step is for the Experience to contain the information about the selected venue. This only requires two steps:

- Updating the Experience class
- Updating the save experience code

The class should now include at least four new properties, with the information about the selected venue:

``` csharp
public class Experience
{
    [PrimaryKey, AutoIncrement] // added using SQLite;
    public int Id { get; set; }

    [MaxLength(50)]
    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string VenueName { get; set; } // NEW

    public string VenueCategory { get; set; } // NEW

    public float VenueLat { get; set; } // NEW

    public float VenueLng { get; set; } // NEW

    public override string ToString()
    {
        return Title;
    }
}
```

Also, when creating the Experience object from the save button event handler in the MainPage, that object should include this new data:

``` csharp
Experience newExperience = new Experience()
{
    Title = titleEntry.Text,
    Content = contentEditor.Text,
    CreatedAt = DateTime.Now,
    UpdatedAt = DateTime.Now,
    VenueName = venueNameLabel.Text,
    VenueCategory = venueCategoryLabel.Text,
    VenueLat = float.Parse(venueCoordinatesLabel.Text.Split(',')[0]),
    VenueLng = float.Parse(venueCoordinatesLabel.Text.Split(',')[1])
};
```

There you go, now the functionality of your app is complete!

You have now used information from the GPS and the Foursquare API to add a venue to the Experience. The Experiences that you insert into the database now contain more information, and you could update the TextCell template for the ListView in the ExperiencesPage to display some of that. However, even if you don't, all those entries in the database now contain some more interesting information, you could even use that information to display them on a map!

It is up to you what you do from here, but I will say something: our code is a mess. Which is why the three remaining days we will focus on implementing the MVVM architectural pattern, which will make our code more elegant, maintainable, and will be easier to understand.

[Get the final source code](https://github.com/LaloCo/10DaysOfXamarin/tree/Day7/final)
