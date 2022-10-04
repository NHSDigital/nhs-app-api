using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.JsonPatch;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.Areas.Messages
{
    internal sealed class UpdateMessageMapperStep
    {
        internal ProcessResult<(List<Expression<Func<UserMessage, bool>>>, UpdateRecordBuilder<UserMessage>),
                MessagePatchResult>
            Map(JsonPatchDocument<Message> messagePatchDocument, MessagePatchType patchType)
        {
            var updates = new UpdateRecordBuilder<UserMessage>();
            var filters = new List<Expression<Func<UserMessage, bool>>>();
            var operation = messagePatchDocument.Operations.FirstOrDefault();
            if (operation == null)
            {
                return (filters, updates);
            }

            switch (patchType)
            {
                case MessagePatchType.Read:
                    updates.Set(x => x.ReadTime, operation.value.Equals(true) ? DateTime.UtcNow : (DateTime?) null);
                    filters.Add(userMessage => userMessage.ReadTime == null);
                    break;
                case MessagePatchType.Reply:
                    var userResponse = Convert.ToString(operation.value, CultureInfo.InvariantCulture);
                    updates.Set(x => x.Reply.Response, !string.IsNullOrEmpty(userResponse) ? userResponse : null);
                    updates.Set(x => x.Reply.ResponseSentDateTime,
                        !string.IsNullOrEmpty(userResponse) ? DateTime.UtcNow : (DateTime?) null);
                    filters.Add(userMessage => userMessage.Reply != null &&
                                               userMessage.Reply.Response == null &&
                                               userMessage.Reply.ResponseSentDateTime == null &&
                                               userMessage.Reply.Options.Any(s => s.Code == userResponse));
                    break;
                case MessagePatchType.ReplyStatus:
                    var userReplyStatusResponse = Convert.ToString(operation.value, CultureInfo.InvariantCulture);
                    updates.Set(x => x.Reply.Status, !string.IsNullOrEmpty(userReplyStatusResponse) ? userReplyStatusResponse : null);
                    updates.Set(x => x.Reply.ResponseCompletedDateTime,
                        !string.IsNullOrEmpty(userReplyStatusResponse) ? DateTime.UtcNow : (DateTime?) null);
                    filters.Add(userMessage => userMessage.Reply != null);
                    break;
            }

            return (filters, updates);
        }
    }
}