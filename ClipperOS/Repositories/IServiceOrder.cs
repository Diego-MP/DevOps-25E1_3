namespace ClipperOS.Repositories;

public interface  IServiceOrder
{
     public void AddServiceOrder();
     public void RemoveServiceOrder();
     public void AlterServiceOrder();
     public void AlterStatusServiceOrder();
     public void FinishServiceOrder();
}