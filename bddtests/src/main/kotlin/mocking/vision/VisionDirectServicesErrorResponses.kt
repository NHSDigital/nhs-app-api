package mocking.vision

import mocking.vision.models.ServiceDefinition
import mocking.vision.models.error.VisionErrorTypes

object VisionDirectServicesErrorResponses {

    fun getInvalidRequestError(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
            serviceDefinition,
            "Request does not match expected format",
            "",
            "INVALID_REQUEST"
        )
    }

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

    fun getInvalidUserCredentialsError(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.INVALIDUSERCREDENTIALS.Code,
                VisionErrorTypes.INVALIDUSERCREDENTIALS.Description
        )
    }

    fun getRegistrationIncomplete(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.REGISTRATIONINCOMPLETE.Code,
                VisionErrorTypes.REGISTRATIONINCOMPLETE.Description
        )
    }

    fun getPatientLockedError(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.RECORDCURRENTLYUNAVAILABLE.Code,
                VisionErrorTypes.RECORDCURRENTLYUNAVAILABLE.Description
        )
    }

    fun getAccessDeniedError(serviceDefinition: ServiceDefinition): String {
        return getMockedError(
                serviceDefinition,
                VisionErrorTypes.ACCESSDENIED.Code,
                VisionErrorTypes.ACCESSDENIED.Description
        )
    }

    fun getMockedError(
        serviceDefinition: ServiceDefinition,
        errorCode:String,
        errorMessage:String = "Mocked Error",
        category: String = "FATAL"):
            String {
        return "<detail>\n" +
                "    <vision:visionFault xmlns:vision=\"urn:vision\" xmlns:mb=\"urn:messagebus\">\n" +
                "        <vision:serviceDefinition>\n" +
                "            <vision:name>${serviceDefinition}</vision:name>\n" +
                "            <vision:version>2.4.0</vision:version>\n" +
                "        </vision:serviceDefinition>\n" +
                "        <vision:error>\n" +
                "            <vision:category>${category}</vision:category>\n" +
                "            <vision:code>VZ201</vision:code>\n" +
                "            <vision:text>${errorCode}</vision:text>\n" +
                "            <vision:details>${errorMessage}</vision:details>\n" +
                "        </vision:error>\n" +
                "    </vision:visionFault>\n" +
                "</detail>"
    }
}
