package mocking.vision

import mocking.vision.models.ServiceDefinition

object VisionConstants {

    // Service names and versions
    var configurationName: String = "VOS.GetConfiguration"
    var configurationVersion: String = "2.3.0"

    // Vision Response
    fun getVisionResponse(serviceContent: String, serviceDefinition: mocking.vision.models.ServiceDefinition): String {

        val response = serviceContent
                .replace("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>", "")
                .replace("</", "</vision:")
                .replace("<", "<vision:")
                .replace("vision:/", "/")


        return "<soapenv:Envelope xmlns:urn=\"urn:vision\" " +
               "xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "    <soapenv:body>\n" +
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
                "            <vision:serviceContent>\n" + response +
                "            </vision:serviceContent>\n" +
                "        </vision:visionResponse>\n" +
                "    </soapenv:body>\n" +
                "</soapenv:Envelope>"
    }

    // Common API Scenarios
    fun getInvalidRequestError(serviceDefinition: mocking.vision.models.ServiceDefinition): String {

        return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "   <soap:Header>\n" +
                "   </soap:Header>\n" +
                "   <soap:body>\n" +
                "      <soap:Fault xmlns:vision=\"urn:vision\">\n" +
                "         <faultcode>soap:Server</faultcode>\n" +
                "         <faultstring>-90001</faultstring>\n" +
                "         <detail>\n" +
                "            <vision:visionFault>\n" +
                "               <vision:serviceDefinition>\n" +
                "                  <vision:name>${serviceDefinition.name}</vision:name>\n" +
                "                  <vision:version>${serviceDefinition.version}</vision:version>\n" +
                "               </vision:serviceDefinition>\n" +
                "               <vision:error>\n" +
                "                  <vision:category>INVALID_REQUEST</vision:category>\n" +
                "                  <vision:code>-90001</vision:code>\n" +
                "                  <vision:text>Request does not match expected format</vision:text>\n" +
                "               </vision:error>\n" +
                "            </vision:visionFault>\n" +
                "         </detail>\n" +
                "      </soap:Fault>\n" +
                "   </soap:body>\n" +
                "</soap:Envelope>"
    }

    val securityHeaderErrorResponse =
            "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                    "   <soap:body>\n" +
                    "      <soap:Fault>\n" +
                    "         <faultcode xmlns:ns1=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-" +
                    "wssecurity-secext-1.0.xsd\">ns1:InvalidSecurity</faultcode>\n" +
                    "         <faultstring>An error was discovered processing the &lt;wsse:Security> " +
                    "header</faultstring>\n" +
                    "      </soap:Fault>\n" +
                    "   </soap:body>\n" +
                    "</soap:Envelope>"

    fun getUnkownError(serviceDefinition: ServiceDefinition): String {
        return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "  <soap:Header>\n" +
                "  </soap:Header>\n" +
                "  <soap:body>\n" +
                "     <vision:visionResponse xmlns:vision=\"urn:vision\">\n" +
                "        <vision:serviceDefinition>\n" +
                "           <vision:name>${serviceDefinition.name}</vision:name>\n" +
                "           <vision:version>${serviceDefinition.version}</vision:version>\n" +
                "        </vision:serviceDefinition>\n" +
                "        <vision:serviceHeader>\n" +
                "           <vision:outcome>\n" +
                "              <vision:successful>false</vision:successful>\n" +
                "              <vision:error>\n" +
                "                 <vision:code>-100</vision:code>\n" +
                "                 <vision:description>Unknown Error</vision:description>\n" +
                "              </vision:error>\n" +
                "           </vision:outcome>\n" +
                "        </vision:serviceHeader>\n" +
                "     </vision:visionResponse>\n" +
                "  </soap:body>\n" +
                "</soap:Envelope>"
    }

    fun getInvalidUserCredentialsError(serviceDefinition: ServiceDefinition): String {
        return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "   <soap:Header>\n" +
                "      <Action xmlns=\"http://www.w3.org/2005/08/addressing\">" +
                "urn:vision:Vision:VisionResponse</Action>\n" +
                "      <MessageID xmlns=\"http://www.w3.org/2005/08/addressing\">" +
                "FAEC4FE2-D8CE-4129-BCB4-F65154B1E60F</MessageID>\n" +
                "      <To xmlns=\"http://www.w3.org/2005/08/addressing\">" +
               "http://www.w3.org/2005/08/addressing/anonymous</To>\n" +
                "      <RelatesTo xmlns=\"http://www.w3.org/2005/08/addressing\">" +
                "uuid:bd81e6a9-c971-4b48-9306-28b2d8cd9a50</RelatesTo>\n" +
                "   </soap:Header>\n" +
                "   <soap:body>\n" +
                "      <vision:visionResponse xmlns:vision=\"urn:vision\">\n" +
                "         <vision:serviceDefinition>\n" +
                "            <vision:name>${serviceDefinition.name}</vision:name>\n" +
                "            <vision:version>${serviceDefinition.version}</vision:version>\n" +
                "         </vision:serviceDefinition>\n" +
                "         <vision:serviceHeader>\n" +
                "            <vision:outcome>\n" +
                "               <vision:successful>false</vision:successful>\n" +
                "               <vision:error>\n" +
                "                  <vision:code>-30</vision:code>\n" +
                "                  <vision:description>Invalid user credentials</vision:description>\n" +
                "               </vision:error>\n" +
                "            </vision:outcome>\n" +
                "         </vision:serviceHeader>\n" +
                "      </vision:visionResponse>\n" +
                "   </soap:body>\n" +
                "</soap:Envelope>"
    }



}