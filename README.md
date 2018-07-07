# OSMO - A skin manager for osu!

 OSMO stands for "**O**pen **S**kin **M**anager for **o**su!" and builds upon the old [Skin Manager](https://osu.ppy.sh/community/forums/topics/231747 "Skin Manager") from 2014. It uses the [Material Design In XAML Toolkit](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit "Material Design In XAML Toolkit") to give you a clean and easy to navigate UI.

### Status

[![Gitter](https://img.shields.io/badge/Gitter-Join%20Chat-green.svg?style=flat-square)](https://gitter.im/OsmoChat/Lobby)
![GitHub issues](https://img.shields.io/github/issues/BlackTasty/Osmo.svg?style=flat-square)
![GitHub last commit](https://img.shields.io/github/last-commit/BlackTasty/Osmo.svg?style=flat-square)
![GitHub commits](https://img.shields.io/github/commits-since/BlackTasty/Osmo/0.0.1.50-alpha.svg?style=flat-square)

![GitHub release](https://img.shields.io/github/release/BlackTasty/Osmo.svg?style=flat-square)
![GitHub Release Date](https://img.shields.io/github/release-date/BlackTasty/Osmo.svg?style=flat-square)

![GitHub (pre-)release](https://img.shields.io/github/release/BlackTasty/Osmo/all.svg?style=flat-square&label=pre-release)
![Github Pre-Releases](https://img.shields.io/github/downloads-pre/BlackTasty/Osmo/latest/total.svg?style=flat-square&colorB=f57b40)
![GitHub (Pre-)Release Date](https://img.shields.io/github/release-date-pre/BlackTasty/Osmo.svg?style=flat-square&label=pre-release%20date&colorB=f57b40)

### First stable release ToDo

- [x] Implement a forum template manager (with autofill)
- [x] Add keyboard shortcuts
- [x] Skin element resize tool (for example create SD elements from HD elements)
- [ ] Implement a custom file picker with Material Design style (and get rid of the default Win32 file picker) -> **Currently in development**
- [ ] Simulator -> Test how your skin would look like in-game without starting osu!
- [ ] Auto-Updater
- [ ] Implement custom titlebar and remove MahApps.Metro dependency
- [ ] Translation system
- [ ] Translation files
 - <s>English</s>
 - <s>Spanish</s>
 - German

### Contributing

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

### Screenshots

Home Screen
![Home Screen](https://puu.sh/ASsxu/a53008ee66.jpg "Home Screen")

Skin Wizard
![Skin Wizard](https://puu.sh/Az2YR/8f1d07c045.jpg "Skin Wizard")

Skin Editor
![Skin Editor](https://puu.sh/ASsAP/6ce83fc85a.jpg "Skin Editor")

Skin Mixer
![Skin Mixer](https://puu.sh/ASsBW/a04543558f.jpg "Skin Mixer")

Animation preview 
https://puu.sh/Az35a/171fdbfb7a.mp4

Template Manager
![Template Manager](https://puu.sh/ASsCI/e2bebb7620.jpg "Template Manager")

Template Editor
![Template Editor](https://puu.sh/ASsFe/be19d4ae3b.jpg "Template Editor")

Resize Tool
![Resize Tool](https://puu.sh/ASsFP/fcce8783ef.jpg "Resize Tool")
