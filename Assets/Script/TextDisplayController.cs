using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Vuforia;
using System.Collections.Generic;

public class ARObjectTextController : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Camera arCamera;
    public TextMeshProUGUI textPrefab;
    public GameObject[] targetObjects; // Array of objects to display text above.
    public float maxDisplayDistance = 5.0f; // Maximum distance for text display.

    private TextMeshProUGUI currentText;
    private GameObject currentTarget;
    private bool isDisplaying = false;

    private Dictionary<GameObject, string> objectToMessageMap = new Dictionary<GameObject, string>();

    private void Start()
    {
        // Initialize the object-to-message mapping here.
        if (targetObjects.Length > 0)
        {
            objectToMessageMap[targetObjects[0]] = "Message for Object 1";
            objectToMessageMap[targetObjects[1]] = "Message for Object 2";
            objectToMessageMap[targetObjects[2]] = "Message for Object 3";
            // Add more objects and messages as needed.
        }
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            Vector3 targetPosition = currentTarget.transform.position;
            float distance = Vector3.Distance(arCamera.transform.position, targetPosition);

            if (distance <= maxDisplayDistance)
            {
                // Object is within the specified display distance.
                if (!isDisplaying)
                {
                    DisplayText(targetPosition, GetMessageForTarget(currentTarget));
                }
            }
            else
            {
                // Object is outside the display distance.
                if (isDisplaying)
                {
                    HideText();
                }
            }
        }
    }

    public void OnDropdownValueChanged()
    {
        // Handle dropdown value change here (e.g., set the current target).
        int selectedIndex = dropdown.value;

        if (selectedIndex >= 0 && selectedIndex < targetObjects.Length)
        {
            SetCurrentTarget(targetObjects[selectedIndex]);
        }
    }

    private void DisplayText(Vector3 targetPosition, string message)
    {
        if (textPrefab != null)
        {
            currentText = Instantiate(textPrefab);
            currentText.transform.SetParent(GameObject.Find("Canvas").transform); // Make it a child of the Canvas GameObject.
            currentText.transform.position = arCamera.WorldToScreenPoint(targetPosition);
            currentText.text = message; // Set the text content from the dictionary.

            // Ensure the text is facing the camera.
            currentText.transform.LookAt(arCamera.transform);
            currentText.transform.Rotate(Vector3.up, 180f);

            isDisplaying = true;
        }
    }

    private void HideText()
    {
        if (currentText != null)
        {
            Destroy(currentText.gameObject);
            currentText = null;
            isDisplaying = false;
        }
    }

    private void SetCurrentTarget(GameObject target)
    {
        HideText(); // Hide any previously displayed text.
        currentTarget = target;
    }

    private string GetMessageForTarget(GameObject target)
    {
        // Retrieve the message from the dictionary based on the target object.
        if (objectToMessageMap.ContainsKey(target))
        {
            return objectToMessageMap[target];
        }
        else
        {
            return "Default Message"; // Provide a default message if the object is not in the dictionary.
        }
    }
}


