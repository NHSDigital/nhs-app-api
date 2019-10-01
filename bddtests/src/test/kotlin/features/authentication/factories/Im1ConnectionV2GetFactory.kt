package features.authentication.factories

import features.authentication.stepDefinitions.PatientVerificationSerenityHelpers
import mocking.SupplierSpecificFactory
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.models.IdentifierType
import models.Patient
import utils.SerenityHelpers
import utils.set

abstract class Im1ConnectionV2GetFactory(protected val gpSystem: String) {

    protected val mockingClient = SerenityHelpers.getMockingClient()

    var patient = Patient.getDefault(gpSystem)

    init{
        PatientVerificationSerenityHelpers.ConnectionToken.set(patient.connectionToken)
        PatientVerificationSerenityHelpers.NationalPracticeCode.set(patient.odsCode)
        PatientVerificationSerenityHelpers.NhsNumbers.set(
                arrayOf(PatientIdentifier(patient.nhsNumbers[0], IdentifierType.NhsNumber))
        )

    }

    abstract fun errorIm1Verify(httpStatusCode: Int, errorCode: String,
                                message: String?)

    companion object : SupplierSpecificFactory<Im1ConnectionV2GetFactory>() {

        override val map: HashMap<String, () -> Im1ConnectionV2GetFactory>
            get() = hashMapOf(
                    "EMIS" to { Im1ConnectionV2GetFactoryEmis() },
                    "TPP" to { Im1ConnectionV2GetFactoryTpp() },
                    "VISION" to { Im1ConnectionV2GetFactoryVision() })
    }
}