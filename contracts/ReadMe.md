Viewing Complete Contract
------------------------
Install if you dont already have it

**npm install --global http-server**

run the http server in the contracts folder

**cd <path/to/yaml/folder>**

**http-server --cors**

Open 
**http://editor2.swagger.io/#!/**

In swagger editor set resolution base path to http server address

**Preferences-->Preferences-->Pointer Resolution Base Path**

e.g. http://172.25.44.161:8080

**Import nhsonline.yaml to view**

Generating Single Contract Document
---------------------------

Swagger-cli outputs json version
**npm install -g swagger-cli**

Converts json contract to yaml
**npm install -g json2yaml**

Outputs contract to index.yaml
**swagger-cli bundle nhsonline.yaml | json2yaml > index.yaml**