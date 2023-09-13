using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class SetNavTargeting : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown _navTargetDropDown;
    [SerializeField]
    private List<Target> _navTargetObjects = new List<Target>();
    [SerializeField]
    private Slider _navYOffset;
    [SerializeField]
    private GameObject canvasPrefab; // Reference to the Canvas prefab
    [SerializeField]
    private GameObject textPrefab; // Reference to the TextMeshProUGUI prefab
    [SerializeField]
    private Transform cameraTransform; // Reference to the camera's transform

    // Add public string fields for each message
    public string message1 = "Scanner";
    public string message2 = "Computer";
    public string message3 = "TV";
    public string message4 = "Elevator";
    public string message5 = "Pipes";
    public string message6 = "Toilet";
    public string message7 = "Janitor";

    private NavMeshPath _path; // Current Calculated Path
    private LineRenderer _lineRenderer; // LineRenderer To Display Path
    private Vector3 _targetPosition = Vector3.zero; // Current Target Position

    public GameObject arCamera;

    public int currentFloor = 1;

    private bool _lineToggle = false;
    private GameObject _currentCanvas; // Stores the currently displayed Canvas
    private List<GameObject> _currentTextObjects = new List<GameObject>(); // Stores the currently displayed TextMeshProUGUI objects

    private void Start() // Create New Path
    {
        _path = new NavMeshPath();
        _lineRenderer = transform.GetComponent<LineRenderer>();
        _lineRenderer.enabled = _lineToggle;
    }

    private void Update() // Calculate Line Position
    {
        if (_lineToggle && _targetPosition != Vector3.zero)
        {
            NavMesh.CalculatePath(arCamera.transform.position, _targetPosition, NavMesh.AllAreas, _path);
            _lineRenderer.positionCount = _path.corners.Length;
            Vector3[] calculatedPathAndOffset = AddLineOffset();
            _lineRenderer.SetPositions(calculatedPathAndOffset);
        }
    }

    public void SetCurrentNavTarget(int selectedValue)
    {
        _targetPosition = Vector3.zero; // Resets Target Position

        // Destroy the previously displayed Canvas and TextMeshProUGUI objects (if any)
        DestroyCanvas();

        string selectedText = _navTargetDropDown.options[selectedValue].text;
        Target currentTarget = _navTargetObjects.Find(x => x.Name.Equals(selectedText));

        if (currentTarget != null)
        {
            if (!_lineRenderer.enabled)
            {
                ToggleVisibility();
            }

            // Check If Floor Is Changing

            _targetPosition = currentTarget.PositionObject.transform.position;

            // Display text above the selected game object
            DisplayTextAboveObject(GetMessageForTarget(currentTarget.Name), currentTarget.PositionObject.transform);
        }
    }

    private string GetMessageForTarget(string targetName)
    {
        // Assign the appropriate message based on the target name
        switch (targetName)
        {
            case "Scanner": return message1;
            case "Computer": return message2;
            case "TV": return message3;
            case "Elevator": return message4;
            case "Pipes": return message5;
            case "Toilet": return message6;
            case "Janitor": return message7;
            default: return "";
        }
    }

    private void DisplayTextAboveObject(string text, Transform targetTransform)
    {
        // Instantiate a Canvas
        _currentCanvas = Instantiate(canvasPrefab, targetTransform.position, Quaternion.identity);

        // Instantiate a TextMeshProUGUI object inside the Canvas
        GameObject textObject = Instantiate(textPrefab, _currentCanvas.transform);
        _currentTextObjects.Add(textObject);

        // Set the text and position of the TextMeshProUGUI object
        TextMeshProUGUI textComponent = textObject.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = text;
            textComponent.transform.LookAt(cameraTransform); // Rotate to face the camera
            textComponent.transform.position = targetTransform.position + Vector3.up * 0f; // Adjust the height as needed
        }
    }

    private void DestroyCanvas()
    {
        if (_currentCanvas != null)
        {
            Destroy(_currentCanvas);
            _currentCanvas = null;
        }

        foreach (var textObject in _currentTextObjects)
        {
            Destroy(textObject);
        }
        _currentTextObjects.Clear();
    }

    public void ToggleVisibility()
    {
        _lineToggle = !_lineToggle;
        _lineRenderer.enabled = _lineToggle;
        Debug.Log("Toggle Line Vis");

        // Destroy the displayed Canvas and TextMeshProUGUI objects when toggling visibility
        DestroyCanvas();
    }

    public void ChangeActiveFloor(int floorNumber)
    {
        currentFloor = floorNumber;
        SetNavTargetDropDownOptions(currentFloor);
    }

    public Vector3[] AddLineOffset()
    {
        if (_navYOffset.value == 0)
        {
            return _path.corners;
        }

        Vector3[] calculatedLine = new Vector3[_path.corners.Length];
        for (int i = 0; i < _path.corners.Length; i++)
        {
            calculatedLine[i] = _path.corners[i] + new Vector3(0, _navYOffset.value, 0);
        }
        return calculatedLine;
    }

    private void SetNavTargetDropDownOptions(int floorNumber)
    {
        _navTargetDropDown.ClearOptions();
        _navTargetDropDown.value = 0;

        if (_lineRenderer.enabled)
        {
            ToggleVisibility();
        }

        if (floorNumber == 1)
        {
            _navTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Scanner"));
            _navTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Computer"));
            _navTargetDropDown.options.Add(new TMP_Dropdown.OptionData("TV"));
            _navTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Elevator"));
        }
        if (floorNumber == 2)
        {
            _navTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Pipes"));
            _navTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Toilet"));
            _navTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Janitor"));
        }
    }
}
