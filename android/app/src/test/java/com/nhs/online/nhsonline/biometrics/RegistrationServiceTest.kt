package com.nhs.online.nhsonline.biometrics

import android.content.pm.PackageInfo
import android.content.pm.PackageManager
import android.content.pm.Signature
import android.support.v4.app.FragmentActivity
import android.support.v4.hardware.fingerprint.FingerprintManagerCompat
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.fidoclient.uaf.crypto.FidoKeystoreAndroidM
import com.nhs.online.nhsonline.biometrics.utils.*
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.ArgumentCaptor
import org.mockito.ArgumentMatchers.any
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class RegistrationServiceTest {
    private lateinit var preferencesService: FingerprintSharedPreferences
    private lateinit var cookieService: FingerprintCookieService
    lateinit var fingerprintSystemChecker: FingerprintSystemChecker
    private lateinit var appWebInterface: AppWebInterface
    private lateinit var registrationService: RegistrationService
    private lateinit var biometricAsyncHandler: BiometricAsyncHandler
    private lateinit var fragmentActivity: FragmentActivity
    private lateinit var fidoKeystore: FidoKeystoreAndroidM

    private lateinit var packageManager: PackageManager
    private lateinit var packageInfo: PackageInfo

    private lateinit var signature: Signature

    private lateinit var uafCallResult: BiometricCallResult
    private lateinit var callResult: BiometricCallResult
    private lateinit var fingerprintDialog: FingerprintDialog
    private var captor = argumentCaptor<(BiometricCallResult?) -> Unit>()
    private lateinit var cryptoObject: FingerprintManagerCompat.CryptoObject
    private lateinit var fingerprintContent: FingerprintContent

    private val androidHash = BiometricMockTestData.getAndroidHash()

    @Before
    fun setUp() {

        preferencesService = mock {
            on { readStringFromSharedPref("appId") } doReturn "appId"
            on { readStringFromSharedPref("keyId") } doReturn "keyId"
            on { getFidoUsername() } doReturn  "username"
        }

        signature = Signature(BiometricMockTestData.getSignature())

        packageInfo = PackageInfo()
        packageInfo.packageName = "testPackageName"
        packageInfo.signatures = arrayOf(signature)

        packageManager = mock {
            on { getPackageInfo(any<String>(), org.mockito.ArgumentMatchers.anyInt()) } doReturn packageInfo
        }

        uafCallResult = mock {
            on { statusCode } doReturn BiometricAsyncHandler.OK
            on { result } doReturn "[{\"username\":\"username\"}]"
        }

        callResult = mock {
            on { statusCode } doReturn BiometricAsyncHandler.OK
            on { result } doReturn BiometricMockTestData.getTestRegResponse()
        }

        fingerprintContent = mock()
        fingerprintDialog = mock{
            on { generateFingerprintContent(true) } doReturn fingerprintContent
        }


        cryptoObject = mock{
            on { signature } doReturn mock<java.security.Signature>()
        }

        fidoKeystore = mock{
            on { getPublicKey("username") } doReturn BiometricMockTestData.getKey()!!
        }

        appWebInterface = mock()

        cookieService = mock{
            on { getAccessTokenFromCookie()} doReturn "accessToken"
        }

        fingerprintSystemChecker = mock {
            on { preRegistrationCheck() } doReturn true
        }

        fragmentActivity = mock {
            on { packageName } doReturn "testPackageName"
            on { packageManager } doReturn packageManager
        }

        biometricAsyncHandler = mock()
    }

    @Test
    fun WhenRegIsCalledRegistrationNotReadyThen10004Dispatched(){
        fingerprintSystemChecker = mock {
            on { preRegistrationCheck() } doReturn false
        }

        registrationService =
            RegistrationService(mock(), mock(), mock(),
                cookieService, mock(), mock(), mock(), fingerprintSystemChecker, preferencesService,
                mock(), appWebInterface){
                    _,_,_-> mock()
            }

        registrationService.startFidoRegistration()
        verify(appWebInterface).biometricCompletion(
            BiometricConstants.REGISTER,
            BiometricConstants.FAILURE,
            BiometricConstants.CANNOT_FIND_CODE)
    }

    @Test
    fun WhenRegIsCalledOnFingerprintAuthCallbackErrorsThen10005Dispatched(){
        registrationService =
            RegistrationService(fragmentActivity,
                biometricAsyncHandler,
                mock(),
                cookieService,
                mock(),
                mock(),
                fingerprintDialog,
                fingerprintSystemChecker,
                preferencesService,
                mock(),
                appWebInterface){_,_,_ -> mock()}

        registrationService.startFidoRegistration()

        val uafRegistrationMessageCaptor = argumentCaptor<(BiometricCallResult?) -> Unit>()
        verify(biometricAsyncHandler).fetchUafRegistrationMessage(eq(androidHash), eq("accessToken"), uafRegistrationMessageCaptor.capture())
        uafRegistrationMessageCaptor.firstValue(uafCallResult)

        val fingerprintAuthCallbackCaptor = argumentCaptor<FingerprintAuthCallback>()
        verify(fingerprintDialog).showFingerprintAuthDialog(fingerprintAuthCallbackCaptor.capture(), eq(fingerprintContent))

        fingerprintAuthCallbackCaptor.firstValue.error()
        verify(appWebInterface).biometricCompletion(
            BiometricConstants.REGISTER,
            BiometricConstants.FAILURE,
            BiometricConstants.CANNOT_CHANGE_CODE)
    }

    @Test
    fun WhenRegIsCalledOnFingerprintAuthCallbackCancelledThenCancelledDispatched(){
        registrationService =
            RegistrationService(fragmentActivity,
                biometricAsyncHandler,
                mock(),
                cookieService,
                mock(),
                mock(),
                fingerprintDialog,
                fingerprintSystemChecker,
                preferencesService,
                mock(),
                appWebInterface){_,_,_ -> mock()}

        registrationService.startFidoRegistration()

        val uafRegistrationMessageCaptor = argumentCaptor<(BiometricCallResult?) -> Unit>()
        verify(biometricAsyncHandler).fetchUafRegistrationMessage(eq(androidHash), eq("accessToken"), uafRegistrationMessageCaptor.capture())
        uafRegistrationMessageCaptor.firstValue(uafCallResult)

        val fingerprintAuthCallbackCaptor = argumentCaptor<FingerprintAuthCallback>()
        verify(fingerprintDialog).showFingerprintAuthDialog(fingerprintAuthCallbackCaptor.capture(), eq(fingerprintContent))

        fingerprintAuthCallbackCaptor.firstValue.cancel()
        verify(appWebInterface).biometricCompletion(
            BiometricConstants.REGISTER,
            BiometricConstants.CANCELLED,
            "")
    }

    @Test
    fun WhenRegIsCalledRegistrationIsSuccessfulThenSuccessDispatched(){

        registrationService =
            RegistrationService(fragmentActivity,
                biometricAsyncHandler,
                mock(),
                cookieService,
                mock(),
                fidoKeystore,
                fingerprintDialog,
                fingerprintSystemChecker,
                preferencesService,
                mock(),
                appWebInterface){_,_,_ -> mock()}

        registrationService.startFidoRegistration()

        //Capture callback for fetchUafRegistrationMessage and execute
        val uafRegistrationMessageCaptor = argumentCaptor<(BiometricCallResult?) -> Unit>()
        verify(biometricAsyncHandler).fetchUafRegistrationMessage(eq(androidHash), eq("accessToken"),
            uafRegistrationMessageCaptor.capture())
        uafRegistrationMessageCaptor.firstValue(uafCallResult)

        //Capture callback for fingerprintAuthCallback and execute
        val fingerprintAuthCallbackCaptor = argumentCaptor<FingerprintAuthCallback>()
        verify(fingerprintDialog).showFingerprintAuthDialog(fingerprintAuthCallbackCaptor.capture(), eq(fingerprintContent))
        fingerprintAuthCallbackCaptor.firstValue.processAuthentication(cryptoObject)

        //Capture callback for sendClientRegistrationMsg and execute
        verify(biometricAsyncHandler).sendClientRegistrationMsg(eq(BiometricMockTestData.getUAFMessage()), captor.capture())
        captor.firstValue(callResult)

        //Assert
        verify(appWebInterface).biometricCompletion(
            BiometricConstants.REGISTER,
            BiometricConstants.SUCCESS, "")
    }

    @Test
    fun WhenRegIsCalledSendClientRegistrationMsgErrorsThen10005Dispatched(){

        val callResult: BiometricCallResult = mock {
            on { statusCode } doReturn BiometricAsyncHandler.ERROR
            on { result } doReturn BiometricMockTestData.getTestRegResponse()
        }

        registrationService =
            RegistrationService(fragmentActivity,
                biometricAsyncHandler,
                mock(),
                cookieService,
                mock(),
                fidoKeystore,
                fingerprintDialog,
                fingerprintSystemChecker,
                preferencesService,
                mock(),
                appWebInterface){_,_,_ -> mock()}

        registrationService.startFidoRegistration()

        val uafRegistrationMessageCaptor = argumentCaptor<(BiometricCallResult?) -> Unit>()
        verify(biometricAsyncHandler).fetchUafRegistrationMessage(eq(androidHash),
            eq("accessToken"),
            uafRegistrationMessageCaptor.capture())
        uafRegistrationMessageCaptor.firstValue(uafCallResult)

        val fingerprintAuthCallbackCaptor = argumentCaptor<FingerprintAuthCallback>()
        verify(fingerprintDialog).showFingerprintAuthDialog(fingerprintAuthCallbackCaptor.capture(), eq(fingerprintContent))

        fingerprintAuthCallbackCaptor.firstValue.processAuthentication(cryptoObject)
        verify(biometricAsyncHandler).sendClientRegistrationMsg(eq(BiometricMockTestData.getUAFMessage()), captor.capture())

        captor.firstValue(callResult)
        verify(appWebInterface).biometricCompletion(
            BiometricConstants.REGISTER,
            BiometricConstants.FAILURE,
            BiometricConstants.CANNOT_CHANGE_CODE)
    }

}