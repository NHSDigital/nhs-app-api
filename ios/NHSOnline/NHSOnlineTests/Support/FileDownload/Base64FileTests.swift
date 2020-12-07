import Foundation
import XCTest

@testable import NHSOnline

class Base64FileTests: XCTestCase {
    let fileName = "bazzFile.pdf"
    let fileMimeType = "application/pdf"
    
    func test_base46File_using_dataScheme() {
        let clobWithScheme = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII="

        let base64File = Base64File(
            fileName: fileName,
            mimeType: fileMimeType,
            encoding: clobWithScheme
        )

        XCTAssertEqual(base64File.dataMediaType, "image/png", "Media Type mismatch")
        XCTAssertEqual(base64File.data, "iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII=", "data mismatch")
    }

    func test_base46File_using_broken_dataScheme() {
        let clobWithScheme = "data:image/png,iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII="

        let base64File = Base64File(
                fileName: fileName,
                mimeType: fileMimeType,
                encoding: clobWithScheme
        )

        XCTAssertEqual(base64File.dataMediaType, "image/png", "Media Type mismatch")
        XCTAssertEqual(base64File.data, "iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII=", "data mismatch")
    }
    
    func test_base46File_base64Clob() {
        let clobWithScheme = "iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII="

        let base64File = Base64File(
            fileName: fileName,
            mimeType: fileMimeType,
            encoding: clobWithScheme
        )

        XCTAssertEqual(base64File.dataMediaType, "", "Media Type mismatch")
        XCTAssertEqual(base64File.data, "iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII=", "data mismatch")
    }
}
