@testable import NHSOnline

import XCTest

class WebViewControllerTests: XCTestCase {
    let TEST_URL = "test1"
    var webViewController: WebViewController!
    var wvd: WebViewDelegate!

    override func setUp() {
        super.setUp()
        
        let storyboard = UIStoryboard(name: "Main", bundle: nil)
        let vc: WebViewController = storyboard.instantiateViewController(withIdentifier: "WebViewController")
            as! WebViewController
        
        let hvc = storyboard.instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        
        let ks: KnownServices = KnownServices(config: config())
        let wai: WebAppInterface = WebAppInterface(controller: hvc!)
        
        wvd = MockWebViewDelegate(controller: hvc!, knownServices: ks, webAppInterface: wai)
        
        webViewController = vc
        webViewController.webViewDelegate = wvd
        
        webViewController.loadView()
    }

    func test_failedUrl_only_updated_if_connected_to_network () {
        webViewController.loadPage(url : TEST_URL, isConnectedToNetwork: true)
        assert((wvd.failedUrl?.absoluteString ?? nil)! == TEST_URL)
        
        let new_url = "test2"
        webViewController.loadPage(url: new_url, isConnectedToNetwork: false)
        assert((wvd.failedUrl?.absoluteString ?? nil)! == TEST_URL)
    }
    
    func test_HomeViewController_calls_clearSelectedTab_always_on_reloadWebView() {
        let hvc = wvd.viewController
        let selectedItem = UITabBarItem()
        hvc.tabBar.selectedItem = selectedItem
        hvc.selectedTab = 2
        
        assert(wvd.failedUrl == nil)
        
        webViewController.reloadWebView()
        
        assert(hvc.tabBar.selectedItem == nil)
        assert(hvc.selectedTab == nil)
    }
}
