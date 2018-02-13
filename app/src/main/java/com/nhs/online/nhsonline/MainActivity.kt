package com.nhs.online.nhsonline

import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)
        setContentView(R.layout.activity_main)

        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true
        loadPage(resources.getString(R.string.baseURL))
    }

    private fun loadPage(url: String) {
        webview.loadUrl(url)
    }
}
