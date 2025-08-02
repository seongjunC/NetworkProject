using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GachaResult
{
    public string Time;
    public string Name;

    public string GetFormattedTime()
    {
        // �����ϰ� DateTime���� ��ȯ
        if (DateTime.TryParseExact(Time, "yyyyMMddHHmmssfff",
                                   null,
                                   System.Globalization.DateTimeStyles.None,
                                   out DateTime parsedTime))
        {
            // 25/08/03/17:05
            return parsedTime.ToString("yy/MM/dd/HH:mm");
        }
        else
        {
            return Time;
        }
    }
}
