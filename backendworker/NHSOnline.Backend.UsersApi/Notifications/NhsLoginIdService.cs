using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public class NhsLoginIdService : INhsLoginIdService
    {
        private readonly IEnumerable<char> _firstCharactersOfHandledNhsLoginIds;

        public NhsLoginIdService(string characters)
        {
            if (string.IsNullOrEmpty(characters))
            {
                throw new ArgumentNullException(nameof(characters));
            }

            _firstCharactersOfHandledNhsLoginIds = characters.ToUpperInvariant().ToCharArray();
        }

        public bool HandlesNhsLoginId(string nhsLoginId)
        {
            if (string.IsNullOrEmpty(nhsLoginId))
            {
                throw new ArgumentNullException(nameof(nhsLoginId));
            }

            return _firstCharactersOfHandledNhsLoginIds.Contains(char.ToUpperInvariant(nhsLoginId.First()));
        }
    }
}
