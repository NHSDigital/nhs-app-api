package mocking.vision

import mocking.vision.models.ServiceDefinition

object VisionErrorResponses {

    fun getInvalidRequestError(serviceDefinition: mocking.vision.models.ServiceDefinition): String {

        return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "   <soap:Header>\n" +
                "   </soap:Header>\n" +
                "   <soap:Body>\n" +
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
                "   </soap:Body>\n" +
                "</soap:Envelope>"
    }

    val securityHeaderErrorResponse =
            "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                    "   <soap:Body>\n" +
                    "      <soap:Fault>\n" +
                    "         <faultcode xmlns:ns1=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-" +
                    "wssecurity-secext-1.0.xsd\">ns1:InvalidSecurity</faultcode>\n" +
                    "         <faultstring>An error was discovered processing the &lt;wsse:Security> " +
                    "header</faultstring>\n" +
                    "      </soap:Fault>\n" +
                    "   </soap:Body>\n" +
                    "</soap:Envelope>"


    fun getUnknownError(serviceDefinition: ServiceDefinition): String {
        return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "  <soap:Header>\n" +
                "  </soap:Header>\n" +
                "  <soap:Body>\n" +
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
                "  </soap:Body>\n" +
                "</soap:Envelope>"
    }

    fun getAccessDeniedError(serviceDefinition: ServiceDefinition): String {
        return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "  <soap:Header>\n" +
                "  </soap:Header>\n" +
                "  <soap:Body>\n" +
                "     <vision:visionResponse xmlns:vision=\"urn:vision\">\n" +
                "        <vision:serviceDefinition>\n" +
                "           <vision:name>${serviceDefinition.name}</vision:name>\n" +
                "           <vision:version>${serviceDefinition.version}</vision:version>\n" +
                "        </vision:serviceDefinition>\n" +
                "        <vision:serviceHeader>\n" +
                "           <vision:outcome>\n" +
                "              <vision:successful>false</vision:successful>\n" +
                "              <vision:error>\n" +
                "                 <vision:code>-35</vision:code>\n" +
                "                 <vision:description>Unknown Error</vision:description>\n" +
                "              </vision:error>\n" +
                "           </vision:outcome>\n" +
                "        </vision:serviceHeader>\n" +
                "     </vision:visionResponse>\n" +
                "  </soap:Body>\n" +
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
                "   <soap:Body>\n" +
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
                "   </soap:Body>\n" +
                "</soap:Envelope>"
    }

    fun getInvalidDetailsProvidedError(serviceDefinition: ServiceDefinition): String {
        return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "  <soap:Header>\n" +
                "  </soap:Header>\n" +
                "  <soap:Body>\n" +
                "     <vision:visionResponse xmlns:vision=\"urn:vision\">\n" +
                "        <vision:serviceDefinition>\n" +
                "           <vision:name>${serviceDefinition.name}</vision:name>\n" +
                "           <vision:version>${serviceDefinition.version}</vision:version>\n" +
                "        </vision:serviceDefinition>\n" +
                "        <vision:serviceHeader>\n" +
                "           <vision:outcome>\n" +
                "              <vision:successful>false</vision:successful>\n" +
                "              <vision:error>\n" +
                "                 <vision:code>-33</vision:code>\n" +
                "                 <vision:description>No Match: couldn't link account with detail provided" +
                "                 </vision:description>\n" +
                "              </vision:error>\n" +
                "           </vision:outcome>\n" +
                "        </vision:serviceHeader>\n" +
                "     </vision:visionResponse>\n" +
                "  </soap:Body>\n" +
                "</soap:Envelope>"
    }

