using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    internal sealed class UpdateMessageMapperStep
    {
        private readonly ILogger _logger;

        public UpdateMessageMapperStep(ILogger<MessageService> logger)
        {
            _logger = logger;
        }

        internal ProcessResult<(List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>), MessagePatchResult>
            Map(JsonPatchDocument<Message> messagePatchDocument)
        {
            var invalidOperations = new List<string>();
            var updates = new UpdateRecordBuilder<UserMessage>();
            var filters = new List<Expression<Func<UserMessage, bool>>>();
            foreach (var operation in messagePatchDocument.Operations)
            {
                switch (operation)
                {
                    case { OperationType: OperationType.Add, path: "/read" }:
                        updates.Set(x => x.ReadTime, operation.value.Equals(true) ? DateTime.UtcNow : (DateTime?)null);
                        filters.Add(userMessage => userMessage.ReadTime == null);
                        break;
                    default:
                        invalidOperations.Add($"{operation.path} : {operation.OperationType}");
                        break;
                }
            }

            if (invalidOperations.Any())
            {
                _logger.LogError($"Invalid operations in Message Update: {string.Join(", ", invalidOperations)}");
                return new MessagePatchResult.BadRequest();
            }

            return (filters, updates);
        }
    }
}