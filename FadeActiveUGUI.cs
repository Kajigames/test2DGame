using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeActiveUGUI : MonoBehaviour
{
    [Header("フェード速度")] public float speed = 1.0f;
    [Header("上昇量")] public float moveDis = 10.0f;
    [Header("上昇時間")] public float moveTime = 1.0f;
    [Header("キャンバスグループ")] public CanvasGroup cg;
    [Header("プレイヤー判定")] public PlayerTriggerCheck trigger;

    private Vector3 defaultPos;
    private float timer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        //初期化
        if (cg == null && trigger == null)
        {
            Debug.Log("インスペクタの設定が足りていません");
            Destroy(this);
        }
        else
        {
            cg.alpha = 0.0f;//アルファ値を0
            defaultPos = cg.transform.position;
            //キャンバスグループのついたオブジェクトの位置をデフォルトより設定した値だけ下にする(初期位置のちょい下からフェードインしながら戻る みたいな感じ)
            cg.transform.position = defaultPos - Vector3.up * moveDis;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーが侵入したら上昇しながらフェードインする
        if (trigger.isOn)
        {
            //キャンバスグループのY軸位置が初期位置より下、もしくはキャンバスグループのアルファ値が1.0未満なら
            if (cg.transform.position.y < defaultPos.y || cg.alpha < 1.0f)
            {
                cg.alpha = timer / moveTime;//アルファ値を設定した時間で1.0fになるようにする
                cg.transform.position += Vector3.up * (moveDis / moveTime) * speed * Time.deltaTime;//キャンバスグループを設定した時間、速度で上昇させる
                timer += speed * Time.deltaTime;
            }
            //フェードイン完了
            else
            {
                cg.alpha = 1.0f;
                cg.transform.position = defaultPos;
            }
        }
        //プレイヤーが範囲外へ出たら下降しながらフェードアウトする
        else
        {
            if (cg.transform.position.y > defaultPos.y - moveDis || cg.alpha > 0.0f)
            {
                cg.alpha = timer / moveTime;//アルファ値を設定した時間で0.0fになるようにする
                cg.transform.position -= Vector3.up * (moveDis / moveTime) * speed * Time.deltaTime;//キャンバスグループを設定した時間、速度で下降させる
                timer += speed * Time.deltaTime;
            }
            else
            {
                timer = 0.0f;
                cg.alpha = 0.0f;
                cg.transform.position = defaultPos - Vector3.up * moveDis;
            }
        }
    }
}
