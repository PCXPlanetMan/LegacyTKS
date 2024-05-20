using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITextDlgSelectionPointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UIDialogPlayer ParentDialog;

    private Text selText;

    // Start is called before the first frame update
    void Start()
    {
        selText = this.gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selText.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selText.color = Color.white;
    }

    void OnDisable()
    {
        selText.color = Color.white;
    }
}
