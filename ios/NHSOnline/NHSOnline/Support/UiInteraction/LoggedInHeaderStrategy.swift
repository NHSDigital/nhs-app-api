import Foundation

class LoggedInHeaderStrategy: HeaderStrategyProtocol {
    var viewController: HomeViewController
    
    init(controller: HomeViewController) {
           self.viewController = controller
    }
    
    func apply(integrationLevel: IntegrationLevel) {
        switch integrationLevel {
        case .Gold,
             .SilverWithoutWebNavigation,
             .SilverWithWebNavigation:
            viewController.setVisibilityOfHeaderAndMenuBars(headerType: .Full)
        case .GoldOverlay:
            viewController.setVisibilityOfHeaderAndMenuBars(headerType: .SlimBack)
        case .Bronze, .GoldWithNoHeaders, .Unknown:
            viewController.setVisibilityOfHeaderAndMenuBars(headerType: .None)
        }
    }
}
