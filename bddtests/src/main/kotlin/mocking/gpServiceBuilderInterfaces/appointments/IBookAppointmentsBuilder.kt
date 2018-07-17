package mocking.gpServiceBuilderInterfaces.appointments

import mocking.models.Mapping
import java.time.Duration


interface IBookAppointmentsBuilder {

    fun withDelay(delayMilliseconds: Duration): IBookAppointmentsBuilder

    fun respondWithSuccess(): Mapping

    fun respondWithUnavailableException(): Mapping

    fun respondWithConflictException(): Mapping

    fun respondWithUnknownException(): Mapping

    fun respondWithExceptionWhenNotEnabled(): Mapping

    fun respondWithExceptionWhenNotAvailable(): Mapping

    fun respondWithExceptionWhenInThePast(): Mapping

}