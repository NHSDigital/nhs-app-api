package worker

import config.Config
import org.apache.http.client.methods.HttpRequestBase

fun HttpRequestBase.addExternalSystemApiKey(includeApiKey: Boolean) {
    if (includeApiKey) {
        val key = Config.instance.nhsAppApiKey
        this.addHeader("X-Api-Key", key)
    }
}
