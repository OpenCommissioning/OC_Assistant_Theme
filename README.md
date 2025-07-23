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
4. Replace the default `Window` class by the `theme:Window` class in your `MainWindow.xaml`.
Content inside the TitleBar can als be rendered by setting `theme:Window.TitleBarContent` property.
```xml
<theme:Window x:Class="MyApplication.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:theme="http://schemas.open-commissioning-assistant.com/2024"
        mc:Ignorable="d"
		ShowIcon="True"
		ShowTitle="False">
    <theme:Windows.TitleBarContent>
        <!-- Here goes your titleBar content. Usually a Menu with MenuItems. -->
    </theme:Windows.TitleBarContent>
    <Grid>
        <!-- Here goes your main application content. -->
    </Grid>
</theme:Window>
```

> [!NOTE]
> Further details about all implemented styles and controls can be found in the [GitHub Repository](https://github.com/OpenCommissioning/OC_Assistant_Theme) of this package.