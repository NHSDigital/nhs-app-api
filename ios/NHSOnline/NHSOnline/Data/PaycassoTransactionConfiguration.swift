import Foundation

struct PaycassoTransactionConfiguration {
    var credentials: (hostUrl: String, token: String)
    var externalReferences: (consumerReference: String, transactionReference: String, appUserId: String, deviceId: String, hasNfcJourney: Bool, transactionType: String)
    var documentType: String
    
    init?(configData: Data) {
        do {
            let json = try JSONSerialization.jsonObject(with: configData, options: []) as? [String: Any]
            guard let credentials = json!["credentials"] as? [String: String],
                let hostUrl = credentials["hostUrl"],
                let token = credentials["token"],
                let externalReferences = json!["externalReferences"] as? [String: Any],
                let consumerReference = externalReferences["consumerReference"] as? String,
                let transactionReference = externalReferences["transactionReference"] as? String,
                let deviceId = externalReferences["deviceId"] as? String,
                let appUserId = externalReferences["appUserId"] as? String,
                let hasNfcJourney = externalReferences["hasNfcJourney"] as? Bool,
                let transactionType = externalReferences["transactionType"] as? String,
                let transactionDetails = json!["transactionDetails"] as? [String: String],
                let documentType = transactionDetails["documentType"]
            
            else {
                return nil
            }

            self.credentials = (hostUrl, token)
            self.externalReferences = (consumerReference, transactionReference, appUserId, deviceId, hasNfcJourney, transactionType)
            self.documentType = documentType
        } catch {
            return nil
        }
    }
}
