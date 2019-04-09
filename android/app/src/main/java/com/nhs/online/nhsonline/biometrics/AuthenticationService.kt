package com.nhs.online.nhsonline.biometrics

import android.annotation.TargetApi
import android.app.Activity
import android.os.Build
import android.support.v4.app.FragmentActivity
import android.support.v4.hardware.fingerprint.FingerprintManagerCompat
import android.util.Log
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.utils.*
import com.nhs.online.nhsonline.fido.Fido
import com.nhs.online.nhsonline.fido.uaf.client.AuthAssertionBuilder
import com.nhs.online.nhsonline.fido.uaf.client.operation.Authentication
import com.nhs.online.nhsonline.fido.uaf.crypto.FidoSignerAndroidM
import com.nhs.online.nhsonline.support.BiometricsInvalidSignatureException
import com.nhs.online.nhsonline.support.GenericBiometricException
import com.nhs.online.nhsonline.support.toBase64
import org.json.JSONObject
import java.lang.RuntimeException

private val TAG = AuthenticationService::class.java.simpleName

@TargetApi(Build.VERSION_CODES.M)
class AuthenticationService(
        private val activity: FragmentActivity,
        private val biometricAsyncHandler: BiometricAsyncHandler,
        private val biometricsInteractor: IBiometricsInteractor,
        private val biometricState: BiometricState,
        private val biometricCleanupHelper: BiometricCleanupHelper,
        private val fingerprintDialog: FingerprintDialog,
        private val fingerprintSystemChecker: FingerprintSystemChecker,
        private val preferencesService: FingerprintSharedPreferences,
        private val uafAuthenticator: Authentication
) {

    var isFingerprintLoginStarted = false

    fun showBiometricLoginIfEnabled(): Boolean {
        if (!biometricState.registered || isFingerprintLoginStarted)
            return false

        Log.d(TAG, "isRegistered")
        biometricsInteractor.toggleBiometricSwitch(true)
        startFidoSignIn()
        return true
    }

    private fun startFidoSignIn() {
        if (!this.fingerprintSystemChecker.preLoginCheck()) {
            biometricCleanupHelper.removeFidoData()
            return
        }
        val facetId = Fido.getFacetId(activity) ?: return
        isFingerprintLoginStarted = true
        biometricsInteractor.showProgressDialog()
        biometricAsyncHandler.requestUafAuthenticationMessage(facetId) { response: BiometricCallResult ->
            if (response.statusCode != BiometricAsyncHandler.OK) {
                biometricsInteractor.dismissProgressDialog()
                return@requestUafAuthenticationMessage
            }
            completeSignInStart(response)
        }
    }

    fun completeSignInStart(response: BiometricCallResult) {
        try {
            val signInCallback = object : FingerprintAuthCallback(response.result) {
                override fun processAuthentication(cryptObj: FingerprintManagerCompat.CryptoObject): Int =
                    completeFidoSignIn(cryptObj, this.uafMessage)

                override fun cancel() {
                    isFingerprintLoginStarted = false
                }
            }
            biometricsInteractor.dismissProgressDialog()
            val fingerprintContent = fingerprintDialog.generateFingerprintContent(false)
            fingerprintDialog.showFingerprintAuthDialog(signInCallback, fingerprintContent)
        } catch (e: BiometricsInvalidSignatureException) {
            isFingerprintLoginStarted = false
            handleInvalidKeys()
        } catch (e: IllegalStateException) {
            Log.d(TAG, "Unable to show the fingerprint dialog: ", e)
            isFingerprintLoginStarted = false
        }
    }

    private fun handleInvalidKeys(): String {
        fingerprintSystemChecker.showInvalidFingerprintDialog()
        biometricCleanupHelper.removeFidoData()
        return ""
    }

    private fun completeFidoSignIn(
            cryptObj: FingerprintManagerCompat.CryptoObject,
            uafLoginMsg: String
    ): Int {
        val processedLoginMsg = processUafLoginMsg(cryptObj, uafLoginMsg)

        isFingerprintLoginStarted = false
        biometricState.hasLoginError = false
        return if (processedLoginMsg.isEmpty()) {
            Activity.RESULT_CANCELED
        } else {
            val biometricLoginUrl = getLoginPathFromUafMessage(processedLoginMsg)
            biometricsInteractor.loadBiometricLoginPage(biometricLoginUrl)
            Activity.RESULT_OK
        }
    }

    private fun getLoginPathFromUafMessage(uafMessage: String): String {
        val authResponse = JSONObject(uafMessage).getString(Fido.UAF_AUTH_RESPONSE_FIELD)
        Log.d(TAG, "AuthResponse message is: $authResponse")

        val authResponseB64 = authResponse.toByteArray().toBase64()
        Log.d(TAG, "Base64 encoded AuthResponse message is: $authResponseB64")

        return "${activity.getString(R.string.fidoLoginPathWithAuthQueryKey)}=$authResponseB64"
    }

    fun processUafLoginMsg(
            cryptObj: FingerprintManagerCompat.CryptoObject,
            uafLoginMsg: String
    ): String {
        return try {
            val signature =
                    cryptObj.signature ?: throw GenericBiometricException("Signature not found",
                            RuntimeException())
            val fidoSigner = FidoSignerAndroidM(signature)
            val keyId: String = preferencesService.readStringFromSharedPref(BiometricConstants.KEY_ID)
            val authAssertionBuilder = AuthAssertionBuilder(fidoSigner, null, keyId)
            uafAuthenticator.auth(uafLoginMsg, authAssertionBuilder)
        } catch (e: BiometricsInvalidSignatureException) {
            handleInvalidKeys()
        } catch (e: GenericBiometricException) {
            fingerprintSystemChecker.showInvalidFingerprintDialog()
            ""
        }
    }
}