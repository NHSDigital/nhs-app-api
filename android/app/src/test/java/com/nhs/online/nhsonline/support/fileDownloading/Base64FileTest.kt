package com.nhs.online.nhsonline.support.fileDownloading

import com.nhaarman.mockitokotlin2.verify
import junit.framework.Assert.assertEquals
import org.junit.Test

class Base64FileTest {
    private val fileName = "bazzFile.pdf"
    private val fileMimeType = "application/pdf"

    @Test
    fun base46File_using_dataScheme() {
        val clobWithScheme = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII="

        val base64File = Base64File(
                fileName,
                fileMimeType,
                clobWithScheme
        )

        assertEquals("image/png", base64File.dataMediaType)
        assertEquals("iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII=", base64File.data)
    }

    @Test
    fun base46File_using_broken_dataScheme() {
        val clobWithScheme = "data:image/png,iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII="

        val base64File = Base64File(
                fileName,
                fileMimeType,
                clobWithScheme
        )

        assertEquals("image/png", base64File.dataMediaType)
        assertEquals("iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII=", base64File.data)
    }

    @Test
    fun base46File_base64Clob() {
        val clobWithScheme = "iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII="

        val base64File = Base64File(
                fileName,
                fileMimeType,
                clobWithScheme
        )

        assertEquals("", base64File.dataMediaType)
        assertEquals("iVBORw0KGgoAAAANSUhEUgAAAAgAAAALCAYAAABCm8wlAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QoPAxIb88htFgAAABl0RVh0Q29tbWVudABDcmVhdGVkIHdpdGggR0lNUFeBDhcAAACxSURBVBjTdY6xasJgGEXP/RvoonvAd8hDyD84+BZBEMSxL9GtQ8Fis7i6BkGI4DP4CA4dnQON3g6WNjb2wLd8nAsHWsR3D7JXt18kALFwz2dGmPVhJt0IcenUDVsgu91eCRZ9IOMfAnBvSCz8I3QYL0yV6zfyL+VUxKWfMJuOEFd+dE3pC1Finwj0HfGBeKGmblcFTIN4U2C4m+hZAaTrASSGox6YV7k+ARAp4gIIOH0BmuY1E5TjCIUAAAAASUVORK5CYII=", base64File.data)
    }
}
