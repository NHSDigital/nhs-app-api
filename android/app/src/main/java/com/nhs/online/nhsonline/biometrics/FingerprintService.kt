package com.nhs.online.nhsonline.biometrics

import android.annotation.TargetApi
import android.os.Build
import android.support.v4.app.FragmentActivity
import android.support.v4.hardware.fingerprint.FingerprintManagerCompat
import com.nhs.online.fidoclient.uaf.client.operation.Authentication
import com.nhs.online.fidoclient.uaf.crypto.FidoKeystoreAndroidM
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.utils.*
import com.nhs.online.nhsonline.interfaces.IInteractor

private const val keyIdPrefix = "com.nhs.online.nhsonline.fidouafclient.keystore.key"

@TargetApi(Build.VERSION_CODES.M)
class FingerprintService(
        biometricsInteractor: BiometricsInteractor,
        private val interactor: IInteractor,
        fidoKeystore: FidoKeystoreAndroidM,
        fingerprintSystemChecker: FingerprintSystemChecker,
        preferencesService: FingerprintSharedPreferences,
        fidoEndpointConfig: FidoEndpointConfig,
        uafAuthenticator: Authentication
) {

    private val authenticationService: AuthenticationService
    private val deRegistrationService: DeRegistrationService
    private val registrationService: RegistrationService
    val biometricState: BiometricState = BiometricState(preferencesService,
        fidoKeystore,
        fingerprintSystemChecker)
    private val biometricAsyncHandler: BiometricAsyncHandler

    init {

        val fidoDataHelper = BiometricCleanupHelper(
            biometricState,
            fidoKeystore,
            preferencesService)

        biometricAsyncHandler = BiometricAsyncHandler(fidoEndpointConfig)

        val signingHelper = SigningHelper(fidoKeystore, preferencesService)

        val activity: FragmentActivity = biometricsInteractor.getActivity()

        val fingerprintDialog = FingerprintDialog(activity, biometricState, signingHelper)

        val cookieService = FingerprintCookieService(activity)

        deRegistrationService = DeRegistrationService(biometricsInteractor,
            fidoDataHelper,
            preferencesService,
            biometricState,
            biometricAsyncHandler,
            cookieService
        )

        registrationService = RegistrationService(activity,
            biometricAsyncHandler,
            biometricsInteractor,
            cookieService,
            fidoDataHelper,
            fidoKeystore,
            fingerprintDialog,
            fingerprintSystemChecker,
            preferencesService,
            biometricState)

        authenticationService = AuthenticationService(activity,
            biometricAsyncHandler,
            biometricsInteractor,
            biometricState,
            fidoDataHelper,
            fingerprintDialog,
            fingerprintSystemChecker,
            preferencesService,
            uafAuthenticator)

    }

    fun cancelAllProgressingTasks() = biometricAsyncHandler.cancelAllTasks()

    fun deRegisterBiometrics() = deRegistrationService.deRegisterBiometrics()

    fun startFidoRegistration() = registrationService.startFidoRegistration()

    fun showBiometricLoginIfEnabled(forceStart: Boolean = false) = authenticationService.showBiometricLoginIfEnabled(forceStart)

    fun dismissBiometricDialog() = authenticationService.dismissBiometricDialog()

    fun notifyLoginErrorOccurrence() {
        biometricState.hasLoginError = true
    }

    companion object {

        fun createIfDeviceSupported(
            biometricsInteractor: BiometricsInteractor,
            fidoServerUrl: String,
            interactor: IInteractor
        ): FingerprintService? {
            if ((Build.VERSION.SDK_INT < Build.VERSION_CODES.M) || fidoServerUrl.isEmpty())
                return null

            val activity: FragmentActivity = biometricsInteractor.getActivity()

            val fidoEndpointConfig = FidoEndpointConfig(
                fidoServerUrl,
                activity.resources.getString(R.string.fidoGetAuthRequestPath),
                activity.resources.getString(R.string.fidoPostDeregResponsePath),
                activity.resources.getString(R.string.fidoGetRegRequestPath),
                activity.resources.getString(R.string.fidoPostRegResponsePath)
            )

            val fingerprintManager = FingerprintManagerCompat.from(activity)
            val fingerprintSystemChecker = FingerprintSystemChecker(fingerprintManager, activity, interactor)
            val fidoKeystore = FidoKeystoreAndroidM(keyIdPrefix)
            val preferencesService = FingerprintSharedPreferences(activity)
            val uafAuthenticator = Authentication()


            return FingerprintService(biometricsInteractor,
                    interactor,
                fidoKeystore,
                fingerprintSystemChecker,
                preferencesService,
                fidoEndpointConfig,
                uafAuthenticator)
        }
    }
}