package com.nhs.online.nhsonline.biometrics

import android.annotation.TargetApi
import android.app.Activity
import android.os.Build
import android.support.v4.app.FragmentActivity
import android.support.v4.hardware.fingerprint.FingerprintManagerCompat
import android.util.Log
import com.nhs.online.fidoclient.constants.ATTESTATION_STATUS
import com.nhs.online.fidoclient.constants.ATTESTATION_STATUS_VALID
import com.nhs.online.fidoclient.constants.REGISTRATION_RESPONSE_ERROR
import com.nhs.online.fidoclient.constants.REGISTRATION_STATUS
import com.nhs.online.fidoclient.constants.REGISTRATION_STATUS_SUCCESS
import com.nhs.online.fidoclient.interfaces.IBiometricsInteractor
import com.nhs.online.fidoclient.uaf.client.RegAssertionBuilder
import com.nhs.online.fidoclient.uaf.client.operation.Registration
import com.nhs.online.fidoclient.uaf.crypto.FidoKeystoreAndroidM
import com.nhs.online.fidoclient.uaf.message.RegistrationRequest
import com.nhs.online.fidoclient.utils.fidoHelpers
import com.nhs.online.nhsonline.biometrics.utils.BiometricState
import com.nhs.online.nhsonline.biometrics.utils.*
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import org.json.JSONArray
import org.json.JSONException
import java.security.KeyStoreException
import java.security.PublicKey
import java.security.Signature

private val TAG = RegistrationService::class.java.simpleName
private const val KEY_ID_PREFIX = "nhs-app-key"

