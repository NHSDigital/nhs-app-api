package mocking.vision

import mocking.vision.models.ServiceDefinition
import mocking.vision.models.error.VisionErrorTypes

object VisionErrorResponses {

    fun getInvalidRequestError(serviceDefinition: ServiceDefinition): String {

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
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.GATEWAYUNKNOWNERROR.Code,
                VisionErrorTypes.GATEWAYUNKNOWNERROR.Description
        )
    }

    fun getConnectionToExternalServiceFailedError(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.GATEWAYCONNECTIONTOEXTERNALSERVICEFAILED.Code,
                VisionErrorTypes.GATEWAYCONNECTIONTOEXTERNALSERVICEFAILED.Description
        )
    }

    fun getAccessDeniedError(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.ACCESSDENIED.Code,
                VisionErrorTypes.ACCESSDENIED.Description
        )
    }

    fun getInvalidUserCredentialsError(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.INVALIDUSERCREDENTIALS.Code,
                VisionErrorTypes.INVALIDUSERCREDENTIALS.Description
        )
    }

    fun getInvalidDetailsProvidedError(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.NOMATCH.Code,
                VisionErrorTypes.NOMATCH.Description
        )
    }

    fun getInvalidParameterProvidedError(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.INVALIDPARAMETERPROVIDED.Code,
                VisionErrorTypes.INVALIDPARAMETERPROVIDED.Description
        )
    }

    fun getPatientAlreadyRegisteredError(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.USERHASALREADYBEENREGISTERED.Code,
                VisionErrorTypes.USERHASALREADYBEENREGISTERED.Description
        )
    }

    fun getPatientLockedError(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.RECORDCURRENTLYUNAVAILABLE.Code,
                VisionErrorTypes.RECORDCURRENTLYUNAVAILABLE.Description
        )
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
