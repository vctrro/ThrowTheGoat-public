using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] private string locale;
    private MenuController menuController;
    private Image buttonSelected;
    
    private void Start()
    {
        menuController = GameObject.Find("MenuController").GetComponent<MenuController>();
        menuController.OnLanguageSelected.AddListener(LanguageSelected);
        GetComponent<Button>().onClick.AddListener(ButtonClicked);
        buttonSelected = GetComponent<Image>();

        if (GameManager.Instance.GameConfig.GameLocale == locale) buttonSelected.enabled = true;
    }

    private void LanguageSelected(string localeCode)
    {
        if (localeCode == locale)
        {
            buttonSelected.enabled = true;
        }
        else
        {
            buttonSelected.enabled = false;
        }
    }

    private void ButtonClicked()
    {
        menuController.OnLanguageSelected.Invoke(locale);
    }
}
