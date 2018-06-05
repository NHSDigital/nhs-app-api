import Foundation

struct ErrorMessage {
    let title:String
    let message:String?
    
    init(title:String, message:String? = nil) {
        self.title = title
        self.message = message
    }
}
