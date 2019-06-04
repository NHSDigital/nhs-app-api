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

class LinkAccountBuilder(linkAccount: LinkAccount) : TppMappingBuilder("POST", "/tpp/") {
    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"
        val typeHeader = "type"
        val typeValue = "LinkAccount"

        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//LinkAccount[" +
                        "@apiVersion='${linkAccount.apiVersion}' and " +
                        "@accountId='${linkAccount.accountId}' and " +
                        "@passphrase='${linkAccount.passphrase}' and " +
                        "@lastName='${linkAccount.lastName}' and " +
                        "@organisationCode='${linkAccount.organisationCode}']")
    }

    fun respondWithSuccess(linkAccountReply: LinkAccountReply): Mapping {
        val responseBody = LinkAccountReply(
                linkAccountReply.passphrase,
                linkAccountReply.uuid,
                passphraseToLink = "passphraseToLink")

        val jaxbContext = JAXBContext.newInstance(LinkAccountReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(responseBody, stringWriter)
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
        val responseBody = Error(
                errorBody.errorCode,
                errorBody.userFriendlyMessage,
                errorBody.uuid
        )

        val jaxbContext = JAXBContext.newInstance(Error::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(responseBody, stringWriter)
        }

        return respondWith(httpResponse) {
            andXmlBody(stringWriter.toString())
                    .build()
        }
    }
}
