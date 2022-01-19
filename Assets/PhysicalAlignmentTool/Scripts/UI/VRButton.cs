using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRButton : MonoBehaviour
{
    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
    }

    public void Press()
    {
        _button.onClick.Invoke();
    }

    public void Select()
    {
        _button.Select();
    }
}
