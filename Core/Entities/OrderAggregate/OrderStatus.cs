using System.Runtime.Serialization;

namespace Core.Entities.OrderAggregate
{
  /* 
  to track our order status
   */
  public enum OrderStatus
  {
    [EnumMember(Value = "Pending")]
    Pending,

    [EnumMember(Value = "PaymentReceived")]
    PaymentReceived,

    [EnumMember(Value = "PaymentFailed")]
    PaymentFailed
  }
}