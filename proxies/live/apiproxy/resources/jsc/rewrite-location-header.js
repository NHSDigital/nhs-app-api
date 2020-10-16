var requestUrl = context.getVariable('request-url');
var pathSplitter = context.getVariable('request-pathSuffix');
var regexString = new RegExp('.+?'+pathSplitter);
var locationHeader = context.targetResponse.headers['Location'];

var responseLocation = locationHeader.replace(regexString, requestUrl);

context.targetResponse.headers['Location'] = responseLocation;
