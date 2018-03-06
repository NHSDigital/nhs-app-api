import UIKit
import WebKit

class AppointmentsViewController: BaseTabBarViewController {
    let baseUrl = config().BaseUrl
    @IBOutlet weak var webView: WKWebView!
    
    override func viewDidLoad() {
        super.viewDidLoad()

        webView.loadPage(stringUrl: baseUrl)
    }
}
