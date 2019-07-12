package com.nhs.online.nhsonline.biometrics.utils

import android.support.v4.hardware.fingerprint.FingerprintManagerCompat
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.interfaces.IInteractor
import org.junit.Assert
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)

class FingerprintSystemCheckerTest {
    private lateinit var fingerprintSystemChecker: FingerprintSystemChecker
    private lateinit var fingerprintManagerMock: FingerprintManagerCompat
    private lateinit var contextMock: MainActivity
    private lateinit var interactorMock: IInteractor

    @Before
    fun setUp() {
        contextMock = mock()
        fingerprintManagerMock = mock()
        interactorMock = mock()

    }

    @Test
    fun hardwareNotSupported() {
        createManagerMock(false, true)

        fingerprintSystemChecker =
                FingerprintSystemChecker(
                    fingerprintManagerMock,
                    contextMock, interactorMock)
        val result = fingerprintSystemChecker.checkIfHardwareSupported()

        Assert.assertFalse(result)
    }

    @Test
    fun hardwareIsSupported() {
        createManagerMock(true, false)

        fingerprintSystemChecker =
                FingerprintSystemChecker(
                    fingerprintManagerMock,
                    contextMock, interactorMock)
        val result = fingerprintSystemChecker.checkIfHardwareSupported()

        Assert.assertTrue(result)
    }

    @Test
    fun hasFingerprintsEnrolled() {
        createManagerMock(false, true)

        fingerprintSystemChecker =
                FingerprintSystemChecker(
                    fingerprintManagerMock,
                    contextMock, interactorMock)
        val result = fingerprintSystemChecker.checkIfFingerprintsExist()

        Assert.assertTrue(result)
    }

    @Test
    fun noFingerprintsEnrolled() {
        createManagerMock(true, false)

        fingerprintSystemChecker =
                FingerprintSystemChecker(
                    fingerprintManagerMock,
                    contextMock, interactorMock)
        val result = fingerprintSystemChecker.checkIfFingerprintsExist()

        Assert.assertFalse(result)
    }

    private fun createManagerMock(hasHardware: Boolean, hasEnrolledFingerprints: Boolean) {
        this.fingerprintManagerMock = mock {
            on { isHardwareDetected } doReturn hasHardware
            on { hasEnrolledFingerprints() } doReturn hasEnrolledFingerprints
        }
    }

}