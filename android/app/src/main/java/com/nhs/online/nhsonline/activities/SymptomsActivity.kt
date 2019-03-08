package com.nhs.online.nhsonline.activities

import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.support.v7.widget.Toolbar
import android.view.MenuItem
import android.view.View
import android.view.WindowManager
import android.widget.TextView
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.interfaces.UnsecureInteractor
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.support.setServiceError
import com.nhs.online.nhsonline.webclients.ChromeClientLocationHandler
import com.nhs.online.nhsonline.webclients.UnsecureWebClient
import kotlinx.android.synthetic.main.check_my_symptoms_banner.*
import kotlinx.android.synthetic.main.error_layout.*

class SymptomsActivity : UnsecureInteractor, AppCompatActivity() {

    private lateinit var chromeClient: ChromeClientLocationHandler
    private lateinit var knownServices: KnownServices
    private var reloadUrl: String? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.check_my_symptoms_banner)
        val toolbar = findViewById<View>(R.id.toolbar) as Toolbar
        setSupportActionBar(toolbar)
        supportActionBar!!.title = null
        supportActionBar!!.setHomeButtonEnabled(true)
        supportActionBar!!.setDisplayHomeAsUpEnabled(true)
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)
        retryButton.setOnClickListener { reloadRequest() }
        configureWebView()
        val urlPath =
            resources.getString(R.string.baseURL) + resources.getString(R.string.checkYourSymptoms) + resources.getString(
                R.string.nhsOnlineRequiredQueries)

        loadPage(urlPath)
    }

    private fun configureWebView() {
        symptomsWebview.settings.javaScriptEnabled = true
        symptomsWebview.settings.domStorageEnabled = true

        chromeClient = ChromeClientLocationHandler(this)
        symptomsWebview.webChromeClient = chromeClient

        knownServices = KnownServices(this)
        symptomsWebview.webViewClient = UnsecureWebClient(this, knownServices, this)
    }

    private fun loadPage(url: String) {
        val urlWithMissingQueryStrings =
            knownServices.findKnownServiceAndAddMissingQueryFor(url)

        symptomsWebview.loadUrl(urlWithMissingQueryStrings)
    }

    private fun reloadRequest() {
        showProgressDialog()
        if (reloadUrl != null) {
            symptomsWebview.loadUrl(reloadUrl)
        } else {
            symptomsWebview.reload()
        }
    }

    override fun setReloadUrl(url: String?) {
        reloadUrl = url
    }

    override fun showProgressDialog() {
        if (progressBarLayoutU.visibility == View.GONE) {
            progressBarLayoutU.visibility = View.VISIBLE
            window.setFlags(WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE,
                WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE)
        }
    }

    override fun dismissProgressDialog() {
        if (progressBarLayoutU.visibility == View.VISIBLE) {
            window.clearFlags(WindowManager.LayoutParams.FLAG_NOT_TOUCHABLE)
            progressBarLayoutU.visibility = View.GONE
        }
    }

    override fun setHeaderText(text: String, description: String?) {
        val toolbar = findViewById<View>(R.id.toolbar) as Toolbar
        val headerText = toolbar.findViewById<View>(R.id.header_text_view) as TextView
        runOnUiThread {
            headerText.text = text
            if (!description.isNullOrEmpty()) {
                headerText.contentDescription = description
            }
            symptomsWebview.announceForAccessibility(text)
        }
    }

    override fun onOptionsItemSelected(menuItem: MenuItem): Boolean {
        if (symptomsWebview.canGoBack()) {
            symptomsWebview.goBack()
        } else if (reloadUrl != null) {
            if (isCheckSymptomsUnsecureURL(reloadUrl!!)) {
                val urlPath =
                    resources.getString(R.string.baseURL) + resources.getString(R.string.checkYourSymptoms) + resources.getString(
                        R.string.nhsOnlineRequiredQueries)
                reloadUrl = urlPath
                loadPage(urlPath)
            } else {
                super.onBackPressed()
            }
        } else {
            super.onBackPressed()
        }
        return true
    }

    private fun isCheckSymptomsUnsecureURL(failedURL: String): Boolean {
        val unsecuredKnownServiceInfo = knownServices.findMatchingServiceInfo(failedURL)
        unsecuredKnownServiceInfo?.header?.let { nativeHeader ->
            return when (nativeHeader) {
                resources.getString(R.string.nhs_111_header),
                resources.getString(R.string.conditions_header) -> true
                else -> false
            }
        }
        return false
    }

    override fun showUnavailabilityError(unavailabilityErrorMessage: ErrorMessage) {
        showErrorScreen()
        errorTextView.setServiceError(unavailabilityErrorMessage.title,
            unavailabilityErrorMessage.message)
        if (unavailabilityErrorMessage.message != null) {
            tryAgainTextView.visibility = View.GONE
        } else {
            tryAgainTextView.visibility = View.VISIBLE
        }
    }

    private fun showErrorScreen() {
        errorViewLayoutU.visibility = View.VISIBLE
        symptomsWebview.visibility = View.GONE
    }

    override fun showWebviewScreen() {
        errorViewLayoutU.visibility = View.GONE
        symptomsWebview.visibility = View.VISIBLE
    }
}



