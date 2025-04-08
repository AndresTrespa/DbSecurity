using Entity.Model;
namespace Entity.Model;
public class Order
{
    public int Id { get; set; }
    public int ConsumerId { get; set; }
    public Consumer Consumer { get; set; }

    public string? Status { get; set; }
    public string? Note { get; set; }

    public DateTime CreateAt { get; set; }
    public DateTime? DeleteAt { get; set; }
}
