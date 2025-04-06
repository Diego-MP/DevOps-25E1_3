namespace ClipperOS.Models;

public class ProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string Category { get; set; }
    public float Price { get; set; }
    public string CodeBar { get; set; }
    public int Stock { get; set; }
    public DateTime Created { get; set; }
}