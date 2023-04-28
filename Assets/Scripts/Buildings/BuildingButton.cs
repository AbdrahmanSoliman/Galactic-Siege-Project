using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building;
    [SerializeField] private Image imageIcon;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private LayerMask floorMask;

    private Camera mainCamera;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingPreviewRenderer;

    private void Start() 
    {
        mainCamera = Camera.main;

        imageIcon.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();
           
    }

    private void Update() 
    {
        if(player == null) // temporarily
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }
        
        if(buildingPreviewInstance == null) return;

        UpdateBuildingPreview();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) return;
        
        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        buildingPreviewRenderer = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        buildingPreviewInstance.SetActive(false);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(buildingPreviewInstance == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            // spawn the building
        }

        Destroy(buildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) return;

        buildingPreviewInstance.transform.position = hit.point;

        if(!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }
    }
}
