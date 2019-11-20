package com.nhs.online.nhsonline.biometrics

import android.os.Build
import android.support.v4.hardware.fingerprint.FingerprintManagerCompat
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.fidoclient.exceptions.FidoInvalidSignatureException
import com.nhs.online.fidoclient.interfaces.IBiometricsInteractor
import com.nhs.online.fidoclient.uaf.client.operation.Authentication
import com.nhs.online.fidoclient.uaf.crypto.FidoKeystoreAndroidM
import com.nhs.online.fidoclient.utils.extractJSONString
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.biometrics.utils.FingerprintSharedPreferences
import com.nhs.online.nhsonline.interfaces.IInteractor
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.util.ReflectionHelpers
import java.security.Signature


@RunWith(RobolectricTestRunner::class)

class FingerprintServiceTest {
    private lateinit var biometricsInteractor: IBiometricsInteractor
    private lateinit var fidoEndpointConfig: FidoEndpointConfig
    private lateinit var mockSignature: Signature
    private lateinit var mockAuthentication: Authentication
    private lateinit var fingerprintService: FingerprintService
    private lateinit var mockPreferencesService: FingerprintSharedPreferences

    private val fidoServerUrl = "\"test@test.com\""

    @Before
    fun setUp() {
        biometricsInteractor = Robolectric.setupActivity(MainActivity::class.java)
        fidoEndpointConfig = FidoEndpointConfig(
            "test@test.com",
            "/authGet",
            "/dereg",
            "/regGet",
            "/regPost"
        )

        mockSignature = mock()

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

        val fidoKeystore: FidoKeystoreAndroidM = mock()
        val interactor: IInteractor = mock()

        fingerprintService = FingerprintService(biometricsInteractor, interactor,
            fidoKeystore,
            mock(),
            mockPreferencesService,
            fidoEndpointConfig, mockAuthentication)
    }

    @Test
    fun fingerprintServiceCompanionClass_ReturnsNonNullWhenApiVersionEqualOrHigherThan23() {
        val marshmallowOrHigherApis =
            listOf(Build.VERSION_CODES.M, Build.VERSION_CODES.N, Build.VERSION_CODES.O)
        val interactor: IInteractor = mock()

        marshmallowOrHigherApis.forEach { version ->
            setAndroidApiVersion(version)
            val fingerprintService =
                FingerprintService.createIfDeviceSupported(biometricsInteractor, fidoServerUrl, interactor)
            Assert.assertNotNull(fingerprintService)
        }
    }

    @Test
    fun fingerprintServiceCompanionClass_ReturnsNullWhenApiVersionLessThan23() {
        val lollipopOrLowerApis =
            listOf(Build.VERSION_CODES.LOLLIPOP_MR1,
                Build.VERSION_CODES.LOLLIPOP,
                Build.VERSION_CODES.KITKAT)
        val interactor: IInteractor = mock()

        lollipopOrLowerApis.forEach { version ->
            setAndroidApiVersion(version)
            val fingerprintService =
                FingerprintService.createIfDeviceSupported(biometricsInteractor, fidoServerUrl, interactor)
            Assert.assertNull(fingerprintService)
        }
    }

    @Test
    fun extractsMessageCorrectly() {
        val uafMsg = "{\"uafProtocolMessage\":\"uafProtocolMessage\"}"
        val extractedMessage = uafMsg.extractJSONString("uafProtocolMessage")
        Assert.assertEquals("uafProtocolMessage", extractedMessage)
    }

    @Test()
    fun returnEmptyStringWhenInvalidFingerprintCaught() {
        val authenticationService = AuthenticationService(mock(), mock(),
            biometricsInteractor, mock(), mock(), mock(), mock(),
            mockPreferencesService, mockAuthentication)

        val result = authenticationService.processUafLoginMsg(FingerprintManagerCompat.CryptoObject(
            mockSignature), "{Error}")

        Assert.assertEquals("", result)
    }

    private fun setAndroidApiVersion(version: Int) {
        ReflectionHelpers.setStaticField(
            Build.VERSION::class.java.getField("SDK_INT"), version)
    }

}
