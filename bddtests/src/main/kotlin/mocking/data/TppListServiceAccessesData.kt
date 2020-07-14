package mocking.data

import mocking.tpp.models.ListServiceAccessesReply
import mocking.tpp.models.ServiceAccess

class TppListServiceAccessesData {

    fun enableService(serviceDescription: String, serviceIdentifier: String, status:
    String, statusDesc: String):
            ListServiceAccessesReply{
        val listServiceAccessesReply = ListServiceAccessesReply()
        val services = mutableListOf<ServiceAccess>()
        services.add(ServiceAccess(
                description = serviceDescription,
                status = status,
                statusDesc = statusDesc,
                serviceIdentifier = serviceIdentifier
                )
            )
        listServiceAccessesReply.Services = services
        return listServiceAccessesReply
    }

    fun enableServices(servicesToEnable: List<ServiceAccess>): ListServiceAccessesReply{
        return ListServiceAccessesReply(Services = servicesToEnable.toMutableList())
    }
}
