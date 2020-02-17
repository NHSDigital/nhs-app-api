package com.nhs.online.nhsonline.browseractivities

import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.content.pm.ResolveInfo
import android.net.Uri
import com.nhaarman.mockito_kotlin.mock
import com.nhaarman.mockito_kotlin.verify
import com.nhaarman.mockito_kotlin.whenever
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class OpenUrlInBrowserActivityTest : ResourceMockingClass() {
    private lateinit var openUrlInBrowserActivity: OpenUrlInBrowserActivity

    @Before
    fun setUp() {
        openUrlInBrowserActivity = OpenUrlInBrowserActivity()
    }

    @Test
    fun start_calls_showUnavailabilityError_whenBrowserIsDisabled() {
        var context: Context = mockContext()
        val interactor: IInteractor = mock()
        val packageManager: PackageManager = mock()

        val url = "https://www.nhs.uk/"
        val resolvedActivityList: List<ResolveInfo> = emptyList()
        val intent = Intent(Intent.ACTION_VIEW, Uri.parse(url))
        val errorMessage = ErrorMessage(context.resources, ErrorType.BrowserNotAvailable)

        whenever(context.packageManager).thenReturn(packageManager)
        whenever(packageManager.queryIntentActivities(intent, 0)).thenReturn(resolvedActivityList)

        openUrlInBrowserActivity.start(context, url, interactor)

        verify(interactor).showUnavailabilityError(errorMessage)
    }
}