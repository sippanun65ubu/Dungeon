using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class OverwriteConfirmation : MonoBehaviour
{
    public Button yesButton;
    public Button noButton;
    public TextMeshProUGUI messageText;

    // These callbacks will be assigned when showing the dialog.
    public Action onConfirm;
    public Action onCancel;

    public void Show(string message, Action confirmCallback, Action cancelCallback)
    {
        messageText.text = message;
        onConfirm = confirmCallback;
        onCancel = cancelCallback;
        gameObject.SetActive(true);
    }

    public void Confirm()
    {
        onConfirm?.Invoke();
        Close();
    }

    public void Cancel()
    {
        onCancel?.Invoke();
        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
