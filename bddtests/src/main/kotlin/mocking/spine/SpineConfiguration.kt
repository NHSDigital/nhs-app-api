package mocking.spine

data class SpineConfiguration(internal val fromAsid: String,
                            internal val userId: String,
                            internal val roleProfileId: String,
                            internal val epsTraceId: String)