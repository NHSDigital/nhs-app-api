package com.nhs.online.nhsonline.support.schemehandlers

import android.content.Context
import android.content.pm.ActivityInfo
import android.content.pm.ApplicationInfo
import android.content.pm.PackageManager
import android.content.pm.ResolveInfo
import com.nhaarman.mockito_kotlin.any
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.verify
import junit.framework.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner


@RunWith(RobolectricTestRunner::class)
class MailToSchemeHandlerTests {

    @Test
    fun handle_returnsTrue_WhenMailToUrl() {
        val applicationInfoMock: ApplicationInfo = mock()
        applicationInfoMock.packageName = "something"

        val activityInfoMock: ActivityInfo = mock()
        activityInfoMock.name = "Something"
        activityInfoMock.applicationInfo = applicationInfoMock

        val resolveInfoMock: ResolveInfo = mock()
        resolveInfoMock.activityInfo = activityInfoMock

        val packageManagerMock: PackageManager = mock { on { resolveActivity(any(), any()) } doReturn resolveInfoMock }
        val contextMock: Context = mock { on { packageManager } doReturn packageManagerMock }

        val url = "mailto:asdfqertasdfertwerfwertwrt"

        val systemUnderTest = MailToSchemeHandler(contextMock)

        val response = systemUnderTest.handle(url)

        verify(contextMock).packageManager
        verify(packageManagerMock).resolveActivity(any(), any())
        verify(contextMock).startActivity(any())
        Assert.assertTrue(response)

    }

    @Test
    fun handle_ReturnsFalse_WhenSchemeIsNotHandled() {
        val contextMock: Context = mock()
        val url = "tel:"

        val systemUnderTest = MailToSchemeHandler(contextMock)

        val response = systemUnderTest.handle(url)

        Assert.assertFalse(response)
    }

    @Test
    fun handle_returnsFalse_WhenNoPackageAvailableToHandleScheme() {
        val resolveInfo: ResolveInfo? = null
        val packageManagerMock: PackageManager = mock { on { resolveActivity(any(), any()) } doReturn resolveInfo }
        val contextMock: Context = mock { on { packageManager } doReturn packageManagerMock }

        val url = "mailto:enquiries@nhsdigital.nhs.uk?subject=111%20online%20enquiry"

        val systemUnderTest = MailToSchemeHandler(contextMock)

        val response = systemUnderTest.handle(url)

        verify(contextMock).packageManager
        verify(packageManagerMock).resolveActivity(any(), any())
        Assert.assertFalse(response)
    }
}