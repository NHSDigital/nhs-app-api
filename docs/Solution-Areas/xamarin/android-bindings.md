# Xamarin Android Bindings

See: (Binding a Java Library)[https://docs.microsoft.com/en-us/xamarin/android/platform/binding-java-library/)

## Dependencies

A `pom.xml` file is created to pull down the library being bound and its dependencies. Here the dependency tree can be modified to limit/correct the files being fetched. For example:

* To correct the package type on a dependency.
* To remove dependencies coming from a NuGet package.

For some dependencies (e.g. android or google libraries) there are Xamarin packages available on NuGet.org, these should be used in preference to referencing the java libraries directly.

<span style='font-size:1.1em; color: yellow'><span style='font-size:1.5em'>&#9888;</span> A separate binding project must be created for each .AAR file that needs to be referenced. This binding project can then be referenced directly or as a NuGet package from the dependent binding project.</span>

The first phase of building the binding project attempts to extract metadata about all of the types in the libraries into an `api.xml` file. If a dependency is missing then types/methods which expose types in that dependency will not be included in the metadata extract. An error is logged (but not reported) for these types and the build log should be checked for these errors.

```text
Error while processing '[Method] java.time.temporal.Temporal adjustInto(java.time.temporal.Temporal temporal, long newValue)' in '[Class] java.time.temporal.ChronoField': Type 'java.time.temporal.Temporal' was not found.
Error while processing '[Method] java.time.temporal.Temporal addTo(java.time.temporal.Temporal temporal, long amount)' in '[Class] java.time.temporal.ChronoUnit': Type 'java.time.temporal.Temporal' was not found.
Error while processing type '[Class] a.a.a.a.a': Type 'android.support.v7.app.AppCompatActivity' was not found.
Error while processing '[Method] a.a.a.b.a.c a(org.opencv.core.Mat p0, com.paycasso.sdk.api.flow.enums.DocumentShape p1, boolean p2, boolean p3)' in '[Class] a.a.a.b.a.b': Type 'org.opencv.core.Mat' was not found.
```

## Customizing Bindings

See: [Customizing Bindings](https://docs.microsoft.com/en-us/xamarin/android/platform/binding-java-library/customizing-bindings/)

Most customisation is done in the `Transforms/Metadata.xml` file.

A good starting point is to filter out any packages we know we don't want to generate bindings for. e.g.

```xml
  <remove-node path="/api/package[not(starts-with(@name, 'com.paycasso.') or @name='paycasso')]" />
  <remove-node path="/api/package[starts-with(@name, 'com.paycasso.sdk.activity')]" />
  <remove-node path="/api/package[starts-with(@name, 'com.paycasso.sdk.api.core')]" />
  <remove-node path="/api/package[starts-with(@name, 'com.paycasso.sdk.core')]" />
```

Here we remove any packages which are not paycasso and then some internal packages within the top level paycasso packages.

Then work through the warnings either filtering out (if not required) or correcting the each issue.

### Fields and Get Set methods

Fields and methods matching `setXXX()`/`getXXX()` both get converted to dotnet properties. This often causes a conflicit. All class fields can be removed from the bindings by:

```xml
  <remove-node path="//class[@extends != 'java.lang.Enum']/field" />
```

### Obfuscated Code

Code that has been obsfuscated is not intended to be used by comsumers of the library so can be safely filtered out. e.g.

```xml
  <remove-node path="//class[starts-with(@extends, 'a.')]" />
  <remove-node path="//method[starts-with(@return, 'a.')]" />
  <remove-node path="//method[parameter[starts-with(@type, 'a.')]]" />
```
