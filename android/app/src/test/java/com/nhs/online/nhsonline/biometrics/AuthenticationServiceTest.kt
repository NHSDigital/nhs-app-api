package com.nhs.online.nhsonline.biometrics

import android.content.pm.PackageInfo
import android.content.pm.PackageManager
import android.content.pm.Signature
import android.support.v4.app.FragmentActivity
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.fidoclient.exceptions.FidoInvalidSignatureException
import com.nhs.online.fidoclient.uaf.client.operation.Authentication
import com.nhs.online.nhsonline.biometrics.utils.BiometricMockTestData.getSignature
import com.nhs.online.nhsonline.biometrics.utils.BiometricMockTestData.getUAFMessage
import com.nhs.online.nhsonline.biometrics.utils.*
import com.nhs.online.nhsonline.webinterfaces.AppWebInterface
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.ArgumentMatchers
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)

class AuthenticationServiceTest {
    private lateinit var authenticationService: AuthenticationService
    private lateinit var fragmentActivityMock: FragmentActivity
    private lateinit var biometricAsyncHandlerMock: BiometricAsyncHandler
    private lateinit var biometricsInteractorMock: BiometricsInteractor
    private lateinit var biometricStateMock: BiometricState
    private lateinit var biometricCleanupHelperMock: BiometricCleanupHelper
    private lateinit var fingerprintDialogMock: FingerprintDialog
    private lateinit var fingerprintSystemCheckerMock: FingerprintSystemChecker
    private lateinit var preferencesServiceMock: FingerprintSharedPreferences
    private lateinit var authenticationMock: Authentication
    private lateinit var appWebInterfaceMock: AppWebInterface

    private lateinit var packageManager: PackageManager
    private lateinit var packageInfo: PackageInfo
    private lateinit var signature: Signature


    @Before
    fun setUp() {
        biometricsInteractorMock = mock()

        authenticationMock = mock {
            on {
                auth(any(), any()
                )
            } doThrow (FidoInvalidSignatureException::class)
        }

        preferencesServiceMock = mock {
            on { getFidoUsername() } doReturn "username"
            on { readStringFromSharedPref(anyVararg()) } doReturn ""
        }

        signature = Signature(getSignature())

        packageInfo = PackageInfo()
        packageInfo.packageName = "testPackageName"
        packageInfo.signatures = arrayOf(signature)

        packageManager = mock {
            on { getPackageInfo(any<String>(), ArgumentMatchers.anyInt()) } doReturn packageInfo
        }
        
        fragmentActivityMock = mock {
            on { packageName } doReturn "testPackageName"
            on { packageManager } doReturn packageManager
        }
        biometricAsyncHandlerMock = mock()
        biometricStateMock = mock()
        biometricCleanupHelperMock = mock()

        fingerprintDialogMock = mock()

        fingerprintSystemCheckerMock = mock {
            on { preLoginCheck() } doReturn true
        }

        appWebInterfaceMock = mock()

        authenticationService =
            AuthenticationService(
                fragmentActivityMock,
                biometricAsyncHandlerMock,
                biometricsInteractorMock,
                biometricStateMock,
                biometricCleanupHelperMock,
                fingerprintDialogMock,
                fingerprintSystemCheckerMock,
                preferencesServiceMock,
                authenticationMock,
                appWebInterfaceMock)
    }

    @Test
    fun startFidoSignIn_ReturnsTrueWhenRegisteredAndNotStarted() {
        authenticationService.isFingerprintLoginStarted = false
        whenever(biometricStateMock.registered).thenReturn(true)

        val result = authenticationService.showBiometricLoginIfEnabled(false)

        Assert.assertTrue(result)
    }

    @Test
    fun isFingerprintLoginStarted_isFalse_WhenForceStartIsTrue() {
        authenticationService.isFingerprintLoginStarted = true
        authenticationService.showBiometricLoginIfEnabled(true)
        Assert.assertFalse(authenticationService.isFingerprintLoginStarted)
    }

    @Test
    fun isFingerprintLoginStarted_remainsTrue_WhenForceStartIsFalse() {
        authenticationService.isFingerprintLoginStarted = true
        authenticationService.showBiometricLoginIfEnabled(false)
        Assert.assertTrue(authenticationService.isFingerprintLoginStarted)
    }


    @Test
    fun startFidoSignIn_ReturnsFalseWhenAlreadyStarted() {
        authenticationService.isFingerprintLoginStarted = true
        whenever(biometricStateMock.registered).thenReturn(true)

        val result = authenticationService.showBiometricLoginIfEnabled(false)

        Assert.assertFalse(result)
    }

    @Test
    fun completeSignInStart_Successful() {
        val response: BiometricCallResult = mock{
            on { result } doReturn getUAFMessage()
            on { statusCode } doReturn BiometricAsyncHandler.OK
        }

        authenticationService.isFingerprintLoginStarted = true

        whenever(response.result).thenReturn("Test")

        authenticationService.completeSignInStart(response)

        Assert.assertTrue(authenticationService.isFingerprintLoginStarted)
    }

    @Test
    fun completeSignInStart_handlesIllegalStateException() {
        val response: BiometricCallResult = mock{
            on { result } doReturn getUAFMessage()
            on { statusCode } doReturn BiometricAsyncHandler.OK
        }

        authenticationService.isFingerprintLoginStarted = true

        whenever(fingerprintDialogMock.showFingerprintAuthDialog(anyVararg(),
            anyVararg())).thenThrow(IllegalStateException())

        authenticationService.completeSignInStart(response)

        verify(appWebInterfaceMock).biometricLoginFailure()
        Assert.assertFalse(authenticationService.isFingerprintLoginStarted)
    }
}