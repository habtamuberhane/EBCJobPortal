namespace EBCJobPortalAdmin.IModels
{
    public interface INotificationService
    {
        void saveNotification(Guid createBy, List<Guid> createdTo, string message);
    }
}
