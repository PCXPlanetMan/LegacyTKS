using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToastHints : MonoBehaviour
{
    // Toast组件数目要足够多
    public List<UIToastAnimation> ListToasts;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowAToast(string strToast)
    {
        int nSelectedToastIndex = ListToasts.Count - 1;
        for (; nSelectedToastIndex >= 0; nSelectedToastIndex--)
        {
            var toast = ListToasts[nSelectedToastIndex];
            if (toast.gameObject.activeSelf == true)
            {
                break;
            }
        }
        nSelectedToastIndex += 1;

        if (nSelectedToastIndex == ListToasts.Count)
        {
            Debug.LogError("Toasts count is not enough");
            return;
        }

        for (; nSelectedToastIndex > 0; nSelectedToastIndex --)
        {
            var bottomToast = ListToasts[nSelectedToastIndex - 1];
            ListToasts[nSelectedToastIndex].UpdateToastContent(bottomToast.GetToastContent());
            ListToasts[nSelectedToastIndex].DoToast();
        }

        var toastHint = ListToasts[nSelectedToastIndex];
        toastHint.UpdateToastContent(strToast);
        toastHint.DoToast();
    }
}
