# Create and include a third party framework as a pod in iOS

## Adding the framework
To create the pod, we need to have a repo to reference. Create a new repo using the azure job at https://dev.azure.com/nhsapp/NHS%20App/_build?definitionId=8. Running this jon will ask for a ticket reference and a new repository name. Please use the format of `<third party name>-framework` explicitly marking this as a framework will help others identify the purpose of this repository.

Once this has been created please check the size of your framework you need to add. This is as keeping in mind we have a max size limit on what can be added to repositories. If you need to go above this you will need to ask someone with access that can temporarily remove this limit. Once this is done push up the framework.

Once this has been merged please remember to create a tag to version this framework.

## Creating the pod
In the same place mentioned above, please create a new repo. Use the same name format as before, but append 'cocoapod' on the end to again help distinguish this is a pod for the named framework. Once this has been created open the root of the project and create a new folder which you will use to reference the pod. 

We should try to maintain some sort of versioning when it comes to updating the pods. If the framework you are using has a version, please create a new folder which reflects the current version of your framework. If the framework is un-versioned or we don't know the version, please use the date of adding in the format of YYYY.MM.DD. 

Once the folder is created run the following command to create a podspec file

`pod spec create <name used to reference pod>|<repo base url>/<new framework repo name>`

This will create a new pod spec file. The format of this may vary depending on what framework you are adding but this is a general base spec file that can be updated and used 

```
Pod::Spec.new do |spec|
  spec.name         = "<name of the pod>"
  spec.version      = "<current version in folder structure>"
  spec.summary      = "<description of the pod (i.e framework purpose)>"
  spec.homepage     = "<repo url>"
  spec.author        = "NHS Digital"
  spec.platform      = :ios, "<min ios version supported for framework>"
  spec.license      = { :type => '<lisence type (like Custom or MIT etc>', :text => <<-LICENSE
        
          <license text>
  LICENSE
  }
  spec.vendored_frameworks = "<name of the framework>"
  spec.source        = { :git => "<repo url>", :tag => "#{spec.version}" }
  spec.source_files  = "<framework file name>/**/*.{h,m,swift}"
end
```

## Checking the pod spec
Once you have updated and saved your changes to the pod spec you can then run a pod spec lint which will reveal any errors in your created pod spec. To do so run the following command:

`pod spec lint <file name of pod spec> --verbose`

If this comes back with any warnings please resolve them. If not you can push this up and get it merged.

## Importing into iOS
Once everything is merged you can now update the podfile in the iOS project. start by adding your cocoapod repo as a source reference in the file. Then you can add 

`pod '<pod name>'` 

This is added inside your desired target. Once this has been added run `pod install` which should install your added pod.