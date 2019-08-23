package com.nhs.online.nhsonline.biometrics

import android.os.AsyncTask
import com.nhs.online.fidoclient.constants.CONNECTION_ERROR_CODE_KEY_AND_VALUE
import com.nhs.online.fidoclient.constants.EMPTY_UAF_RESPONSE_MESSAGE
import com.nhs.online.fidoclient.uaf.client.operation.Authentication
import com.nhs.online.fidoclient.uaf.client.operation.DeRegistration
import com.nhs.online.fidoclient.uaf.client.operation.Registration
import com.nhs.online.fidoclient.uaf.operationcall.RegistrationCall


class BiometricAsyncHandler(private val fidoEndpointConfig: FidoEndpointConfig) {
    private val asyncs = mutableSetOf<BiometricAsync>()

    fun fetchUafRegistrationMessage(
        facetId: String,
        accessToken: String,
        callback: (BiometricCallResult) -> Unit
    ) {
        performAsyncTask(callback) {
            return@performAsyncTask Registration()
                .requestUafRegistrationMessage(facetId, accessToken, fidoEndpointConfig.getRegRequestGet())
        }
    }

    fun sendClientRegistrationMsg(uafMessage: String, callback: (BiometricCallResult) -> Unit) {
        performAsyncTask(callback) {
            return@performAsyncTask RegistrationCall()
                .sendClientRegistrationMessage(uafMessage, fidoEndpointConfig.getRegResponsePost())
        }
    }

    fun requestUafAuthenticationMessage(facetId: String, callback: (BiometricCallResult) -> Unit) {
        performAsyncTask(callback) {
            return@performAsyncTask Authentication()
                .requestUafAuthenticationMessage(facetId, fidoEndpointConfig.getAuthRequestGet())
        }
    }

    fun sendDeRegistrationOperation(
        appId: String,
        keyId: String,
        accessToken: String?,
        callback: (BiometricCallResult?) -> Unit
    ) {
        performAsyncTask(callback) {
            if (accessToken != null) {
                DeRegistration()
                        .sendDeRegistrationOperation(appId, keyId, accessToken, fidoEndpointConfig.getDeregPost())
            }
            return@performAsyncTask ""
        }
    }

    fun cancelAllTasks() {
        asyncs.forEach { it.cancel(true) }
        asyncs.clear()
    }

    private fun performAsyncTask(callback: (BiometricCallResult) -> Unit, task: () -> String) {
        val asyncTask = BiometricAsync(task) { result: BiometricCallResult, biometricTask ->
            callback.invoke(result)
            asyncs.remove(biometricTask)
        }
        asyncTask.execute()
    }


    private class BiometricAsync(
        private val task: () -> String,
        private val taskCompleteCallback: (BiometricCallResult, BiometricAsync) -> Unit
    ) :
        AsyncTask<Void, Void, String?>() {

        override fun doInBackground(vararg params: Void): String? {
            if (isCancelled)
                return null
            return task.invoke()
        }

        override fun onPostExecute(result: String?) {
            val biometricCallResult = convertToBiometricCallResult(result)
            taskCompleteCallback.invoke(biometricCallResult, this)
        }

        private fun convertToBiometricCallResult(result: String?): BiometricCallResult {
            if (result == null)
                return BiometricCallResult("", ERROR)

            if (result == EMPTY_UAF_RESPONSE_MESSAGE)
                return BiometricCallResult(result, ERROR)

            if (result.contains(CONNECTION_ERROR_CODE_KEY_AND_VALUE)) {
                return BiometricCallResult(result, CONNECTION_ERROR)
            }
            return BiometricCallResult(result, OK)
        }
    }

    companion object {
        const val ERROR = -1
        const val CONNECTION_ERROR = -100

        const val OK = 0
    }
}