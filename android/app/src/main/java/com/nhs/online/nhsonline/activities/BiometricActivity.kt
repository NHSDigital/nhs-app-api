package com.nhs.online.nhsonline.activities

import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.app.AppCompatDelegate
import android.support.v7.widget.Toolbar
import android.view.MenuItem
import android.view.View
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.support.setServiceError
import kotlinx.android.synthetic.main.biometric_authentication_layout.*
import kotlinx.android.synthetic.main.biometric_layout_content.*
import kotlinx.android.synthetic.main.error_layout.*


class BiometricActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.biometric_authentication_layout)
        val toolbar = findViewById<View>(R.id.toolbar) as Toolbar
        setSupportActionBar(toolbar)
        supportActionBar!!.title = null
        supportActionBar!!.setHomeButtonEnabled(true)
        supportActionBar!!.setDisplayHomeAsUpEnabled(true)
        AppCompatDelegate.setCompatVectorFromResourcesEnabled(true)
    }

    override fun onOptionsItemSelected(menuItem: MenuItem): Boolean {
            super.onBackPressed()
            return true;
    }

    fun showUnavailabilityError(unavailabilityErrorMessage: ErrorMessage) {
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
        errorViewLayoutN.visibility = View.VISIBLE
        biometricLayoutContent.visibility = View.GONE
    }

    fun showNativeScreen() {
        errorViewLayoutN.visibility = View.GONE
        biometricLayoutContent.visibility = View.VISIBLE
    }
}