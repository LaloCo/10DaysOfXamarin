In the past few days, you have created an experiences application that lets users insert experiences in a local database, and even lets them add a real venue to those experiences, so they know where the experience happened. This has led you to learn how to use not only XAML and C# but SQLite and REST services.

# Moving on

Our application, however, has a very complex code base. Our code is not clean, it is not well organized, and it is not the best we could do.

So far we have already structured our code in two folders: the View and the Model, which are two of the three parts that comprise the MVVM architectural pattern, but the View Model is still missing.

In short, the View Model will contain the functionality of your application, and that is currently inside of the Views folder, in the code behind for our XAML Pages. It is time to change that.

[This branch](https://github.com/LaloCo/10DaysOfXamarin/tree/Day8/initial)

# Challenge

To implement the MVVM architectural pattern, we start by using the INotifyPropertyChanged interface in a way that helps us clean some of the code. This interface removes the necessity to get and set values to and from the XAML pages into C# objects. Instead, Data Binding does the work.

You need to create two View Model classes, one for each of the views we currently have, create some properties inside of those classes, make the classes implement the INotifyPropertyChanged interface, and finally, bind the properties to some elements back in the corresponding XAML page.

## Problem

Our code is not very well structured yet; we need to clean it, so now, you create the View Model classes. Today you make sure that these classes implement the INotifyPropertyChanged interface and that they have the properties that are required. Spoiler alert: the ExperiencesPage doesn't require any properties just yet.

## App description

Here are a couple of examples of the properties that you may need:

- Title. This property is bound to the text inside of the title Entry that is inside the Main Page.
- Content. This property is bound to the text inside of the content Editor.

This means that, from the code, we don't need to keep getting and setting properties, we need to establish this binding.

You need an instance of the ViewModel class in the code behind for your views. You need to set this instance as the binding context for the entire page for the binding to work, and for the app's functionality to remain intact.

## Constraints

Lists work with an interface different to the INotifyPropertyChanged, so don't create properties to bind to the ListViews just yet.

# Solution

We start to implement the MVVM architectural pattern by creating a ViewModels folder -so now we have all the folders ready- and adding one class for each view that we have. In our case, our ViewModels folder has these two classes:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day8-001.png  )

It is often a great idea to call these classes in a way that is easy to identify to which view are they related. In some scenarios naming these classes, ExperiencesViewModel and MainViewModel may be a better idea, but this is good for now.

## Implementing the interface

Like I said, your View Models should implement the INotifyPropertyChanged interface. Granted, the MainVM won't need it but bear with me here. Let's implement the interface in both classes. Your class definitions now look like this:

``` csharp
public class MainVM : INotifyPropertyChanged { } // added using System.ComponentModel;

public class ExperiencesVM : INotifyPropertyChanged { }
```

Also, you have a new member added:

``` csharp
public event PropertyChangedEventHandler PropertyChanged;
```

This member is the critical part of the interface. It includes an event that the View subscribes to, and that we must fire every time a property changes (hence the name). This means that, from our View Model class, we must fire the event, and that back in the XAML files, elements are listening to this event and update accordingly.

For example, if we were to have a Title property binding to the text of a label, and we change the value of the property to something new, we must fire the PropertyChanged event. If we fire this event, the text property for the label knows about the change and update itself to the new value of the Title property without us needing to do it manually (aka doing something like titleLabel.Text = viewModel.Title; )

## Notifying for property changes

It is common practice to define a method that fires this event like this:

``` csharp
private void OnPropertyChanged(string propertyName)
{
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```

This method fires an event, which requires the sender and the name of the property that changed, only if there have been any subscriptions to it (hence the question mark). We have to call this method from the setter of our properties (you know, the method that is executed when the value of the property changes). Brilliant, right?

Here are the properties that we need in the MainVM, for all of the values that need to be bound over to the MainPage:

