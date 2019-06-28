package features.authentication.factories

import constants.DateTimeFormats
import mocking.gpServiceBuilderInterfaces.IErrorMappingBuilder
import mocking.models.Mapping
import mockingFacade.linkage.LinkageInformationFacade
import java.time.Duration

class Im1ConnectionV2FactoryMicrotest : Im1ConnectionV2Factory("MICROTEST") {
    override fun successfulIm1Register(linkageFacade: LinkageInformationFacade, delay: Duration?) {
        TODO("not implemented")
    }

    override fun errorIm1Register(httpStatusCode: Int, errorCode: String, message: String?) {
        TODO("not implemented")
    }

    override fun successfulLinkagePost(linkageInformationFacade: LinkageInformationFacade) {
        TODO("not implemented")
    }

    override fun successfulLinkageGet(linkageInformationFacade: LinkageInformationFacade) {
        TODO("not implemented")
    }

    override val linkageDateOfBirthFormat = DateTimeFormats.backendDateTimeFormatWithoutTimezone

    override fun linkageGet(linkageInformationFacade: LinkageInformationFacade,
                            action: (IErrorMappingBuilder)-> Mapping) {
        TODO("not implemented")
    }

    override fun linkagePost(linkageInformationFacade: LinkageInformationFacade,
                            action: (IErrorMappingBuilder)-> Mapping) {
        TODO("not implemented")
    }
}
