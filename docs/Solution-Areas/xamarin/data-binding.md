# Xamarin Data Bindings

## Binding Context

There are many ways to specify the binding context in XAML. We have decided to always specify the source of the binding inline in the binding. This should avoid issues with interited binding contexts and keep the binding information all in one place at the cost of some duplication. For clarity the source should be a reference to a named element.

e.g.

```xml
LaunchPaycassoCommand="{Binding LaunchPaycassoCommand, Source={x:Reference Name=Page}}"
```

### Binding Context In Pages

Every page should have a name of `Page`. This will make it likely that bindings copied from one page to another require minimal changes to work.

```xml
<pages:NhsAppCloseSlimHeaderPage xmlns="http://xamarin.com/schemas/2014/forms"
                                 ...
                                 x:Name="Page">
```

Commands on the page's view clas can then be bound using `Source={x:Reference Name=Page}`.

Reusable page layouts should have a name of `Root` so the names do not clash.

```xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             ...
             x:Name="Root">
```
