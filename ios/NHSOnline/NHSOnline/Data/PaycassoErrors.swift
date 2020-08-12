import Foundation

struct PaycassoErrors {
    var appWebInterface: AppWebInterface?
    var errorMessage: String

    init(errorMessage: String, appWebInterface: AppWebInterface) {
        self.appWebInterface = appWebInterface
        self.errorMessage = errorMessage
    }

    func callbackCustomResponseToWeb() {
        
        self.appWebInterface?.paycassoCustomFailureCallback(isFaceMatched: false, errorMessage: self.errorMessage)
    }
    
    func callbackPaycassoResponseToWeb(code: Int) {
        
        self.appWebInterface?.paycassoResponseFailureCallback(isFaceMatched: false, errorCode: code, errorMessage: self.errorMessage)
    }
}
