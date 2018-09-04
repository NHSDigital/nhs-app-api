package worker

import com.google.gson.FieldNamingPolicy
import com.google.gson.Gson
import com.google.gson.GsonBuilder
import com.google.gson.JsonParser
import config.Config

import org.apache.http.HttpResponse
import org.apache.http.HttpStatus.SC_CREATED
import org.apache.http.HttpStatus.SC_OK
import org.apache.http.HttpStatus.SC_NO_CONTENT
import org.apache.http.client.HttpClient
import org.apache.http.client.methods.HttpEntityEnclosingRequestBase
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPost
import org.apache.http.client.methods.HttpUriRequest
import org.apache.http.client.protocol.HttpClientContext
import org.apache.http.client.utils.URIBuilder
import org.apache.http.impl.client.HttpClients
import worker.models.patient.Im1ConnectionResponse
import org.apache.http.entity.StringEntity
import org.apache.http.impl.client.BasicCookieStore
import org.apache.http.impl.cookie.BasicClientCookie
import org.apache.http.protocol.BasicHttpContext
import org.apache.http.protocol.HttpContext
import worker.models.appointments.AppointmentBookRequest
import worker.models.appointments.AppointmentSlotsResponse
import worker.models.appointments.CancelAppointmentRequest
import worker.models.appointments.MyAppointmentsResponse
import worker.models.courses.CoursesListResponse
import worker.models.demographics.Demographics
import worker.models.linkage.CreateLinkageRequest
import worker.models.linkage.LinkageResponse
import worker.models.myrecord.MyRecordResponse
import worker.models.ndop.NdopResponse
import worker.models.prescriptions.PrescriptionsListResponse
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse
import worker.models.session.UserSessionResponse.UserSessionResponseCookie
import java.io.InputStreamReader
import java.io.BufferedReader
import java.net.URI
import java.net.URLEncoder
import javax.servlet.http.Cookie

@Suppress("TooManyFunctions", "LargeClass")
class WorkerClient(config:Config = Config.instance){
    var cookieHeaderKey = "Set-Cookie"
    private val _client: HttpClient
    private val gsonBuilder: GsonBuilder = GsonBuilder()
    private var gson: Gson
    private val config = config
    private var csrfToken = ""

    init {
        _client = HttpClients.createDefault()
        gsonBuilder.setFieldNamingPolicy(FieldNamingPolicy.IDENTITY)
        gson = gsonBuilder.create()
    }

    fun setCsrfToken(token: String) {
        this.csrfToken = token
    }

    fun getIm1Connection(connectionToken: String?, odsCode: String?): Im1ConnectionResponse {
        val httpGet = HttpGet(config.cidBackendUrl + WorkerPaths.patientIm1Connection)
        httpGet.setHeader(WorkerHeaders.ConnectionToken, connectionToken)
        httpGet.setHeader(WorkerHeaders.OdsCode, odsCode)

        val response = sendAsync(httpGet)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpGet.releaseConnection()

        return gson.fromJson(result, Im1ConnectionResponse::class.java)
    }

