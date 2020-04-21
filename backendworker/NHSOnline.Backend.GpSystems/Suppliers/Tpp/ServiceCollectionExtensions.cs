using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Services;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppPfsServices(this IServiceCollection services)
        {
            services.RegisterTppBaseServices();
            services.RegisterTppPrescriptionsServices();
            services.RegisterTppAppointmentsServices();
            services.RegisterTppDemographicsServices();
            services.RegisterTppPatientRecordServices();
            services.RegisterTppPatientPracticeMessagingServices();
            services.RegisterTppSessionServices();
            services.RegisterTppLinkedAccountsServices();
            return services;
        }

        private static IServiceCollection RegisterTppBaseServices(this IServiceCollection services)
        {
            services.AddTransient<TppHttpClientHandler>();
            services.AddTransient<TppHttpRequestIdentifier>();

            services.AddTransient<IGpSystem, TppGpSystem>();

            services.AddHttpClient<TppHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<TppHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<TppHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<TppHttpRequestIdentifier>>();

            services.AddTransient<TppTokenValidationService>();
            services.AddTransient<TppClientRequestBuilder>();
            services.AddTransient<Func<TppClientRequestBuilder>>(provider => provider.GetRequiredService<TppClientRequestBuilder>);
            services.AddTransient<TppClientRequestExecutor>();

            services.AddScoped<TppClientRequestLock>();
            services.AddTransient<ITppClientRequestSender, TppClientRequestSerializer>();
            services.AddTransient<TppClientRequestSender>();

            services
                .AddTppClientRequest<Authenticate, AuthenticateReply, TppClientAuthenticatePost>()
                .AddTppClientRequest<(TppRequestParameters, BookingDates, AppointmentBookRequest), BookAppointmentReply, TppClientBookAppointmentSlotPost>()
                .AddTppClientRequest<(TppRequestParameters, AppointmentCancelRequest), CancelAppointmentReply, TppClientCancelAppointmentPost>()
                .AddTppClientRequest<TppRequestParameters, ListRepeatMedicationReply, TppClientListRepeatMedicationPost>()
                .AddTppClientRequest<TppUserSession, ListServiceAccessesReply, TppClientListServiceAccessesPost>()
                .AddTppClientRequest<(TppRequestParameters, AppointmentSlotsDateRange), ListSlotsReply, TppClientListSlotsPost>()
                .AddTppClientRequest<TppUserSession, LogoffReply, TppClientLogoffPost>()
                .AddTppClientRequest<AddNhsUserRequest, AddNhsUserResponse, TppClientNhsUserPost>()
                .AddTppClientRequest<(TppRequestParameters, RepeatPrescriptionRequest), RequestMedicationReply, TppClientOrderPrescriptionsPost>()
                .AddTppClientRequest<TppUserSession, MessagesViewReply, TppClientMessagesViewPost>()
                .AddTppClientRequest<LinkAccount, LinkAccountReply, TppClientLinkAccountPost>()
                .AddTppClientRequest<TppRequestParameters, RequestSystmOnlineMessagesReply, TppClientRequestSystmOnlineMessages>()
                .AddTppClientRequest<(TppRequestParameters, string), TestResultsViewReply, TppClientTestResultsViewDetailed>()
                .AddTppClientRequest< (TppRequestParameters, AppointmentViewType), ViewAppointmentsReply, TppClientViewAppointmentsPost>()
                .AddTppClientRequest<TppUserSession, MessageRecipientsReply, TppClientMessageRecipientsPost>()
                .AddTppClientRequest<TppRequestParameters, ViewPatientOverviewReply, TppClientPatientOverviewPost>()
                .AddTppClientRequest<TppRequestParameters, PatientSelectedReply, TppClientPatientSelectedPost>()
                .AddTppClientRequest<TppRequestParameters, RequestPatientRecordReply, TppClientRequestPatientRecordPost>()
                .AddTppClientRequest<(TppRequestParameters, string), RequestBinaryDataReply, TppClientRequestBinaryDataPost>()
                .AddTppClientRequest<(TppRequestParameters, string, string), TestResultsViewReply, TppClientTestResultsView>()
                .AddTppClientRequest<(TppUserSession, string, string), MessageCreateReply, TppClientMessagesSendMessagePost>();

            return services;
        }

        public static IServiceCollection RegisterTppCidServices(this IServiceCollection services)
        {
            services.RegisterTppBaseServices();
            services.RegisterTppLinkageServices();
            services.RegisterTppIm1ConnectionServices();

            return services;
        }

        private static IServiceCollection AddTppClientRequest<TParams, TReply, TRequest>(
            this IServiceCollection services) where TRequest : class, ITppClientRequest<TParams, TReply>
            => services.AddTransient<ITppClientRequest<TParams, TReply>, TRequest>();
    }
}