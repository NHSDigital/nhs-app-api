package mocking.defaults.dataPopulation.journies.linkage

data class Linkage(
        val odsCode: String,
        val nhsNumber: String,
        val linkageKey: String? = null,
        val accountId: String? = null)