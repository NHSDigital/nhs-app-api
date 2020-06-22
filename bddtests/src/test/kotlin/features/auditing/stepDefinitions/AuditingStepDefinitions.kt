package features.auditing.stepDefinitions

import cucumber.api.java.en.Then
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryAuditInfo
import org.junit.Assert

class AuditingStepDefinitions {

    @Then("^the operation (.*) has been audited$")
    fun theOperationHasBeenAudited(value: String) {
        val auditInfo = MongoDBConnection.AuditInfoCollection
                .getValuesWhere<MongoRepositoryAuditInfo>(MongoRepositoryAuditInfo::class.java, "Operation", value)
        Assert.assertNotNull(auditInfo)
        Assert.assertTrue(auditInfo.count() > 0)
    }

    @Then("^the expected field (.*) is audited containing the value (.*)$")
    fun hasBeenAuditingContainingTheDetails(field: String, value: String) {
        val auditInfo = MongoDBConnection.AuditInfoCollection
                .getValuesWhere<MongoRepositoryAuditInfo>(MongoRepositoryAuditInfo::class.java, field, value)
        Assert.assertNotNull(auditInfo)
        Assert.assertTrue(auditInfo.count() > 0)
    }

}
