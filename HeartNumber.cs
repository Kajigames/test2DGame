using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    private Text heartText = null;
    private int oldHeart = 0;

    // Start is called before the first frame update
    void Start()
    {
        heartText = GetComponent<Text>();
        if (GManager.instance != null)
        {
            heartText.text = "×" + GManager.instance.heartNum;
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
        //残機が更新されたときのみ残機の表示をアップデート
        if (oldHeart != GManager.instance.heartNum)
        {
            heartText.text = "×" + GManager.instance.heartNum;
            oldHeart = GManager.instance.heartNum;
        }

    }
}
