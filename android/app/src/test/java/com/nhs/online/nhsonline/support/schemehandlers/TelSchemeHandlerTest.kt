package com.nhs.online.nhsonline.support.schemehandlers

import android.content.Context
import android.content.Intent
import android.content.pm.ActivityInfo
import android.content.pm.ApplicationInfo
import android.content.pm.PackageManager
import android.content.pm.ResolveInfo
import android.net.Uri
import com.nhaarman.mockito_kotlin.*
import org.junit.Assert
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class TelSchemeHandlerTest {
    private lateinit var mockContext: Context
    private lateinit var telSchemeHandler: TelSchemeHandler

    @Before
    fun setUp() {
        val applicationInfo = ApplicationInfo()
        applicationInfo.packageName = "packageName"

        val activityInfo = ActivityInfo()
        activityInfo.name = "name"
        activityInfo.applicationInfo = applicationInfo

        val resolveInfo = ResolveInfo()
        resolveInfo.activityInfo = activityInfo

        val mockPackageManager = mock<PackageManager> {
            on { resolveActivity(any(), any()) } doReturn resolveInfo
        }
        mockContext = mock {
            on { packageManager } doReturn mockPackageManager
        }
        telSchemeHandler = TelSchemeHandler(mockContext)
    }

    @Test
    fun handle_WithValidUrl_ReturnsTrue() {
        val url = "tel:foo"
        val result = telSchemeHandler.handle(url)

        argumentCaptor<Intent>().apply {
            verify(mockContext).startActivity(capture())
            Assert.assertEquals(firstValue.action, Intent.ACTION_DIAL)
            Assert.assertEquals(firstValue.data, Uri.parse(url))
        }
        assertTrue(result)
    }
}