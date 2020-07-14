package mocking.models

class Request(val urlPath: String,
              val method: String,
              val headers: Map<String, Map<String, String>>,
              val queryParameters: Map<String, Map<String, String>>,
              val bodyPatterns: List<Map<String, String>>)
