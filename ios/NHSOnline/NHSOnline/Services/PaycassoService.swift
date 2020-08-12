import Foundation

class PaycassoService {
    
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
        loggingService.logInfo(message: "Starting paycasso transaction")
        
        let paycassoConfiguration = PaycassoTransactionConfiguration(configData: configData)
        
        if (paycassoConfiguration == nil) {
            loggingService.logError(message: "Creating Paycasso transaction configuration returned nil, exiting journey and returning response to login")
            PaycassoErrors.init(errorMessage: "Creating Paycasso transaction configuration returned nil", appWebInterface: appWebInterface).callbackCustomResponseToWeb()
            return
        }
        
        if (paycassoConfiguration?.documentType == PaycassoDocumentTypes.document.rawValue) {
            loggingService.logError(message: "Unsupported document type 'Document' passed, exiting journey and returning response to login")
            PaycassoErrors.init(errorMessage: "Unsupported document type 'Document' passed", appWebInterface: appWebInterface).callbackCustomResponseToWeb()
            return
        }
        
        let credentials = PCSCredentials.init(
            token: paycassoConfiguration?.credentials.token,
            andHostUrl: NSURL(string: (paycassoConfiguration?.credentials.hostUrl)!) as URL?)
        
        let documentConfiguration = createDocConfigList(documentType: paycassoConfiguration!.documentType, echipRequested: (paycassoConfiguration?.externalReferences.hasNfcJourney)!)
        
        if (documentConfiguration.count == 0) {
            loggingService.logError(message: "No configuration could be found, exiting the paycasso journey")
            PaycassoErrors.init(errorMessage: "No configuration could be found", appWebInterface: appWebInterface).callbackCustomResponseToWeb()
            return
        }
        
        var transactionType: PCSTransactionType
        
        if (PaycassoTransactionTypes.docuSure.rawValue == paycassoConfiguration?.externalReferences.transactionType) {
            transactionType = PCSTransactionType.DocuSure
        } else if (PaycassoTransactionTypes.instaSure.rawValue == paycassoConfiguration?.externalReferences.transactionType) {
            transactionType = PCSTransactionType.InstaSure
        } else if (PaycassoTransactionTypes.veriSure.rawValue == paycassoConfiguration?.externalReferences.transactionType) {
            transactionType = PCSTransactionType.VeriSure
        } else {
            loggingService.logError(message: "Invalid transaction type used, exiting the paycasso journey")
            PaycassoErrors.init(errorMessage: "Invalid transaction type used", appWebInterface: appWebInterface).callbackCustomResponseToWeb()
            return
        }
        
        let request = PCSFlowRequest.init(
            transactionType: transactionType,
            documentConfigurations: documentConfiguration,
            consumerReference: paycassoConfiguration?.externalReferences.consumerReference)
            
        loggingService.logInfo(message: "Successfully created and obtained neccessary Paycasso requirements, starting paycasso flow using the \(transactionType) transaction type")
        
        PaycassoFlow.shared().start(
            with: credentials,
            request: request,
            configuration: configureSDK(),
            viewModel: PaycassoViewModel(),
            delegate: homeViewController);
    }
    
    func createDocConfigList( documentType:String, echipRequested:Bool) -> [PCSDocumentConfiguration]
    {
        switch documentType {
            case PaycassoDocumentTypes.passport.rawValue : return [
               PCSDocumentConfiguration(faceLocation: .FACE_FRONT_LOCATION,
                                       barcodeLocation: .NO_BARCODE,
                                       mrzLocation: .MRZ_FRONT_LOCATION,
                                       documentShape: .PASSPORT,
                                       bothSidesStatus: false,
                                       echipPresence: echipRequested,
                                       isDocCheckNeeded: true
               )]
               case PaycassoDocumentTypes.driversLicense.rawValue : return [
               PCSDocumentConfiguration(faceLocation: .FACE_FRONT_LOCATION,
                                       barcodeLocation: .NO_BARCODE,
                                       mrzLocation: .NO_MRZ,
                                       documentShape: .ID,
                                       bothSidesStatus: true,
                                       echipPresence: false,
                                       isDocCheckNeeded: true
               )]
               case PaycassoDocumentTypes.id.rawValue : return [
               PCSDocumentConfiguration(faceLocation: .FACE_FRONT_LOCATION,
                                       barcodeLocation: .NO_BARCODE,
                                       mrzLocation: .NO_MRZ,
                                       documentShape: .ID,
                                       bothSidesStatus: false,
                                       echipPresence: false,
                                       isDocCheckNeeded: true
               )]
               default : return []
        }
    }
    
    private func configureSDK() -> PaycassoFlowConfiguration {
        let paycassoFlowConfiguration = PaycassoFlowConfiguration()
        paycassoFlowConfiguration.displayCancelButton = true
        paycassoFlowConfiguration.isGeolocationRequired = false
        paycassoFlowConfiguration.displayDocumentPreview = true
        paycassoFlowConfiguration.receiveMrzData = true
        paycassoFlowConfiguration.receiveBarcodeData = false
        return paycassoFlowConfiguration
    }
}
