package mocking.vision.helpers

import mocking.vision.models.ServiceDefinition

class VisionConstantsHelper {

    companion object {

        fun setContextOnServiceContent(serviceContent: String, context: String): String {
            return serviceContent
                    .replace("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>", "")
                    .replace("<ns2:$context xmlns:ns2=\"urn:vision\">",
                            "<$context xmlns=\"urn:vision\" xmlns:mb=\"urn:messagebus\">")
                    .replace("</ns2:$context>", "</$context>")
        }

        fun getBaseVisionFailedResponse(
                serviceDefinition: ServiceDefinition,
                errorCode: String, description: String = "0"): String {
            return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                    "    <soap:Body>\n" +
                    "        <vision:visionResponse xmlns:vision=\"urn:vision\">\n" +
                    "            <vision:serviceDefinition>\n" +
                    "                <vision:name>${serviceDefinition.name}</vision:name>\n" +
                    "                <vision:version>${serviceDefinition.version}</vision:version>\n" +
                    "            </vision:serviceDefinition>\n" +
                    "            <vision:serviceHeader>\n" +
                    "                <vision:outcome>\n" +
                    "                    <vision:successful>false</vision:successful>\n" +
                    "                    <vision:error>\n"+
                    "                       <vision:code>$errorCode</vision:code>\n" +
                    "                       <vision:description>$description</vision:description>\n" +
                    "                    </vision:error>\n"+
                    "                </vision:outcome>\n" +
                    "            </vision:serviceHeader>\n" +
                    "        </vision:visionResponse>\n" +
                    "    </soap:Body>\n" +
                    "</soap:Envelope>"
        }

        fun getBaseVisionResponse(response: String, serviceDefinition: ServiceDefinition):
                String {

            return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                    "    <soap:Body>\n" +
                    "        <vision:visionResponse xmlns:vision=\"urn:vision\">\n" +
                    "            <vision:serviceDefinition>\n" +
                    "                <vision:name>${serviceDefinition.name}</vision:name>\n" +
                    "                <vision:version>${serviceDefinition.version}</vision:version>\n" +
                    "            </vision:serviceDefinition>\n" +
                    "            <vision:serviceHeader>\n" +
                    "                <vision:outcome>\n" +
                    "                    <vision:successful>true</vision:successful>\n" +
                    "                </vision:outcome>\n" +
                    "            </vision:serviceHeader>\n" +
                    //           putting service content on one line as response can be raw text (avoiding new lines)
                    "            <vision:serviceContent>" + response + "</vision:serviceContent>" +
                    "        </vision:visionResponse>\n" +
                    "    </soap:Body>\n" +
                    "</soap:Envelope>"
        }
    }
}
