using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GachaResult
{
    public long Time;
    public string Name;

    public string GetFormattedTime()
    {
        // 안전하게 DateTime으로 변환
        if (DateTime.TryParseExact(Time.ToString(), "yyyyMMddHHmmssfff",
                                   null,
                                   System.Globalization.DateTimeStyles.None,
                                   out DateTime parsedTime))
        {
            // 25/08/03/17:05
            return parsedTime.ToString("yy/MM/dd/HH:mm");
        }
        else
        {
            return Time.ToString();
        }
    }
}
