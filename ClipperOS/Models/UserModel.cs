namespace ClipperOS.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public DateTime Created { get; set; }
    
}

public class HomeIndexViewModel
{
    public List<UserModel> Users { get; set; } = new List<UserModel>();
    public int TotalUsers { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;
    public string? ErrorMessage { get; set; }
    public bool HasError { get; set; } = false;
}