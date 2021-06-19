using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageNum : MonoBehaviour
{
    private Text stageText = null;
    private int oldStageNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        stageText = GetComponent<Text>();
        if (GManager.instance != null)
        {
            stageText.text = "Stage：" + GManager.instance.stageNum;
        }
        else
        {
            Debug.Log("GameManagerがありません");
            Destroy(this);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //ステージ番号が更新されたときのみステージ番号の表示をアップデート
        if (oldStageNum != GManager.instance.stageNum)
        {
            stageText.text = "Stage：" + GManager.instance.stageNum;
            oldStageNum = GManager.instance.stageNum;
        }

    }
}
