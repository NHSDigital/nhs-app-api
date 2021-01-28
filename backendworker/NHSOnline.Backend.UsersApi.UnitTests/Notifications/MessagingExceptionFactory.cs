using System.Net;
using System.Reflection;
using FluentAssertions;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using BindingFlags = System.Reflection.BindingFlags;

namespace NHSOnline.Backend.UsersApi.UnitTests.Notifications
{
    /// <summary>
    /// It is no longer possible to create a MessagingException directly as its constructors are all internal
    /// </summary>
    internal static class MessagingExceptionFactory
    {
        internal static MessagingException Create()
        {
            var detail = typeof(MessagingExceptionDetail)
                .GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[]
                    {
                        typeof(ExceptionErrorCodes),
                        typeof(string),
                        typeof(MessagingExceptionDetail.ErrorLevelType),
                        typeof(HttpStatusCode),
                        typeof(string)
                    },
                    System.Array.Empty<ParameterModifier>())
                ?.Invoke(new object[]
                {
                    ExceptionErrorCodes.BadRequest,
                    "Messaging Exception",
                    MessagingExceptionDetail.ErrorLevelType.ServerError,
                    HttpStatusCode.BadRequest,
                    "TrackingId"
                }) as MessagingExceptionDetail;
            detail.Should().NotBeNull("MessagingExceptionDetail instance should have been constructed");

            var messagingException = typeof(MessagingException)
                .GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[]
                    {
                        typeof(MessagingExceptionDetail),
                        typeof(bool)
                    },
                    System.Array.Empty<ParameterModifier>())
                ?.Invoke(new object[]
                {
                    detail,
                    true
                }) as MessagingException;
            messagingException.Should().NotBeNull("MessagingExceptionDetail instance should have been constructed");

            return messagingException;
        }
    }
}