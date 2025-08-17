namespace ApplicationLayer.DTOs.Commissions
{
    public class CreateCommissionDto
    {
        public int RequestId { get; set; }

        public int CarrierUserId { get; set; }

        public decimal RequestPrice { get; set; }
    }
}