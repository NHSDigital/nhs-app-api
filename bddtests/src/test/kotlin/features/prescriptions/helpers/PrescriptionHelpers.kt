package features.prescriptions.helpers

import constants.SerenitySessionKeys
import net.serenitybdd.core.Serenity

class PrescriptionHelpers {
    companion object {
        fun getPrescriptionCommentsAllowed(): Boolean {
            if (Serenity.hasASessionVariableCalled(SerenitySessionKeys.PRESCRIPTION_COMMENTS_ALLOWED) ){
                return Serenity.sessionVariableCalled<Boolean>(SerenitySessionKeys.PRESCRIPTION_COMMENTS_ALLOWED)
            }
            return true
        }

        fun setPrescriptionCommentsAllowed(prescriptionCommentsAllowed: Boolean) {
            Serenity.setSessionVariable(SerenitySessionKeys.PRESCRIPTION_COMMENTS_ALLOWED).to(prescriptionCommentsAllowed)
        }
    }
}