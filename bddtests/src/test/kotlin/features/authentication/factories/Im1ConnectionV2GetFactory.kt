package features.authentication.factories

import constants.Supplier
import features.authentication.stepDefinitions.PatientVerificationSerenityHelpers
import mocking.SupplierSpecificFactory
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.models.IdentifierType
import models.Patient
import utils.SerenityHelpers
import utils.set

abstract class Im1ConnectionV2GetFactory(protected val gpSystem: Supplier) {

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

        override val map: HashMap<Supplier, () -> Im1ConnectionV2GetFactory>
            get() = hashMapOf(
                    Supplier.EMIS to { Im1ConnectionV2GetFactoryEmis() },
                    Supplier.TPP to { Im1ConnectionV2GetFactoryTpp() },
                    Supplier.VISION to { Im1ConnectionV2GetFactoryVision() },
                    Supplier.MICROTEST to { Im1ConnectionV2GetFactoryMicrotest()} )
    }
}
