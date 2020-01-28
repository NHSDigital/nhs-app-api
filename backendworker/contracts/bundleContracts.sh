npm install -g swagger-cli
npm install -g json2yaml

swagger-cli bundle --dereference nhsonline.yaml | json2yaml > ../../web/contracts/index.yaml