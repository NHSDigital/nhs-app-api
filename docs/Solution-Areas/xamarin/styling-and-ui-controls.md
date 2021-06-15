# Styling & User Interface Controls

[[_TOC_]]

## Styling
There are a few ways to apply styling on elements. Here are some general rules we'd like to follow.

1. Avoid adding styling properties directly to an element. We prefer styles in a `ResourceDictionary`. 
2. `ResourceDictionary` should go above `VisualStateManager.VisualStateGroups` to avoid "warning squiggles" showing in rider.
3. Vertical spacing can be easily messed up. to keep it simple, try to add padding mainly to the bottom of and element, than evenly to top and bottom. Most text elements have a bottom padding of 16px (24px when large displays). Though Heading2's have a small amount top padding to visually group the content.

## Responsive Elements

We have a pattern to replicate some of the responsive behaviour of the web code in Xamarin. This is done by applying styles to controls with the visual state manager. 

```C#
    <VisualStateManager.VisualStateGroups>
        <VisualStateGroup x:Name="ResponsiveFontStates">
            <VisualState x:Name="SmallDisplay" />
            <VisualState x:Name="LargeDisplay">
                <VisualState.Setters>
                    <Setter TargetName="Label" Property="Style" Value="{StaticResource HeadingWide}" />
                </VisualState.Setters>
            </VisualState>
        </VisualStateGroup>
    </VisualStateManager.VisualStateGroups>
```

The `SmallDisplay` and `LargeDisplay` are toggled thanks to this block in the code-behind. `SetVisualStateBreakpoints()` toggles the VisualStateManager between large and small displays if the devices current width.

```C#
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        ResponsiveStates.SetVisualStateBreakpoints(this);
    }
```

We specify the default (`SmallDisplay`) styles targeting the element by `TargetType`. Then `LargeDisplays` styles override the default styles with the new values/properties. 

```C#
  <Setter TargetName="[`x:Name` of element to apply style too]" Property="Style" Value="{StaticResource [`x:Key` of styles we want to apply]}" />
```

## Types of Controls
To keep things nicely organised, controls are organised into the following folders:

* Elements - Single item control with specific styles and accessibility helpers
* Panels - Common panels or cards used across the app 
* Navigation - Common Header and Footer Navigation elements
* Effects - Control customisations
* Icons - SVGs and related classes
* Styles - NHSUK styles and colours 

## Deprecated elements
The elements in the Deprecated namespace should not be used. Each control have a responsive equivalent. 

