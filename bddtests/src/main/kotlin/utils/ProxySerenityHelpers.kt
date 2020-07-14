package utils

import mockingFacade.linkedProfiles.LinkedProfileFacade
import models.Patient

class ProxySerenityHelpers {

    companion object {

        fun getPatientOrProxy(): Patient {
            val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrNull<LinkedProfileFacade>()
            if (selectedProfile != null) {
                return selectedProfile.profile
            }
            return SerenityHelpers.getPatientOrNull() ?: Patient.getDefault(SerenityHelpers.getGpSupplier())
        }
    }
}
