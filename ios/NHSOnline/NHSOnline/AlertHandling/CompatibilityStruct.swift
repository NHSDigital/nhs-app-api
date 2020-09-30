import Foundation

struct Compatibility {
    var canUpdate: Bool
    var correctVersion: Bool
    
    init(canUpdate: Bool, correctVersion: Bool) {
        self.canUpdate = canUpdate
        self.correctVersion = correctVersion
    }
}
