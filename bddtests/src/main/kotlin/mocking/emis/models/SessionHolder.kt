package mocking.emis.models

import mocking.emis.demographics.Sex

data class SessionHolder(
        var clinicianId: Int,
        var displayName: String? = null,
        var forenames: String? = null,
        var surname: String? = null,
        var title: String? = null,
        var sex: Sex? = null,
        var jobRole: String? = null,
        var languages: ArrayList<Language> = ArrayList()
)
