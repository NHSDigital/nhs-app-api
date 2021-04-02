package mocking.tpp.registration
import constants.ErrorResponseCodeTpp
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.LinkAccount
import mocking.tpp.models.LinkAccountReply
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class LinkAccountBuilder(val linkAccount: LinkAccount) : TppMappingBuilder("POST", "/tpp/") {
    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"
        val typeHeader = "type"
        val typeValue = "LinkAccount"

        val dateOfBirthInCorrectFormat =
            if (linkAccount.dateOfBirth.endsWith("T00:00:00")) linkAccount.dateOfBirth
            else linkAccount.dateOfBirth.plus("T00:00:00")

        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//LinkAccount[" +
                        "@apiVersion='${linkAccount.apiVersion}' and " +
                        "@lastName='${linkAccount.lastName}' and " +
                        "@dateOfBirth='${dateOfBirthInCorrectFormat}' and " +
                        "@accountId='${linkAccount.accountId}' and " +
                        "@passphrase='${linkAccount.passphrase}' and " +
                        "@organisationCode='${linkAccount.organisationCode}']")
    }

    fun respondWithSuccess(linkAccountReply: LinkAccountReply): Mapping {
        val jaxbContext = JAXBContext.newInstance(LinkAccountReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(linkAccountReply, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString())
                    .build()
        }
    }

    fun respondWithInvalidLinkageCredentials(): Mapping {
        val invalidLinkageCredentialsError = Error(
                ErrorResponseCodeTpp.INVALID_LINKAGE_CREDENTIALS,
                "There was a problem linking your account." +
                        " Please contact Kainos GP Demo Unit to complete the online account registration."
        )

        return respondWithError(invalidLinkageCredentialsError)
    }

    fun respondWithError(errorBody: Error, httpResponse : Int = HttpStatus.SC_OK): Mapping {
        val jaxbContext = JAXBContext.newInstance(Error::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(errorBody, stringWriter)
        }

        return respondWith(httpResponse) {
            andXmlBody(stringWriter.toString())
                    .build()
        }
    }
}
