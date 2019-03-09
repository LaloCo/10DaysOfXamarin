You now have a better understanding of how Xamarin works. By now you should start to feel comfortable defining elements inside a XAML file, accessing them from the C# code behind, and even creating event handlers.

# Moving on

So let's now use this new knowledge to create the interface of the application that we will build throughout the rest of the challenges. Today you will also use XAML styles, which means you will not only learn to add some styling to your user interface but that you will share these styles easily between elements.

This is of great use in all severe applications, which have to follow some brand guidelines in the form of colors and similar styles for buttons and label hierarchy.

# Challenge

Using the project that we created yesterday, or an entirely new one if you like, you will define an interface that contains two entries and two buttons. This interface will allow the users to write about their experiences, like in a diary.

[LaloCo/10DaysOfXamarin](https://github.com/LaloCo/10DaysOfXamarin/tree/Day2/initial)

## Problem

Create an interface that allows users to enter a title and the content for a new experience. This interface should also display a button for the user to save the experience (the actual functionality won't yet be implemented) and another button to cancel the creation of the new experience.

The save button should be disabled if either the title or the content entries are empty.

The usage of styles is crucial for this challenge since reusing code is the main focus here. This means that the buttons and the entries should look similar between them.

## App description

The app's interface should contain at least four elements:

- An entry for the title of the experience
- An entry or editor for the content of the experience
- A save button
- A cancel button

As soon as the save button is clicked, both entries should be cleaned in anticipation for a new experience to be added. No more functionality is required at the moment.

## Constraints

Remember only to enable the save button when both entries contain some text. It should remain disabled if either entry is empty.

# Solution

The main functionality should be relatively easy to implement, given what you learned yesterday, so if you have that working correctly, feel free to ignore everything after the *Creating the styles* section in this post.

## Defining the interface

The main interface should be something very similar to this:

``` xml
<StackLayout Margin="16,20">
    <Entry x:Name="titleEntry"
           Placeholder="Title of the experience"/>
    <Editor x:Name="contentEditor"
            VerticalOptions="FillAndExpand"
            Placeholder="Write your experience..."/>
    <Button Text="Save"/>
    <Button Text="Cancel"/>
</StackLayout>
```

There is a new element in here: an Editor element. You could have also used an Entry, but the difference is that the Editor allows users to write more than one line in it. Because of the context of this interface, it may be a better option to use the Editor.

Still, this Editor element should have a placeholder and a name. Notice, however, how it is also setting its Vertical Options to FillAndExpand. This means that the Editor will use, vertically, all the available space. This means that the Editor will use the entire height of its container (the StackLayout) minus the height used by the other elements. If I momentarily set a background color to the Editor, you will easily notice what this property does:

| With FillAndExpand        | Without FillAndExpand           |
| ------------- |:-------------:|
| ![](https://10daysofxamarin.files.wordpress.com/2019/03/day2-001.png)      | ![](https://10daysofxamarin.files.wordpress.com/2019/03/day2-002.png) |

## Creating the styles

The interface is still far from perfect. It needs some color, and for it to be more homogeneous. Notice for example that the place holder color for the Entry and the Editor are different. The buttons should also look similar, but not identical since the operations that they perform are different.

If we want to reuse the most code while making the required changes for the interface to be better, we need to use styles, along with some other resources.

### Creating the resources

We'll start by defining some resources over in the App.xaml file. Notice that an Application. Resources tag is already in place. The first resources that I define are the primary colors that all apps should include. Feel free to change the actual colors for the ones you want:

``` xml
<Application.Resources>
    <Color x:Key="mainColor">#326273</Color>
    <Color x:Key="whiteColor">#EEEEEE</Color>
    <Color x:Key="blackColor">#1C1C1C</Color>
    <Color x:Key="accentColor">#4392F1</Color>
    <Color x:Key="destructiveColor">#EF798A</Color>
    <Color x:Key="secondaryColor">#BFBFBF</Color>
</Application.Resources>
```

This colors now can be reused all over the application. The best part is that, if your branding ever changes, you only need to change the value of that resource and that would be applied everywhere in your app. Are you starting to see the benefits of resources and styles?

### Defining the styles

Now that we have these resources let us start creating the styles by defining how our save button should look like. Still, inside this Resources tag in the App.xaml file, I need to define the style that is applied to buttons, and that I can quickly identify by its key as a style that should be applied to all main buttons:

``` xml
<Style TargetType="Button" x:Key="mainButtonStyle">
    <Setter Property="BackgroundColor" Value="{StaticResource accentColor}"/>
    <Setter Property="TextColor" Value="{StaticResource whiteColor}"/>
</Style>
```

Notice three main things with this code:

- The TargetType: This is how we define what elements can use this Style.
- The setters: This is how we establish all of the characteristics our buttons should have. Here I am only setting a background color and a text color, but you could create a setter for every property the Buttons have.
- The Values. Here I am using the color resources that we defined earlier, did you notice? This unique syntax is how you reference a resource by using its key.

Let me mention the benefit of this resource structure again: if the accent color ever changes, I only need to change the value of the accentColor color resource, and that changes the background color of all buttons that use the mainButtonStyle automatically, without me needing to do anything else.

### Using the style

Finally, let's use this style over in our save button. Since we also are referencing a resource, the syntax is familiar:

``` xml
<Button Text="Save"
        Style="{StaticResource mainButtonStyle}"/>
```

### Style "inheritance"

Of course, one style is rarely enough. In our simple interface, we already need a second style for the cancel button, and while we don't have one, maybe also define a style for a delete button.

However, we also need to reuse as much code as possible. In comes the ability to base one Style on another. Let's say then that the buttons for cancel and delete are going to be based on the mainButtonStyle:

``` xml
<Style TargetType="Button" x:Key="cancelButtonStyle" BasedOn="{StaticResource mainButtonStyle}">
    <Setter Property="BackgroundColor" Value="{StaticResource secondaryColor}"/>
</Style>
<Style TargetType="Button" x:Key="deleteButtonStyle" BasedOn="{StaticResource mainButtonStyle}">
    <Setter Property="BackgroundColor" Value="{StaticResource destructiveColor}"/>
</Style>
```

They have now "inherited" all the setters from the mainButtonStyle style, and can, of course, override them and define their own.

The benefit of doing this style "inheritance" is that now if the style of all buttons must change, we only need to change it once in the main style, and the other styles inherit the changes.

Now that we have this new setters, our cancel button should look like this:

``` xml
<Button Text="Cancel"
        Style="{StaticResource cancelButtonStyle}"/>
```

It has inherited the TextColor setter so that the interface so far looks like this:

Now that you know how to define the styles, their setters, and how to use them as the styles of a particular element, creating a couple of other styles should be straight forward.

Let's then create one Style for the Entry, so that it uses the colors that we defined. Also, let's do the same for the Editor, which so far is using a different color for the place holder.

Of course, feel free to use even more setters, so you customize the interface to match your taste.

Here is what my styles look like:

![](https://10daysofxamarin.files.wordpress.com/2019/03/day2-003-1.png?w=534&h=940)

``` xml
<Style TargetType="Entry" x:Key="mainEntryStyle">
    <Setter Property="PlaceholderColor" Value="{StaticResource secondaryColor}"/>
    <Setter Property="TextColor" Value="{StaticResource blackColor}"/>
</Style>
<Style TargetType="Editor" x:Key="mainEditorStyle">
    <Setter Property="PlaceholderColor" Value="{StaticResource secondaryColor}"/>
    <Setter Property="TextColor" Value="{StaticResource blackColor}"/>
</Style>
```

Since both are using the same colors, once I apply these styles to the elements over on my MainPage, the entry and the editor now look more alike, and match the branding of my app better: 

![](https://10daysofxamarin.files.wordpress.com/2019/03/day2-004.png?w=251&h=438)

## Adding the functionality

We need a few event handlers to solve the challenge. Remember that from the beginning the save button should be disabled. Disabling a button is easy:

``` xml
<Button Text="Save"
        IsEnabled="false"
        Style="{StaticResource mainButtonStyle}"/>
```

This prevents the button from being clicked (and also impacts the way it looks), but what we need to do is enable it once both the entry and the editor have some text in them. To know this, we need to handle the event that fires whenever the text inside these elements changes. Creating the event handler for these new events is done in precisely the same way as creating the one for the click of the button, as we saw yesterday so that I focus on the functionality:

First, since this time we need to set the value of a property for our button, we do need to set a name to it (I called it saveButton).

Second, from both event handlers (the one that handles the text change for the entry and the one that handles it for the editor) the same thing happens, so I define a method that is called from both event handlers:

``` csharp
private void CheckIfShouldBeEnabled()
{
    saveButton.IsEnabled = false;
    if (!string.IsNullOrWhiteSpace(titleEntry.Text) && !string.IsNullOrWhiteSpace(contentEditor.Text))
        saveButton.IsEnabled = true;
}
```

Hopefully, the functionality of this method is quite clear. I am by default setting the IsEnabled property of the button to false (just like I did from the XAML definition of the button), and only if both the entry and the editor **are not** empty, do I set it to true.

Finally, all there is left to do is to call this method every time the text in the entry or the editor changes:

``` csharp
void TitleEntry_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
{
    CheckIfShouldBeEnabled();
}

void ContentEditor_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
{
    CheckIfShouldBeEnabled();
}
```

Also, don't forget to clean the text in both the entry and the editor when the save button is clicked. So one last event handler is required here. I will also already create the event handler for the cancel button, even if it's not required yet:

``` csharp
void SaveButton_Clicked(object sender, System.EventArgs e)
{
    titleEntry.Text = string.Empty;
    contentEditor.Text = string.Empty;
}

void ContentEntry_Clicked(object sender, System.EventArgs e)
{

}
```

There you go, the challenge is complete.

[Get the final source code](https://github.com/LaloCo/10DaysOfXamarin/tree/Day2/final)

# Learn more

Do you wish the disabled button looked better? Learn about the Visual State Manager:

[Handling Visual States in Xamarin Forms](https://lalorosas.com/blog/visual-state-manager)
