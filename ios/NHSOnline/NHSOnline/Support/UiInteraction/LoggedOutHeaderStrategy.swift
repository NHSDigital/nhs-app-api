import Foundation

class LoggedOutHeaderStrategy: HeaderStrategyProtocol {
    var viewController: HomeViewController
    
    init(controller: HomeViewController) {
           self.viewController = controller
    }
    
    func apply(integrationLevel: IntegrationLevel) {
        switch integrationLevel {
        case .Gold:
            viewController.setVisibilityOfHeaderAndMenuBars(headerType: .None)
        case .GoldOverlay,
             .SilverWithWebNavigation:
            viewController.setVisibilityOfHeaderAndMenuBars(headerType: .SlimBack)
        case .SilverWithoutWebNavigation:
            viewController.setVisibilityOfHeaderAndMenuBars(headerType: .SlimClose)
        case .Bronze, .GoldWithNoHeaders, .Unknown:
            viewController.setVisibilityOfHeaderAndMenuBars(headerType: .None)
        }
    }
}
