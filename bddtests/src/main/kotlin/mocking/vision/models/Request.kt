package mocking.vision.models

import java.time.LocalDate
import java.time.format.DateTimeFormatter
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "request")
data class Request(

        @XmlElement(namespace= "urn:vision", name = "date")
        var date: String?,

        @XmlElement(namespace = "urn:vision", name = "status")
        var status: StatusCode?,

        @XmlElement(namespace= "urn:vision", name = "repeat")
        var repeat: ArrayList<Repeat> = arrayListOf()) : Comparable<Request> {

        constructor() : this(null, null, arrayListOf())

        override fun compareTo(other: Request): Int {
                var result = 0
                val thisDate =  LocalDate.parse(date, DateTimeFormatter. ISO_LOCAL_DATE_TIME)
                val otherDate =  LocalDate.parse(other.date, DateTimeFormatter. ISO_LOCAL_DATE_TIME)

                if(thisDate < otherDate){
                        result = 1
                } else if (thisDate > otherDate) {
                        result = -1
                } else {
                        compareByStatus(this, other)
                }
                return result
        }

        fun compareByStatus(thisRequest: Request, otherRequest: Request ) : Int{
                var result : Int
                if (thisRequest.status?.statusValue == "Rejected" && otherRequest.status?.statusValue != "Rejected")  {
                        result = -1
                } else if (thisRequest.status?.statusValue == "Requested" &&
                        otherRequest.status?.statusValue != "Requested") {
                        result = 1
                } else if (otherRequest.status?.statusValue == "Rejected") {
                        result = 1
                } else {
                        result = -1
                }
                return result
        }
        }




