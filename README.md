Yesterday we created this interface that allows our users to write their experiences, similar to what they may do on a diary. So far, however, this is quite useless, since the experiences are not saved anywhere. Our users can write, click on the save button and see the text disappear, but the experience won't be anywhere. Today we fix that.

# Moving on

Today we create a lightweight database that holds all the experiences the user may add. We use SQLite, which exists in a single file and still maintain some relational-database functionality. We, however, won't be using SQL to make the queries, that is done in plain C#, with the help of Linq and a handy third-party package that we can add to our projects.

# Challenge

Add the [SQLite-net-PCL NuGet package](https://github.com/praeclarum/sqlite-net) to all projects and make sure that each platform creates the database file in a specific path. Each platform then should pass the platform-specific file path to the shared .NET Standard library project, for its usage with shared code.

## Problem

While the file path to the database could be shared among platforms, -thanks to a proper Xamarin functionality that translates a path to the appropriate directory depending on the platform- Apple may require the file to be contained inside a folder that does not exist on the Android directory. Other platforms may have similar restrictions.

You should create the file in a different path depending on the platform, and we use this example to learn how to create platform-specific code and share it over to the shared code.

> NOTE: Do not use this technique to pass too much information from the platform-specific projects over to the .NET Standard Library. This example only passes a string, so no big deal here. For more information check the Learn more section at the end of this post.

## App description

The App class has to change a bit. More specifically, you need a new version of its constructor that can receive and store the file path that it may receive from the Android or iOS projects.

Both the Android and the iOS projects should create a distinct file path to the database that holds the user's experiences, and then pass that file path as a string over to the shared library with the help of the second overload of the App's constructor.

## Constraints

The file path for iOS should be different than the one for Android.

# Solution

Continuing where we left off:

[This branch](https://github.com/LaloCo/10DaysOfXamarin/tree/Day3/initial)

So we are going to create some unique file paths from the Android and iOS projects, so we can eventually use that file to create some database tables and hold data inside. First, however, we need to add the third-party package that is used for that, even if we don't necessarily need it yet. This way, for tomorrow, our app is entirely ready to start inserting entries into a new table.

## Referencing NuGet packages

[https://www.youtube.com/watch?v=WGyJ6S3mpK4](https://www.youtube.com/watch?v=WGyJ6S3mpK4)

[https://www.youtube.com/watch?v=gCOnPlflc_c](https://www.youtube.com/watch?v=gCOnPlflc_c)

## The App constructor

So I mention in the challenge that a new version of the App constructor is what you need, but I don't explain why.

To understand this, open the AppDelegate.cs file from the iOS project, and the MainActivity.cs file from the Android project. Notice that both contain this line of code:

``` csharp
LoadApplication(new App());
```

This line is the one that handles the creation of the native application using the data over in the .NET Standard Library -our shared code-. You can think of this App constructor as the entry point to the shared code, and the way everything else is initialized.

Now open the App.xaml.cs file from the shared library. In its constructor it is creating a new MainPage, which is why when the app is created, it is the MainPage the one that is used to create the interface. That constructor is the one that is called from both the AppDelegate and the MainActivity.

### A new App constructor

So let's create a new version of this constructor that now requests a string -the database's file path-. Without deleting the parameterless constructor, add one that receives a string and stores it in a global static variable:

``` csharp
public static string DatabasePath;

public App(string databasePath)
{
    InitializeComponent();

    DatabasePath = databasePath;

    MainPage = new MainPage();
}
```

It is imperative that this new constructor still calls the InitializeComponent method, and that it still sets the MainPage property -which is inherited from the Application class- to a new MainPage.

### Using the new App constructor

So now, from the AppDelegate and the MainActivity, we can use this new constructor.

Let us first create the database file then. In the case of Android, I use the MyDocuments folder and create a db3 file inside it. Once this is ready I pass it over to the App class using its second constructor:

``` csharp
string fileName = "database.db3";
string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
string fullPath = Path.Combine(folderPath, fileName); //Added using System.IO;

LoadApplication(new App(fullPath));
```

There are three things to note here:

- The MyDocuments "special folder" is not necessarily named that way. SpecialFolder enumerates many existing folders and maps them to the correct path depending on the platform. This means that the path to the "SpecialFolder.MyDocuments" folder could be different on Android than it is on iOS and we would still get the correct value.
- An Environment class exists on both the Android.OS and the System namespaces. This is why it is crucial that I explicitly set which one I want to use by writing System.Environment.
- To be able to use the Path class, you need to add a using directive to the [System.IO](http://system.io/) namespace.

Something very similar happens for iOS; the only difference is that we use a different folder:

``` csharp
string fileName = "database.db3";
string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library");
string fullPath = Path.Combine(folderPath, fileName); //Added using System.IO;

LoadApplication(new App(fullPath));
```

Notice that here I am still using the MyDocuments' path, but I use that path only to "exit" the MyDocuments directory and enter another one -the Library folder." For this, I need to use the Path.Combine method.

Also, notice that here I do not need to write System.Environment explicitly, since on iOS there is only one Environment class available -the Android.OS namespace, of course, does not exist in this context-.

With this now each platform passes its file path over to your shared code, for you to later use it as you want. Remember that now the DatabasePath variable over in your App class contains the file that either the Android or the iOS projects have passed from the AppDelegate and MainActivity respectively.

[Get the final source code](https://github.com/LaloCo/10DaysOfXamarin/tree/Day3/final)

# Learn more

Want to learn how to use platform-specific code from the shared library better?

[Dependency Services on Xamarin Forms](https://lalorosas.com/blog/dependency-services)
