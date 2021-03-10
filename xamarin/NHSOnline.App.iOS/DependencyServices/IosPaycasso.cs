#if IPHONE
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls.WebViews.Payloads.Paycasso;
using NHSOnline.App.DependencyServices.Paycasso;
using NHSOnline.App.iOS.DependencyServices;
using NHSOnline.App.Logging;
using NHSOnline.App.Threading;
using Paycasso;
using Paycasso.View;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(IosPaycasso))]
namespace NHSOnline.App.iOS.DependencyServices
{
    internal sealed class IosPaycasso: IPaycasso
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger<IosPaycasso>();

        public async Task<PaycassoResult> Launch(LaunchPaycassoRequest request)
        {
            using var pcsCredentials = CreateCredentials(request);
            using var pcsFlowRequest = CreateFlowRequest(request);
            using var paycassoFlowConfiguration = ConfigureSdk();
            using var paycassoViewModel = new PaycassoViewModel();

            var completionSource = new TaskCompletionSource<PaycassoResult>();
            using var paycassoDelegate = new PaycassoDelegate(Logger, completionSource);

            PaycassoFlow.SharedFlow.StartFlowWithCredentials(
                pcsCredentials,
                pcsFlowRequest,
                paycassoFlowConfiguration,
                paycassoViewModel,
                paycassoDelegate);

            return await completionSource.Task.ResumeOnThreadPool();
        }

        private static PCSCredentials CreateCredentials(LaunchPaycassoRequest request)
        {
            return new PCSCredentials
            {
                HostUrl = new NSUrl(request.Credentials.HostUrl),
                Token = request.Credentials.Token
            };
        }

        private static PCSFlowRequest CreateFlowRequest(LaunchPaycassoRequest request)
        {
            var documentConfigurations = GetDocumentConfiguration(request);

            var transactionType = MapTransactionType(request);

            return new PCSFlowRequest
            {
                TransactionType = transactionType,
                DocumentConfigurations = documentConfigurations,
                ExternalConsumerReference = request.ExternalReferences.ConsumerReference
            };
        }

        private static PCSTransactionType MapTransactionType(LaunchPaycassoRequest request)
        {
            return request.ExternalReferences.TransactionType switch
            {
                "DocuSure" => PCSTransactionType.DocuSure,
                "InstaSure" => PCSTransactionType.InstaSure,
                "VeriSure" => PCSTransactionType.VeriSure,
                _ => throw new InvalidOperationException("Invalid transaction type")
            };
        }

        private static PCSDocumentConfiguration[] GetDocumentConfiguration(LaunchPaycassoRequest request)
        {
            var documentType = request.TransactionDetails.DocumentType;

            var documentConfiguration = documentType switch
            {
                PaycassoDocumentType.DriversLicence => new PCSDocumentConfiguration(
                    FaceLocation.FaceFrontLocation,
                    BarcodeLocation.NoBarcode,
                    MrzLocation.NoMrz,
                    DocumentShape.Id,
                    true,
                    false,
                    true),
                PaycassoDocumentType.Passport => new PCSDocumentConfiguration(
                    FaceLocation.FaceFrontLocation,
                    BarcodeLocation.NoBarcode,
                    MrzLocation.MrzFrontLocation,
                    DocumentShape.Passport,
                    false,
                    request.ExternalReferences.HasNfcJourney,
                    true),
                PaycassoDocumentType.PhotoId => new PCSDocumentConfiguration(
                    FaceLocation.FaceFrontLocation,
                    BarcodeLocation.NoBarcode,
                    MrzLocation.NoMrz,
                    DocumentShape.Id,
                    false,
                    false,
                    true),
                _ => throw new InvalidOperationException($"Unsupported document type: {documentType}")
            };

            return new[] { documentConfiguration };
        }

        private static PaycassoFlowConfiguration ConfigureSdk() =>
            new PaycassoFlowConfiguration
            {
                DisplayCancelButton = true,
                IsGeolocationRequired = false,
                DisplayDocumentPreview = true,
                ReceiveMrzData = true,
                ReceiveBarcodeData = false
            };

        private sealed class PaycassoDelegate : PaycassoFlowDelegate
        {
            private readonly ILogger _logger;
            private readonly TaskCompletionSource<PaycassoResult> _completionSource;

            public PaycassoDelegate(
                ILogger logger,
                TaskCompletionSource<PaycassoResult> completionSource)
            {
                _logger = logger;
                _completionSource = completionSource;
            }

            [Export("presentViewController:animated:completion:")]
            [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Called dynamically by iOS runtime")]
            public void PresentViewController(
                UIViewController viewControllerToPresent,
                bool animated,
                Action completionHandler)
                => ActOnRootViewController(controller => controller.PresentViewController(viewControllerToPresent, animated, completionHandler));

            [Export("dismissViewControllerAnimated:completion:")]
            [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Called dynamically by iOS runtime")]
            public void DismissViewController(bool animated, Action completionHandler)
                => ActOnRootViewController(controller => controller.DismissViewController(animated, completionHandler));
            
            private void ActOnRootViewController(
                Action<UIViewController> action,
                [CallerMemberName] string methodName = "")
            {
                if (UIApplication.SharedApplication.KeyWindow.RootViewController is { } rootViewController)
                {
                    action(rootViewController);
                }
                else
                {
                    _logger.LogError("UIApplication.SharedApplication.KeyWindow.RootViewController is null: not calling {MethodName}", methodName);
                    _completionSource.TrySetResult(new PaycassoResult.Failure("Unable to launch Paycasso (RootViewController is null)"));
                }
            }

            public override void OnFailure(PCSFlowFailureResponse response)
            {
                _completionSource.SetResult(new PaycassoResult.Failure(response.FailureMessage, (int)response.FailureCode));
            }

            public override void OnSuccess(PCSFlowResponse response)
            {
                PaycassoTransactionType transactionType = response switch
                {
                    PCSInstaSureFlowResponse _ => PaycassoTransactionType.InstaSureFlowResponse,
                    PCSVeriSureFlowResponse _ => PaycassoTransactionType.VeriSureFlowResponse,
                    PCSDocuSureFlowResponse _ => PaycassoTransactionType.DocuSureFlowResponse,
                    _ => PaycassoTransactionType.Unknown
                };

                var isFaceMatched = transactionType == PaycassoTransactionType.InstaSureFlowResponse;

                PaycassoResult result = transactionType switch
                {
                    PaycassoTransactionType.Unknown => new PaycassoResult.Failure($"DocumentResponse type {response?.GetType().Name ?? "<null>"} not recognised"),
                    _ => new PaycassoResult.Success(response.TransactionId, transactionType, isFaceMatched)
                };

                _completionSource.SetResult(result);
            }
        }
    }
}
#endif