using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public SaveDialog saveDialog;
    public LoadDialog loadDialog;
    // Start is called before the first frame update
    void Start()
    {
        saveDialog.gameObject.SetActive(false);
        loadDialog.gameObject.SetActive(false);
    }
    public void setSaveDialogActive()
    {
        saveDialog.gameObject.SetActive(true);
    }
    public void setLoadDialogActive()
    {
        loadDialog.gameObject.SetActive(true);
    }
}
