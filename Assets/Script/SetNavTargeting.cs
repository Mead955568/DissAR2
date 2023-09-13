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
    private Transform cameraTransform; // Reference to the camera's transform

    private NavMeshPath _path; // Current Calculated Path
    private LineRenderer _lineRenderer; // LineRenderer To Display Path
    private Vector3 _targetPosition = Vector3.zero; // Current Target Position

    public GameObject arCamera;

    public int currentFloor = 1;

    private bool _lineToggle = false;
    private List<GameObject> _canvasObjects = new List<GameObject>(); // List of Canvas GameObjects associated with targets

    private void Start() // Create New Path
    {
        _path = new NavMeshPath();
        _lineRenderer = transform.GetComponent<LineRenderer>();
        _lineRenderer.enabled = _lineToggle;

        // Initialize the list of Canvas GameObjects using the target names
        foreach (var target in _navTargetObjects)
        {
            GameObject canvasObject = GameObject.Find(target.Name);
            _canvasObjects.Add(canvasObject);
            canvasObject.SetActive(false);
        }
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

            // Disable all Canvas GameObjects
            DisableAllCanvases();

            // Enable the Canvas linked to the selected target
            int index = _navTargetObjects.IndexOf(currentTarget);
            if (index != -1)
            {
                _canvasObjects[index].SetActive(true);
            }
        }
    }

    private void DisableAllCanvases()
    {
        foreach (var canvasObject in _canvasObjects)
        {
            canvasObject.SetActive(false);
        }
    }

    public void ToggleVisibility()
    {
        _lineToggle = !_lineToggle;
        _lineRenderer.enabled = _lineToggle;
        Debug.Log("Toggle Line Vis");
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
