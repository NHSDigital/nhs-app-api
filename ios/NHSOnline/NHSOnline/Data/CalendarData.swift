import Foundation

struct CalendarData: Codable {
    var subject: String?
    var body: String?
    var location: String?
    var startTimeEpochSeconds: Int64?
    var endTimeEpochSeconds: Int64?
    var source: JavaScriptInteractionMode

    init(subject: String?,
         body: String?,
         location: String?,
         startTimeEpochSeconds: Int64?,
         endTimeEpochSeconds: Int64?,
         source: JavaScriptInteractionMode
    ) {
        self.subject = subject
        self.body = body
        self.location = location
        self.startTimeEpochSeconds = startTimeEpochSeconds
        self.endTimeEpochSeconds = endTimeEpochSeconds
        self.source = source
    }
}
