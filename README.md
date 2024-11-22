# Open Commissioning Assistant Theme

The official WPF Theme of the [Open Commissioning Assistant](https://github.com/OpenCommissioning/OC_Assistant).

[![NuGet Status](https://img.shields.io/nuget/v/OC.Assistant.Theme.svg)](https://www.nuget.org/packages/OC.Assistant.Theme/)

### How to use:
1. Create a ```WPF Application``` project with target framework ```.NET8``` or ```.NET Framework 4.8.1```
2. Add the ```OC.Assistant.Theme``` package via nuget
3. Add the ResourceDictionary of the theme to your `App.xaml`: 
```xml
<Application x:Class="OC.Assistant.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:theme="http://schemas.open-commissioning-assistant.com/2024"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <theme:ResourceDictionary />
    </Application.Resources>
</Application>
```
4. Set the `UseTheme` property in your `MainWindow.xaml`:
```xml
<Window x:Class="MyApplication.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:theme="http://schemas.open-commissioning-assistant.com/2024"
        mc:Ignorable="d"
        theme:WindowStyle.UseTheme="True">
    <Grid>
    </Grid>
</Window>
```

> [!NOTE]
> Further details about all implemented styles and controls can be found in the [GitHub Repository](https://github.com/OpenCommissioning/OC_Assistant_Theme) of this package.