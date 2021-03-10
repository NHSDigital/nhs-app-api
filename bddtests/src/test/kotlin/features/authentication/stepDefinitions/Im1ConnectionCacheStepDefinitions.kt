package features.authentication.stepDefinitions

import constants.SerenitySessionKeys
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryIm1ConnectionToken
import net.serenitybdd.core.Serenity
import org.junit.Assert

class Im1ConnectionCacheStepDefinitions {

    @Given("^no IM1 Connection Token is currently cached$")
    fun im1ConnectionTokensClearedFromTheCache() {
        MongoDBConnection.Im1CacheCollection.clearCache()
    }

    @Then("^the IM1 Connection Token is in the cache$")
    fun theIm1ConnectionTokenIsInTheCache() {
        MongoDBConnection.Im1CacheCollection.assertNumberOfDocuments(1)
        val connectionToken = MongoDBConnection.Im1CacheCollection
                .getValues<MongoRepositoryIm1ConnectionToken>(MongoRepositoryIm1ConnectionToken::class.java).first()
        Serenity.setSessionVariable(SerenitySessionKeys.LAST_IM1_TOKEN_CACHED).to(connectionToken.token)
    }

    @Then("^the IM1 Connection Token is no longer in the cache$")
    fun theIm1ConnectionTokenIsNoLongerInTheCache() {
        MongoDBConnection.Im1CacheCollection.assertNumberOfDocuments(0)
    }

    @Then("^the IM1 Connection Token has a different cached value$")
    fun theIm1ConnectionTokenHasADifferentCachedValue() {
        MongoDBConnection.Im1CacheCollection.assertNumberOfDocuments(1)
        val connectionToken = MongoDBConnection.Im1CacheCollection
                .getValues<MongoRepositoryIm1ConnectionToken>(MongoRepositoryIm1ConnectionToken::class.java).first()
        val lastCachedToken = Serenity.sessionVariableCalled<String>(SerenitySessionKeys.LAST_IM1_TOKEN_CACHED)
        Assert.assertNotNull("Im1ConnectionToken is null", connectionToken)
        Assert.assertNotNull("Last cached token is null", lastCachedToken)
        Assert.assertNotEquals(lastCachedToken, connectionToken.token)
    }
}
