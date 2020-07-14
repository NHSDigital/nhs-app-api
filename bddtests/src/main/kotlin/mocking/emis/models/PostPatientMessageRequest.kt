package mocking.emis.models

data class PostPatientMessageRequest(val Subject: String? = null,
                                         val MessageBody: String? = null,
                                         var Recipients: List<String>? = null,
                                         val UserPatientLinkToken: String)
