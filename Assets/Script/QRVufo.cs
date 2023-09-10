using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class QRVufo : MonoBehaviour
{
    public List<QRCodeFloorMapping> qrCodeFloorMappings = new List<QRCodeFloorMapping>();
    private BarcodeBehaviour mBarcodeBehaviour;

    public SetNavTargeting floorController; // Reference to the SetNavTargeting script

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
        QRCodeFloorMapping mapping = qrCodeFloorMappings.Find(x => x.QRCodeContent.ToLower() == qrCodeText.ToLower());

        if (mapping != null)
        {
            // Call the ChangeActiveFloor method with the floorNumber from the mapping
            floorController.ChangeActiveFloor(mapping.FloorNumber);
        }
    }
}

[System.Serializable]
public class QRCodeFloorMapping
{
    public string QRCodeContent;
    public int FloorNumber; // Assign different floor numbers to each QR code mapping
}