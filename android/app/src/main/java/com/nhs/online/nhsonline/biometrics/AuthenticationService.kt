package com.nhs.online.nhsonline.biometrics

import android.annotation.TargetApi
import android.app.Activity
import android.os.Build
import androidx.fragment.app.FragmentActivity
import androidx.core.hardware.fingerprint.FingerprintManagerCompat
import android.util.Log
import com.nhs.online.fidoclient.constants.UAF_AUTH_RESPONSE_FIELD
import com.nhs.online.fidoclient.utils.fidoHelpers
import com.nhs.online.fidoclient.exceptions.FidoInvalidSignatureException
import com.nhs.online.fidoclient.exceptions.GenericFidoException
import com.nhs.online.fidoclient.uaf.client.AuthAssertionBuilder
import com.nhs.online.fidoclient.uaf.client.operation.Authentication
import com.nhs.online.fidoclient.uaf.crypto.FidoSignerAndroidM
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.utils.*
import com.nhs.online.nhsonline.support.toBase64
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.json.JSONObject
import java.lang.RuntimeException

private val TAG = AuthenticationService::class.java.simpleName

@TargetApi(Build.VERSION_CODES.M)
class AuthenticationService(
        private val activity: FragmentActivity,
        private val biometricAsyncHandler: BiometricAsyncHandler,
        private val biometricsInteractor: BiometricsInteractor,
        private val biometricState: BiometricState,
        private val biometricCleanupHelper: BiometricCleanupHelper,
        private val fingerprintDialog: FingerprintDialog,
        private val fingerprintSystemChecker: FingerprintSystemChecker,
        private val preferencesService: FingerprintSharedPreferences,
        private val uafAuthenticator: Authentication,
        private val appWebInterface: AppWebInterface
) {
    var isFingerprintLoginStarted = false

    fun showBiometricLoginIfEnabled(forceStart: Boolean = false): Boolean {
        if (forceStart)
            isFingerprintLoginStarted = false

        if (!biometricState.registered || isFingerprintLoginStarted)
            return false

        Log.d(TAG, "isRegistered")
        with(biometricsInteractor) {
            dismissBiometricNotification()
        }

        try {
            startFidoSignIn()
            return true
        } catch (fidoException: FidoInvalidSignatureException) {
            Log.d(TAG, "Invalid signature")
            throw fidoException
        }

    }

    private fun startFidoSignIn() {
        if (!this.fingerprintSystemChecker.preLoginCheck()) {
            biometricCleanupHelper.removeFidoData()
            return
        }
        val facetId = fidoHelpers.getFacetId(activity) ?: return
        isFingerprintLoginStarted = true
        biometricAsyncHandler.requestUafAuthenticationMessage(facetId) { response: BiometricCallResult ->
            if (response.statusCode != BiometricAsyncHandler.OK) {
                return@requestUafAuthenticationMessage
            }

            try {
                completeSignInStart(response)
            } catch (fidoException: FidoInvalidSignatureException) {
                Log.d(TAG, "Invalid signature")
                throw fidoException
            }

        }
    }

    fun completeSignInStart(response: BiometricCallResult) {
        try {
            val signInCallback = object : FingerprintAuthCallback(response.result) {
                override fun processAuthentication(cryptObj: FingerprintManagerCompat.CryptoObject): Int =
                    completeFidoSignIn(cryptObj, this.uafMessage)

                override fun cancel() {
                    biometricsInteractor.dismissProgressDialog()
                    isFingerprintLoginStarted = false
                }

                override fun error() {
                    biometricsInteractor.dismissProgressDialog()
                    showBiometricLoginIfEnabled(true)
                }
            }
            val fingerprintContent = fingerprintDialog.generateFingerprintContent(false)
            fingerprintDialog.showFingerprintAuthDialog(signInCallback, fingerprintContent)
        } catch (fidoException: FidoInvalidSignatureException) {
            biometricsInteractor.dismissProgressDialog()
            isFingerprintLoginStarted = false
            handleInvalidKeys()
            throw fidoException
        } catch (e: IllegalStateException) {
            Log.d(TAG, "Unable to show the fingerprint dialog: ", e)
            biometricsInteractor.dismissProgressDialog()
            isFingerprintLoginStarted = false
            appWebInterface.biometricLoginFailure()
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
            biometricsInteractor.dismissProgressDialog()
            Activity.RESULT_CANCELED
        } else {
            biometricsInteractor.showProgressDialog()
            val biometricLoginUrl = getLoginPathFromUafMessage(processedLoginMsg)
            biometricsInteractor.loadBiometricLoginPage(biometricLoginUrl)
            Activity.RESULT_OK
        }
    }

    private fun getLoginPathFromUafMessage(uafMessage: String): String {
        val authResponse = JSONObject(uafMessage).getString(UAF_AUTH_RESPONSE_FIELD)
        Log.d(TAG, "AuthResponse message is: $authResponse")

        val authResponseB64 = authResponse.toByteArray().toBase64()
        Log.d(TAG, "Base64 encoded AuthResponse message is: $authResponseB64")

        return "${activity.getString(R.string.fidoAuthQueryKey)}=$authResponseB64"
    }

    fun processUafLoginMsg(
            cryptObj: FingerprintManagerCompat.CryptoObject,
            uafLoginMsg: String
    ): String {
        return try {
            val signature =
                    cryptObj.signature ?: throw GenericFidoException("Signature not found",
                            RuntimeException())
            val fidoSigner = FidoSignerAndroidM(signature)
            val keyId: String = preferencesService.readStringFromSharedPref(BiometricConstants.KEY_ID)
            val authAssertionBuilder = AuthAssertionBuilder(fidoSigner, null, keyId)
            uafAuthenticator.auth(uafLoginMsg, authAssertionBuilder)
        } catch (e: FidoInvalidSignatureException) {
            handleInvalidKeys()
        } catch (e: GenericFidoException) {
            fingerprintSystemChecker.showInvalidFingerprintDialog()
            ""
        }
    }

    fun dismissBiometricDialog() {
        fingerprintDialog.dismissFingerprintAuthDialog()
        isFingerprintLoginStarted = false
    }
}
