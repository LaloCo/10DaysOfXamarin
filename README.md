Our application is already successfully saving and listing experiences that we stored on a local SQLite database. So far, however, the experiences only include a title and a content that users may have added. While this functionality is useful, I want to make this app a bit more interesting, which is why we are now going to be working on adding information about the venue where the user has the experience.

# Moving on

I want our application to let the user select, from a list of nearby venues, the place where the experience took place. To add this functionality, we need to things:

- The location of the user, which we get today using the GPS.
- The list of nearby venues, which we will get tomorrow using Foursquare's REST API.

This way, each experience contains, in addition to the title and content, information about the venue where it took place.

[This branch](https://github.com/LaloCo/10DaysOfXamarin/tree/Day6/initial)

# Challenge

Prepare the projects, so they can access GPS information from the device and code the functionality that gets us the coordinates for the current location.

## Problem

Accessing the GPS requires the user to grant the permissions. Asking for permissions can be particularly tricky on Android, so you use a [handy plugin](https://github.com/jamesmontemagno/PermissionsPlugin) that makes things easier.

Once you are confident that the user granted the required permissions, you can request the current location and store the latitude and longitude in a couple of variables. That couple of variables are used tomorrow to get the nearby venues.

## App description

The UI won't change today, but you should add a new NuGet package to your projects and make some changes to the platform-specific projects so that the permission requests work.

## Constraints

The accuracy of the location should be high so that the coordinates received from the GPS are as close to the actual location as possible. By setting a high accuracy for the location, we can get very accurate lists of nearby venues. High accuracy can quickly drain a phone's battery, so make sure that the events for location changes are only fired when the change in location is significant (e.g., 500 meters or 1500 feet).

# Solution

[The Permissions Plugin](https://github.com/jamesmontemagno/PermissionsPlugin) that I mentioned above is going to make things way easier when it comes to asking for various permissions. Permissions are required when you access specific capabilities that Apple or Google deem essential, such as camera, GPS, FaceID, photos library, microphone, etc.

## Preparing the application

So, we need to add that plugin to our projects, in a similar way in which we added the SQLite package: through NuGet. Make sure you check Day 3 if you need a reminder of how to do so, and add the Plugin. Permissions package to all of the projects in your solution.

### Android Specific

Note that in the case of the Android project, another package is added automatically: [the Current Activity plugin](https://github.com/jamesmontemagno/CurrentActivityPlugin). If for some reason you don't see this package referenced, do add it manually. Again, the Android project is the only one that requires this package.

Whether because it was added automatically, or because you added it manually, your MainActivity's OnCreate method should now have this next line of code, if it doesn't, make sure you add it:

``` csharp
// Here, the savedInstanceState is the name of the Bundle variable received
// by the OnCreate method
Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
```

Still, on the MainActivity, we now need to set up the Permissions plugin. In the Activity, there exists an OnRequestPermissionsResult virtual method that we can override. This method, as its name suggests, receives some information about whether or not the user granted a permission request. The Permissions Plugin needs this information, so make sure that this method gets overridden and that the plugin receives that information as well:

``` csharp
public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
{
    // added using Plugin.Permissions;
    PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}
```

Finally, you need to let the OS know that your app wants to use a precise location. For this, open the AndroidManifest.xml file and select from the required permissions the AccesFineLocation value. Note that there are more location values, but the Fine location gets a more precise location than coarse. Alternatively, you should now see this line in the XML source code:

``` xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

[https://www.youtube.com/watch?v=0SrAzPt2XqA](https://www.youtube.com/watch?v=0SrAzPt2XqA)

[https://www.youtube.com/watch?v=SElRz9nufIQ](https://www.youtube.com/watch?v=SElRz9nufIQ)

### iOS Specific

The iOS project requires us to define specific information to the info.plist when we need to request permission to access specific capabilities. Since we are going to be accessing the location, we need to add the following key to the file:

[https://www.youtube.com/watch?v=BIvBt9Y4Qic](https://www.youtube.com/watch?v=BIvBt9Y4Qic)

[https://www.youtube.com/watch?v=DjX1UC8dL6I](https://www.youtube.com/watch?v=DjX1UC8dL6I)

Privacy - Location When In Use Usage Description (NSLocationWhenInUseUsageDescription): We use your device's location to get nearby venues for the experiences to be pinned to the place where you had them.

That key defines the message that the operating system displays to the user when you app requests access to the location. Now, this particular key works for when you request access to the location **only** when the app is being used, which is precisely what we need. There is however a second key that you could use if you want your app to access the location even when it is not in use. In that case, you set the *Privacy - Location Always Usage Description (NSLocationAlwaysUsageDescription)* key.

Keep in mind which is the one that you are setting, for it is going to be important when requesting the permission.

These are the permissions that you need by default. However, the creator of the Permissions plugin states that, "due to API usage, it is required to add the Calendar permission :(" and that "you may also see this for (Bluetooth)." So while this couple of keys make no sense in the context of the permission, you need to add them:

- Privacy - Calendars Usage Description (NSCalendarsUsageDescription): "We may use your calendar to create experiences based on current events."
- Privacy - Bluetooth Peripheral Usage Description (NSBluetoothPeripheralUsageDescription): "We may use Bluetooth Peripherals to understand what devices you use while saving an experience."

The descriptions here are, of course, not real, but Apple may refuse to publish your app in the store if the description is not useful to the user.

## Requesting the permission

Now that both our platform-specific projects are complete, let's navigate over to the MainPage C# file and add the code that requests permission to access the location.

I start by defining a new GetLocationPermission method that is called form the OnAppearing method of the MainPage. This method will, right from the beginning, check if the permission for location is already granted. Now notice that I am checking for the status of the LocationWhenInUse description, the permission that you check the status for changes depending on the setting that you set in the info.plist file.

``` csharp
private async void GetLocationPermission()
{
    // added using Plugin.Permissions;
    // added using Plugin.Permissions.Abstractions;
    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
    if(status != PermissionStatus.Granted)
    {
        // Not granted, request permission
    }

    // Already granted (maybe), go on
}
```

It is worth mentioning that, while the name of the permissions reflects the different options available on iOS (LocationWhenInUse, LocationAlways, and Location), all of the Location permissions listed in the Permissions enumerable work in the same way for Android.

Also, make sure that you set the method to be async, this means that the method executes asynchronously, which is required because the CheckPermissionStatusAsync method is also asynchronous.

Returning to the code, if the permission for the location is not granted, we must request permission, and then assign to that same status variable, the result of the request:

``` csharp
// Not granted, request permission
if(await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.LocationWhenInUse))
{
    // This is not the actual permission request
    await DisplayAlert("Need your permission", "We need to access your location", "Ok");
}

// This is the actual permission request
var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.LocationWhenInUse);
if (results.ContainsKey(Permission.LocationWhenInUse))
    status = results[Permission.LocationWhenInUse];
```

Notice the new if statement. This won't do anything in the case of iOS since this is Android specific, but in case the Location permission requires us to "show request permission rational," we should display a message to the user.

So now we have requested the permission! Awesome. Either because the permission was already granted, or because the status changed after requesting it, we can now double check the values in the status variable and act accordingly:

``` csharp
// Already granted (maybe), go on
if(status == PermissionStatus.Granted)
{
    // Granted! Get the location
}
else
{
    await DisplayAlert("Access to location denied", "We don't have access to your location", "Ok");
}
```

With our application requesting the permissions, we can now run it, navigate to the MainPage, and see each operating system requesting this permission:

| Image 1        | Image 2           |
| ------------- |:-------------:|
| ![](https://10daysofxamarin.files.wordpress.com/2019/03/day6-001.png)      | ![](https://10daysofxamarin.files.wordpress.com/2019/03/day6-002.png) |

## Getting the location

Now that we request this permission, we can get the actual location, which we can request with yet another plugin by James Montemagno: [the Geolocator Plugin](https://github.com/jamesmontemagno/GeolocatorPlugin).

Start by adding this plugin to all of your projects, and once it is there, open the MainPage again.

I now create a GetLocation method that I call once I know that the Permission has been granted. This method also calls an asynchronous method, so I mark it as async. Also, inside our new GetLocation method, all we need is to create a CrossGeolocator variable and use it to get the current location:

``` csharp
Position position;
private async void GetLocation()
{
    // added using Plugin.Geolocator;
    var locator = CrossGeolocator.Current;
    position = await locator.GetPositionAsync();
}
```

Voilà! Now the position variable (which is now a global variable) contains the coordinates with the location of the user:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day6-003.png)

## Handling Position Changes

We are already getting the location, and we could leave the code as it is right now and it works very well. However, I do want to get a new location every time it has changed significantly (let's say 500 meters or 1500 feet).

To handle the location changes, we continue to use the locator variable that we created in the GetLocation method. However, since I need this locator variable from outside the GetLocation method, let me define it as a global variable:

``` csharp
// added using Plugin.Geolocator;
// added using Plugin.Geolocator.Abstractions;
IGeolocator locator = CrossGeolocator.Current;
```

I still use it from the GetLocation method, but I now have access to it from every method in the class.

Having access to it from every method is important because I need it from three different methods:

- From the constructor of the class, where I create an event handler for its PositionChanged event

``` csharp
public MainPage()
{
    InitializeComponent();

    locator.PositionChanged += Locator_PositionChanged;
}

void Locator_PositionChanged(object sender, PositionEventArgs e)
{
    position = e.Position; // this uses the global variable defined earlier
}
```

- From the GetLocation method, where I tell the locator to start listening for location changes

``` csharp
private async void GetLocation()
{
    var position = await locator.GetPositionAsync();
    await locator.StartListeningAsync(TimeSpan.FromMinutes(30), 500);
}
```

- From the overridden OnDisappearing, where I tell the locator to stop listening for location changes

``` csharp
protected override void OnDisappearing()
{
    base.OnDisappearing();

    locator.StopListeningAsync();
}
```

Let's start by taking a look at the event handler. Here is where we know the location has changed by more than 500 meters (approximately 1500 feet). This event handler already contains, in its arguments, the new location.

Now onto the GetLocation method. This method is calling the StartListeningAsync method, telling the locator only to fire the PositionChanged event once either of two things has happened:

- Thirty minutes have passed since we last received the location
- The position has changed by at least five hundred meters.

It is essential to set these restrictions because listening for location changes too often drains the battery, so make sure you set this event to fire as little as possible.

Finally, OnDisappearing. I think you can guess when this method is called, it is the opposite of the OnAppearing, and calling the StopListeningAsync from here makes sure that we stop listening for location changes since the user would no longer be in the page that needs the location.

Now that we get the current location we are ready to use the Foursquare API to get nearby venues. That is what tomorrow's challenge is about.

[Get the final source code](https://github.com/LaloCo/10DaysOfXamarin/tree/Day6/final)
