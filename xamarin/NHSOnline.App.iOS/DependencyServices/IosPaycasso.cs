#if IPHONE
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Foundation;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Logging;
using Paycasso;
using Paycasso.View;
using UIKit;

namespace NHSOnline.App.iOS.DependencyServices
{
    internal sealed class IosPaycasso: IPaycasso
    {
        private static ILogger Logger => NhsAppLogging.CreateLogger<IosPaycasso>();

        public async Task<PaycassoCallbackResponse> Launch(PaycassoData data)
        {
            using var pcsCredentials = new PCSCredentials
            {
                HostUrl = new NSUrl(data.Credentials.HostUrl),
                Token = data.Credentials.Token
            };

            var documentConfigurations = GetDocumentConfiguration(
                data.TransactionDetails.DocumentType,
                data.ExternalReferences.HasNfcJourney);

            var transactionType = data.ExternalReferences.TransactionType switch
            {
                "DocuSure" => PCSTransactionType.DocuSure,
                "InstaSure" => PCSTransactionType.InstaSure,
                "VeriSure" => PCSTransactionType.VeriSure,
                _ => throw new InvalidOperationException("Invalid transaction type")
            };

            using var pcsFlowRequest = new PCSFlowRequest
            {
                TransactionType = transactionType,
                DocumentConfigurations = documentConfigurations,
                ExternalConsumerReference = data.ExternalReferences.ConsumerReference
            };

            var completionSource = new TaskCompletionSource<PaycassoCallbackResponse>();

            using var paycassoDelegate = new PaycassoDelegate(Logger, completionSource);
            using var paycassoFlowConfiguration = ConfigureSdk();
            using var paycassoViewModel = new PaycassoViewModel();

            PaycassoFlow.SharedFlow.StartFlowWithCredentials(
                pcsCredentials,
                pcsFlowRequest,
                paycassoFlowConfiguration,
                paycassoViewModel,
                paycassoDelegate);

            return await completionSource.Task.ConfigureAwait(true);
        }

        private static PCSDocumentConfiguration[] GetDocumentConfiguration(PaycassoDocumentType documentType, bool eChipRequested)
        {
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
                    eChipRequested,
                    true),
                PaycassoDocumentType.PhotoId => new PCSDocumentConfiguration(
                    FaceLocation.FaceFrontLocation,
                    BarcodeLocation.NoBarcode,
                    MrzLocation.NoMrz,
                    DocumentShape.Id,
                    false,
                    false,
                    true),
                _ => throw new InvalidOperationException()
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
            private readonly TaskCompletionSource<PaycassoCallbackResponse> _completionSource;

            public PaycassoDelegate(
                ILogger logger,
                TaskCompletionSource<PaycassoCallbackResponse> completionSource)
            {
                _logger = logger;
                _completionSource = completionSource;
            }

            [Export("presentViewController:animated:completion:")]
            [SuppressMessage("ReSharper", "UnusedMember.Local")]
            public void PresentViewController(
                UIViewController viewControllerToPresent,
                bool animated,
                Action completionHandler)
                => ActOnRootViewController(controller => controller.PresentViewController(viewControllerToPresent, animated, completionHandler));

            [Export("dismissViewControllerAnimated:completion:")]
            [SuppressMessage("ReSharper", "UnusedMember.Local")]
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
                    _completionSource.TrySetResult(PaycassoCallbackResponse.ForError("Unable to launch Paycasso (RootViewController is null)"));
                }
            }

            public override void OnFailure(PCSFlowFailureResponse response)
            {
                _completionSource.SetResult(PaycassoCallbackResponse.ForError(response.FailureMessage, (int)response.FailureCode));
            }

            public override void OnSuccess(PCSFlowResponse response)
            {
                PaycassoTransactionType transactionType = PaycassoTransactionType.Unknown;
                var isFaceMatched = false;
                PaycassoError? paycassoError = null;

                switch (response)
                {
                    case PCSInstaSureFlowResponse _:
                        transactionType = PaycassoTransactionType.InstaSureFlowResponse;
                        isFaceMatched = true;
                        break;
                    case PCSVeriSureFlowResponse _:
                        transactionType = PaycassoTransactionType.VeriSureFlowResponse;
                        break;
                    case PCSDocuSureFlowResponse _:
                        transactionType = PaycassoTransactionType.DocuSureFlowResponse;
                        break;
                    default:
                        paycassoError = new PaycassoError("DocumentResponse type not recognised");
                        break;
                }

                var paycassoCallbackResponse = PaycassoCallbackResponse.ForSuccess(
                    response.TransactionId,
                    transactionType,
                    isFaceMatched,
                    paycassoError);

                _completionSource.SetResult(paycassoCallbackResponse);
            }
        }
    }
}
#endif