package mocking.vision.helpers

class VisionConstantsHelper {

    companion object {

        fun setContextOnServiceContent(serviceContent: String, context: String): String {
            return serviceContent
                    .replace("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>", "")
                    .replace("<ns2:$context xmlns:ns2=\"urn:vision\">",
                            "<$context xmlns=\"urn:vision\" xmlns:mb=\"urn:messagebus\">")
                    .replace("</ns2:$context>", "</$context>")
        }

        fun getBaseVisionResponse(response: String, serviceDefinition: mocking.vision.models.ServiceDefinition):
                String {

            return "<soap:Envelope xmlns:urn=\"urn:vision\" " +
                    "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
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