    fun getInvalidParameterProvidedError(serviceDefinition: ServiceDefinition): String {
        return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "  <soap:Header>\n" +
                "  </soap:Header>\n" +
                "  <soap:Body>\n" +
                "     <vision:visionResponse xmlns:vision=\"urn:vision\">\n" +
                "        <vision:serviceDefinition>\n" +
                "           <vision:name>${serviceDefinition.name}</vision:name>\n" +
                "           <vision:version>${serviceDefinition.version}</vision:version>\n" +
                "        </vision:serviceDefinition>\n" +
                "        <vision:serviceHeader>\n" +
                "           <vision:outcome>\n" +
                "              <vision:successful>false</vision:successful>\n" +
                "              <vision:error>\n" +
                "                 <vision:code>-31</vision:code>\n" +
                "                 <vision:description>Invalid parameter provided</vision:description>\n" +
                "              </vision:error>\n" +
                "           </vision:outcome>\n" +
                "        </vision:serviceHeader>\n" +
                "     </vision:visionResponse>\n" +
                "  </soap:Body>\n" +
                "</soap:Envelope>"
    }

    fun getPatientAlreadyRegisteredError(serviceDefinition: ServiceDefinition): String {
        return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "  <soap:Header>\n" +
                "  </soap:Header>\n" +
                "  <soap:Body>\n" +
                "     <vision:visionResponse xmlns:vision=\"urn:vision\">\n" +
                "        <vision:serviceDefinition>\n" +
                "           <vision:name>${serviceDefinition.name}</vision:name>\n" +
                "           <vision:version>${serviceDefinition.version}</vision:version>\n" +
                "        </vision:serviceDefinition>\n" +
                "        <vision:serviceHeader>\n" +
                "           <vision:outcome>\n" +
                "              <vision:successful>false</vision:successful>\n" +
                "              <vision:error>\n" +
                "                 <vision:code>-2</vision:code>\n" +
                "                 <vision:description>User has already been registered</vision:description>\n" +
                "              </vision:error>\n" +
                "           </vision:outcome>\n" +
                "        </vision:serviceHeader>\n" +
                "     </vision:visionResponse>\n" +
                "  </soap:Body>\n" +
                "</soap:Envelope>"
    }

    fun getPatientLockedError(serviceDefinition: ServiceDefinition): String {
        return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "  <soap:Header>\n" +
                "  </soap:Header>\n" +
                "  <soap:Body>\n" +
                "     <vision:visionResponse xmlns:vision=\"urn:vision\">\n" +
                "        <vision:serviceDefinition>\n" +
                "           <vision:name>${serviceDefinition.name}</vision:name>\n" +
                "           <vision:version>${serviceDefinition.version}</vision:version>\n" +
                "        </vision:serviceDefinition>\n" +
                "        <vision:serviceHeader>\n" +
                "           <vision:outcome>\n" +
                "              <vision:successful>false</vision:successful>\n" +
                "              <vision:error>\n" +
                "                 <vision:code>-15</vision:code>\n" +
                "                 <vision:description>Record currently unavailable - please try again later or " +
                "                                       contact your Practice: VOSUsers record is locked, is " +
                "                                       patient selected in registration?</vision:description>\n" +
                "              </vision:error>\n" +
                "           </vision:outcome>\n" +
                "        </vision:serviceHeader>\n" +
                "     </vision:visionResponse>\n" +
                "  </soap:Body>\n" +
                "</soap:Envelope>"
    }

    fun getMockedError(serviceDefinition: ServiceDefinition, errorCode:String, errorMessage:String = "Mocked Error"):
            String {
        return "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
                "  <soap:Header>\n" +
                "  </soap:Header>\n" +
                "  <soap:Body>\n" +
                "     <vision:visionResponse xmlns:vision=\"urn:vision\">\n" +
                "        <vision:serviceDefinition>\n" +
                "           <vision:name>${serviceDefinition.name}</vision:name>\n" +
                "           <vision:version>${serviceDefinition.version}</vision:version>\n" +
                "        </vision:serviceDefinition>\n" +
                "        <vision:serviceHeader>\n" +
                "           <vision:outcome>\n" +
                "              <vision:successful>false</vision:successful>\n" +
                "              <vision:error>\n" +
                "                 <vision:code>$errorCode</vision:code>\n" +
                "                 <vision:description>$errorMessage" +
                "                 </vision:description>\n" +
                "              </vision:error>\n" +
                "           </vision:outcome>\n" +
                "        </vision:serviceHeader>\n" +
                "     </vision:visionResponse>\n" +
                "  </soap:Body>\n" +
                "</soap:Envelope>"
    }
}
