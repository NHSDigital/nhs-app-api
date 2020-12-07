package com.nhs.online.nhsonline.support.fileDownloading

import android.Manifest
import android.app.Activity
import android.content.Intent
import android.content.pm.PackageManager
import android.os.Environment
import androidx.core.app.ActivityCompat
import androidx.core.content.FileProvider
import android.util.Log
import androidx.core.content.ContextCompat
import com.nhs.online.nhsonline.BuildConfig
import java.io.File
import java.io.FileOutputStream


internal const val STORAGE_REQUEST_CODE = 104

private val TAG = FileDownloadHelper::class.java.simpleName

class FileDownloadHelper (
    private val activity: Activity
){
    var base64File: Base64File? = null
        private set

    fun setFileAndCheckForPermission(base64File: Base64File): Boolean {
        this.base64File = base64File
        // User needs to allow access to their
        // internal storage on the device.
        val hasPermission = isStoragePermissionGranted()
        if (!hasPermission ) {
            showStoragePermissionsPopup()
        }
        return hasPermission
    }

    fun clearFile() {
        this.base64File = null
    }

    fun tryDownload(): Boolean {
        return try {
            convertBase64StringToFileAndStoreIt()
            true
        } catch (e: Exception) {
            Log.e(TAG, "Download document resulted in error: ", e)
            false
        }
    }

    private fun convertBase64StringToFileAndStoreIt() {
        val dataAsBytes = base64File!!.decode()
        val directory = File(Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS).absolutePath)
        // create the directory before saving the file in case it has been deleted
        directory.mkdirs()
        val file =  File(directory, base64File!!.fileName)
        file.createNewFile()

        // Write file to android storage
        val outputStream = FileOutputStream(file)
        outputStream.write(dataAsBytes)
        outputStream.close()

        // Show downloaded file in application
        // of the viewers choice
        showDownloadedDocument(file, base64File!!.fileMimeType)
    }

    private fun showDownloadedDocument(file: File, mimeType: String?) {
        val intent = Intent(Intent.ACTION_VIEW)
        val data = FileProvider.getUriForFile(activity, BuildConfig.APPLICATION_ID + ".provider", file)
        intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION or Intent.FLAG_ACTIVITY_NEW_TASK)
        intent.setDataAndType(data, mimeType)
        activity.startActivity(intent)
    }

    fun isStoragePermissionGranted(): Boolean {
        return ContextCompat.checkSelfPermission(activity,
            Manifest.permission.WRITE_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED
    }

    private fun showStoragePermissionsPopup() {
        ActivityCompat.requestPermissions(activity,
                arrayOf(Manifest.permission.WRITE_EXTERNAL_STORAGE),
                STORAGE_REQUEST_CODE)
    }
}
