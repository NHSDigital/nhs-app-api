package mocking.data.nhsAzureSearchData

import mocking.nhsAzureSearchService.NHSAzureSearchReply
import mocking.nhsAzureSearchService.NhsAzureSearchItem

object NhsAzureSearchData {

    const val LIMIT = 20
    // 1 less than F81090 in redis cache
    const val BASE_NACSCODE = 81089
    const val BASE_ORGANISATION_ID = 4648
    const val ORGANISATION_NAME = "Clay Cross Medical Centre"

    fun getLessThanMaxNumberOfSearchData(): NHSAzureSearchReply {
        return generateXSearchData(2)
    }

    fun getMaxNumberOfSearchData(): NHSAzureSearchReply {
        return generateXSearchData(LIMIT)
    }

    fun getZeroSearchData(): NHSAzureSearchReply {
        return generateXSearchData(0)
    }

    fun getMoreThanMaxNumberOfSearchData(): NHSAzureSearchReply {
        return generateXSearchData(LIMIT + 2)
    }

    private fun generateXSearchData(numberOfItems: Int): NHSAzureSearchReply {
        val searchItems = mutableListOf<NhsAzureSearchItem>()

        for(i in 1..Math.min(numberOfItems, NhsAzureSearchData.LIMIT)) {
            val numericNACSCode = BASE_NACSCODE + i
            val organisationId = BASE_ORGANISATION_ID + i

            searchItems.add(NhsAzureSearchItem("$organisationId", "$ORGANISATION_NAME $i", "$i Bridge Street",
                    "Clay Cross", "", "Chesterfield", "County", "S$i 9NG", "F$numericNACSCode"))
        }

        return NHSAzureSearchReply(searchItems, numberOfItems)
    }

}
