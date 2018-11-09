package mocking.data.nhsAzureSearchData

import mocking.nhsAzureSearchService.NHSAzureSearchReply
import mocking.nhsAzureSearchService.NhsAzureSearchItem

object NhsAzureSearchData {

    fun getNhsAzureSearchData(): NHSAzureSearchReply {

        val searchItems = mutableListOf<NhsAzureSearchItem>()

        searchItems.add(NhsAzureSearchItem("4624", "Clay Cross Medical Centre", "Bridge Street",
                "Clay Cross",
                "", "Chesterfield", "County", "S45 9NG",
                "C81056"))

        searchItems.add(NhsAzureSearchItem("4625", "Clay Cross Medical Centre", "6 Bridge Street",
                "Another Street",
                "", "Chesterfield", "Another County", "S45 9NG",
                "C81057"))

        return NHSAzureSearchReply(
                value = searchItems
        )
    }
}
