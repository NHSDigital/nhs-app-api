package com.nhs.online.nhsonline.activities

import android.view.View
import com.nhs.online.nhsonline.data.ErrorMessage
import kotlinx.android.synthetic.main.biometric_authentication_layout.*
import kotlinx.android.synthetic.main.biometric_layout_content.*
import kotlinx.android.synthetic.main.error_layout.*
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class BiometricTest {

    private lateinit var mockBiometricActivity: BiometricActivity

    @Before
    fun setUp(){
        mockBiometricActivity = Robolectric.setupActivity(BiometricActivity::class.java)
    }

    @Test
    fun showNativeScreen() {
        // Ensure native screen isn't shown at start
        val err = ErrorMessage("Test Error", null, null)
        mockBiometricActivity.showUnavailabilityError(err)


        // Should hide error view and show layout content
        mockBiometricActivity.showNativeScreen()
        assert(mockBiometricActivity.errorViewLayoutN.visibility == View.GONE)
        assert(mockBiometricActivity.biometricLayoutContent.visibility == View.VISIBLE)
    }

    @Test
    fun errorMessageNullMessage(){
        mockBiometricActivity.showNativeScreen()

        val err = ErrorMessage("Test Error", null, null)
        mockBiometricActivity.showUnavailabilityError(err)

        // Check error screen shown
        assert(mockBiometricActivity.errorViewLayoutN.visibility == View.VISIBLE)
        assert(mockBiometricActivity.biometricLayoutContent.visibility == View.GONE)

        assert(mockBiometricActivity.tryAgainTextView.visibility == View.VISIBLE)
    }

    @Test
    fun errorMessageNonNullMessage(){
        mockBiometricActivity.showNativeScreen()

        val err = ErrorMessage("Test Error", "Test Message", null)
        mockBiometricActivity.showUnavailabilityError(err)

        // Check error screen shown
        assert(mockBiometricActivity.errorViewLayoutN.visibility == View.VISIBLE)
        assert(mockBiometricActivity.biometricLayoutContent.visibility == View.GONE)

        assert(mockBiometricActivity.tryAgainTextView.visibility == View.GONE)
    }


}