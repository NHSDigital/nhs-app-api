package com.nhs.online.nhsonline.support

import android.Manifest
import android.app.Activity
import android.content.Intent
import android.content.pm.PackageManager
import android.os.Environment
import android.support.v4.app.ActivityCompat
import android.support.v4.content.FileProvider
import android.util.Base64
import com.nhs.online.nhsonline.BuildConfig
import java.io.File
import java.io.FileOutputStream

internal const val STORAGE_REQUEST_CODE = 104
class FileDownloadHelper(
    private val activity: Activity
){
    var fileName: String = ""
    var fileMimeType: String = ""
    var base64Data: String = ""


    fun convertBase64StringToFileAndStoreIt() {
        val base64 = base64Data.substring(base64Data.indexOf(","))
        val dataAsBytes = Base64.decode(base64, Base64.DEFAULT)
        val directory = File(Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_DOWNLOADS).absolutePath)
        // create the directory before saving the file in case it has been deleted
        directory.mkdirs()
        val file =  File(directory, fileName)
        file.createNewFile()

        // Write file to android storage
        val outputStream = FileOutputStream(file)
        outputStream.write(dataAsBytes)
        outputStream.close()

        // Show downloaded file in application
        // of the viewers choice
        showDownloadedDocument(file, fileMimeType)
    }

    private fun showDownloadedDocument(file: File, mimeType: String?) {
        val intent = Intent(Intent.ACTION_VIEW)
        val data = FileProvider.getUriForFile(activity, BuildConfig.APPLICATION_ID + ".provider", file)
        intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION or Intent.FLAG_ACTIVITY_NEW_TASK)
        intent.setDataAndType(data, mimeType)
        activity.startActivity(intent)
    }

    fun isStoragePermissionGranted(): Boolean {
        return ActivityCompat.checkSelfPermission(activity,
            Manifest.permission.WRITE_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED
    }

    fun showStoragePermissionsPopup() {
        ActivityCompat.requestPermissions(activity,
            arrayOf(Manifest.permission.WRITE_EXTERNAL_STORAGE),
            STORAGE_REQUEST_CODE)
    }
}
