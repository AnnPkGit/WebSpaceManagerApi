namespace WebSpaceManager.Entities
{
    public class Contract
    {
        public Guid Id { get; set; }

        public Guid SpaceId { get; set; }

        public Guid EquipmentId { get; set; }

        public int Amount { get; set; }
    }
}
