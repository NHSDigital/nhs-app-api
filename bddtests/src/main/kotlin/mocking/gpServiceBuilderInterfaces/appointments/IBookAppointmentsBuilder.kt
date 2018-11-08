package mocking.gpServiceBuilderInterfaces.appointments

import mocking.gpServiceBuilderInterfaces.IBuilderCommonResponses
import mocking.models.Mapping
import java.time.Duration


interface IBookAppointmentsBuilder:IBuilderCommonResponses {

    fun withDelay(delayMilliseconds: Duration): IBookAppointmentsBuilder

    fun respondWithSuccess(): Mapping

    fun respondWithConflictException(): Mapping

    fun respondWithBookingLimitException(): Mapping

    fun respondWithExceptionWhenNotAvailable(): Mapping

    fun respondWithExceptionWhenInThePast(): Mapping

}