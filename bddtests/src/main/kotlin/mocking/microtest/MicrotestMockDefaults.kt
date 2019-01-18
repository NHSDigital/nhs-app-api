package mocking.microtest

import models.Patient

class MicrotestMockDefaults{

    companion object {

        const val DEFAULT_ODS_CODE_MICROTEST: String = "A21410"

        val patient = Patient.getDefault("MICROTEST")
    }
}
