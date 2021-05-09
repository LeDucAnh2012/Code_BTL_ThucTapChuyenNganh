using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HuongDanDialog : MonoBehaviour
{
    public bool HuongDan;
    [SerializeField]
    private GameObject PanelHD;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void _LoadHuongDan()
    {
        HuongDan = !HuongDan;
        if (HuongDan)
            PanelHD.SetActive(true);
        else
            PanelHD.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
