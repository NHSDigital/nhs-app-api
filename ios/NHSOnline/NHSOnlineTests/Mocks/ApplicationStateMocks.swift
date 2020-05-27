@testable import NHSOnline

class ApplicationStateMocks: ApplicationState {
    var isBlocked = false

    override func block() {
        isBlocked = true
    }

    override func unBlock() {
        isBlocked = false
    }
}
