package com.nhs.online.nhsonline.support.schemehandlers

import android.content.Context
import android.content.pm.ActivityInfo
import android.content.pm.ApplicationInfo
import android.content.pm.PackageManager
import android.content.pm.ResolveInfo
import com.nhaarman.mockito_kotlin.*
import junit.framework.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner


@RunWith(RobolectricTestRunner::class)
class MailToSchemeHandlerTests {

    @Test
    fun handle_returnsTrue_WhenMailToUrl() {
        val mockApplicationInfo: ApplicationInfo = mock()
        mockApplicationInfo.packageName = "something"

        val mockActivityInfo: ActivityInfo = mock()
        mockActivityInfo.name = "Something"
        mockActivityInfo.applicationInfo = mockApplicationInfo

        val mockResolveInfo: ResolveInfo = mock()
        mockResolveInfo.activityInfo = mockActivityInfo

        val mockPackageManger: PackageManager = mock { on { resolveActivity(any(), any()) } doReturn mockResolveInfo }
        val mockContext: Context = mock { on { packageManager } doReturn mockPackageManger }

        val url = "mailto:asdfqertasdfertwerfwertwrt"

        val systemUnderTest = MailToSchemeHandler(mockContext)

        val response = systemUnderTest.handle(url)

        verify(mockContext, times(1)).packageManager
        verify(mockPackageManger, times(1)).resolveActivity(any(), any())
        verify(mockContext, times(1)).startActivity(any())
        Assert.assertTrue(response)

    }

    @Test
    fun handle_ReturnsFalse_WhenSchemeIsNotHandled() {
        val mockContext: Context = mock()
        val url = "tel:"

        val systemUnderTest = MailToSchemeHandler(mockContext)

        val response = systemUnderTest.handle(url)

        Assert.assertFalse(response)
    }

    @Test
    fun handle_returnsFalse_WhenNoPackageAvailableToHandleScheme() {
        val resolveInfo: ResolveInfo? = null
        val mockPackageManger: PackageManager = mock { on { resolveActivity(any(), any()) } doReturn resolveInfo }
        val mockContext: Context = mock { on { packageManager } doReturn mockPackageManger }

        val url = "mailto:enquiries@nhsdigital.nhs.uk?subject=111%20online%20enquiry"

        val systemUnderTest = MailToSchemeHandler(mockContext)

        val response = systemUnderTest.handle(url)

        verify(mockContext, times(1)).packageManager
        verify(mockPackageManger, times(1)).resolveActivity(any(), any())
        Assert.assertFalse(response)
    }
}