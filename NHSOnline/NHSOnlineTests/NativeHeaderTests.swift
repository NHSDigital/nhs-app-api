//
//  NativeHeaderTests.swift
//  NHSOnlineTests
//
//  Created by Nathanael Holmes on 18/04/2018.
//  Copyright © 2018 NHS Digital. All rights reserved.
//

import XCTest
@testable import NHSOnline

class NativeHeaderTests: XCTestCase {
    var viewController: HomeViewController?
    var webViewDelegate: WebViewDelegate?
    var knownServices: KnownServices?
    
    override func setUp() {
        super.setUp()
        knownServices = KnownServices(config: config())
        viewController = UIStoryboard(name: "Main", bundle: nil).instantiateViewController(withIdentifier: "HomeViewController") as? HomeViewController
        webViewDelegate = WebViewDelegate(controller: viewController!, knownServices: knownServices!)
    }
    
    override func tearDown() {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
        super.tearDown()
    }
    
    func test_navBarHeaderTitleIsCorrectlyUpdated() {
        let testHeaderText = "Test Header Text"
        viewController?.updateHeaderText(headerText: testHeaderText)
        XCTAssert(viewController?.headerBar.headerTitle.text == testHeaderText)
    }
    
}
