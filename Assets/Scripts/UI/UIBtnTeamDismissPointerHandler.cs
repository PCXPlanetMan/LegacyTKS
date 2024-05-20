using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBtnTeamDismissPointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image ImgHighLight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ImgHighLight.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ImgHighLight.gameObject.SetActive(false);
    }
}
