package com.nhs.online.nhsonline.webclients

import android.Manifest
import android.annotation.SuppressLint
import android.app.Activity
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.location.LocationManager
import android.net.Uri
import android.os.Build
import android.os.Environment
import android.provider.MediaStore
import android.provider.Settings
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import android.util.Log
import android.webkit.*
import java.io.File
import java.io.IOException
import java.text.SimpleDateFormat
import java.util.*

internal const val LOCATION_REQUEST_CODE = 101
internal const val CAMERA_STORAGE_REQUEST_CODE = 102
internal const val UPLOAD_FILE_REQUEST_CODE = 103

private const val VIDEO_PREFIX = "video/"

private val TAG = ChromeClientLocationHandler::class.java.simpleName

var fileUploadCallback: ValueCallback<Array<Uri>>? = null
var uploadedFileLocation: String? = null
var videoFilePending = false

class ChromeClientLocationHandler(private val activity: Activity) : WebChromeClient() {
    private var mCallback: GeolocationPermissions.Callback? = null
    private var mOrigin: String? = null
    private fun requiresLocationPermissionLocationRationale(): Boolean {
        return ActivityCompat.shouldShowRequestPermissionRationale(activity,
            Manifest.permission.ACCESS_FINE_LOCATION)
    }

    private fun showLocationPermissionPopup() {
        ActivityCompat.requestPermissions(activity,
            arrayOf(Manifest.permission.ACCESS_FINE_LOCATION),
            LOCATION_REQUEST_CODE)
    }

    private fun isLocationPermissionGranted(): Boolean {
        return ActivityCompat.checkSelfPermission(activity,
            Manifest.permission.ACCESS_FINE_LOCATION) == PackageManager.PERMISSION_GRANTED
    }

    override fun onGeolocationPermissionsShowPrompt(
        origin: String?,
        callback: GeolocationPermissions.Callback?
    ) {
        if (isLocationPermissionGranted()) {
            callback?.invoke(origin, true, false)
        } else {
            if (!requiresLocationPermissionLocationRationale()) {
                showLocationPermissionPopup()
                mCallback = callback
                mOrigin = origin
            } else {
                callback?.invoke(origin, false, false)
            }
        }
    }

    override fun onPermissionRequest(request: PermissionRequest) {
        Log.d(TAG, "Permission Request: ${request.resources}")
        request.grant(request.resources)
    }

