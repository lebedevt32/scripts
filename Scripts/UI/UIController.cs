using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private SpellWindow spellWindow;
    [SerializeField] private CustomPanel customPanel;
    [SerializeField] private GameObject customButtton;
    [SerializeField] private GameObject closeCustomButton;
    [SerializeField] private HitBar hitbar;

    private GameObject _tempSenderButton;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("Fire1", "110");
        PlayerPrefs.SetString("Fire3", "000");
        PlayerPrefs.SetString("Jump", "141");
        customPanel.Close();
        spellWindow.Close();
        closeCustomButton.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OpenCustom()
    {
        customPanel.Open();
        customButtton.SetActive(false);
        closeCustomButton.SetActive(true);
    }
    public void CloseCustom()
    {
        customPanel.Close();
        closeCustomButton.SetActive(false);
        customButtton.SetActive(true);
    }

    public void SpellCraftClick(GameObject senderButton)
    {
        if (spellWindow.transform.gameObject.activeInHierarchy && _tempSenderButton == senderButton)
        {
            spellWindow.Close();
        }
        else
        {
            Vector3 position = senderButton.transform.position;
            position.x += 130;
            spellWindow.Open(position);

            spellWindow.SetSpellcode(senderButton.tag); //в теге кнопки ключ к коду соответствующего спелла
        }
        _tempSenderButton = senderButton;
    }

    public void UpdateHitBar(float health)
    {
        hitbar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)(health * 51));
    }

}
