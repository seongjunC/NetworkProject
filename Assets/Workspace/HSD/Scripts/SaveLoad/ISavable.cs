using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISavable
{
    public void Save(ref GameData data);
    public void Load(GameData data);
}
