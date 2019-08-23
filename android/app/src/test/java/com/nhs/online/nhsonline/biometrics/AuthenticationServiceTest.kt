package com.nhs.online.nhsonline.biometrics

import android.content.pm.PackageInfo
import android.content.pm.PackageManager
import android.content.pm.Signature
import android.support.v4.app.FragmentActivity
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.fidoclient.exceptions.FidoInvalidSignatureException
import com.nhs.online.fidoclient.interfaces.IBiometricsInteractor
import com.nhs.online.fidoclient.uaf.client.operation.Authentication
import com.nhs.online.nhsonline.biometrics.utils.BiometricCleanupHelper
import com.nhs.online.nhsonline.biometrics.utils.FingerprintDialog
import com.nhs.online.nhsonline.biometrics.utils.FingerprintSharedPreferences
import com.nhs.online.nhsonline.biometrics.utils.FingerprintSystemChecker
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.mockito.ArgumentMatchers
import org.robolectric.RobolectricTestRunner
import java.lang.IllegalStateException

@RunWith(RobolectricTestRunner::class)

class AuthenticationServiceTest {
    private lateinit var authenticationService: AuthenticationService
    private lateinit var mockFragmentActivity: FragmentActivity
    private lateinit var mockBiometricAsyncHandler: BiometricAsyncHandler
    private lateinit var mockBiometricsInteractor: IBiometricsInteractor
    private lateinit var mockBiometricState: BiometricState
    private lateinit var mockBiometricCleanupHelper: BiometricCleanupHelper
    private lateinit var mockFingerprintDialog: FingerprintDialog
    private lateinit var mockFingerprintSystemChecker: FingerprintSystemChecker
    private lateinit var mockPreferencesService: FingerprintSharedPreferences
    private lateinit var mockAuthentication: Authentication

    private lateinit var packageManager: PackageManager
    private lateinit var packageInfo: PackageInfo
    private lateinit var signature: Signature


    @Before
    fun setUp() {
        mockBiometricsInteractor = mock()

        mockAuthentication = mock {
            on {
                auth(any(), any()
                )
            } doThrow (FidoInvalidSignatureException::class)
        }

        mockPreferencesService = mock {
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


        mockFragmentActivity = mock {
            on { packageName } doReturn "testPackageName"
            on { packageManager } doReturn packageManager
        }
        mockBiometricAsyncHandler = mock()
        mockBiometricState = mock()
        mockBiometricCleanupHelper = mock()

        mockFingerprintDialog = mock()

        mockFingerprintSystemChecker = mock {
            on { preLoginCheck() } doReturn true
        }

        authenticationService = AuthenticationService(
            mockFragmentActivity,
            mockBiometricAsyncHandler,
            mockBiometricsInteractor,
            mockBiometricState,
            mockBiometricCleanupHelper,
            mockFingerprintDialog,
            mockFingerprintSystemChecker,
            mockPreferencesService,
            mockAuthentication
        )
    }

    @Test
    fun startFidoSignIn_ReturnsTrueWhenRegisteredAndNotStarted() {
        authenticationService.isFingerprintLoginStarted = false
        whenever(mockBiometricState.registered).thenReturn(true)

        val result = authenticationService.showBiometricLoginIfEnabled()

        Assert.assertTrue(result)
    }

    @Test
    fun startFidoSignIn_ReturnsFalseWhenAlreadyStarted() {
        authenticationService.isFingerprintLoginStarted = true
        whenever(mockBiometricState.registered).thenReturn(true)

        val result = authenticationService.showBiometricLoginIfEnabled()

        Assert.assertFalse(result)
    }

    @Test
    fun completeSignInStart_Successful() {
        val response: BiometricCallResult = mock()
        authenticationService.isFingerprintLoginStarted = true

        whenever(response.result).thenReturn("Test")

        authenticationService.completeSignInStart(response)

        Assert.assertTrue(authenticationService.isFingerprintLoginStarted)
    }

    @Test
    fun completeSignInStart_handlesIllegalStateException() {
        val response: BiometricCallResult = mock()
        authenticationService.isFingerprintLoginStarted = true

        whenever(response.result).thenReturn("Test")
        whenever(mockFingerprintDialog.showFingerprintAuthDialog(anyVararg(),
            anyVararg())).thenThrow(IllegalStateException())

        authenticationService.completeSignInStart(response)

        Assert.assertFalse(authenticationService.isFingerprintLoginStarted)
    }


    private fun getSignature(): String {
        return "308201dd30820146020101300d06092a864886f70d010105050030373116301406035504030c0d416e64726f69642044656275673110300e060355040a0c07416e64726f6964310b3009060355040613025553301e170d3138303733313038323930365a170d3438303732333038323930365a30373116301406035504030c0d416e64726f69642044656275673110300e060355040a0c07416e64726f6964310b300906035504061302555330819f300d06092a864886f70d010101050003818d0030818902818100d68bb09bc83dcf88ef5d4120d753e2df881ba938358b865206380c40b5dff779ffa51e7244fb74edbfbff7e44cc4485849280d5c7299a872592ccacdf4daa1e09e0200ad74acbe4858ff320906034ef21c0fd467c71c0a0b1cb39ea58700d54f2b4976f2fbae6c381cea85d9379a825c70c139dbfe9daf25013407fc9e50f1bb0203010001300d06092a864886f70d010105050003818100a1cdabb8310ef0dac7cc688f1fc6f4de1d25c3b666c0f70211f836629603ea7241a458c9506bfd4677c7a2de67f38f5259dbb36ad4094154451985fe6fa00e7ac9c929b4762bb855ddbf245fd898051987de32feee42c6e586914d26854a0c5b1431302074e2c31075e2e8979b3c35b5daa664edd200ea82bd3bfed1c0568df2"
    }

}