    fun handleLocationPermissionResult(grantResults: IntArray) {
        if (grantResults.isNotEmpty() && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            val lm = activity.getSystemService(Context.LOCATION_SERVICE) as LocationManager
            val gpsEnabled = lm.isProviderEnabled(LocationManager.GPS_PROVIDER)
            if (gpsEnabled) {
                onLocationPermissionResponded(true)
            } else {
                val gpsOptionsIntent =
                    Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS)
                activity.startActivity(gpsOptionsIntent)
                onLocationPermissionResponded(true)
            }
        } else {
            onLocationPermissionResponded(false)
        }
    }

    fun handleCameraFilePermissionResult(grantResults: IntArray) {
        if (grantResults.isNotEmpty() && grantResults.all { it == PackageManager.PERMISSION_GRANTED }) {
            Log.d(TAG, "Permissions granted for camera and file")

            uploadFileToWebView()
        } else {
            Log.d(TAG, "Permission not granted")
        }
    }

    override fun onShowFileChooser(
        webView: WebView,
        filePathCallback: ValueCallback<Array<Uri>>,
        fileChooserParams: WebChromeClient.FileChooserParams
    ): Boolean {
        videoFilePending = isVideoRequest(fileChooserParams)

        val requiredPermissions = getRequiredPermissions(videoFilePending)


        Log.d(TAG, "Showing File Chooser")

        fileUploadCallback = filePathCallback

        if (!checkMissingPermissions(requiredPermissions)) {
            uploadFileToWebView()
        } else {
            Log.d(TAG, "Permissions not set")

            askForPermissions(requiredPermissions, CAMERA_STORAGE_REQUEST_CODE)
        }

        return true
    }

    private fun uploadFileToWebView() {
        var intentArray: Array<Intent?>

        var externalMediaIntent: Intent?

        externalMediaIntent =
            if (videoFilePending) Intent(MediaStore.ACTION_VIDEO_CAPTURE) else Intent(MediaStore.ACTION_IMAGE_CAPTURE)

        if (externalMediaIntent.resolveActivity(activity.packageManager) != null) {
            var tempUploadFile: File? = createTemporaryFile(videoFilePending)

            if (tempUploadFile != null) {
                if (!videoFilePending)
                    externalMediaIntent.putExtra("PhotoPath", uploadedFileLocation)
                uploadedFileLocation = "file:${tempUploadFile.absolutePath}"
                externalMediaIntent.putExtra(MediaStore.EXTRA_OUTPUT,
                    Uri.fromFile(tempUploadFile))
            } else {
                externalMediaIntent = null
            }

        } else
            externalMediaIntent = null

        intentArray = if (externalMediaIntent != null) {
            arrayOf(externalMediaIntent)
        } else {
            arrayOfNulls(0)
        }

        val contentSelectionIntent = Intent(Intent.ACTION_GET_CONTENT)
        contentSelectionIntent.addCategory(Intent.CATEGORY_OPENABLE)
        contentSelectionIntent.type = "*/*"

        val chooserIntent = Intent(Intent.ACTION_CHOOSER)
        chooserIntent.putExtra(Intent.EXTRA_INTENT, contentSelectionIntent)
        chooserIntent.putExtra(Intent.EXTRA_TITLE, "File Chooser")
        chooserIntent.putExtra(Intent.EXTRA_INITIAL_INTENTS, intentArray)
        activity.startActivityForResult(chooserIntent, UPLOAD_FILE_REQUEST_CODE)
    }


    fun onLocationPermissionResponded(permissionGranted: Boolean) {
        mCallback?.invoke(mOrigin, permissionGranted, false)
    }

    fun getFileUploadCallback(): ValueCallback<Array<Uri>>? = fileUploadCallback

    fun resetFileUploadCallback() {
        fileUploadCallback = null
    }

    fun getUploadedFileLocation(): String? = uploadedFileLocation

    private fun checkMissingPermissions(requiredPermissions: Array<String>): Boolean {
        return requiredPermissions.any {
            ContextCompat.checkSelfPermission(activity, it) != PackageManager.PERMISSION_GRANTED
        }
    }

    private fun askForPermissions(requiredPermissions: Array<String>, permissionRequestCode: Int) {
        if (Build.VERSION.SDK_INT >= 23) {
            ActivityCompat.requestPermissions(activity,
                requiredPermissions,
                permissionRequestCode)
        }
    }

    private fun createTemporaryFile(isVideoFile: Boolean): File? {
        return try {
            if (isVideoFile)
                createVideoFile()
            else
                createImageFile()
        } catch (ex: IOException) {
            Log.e(TAG, "File creation failed", ex)
            null
        }
    }


    @Throws(IOException::class)
    private fun createImageFile(): File {
        @SuppressLint("SimpleDateFormat") val timeStamp =
            SimpleDateFormat("yyyyMMdd_HHmmss").format(Date())
        val imageFileName = """img_$timeStamp"""

        val storageDir =
            Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_PICTURES)
        Log.d(TAG, "Creating file: $imageFileName in directory: ${storageDir.absolutePath}" )
        return File.createTempFile(imageFileName, ".jpg", storageDir)
    }

    @Throws(IOException::class)
    private fun createVideoFile(): File {
        @SuppressLint("SimpleDateFormat") val timeStamp =
            SimpleDateFormat("yyyyMMdd_HHmmss").format(Date())
        val videoFileName = """vid_$timeStamp"""

        val storageDir =
            Environment.getExternalStoragePublicDirectory(Environment.DIRECTORY_MOVIES)
        Log.d(TAG, "Creating file: $videoFileName in directory: ${storageDir.absolutePath}" )
        return File.createTempFile(videoFileName, ".mp4", storageDir)
    }

    private fun isVideoRequest(fileChooserParams: WebChromeClient.FileChooserParams): Boolean {
        var acceptedTypes = fileChooserParams.acceptTypes?.get(0) ?: return false

        return acceptedTypes
            .split(",")
            .all {
                it.startsWith(VIDEO_PREFIX)
            }
    }

    private fun getRequiredPermissions(isVideoFile: Boolean): Array<String> {
        var requiredBasePermissions = arrayOf(Manifest.permission.WRITE_EXTERNAL_STORAGE,
            Manifest.permission.READ_EXTERNAL_STORAGE,
            Manifest.permission.CAMERA)

        var requiredVideoPermissions =
            requiredBasePermissions +
                    arrayOf(Manifest.permission.MODIFY_AUDIO_SETTINGS,
                        Manifest.permission.RECORD_AUDIO)

        return if (isVideoFile) {
            requiredVideoPermissions
        } else {
            requiredBasePermissions
        }
    }
}
