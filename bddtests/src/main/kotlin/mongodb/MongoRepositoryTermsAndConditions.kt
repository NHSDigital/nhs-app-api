package mongodb

data class MongoRepositoryTermsAndConditions(val NhsLoginId: String,
                                             val ConsentGiven: Boolean,
                                             val AnalyticsCookieAccepted: Boolean,
                                             val DateOfConsent: String,
                                             val DateOfAnalyticsCookieToggle: String)
