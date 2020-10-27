namespace Core.Entities.OrderAggregate
{
  /* 
  snapshop of the order product item at the time it was placed
  because the item can change in time thats why save it seperately
  owned by order -- so no it
   */
  public class ProductItemOrdered
  {
    public ProductItemOrdered()
    {
    }

    public ProductItemOrdered(int productItemId, string productName, string pictureUrl)
    {
      ProductItemId = productItemId;
      ProductName = productName;
      PictureUrl = pictureUrl;
    }

    public int ProductItemId { get; set; }
    public string ProductName { get; set; }
    public string PictureUrl { get; set; }
  }
}