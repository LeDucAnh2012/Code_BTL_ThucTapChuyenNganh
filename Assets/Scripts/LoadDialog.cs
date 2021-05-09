using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadDialog : MonoBehaviour
{
    public Dropdown patternName;
    // Start is called before the first frame update
    void Start()
    {
        ReloadOptions();
    }

    private void onEnable()
    {
        ReloadOptions();
    }

    void ReloadOptions()
    {
        List<string> options = new List<string>();
        string[] filePaths = Directory.GetFiles(@"Patterns/");
        Debug.Log(filePaths.Length);
        foreach (string name in filePaths)
        {
            string fileName = name.Substring(name.LastIndexOf("/") + 1);
            string extension = System.IO.Path.GetExtension(fileName);

            fileName = fileName.Substring(0, fileName.Length - extension.Length);
            options.Add(fileName);
        }
        patternName.ClearOptions();
        patternName.AddOptions(options);
    }

    public void disableLoadDialog()
    {
        gameObject.SetActive(false);
    }
    public void loadPattern()
    {
        EventManager.TriggerEvent("LoadPattern");        
        gameObject.SetActive(false);
    }
}
