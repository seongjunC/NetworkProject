using Game;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] Sprite sprite;
    [SerializeField] Texture2D texture;
    [SerializeField] MapType map;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            Sprite sprite = Manager.Resources.Load<Sprite>($"MapIcon/{map.ToString()}");
            Texture2D t = Manager.Resources.Load<Texture2D>($"MapIcon/{map.ToString()}");
            if (sprite == null)
            {
                Debug.Log("Null");
            }
            if (t == null)
            {
                Debug.Log("Texture is null");
            }

            this.sprite = sprite;
            this.texture = t;
        }
    }
}