``` csharp
private string title;
public string Title
{
    get { return title; }
    set
    {
        title = value;
        OnPropertyChanged("Title");
    }
}

private string query;
public string Query
{
    get { return query; }
    set
    {
        query = value;
        OnPropertyChanged("Query");
    }
}

private Venue selectedVenue; // added using TenDaysOfXamarin.Model;
public Venue SelectedVenue
{
    get { return selectedVenue; }
    set
    {
        selectedVenue = value;
        OnPropertyChanged("SelectedVenue");
    }
}

private string content;
public string Content
{
    get { return content; }
    set
    {
        content = value;
        OnPropertyChanged("Content");
    }
}
```

Hopefully, all of them are self-explanatory, notice how all of them call the OnPropertyChanged method from the setter.

> Quick tip: to define a full property in Visual Studio, you can use the propfull snippet. This speeds up your workflow.

Eventually, we need another property, one that is bound to the ItemsSource for the ListView, but that uses a different interface to the one that we are using right now, so let's ignore that for now.

## Binding the properties

Now that we have all the properties ready, it is time to bind them to the view and modify our code behind a little bit.

### Binding the whole View Model

The idea here is to access all these new properties from XAML, for which we need to bind the whole View Model to the page. Similar to how we can access the properties from a venue or an experience from the template of the ListView, remember?

So what I do is define, inside of the MainPage.xaml.cs file, an instance of our MainVM class, and set it as the BindingContext for the entire Page:

``` csharp
// added using TenDaysOfXamarin.ViewModels;
MainVM viewModel;

public MainPage()
{
    InitializeComponent();

    viewModel = new MainVM();
    BindingContext = viewModel;

    locator.PositionChanged += Locator_PositionChanged;
}
```

That event handler creation was already there, by the way. This is similar to what we did when assigning the ItemsSource for the ListView. Now, from XAML, we have access to all the properties of the ViewModel, because it is the BindingContext for the whole thing.

This means that now we can bind the text of the titleEntry to our Title property like this:

``` xml
<Entry x:Name="titleEntry"
       Text="{Binding Title, Mode=TwoWay}"
       .../>
```

Setting the Mode to TwoWay makes sure that the binding works from the XAML element back to the ViewModel's property too. So while firing the PropertyChanged event from the setter of the Title property updates the Text in this entry, setting the mode to TwoWay makes sure that, if the user changes the text of the entry, it updates the Title property as well. This, if you think about it, makes the TextChanged events useless, but let's take one step at the time.

This is how the other XAML elements must now bind to the properties of our view model:

``` xml
<Entry x:Name="searchEntry"
       Text="{Binding Query, Mode=TwoWay}"
       .../>
<ListView x:Name="venuesListView"
          SelectedItem="{Binding SelectedVenue, Mode=TwoWay}"
          ...>
    ...
</ListView>
<StackLayout x:Name="selectedVenueStackLayout"
             IsVisible="false">
    <Label x:Name="venueNameLabel"
           Text="{Binding SelectedVenue.name}"
           .../>
    <Label x:Name="venueCategoryLabel"
           Text="{Binding SelectedVenue.MainCategory}"
           .../>
    <Label x:Name="venueCoordinatesLabel"
           Text="{Binding SelectedVenue.location.Coordinates}"
           .../>
</StackLayout>

<Editor x:Name="contentEditor"
        Text="{Binding Content, Mode=TwoWay}"
        .../>
```

Two main things to note here:

- The labels don't require us to set the Mode to TwoWay; in fact, it would be useless to do so. Users can't change what is on the labels!
- I added a new property to the Venue and Location classes, so it is easier to get the first category and coordinates:

``` csharp
// add to the Location class
private string coordinates;
public string Coordinates
{
    get { return $"{lat:0.000}, {lng:0.000}"; }
}

// add to the Venue class
private string mainCategory;
public string MainCategory
{
    // added using System.Linq;
    get { return categories.FirstOrDefault()?.name; }
}
```

Try running your app with a breakpoint in the event handler for the save of the button. Once the breakpoint hits, try inspecting the viewModel variable that we defined earlier. Notice something interesting?

