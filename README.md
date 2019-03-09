Yesterday you prepared the interface that allows users to write new experiences and "save" them or cancel the operation. What's more, you already imported the NuGet package that helps us code the function that creates an SQLite database and performs queries to it. Heck, you now even have the path to that database over in your .NET Standard library, it is coming from either the Android or the iOS projects.

# Moving on

Now that you have your app ready with the database file location, the NuGet package imported and the interface ready, actually inserting the elements into a new Table is quite straight forward. Today you will create a class that represents the table, and learn how to insert new items into it, with the information that the users have written in the Entry and the Editor.

[This branch](https://github.com/LaloCo/10DaysOfXamarin/tree/Day4/initial)

# Challenge

Create a class that represents the table that holds the experiences, along with some attributes that help model this table when it is created. These attributes include those limiting how long a title can be and setting the primary key.

After this, you have to create an instance of this class and insert it into the table.

## Problem

Inserting into a database is very easy now that we have the location where it is stored, the package that helps us interact with SQLite through plain C#, and the interface where the user writes the data.

The primary challenge today then, is to create the C# class -or model- that represents the table. This model should also use some SQLite attributes that limit the length of the title, and that set a specific property to be the primary key.

## App description

The class that you create should have five properties, one of which is vital for the app to work, and two of which are quite standard among this kind of tables:

- The Id, which functions as the primary key
- The Title, which holds the title of the experiences
- The Content, which contains the content that users write
- The CreatedAt, which has the date at which the experience is inserted
- The UpdatedAt, which also has a date but changes every time the experience is updated. This property is optional since the scope of these challenges don't cover updating or deleting.

## Constraints

The title of the experiences that are inserted has to be less than 50 characters. The enforcement of this constraint should be done through SQLite attributes for the table only.

The Id should be an integer that is auto-incremented with every element that is inserted.

# Solution

Let's start by creating the model, or that C# class that represents the table. It is a good practice to have this type of classes inside of its folder, so I first create a new "Model" folder inside of the .NET Standard Library project, and inside it, I create a new class: the Experience class.

[https://www.youtube.com/watch?v=J8sg5gDnVes](https://www.youtube.com/watch?v=J8sg5gDnVes)

[https://www.youtube.com/watch?v=tVoJnsiIDoQ](https://www.youtube.com/watch?v=tVoJnsiIDoQ)

## The Model

In this file, I define the five properties that we need, along with a couple of attributes that define the Id as the primary key and as a value that auto-increments, and the attribute that limits the length of the title:

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
}
```

Notice the syntax for defining these attributes, and the fact that to be able to use them, you need to add a using directive to the SQLite namespace.

This is the C# class that models the table that we will create. Each property becomes a column, the name of the class is used as the name of the table itself, and those attributes are crucial when modeling this table inside the database.

## Inserting into the database

Now that we have that out of the way, all there is left for us to do is to create an instance of this Experience whenever required and insert it.

Of course, we need to create this every time the save button is pressed. So, over in the handler for the click of the button, let's create a new Experience with the values that the user has written down on the Entry and the Editor. You should have come up with something like this:

``` csharp
// added using TenDaysOfXamarin.Model;
Experience newExperience = new Experience()
{
    Title = titleEntry.Text,
    Content = contentEditor.Text,
    CreatedAt = DateTime.Now,
    UpdatedAt = DateTime.Now
};
```

Now, inserting it is quite simple, but I want you to understand something before we go ahead and add that code:

There can only be one connection to the SQLite database at a time. So every time you create a new connection, you should remember to close it. This may sound like an easy thing to do, but it is not the difficulty I'm worried about, it is our uncanny ability to forget things.

So instead of trusting our memory to remember to close every connection we offer, I show you how to create a connection so that it only exists inside a small context, and then it is closed as soon as the execution leaves that context:

### Using statements

No, these are not using directives, which are the ones that reference an external namespace for you to use their classes and other types inside a file.

Using statements define a variable that is disposed of as soon as the execution leaves its body -the using statement's body I mean-. Disposing of this variable, in the case of the connection to an SQLite database, also means that the connection is closed so that new ones could be created.

Creating a connection to the SQLite database, creating a table so that we know it exists, and inserting our newExperience, by using a using statement, will look like this:

``` csharp
// added using SQLite;
using (SQLiteConnection conn = new SQLiteConnection(App.DatabasePath))
{
    conn.CreateTable<Experience>();
    conn.Insert(newExperience);
}
// here the conn has been disposed of, hence closed
```

Hopefully, all these lines are self-explanatory. The only exciting thing is the using statement, which creates a variable of type SQLiteConnection that gets disposed of by the end of the block.

Then, the table is created. Keep in mind here that, if this piece of code is called and the table already exists, then the CreateTable method won't do anything, so no need to worry about this being called multiple times.

Another thing that is important to mention here is that the Insert method returns an integer with the number of elements that were inserted into the table. This means that you could check if your code works -while we learn how to read and list the elements from a table- by getting the value that the Insert method returns and making sure that it is not zero. If you want to improve the user experience by doing so, you may also as well have your event handler display an error and only cleaning the values in the Entry and Editor once the item has been successfully inserted:

``` csharp
void SaveButton_Clicked(object sender, System.EventArgs e)
{
    // added using TenDaysOfXamarin.Model;
    Experience newExperience = new Experience()
    {
        Title = titleEntry.Text,
        Content = contentEditor.Text,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now
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
        titleEntry.Text = string.Empty;
        contentEditor.Text = string.Empty;
    }
    else
    {
        DisplayAlert("Error", "There was an error inserting the Experience, please try again", "Ok");
    }
}
```

There it is, challenge complete. Tomorrow we will learn to read from the table and list the experiences in a new Page.

[Get the final source code](https://github.com/LaloCo/10DaysOfXamarin/tree/Day4/final)
