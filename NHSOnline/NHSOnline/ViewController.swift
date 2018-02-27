import UIKit
import WebKit

class ViewController: UIViewController, UITabBarDelegate {
    let BASE_URL_BUNDLE_KEY = "BaseUrl"
    let NHS111_URL_BUNDLE_KEY = "nhs111Url"
    
    let SYMPTOMS_TAG = 0
    
    @IBOutlet weak var webView: WKWebView!
    @IBOutlet weak var tabBar: UITabBar!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        tabBar.delegate = self
        
        loadDefaultWebPage()
    }
    
    func tabBar(_ tabBar: UITabBar, didSelect item: UITabBarItem) {
        switch item.tag {
        case SYMPTOMS_TAG:
            onSymptomsMenuSelected()
        default:
            loadDefaultWebPage()
        }
    }
    
    private func onSymptomsMenuSelected() {
        let nhs111 = Bundle.main.infoDictionary![NHS111_URL_BUNDLE_KEY] as! String
        loadPage(urlString: nhs111)
    }
    
    private func loadDefaultWebPage() {
        let baseUrl = Bundle.main.infoDictionary![BASE_URL_BUNDLE_KEY] as! String
        loadPage(urlString: baseUrl)
    }
    
    private func loadPage(urlString url:String) {
        let urlRequest = URLRequest(url: URL(string: url)!)
        webView.load(urlRequest)
    }
}