    fun postSessionConnection(requestBody: UserSessionRequest): UserSessionResponse {
        val httpPost = HttpPost(config.pfsBackendUrl + WorkerPaths.sessionConnection)
        val entity = StringEntity(gson.toJson(requestBody), "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sendAsync(httpPost)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpPost.releaseConnection()
        println(result)

        // Extract csrfToken
        val parsedResponse = JsonParser().parse(result)
        setCsrfToken(parsedResponse.asJsonObject.get("token").asString)

        val userSessionResponseBody = gson.fromJson<UserSessionResponse.UserSessionResponseBody>(result,
                UserSessionResponse.UserSessionResponseBody::class.java)

        val userSessionResponseCookie = UserSessionResponseCookie(Cookie(
                cookieHeaderKey, response.getHeaders(cookieHeaderKey).first().value))
        return UserSessionResponse(userSessionResponseCookie, userSessionResponseBody)
    }

    fun getMyAppointments(fromDate: String, includePastAppointments: Boolean = false): MyAppointmentsResponse {
        val uriBuilder = URIBuilder(config.pfsBackendUrl)
                .setPath(WorkerPaths.myAppointments)
                .addParameter("pastAppointmentsFromDate", fromDate)
                .addParameter("includePastAppointments", includePastAppointments.toString())

        val httpGet = HttpGet(uriBuilder.build())

        val response = sendAsync(httpGet, null)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpGet.releaseConnection()
        println(result)

        return gson.fromJson<MyAppointmentsResponse>(result, MyAppointmentsResponse::class.java)
    }

    fun getAppointmentSlots(fromDate: String? = null,
                            toDate: String? = null,
                            sessionCookie: Cookie? = null): AppointmentSlotsResponse {
        val uriBuilder = createUriBuilderForAppointmentSlots(fromDate, toDate)
        val httpGet = HttpGet(uriBuilder.build())

        if (sessionCookie != null) httpGet.addHeader("Cookie", sessionCookie.value.split(";")[0])

        val response = sendAsync(httpGet, null)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpGet.releaseConnection()
        println(result)

        return gson.fromJson<AppointmentSlotsResponse>(result, AppointmentSlotsResponse::class.java)
    }

    fun getPrescriptionsConnection(fromDate: String?, context: HttpContext? = null): PrescriptionsListResponse {
        var queryString = ""
        if (fromDate != null) queryString = "?FromDate=" + URLEncoder.encode(fromDate, "UTF-8")
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.getPrescriptionsConnection + queryString)
        val response = sendAsync(httpGet, context)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpGet.releaseConnection()

        return gson.fromJson<PrescriptionsListResponse>(result, PrescriptionsListResponse::class.java)
    }

    fun deleteAppointment(requestBody: CancelAppointmentRequest): HttpResponse {
        val httpDelete = HttpDeleteWithBody(config.pfsBackendUrl + WorkerPaths.myAppointments)
        val entity = StringEntity(gson.toJson(requestBody), "UTF-8")
        entity.setContentType("application/json")
        httpDelete.entity = entity

        val response = sendAsync(httpDelete)
        httpDelete.releaseConnection()
        return response
    }

    fun postAppointment(appointmentBookRequest: AppointmentBookRequest, sessionCookie: Cookie? = null): HttpResponse {
        val httpPost = HttpPost(config.pfsBackendUrl + WorkerPaths.myAppointments)

        if (sessionCookie != null) httpPost.addHeader("Cookie", sessionCookie.value.split(";")[0])
        val entity = StringEntity(gson.toJson(appointmentBookRequest), "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sendAsync(httpPost, null)
        httpPost.releaseConnection()
        return response
    }

    fun postPrescriptionsConnection(requestBody: PrescriptionSubmissionRequest?): HttpResponse {
        val httpPost = HttpPost(config.pfsBackendUrl + WorkerPaths.postPrescriptionsConnection)
        val entity = StringEntity(gson.toJson(requestBody), "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sendAsync(httpPost)
        httpPost.releaseConnection()
        return response
    }

    fun getCoursesConnection(): CoursesListResponse {
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.getCoursesConnection)

        val response = sendAsync(httpGet)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpGet.releaseConnection()

        return gson.fromJson<CoursesListResponse>(result, CoursesListResponse::class.java)
    }

    fun getDemographics(): Demographics {
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.getDemographicsConnection)
        val response = sendAsync(httpGet)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpGet.releaseConnection()

        return gson.fromJson<Demographics>(result, Demographics::class.java)
    }

    fun getMyRecord(): MyRecordResponse {
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.getMyRecordConnection)
        val response = sendAsync(httpGet)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpGet.releaseConnection()
        val json = gson.fromJson<MyRecordResponse>(result, MyRecordResponse::class.java)

