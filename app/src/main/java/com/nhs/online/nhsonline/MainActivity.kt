package com.nhs.online.nhsonline

import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import com.nhs.online.nhsonline.bmenu.MenuBar
import com.nhs.online.nhsonline.bmenu.MenuBarItem
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : AppCompatActivity() {
    private var currentUrl = "https://111.nhs.uk"
    private val mOnMenuItemSelectListener = object : MenuBar.OnCustomMenuItemSelectedListener {
        override fun onItemSelected(item: MenuBarItem) {
            when (item.id) {
                R.id.symptoms -> {
                    if (currentUrl == "https://111.nhs.uk") return
                    currentUrl = "https://111.nhs.uk"
                    loadPage(currentUrl)
                }
                R.id.appointments -> {

                }
                R.id.prescriptions -> {

                }
                R.id.myRecord -> {

                }
                R.id.more -> {

                }
            }
        }

    }


    private fun loadPage(url: String) {
        webview.loadUrl(url)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)
        setContentView(R.layout.activity_main)
        customMenu.setOnMenuItemSelectListener(mOnMenuItemSelectListener)
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true
        loadPage(currentUrl)
    }

}
