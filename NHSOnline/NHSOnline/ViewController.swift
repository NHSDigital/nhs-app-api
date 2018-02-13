import UIKit
import WebKit

class ViewController: UIViewController {
    @IBOutlet weak var webView: WKWebView!
    @IBOutlet weak var tabBar: UITabBar!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        loadDefaultWebPage()
    }
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }
    
    private func loadDefaultWebPage() {
        let webViewEndpointUrl = Bundle.main.infoDictionary!["BaseUrl"] as! String
        let urlRequest = URLRequest(url: URL(string: webViewEndpointUrl)!)
        webView.load(urlRequest)
    }
}

