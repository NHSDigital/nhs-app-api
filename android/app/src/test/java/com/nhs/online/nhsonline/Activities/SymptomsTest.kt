package com.nhs.online.nhsonline.activities

import android.view.View
import com.nhaarman.mockito_kotlin.verify
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.support.setServiceError
import kotlinx.android.synthetic.main.check_my_symptoms_banner.*
import kotlinx.android.synthetic.main.error_layout.*
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class SymptomsTest {

    private lateinit var symptomsActivity: SymptomsActivity

    @Before
    fun setUp() {
        symptomsActivity = Robolectric.setupActivity(SymptomsActivity::class.java)
    }

    @Test
    fun showProgressDialog() {
        symptomsActivity.dismissProgressDialog()

        symptomsActivity.showProgressDialog()
        assert(symptomsActivity.progressBarLayoutU.visibility == View.VISIBLE)
    }

    @Test
    fun hideProgressDialog() {
        symptomsActivity.showProgressDialog()

        symptomsActivity.dismissProgressDialog()
        assert(symptomsActivity.progressBarLayoutU.visibility == View.GONE)
    }

    @Test
    fun setHeaderText() {
        symptomsActivity.setHeaderText("Check Symptoms with App", "Look up your symptoms online")

        assert(symptomsActivity.header_text_view.text == "Check Symptoms with App")
        assert(symptomsActivity.header_text_view.contentDescription == "Look up your symptoms online")
    }

    @Test
    fun setHeaderTextNullDescription() {
        symptomsActivity.setHeaderText("Check Symptoms with App", null)

        assert(symptomsActivity.header_text_view.text == "Check Symptoms with App")
        assert(symptomsActivity.header_text_view.contentDescription == null)
    }

    @Test
    fun errorMessageNullMessage(){
        symptomsActivity.showWebviewScreen()

        val err = ErrorMessage("Device has no connection", null, null)
        symptomsActivity.showUnavailabilityError(err)

        // Check error screen shown
        assert(symptomsActivity.errorViewLayoutU.visibility == View.VISIBLE) {}
        assert(symptomsActivity.symptomsWebview.visibility == View.GONE)

        assert(symptomsActivity.tryAgainTextView.visibility == View.VISIBLE)

        // checks set service error is called
        assert(symptomsActivity.errorTextView.text.toString().contains("Device has no connection"))
    }

    @Test
    fun errorMessage(){
        symptomsActivity.showWebviewScreen()

        val err = ErrorMessage("Device has no connection",
                "Check your device connectivity",
                null)
        symptomsActivity.showUnavailabilityError(err)

        // Check error screen shown
        assert(symptomsActivity.errorViewLayoutU.visibility == View.VISIBLE)
        assert(symptomsActivity.symptomsWebview.visibility == View.GONE)

        assert(symptomsActivity.tryAgainTextView.visibility == View.GONE)

        assert(symptomsActivity.errorTextView.text.toString().contains("Device has no connection"))
    }

    @Test
    fun showWebViewScreen() {
        // Set up to ensure error view is visible
        val err = ErrorMessage("Device has no connection",
                null,
                null)
        symptomsActivity.showUnavailabilityError(err)


        symptomsActivity.showWebviewScreen()

        assert(symptomsActivity.errorViewLayoutU.visibility == View.GONE)
        assert(symptomsActivity.symptomsWebview.visibility == View.VISIBLE)
    }
}