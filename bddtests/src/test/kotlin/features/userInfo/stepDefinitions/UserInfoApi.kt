package features.userInfo.stepDefinitions

import net.serenitybdd.core.Serenity
import utils.SerenityHelpers
import utils.set
import worker.WorkerClient
import worker.models.userInfo.UserResearchPreferenceRequest

class UserInfoApi {
    companion object {

        fun postUserInfoWithGivenToken(authToken: String?) {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userInfo.postUserInfo(authToken)
            SerenityHelpers.setHttpResponse(response)
        }

        fun postUserResearch(authToken: String?, preference: String) {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userInfo.postUserResearchPreference(authToken, UserResearchPreferenceRequest(preference))
            SerenityHelpers.setHttpResponse(response)
        }

        fun getUserInfoWithGivenToken(authToken: String?) {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userInfo.getUserInfo(authToken)
            UserInfoSerenityHelpers.GET_USER_INFO_RESPONSE.set(response)
        }

        fun getUserInfo(odsCode: String?, nhsNumber: String?, includeApiKey: Boolean = true) {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userInfo.getUserInfo(odsCode, nhsNumber, includeApiKey)
            UserInfoSerenityHelpers.GET_USER_INFO_NHSLOGINIDS_RESPONSE.set(response)
        }
    }
}
