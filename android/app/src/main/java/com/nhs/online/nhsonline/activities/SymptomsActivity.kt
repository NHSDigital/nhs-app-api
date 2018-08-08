package com.nhs.online.nhsonline.activities

import android.net.Uri
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.support.v7.widget.Toolbar
import android.view.MenuItem
import android.view.View
import android.webkit.WebViewClient
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.services.KnownServices
import com.nhs.online.nhsonline.webclients.ChromeClientLocationHandler
import com.nhs.online.nhsonline.webclients.UnsecureWebClient
import kotlinx.android.synthetic.main.check_my_symptoms_banner.*
import java.net.URL

class SymptomsActivity : AppCompatActivity() {

    private lateinit var chromeClient: ChromeClientLocationHandler
    private lateinit var knownServices: KnownServices

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.check_my_symptoms_banner)
        val toolbar = findViewById<View>(R.id.toolbar) as Toolbar
        setSupportActionBar(toolbar)
        supportActionBar!!.setHomeButtonEnabled(true)
        supportActionBar!!.setDisplayHomeAsUpEnabled(true)
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)

        configureWebView()
        val urlPath = resources.getString(R.string.baseURL)+resources.getString(R.string.checkYourSymptoms)+resources.getString(R.string.nhsOnlineRequiredQueries)

        loadPage(urlPath)
    }

    private fun configureWebView() {
        symptomsWebview.settings.javaScriptEnabled = true
        symptomsWebview.settings.domStorageEnabled = true

        chromeClient = ChromeClientLocationHandler(this)
        symptomsWebview.webChromeClient = chromeClient

        knownServices = KnownServices(this)
        symptomsWebview.webViewClient =  UnsecureWebClient(this, this)
    }

    private fun loadPage(url: String) {
        val urlWithMissingQueryStrings =
                knownServices.findKnownServiceAddMissingQueryFor(url)

        symptomsWebview.loadUrl(urlWithMissingQueryStrings)
    }

    fun setHeaderText(text: String) {
        runOnUiThread {
            toolbar.title = text
            symptomsWebview.announceForAccessibility(text)
        }
    }
    override fun onOptionsItemSelected(menuItem: MenuItem): Boolean {
        if (symptomsWebview.canGoBack()) {
            symptomsWebview.goBack()
        } else {
            // Otherwise defer to system default behavior.
            super.onBackPressed()
        }
        return true
    }
}



