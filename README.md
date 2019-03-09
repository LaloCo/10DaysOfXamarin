Our implementation of the MVVM architectural pattern has led us to implement the ICommand and INotifyPropertyChanged interfaces in certain classes. When using the INotifyPropertyChanged, we used its event to notify of property changes so the view can be updated accordingly.

So far, however, we are not yet creating the list properties that would bind to the item source of our list views, that is what we do today.

# Moving on

We don't yet have properties that notify of changes on a list, so we can bind it to the items source of a list view, and notify it whenever that property changes. Can you guess why not? Why not use the INotifyPropertyChanged?

The reason is simple: a list doesn't change when its items do. We don't need notifications for when the list property changes, we need notification for when its content changes.

The INotifyPropertyChanged can't notify for collection changes, aka new items added or items deleted. Enter the INotifyCollectionChanged. However, you don't need to create a new class that implements this interface, there is already one class that we have access to, and that implements it: the ObservableCollection<T>.

[this branch](https://github.com/LaloCo/10DaysOfXamarin/tree/Day10/initial)

# Challenge

As the name of theObservableCollection<T> class implies, changes to the collection are observed; hence, if we use this to bind to our view, the view can react to those changes.

You need to replace the code that is currently getting the lists (of venues or experiences) and assigning them as the "ItemsSource" for the corresponding ListViews, for new ObservableCollection properties inside of the View Models. You can then bind these new properties to the same ItemsSource, but now from XAML code, so the code behind is much cleaner.

## Problem

Getting the venues depends on getting some permissions and the location coordinates. This code should be migrated to the ViewModel as well. For better code structure, I recommend you create a new Helpers folder inside of the ViewModels folder, and two new classes inside of the new folder: the PermissionsHelper and the LocationHelper.

## App description

We should migrate the code that is already inside of the code behind for the MainPage to the ViewModel, preferably with this structure:

- To the PermissionsHelper class: all the code that handles the permission request for location access.
- To the LocationHelper class: all the code that handles getting the current location and getting updates on location changes.
- To the Experience class: all the code that reads the database.
- To the Search class: all the code that makes the REST request.
- To the MainVM class: all the remaining code that uses all the helpers and prepares the information for the view to display.

## Constraints

You should leave both code-behind files with nothing but the definition of a view model object, its initialization, and its setting as the BindingContext for the Page. Optionally, you could also remove this code by setting the BindingContext from the XAML file.

Another thing that you can do is move the code that is currently inserting to the SQLite table over to the Experience class, but this won't affect the amount of code that is in the code behind anymore.

# Solution

Let's start with the Experiences page, which is the simpler one to solve. It is pretty much a matter of moving the code that we have in the code behind over to some other class, outside of the View.

Currently, the ExperiencesPage.xaml.cs file contains the ReadExperiences method, and we call this method from the OnAppearing. We should change these two tiny things.

## Reading from the model

Instead of reading the table from the view, we should consider moving the code that interacts with the database to the model. This means that we have to add the code that reads the table to the Experience class. After we do this, it is the model's job to get the values from the table. Create a new method in the Experience class that looks like this:

``` csharp
public static List<Experience> GetExperiences()
{
    // added using System.Collections.Generic;
    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
    {
        conn.CreateTable<Experience>();
        return conn.Table<Experience>().ToList();
    }
}
```

As you can see, it does the same thing as before, except that it sets no ItemsSource.

Having this code, we could change the method that is inside of the code-behind to this:

``` csharp
private void ReadExperiences()
{
    experiencesListView.ItemsSource = Experience.GetExperiences();
}
```

We have just moved the code that communicates with the database from the view to the model.

## Binding from the View Model

The goal, though, is to remove all code from the view. The next step, then, is to move that ReadExperiences method over to the MainVM class. That is easy; we only define a new method to get the experiences:

``` csharp
public void ReadExperiences()
{
    // added using TenDaysOfXamarin.Model;
    var experiences = Experience.GetExperiences();
}
```

We can't set the ItemsSource because the list view doesn't exist in this context, but we already have that list of experiences. What's more, we can remove the method from the code behind, which means we also remove the call to that method from the OnAppearing method.

To bind this list of experiences, now we define the ObservableCollection<T> property. The T inside of the angle brackets, by the way, stands for the type of elements that this collection holds. For example, if we create it as an ObservableCollection<string>, then the collection is only able to hold strings. Knowing this, our ObservableCollection looks like this:

``` csharp
public ObservableCollection<Experience> Experiences { get; set; }

public ExperiencesVM()
{
    NewExperienceCommand = new NewExperienceCommand(this);
    Experiences = new ObservableCollection<Experience>();
    ReadExperiences();
}
```

Notice I initialize it in the constructor, and the constructor now calls the ReadExperiences method, so we read the table since the beginning. Now, it's time to add all of the elements that we have retrieved from the database to this new collection. Add this code to the ReadExperiences method:

``` csharp
Experiences.Clear();
foreach(var experience in experiences)
{
    Experiences.Add(experience);
}
```

It is essential to clear the collection before adding more elements, just so we remove previous elements. This way, only the items that we retrieve from the table are listed, and there are no duplicates.

Finally, bind our new property over to the ListView's ItemsSource:

``` xml
<ListView ItemsSource="{Binding Source={StaticResource vm}, Path=Experiences}">
    ...
</ListView>
```

Notice the binding syntax. Instead of setting the binding context, I am pointing to the resource directly from the binding. In this case, I need to set the resource to be the source, and the path to be the property I need. This has the same effect as defining the binding context and items source like this:

``` xml
<ListView BindingContext="{StaticResource vm}"
          ItemsSource="{Binding Experiences}">
    ...
</ListView>
```

So no, it doesn't need any names anymore.

We should still call the ReadExperiences method from the OnAppearing of our view. We could change the binding from the XAML version to the C# version, or we can get the resource like this:

``` csharp
ExperiencesVM viewModel;
public ExperiencesPage()
{
    InitializeComponent();

    viewModel = Resources["vm"] as ExperiencesVM;
}
```

This code gets the resource that we define in XAML, casts it as an ExperiencesVM (which we know is its type) and assigns it to a global viewModel variable. Now we use the view model to call the method from the OnAppearing:

``` csharp
protected override void OnAppearing()
{
    base.OnAppearing();

    viewModel.ReadExperiences();
}
```

There you go, we barely have any code in the code behind anymore. All of the code inside it is entirely related to the view.

## Moving the MainPage functionality

The MainPage has a lot more functionality that the ExperiencesPage. We could identify three main components for this functionality:

- The Permissions functionality
- The Location functionality
- The Venues request

We should move each of these parts somewhere else. It wouldn't be ideal for the MainVM to hold all of this functionality, since getting permissions, getting the location, and getting the venues are not something that is unique for the Main VM, other classes and pages may eventually need to do the same. So I recommend you create a new Helpers folder inside of the ViewModel with two new classes:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day10-001.png)

### Moving the Permissions functionality

Let's start by moving the permissions functionality to the new PermissionsHelper class. This is pretty much copy paste. Just make sure that the code in the PermissionsHelper looks like this:

``` csharp
// added using Plugin.Permissions.Abstractions;
// added using System.Threading.Tasks;
public static async Task<PermissionStatus> GetPermission(Permission permissionType)
{
    // added using Plugin.Permissions;
    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permissionType);
    if (status != PermissionStatus.Granted)
    {
        // This is the actual permission request
        var results = await CrossPermissions.Current.RequestPermissionsAsync(permissionType);
        if (results.ContainsKey(permissionType))
            status = results[permissionType];
    }

    return status;
}
```

I did change a couple of things. Mainly there is no DisplayAlert anymore, and the method requests the permission type, instead of assuming LocationWhenInUse; this way, our method works for any permission that we may need to request.

Also, notice that the method is async and should return the status of the permission. So far, all of our async methods have been void, but if an async method should return a value, it should also be a Task. Returning a task means that only after awaiting the call for this method will we get the PermissionStatus. To clear this up, take a look at this brief example:

``` csharp
// variable is of type Task
var variable = GetPermission(Permission.LocationWhenInUse);
// result is of type PermissionStatus
var result = await variable;

// you can also get the result (of type PermissionStatus) in one line:
var result = await GetPermission(Permission.LocationWhenInUse);
```

Having moved the code for the request of the permission to its class, we need to change the code behind a bit:

``` csharp
private async void GetLocationPermission()
{
    // added using TenDaysOfXamarin.ViewModels.Helpers;
    var status = await PermissionsHelper.GetPermission(Permission.LocationWhenInUse);

    // Already granted (maybe), go on
    if (status == PermissionStatus.Granted)
    {
        // Granted! Get the location
        GetLocation();
    }
    else
    {
        await DisplayAlert("Access to location denied", "We don't have access to your location", "Ok");
    }
}
```

Not yet perfect, but cleaner.

### Moving the Location functionality

Let's now move the functionality that gets the current location to our new LocationHelper class. Again, this is pretty much copy and paste. Your LocationHelper class should look like this:

``` csharp
public class LocationHelper
{
    // added using Plugin.Geolocator;
    // added using Plugin.Geolocator.Abstractions;
    public IGeolocator locator = CrossGeolocator.Current;
    public Position position;

    public LocationHelper()
    {
        locator.PositionChanged += Locator_PositionChanged;
    }

    void Locator_PositionChanged(object sender, PositionEventArgs e)
    {
        position = e.Position;
    }

    // added using System.Threading.Tasks;
    public async Task<Position> GetLocation(TimeSpan minimumTime, int minimumMeters)
    {
        position = await locator.GetPositionAsync();
        await locator.StartListeningAsync(minimumTime, minimumMeters);
        return position;
    }

    public async void StopListening()
    {
        await locator.StopListeningAsync();
    }
}
```

Momentarily the code-behind won't work, but now all the location functionality is in this new LocationHelper class. Also notice that the GetLocation method requests the minimum time and minimum meters, instead of using 30 minutes and 500 meters by default.

### Moving the Foursquare functionality

Finally, let's move the functionality that gets the venues over to the Search class that we have in the Search.cs file. The Search class should now have a new SearchRequest method, which I define to look like this:

``` csharp
public static async Task<Response> SearchRequest(double lat, double lng, int radius, string query, int limit = 3)
{
    string url = $"https://api.foursquare.com/v2/venues/search?ll={lat},{lng}&radius={radius}&query={query}&limit={limit}&client_id={Helpers.Constants.FOURSQR_CLIENT_ID}&client_secret={Helpers.Constants.FOURSQR_CLIENT_SECRET}&v={DateTime.Now.ToString("yyyyMMdd")}";

    // added using System.Net.Http;
    using (HttpClient client = new HttpClient())
    {
        // made the method async
        string json = await client.GetStringAsync(url);

        // added using Newtonsoft.Json;
        return JsonConvert.DeserializeObject<Search>(json).response;
    }
}
```

Notice that the method now receives the latitude, longitude, radius, etcetera. It also sets a default 3 to the limit, but we could change it. The method returns only the response, ignoring the meta property, but you could change this by returning the entire Search value.

## Gluing everything together from the View Model

Now that we have all of the functionality outside of the code behind for our view is time to glue everything together from the View Model. If you have been removing the code from the code behind, it should look like some incomplete shell that calls methods from outside of itself:

``` csharp
public partial class MainPage : ContentPage
{
    // added using TenDaysOfXamarin.ViewModels;
    MainVM viewModel;

    public MainPage()
    {
        InitializeComponent();

        viewModel = new MainVM();
        BindingContext = viewModel;
    }

    private async void GetLocationPermission()
    {
        // added using TenDaysOfXamarin.ViewModels.Helpers;
        var status = await PermissionsHelper.GetPermission(Permission.LocationWhenInUse);

        // Already granted (maybe), go on
        if (status == PermissionStatus.Granted)
        {
            // Granted! Get the location
            GetLocation();
        }
        else
        {
            await DisplayAlert("Access to location denied", "We don't have access to your location", "Ok");
        }
    }

    async void SearchEntry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(viewModel.Query))
        {
            venuesListView.ItemsSource = (await Search.SearchRequest()).venues;
            viewModel.ShowVenues = true;
        }
        else
        {
            viewModel.ShowVenues = false;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        GetLocationPermission();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}
```

The following is how each method should change:

- The Constructor remains the same
- We move the GetLocationPermission to the view model
- We remove the SearchEntry_TextChanged, and we now call the code inside of it from the setter for the Query property that is inside of the view model.
- The OnAppearing still calls the GetLocationPermission, which is now in the view model.
- The OnDisappearing still calls the StopListening that exists now in the LocationHelper, except that the locator now exists in the View Model.

Start by moving the GetLocationPermission, which, as I mentioned before, we still call from the OnAppearing back in the view's code behind:

``` csharp
LocationHelper locationHelper;
public MainVM()
{
    // added using Xamarin.Forms;
    CancelCommand = new Command(CancelAction);
    SaveCommand = new Command<bool>(SaveAction, CanExecuteSave);

    locationHelper = new LocationHelper();
}

public async void GetLocationPermission()
{
    // added using TenDaysOfXamarin.ViewModels.Helpers;
    // added using Plugin.Permissions.Abstractions;
    var status = await PermissionsHelper.GetPermission(Permission.LocationWhenInUse);

    // Already granted (maybe), go on
    if (status == PermissionStatus.Granted)
    {
        // Granted! Get the location
        await locationHelper.GetLocation(TimeSpan.FromMinutes(30), 500);
    }
    else
    {
        await App.Current.MainPage.DisplayAlert("Access to location denied", "We don't have access to your location", "Ok");
    }
}
```

Notice that I defined a global LocationHelper variable, which we also initialized in the constructor. It is then used to call the GetLocation method that we defined in that class, which receives the minimum time and minimum distance to use when getting location changes.

Back in the code behind we can update the OnAppearing to use this method:

``` csharp
protected override void OnAppearing()
{
    base.OnAppearing();

    viewModel.GetLocationPermission();
}
```

To stop listening for those location changes, define a new method in the view model:

``` csharp
public void StopListeningLocationUpdates()
{
    locationHelper.StopListening();
}
```

It should, of course, use the method that we defined in the LocationHelper class. This method can now be called from the OnDisappearing, back in the code behind:

``` csharp
protected override void OnDisappearing()
{
    base.OnDisappearing();
    viewModel.StopListeningLocationUpdates();
}
```

Finally, getting the venues. For this, I need to perform three steps in the view model:

1. Create a new ObservableCollection and make sure you initialize it from the constructor of the view model:

``` csharp
// added using System.Collections.ObjectModel;
public ObservableCollection<Venue> Venues { get; set; }
```

2. Create the method that gets the venues using the Search class and adds them to the collection. Don't forget to clean the collection beforehand!

``` csharp
public async void GetVenues()
{
    var response = await Search.SearchRequest(locationHelper.position.Latitude, locationHelper.position.Longitude, 500, Query);

    Venues.Clear();
    foreach(var venue in response.venues)
    {
        Venues.Add(venue);
    }
}
```

3. Call this method when the Query changes -from its setter- and it is not null:

``` csharp
private string query;
public string Query
{
    get { return query; }
    set
    {
        query = value;
        if (!string.IsNullOrWhiteSpace(query))
        {
            GetVenues();
            ShowVenues = true;
        }
        else
        {
            ShowVenues = false;
        }
        OnPropertyChanged("Query");
    }
}
```

Notice that I change the value of the ShowVenues property accordingly. Adding this code to the setter of the query means we don't need the event handler for the TextChanged event of the search entry. Remove it from both the C# file and the XAML definition.

Finally, bind the new ObservableCollection property to the ListView:

``` xml
<ListView x:Name="venuesListView"
          ItemsSource="{Binding Venues}"
          ...>
    ...
</ListView>
```

With this, the challenge is complete!

You should be able to run the application and check that the functionality is the same, but our view's code behind is now super clean!

Feel free to compare your code with the last branch of our repo, which contains all the changes that we made today:

[Get the final source code](https://github.com/LaloCo/10DaysOfXamarin/tree/Day10/final)

# What’s next?
You learned a lot about Xamarin in the past 10 days, but there is so much more to learn!

My Complete Xamarin Course on Udemy includes 28.5 hours of content! It covers topics like:

1. Maps and Location
2. Cloud Services and Databases
3. Cloud File Containers
4. Way more about MVVM
5. Native Xamarin
6. Publishing your app
7. And much, much more!

Even better, you can enroll for only $9.99 if you use the button below. Continue your career as a Xamarin Developer!

[Get The Complete Xamarin Developer Course!](https://www.udemy.com/complete-xamarin-developer-course-ios-and-android/?couponCode=10DAYSOFXAMARIN)
