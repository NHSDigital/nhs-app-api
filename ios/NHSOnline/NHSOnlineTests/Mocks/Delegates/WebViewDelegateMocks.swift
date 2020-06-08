import Foundation
import WebKit
@testable import NHSOnline

class WebViewDelegateMocks : WebViewDelegate {
    override init(controller: HomeViewController,
                  knownServiceProvider: KnownServicesProtocol,
                  configurationServiceProvider: ConfigurationServiceProtocol,
                  webAppInterface: WebAppInterface,
                  appWebInterface: AppWebInterface = AppWebInterfaceMocks(webView: WebViewMocks())) {
        super.init(controller: controller,
                   knownServiceProvider: knownServiceProvider,
                   configurationServiceProvider: configurationServiceProvider,
                   webAppInterface: webAppInterface,
                   appWebInterface: appWebInterface)
    }
}
