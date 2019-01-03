package webdrivers.options

import com.neovisionaries.ws.client.WebSocketFactory
import org.apache.commons.io.IOUtils
import org.json.JSONArray
import java.io.BufferedReader
import java.io.InputStreamReader
import java.net.URL


class ChromeOptionManager private constructor() {

    private object Holder {
        val INSTANCE = ChromeOptionManager()
    }

    companion object {
        const val DEBUG_PORT = 9222
        val instance: ChromeOptionManager by lazy { Holder.INSTANCE }
    }

    /**
     * *** CAVEAT *** - When running locally ensure that all chrome instances are
     * closed. This will cause the correct instance to be configured.
     */
    fun configureOption(webDriverOption: IWebDriverOption) {
        val url = URL("http://localhost:$DEBUG_PORT/json")
        val conn = url.openConnection()
        val reader = BufferedReader(InputStreamReader(conn.getInputStream()))
        val jsonArray = JSONArray(IOUtils.toString(reader))

        if (webDriverOption.message.isPresent && jsonArray.length() > 0) {
            var idx = 0
            while (idx < jsonArray.length()) {
                val jsonObject = jsonArray.getJSONObject(idx)
                if (jsonObject.getString("type") == "page") {
                    sendMessage(jsonObject.getString("webSocketDebuggerUrl"),
                            webDriverOption.message.get())
                }
                idx += 1
            }
        }
    }

    private fun sendMessage(webSocketDebuggerUrl: String, message: String) {
        WebSocketFactory()
                .createSocket(webSocketDebuggerUrl)
                .connect()
                .sendText(message)
    }
}