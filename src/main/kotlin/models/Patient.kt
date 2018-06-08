package models

import mocking.defaults.MockDefaults

data class Patient(
        val title:String = "",
        val firstName:String = "",
        val surname:String = "",
        val dateOfBirth:String = "",
        val accountId:String = "",
        val odsCode:String = "",
        val connectionToken:String = "",
        val sessionId:String = "",
        val endUserSessionId:String = "",
        val linkageKey:String = "",
        val userPatientLinkToken:String = "",
        val nhsNumbers: List<String> = emptyList()
) {
    companion object {
        val paulSmith = Patient(
                title = "Mr",
                firstName = "Paul",
                surname = "Smith",
                odsCode = MockDefaults.DEFAULT_ODS_CODE,
                userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
                sessionId = MockDefaults.DEFAULT_SESSION_ID,
                connectionToken = MockDefaults.DEFAULT_CONNECTION_TOKEN,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID)
        
        val jackJackson = Patient(
                title = "Mr",
                firstName = "Jack",
                surname = "Jackson",
                odsCode = MockDefaults.DEFAULT_ODS_CODE,
                sessionId = "gY39SJJMEEg7rNbcsfF8",
                connectionToken = "efa22020-9221-46a6-a0f0-6c0340b8f44d",
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID
        )
        
        val alanCook = Patient(
                title = "Mr",
                firstName = "Alan",
                surname = "Cook",
                odsCode = MockDefaults.DEFAULT_ODS_CODE,
                sessionId = "fbWgorZ8Fggk9c5PgKd7",
                connectionToken = "7e14cfb4-eb7a-44c3-8603-28ee36c7a9bf",
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID
        )

        val halleDawe = Patient(
                title = "Miss",
                firstName = "Halle",
                surname = "Dawe",
                dateOfBirth = "1994-02-21T00:00:00",
                odsCode = MockDefaults.DEFAULT_ODS_CODE,
                sessionId = "4RDwmQVi3OfSbp47dbAnRF",
                connectionToken = "1da4fe9d-0fd2-45bc-90a9-014e57291d0f",
                endUserSessionId = "7YjG1LYkOkSY1iAcXGG8ZU",
                nhsNumbers = listOf("2227007273"),
                accountId = "4937786121",
                linkageKey = "tTALtBP3rLR16",
                userPatientLinkToken = "DbLYlUrwyGpgZ65Mlk6601"
        )
        
        val montelFrye = Patient(
                title =  "Mr",
                firstName =  "Montel",
                surname =  "Frye",
                dateOfBirth =  "1972-04-12T00:00:00",
                odsCode =  MockDefaults.DEFAULT_ODS_CODE,
                sessionId =  "2jM47sZ0ic4FIAcVogI4WI",
                connectionToken =  "7a3a3cf8-a797-4fcc-a4b9-629cdbe104fc",
                endUserSessionId =  "7YjG1LYkOkSY1iAcXGG8ZU",
                nhsNumbers =  listOf("0968764215"),
                accountId =  "4140044939",
                linkageKey =  "vVGO8bgV6fvPb",
                userPatientLinkToken =  "gpSWtREiH9499bPzix8v5b"
        )
    }
}
