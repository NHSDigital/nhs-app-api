using System;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddTransient<ITppClient, TppClient>();

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
                .AddTppClientRequest<(TppUserSession, BookAppointment), BookAppointmentReply, TppClientBookAppointmentSlotPost>()
                .AddTppClientRequest<(CancelAppointment, string), CancelAppointmentReply, TppClientCancelAppointmentPost>()
                .AddTppClientRequest<TppUserSession, ListRepeatMedicationReply, TppClientListRepeatMedicationPost>()
                .AddTppClientRequest<TppUserSession, ListServiceAccessesReply, TppClientListServiceAccessesPost>()
                .AddTppClientRequest<(ListSlots, string), ListSlotsReply, TppClientListSlotsPost>()
                .AddTppClientRequest<TppUserSession, LogoffReply, TppClientLogoffPost>()
                .AddTppClientRequest<AddNhsUserRequest, AddNhsUserResponse, TppClientNhsUserPost>()
                .AddTppClientRequest<(TppUserSession, RequestMedication), RequestMedicationReply, TppClientOrderPrescriptionsPost>()
                .AddTppClientRequest<TppUserSession, ViewPatientOverviewReply, TppClientPatientOverviewPost>()
                .AddTppClientRequest<TppUserSession, PatientSelectedReply, TppClientPatientSelectedPost>()
                .AddTppClientRequest<TppUserSession, MessagesViewReply, TppClientMessagesViewPost>()
                .AddTppClientRequest<LinkAccount, LinkAccountReply, TppClientLinkAccountPost>()
                .AddTppClientRequest<TppUserSession, RequestPatientRecordReply, TppClientRequestPatientRecordPost>()
                .AddTppClientRequest<(TppUserSession, string), RequestBinaryDataReply, TppClientRequestBinaryDataPost>()
                .AddTppClientRequest<(RequestSystmOnlineMessages, string), RequestSystmOnlineMessagesReply, TppClientRequestSystmOnlineMessages>()
                .AddTppClientRequest<(TppUserSession, string, string), TestResultsViewReply, TppClientTestResultsView>()
                .AddTppClientRequest<(TppUserSession, string), TestResultsViewReply, TppClientTestResultsViewDetailed>()
                .AddTppClientRequest<(ViewAppointments, string), ViewAppointmentsReply, TppClientViewAppointmentsPost>()
                .AddTppClientRequest<TppUserSession, MessageRecipientsReply, TppClientMessageRecipientsPost>();

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