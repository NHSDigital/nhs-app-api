package mocking.emis.session

import mocking.emis.models.AssociationType
import mocking.emis.models.UserPatientLink
import org.joda.time.DateTime

class CreateSessionResponseModel(val sessionId: String,
                                 val title: String,
                                 val firstName: String,
                                 val surname: String,
                                 userPatientLinkToken: String,
                                 odsCode: String,
                                 associationType: AssociationType) {
    val lastAccessTime = DateTime.now()
    var userPatientLinks = mutableListOf(UserPatientLink(title,
                                                   firstName, surname,
                                                   userPatientLinkToken,
                                                   odsCode, associationType))
}
