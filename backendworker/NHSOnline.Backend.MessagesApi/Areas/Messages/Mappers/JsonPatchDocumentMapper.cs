using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Mappers
{
    internal class JsonPatchDocumentMapper :
        IMapper<JsonPatchDocument<Message>, JsonPatchDocument<UserMessage>>
    {
        private readonly ILogger<JsonPatchDocumentMapper> _logger;

        public JsonPatchDocumentMapper(ILogger<JsonPatchDocumentMapper> logger)
        {
            _logger = logger;
        }

        public JsonPatchDocument<UserMessage> Map(JsonPatchDocument<Message> source)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(source, nameof(source), ThrowError)
                .IsValid();

            if (!source.Operations.Any())
            {
                return new JsonPatchDocument<UserMessage>();
            }

            var operations = new List<Operation<UserMessage>>();
            foreach (var operation in source.Operations)
            {
                if (operation.path.Equals("/read", StringComparison.OrdinalIgnoreCase))
                {
                    operations.Add(new Operation<UserMessage>(
                        operation.op,
                        "/readTime",
                        operation.from,
                        operation.value.Equals(true)? DateTime.UtcNow: (DateTime?) null
                    ));
                }
                else
                {
                    operations.Add(new Operation<UserMessage>(
                        operation.op,
                        operation.path,
                        operation.from,
                        operation.value
                    ));
                }
            }
            var map = new JsonPatchDocument<UserMessage>();
            map.Operations.AddRange(operations);
            
            return map;
        }
    }
}