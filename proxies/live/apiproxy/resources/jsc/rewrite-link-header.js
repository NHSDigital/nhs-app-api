var requestUrl = context.getVariable('request-url');
var pathSplitter = context.getVariable('request-pathSuffix');
var baseUrl = '<' + (requestUrl.match('^(.+?)'+pathSplitter) || [requestUrl])[0];

var regexString = new RegExp('.+?'+pathSplitter);

var linkHeader = context.getVariable('response.header.Link.values.string');

var links = linkHeader.split(',');

for(var i = 0; i < links.length; i++){
    context.targetResponse.headers['Link'][i] = links[i].replace(regexString, baseUrl);
}
