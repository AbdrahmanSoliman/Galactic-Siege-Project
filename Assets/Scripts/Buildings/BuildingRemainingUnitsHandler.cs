using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingRemainingUnitsHandler : MonoBehaviour
{
    // Its only purpose to don't show "0" it is better in design :)
    [SerializeField] private TMP_Text remainingUnitsText;

    void Update()
    {
        if(remainingUnitsText.text == "0")
        {
            remainingUnitsText.text = "";
        }
    }
}
