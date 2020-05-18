package worker

import org.apache.http.client.utils.URIBuilder

fun URIBuilder.setParameterIfNotNull(key: String, value: String?): URIBuilder {
    if (value!=null) {
        this.addParameter(key, value)
    }
    return this
}
