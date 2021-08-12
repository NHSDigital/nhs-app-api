var requestUrl = context.getVariable('request-url');
var pathSplitter = context.getVariable('request-pathSuffix');
var regexString = new RegExp('(?<=<)(.*?)(?:'+pathSplitter+')', 'g');
var linkHeader = context.targetResponse.headers['Link'];

var responseLink = linkHeader.replace(regexString, requestUrl);

context.targetResponse.headers['Link'] = responseLink;