package com.nhs.online.nhsonline.fido

import android.annotation.SuppressLint
import android.content.Context
import android.content.pm.PackageManager
import android.util.Base64
import android.util.Log
import com.nhs.online.nhsonline.fido.uaf.crypto.Base64url
import org.mindrot.jbcrypt.BCrypt
import java.io.ByteArrayInputStream
import java.security.MessageDigest
import java.security.cert.CertificateFactory

object Fido {
    const val CONNECTION_ERROR_CODE_KV = "\"error_code\":\"connect_fail\""

    const val REGISTRATION_STATUS = "status"
    const val ATTESTATION_STATUS = "attestVerifiedStatus"
    const val REGISTRATION_STATUS_SUCCESS = "SUCCESS"
    const val ATTESTATION_STATUS_VALID = "VALID"
    const val REGISTRATION_RESPONSE_ERROR = "Error"

    const val UAF_AUTH_RESPONSE_FIELD = "uafProtocolMessage"
    const val EMPTY_UAF_RESPONSE_MESSAGE =
        "{\"$UAF_AUTH_RESPONSE_FIELD\": {\"$UAF_AUTH_RESPONSE_FIELD\": \"\"}"

    const val AAID = "EBA0#0001"
    const val ALGORITHM_SHA256: String = "SHA-256"
    private const val ALGORITHM_SHA1 = "SHA1"
    private const val CERTIFICATE_TYPE_X509 = "X509"

    private const val KEY_PREFIX = "nhs-app-key"
    private const val FACET_KEY_PREFIX = "android:apk-key-hash"

    fun generateFidoKeyId(): String {
        val key = "$KEY_PREFIX-${Base64url.encodeToString(BCrypt.gensalt().toByteArray())}"
        return Base64url.encodeToString(key.toByteArray())
    }

    @SuppressLint("PackageManagerGetSignatures") // This vulnerability can only be exploited in Android version 4.4 and below. This is below the minimum supported version of the app
    fun getFacetId(context: Context): String? {
        try {
            val packageInfo = context.packageManager.getPackageInfo(context.packageName,
                PackageManager.GET_SIGNATURES)
            val byteArrayInputStream =
                ByteArrayInputStream(packageInfo.signatures[0].toByteArray())
            val certificate =
                CertificateFactory.getInstance(CERTIFICATE_TYPE_X509)
                    .generateCertificate(byteArrayInputStream)
            val messageDigest = MessageDigest.getInstance(ALGORITHM_SHA1)

            return "$FACET_KEY_PREFIX:" + Base64.encodeToString((messageDigest as MessageDigest).digest(
                certificate.encoded), 3)
        } catch (e: Exception) {
            Log.d("Fido", "Failed to get Facet ID with error: $e")
            return null
        }
    }
}