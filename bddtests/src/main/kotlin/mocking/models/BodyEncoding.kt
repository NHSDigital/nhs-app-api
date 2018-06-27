package mocking.models

class BodyEncoding {

    var codePage: Int = 0
    var encodingName: String? = null
    var webName: String? = null

    init {
        codePage = 65001
        encodingName = "Unicode (UTF-8)"
        webName = "utf-8"
    }
}