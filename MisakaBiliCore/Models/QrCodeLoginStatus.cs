namespace MisakaBiliCore.Models;

public enum QrCodeLoginStatus
{
    WaitingForScan = 86101,
    WaitingForConfirm = 86090,
    QrCodeExpired = 86038,
    Success = 0
}
