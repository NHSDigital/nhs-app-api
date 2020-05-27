import WebKit

class WKScriptMessageMock: WKScriptMessage {
    private let _body: Any
    var _name: String
    var _frameInfo: WKFrameInfoMock
    
    init(name: String, body: Any, url: String) {
        self._body = body
        self._name = name
        self._frameInfo = WKFrameInfoMock(url: url)
    }
    
    override var body: Any { get { return _body } }

    override var name: String { get { return _name } }
    
    override var frameInfo: WKFrameInfo { get { return _frameInfo } }
}

class AddEventToCalendarWKScriptMessageMock : WKScriptMessageMock {
    var subject: String?
    var messageBody: String?
    var location: String?
    var startTimeEpochSeconds: NSNumber?
    var endTimeEpochSeconds: NSNumber?

    init(subject: String?,
         messageBody: String?,
         location: String?,
         startTimeEpochSeconds: NSNumber?,
         endTimeEpochSeconds: NSNumber?
    ) {
        let body = """
                   {
                   "subject": \(subject != nil ? "\"\(subject!)\"" : "null"),
                   "body": \(messageBody != nil ? "\"\(messageBody!)\"" : "null"),
                   "location": \(location != nil ? "\"\(location!)\"" : "null"),
                   "startTimeEpochInSeconds": \(startTimeEpochSeconds != nil ? "\(startTimeEpochSeconds!)" : "null"),
                   "endTimeEpochInSeconds": \(endTimeEpochSeconds != nil ? "\(endTimeEpochSeconds!)" : "null"),
                   }
                   """
        super.init(name: "addEventToCalendar", body: body, url: "http://www.example.com")
        
        self.subject = subject
        self.messageBody = messageBody
        self.location = location
        self.startTimeEpochSeconds = startTimeEpochSeconds
        self.endTimeEpochSeconds = endTimeEpochSeconds
    }
}
