using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform unitSelectionArea;
    [SerializeField] private LayerMask layerMask;

    Vector2 startPosition;
    private RTSPlayer player;
    private Camera mainCamera;
    public List<Unit> SelectedUnits {get;} = new List<Unit>();

    private void Start() 
    {
        mainCamera = Camera.main;
        
    }

    private void Update() 
    {
        if(player == null) // temporarily
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }


        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if(Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void StartSelectionArea()
    {
        // start box selection
        foreach(Unit selectedUnit in SelectedUnits)
        {
            selectedUnit.Deselect();
        }

        SelectedUnits.Clear();

        unitSelectionArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();

    }
    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth/2, areaHeight/2);
    }

    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        if(unitSelectionArea.sizeDelta.magnitude == 0) // player selected an individual unit instead of creating a drag box
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;

            if (!unit.isOwned) return;

            SelectedUnits.Add(unit);

            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Select();
            }

            return;
        }

        // player created a drag box and we're trying to figure out if there're units in this box that belongs to that player to multiselect

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach(Unit unit in player.GetPlayerUnits())
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if(min.x < screenPosition.x && max.x > screenPosition.x && min.y < screenPosition.y && max.y > screenPosition.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }
}
