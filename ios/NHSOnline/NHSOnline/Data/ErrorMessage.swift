import Foundation

struct ErrorMessage {
    let title:String
    let message:String?
    let accessibleMessage:String?
    
    init(title:String, message:String? = nil, accessibleMessage:String? = nil) {
        self.title = title
        self.message = message
        self.accessibleMessage = accessibleMessage
    }
}
