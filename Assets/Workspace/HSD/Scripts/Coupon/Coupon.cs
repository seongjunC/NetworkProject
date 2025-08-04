using System;
using Unity.VisualScripting;

[Serializable]
public class Coupon
{
    public string CouponKey;
    public bool IsUse = false;    

    public Coupon(string _couponKey)
    {
        CouponKey = _couponKey;
        IsUse = false;
    }
}
