package mocking.emis.models

data class MedicationMixture (
     var mixtureName : String,
     var constituents: MutableList<MedicationMixtureItem>
)
