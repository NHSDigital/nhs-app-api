package features.prescriptions.helpers

import constants.SerenitySessionKeys
import net.serenitybdd.core.Serenity

class PrescriptionHelpers {
    companion object {
        fun setPrescriptionCommentsAllowed(prescriptionCommentsAllowed: Boolean) {
            Serenity.setSessionVariable(SerenitySessionKeys.PRESCRIPTION_COMMENTS_ALLOWED)
                    .to(prescriptionCommentsAllowed)
        }
    }
}
