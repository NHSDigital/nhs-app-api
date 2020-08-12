package features.authentication.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mongodb.MongoDBConnection

class Im1ConnectionCacheStepDefinitions {

    @Given("^no IM1 Connection Token is currently cached$")
    fun im1ConnectionTokensClearedFromTheCache() {
        MongoDBConnection.Im1CacheCollection.clearCache()
    }

    @Then("^the IM1 Connection Token is in the cache$")
    fun theIm1ConnectionTokenIsInTheCache() {
        MongoDBConnection.Im1CacheCollection.assertNumberOfDocuments(1)
    }

    @Then("^the IM1 Connection Token is no longer in the cache$")
    fun theIm1ConnectionTokenIsNoLongerInTheCache() {
        MongoDBConnection.Im1CacheCollection.assertNumberOfDocuments(0)
    }
}
