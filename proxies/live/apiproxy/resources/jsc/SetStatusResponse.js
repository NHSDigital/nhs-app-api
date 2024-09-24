var apiproxy_revision = context.getVariable('apiproxy.revision');

var nhsa_response_code = context.getVariable('healthCheckResponse.status.code');
var nhsa_response = context.getVariable('healthCheckResponse.content');
var nhsa_request_url = context.getVariable('healthCheckRequest.url');

var nhsa_request_has_failed = context.getVariable("servicecallout.ServiceCallout.CallHealthCheckEndpoint.failed");

var nhsa_status = "fail";

if(nhsa_response_code / 100 == 2){
    nhsa_status = "pass";
}

var timeout = "false";

if(nhsa_response_code === null && nhsa_request_has_failed){
    timeout = "true";
}

var nhsa_service = {
"nhsa:status" : [
    {
    "status": nhsa_status,
    "timeout" : timeout,
    "responseCode" : nhsa_response_code,
    "outcome": nhsa_response,
    "links" : {"self": nhsa_request_url}
   }]
};

var apigee_status = "pass";

if(nhsa_status != "pass"){
    apigee_status = "fail";
}

var response = {
    "status" : apigee_status,
    "version" : "{{ DEPLOYED_VERSION }}" ,
    "revision" : apiproxy_revision,
    "releaseId" : "{{ RELEASE_RELEASEID }}",
    "commitId": "{{ SOURCE_COMMIT_ID }}",
    "checks" : nhsa_service
};

context.setVariable("status.response", JSON.stringify(response));
context.setVariable("response.content", JSON.stringify(response));
context.setVariable("response.header.Content-Type", "application/json");
