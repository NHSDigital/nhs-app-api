package mocking.models

class Request(@Suppress("unused")
              val url: String?,
              val urlPath: String?,
              val urlPattern: String?,
              val method: String,
              val headers: Map<String, Map<String, String>>,
              val queryParameters: Map<String, Map<String, String>>,
              val bodyPatterns: List<Map<String, String>>)
