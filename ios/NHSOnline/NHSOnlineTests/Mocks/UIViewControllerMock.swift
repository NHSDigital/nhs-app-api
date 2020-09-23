//
//  UIViewControllerMock.swift
//  NHSOnlineTests
//
//  Created by James Lavery on 18/09/2020.
//  Copyright © 2020 NHS Digital. All rights reserved.
//

import Foundation
import UIKit

class UIViewControllerMock: UIViewController {
    
    var presentWasCalled = false;
    
    override func present(_ viewControllerToPresent: UIViewController, animated flag: Bool, completion: (() -> Void)? = nil) {
        presentWasCalled = true
    }
}
