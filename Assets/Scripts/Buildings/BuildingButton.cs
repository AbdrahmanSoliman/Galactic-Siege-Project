using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour // IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building;
    [SerializeField] private Image imageIcon;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private LayerMask floorMask;

    [SerializeField] private Camera mainCamera;
    private BoxCollider boxCollider;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingPreviewRenderer;

    private void Start() 
    {
        imageIcon.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();

        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        boxCollider = building.GetComponent<BoxCollider>();
           
    }

    private void Update() 
    {
        if(buildingPreviewInstance == null) return;

        UpdateBuildingPreview();
        PlaceBuilding();
    }

    public void Build() 
    {
        if(player.GetResources() < building.GetPrice()) return;
 
        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        buildingPreviewRenderer = buildingPreviewInstance.GetComponentInChildren<Renderer>();
 
        buildingPreviewInstance.SetActive(false);
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
 
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) { return; }
 
        buildingPreviewInstance.transform.position = hit.point;
 
        if(!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }
 
        Color color = player.CanPlaceBuilding(boxCollider, hit.point) ? Color.green : Color.red;
 
        buildingPreviewRenderer.material.SetColor("_BaseColor", color);
    }

    private void PlaceBuilding()
    {
        if(buildingPreviewInstance == null)
            return;
 
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
 
            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
            {
                player.CmdTryPlaceBuilding(building.GetId(), hit.point);
            }
            
            Destroy(buildingPreviewInstance);
        }
 
        if(Mouse.current.rightButton.wasPressedThisFrame)
        {
            Destroy(buildingPreviewInstance);
        }
    }

    #region oldScript
    // Older One -- Mouse Drag Instead of Click to Spawn a Building
    // private void Update() 
    // {
    //     if(buildingPreviewInstance == null) return;

    //     UpdateBuildingPreview();
    // }


    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     if(eventData.button != PointerEventData.InputButton.Left) return;

    //     if(player.GetResources() < building.GetPrice()) return;
        
    //     buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
    //     buildingPreviewRenderer = buildingPreviewInstance.GetComponentInChildren<Renderer>();

    //     buildingPreviewInstance.SetActive(false);
    // }
    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     if(buildingPreviewInstance == null) return;

    //     Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

    //     if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
    //     {
    //         // Place the building

    //         player.CmdTryPlaceBuilding(building.GetId(), hit.point);
    //     }

    //     Destroy(buildingPreviewInstance);
    // }

    // private void UpdateBuildingPreview()
    // {
    //     Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
    //     if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) return;

    //     buildingPreviewInstance.transform.position = hit.point; // update building preview position based on mouse position

    //     if(!buildingPreviewInstance.activeSelf)
    //     {
    //         buildingPreviewInstance.SetActive(true);
    //     }

    //     Color color = player.CanPlaceBuilding(boxCollider, hit.point) ? Color.green : Color.red;
    //     buildingPreviewRenderer.material.SetColor("_BaseColor", color);
    // }
    #endregion
}
