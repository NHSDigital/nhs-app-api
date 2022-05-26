namespace NHSOnline.AuditLogFunctionApp;

public static class AuditingOperations
{
    public static readonly string[] ExcludedList = { "Login_Success", "NotificationToggle_Response", "InitialNotificationPrompt_Decision" };
}