![](https://10daysofxamarin.files.wordpress.com/2019/03/day8-002.png)

All the values are there! Without us needing to set them manually! Our bindings work!

## Code changes

Since most of the information is now in the view model, we can change the code a little bit, so we don't rely on the elements having names. Guess what this means? We don't need to name our XAML elements any more! So we can remove the names to all of the elements listed above (except for the ListView, more on that on Day 10), and change the code behind to this:

The CheckIfShouldBeEnabled now looks like this:

``` csharp
private void CheckIfShouldBeEnabled()
{
    saveButton.IsEnabled = false;
    if (!string.IsNullOrWhiteSpace(viewModel.Title) && !string.IsNullOrWhiteSpace(viewModel.Content))
        saveButton.IsEnabled = true;
}
```

Notice how I don't use the XAML elements anymore, but the value of the view model properties.

Similarly, the event handler for the click of the save button now looks like this:

``` csharp
void SaveButton_Clicked(object sender, System.EventArgs e)
{
    // added using TenDaysOfXamarin.Model;
    Experience newExperience = new Experience()
    {
        Title = viewModel.Title,
        Content = viewModel.Content,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now,
        VenueName = viewModel.SelectedVenue.name,
        VenueCategory = viewModel.SelectedVenue.MainCategory,
        VenueLat = float.Parse(viewModel.SelectedVenue.location.Coordinates.Split(',')[0]),
        VenueLng = float.Parse(viewModel.SelectedVenue.location.Coordinates.Split(',')[1])
    };

    int insertedItems = 0;
    // added using SQLite;
    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
    {
        conn.CreateTable<Experience>();
        insertedItems = conn.Insert(newExperience);
    }
    // here the conn has been disposed of, hence closed
    if (insertedItems > 0)
    {
        viewModel.Title = string.Empty;
        viewModel.Content = string.Empty;
        viewModel.SelectedVenue = null;
    }
    else
    {
        DisplayAlert("Error", "There was an error inserting the Experience, please try again", "Ok");
    }
}
```

The event handler for the TextChanged event of the search entry now evaluates the view model's Query property and uses it to create the url:

``` csharp
if (!string.IsNullOrWhiteSpace(viewModel.Query))
{
    string url = $"https://api.foursquare.com/v2/venues/search?ll={position.Latitude},{position.Longitude}&radius=500&query={viewModel.Query}&limit=3&client_id={Helpers.Constants.FOURSQR_CLIENT_ID}&client_secret={Helpers.Constants.FOURSQR_CLIENT_SECRET}&v={DateTime.Now.ToString("yyyyMMdd")}";
    ...
}
```

And the most significant change of all so far. The handler for the selection of an item from the ListView now only handles hiding and showing elements (and cleans the search entry through the Query property).

``` csharp
void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
{
    if(venuesListView.SelectedItem != null)
    {
        selectedVenueStackLayout.IsVisible = true;
        viewModel.Query = string.Empty;
        venuesListView.IsVisible = false;
    }
    else
    {
        selectedVenueStackLayout.IsVisible = false;
    }
}
```

Our code is a bit cleaner right now, and it keeps working in the same way. The main takeaway is the fact that we are beginning to separate our view from the logic (the View Model), and that is precisely what we should always aim for.

## Bonus

With the INotifyPropertyChanged implementation that we already have, we can do some more code cleaning with little effort.

Take a look, for example, at these three methods:

``` csharp
private void CheckIfShouldBeEnabled()
{
    saveButton.IsEnabled = false;
    if (!string.IsNullOrWhiteSpace(viewModel.Title) && !string.IsNullOrWhiteSpace(viewModel.Content))
        saveButton.IsEnabled = true;
}

void TitleEntry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
{
    CheckIfShouldBeEnabled();
}

void ContentEditor_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
{
    CheckIfShouldBeEnabled();
}
```

It should feel like you can get rid of them, after all, with the help of the PropertyChanged event and the binding mode set to TwoWay, you already know when the text in the title entry and the content editor change! Moreover, every time they do, their setters are called, so perhaps the CheckIfShouldBeEnabled method should be called from the setters of the view model's Title and Content properties.

However, I have an even better solution. Let's create a new property: the CanSave property that only returns true when both the Title and the Content are not null nor whitespace:

``` csharp
public bool CanSave
{
    get { return !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Content); }
}
```

It does not need a setter because it relies entirely on other properties. However, this means that those other properties should call the OnPropertyChanged method for this property when *they* change:

``` csharp
private string title;
public string Title
{
    get { return title; }
    set
    {
        title = value;
        OnPropertyChanged("Title");
        OnPropertyChanged("CanSave");
    }
}

private string content;
public string Content
{
    get { return content; }
    set
    {
        content = value;
        OnPropertyChanged("Content");
        OnPropertyChanged("CanSave");
    }
}
```

This is the MVVM version of the event handlers for when the text changes in the entry and editor. Finally, let's bind our new property to the IsEnabled property of our save button:

``` xml
<Button x:Name="saveButton"
        IsEnabled="{Binding CanSave}"
        .../>
```

This code removes the need for all three methods mentioned earlier. So go ahead and remove them. Also, don't forget to update the Editor and title Entry XAML elements accordingly (they should not create any event handlers any more).

Something similar can happen with the SearchEntry_TextChanged event handler. This, however, results in the list that gets bound to the list view, that is something we cover until day 10.

Something else we can change, though, is the Handle_ItemSelected event handler. All it does is hide and show elements. It works very similarly to our CanSave method. So let's create a couple of new properties in the view model:

``` csharp
public bool ShowSelectedVenue
{
    get { return SelectedVenue != null; }
}

private bool showVenues;
public bool ShowVenues
{
    get { return showVenues; }
    set
    {
        showVenues = value;
        OnPropertyChanged("ShowVenues");
    }
}
```

The first one, again, relies entirely on another property, which should call the OnPropertyChanged and pass "ShowSelectedVenue":

``` csharp
private Venue selectedVenue;
public Venue SelectedVenue
{
    get { return selectedVenue; }
    set
    {
        selectedVenue = value;
        OnPropertyChanged("SelectedVenue");
        OnPropertyChanged("ShowSelectedVenue");
    }
}
```

The second one is independent, and for it to function correctly, we need to set its value instead of setting the venuesListView.IsVisible value. So, inside of the SearchEntry_TextChanged method, search all the two instances of that venuesListView.IsVisible and change them, so they now use the ShowVenues property from the view model.

To finally make the Handle_ItemSelected useless, let's move the lines where we set the query to be empty and hide the list view over to the setter of the SelectedVenue property, which, after all, acts pretty much like the event handler for when the selected item changes:

``` csharp
private Venue selectedVenue;
public Venue SelectedVenue
{
    get { return selectedVenue; }
    set
    {
        selectedVenue = value;
        if (selectedVenue != null)
        {
            ShowVenues = false;
            Query = string.Empty;
        }

        OnPropertyChanged("SelectedVenue");
        OnPropertyChanged("ShowSelectedVenue");
    }
}
```

Don't forget to bind the properties from XAML and now, you can get rid of that Handle_ItemSelected method from both the C# file and the XAML definition for the ListView:

``` xml
<ListView x:Name="venuesListView"
          IsVisible="{Binding ShowVenues}"
          ...>
    ...
</ListView>
<StackLayout x:Name="selectedVenueStackLayout"
             IsVisible="{Binding ShowSelectedVenue}">
    ...
</StackLayout>
```

By the way, because of the changes that we made, the following elements don't require a name any more:

- The selected venue StackLayout
- The Save Button
- Unfortunately, the venuesListView still needs a button, but more on how to get rid of it later!

If you got lost in the code a little bit, your app is not working as described, or you found some bugs, check out the branch for today's code here:

[Get the final source code](https://github.com/LaloCo/10DaysOfXamarin/tree/Day8/final)
