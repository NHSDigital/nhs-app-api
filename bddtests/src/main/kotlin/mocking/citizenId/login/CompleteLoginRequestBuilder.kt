package mocking.citizenId.login

import config.Config
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping
import models.Patient
import utils.GlobalSerenityHelpers
import utils.getOrFail

class CompleteLoginRequestBuilder(val patient: Patient, customId: String? = null)
    : CitizenIdMappingBuilder("GET",
        "/complete-login") {

    init {
        val customIdForLogin = customId ?: patient.hashCode().toString()
        requestBuilder.andQueryParameter("mock_patient",customIdForLogin , "equalTo")
    }

    fun respondWithRedirectResponse(): Mapping {
        val redirectUri = if(GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.getOrFail()
                && !Config.instance.isNativeAppTestRun) {
            Config.instance.cidRedirectUri
        } else {
            "{{request.query.redirect_uri}}"
        }
        return redirectTo(
                "${redirectUri}?state={{request.query.state}}&code=" +
                        patient.authCode)
    }

    fun respondWithTermsNotAcceptedResponse(): Mapping {
        val redirectUri = if(GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.getOrFail()
                && !Config.instance.isNativeAppTestRun) {
            Config.instance.cidRedirectUri
        } else {
            "{{request.query.redirect_uri}}"
        }

        return redirectTo("${redirectUri}?state={{request.query.state}}&" +
                "error=access_denied&error_description=ConsentNotGiven")
    }
}
