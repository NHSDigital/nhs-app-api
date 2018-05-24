package mocking.emis.models.appointmentslots

import mocking.emis.models.SessionType

class MetaSession(var sessionName: String? = null,
                  var sessionId: Int? = null,
                  var locationId: Int? = null,
                  var defaultDuration: Int? = null,
                  var sessionType: SessionType? = null,
                  var numberOfSlots: Int? = null,
                  var clinicianIds: List<Int> = emptyList())