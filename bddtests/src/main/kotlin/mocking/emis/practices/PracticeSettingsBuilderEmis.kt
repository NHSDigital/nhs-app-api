package mocking.emis.practices

import mocking.GsonFactory
import mocking.emis.EmisMappingBuilder
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus.SC_OK

class PracticeSettingsBuilderEmis(patient: Patient) :
        EmisMappingBuilder(null, "GET", "/practices/${patient.odsCode}/settings") {

    fun respondWithSuccess(settingsResponse: SettingsResponseModel): Mapping {
        return respondWithSuccessAny(settingsResponse)
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(SC_OK) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}
