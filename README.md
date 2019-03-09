Yesterday we learned how to connect to an SQLite database, create a table and insert into it. So far, however, we are only able to know if we successfully inserted by getting the return value of the Insert method, we don't yet know how to read the table and check if the items are there. Because of this, we are also not yet listing the items for the user to see.

# Moving on

Today then, we learn how to read the table that we created yesterday and use the list of experiences that is retrieved to populate a list view that displays the information that we require.

What's more, today we learn a bit about data binding, which is super important for all Xamarin developers to learn. It's ok if Data Binding doesn't entirely settle into your workflow, throughout the reminding days, we will focus on using MVVM -which relies on Data Binding a lot- so you get a chance to keep practicing it.

[LaloCo/10DaysOfXamarin](https://github.com/LaloCo/10DaysOfXamarin/tree/Day5/initial)

# Challenge

Create a new ExpriencesPage page that contains a ListView. Make this new page the MainPage from the App's constructor. Make sure you fully implement navigation to the page where users can insert new experiences. The navigation must happen at the click of an "add" or "new" button, I recommend you use a ToolbarItem type of button, but a standard button could do.

Create a new connection to the same SQLite database that we used yesterday, this time to read the Experience table. Once the items in the table exist on a local variable, use that variable to populate a ListView that you also need to define.

## Problem

Implementing the navigation requires a specific type of container for the pages: the NavigationPage. This handy container handles back buttons, title bars, and of course navigation between pages. Make sure you create one of these pages when assigning the MainPage from the App.xaml.cs file.

The ListViews must have a template for the cells that they display. Here is where DataBinding comes into play, but also, existing cell styles are useful, so we don't define the UI from the ground up. Here I recommend you use a TextCell, which already contains a couple of Labels for you to display some text.

## App description

The app now has two pages:

- The ExperiencesPage, which lists all the items added to the Experience table
- The MainPage, which we already defined and that allows users to insert into the Experience table.

The ExperiencesPage should contain two elements:

- A Button -or preferably a ToolbarItem- for adding new experiences
- A ListView, listing all the experiences from the table

An so, there must be some navigation implemented: when users click the button in the ExperiencesPage, they should be navigated over to the MainPage. Also, once the cancel button is pressed, users must be navigated back to the ExperiencesPage.

## Constraints

The population of the Experiences ListView must be done through a simple form of DataBinding, setting the ItemsSource for that list view from the C# code behind, but creating some binding from the XAML file to a couple of the Experience class' properties.

# Solution

Today's is a challenging problem. There are a couple of new things that we must learn today:

- Navigation between pages
- Data Binding to a ListView

The first one is super easy; you'll get the grabs of it quickly. The second one, however, can be overwhelming at first, and if you are like me, you need to practice it for a few weeks before you fully master it. The slight complexity of Data Binding is one of the reasons why I want to introduce it step by step throughout the rest of the challenges, starting with this one, so let's get started.

## New Page and Navigation

Let's start with the most natural part of this challenge. Like I mentioned, you need to create a new Page.

### Creating the Page

Traditionally, you'll want to create this new Page inside of a new folder called Views. Similar to how we created the Experience class inside a Model folder, creating the new Page inside of the Views folder is by no means a requirement, but it helps you keep your pages organized. Optionally, you can move the MainPage over to this new Views folder as well.

[https://www.youtube.com/watch?v=uY0LjtVHUsA](https://www.youtube.com/watch?v=uY0LjtVHUsA)

[https://www.youtube.com/watch?v=vdcnDiswJZM](https://www.youtube.com/watch?v=vdcnDiswJZM)

Pay particular attention to the template that you use to create these pages, as described in the videos above, selecting the wrong template results in you not having a XAML file where to define the UI.

### ListView and ToolbarItem

This new Page lists all experiences, and display a "new" button that navigates our users to the MainPage for them to add new experiences. I use a ToolbarItem just because I think it looks much better, but you could also define a button:

``` xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="TenDaysOfXamarin.Views.ExperiencesPage">
    <ContentPage.ToolbarItems>
        <ToolbarItem Text="New"
                     Clicked="Handle_Clicked"/>
    </ContentPage.ToolbarItems>
    
    <ContentPage.Content>
        <ListView x:Name="experiencesListView">
        </ListView>
    </ContentPage.Content>
</ContentPage>
```

Now, I want you to focus on a couple of things going on with this code:

- The ToolbarItems are defined *outside* of the content for the page. The ToolbarItems won't appear in the designer; they only appear once you run the app, in the form of small buttons at the top right corner of the screen, inside the title bar (more on the title bar in a minute).
- I defined the ListView not in a single line, but with an opening and a closing tag. This is because, between these two tags, I want to add more code that defines the template used to create the cells, but more on that in another minute when we cover Data Binding.
- The ToolbarItem, just like the button, has a Clicked event, for which I have created an event handler:

``` csharp
void Handle_Clicked(object sender, System.EventArgs e)
{

}
```

### Navigation

Inside of that Event Handler, we add the code that navigates our users to the MainPage, which also means that the MainPage will no longer be the MainPage. If this sounds weird is because I am talking about two different MainPages. Remember the App constructors? These constructors are setting the MainPage property to a new MainPage object. This defines the MainPage class as the first page that must be displayed when the app launches. However, we should now first display the ExperiencesPage, so the constructors should now look like this:

``` csharp
public App(string databasePath)
{
    InitializeComponent();

    MainPage = new ExperiencesPage(); // added using TenDaysOfXamarin.Views;

    DatabasePath = databasePath;
}

public App()
{
    InitializeComponent();

    MainPage = new ExperiencesPage();
}
```

This means that, when we launch the app now, it is the new ExperiencesPage the one to be displayed:

Wait, where is our button? We can see the ListView. I mean, it is empty right now, but we know it's there because of all those lines, but there is no ToolbarItem anywhere.

The ToolbarItem is not there because, as I said, ToolbarItems exist in the title bar, and there is no title bar in our view.

No, we don't need to define another element. The NavigationPage handles the Title bar. Once we set a NavigationPage as the MainPage (over in the App constructors), it handles the title bars, the back buttons, and the navigation between pages.

So yes, we need to change the MainPage again, but only slightly. The ExperiencesPage still needs to be the first Page to be displayed, but we also need all of the Pages to exist inside the NavigationPage, so here is what we need: we need the NavigationPage to display the ExperiencesPage first, but to also handle the navigation to and from the MainPage. We also need the NavigationPage to display the title bar, so our ToolbarItem is shown, and to display back buttons when appropriate. All of this can be accomplished with a small change to the App constructors:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day5-001.png?w=512&h=1108)

``` csharp
public App(string databasePath)
{
    InitializeComponent();

    DatabasePath = databasePath;

    MainPage = new NavigationPage(new ExperiencesPage()); // added using TenDaysOfXamarin.Views;
}

public App()
{
    InitializeComponent();

    MainPage = new NavigationPage(new ExperiencesPage());
}
```

The NavigationPage has a constructor that receives an instance of the Page that acts as the root of the navigation. So we can now add the code that navigates users over to the MainPage. The event handler for the click of the Toolbar item should have this new line:

``` csharp
Navigation.PushAsync(new MainPage());
```

It will all work seamlessly now. Run your app and you will see the title bar, the navigation to the MainPage happening, and the back button being added. All by simply creating a NavigationPage:

| Image 1        | Image 2           |
| ------------- |:-------------:|
| ![](https://10daysofxamarin.files.wordpress.com/2019/03/day5-002.png)      | ![](https://10daysofxamarin.files.wordpress.com/2019/03/day5-003.png) |

## Reading the Table

Reading a table resembles inserting quite a bit:

1. We need to create a connection to the database
2. Make sure that the table exists
3. Read the table

Take a look at the code needed for us to read the Experience table -this method, by the way, is called from the ExperiencesPage's OnAppearing method, which is called every time this page is displayed-:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day5-override.gif)

``` csharp
protected override void OnAppearing()
{
    base.OnAppearing();

    ReadExperiences();
}

private void ReadExperiences()
{
    // added using SQLite;
    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
    {
        conn.CreateTable<Experience>(); // added using TenDaysOfXamarin.Model;
        List<Experience> experiences = conn.Table<Experience>().ToList();
    }
}
```

> The OnAppearing method is called every time a page is appearing -aka, navigated to-. The reason I call this method from that overridden method is that, by doing so, we can re-read the table when we navigate back from the MainPage (where we may have added new experiences). This way, ExperiencesPage's list view is always up to date.

The Table method returns a query to the specified table. Then, the ToList method uses that query to get a List of the items that the query returns. By the end of the using statement, the experiences variable already holds all the items in the Experience table.

To check that we have correctly read the table, set a breakpoint at the end of the using statement and run the app.

[https://www.youtube.com/watch?v=-5_tiSi2pxw](https://www.youtube.com/watch?v=-5_tiSi2pxw)

If we inspect the experiences variable, it now contains all of the items that we have inserted into the Experience table:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day5-004.png)

## Listing the Experiences

Now that we have the experiences inside this variable, we can use it to set the items source for the ListView:

``` csharp
private void ReadExperiences()
{
    // added using SQLite;
    using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
    {
        conn.CreateTable<Experience>(); // added using TenDaysOfXamarin.Model;
        List<Experience> experiences = conn.Table<Experience>().ToList();
        experiencesListView.ItemsSource = experiences;
    }
}
```

That line is enough to tell our ListView that those items are the ones it is supposed to display. It is not, however, enough to let the ListView know how to display them:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day5-005.png?w=532&h=1152)

Since we have not defined the template to be used when displaying the experiences, the ListView defaults to using the ToString method that exists in all classes. This method, by default, only returns the type of the item -along with the namespace- as a string.

### Overriding the ToString method

The first thing we can do to solve this is to change that the ToString method does by default. We could, for example, make the ToString method for the Experience class to return the title of the experience instead of its default behavior. To try this out, open the Experience.cs file and override the ToString method inside the Experience class:

``` csharp
public override string ToString()
{
    return Title;
}
```

The result is acceptable:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day5-006.png?w=526&h=1138)

The ListView still uses the ToString method, but now that method returns something readable. We are still limited to that single string though, so most of the times you need to define a template, rather than merely overriding this method:

### Defining a template

Instead of relying on this string, we can define a template that the ListView uses when displaying the experiences. There exist a few pre-defined cells that we can use as the Templates. I use TextCell, which is ideal for this scenario. The TextCell already contains a couple of labels that we can use to display the properties that we need. The template for this ListView then contains a TextCell:

``` xml
<ListView x:Name="experiencesListView">
    <ListView.ItemTemplate>
        <DataTemplate>
            <TextCell/>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

### Data Binding

Now that there is a template, the ListView will no longer use the ToString method to display the experience elements. The ListView now knows that each experience object uses a TextCell to be displayed. So far, however, the TextCell is not displaying any of the properties from the experience objects. Each cell has its elements, but since we know the TextCell contains two labels, we can choose a couple of the Experience's properties and display them like this:

``` xml
<TextCell Text="{Binding Title}"
          Detail="{Binding UpdatedAt}"/>
```          

The Text displays the Title property, and the Detail -which is a second, smaller, colored label- displays the UpdatedAt property. Of course, you could choose to display other properties.

Notice the syntax, with the curly brackets and the Binding keyword. This is how we define that the value being assigned is not a static one -like the one we have set for placeholders or the text of a button-, but one that comes from a property. We have seen a similar syntax when working with styles and resources, and we will continue to use this syntax as we progress through the rest of the challenges.

![](https://10daysofxamarin.files.wordpress.com/2019/03/day5-007.png?w=568&h=1228)

## Navigation back

Finally, let's navigate back to the ExperiencePage, from the MainPage, when the cancel button is clicked. A single line of code is enough, so now the event handler for the click of our cancel button looks like this:

``` csharp
void ContentEntry_Clicked(object sender, System.EventArgs e)
{
    Navigation.PopAsync();
}


As simple as that. Of course, you notice, the back button acts in this same way, so perhaps the cancel button is not necessary, but now you know how to navigate back programmatically.

[LaloCo/10DaysOfXamarin](https://github.com/LaloCo/10DaysOfXamarin/tree/Day5/final)
