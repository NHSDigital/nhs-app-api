using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    internal static class ClientRequestsServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureUserClientRequests(this IServiceCollection services)
        {
            services.AddTppClientRequest<Authenticate, AuthenticateReply, TppClientAuthenticatePost>()
                .AddTppClientRequest<TppUserSession, LogoffReply, TppClientLogoffPost>()
                .AddTppClientRequest<AddNhsUserRequest, AddNhsUserResponse, TppClientNhsUserPost>()
                .AddTppClientRequest<LinkAccount, LinkAccountReply, TppClientLinkAccountPost>();

            return services;
        }

        public static IServiceCollection ConfigurePatientClientRequests(this IServiceCollection services)
        {
            services.AddTppClientRequest<TppRequestParameters, ViewPatientOverviewReply, TppClientPatientOverviewPost>()
                .AddTppClientRequest<TppRequestParameters, PatientSelectedReply, TppClientPatientSelectedPost>()
                .AddTppClientRequest<TppRequestParameters, ViewPatientOverviewReply, TppClientPatientOverviewPost>();

            return services;
        }

        public static IServiceCollection ConfigureMedicationClientRequests(this IServiceCollection services)
        {
            services.AddTppClientRequest<TppRequestParameters, ListRepeatMedicationReply, TppClientListRepeatMedicationPost>()
                .AddTppClientRequest<(TppRequestParameters, RepeatPrescriptionRequest), RequestMedicationReply, TppClientOrderPrescriptionsPost>()
                .AddTppClientRequest<TppRequestParameters, RequestSystmOnlineMessagesReply, TppClientRequestSystmOnlineMessages>();

            return services;
        }

        public static IServiceCollection ConfigureAppointmentClientRequests(this IServiceCollection services)
        {
            services.AddTppClientRequest<(TppRequestParameters, BookingDates, AppointmentBookRequest), BookAppointmentReply, TppClientBookAppointmentSlotPost>()
                .AddTppClientRequest<(TppRequestParameters, AppointmentCancelRequest), CancelAppointmentReply, TppClientCancelAppointmentPost>()
                .AddTppClientRequest<TppUserSession, ListServiceAccessesReply, TppClientListServiceAccessesPost>()
                .AddTppClientRequest<(TppRequestParameters, AppointmentSlotsDateRange), ListSlotsReply, TppClientListSlotsPost>()
                .AddTppClientRequest< (TppRequestParameters, AppointmentViewType), ViewAppointmentsReply, TppClientViewAppointmentsPost>();

            return services;
        }

        public static IServiceCollection ConfigureTestResultClientRequests(this IServiceCollection services)
        {
            services
                .AddTppClientRequest<(TppRequestParameters, string, string), TestResultsViewReply, TppClientTestResultsView>()
                .AddTppClientRequest<(TppRequestParameters, string), TestResultsViewReply, TppClientTestResultsViewDetailed>();

            return services;
        }

        public static IServiceCollection ConfigureDocumentClientRequests(this IServiceCollection services)
        {
            services.AddTppClientRequest<TppRequestParameters, RequestPatientRecordReply, TppClientRequestPatientRecordPost>()
                .AddTppClientRequest<(TppRequestParameters, string), RequestBinaryDataReply, TppClientRequestBinaryDataPost>();

            return services;
        }

        public static IServiceCollection ConfigureIm1MessagingClientRequests(this IServiceCollection services)
        {
            services.AddTppClientRequest<TppUserSession, MessagesViewReply, TppClientMessagesViewPost>()
                .AddTppClientRequest<(TppRequestParameters, List<string>), MessagesMarkAsReadReply, TppClientMessagesMarkAsReadPost>()
                .AddTppClientRequest<TppUserSession, MessageRecipientsReply, TppClientMessageRecipientsPost>()
                .AddTppClientRequest<
                    (TppUserSession tppUserSession, string recipientIdentifier, string messageText),
                    MessageCreateReply,
                    TppClientMessagesSendMessagePost>();

            return services;
        }

        private static IServiceCollection AddTppClientRequest<TParams, TReply, TRequest>(
            this IServiceCollection services) where TRequest : class, ITppClientRequest<TParams, TReply>
            => services.AddTransient<ITppClientRequest<TParams, TReply>, TRequest>();
    }
}