using System.Collections.Generic;
 using System.Threading.Tasks;
 using NHSOnline.Backend.GpSystems.Session;
 using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
 using NHSOnline.Backend.Support;

 namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Session
 {
     [FakeGpAreaBehaviour(Behaviour.BadGateway)]
     public class BadGatewaySessionAreaBehaviour : ISessionAreaBehaviour
     {
         public Task<GpSessionCreateResult> Create(
             string connectionToken,
             string odsCode,
             string nhsNumber,
             FakeUser user
         )
         {
             return Task.FromResult<GpSessionCreateResult>(
                 new GpSessionCreateResult.BadGateway(nameof(BadGatewaySessionAreaBehaviour))
             );
         }

         public Task<SessionLogoffResult> Logoff(GpUserSession gpUserSession)
         {
             return Task.FromResult<SessionLogoffResult>(new SessionLogoffResult.BadGateway());
         }

         public Task<GpSessionRecreateResult> Recreate(
             string connectionToken,
             string odsCode,
             string nhsNumber,
             string patientId)
         {
             return Task.FromResult<GpSessionRecreateResult>(
                 new GpSessionRecreateResult.Failure()
             );
         }
     }
 }