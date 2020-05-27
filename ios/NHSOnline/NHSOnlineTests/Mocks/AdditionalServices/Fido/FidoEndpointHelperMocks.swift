import Foundation
@testable import NHSOnline

class FidoEndpointHelperMocks: FidoEndpointHelper {
    var isVisible = true
    override var requestRequestEndpoint : String? {
        get {
            return "Test"
        }
        set {
            super.requestRequestEndpoint = newValue
        }
    }
    override var registrationResponseEndpoint: String? {
        get {
            return "Test"
        }
        set {
            super.registrationResponseEndpoint = newValue
        }
    }
    
    override var deregistrationRequestEndpoint: String? {
        get {
            return "Test"
        }
        set {
            super.deregistrationRequestEndpoint = newValue
        }
    }
    
    override var authenticationRequestEndpoint: String? {
        get {
            return "Test"
        }
        set {
            super.authenticationRequestEndpoint = newValue
        }
    }
    
    override func configure() {
        return
    }
}
