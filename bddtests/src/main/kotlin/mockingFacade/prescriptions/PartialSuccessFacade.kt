package mockingFacade.prescriptions

data class PartialSuccessFacade(
        var unsuccessfulMedications: List<String> = arrayListOf(),
        var successfulMedications: List<String> = arrayListOf()
)
