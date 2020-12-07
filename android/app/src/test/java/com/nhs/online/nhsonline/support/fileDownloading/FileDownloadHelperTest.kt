package com.nhs.online.nhsonline.support.fileDownloading

import android.Manifest
import android.content.Intent
import android.content.pm.PackageManager
import androidx.core.app.ActivityCompat
import com.nhaarman.mockitokotlin2.*
import com.nhs.online.nhsonline.activities.MainActivity
import junit.framework.Assert.assertEquals
import org.junit.Before
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.Robolectric
import org.robolectric.RobolectricTestRunner
import org.robolectric.RuntimeEnvironment
import org.robolectric.Shadows.shadowOf

@RunWith(RobolectricTestRunner::class)
class FileDownloadHelperTest {

    private val base64FileMock: Base64File = mock()
    private val permissionDelegateMock: ActivityCompat.PermissionCompatDelegate = mock()

    private lateinit var mainActivity: MainActivity
    private lateinit var spyActivity: MainActivity

    @Before
    fun setUp() {
        mainActivity = Robolectric.buildActivity(MainActivity::class.java).create().get()
        ActivityCompat.setPermissionCompatDelegate(permissionDelegateMock)
    }

    private fun setUpPermissions(permissionOutcome: Int) {
        spyActivity = spy(mainActivity) {
            on {
                checkPermission( eq(Manifest.permission.WRITE_EXTERNAL_STORAGE), any(), any() )
            }.thenReturn(permissionOutcome)
        }
    }

    @Test
    fun writeExternalStoragePermission_notGranted() {
        setUpPermissions(PackageManager.PERMISSION_DENIED)
        val fileDownloadHelper = FileDownloadHelper(spyActivity)
        assertEquals(false, fileDownloadHelper.setFileAndCheckForPermission(base64FileMock))
        verify(permissionDelegateMock, times(1)).requestPermissions(
                spyActivity,
                arrayOf(Manifest.permission.WRITE_EXTERNAL_STORAGE),
                STORAGE_REQUEST_CODE)
    }

    @Test
    fun writeExternalStoragePermission_granted() {
        setUpPermissions(PackageManager.PERMISSION_GRANTED)
        val fileDownloadHelper = FileDownloadHelper(spyActivity)
        assertEquals(true, fileDownloadHelper.setFileAndCheckForPermission(base64FileMock))
        verify(permissionDelegateMock, times(0)).requestPermissions(
                spyActivity,
                arrayOf(Manifest.permission.WRITE_EXTERNAL_STORAGE),
                STORAGE_REQUEST_CODE)
    }

    @Test
    fun clearFileTest() {
        setUpPermissions(PackageManager.PERMISSION_GRANTED)
        val fileDownloadHelper = FileDownloadHelper(spyActivity)
        fileDownloadHelper.setFileAndCheckForPermission(base64FileMock)
        assertEquals(base64FileMock, fileDownloadHelper.base64File)

        fileDownloadHelper.clearFile()
        assertEquals(null, fileDownloadHelper.base64File)
    }

    @Test
    fun tryDownload_withEmptyFile_returnsFalse() {
        setUpPermissions(PackageManager.PERMISSION_GRANTED)
        val fileDownloadHelper = FileDownloadHelper(spyActivity)
        fileDownloadHelper.setFileAndCheckForPermission(base64FileMock)
        assertEquals(false, fileDownloadHelper.tryDownload())

    }

    @Test
    fun tryDownload_withFile_returnsTrue() {
        setUpPermissions(PackageManager.PERMISSION_GRANTED)
        val fileDownloadHelper = FileDownloadHelper(spyActivity)
        val mimeType = "application/pdf"

        val testFile = Base64File(
                "bazzFile.pdf",
                mimeType,
                "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII="
        )
        fileDownloadHelper.setFileAndCheckForPermission(testFile)
        assertEquals(true, fileDownloadHelper.tryDownload())

        verify(spyActivity, times(1)).startActivity(any<Intent>())
        val actualIntent = shadowOf(RuntimeEnvironment.application).nextStartedActivity

        assertEquals(Intent.ACTION_VIEW, actualIntent.action)
        assertEquals(Intent.FLAG_GRANT_READ_URI_PERMISSION or Intent.FLAG_ACTIVITY_NEW_TASK, actualIntent.flags)
        assertEquals(mimeType, actualIntent.type)
    }
}
