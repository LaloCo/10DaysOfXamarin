# 10DaysOfXamarin

## Days

1. *Greetings - XAML and Events*. Users must define a Label, an Entry and a Button. After writing something on the entry, they could click the button and see the Label change to a greeting with whatever they wrote on the Entry (eg. Their name). *Bonus*: display an alert if there is no text in the entry. 

    [Day 1](https://www.notion.so/1992a315899c40d3ae6966b45dfe86ed)

2. *Adding experiences - XAML Styles*. Users must define an array of entries and buttons (save, cancel) that will allow users to “add” new experiences like diary entries (title and content will suffice). Also, users will be challenged to use XAML Styles to reuse some styling across Entries and Buttons. The save button should be disabled if an entry inside the form is empty.

    [Day 2](https://www.notion.so/913c0a1be30b41729de95b4d14420121)

3. *Platform specific code - SQLite Database*. Users will import the SQLite-net-PCL package into the projects and add the platform-specific code that will create the sqlite database in different locations for iOS and Android.

    [Day 3](https://www.notion.so/b8984192928f4e9b84bcee3a1c678885)

4. *Inserting Experiences - Inserting into a table*. Users will create a class that will define an experience, and define the class with a couple of SQLite attributes, for then connect to the database and insert into it.

    [Day 4](https://www.notion.so/92dee893956f467bbd6feb753b1e4766)

5. *Listing Experiences - Reading from the table and using ListViews*. Users will read from the SQLite table and learn to list the elements inside a ListView using simple ListView.ItemsSource through C# (no MVVM binding yet)
6. *Getting the location of the device - GPS*. Users must setup their projects to be able to access the location of the device and get the coordinates in each platform using shared code.
7. *Getting Nearby Venues - REST APIs*. By using the free version of the Foursquare Places API, and the coordinates obtained in the previous challenge, users will be able to get nearby places and list them using a ListView. Updates to the experience model are in order, to fit the new data (selected venue).
8. *MVVM Part 1 - INotifyPropertyChanged*. Using the INotifyPropertyChanged, users will clean the code that is being used to save experiences. Instead of manually assigning the value to the properties of a new Experience, this will happen through binding with a new VM class.
9. *MVVM Part 2 - ICommand*. Using the ICommand interface, users will now clean up event handlers and form evaluations (for button enabling/disabling).
10. *MVVM Part 3 - ObservableCollection<T>*. Finally, some more binding is going to happen so the source of the ListViews is easily updated.
