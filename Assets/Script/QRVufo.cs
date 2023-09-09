using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class QRVufo : MonoBehaviour
{
    public List<QRCodePanelMapping> qrCodeMappings = new List<QRCodePanelMapping>();
    private BarcodeBehaviour mBarcodeBehaviour;

    void Start()
    {
        mBarcodeBehaviour = GetComponent<BarcodeBehaviour>();
    }

    void Update()
    {
        if (mBarcodeBehaviour != null && mBarcodeBehaviour.InstanceData != null)
        {
            string qrCodeText = mBarcodeBehaviour.InstanceData.Text;
            HandleQRCode(qrCodeText);
        }
    }

    private void HandleQRCode(string qrCodeText)
    {
        QRCodePanelMapping mapping = qrCodeMappings.Find(x => x.QRCodeContent.ToLower() == qrCodeText.ToLower());

        if (mapping != null)
        {
            // Activate the corresponding TextMeshPro panel and deactivate others
            foreach (QRCodePanelMapping qrCodeMapping in qrCodeMappings)
            {
                qrCodeMapping.PanelObject.SetActive(qrCodeMapping == mapping);
            }
        }
    }
}

[System.Serializable]
public class QRCodePanelMapping
{
    public string QRCodeContent;
    public GameObject PanelObject;
}
