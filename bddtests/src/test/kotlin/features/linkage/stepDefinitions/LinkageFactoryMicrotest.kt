package features.linkage.stepDefinitions

import constants.DateTimeFormats
import features.linkage.LinkageResult
import mockingFacade.linkage.LinkageInformationFacade

class LinkageFactoryMicrotest : LinkageFactory("MICROTEST") {
    override val validOtherLinkageDetails = LinkageInformationFacade()

    override val validLinkageDetails = LinkageInformationFacade(
            odsCode = patient.odsCode,
            linkageKey = patient.linkageKey,
            accountId = patient.accountId,
            nhsNumber = patient.nhsNumbers.first(),
            surname = patient.surname,
            dateOfBirth = patient.dateOfBirth)

    override val linkageDateOfBirthFormat = DateTimeFormats.backendDateTimeFormatWithoutTimezone

    override fun mockLinkagePostResult(linkageInformationFacade: LinkageInformationFacade,
                                       linkageResult: LinkageResult, delay: Long?) {
    }

    override fun mockLinkageGetResult(linkageInformationFacade: LinkageInformationFacade,
                                      linkageResult: LinkageResult) {
    }
}
