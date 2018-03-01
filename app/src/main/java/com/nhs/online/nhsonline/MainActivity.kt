package com.nhs.online.nhsonline

import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import com.nhs.online.nhsonline.navigation.MenuBarItem
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)
        setContentView(R.layout.activity_main)

        configureWebView()
        menuBar.menuItemSelectedListener = { menuBarItem -> onMenuSelected(menuBarItem) }
        loadWelcomePage()
    }

    private fun configureWebView() {
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true
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
}

