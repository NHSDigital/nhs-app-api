using System.Collections.Generic;
using NHSOnline.Backend.Worker.Mocking.Models;

namespace NHSOnline.Backend.Worker.Stubs.Journeys
{
    internal abstract class Journey
    {
        internal abstract IEnumerable<Mapping> GetMappings();
    }
}
