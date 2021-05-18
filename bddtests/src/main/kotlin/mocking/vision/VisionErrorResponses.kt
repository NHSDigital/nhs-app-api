package mocking.vision

import mocking.vision.models.ServiceDefinition
import mocking.vision.models.error.VisionErrorTypes

object VisionErrorResponses {

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
