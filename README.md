We have started to implement the MVVM architectural pattern, which means that our code behind (that which is inside of the C# files for our Pages) is now cleaner and that we have separated the logic from the view.

Today we are going to use yet another interface that helps us implement the MVVM pattern; this time, it is the ICommand's turn.

# Moving on

The ICommand interface implements members that can help us replace the event handlers that we currently have for things like button and toolbar item clicks. Indeed we have already used the INotifyPropertyChanged interface to replace some of the event handlers that we had, but those events depended on a property change. Click events, on the other hand, don't change any properties, so handling them can only be done through the event itself, not checking for property changes. Enter the ICommand.

[This branch](https://github.com/LaloCo/10DaysOfXamarin/tree/Day9/initial)

# Challenge

Today you identify three remaining event handlers in our code and substitute them for commands. The event handlers are for the click of two buttons and one toolbar item, your challenge today is to add one Command for each of these events to the corresponding view model and bind that command to the appropriate element.

## Problem

Currently, handling the click of the buttons in our interface is the job of the view, which is not entirely wrong, but the functionality is more of the view model's job. So today you create commands that respond to the click of those buttons and execute a particular method when that happens.

You have to create, in the appropriate view model, one ICommand property for each of the buttons and toolbar items that require them. This ICommand property must then point to a method that executes when users click the bound button (or toolbar item).

Once the ICommand properties are ready -just like binding any other property- you bind them to the Command property of the corresponding button (or toolbar item).

Optionally, replace the CanSave functionality that enables and disables the save button with the CanExecute member of the ICommand interface.

## App description

The MainVM should now contain two new properties, each bound to the Command property of the respective buttons:

- The SaveCommand
- The CancelCommand

Also, the ExperiencesVM should now contain one new property, bound to the toolbar item:

- The NewExperienceCommand

## Constraints

Don't remove the SearchEntry_TextChanged event handler just yet. This method will be removed tomorrow with the help of the Query property that we created yesterday. We still need to learn how to handle lists in the MVVM pattern, so ignore this event handler for now.

# Solution

Let me start by replacing the event handler for the click of the toolbar item that we have in the Experiences page. Our primary goal here is to move the logic that we currently have in the code behind over to the view model. The logic for this particular event handler is not that complex, it barely navigates us to the other page, but it is a great way to show you how the command works.

## The long version

Let me create this first command using the extended version. Creating this first command like this helps you understand everything that is happening with that command. Once you understand that, we can create a second command in the short version, which is the more popular, but may be harder to understand at first, especially if you haven't done the extended version first.

### Create the command

Start by creating a new folder inside of the ViewModels folder. This new folder holds only one command since we create the other two using the short way, but if you do choose to create commands inside of their classes -which we do with this command- it is a good idea for them to have their folder.

Name the new folder "Commands" and create a new NewExperienceCommand class inside of it.

This new class needs to implement the ICommand interface. Notice that implementing the interface means adding three members to the class. Let's inspect each of them:

- The CanExecute method: This returns a boolean value defining whether the command can be executed or not. The exciting part is that buttons and other elements react to changes in this value. If the CanExecute returns false, the button is disabled until it suddenly returns true! Hopefully, this gives you a hint about how to replace a specific functionality that we have in the MainPage.
- The CanExecuteChanged event. You may have guessed it; when the CanExecute returns a different value than before, it fires this method. Check the description for the parameters, bellow to learn more about how we could influence the CanExecute.
- The Execute method. This method holds the code that gets called whenever the command is executed. You usually call a method that is inside of the view model class.

The parameter is not precisely a member, but notice that both methods receive a parameter object. Buttons (and other elements), in addition to a Command, also have a CommandParameter property that we can bind to whatever we want. If that CommandParameter is binding to something, both of these methods receive that parameter. If the CommandParameter happens to be binding to a property that calls the PropertyChanged event from the INotifyPropertyChanged interface, the CanExecute re-evaluates the value that it should return.

### Coding the functionality

For this first command, we don't need to worry too much about the CanExecute. This command should always be available for execution, so the method always returns true:

``` csharp
public bool CanExecute(object parameter)
{
    return true;
}
```

The Execute method, as I mentioned before, should call a method from the view model. This call is there for the view model to hold all the logic, and so the command makes use of it. So I define the NewExperience method in the ExperiencesVM class:

``` csharp
public void NewExperience()
{
    App.Current.MainPage.Navigation.PushAsync(new MainPage());
}
```

This code is the same functionality that currently exists inside the event handler for the click of the toolbar item, except in this case we are not inside of a class that inherits from the Page class, so there is no Navigation property available. However, remember that the App class has a MainPage, which does have a Navigation property, so I use that instead.

The Execute method should call that NewExperience method, which means that I need an instance of the ExperiencesVM. I solve it like this:

``` csharp
public class NewExperienceCommand : ICommand
{
    private ExperiencesVM viewModel;
    public NewExperienceCommand(ExperiencesVM viewModel)
    {
        this.viewModel = viewModel;
    }

    ...

    public void Execute(object parameter)
    {
        viewModel.NewExperience();
    }
}
```

The constructor for the command now requests that instance that we need and saves it in a global variable that I can then use from the Execute. Now, every time the command is executed, the NewExperience method is executed as well.

### A Command property in the View Model

The NewExperienceCommand class is ready; it is now time to create a property in the View Model that uses this type:

``` csharp
// added using TenDaysOfXamarin.ViewModels.Commands;
public NewExperienceCommand NewExperienceCommand { get; set; }
```

Just like any of the properties that we defined yesterday in the other view model, we can now bind this one to an element over in the XAML Page, but first, we need to initialize it from the view model's constructor:

``` csharp
public ExperiencesVM()
{
    NewExperienceCommand = new NewExperienceCommand(this);
}
```

Indeed, because the NewExperienceCommand's constructor requests an instance of an ExperiencesVM, we can pass this as the value.

### Binding the Command in XAML

Yesterday we created an instance of the MainVM inside of the MainPage C# file, and set it to be the BindingContext for the entire page.

Today I do that same thing but from the XAML file.

For instance, instead of defining a C# object, I define a XAML resource that is of type ExperiencesVM:

``` xml
<ContentPage ...
             xmlns:vm="clr-namespace:TenDaysOfXamarin.ViewModels">
    <ContentPage.Resources>
        <ResourceDictionary>
            <vm:ExperiencesVM x:Key="vm"/>
        </ResourceDictionary>
    </ContentPage.Resources>
    
    ...
</ContentPage>
```

Notice that, just like on C# I need using directives, here I also needed to point to a specific namespace using namespace mapping. I define this namespace through "vm."

Now that we have this "instance" of the ExperiencesVM class, we can set it as the BindingContext for our toolbar item, and bind the command:

``` xml
<ToolbarItem Text="New"
             BindingContext="{StaticResource vm}"
             Command="{Binding NewExperienceCommand}"/>
```

Yes, BindingContext is not unique to the Page; all XAML elements have binding contexts. Setting it to the Page inherits that same binding context to all the elements inside of it, setting it to a StackLayout inherits it to all the stacked elements inside of it, and so on. Right now though, we only need this context in the toolbar item. Hence I only set it to that element instead of the whole page.

Also, notice how I removed the event handler. Our first view is now event handler free!

## The short version

Let's now jump over to the MainPage, where we have a cancel button which functionality resembles a lot what we just did with the toolbar item. It should always be enabled, and it should navigate to a different page.

This time, however, I create the ICommand property using the short version: with no command classes.

Using the short version means that I can now go directly to the view model and define the property like this:

``` csharp
// added using System.Windows.Input;
public ICommand CancelCommand { get; set; }
```

Now the type of this property is ICommand, instead of a class that implements ICommand.

It is when initializing this property from the constructor where the important thing happens:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day9-001.png)

Here I use the Command class and notice that it contains four different constructors. One asking for an execute action (basically the Execute method) without a parameter, another one asking for the same execute action, but with a parameter, some asking for the can execute, and so on.

For this simple command though, that always should be executable, we can use the first overload and create an action for the execute. This action can be a new method, which is exactly what I create:

``` csharp
public MainVM()
{
    // added using Xamarin.Forms;
    CancelCommand = new Command(CancelAction);
}

void CancelAction(object obj)
{
    App.Current.MainPage.Navigation.PopAsync();
}
```

Having the code like this is the short version of having that new method being called from the Execute method when creating the entire class. So just like this, the functionality for this second command is ready.

### Binding the command

Binding the command this time is quicker because we have already established the binding context before, from the C# file. You may remember how we set the entire page's binding context to an instance of the view model, so now all that we need is this:

``` xml
<Button Text="Cancel"
        Command="{Binding CancelCommand}"
        Style="{StaticResource cancelButtonStyle}"/>
```

Notice how I removed the event handler creation from that button, which means that the event handler method can be removed from the C# file as well.

## A command with parameter and can execute

The save button requires a more complex command. The command for this button should change the value that the can execute method returns, based on a parameter. Remember our CanSave property? It turns out that it is an excellent parameter in this case since it already fires an event every time it changes.

Binding that property to that parameter means we can change the value that the can execute method returns based on the CanSave

Start by defining the command in the same way as the last one:

``` csharp
public ICommand SaveCommand { get; set; }
```

This time, however, we use a different constructor from the Command class.

``` csharp
SaveCommand = new Command<bool>(SaveAction, CanExecuteSave);
```

Inside of the angle brackets, we define the type of the parameter that the methods receive. The SaveAction is the name of the method that is executed (just like what we did with the CancelAction), but now we also have a CanExecuteSave method, for which I'm sure you can guess the functionality.

The two methods look like this:

``` csharp
bool CanExecuteSave(bool arg)
{
    return arg;
}

void SaveAction(bool obj)
{
    Experience newExperience = new Experience()
    {
        Title = Title,
        Content = Content,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now,
        VenueName = SelectedVenue.name,
        VenueCategory = SelectedVenue.MainCategory,
        VenueLat = float.Parse(SelectedVenue.location.Coordinates.Split(',')[0]),
        VenueLng = float.Parse(SelectedVenue.location.Coordinates.Split(',')[1])
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
        Title = string.Empty;
        Content = string.Empty;
        SelectedVenue = null;
    }
    else
    {
        App.Current.MainPage.DisplayAlert("Error", "There was an error inserting the Experience, please try again", "Ok");
    }
}
```

Notice that the SaveAction method contains the same code as the event handler that is over in the MainPage C# file, except that the properties now exist locally and not through a "viewModel" variable, and the DisplayAlert method is not available, so I use the MainPage from the App class, just like I do when performing navigation.

Also, notice how both methods receive a boolean argument, the type of this argument is defined when initializing the constructor (you know, the value between the angle brackets).

### Binding command and command parameter

The binding works in the same way; only this time we also need to bind a command parameter. Both the SaveAction and the CanExecuteSave methods receive this command parameter. The SaveAction ignores it completely, but the CanExecuteSave returns that same value.

``` xml
<Button Text="Save"
        Command="{Binding SaveCommand}"
        CommandParameter="{Binding CanSave}"
        Style="{StaticResource mainButtonStyle}"/>
```

Because of this command parameter, we don't need to bind anything to the IsEnabled property anymore. You can also delete the SaveButton_Clicked event handler.

Now, our code behind is much cleaner, thanks to the ICommand interface.

[LaloCo/10DaysOfXamarin](https://github.com/LaloCo/10DaysOfXamarin/tree/Day9/final)
