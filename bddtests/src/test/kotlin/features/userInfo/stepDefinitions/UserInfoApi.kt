package features.userInfo.stepDefinitions

import net.serenitybdd.core.Serenity
import utils.SerenityHelpers
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient

class UserInfoApi {
    companion object {
        fun postUserInfoWithGivenToken(authToken: String?) {
            try {
                val response = Serenity
                        .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .userInfo.postUserInfo(authToken)
                SerenityHelpers.setHttpResponse(response)
            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
        }

        fun getUserInfoWithGivenToken(authToken: String?) {
            try {
                val response = Serenity
                        .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .userInfo.getUserInfo(authToken)
                UserInfoSerenityHelpers.GET_USER_INFO_RESPONSE.set(response)
            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
        }

        fun getUserInfo(odsCode: String?, nhsNumber:String?) {
            try {
                val response = Serenity
                        .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .userInfo.getUserInfo(odsCode, nhsNumber)
                UserInfoSerenityHelpers.GET_USER_INFO_NHSLOGINIDS_RESPONSE.set(response)
            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
        }
    }
}
