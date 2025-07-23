using System.Windows;
using System.Windows.Media;

namespace OC.Assistant.Theme;

/// <summary>
/// Provides additional dependency properties.
/// </summary>
public static class Properties
{
    /// <summary>
    /// Identifies the CornerRadius dependency property.
    /// </summary>
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.RegisterAttached(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(Properties),
            null);

    /// <summary>
    /// Gets the radius for the corners of the element's border.
    /// </summary>
    /// <param name="element">The element from which to read the property value.</param>
    /// <returns>
    /// The degree to which the corners are rounded, expressed as values of the CornerRadius
    /// structure.
    /// </returns>
    public static CornerRadius GetCornerRadius(DependencyObject element)
    {
        return (CornerRadius)element.GetValue(CornerRadiusProperty);
    }

    /// <summary>
    /// Sets the radius for the corners of the element's border.
    /// </summary>
    /// <param name="element">The element on which to set the attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetCornerRadius(DependencyObject element, CornerRadius value)
    {
        element.SetValue(CornerRadiusProperty, value);
    }

    /// <summary>
    /// Identifies the Header dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.RegisterAttached(
            "Header",
            typeof(object),
            typeof(Properties),
            new FrameworkPropertyMetadata(OnHeaderChanged));

    /// <summary>
    /// Gets the content for the element's header.
    /// </summary>
    /// <param name="element">The element from which to read the property value.</param>
    /// <returns>The content of the element's header. The default is **null**.</returns>
    public static object? GetHeader(DependencyObject element)
    {
        return element.GetValue(HeaderProperty);
    }

    /// <summary>
    /// Sets the content for the element's header.
    /// </summary>
    /// <param name="element">The element on which to set the attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetHeader(DependencyObject element, object value)
    {
        element.SetValue(HeaderProperty, value);
    }

    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UpdateHeaderVisibility(d);
    }

    /// <summary>
    /// Identifies the HeaderTemplate dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.RegisterAttached(
            "HeaderTemplate",
            typeof(DataTemplate),
            typeof(Properties),
            new FrameworkPropertyMetadata(OnHeaderTemplateChanged));

    /// <summary>
    /// Gets the DataTemplate used to display the content of the element's header.
    /// </summary>
    /// <param name="element">The element from which to read the property value.</param>
    /// <returns>
    /// The template that specifies the visualization of the header object. The default
    /// is **null**.
    /// </returns>
    public static DataTemplate? GetHeaderTemplate(DependencyObject element)
    {
        return (DataTemplate)element.GetValue(HeaderTemplateProperty);
    }

    /// <summary>
    /// Sets the DataTemplate used to display the content of the element's header.
    /// </summary>
    /// <param name="element">The element on which to set the attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetHeaderTemplate(DependencyObject element, DataTemplate value)
    {
        element.SetValue(HeaderTemplateProperty, value);
    }

    private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UpdateHeaderVisibility(d);
    }

    private static readonly DependencyPropertyKey HeaderVisibilityPropertyKey =
        DependencyProperty.RegisterAttachedReadOnly(
            "HeaderVisibility",
            typeof(Visibility),
            typeof(Properties),
            new FrameworkPropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Identifies the HeaderVisibility read-only dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderVisibilityProperty =
        HeaderVisibilityPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the visibility of the header for the specified element.
    /// </summary>
    /// <param name="element">The element from which to read the header visibility property.</param>
    /// <returns>
    /// A <see cref="Visibility"/> value that indicates the visibility state of the header.
    /// </returns>
    public static Visibility GetHeaderVisibility(DependencyObject element)
    {
        return (Visibility)element.GetValue(HeaderVisibilityProperty);
    }

    private static void SetHeaderVisibility(DependencyObject element, Visibility value)
    {
        element.SetValue(HeaderVisibilityPropertyKey, value);
    }

    private static void UpdateHeaderVisibility(DependencyObject element)
    {
        Visibility visibility;

        if (GetHeaderTemplate(element) != null)
        {
            visibility = Visibility.Visible;
        }
        else
        {
            visibility = string.IsNullOrEmpty(GetHeader(element) as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        SetHeaderVisibility(element, visibility);
    }
    
    /// <summary>
    /// Identifies the PlaceholderText dependency property.
    /// </summary>
    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.RegisterAttached(
            "PlaceholderText",
            typeof(string),
            typeof(Properties),
            new FrameworkPropertyMetadata(string.Empty, null));

    /// <summary>
    /// Gets the text displayed in the element until the value is changed
    /// by a user action or some other operation.
    /// </summary>
    /// <param name="element">The element from which to read the property value.</param>
    /// <returns>
    /// The text that is displayed in the element when no value is entered. The default
    /// is an empty string ("").
    /// </returns>
    public static string GetPlaceholderText(DependencyObject element)
    {
        return (string)element.GetValue(PlaceholderTextProperty);
    }

    /// <summary>
    /// Sets the text displayed in the element until the value is changed
    /// by a user action or some other operation.
    /// </summary>
    /// <param name="element">The element on which to set the attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetPlaceholderText(DependencyObject element, string value)
    {
        element.SetValue(PlaceholderTextProperty, value);
    }
    
    /// <summary>
    /// Identifies the PlaceholderForeground dependency property.
    /// </summary>
    public static readonly DependencyProperty PlaceholderForegroundProperty =
        DependencyProperty.RegisterAttached(
            "PlaceholderForeground",
            typeof(Brush),
            typeof(Properties),
            null);

    /// <summary>
    /// Gets a brush that describes the color of placeholder text.
    /// </summary>
    /// <param name="element">The element from which to read the property value.</param>
    /// <returns>The brush that describes the color of placeholder text.</returns>
    public static Brush GetPlaceholderForeground(DependencyObject element)
    {
        return (Brush)element.GetValue(PlaceholderForegroundProperty);
    }

    /// <summary>
    /// Sets a brush that describes the color of placeholder text.
    /// </summary>
    /// <param name="element">The element on which to set the attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetPlaceholderForeground(DependencyObject element, Brush value)
    {
        element.SetValue(PlaceholderForegroundProperty, value);
    }
    
    /// <summary>
    /// Identifies the Description dependency property.
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.RegisterAttached(
            "Description",
            typeof(object),
            typeof(Properties),
            new FrameworkPropertyMetadata(OnDescriptionChanged));

    /// <summary>
    /// Gets content shown below the element. The content should provide
    /// guidance about the input expected by the element.
    /// </summary>
    /// <param name="element">The element from which to read the property value.</param>
    /// <returns>The content to be displayed below the element. The default is **null**.</returns>
    public static object? GetDescription(DependencyObject element)
    {
        return element.GetValue(DescriptionProperty);
    }

    /// <summary>
    /// Sets content shown below the element. The content should provide
    /// guidance about the input expected by the element.
    /// </summary>
    /// <param name="element">The element on which to set the attached property.</param>
    /// <param name="value">The property value to set.</param>
    public static void SetDescription(DependencyObject element, object value)
    {
        element.SetValue(DescriptionProperty, value);
    }

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UpdateDescriptionVisibility(d);
    }

    /// <summary>
    /// Identifies the key for the DescriptionVisibility dependency property.
    /// </summary>
    public static readonly DependencyProperty DescriptionVisibilityProperty =
        DependencyProperty.RegisterAttached(
            "DescriptionVisibility",
            typeof(Visibility),
            typeof(Properties),
            new FrameworkPropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Gets the visibility state of the description associated with a specific element.
    /// </summary>
    /// <param name="element">The DependencyObject from which to retrieve the description visibility value.</param>
    /// <returns>
    /// A Visibility enumeration value that indicates whether the description is visible, collapsed, or hidden.
    /// </returns>
    public static Visibility GetDescriptionVisibility(DependencyObject element)
    {
        return (Visibility)element.GetValue(DescriptionVisibilityProperty);
    }

    /// <summary>
    /// Sets the visibility state of the description for the specified element.
    /// </summary>
    /// <param name="element">The element on which to set the visibility state.</param>
    /// <param name="value">The visibility state to apply to the description.</param>
    private static void SetDescriptionVisibility(DependencyObject element, Visibility value)
    {
        element.SetValue(DescriptionVisibilityProperty, value);
    }

    /// <summary>
    /// Updates the visibility of the description element based on the presence of its associated header.
    /// </summary>
    /// <param name="element">The element whose description visibility needs to be updated.</param>
    private static void UpdateDescriptionVisibility(DependencyObject element)
    {
        SetDescriptionVisibility(element, string.IsNullOrEmpty(GetHeader(element) as string) ? Visibility.Collapsed : Visibility.Visible);
    }

    /// <summary>
    /// Identifies the AdditionalContent dependency property.
    /// </summary>
    public static readonly DependencyProperty AdditionalContentProperty =
        DependencyProperty.RegisterAttached(
            "AdditionalContent", 
            typeof(object), 
            typeof(Properties),
            new PropertyMetadata(null));

    /// <summary>
    /// Sets the value of the AdditionalContent attached property for a specified element.
    /// </summary>
    /// <param name="element">The dependency object on which to set the property value.</param>
    /// <param name="value">The value to set for the AdditionalContent property.</param>
    public static void SetAdditionalContent(DependencyObject element, object value)
    {
        element.SetValue(AdditionalContentProperty, value);
    }

    /// <summary>
    /// Gets the additional content associated with the specified element.
    /// </summary>
    /// <param name="element">The element from which to read the additional content.</param>
    /// <returns>
    /// The additional content associated with the element, or null if no value is set.
    /// </returns>
    public static object GetAdditionalContent(DependencyObject element)
    {
        return element.GetValue(AdditionalContentProperty);
    }
}