namespace ClipperOS.Models;


public class ServiceOrderModel
{
    public int OsId { get; set; }
    public UserModel User { get; set; }
    public List<ProductModel> Products { get; set; }
    public string Description { get; set; }
    public float TotalPrice { get; set; }
    public string Status { get; set; }
}