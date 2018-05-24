package mocking.emis.models

data class SessionHolder(
        var clinicianId: Int? = null,
        var displayName: String? = null,
        var forenames: String? = null,
        var surname: String? = null,
        var title: String? = null,
        var sex: Sex? = null,
        var jobRole: String? = null,
        var languages: ArrayList<Language> = ArrayList()
)
