using System;
using Unity.VisualScripting;

[Serializable]
public class Coupon
{
    public string CouponKey;
    public bool IsUse = false;
    public int Amount;

    public Coupon(string _couponKey, int _amount)
    {
        CouponKey = _couponKey;
        Amount = _amount;
    }
}
