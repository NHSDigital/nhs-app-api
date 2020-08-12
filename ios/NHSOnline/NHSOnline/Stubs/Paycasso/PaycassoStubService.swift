import Foundation

class PaycassoService  {
    
    let homeViewController: HomeViewController
    let appWebInterface: AppWebInterface
    let loggingService: LoggingService

    init(homeViewController controller: HomeViewController,
         appWebInterface: AppWebInterface,
         loggingService: LoggingService) {
        self.homeViewController = controller
        self.appWebInterface = appWebInterface
        self.loggingService = loggingService
    }
    
    func startTransaction(configData: Data) {
        loggingService.logError(message: "An attempt has been made to start the Paycasso process but it is currently disabled")
    }
}
