using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClicked : MonoBehaviour
{
    private Button button;
    private AudioSource clickSound;

    private void Start()
    {
        button = GetComponent<Button>();
        clickSound = GetComponent<AudioSource>();

        button.onClick.AddListener(()=>{clickSound.Play();});
    }

}
