using Microsoft.AspNetCore.JsonPatch.Operations;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Messages.Areas.Messages.Mappers
{
    public class MessagePatchTypeMapper : IMapper<Operation<Message>, MessagePatchType>
    {
        public MessagePatchType Map(Operation<Message> source)
        {
            MessagePatchType patchType;
            switch (source)
            {
                case { OperationType: OperationType.Add, path: "/read" }:
                    patchType = MessagePatchType.Read;
                    break;
                case { OperationType: OperationType.Add, path: "/reply/response" }:
                    patchType = MessagePatchType.Reply;
                    break;
                case { OperationType: OperationType.Add, path: "/reply/status" }:
                    patchType = MessagePatchType.ReplyStatus;
                    break;
                default:
                    patchType = MessagePatchType.Unknown;
                    break;
            }

            return patchType;
        }
    }
}