package com.nhs.online.nhsonline.activities

import kotlinx.android.synthetic.main.activity_main.*
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import android.app.onResume
import com.nhs.online.nhsonline.R
import junit.framework.Assert.assertEquals
import org.robolectric.Robolectric


@RunWith(RobolectricTestRunner::class)
class MainActivityTest {
    lateinit var mainActivity: MainActivity;

    @Before
    fun setUp() {
        mainActivity = Robolectric.setupActivity(MainActivity::class.java)
    }

    @Test
    fun onResume_nullWebViewUrl_noException_resetUrlToLogin() {
        mainActivity.webview.loadUrl(null)
        mainActivity.isSuccessfulConfigCheck = true

        try {
            mainActivity.onResume()
        }
        catch (e: Exception) {
            assert(false)
        }
        val loginUrl = mainActivity.resources.getString(R.string.baseURL) + mainActivity.resources.getString(R.string.loginPath)
        assertEquals(mainActivity.webview.url, loginUrl)
    }
}