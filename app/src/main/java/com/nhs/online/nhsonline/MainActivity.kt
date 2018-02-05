package com.nhs.online.nhsonline

import android.os.Bundle
import android.support.design.widget.BottomNavigationView
import android.support.v7.app.AppCompatActivity
import com.nhsonline.nhsdigital.nhsonline.helpers.BottomNavigationViewHelper
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : AppCompatActivity() {
    private lateinit var bottomNavHelper: BottomNavigationViewHelper


    private val mOnNavigationItemSelectedListener = BottomNavigationView.OnNavigationItemSelectedListener { item ->
        when (item.itemId) {
            R.id.symptom -> {
                loadPage("https://111.nhs.uk")
                return@OnNavigationItemSelectedListener true
            }
            R.id.appointment -> {
                return@OnNavigationItemSelectedListener true
            }
            R.id.prescription -> {
                return@OnNavigationItemSelectedListener true
            }
            R.id.my_record -> {
                return@OnNavigationItemSelectedListener true
            }
            R.id.more -> {
                return@OnNavigationItemSelectedListener true
            }
        }
        false
    }


    private fun loadPage(url:String){
        webview.loadUrl(url)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        bottomNavHelper = BottomNavigationViewHelper
        bottomNavHelper.disableShiftMode(navigation)
        webview.settings.javaScriptEnabled = true
        webview.settings.domStorageEnabled = true
        navigation.setOnNavigationItemSelectedListener(mOnNavigationItemSelectedListener)
    }

}
