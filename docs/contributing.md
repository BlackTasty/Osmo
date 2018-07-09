## Contributing

If you want to introduce new user controls and/or add new controls to existing ones, please follow the MVVM pattern and the [Material Design Guidelines](https://material.io/design/).

Also please follow these guidelines:
- Create Viewmodels inside the namespace "ViewModel" and append "ViewModel" at the end of the class name. (For example "SkinSelect**ViewModel**")
- If you add a new section to the sidebar, create a new usercontrol inside the namespace "UI". *Every section must implement the [Singleton](http://csharpindepth.com/Articles/General/Singleton.aspx) pattern*. Then open the class "OsmoViewModel", go to the constructor and add the section as a new SidebarEntry.
  - Example: *new SidebarEntry(Helper.FindString("sidebar_home"), MaterialDesignThemes.Wpf.PackIconKind.Home, SkinSelect.Instance)*
- Feel free to translate Osmo into your language! To do so, follow these steps:
  - Create a copy of the "StringResources.xaml" file inside the namespace "Localisation"
  - Rename your copy like this: "StringResources.[culture code].xaml". Culture codes can be found [here](https://msdn.microsoft.com/en-us/library/hh441729.aspx?f=255&MSPPError=-2147217396). An example for a German translation would be "StringResources.de.xaml".
  - Translate the text between ">" and "</system:String>" in every line. **Don't change the "x:Key" attribute!**
  - Finally open an issue with the title "Translation for [Language name] finished". Example: "Translation for German finished". 
