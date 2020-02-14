import Foundation

protocol BaseSchemeHandler {
    var Scheme: String? { get }
    
    func handle(url: URL) -> Bool
}
