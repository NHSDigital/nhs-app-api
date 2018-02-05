package com.nhs.online.nhsonline

import android.os.Bundle
import android.support.design.widget.BottomNavigationView
import android.support.v7.app.AppCompatActivity
import android.util.Log
import android.widget.Toast
import com.nhsonline.nhsdigital.nhsonline.helpers.BottomNavigationViewHelper
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : AppCompatActivity() {
    private lateinit var bottomNavHelper: BottomNavigationViewHelper
    private val loginUrl = "https://keycloak.sandbox.signin.nhs.uk/cicauth/realms/NHS/protocol/openid-connect/auth?client_id=account&redirect_uri=https%3A%2F%2Fkeycloak.sandbox.signin.nhs.uk%2Fcicauth%2Frealms%2FNHS%2Faccount%2Flogin-redirect&state=0%2F1568c175-dcba-4e88-b9fb-4b0d7bddb2a9&response_type=code&scope=openid"


    private val mOnNavigationItemSelectedListener = BottomNavigationView.OnNavigationItemSelectedListener { item ->
        when (item.itemId) {
            R.id.symptom -> {
                loadPage("https://111.nhs.uk")
                return@OnNavigationItemSelectedListener true
            }
            R.id.appointment -> {
                loadPage(loginUrl)
                return@OnNavigationItemSelectedListener true
            }
            R.id.prescription -> {
                showToastMessage(R.string.testtext)
                return@OnNavigationItemSelectedListener true
            }
            R.id.my_record -> {
                loadLocalPage(R.string.baseURL)
                return@OnNavigationItemSelectedListener true
            }
            R.id.more -> {
                return@OnNavigationItemSelectedListener true
            }
        }
        false
    }

    private fun showToastMessage(messageId:Int ) {
        Toast.makeText(this, resources.getString(messageId), Toast.LENGTH_LONG).show()
    }

    private fun loadPage(url:String){
        webview.loadUrl(url)
    }

    private fun loadLocalPage(redId:Int){
        val url = resources.getString(redId)
        Log.d("Main Activity","The url is: $url")
        webview.loadUrl(resources.getString(redId))
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
