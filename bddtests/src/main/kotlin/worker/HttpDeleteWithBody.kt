package worker

import org.apache.http.client.methods.HttpEntityEnclosingRequestBase
import java.net.URI

class HttpDeleteWithBody constructor() : HttpEntityEnclosingRequestBase() {
    companion object {
        const val METHOD_NAME = "DELETE"
    }

    constructor(uri: String) : this() {
        setURI(URI.create(uri))
    }

    override fun getMethod(): String {
        return METHOD_NAME
    }
}