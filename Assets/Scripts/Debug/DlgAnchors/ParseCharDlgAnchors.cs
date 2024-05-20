using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.tksr.schema;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class ParseCharDlgAnchors : MonoBehaviour
{
    private List<EditorCharDlgAnchor> allCharsDlg;

    // Start is called before the first frame update
    void Start()
    {
        var allDlgs = this.gameObject.transform.GetComponentsInChildren<EditorCharDlgAnchor>(true);
        allCharsDlg = allDlgs.ToList();

        for (int i = 0; i < allCharsDlg.Count; i++)
        {
            allCharsDlg[i].Dlg.gameObject.SetActive(false);
            allCharsDlg[i].gameObject.SetActive(false);
        }
        PreviewAllCharsAnimation();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PreviewAllCharsAnimation()
    {
        StartCoroutine(FuncLoopPlayCharAnimations());
    }

    private static string[] PreviewAnims =
    {
        "Run NE",
        "Run SE",
        "Run SW",
        "Run NW"
    };

    private IEnumerator FuncLoopPlayCharAnimations()
    {
        while (allCharsDlg.Count > 0)
        {
            var charDlg = allCharsDlg[0];
            charDlg.gameObject.SetActive(true);
            charDlg.Dlg.gameObject.SetActive(true);
            var charAnim = charDlg.gameObject.GetComponent<Animator>();

            AnimationClip[] clips = charAnim.runtimeAnimatorController.animationClips;
            for (int i = 0; i < PreviewAnims.Length; i++)
            {
                string animName = PreviewAnims[i];
                AnimationClip clip = clips.FirstOrDefault(x => x.name.CompareTo(animName) == 0);
                if (clip != null)
                {
                    charAnim.Play(animName);
                    yield return new WaitForSeconds(clip.length);
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }

            charDlg.gameObject.SetActive(false);
            charDlg.Dlg.gameObject.SetActive(false);

            allCharsDlg.RemoveAt(0);
            allCharsDlg.Add(charDlg);
        }
    }

    public void OnClickSaveDlgParam()
    {
        SaveDlgAnchorsToJson();
    }

    private void SaveDlgAnchorsToJson()
    {
        ConfigDlgAnchors configOfDlgAnchors = null;
        if (configOfDlgAnchors == null)
        {
            configOfDlgAnchors = new ConfigDlgAnchors();
            configOfDlgAnchors.allDlgAnchors = new List<DlgAnchor>();
        }

        for (int i = 0; i < allCharsDlg.Count; i++)
        {
            var charDlg = allCharsDlg[i];
            if (charDlg.Dlg != null)
            {
                Vector3 charDlgUIPos = charDlg.Dlg.position;
                Vector3 charDlgLocalPos = Camera.main.ScreenToWorldPoint(charDlgUIPos);
                charDlgLocalPos = charDlg.transform.InverseTransformPoint(charDlgLocalPos);

                configOfDlgAnchors.allDlgAnchors.Add(new DlgAnchor()
                {
                    charTemplateId = charDlg.CharTemplateID,
                    dlgLocalX = charDlgLocalPos.x,
                    dlgLocalY = charDlgLocalPos.y
                });
            }
        }

        var dataPath = Path.Combine(Application.dataPath + "/Resources", ResourceUtils.CONFIG_CHAR_DLG_ANCHORS);
        if (File.Exists(dataPath))
        {
            File.Delete(dataPath);
        }

        string dataAsJson = JsonConvert.SerializeObject(configOfDlgAnchors, Formatting.Indented);
        File.WriteAllText(dataPath, dataAsJson);
    }
}
