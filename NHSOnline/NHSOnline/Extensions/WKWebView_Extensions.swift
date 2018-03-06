import WebKit

extension WKWebView {
    
    func loadPage(stringUrl: String) {
        let urlRequest = URLRequest(url: URL(string: stringUrl)!)
        self.load(urlRequest)
    }
}