@TargetApi(Build.VERSION_CODES.M)
class RegistrationService(
    private val activity: FragmentActivity,
    private val biometricAsyncHandler: BiometricAsyncHandler,
    private val biometricsInteractor: IBiometricsInteractor,
    private val cookieService: FingerprintCookieService,
    private val biometricCleanupHelper: BiometricCleanupHelper,
    private val fidoKeystore: FidoKeystoreAndroidM,
    private val fingerprintDialog: FingerprintDialog,
    private val fingerprintSystemChecker: FingerprintSystemChecker,
    private val preferencesService: FingerprintSharedPreferences,
    private val biometricState: BiometricState,
    private val appWebInterface: AppWebInterface,
    val assertionFactory: (publicKey: PublicKey, signature: Signature, keyId: String) ->
        RegAssertionBuilder = {p,s,k -> RegAssertionBuilder(p, s, k) }
    ) {
    fun startFidoRegistration() {
        if (!isRegistrationReady()) {
            appWebInterface.biometricCompletion(
                BiometricConstants.REGISTER,
                BiometricConstants.FAILURE,
                BiometricConstants.CANNOT_FIND_CODE)
            return
        }

        val facetId = fidoHelpers.getFacetId(activity)
        val accessToken = cookieService.getAccessTokenFromCookie()
        if (facetId.isNullOrBlank() || accessToken.isNullOrBlank()) {
            appWebInterface.biometricCompletion(
                BiometricConstants.REGISTER,
                BiometricConstants.FAILURE,
                BiometricConstants.CANNOT_CHANGE_CODE)
            return
        }

        biometricAsyncHandler.fetchUafRegistrationMessage(facetId,
            accessToken) { response: BiometricCallResult ->
            handleRegistrationResponse(response)
        }

    }

    fun doFingerprintsExist(): Boolean {
        return fingerprintSystemChecker.checkIfFingerprintsExist()
    }

    private fun isRegistrationReady(): Boolean {
        val fidoUser = preferencesService.getFidoUsername()
        val registeredState = preferencesService.getFingerprintRegisteredState()
        if (fidoUser.isNotEmpty() || registeredState) {
            biometricCleanupHelper.removeFidoData()
        }

        return fingerprintSystemChecker.preRegistrationCheck()

    }

    private fun handleRegistrationResponse(response: BiometricCallResult) {
        try {
            if (response.statusCode != BiometricAsyncHandler.OK) {
                appWebInterface.biometricCompletion(
                    BiometricConstants.REGISTER,
                    BiometricConstants.FAILURE,
                    BiometricConstants.CANNOT_CHANGE_CODE)
                return
            }
            val result = response.result
            val username = extractUserFromUafRegMessage(result)
            preferencesService.saveFidoUsername(username)
            fidoKeystore.generateKeyPair(username)

            val registerCallback = object : FingerprintAuthCallback(result) {
                override fun processAuthentication(cryptObj: FingerprintManagerCompat.CryptoObject): Int =
                        completeFidoRegistration(cryptObj, this.uafMessage)

                override fun cancel() {
                    appWebInterface.biometricCompletion(
                        BiometricConstants.REGISTER,
                        BiometricConstants.CANCELLED,
                        "")
                    biometricState.registrationStateChangeInProgress = false
                    return
                }

                override fun error() {
                    appWebInterface.biometricCompletion(
                        BiometricConstants.REGISTER,
                        BiometricConstants.FAILURE,
                        BiometricConstants.CANNOT_CHANGE_CODE)
                    return
                }
            }

            biometricState.registrationStateChangeInProgress = true
            val fingerprintContent = fingerprintDialog.generateFingerprintContent(true)
            fingerprintDialog.showFingerprintAuthDialog(registerCallback, fingerprintContent)
        }
        catch(fidoException: Exception) {
            Log.d(TAG, "Registration call failed due to fido exception", fidoException)
            showErrorAndRemoveData(
                BiometricConstants.REGISTER,
                BiometricConstants.FAILURE,
                BiometricConstants.CANNOT_CHANGE_CODE)
        }
        catch(e: Exception) {
            Log.d(TAG, "Registration call failed", e)
            showErrorAndRemoveData(
                BiometricConstants.REGISTER,
                BiometricConstants.FAILURE,
                BiometricConstants.CANNOT_CHANGE_CODE)
        }
        biometricState.registrationStateChangeInProgress = false
    }

    fun completeFidoRegistration(
        cryptObj: FingerprintManagerCompat.CryptoObject,
        uafMessage: String
    ): Int {
        val signature = cryptObj.signature
        if (signature == null) {
            showErrorAndRemoveData(
                BiometricConstants.REGISTER,
                BiometricConstants.FAILURE,
                BiometricConstants.CANNOT_CHANGE_CODE)
            return Activity.RESULT_CANCELED
        }
        try {
            val processedRegMessage = processUafRegistrationMsg(uafMessage, signature)
            Log.d(TAG, "UAF processed message $processedRegMessage")

            biometricAsyncHandler.sendClientRegistrationMsg(processedRegMessage) { response: BiometricCallResult ->
                if (response.statusCode != BiometricAsyncHandler.OK) {
                    showErrorAndRemoveData(
                        BiometricConstants.REGISTER,
                        BiometricConstants.FAILURE,
                        BiometricConstants.CANNOT_CHANGE_CODE)
                    return@sendClientRegistrationMsg
                }

                if (verifyIfRegistrationSuccess(response.result)) {
                    preferencesService.storeFingerprintState(true)
                    biometricState.registered = true
                    appWebInterface.biometricCompletion(
                        BiometricConstants.REGISTER,
                        BiometricConstants.SUCCESS,
                        "")
                } else {
                    showErrorAndRemoveData(
                        BiometricConstants.REGISTER,
                        BiometricConstants.FAILURE,
                        BiometricConstants.CANNOT_CHANGE_CODE)
                }
            }
        }
        catch (keyStoreException: KeyStoreException){
            showErrorAndRemoveData(
                BiometricConstants.REGISTER,
                BiometricConstants.FAILURE,
                BiometricConstants.CANNOT_CHANGE_CODE)
        }

        return Activity.RESULT_OK
    }

    private fun processUafRegistrationMsg(inMsg: String, signature: Signature): String {
        Log.d(TAG, "op=Reg")
        val username = preferencesService.getFidoUsername()
        Log.i(TAG, "USERNAME: $username")
        val registerResponseHandler = Registration()

        val appId = registerResponseHandler.retrieveApplicationIdFrom(inMsg)
        preferencesService.storeString(BiometricConstants.APP_ID, appId)
        val keyId = fidoHelpers.generateFidoKeyId(KEY_ID_PREFIX)
        preferencesService.storeString(BiometricConstants.KEY_ID, keyId)

        val publicKey = fidoKeystore.getPublicKey(preferencesService.getFidoUsername())
        val assertionBuilder = assertionFactory(publicKey, signature, keyId)
        
        return registerResponseHandler.processRegisterMessage(inMsg, assertionBuilder)
    }

    private fun verifyIfRegistrationSuccess(serverResponse: String): Boolean {
        if (serverResponse.equals(REGISTRATION_RESPONSE_ERROR, true))
            return false

        return try {
            val responseJSONArray = JSONArray(serverResponse)
            if (responseJSONArray.length() == 0)
                return false

            val responseJsonObject = responseJSONArray.getJSONObject(0)
            if (!(responseJsonObject.has(REGISTRATION_STATUS) && responseJsonObject.has(ATTESTATION_STATUS)))
                return false

            val status = responseJsonObject.getString(REGISTRATION_STATUS)
            val attestationStatus = responseJsonObject.getString(ATTESTATION_STATUS)

            status.equals(REGISTRATION_STATUS_SUCCESS, true)
                    && attestationStatus.equals(ATTESTATION_STATUS_VALID, true)
        } catch (e: JSONException) {
            false
        }
    }

    private fun extractUserFromUafRegMessage(uafRegMsg: String): String {
        val regRequest = Moshi.Builder()
                .add(KotlinJsonAdapterFactory())
                .build()
                .adapter(Array<RegistrationRequest>::class.java)
                .fromJson(uafRegMsg)!![0]

        return regRequest.username
    }

    private fun showErrorAndRemoveData(action: String, outcome: String, errorCode: String){
        appWebInterface.biometricCompletion(action, outcome, errorCode)
        biometricCleanupHelper.removeFidoData()
    }
}