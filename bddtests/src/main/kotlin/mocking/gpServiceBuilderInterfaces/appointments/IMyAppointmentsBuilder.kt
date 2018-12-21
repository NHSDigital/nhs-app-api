package mocking.gpServiceBuilderInterfaces.appointments

import mocking.gpServiceBuilderInterfaces.IBuilderCommonResponses
import mocking.models.Mapping
import mockingFacade.appointments.MyAppointmentsFacade

interface IMyAppointmentsBuilder : IBuilderCommonResponses {

    fun respondWithSuccess(facade: MyAppointmentsFacade): Mapping

    fun respondWithSuccess(body: String): Mapping

}