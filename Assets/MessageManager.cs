using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    private void OnEnable()
    {
        Invoke("DisableObject", 1.5f);
    }

    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void SetMessage(string message)
    {
        messageText.text = ($"{message}");
    }
}
