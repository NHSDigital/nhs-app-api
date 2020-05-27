public struct AAID: Codable {
    public let aaid: [String]?
    
    init(aaid: [String]? = nil){
        self.aaid = aaid
    }
}
