package mocking.models

class Response(val status: Int,
               val body: String?,
               val transformers: List<String>,
               var fixedDelayMilliseconds: Int?,
               val headers: Map<String, String>)
