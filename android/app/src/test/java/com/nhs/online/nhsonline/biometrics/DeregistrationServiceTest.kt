package com.nhs.online.nhsonline.biometrics

import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.biometrics.utils.BiometricConstants
import com.nhs.online.nhsonline.biometrics.utils.FingerprintCookieService
import com.nhs.online.nhsonline.biometrics.utils.FingerprintSharedPreferences
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import java.lang.ClassCastException

@RunWith(RobolectricTestRunner::class)
class DeregistrationServiceTest {
    private lateinit var preferencesService: FingerprintSharedPreferences
    private lateinit var cookieService: FingerprintCookieService
    private lateinit var appWebInterface: AppWebInterface
    private lateinit var deRegistrationService: DeRegistrationService
    private lateinit var biometricAsyncHandler: BiometricAsyncHandler

    @Before
    fun setUp() {

        preferencesService = mock {
            on { readStringFromSharedPref(BiometricConstants.APP_ID) } doReturn "appId"
            on { readStringFromSharedPref(BiometricConstants.KEY_ID) } doReturn "keyId"
        }

        appWebInterface = mock()

        biometricAsyncHandler = mock()

        cookieService = mock{
            on { getAccessTokenFromCookie()} doReturn "accessToken"
        }

        deRegistrationService =
            DeRegistrationService(mock(),
                preferencesService,
                mock(),
                biometricAsyncHandler,
                cookieService,
                appWebInterface)
    }

    @Test
    fun WhenDeRegIsCalledDeRegistrationIsSuccessfulThenSuccessDispatched(){
        //Act
        val callResult: BiometricCallResult = mock()
        val captor = argumentCaptor<(BiometricCallResult?) -> Unit>()
        deRegistrationService.deRegisterBiometrics()
        verify(biometricAsyncHandler).sendDeRegistrationOperation(eq("appId"), eq("keyId"),
            eq("accessToken"), captor.capture())

        captor.firstValue(callResult)

        //Assert
        verify(appWebInterface).biometricCompletion(
        BiometricConstants.DEREGISTER,
        BiometricConstants.SUCCESS,
        "")
    }

    @Test
    fun WhenDeRegIsCalledDeRegistrationThrowsExceptionThen10005Dispatched(){
        //Act
        preferencesService = mock {
            on { readStringFromSharedPref(BiometricConstants.APP_ID) } doThrow ClassCastException()
            on { readStringFromSharedPref(BiometricConstants.KEY_ID) } doThrow ClassCastException()
        }

        deRegistrationService =
            DeRegistrationService(mock(),
                preferencesService,
                mock(),
                mock(),
                cookieService,
                appWebInterface)

        deRegistrationService.deRegisterBiometrics()

        //Assert
        verify(appWebInterface).biometricCompletion(
            BiometricConstants.DEREGISTER,
            BiometricConstants.FAILURE,
            BiometricConstants.CANNOT_CHANGE_CODE)
    }

}