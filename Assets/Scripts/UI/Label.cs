using UnityEngine;
using TMPro;
using System;

public class Label : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TextMeshPro>().text = Char.ToUpper(GameManager.Instance.GameConfig.GoatName[0]).ToString();
        GetComponent<Renderer>().sortingOrder = 14;
    }
}
