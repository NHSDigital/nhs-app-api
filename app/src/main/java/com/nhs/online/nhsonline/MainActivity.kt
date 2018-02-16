package com.nhs.online.nhsonline

import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.view.View
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.navigation.MenuBarItem
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : IInteractor, AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)

        configureWebView()
        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }
        retryButton.setOnClickListener { onSymptomMenuSelected() }
        loadWelcomePage()
    }

    private fun configureWebView() {
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true
        webview.webViewClient = WebClientInterceptor(this, resources.getStringArray(R.array.serviceUrls))
    }

    private fun onMenuSelected(menuBarItem: MenuBarItem) {
        when (menuBarItem.id) {
            R.id.symptoms -> onSymptomMenuSelected()
            else -> loadWelcomePage()
        }
    }

    private fun onSymptomMenuSelected() = loadPage(resources.getString(R.string.nhs111))

    private fun loadWelcomePage() = loadPage(resources.getString(R.string.baseURL))

    private fun loadPage(url: String) {
        webview.loadUrl(url)
    }

    override fun showProgressDialog() {
        if (progressBarLayout.visibility == View.GONE)
            progressBarLayout.visibility = View.VISIBLE
    }

    override fun dismissProgressDialog() {
        if (progressBarLayout.visibility == View.VISIBLE)
            progressBarLayout.visibility = View.GONE
    }

    override fun selectSymptomsMenuActive() {
        menuBar.switchActiveMenuItemTo(R.id.symptoms)
    }

    override fun showUnavailabilityError() {
        showErrorScreen()

        val errorMessage = resources.getString(R.string.nhs111_connection_error)
        errorTextView.text = errorMessage

    }

    private fun showErrorScreen() {
        errorViewLayout.visibility = View.VISIBLE
        webview.visibility = View.GONE
    }

    override fun showWebviewScreen() {
        errorViewLayout.visibility = View.GONE
        webview.visibility = View.VISIBLE
    }

}

