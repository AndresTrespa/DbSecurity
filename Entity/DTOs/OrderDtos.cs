namespace Entity.DTOs
{
    public class OrderDto
{
    public int Id { get; set; }
    public int ConsumerId { get; set; }
    public string? Status { get; set; }
    public string? Note { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime? DeleteAt { get; set; }
}
