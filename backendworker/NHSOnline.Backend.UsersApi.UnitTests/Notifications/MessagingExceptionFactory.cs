#nullable enable
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
        internal static MessagingException CreateMessagingException()
        {
            var detail = GetMessagingExceptionConstructorInfoWithHttpErrorCode()
                ?.Invoke(new object[]
                {
                    ExceptionErrorCodes.BadRequest,
                    "Messaging Exception",
                    MessagingExceptionDetail.ErrorLevelType.ServerError,
                    HttpStatusCode.BadRequest,
                    "TrackingId"
                }) as MessagingExceptionDetail;
            
            detail.Should().NotBeNull("MessagingExceptionDetail instance should have been constructed");

            var messagingException = GetMessagingExceptionConstructorInfoWithExceptionDetail()?
                .Invoke(new object[]
                {
                    detail!,
                    true
                }) as MessagingException;
            
            messagingException.Should().NotBeNull("MessagingExceptionDetail instance should have been constructed");
            
            return messagingException!;
        }
        
        internal static MessagingEntityNotFoundException CreateMessagingEntityNotFoundException()
        {
            var detail = GetMessagingExceptionConstructorInfoWithHttpErrorCode()
                ?.Invoke(new object[]
                {
                    ExceptionErrorCodes.ResourceNotFound,
                    "Messaging Exception",
                    MessagingExceptionDetail.ErrorLevelType.ServerError,
                    HttpStatusCode.NotFound,
                    "TrackingId"
                }) as MessagingExceptionDetail;
            
            detail.Should().NotBeNull("MessagingExceptionDetail instance should have been constructed");

            var messagingException = GetMessagingEntityNotFoundExceptionConstructorInfoWithExceptionDetail()?
                .Invoke(new object[]
                {
                    detail!
                }) as MessagingEntityNotFoundException;
            
            messagingException.Should().NotBeNull("MessagingExceptionDetail instance should have been constructed");

            return messagingException!;
        }
        private static ConstructorInfo? GetMessagingExceptionConstructorInfoWithHttpErrorCode()
        {
            return typeof(MessagingExceptionDetail)
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
                    System.Array.Empty<ParameterModifier>());
        }
        
        private static ConstructorInfo? GetMessagingExceptionConstructorInfoWithExceptionDetail()
        {
            return typeof(MessagingException)
                .GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[]
                    {
                        typeof(MessagingExceptionDetail),
                        typeof(bool)
                    },
                    System.Array.Empty<ParameterModifier>());
        }
        
        private static ConstructorInfo? GetMessagingEntityNotFoundExceptionConstructorInfoWithExceptionDetail()
        {
            return typeof(MessagingEntityNotFoundException)
                .GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[]
                    {
                        typeof(MessagingExceptionDetail)
                    },
                    System.Array.Empty<ParameterModifier>());
        }
       
    }
}