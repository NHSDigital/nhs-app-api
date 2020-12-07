import Foundation

class Base64File {
    private let DATA_SCHEME = "data:"
    let fileName: String
    let fileMimeType: String
    let dataMediaType: String
    let data: String
    
    public init( fileName: String,
                 mimeType: String,
                 encoding: String ) {
        self.fileName = fileName
        self.fileMimeType = mimeType
        
        if (encoding.starts(with: DATA_SCHEME)) {
            let clobWithoutScheme = encoding.replacingOccurrences(of: DATA_SCHEME, with: "")
            // split to place file mime type into first element
            let clobSplitOnComma = clobWithoutScheme.split(separator: ",")
            // this should contain ';base64' at end, so strip that as well
            self.dataMediaType = String(clobSplitOnComma[0].split(separator: ";")[0])
            self.data = String(clobSplitOnComma[1])
        }
        else {
            self.dataMediaType = ""
            self.data = encoding
        }
    }
    
    public func decode() -> Data {
        return Data(
            base64Encoded: String(describing: self.data),
            options: .ignoreUnknownCharacters)!
    }
}