        return json
    }

    fun getNdopToken(): NdopResponse {
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.ndopConnection)
        val response = sendAsync(httpGet)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpGet.releaseConnection()
        val json = gson.fromJson<NdopResponse>(result, NdopResponse::class.java)

        return json
    }

    fun getLinkageKey(nhsNumber: String?, odsCode: String?, identityToken: String?): LinkageResponse {
        val httpGet = HttpGet(config.cidBackendUrl + WorkerPaths.LinkageKey)
        httpGet.setHeader(WorkerHeaders.NhsNumber, nhsNumber)
        httpGet.setHeader(WorkerHeaders.OdsCode, odsCode)
        httpGet.setHeader(WorkerHeaders.IdentityToken, identityToken)
        val response = sendAsync(httpGet)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpGet.releaseConnection()
        println(result)

        val json = gson.fromJson<LinkageResponse>(result, LinkageResponse::class.java)

        return json
    }

    fun postLinkageKey(requestBody: CreateLinkageRequest): LinkageResponse {
        val httpPost = HttpPost(config.cidBackendUrl + WorkerPaths.LinkageKey)
        val entity = StringEntity(gson.toJson(requestBody), "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sendAsync(httpPost)
        val rd = BufferedReader(InputStreamReader(response.entity.content))
        val result = rd.use { it.readText() }
        httpPost.releaseConnection()
        println(result)
        return gson.fromJson<LinkageResponse>(result, LinkageResponse::class.java)
    }


    private fun createUriBuilderForAppointmentSlots(fromDate: String?, toDate: String?): URIBuilder {
        val uriBuilder = URIBuilder(config.pfsBackendUrl + WorkerPaths.appointmentSlots)
        if (!fromDate.isNullOrEmpty()) {
            uriBuilder.setParameter("fromDate", fromDate)
        }
        if (!toDate.isNullOrEmpty()) {
            uriBuilder.setParameter("toDate", toDate)
        }
        return uriBuilder
    }

    private fun sendAsync(request: HttpUriRequest, context: HttpContext? = null): HttpResponse {
        // If we have a token, use it
        if (this.csrfToken.isNotEmpty()) {
            request.addHeader("X-CSRF-TOKEN", csrfToken)
        }

        val response = if (context != null) _client.execute(request, context) else _client.execute(request)

        if (response.statusLine.statusCode != SC_OK &&
                response.statusLine.statusCode != SC_CREATED &&
                response.statusLine.statusCode != SC_NO_CONTENT) {
            // Exception is thrown here to ensure that the
            // tests fail at the appropriate location and not further down the line
            // when values are not as expected.  This makes it easier to debug.
            throw NhsoHttpException(request, response)
        } else {
            return response
        }
    }

    companion object {

        fun getHttpContext(includeBadCookie: Boolean): HttpContext {
            val localContext = BasicHttpContext()
            val cookieStore = BasicCookieStore()

            if (includeBadCookie) {
                val cookie = BasicClientCookie("NHSO-Session-Id", "ErrorCookieId")
                cookie.path = "/"
                cookie.domain = ".bitraft.io"
                cookie.setAttribute("httponly", "null")
                cookie.setAttribute("path", "/")
                cookie.setAttribute("samesite", "lax")
                cookie.isSecure = false
                cookie.version = 0

                cookieStore.addCookie(cookie)
            }

            localContext.setAttribute(HttpClientContext.COOKIE_STORE, cookieStore)

            return localContext
        }
    }

    class HttpDeleteWithBody constructor() : HttpEntityEnclosingRequestBase() {
        val METHOD_NAME = "DELETE"

        constructor(uri: URI) : this() {
            setURI(uri)
        }

        constructor(uri: String) : this() {
            setURI(URI.create(uri))
        }

        override fun getMethod(): String {
            return METHOD_NAME
        }
    }
}