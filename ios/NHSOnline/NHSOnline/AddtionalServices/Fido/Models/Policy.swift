import SwiftyJSON

public struct Policy : Codable {
    public let assertionScheme: String?
    public let accepted: [[AAID]]
    public let tcDisplayPNGCharacteristics: String?
    
    init(){
        self.assertionScheme = "assertionScheme"
        self.accepted = [[AAID()]]
        self.tcDisplayPNGCharacteristics = "tcDisplayPNGCharacteristics"
    }
    
    init(with json: JSON) {
        let policyJSON = json["accepted"].arrayObject
        accepted = policyJSON as! [[AAID]]
        assertionScheme = nil
        tcDisplayPNGCharacteristics = nil
    }
}
