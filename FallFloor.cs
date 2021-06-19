using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFloor : MonoBehaviour
{
    //インスペクタで編集する用の変数
    [Header("スプライトが付いているオブジェクト")] public GameObject spriteObj;
    [Header("振動幅")] public float vibWidth = 0.1f;
    [Header("振動速度")] public float vibSpeed =  30.0f;
    [Header("落下待機時間")] public float fallTime = 1.0f;
    [Header("落下速度")] public float fallSpeed = 10.0f;
    [Header("落下後戻ってくるまでの時間")] public float returnTime = 5.0f;
    [Header("振動アニメーションカーブ")] public AnimationCurve curve;

    //private変数
    private bool isOn;//playerが乗ったか判定
    private bool isFall;//落下判定
    private bool isRetrun;//戻り判定
    private Vector3 spriteDefaultPos;//spriteの付いているオブジェクトの初期位置
    private Vector3 floorDefaultPos;//当たり判定を持った床のオブジェクトの初期位置
    private Vector2 fallVelocity;//落下速度
    private BoxCollider2D col;//床のBoxCollider2D
    private Rigidbody2D rb;//床のRigidbody2D
    private ObjectCollision oc;//床とplayerの衝突判定
    private SpriteRenderer sr;//床のSpriteRenderer
    private float timer = 0.0f;//経過時間測定用
    private float fallTimer = 0.0f;//落下時間
    private float returnTimer = 0.0f;//戻り時間
    private float blinkTimer = 0.0f;//点滅時間

    // Start is called before the first frame update
    void Start()
    {
        //使用する変数などを初期化
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        oc = GetComponent<ObjectCollision>();

        //設定が足りているか確認
        if (spriteObj != null && oc != null && col != null && rb != null)
        {
            //spriteが付いているゲームオブジェクト(床)の位置を取得
            spriteDefaultPos = spriteObj.transform.position;
            //落下速度をインスペクタで設定した速度にする
            fallVelocity = new Vector2(0 , -fallSpeed);
            //当たり判定が付いた床の初期位置を取得
            floorDefaultPos = gameObject.transform.position;
            //spriteが付いた床のSpriteRendererを取得
            sr = spriteObj.GetComponent<SpriteRenderer>();

            if (sr == null)
            {
                Debug.Log("SpriteRendererが付いてないよ");
                Destroy(this);
            }
        }
        else
        {
            Debug.Log("インスペクタの設定が足りていません");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //playerが1回でも乗ったらフラグをオンにする
        if (oc.playerOn)
        {
            isOn = true;
            oc.playerOn = false;
        }

        if (isOn && !isFall)
        {
            //床をx方向に揺らす(見た目だけ)
            float x = curve.Evaluate(timer * vibSpeed) * vibWidth;//x軸方向の位置をアニメーションカーブから求める(時間×スピード×揺れ幅)
            spriteObj.transform.position = spriteDefaultPos + new Vector3(x, 0, 0);//x軸方向の位置を初期位置に加算

            //一定時間経過したら
            if (timer > fallTime)
            {
                isFall = true;//落下フラグをONにする
            }
            timer += Time.deltaTime;//時間を加算
        }

        //戻ってきたら点滅させる処理
        if (isRetrun)
        {
            //明滅(戻り処理)
            if (blinkTimer > 0.2f)
            {
                sr.enabled = true;
                blinkTimer = 0.0f;
            }
            //明滅(消灯)
            else if (blinkTimer > 0.1f)
            {
                sr.enabled = false;
            }
            //明滅(点灯)
            else
            {
                sr.enabled = true;
            }

            //コンテニュー演出が1秒以上経過したら(終了処理)
            if (returnTimer > 0.8f)
            {
                //元の状態にリセットする(終了)
                isRetrun = false;
                blinkTimer = 0.0f;
                returnTimer = 0.0f;
                sr.enabled = true;
            }
            //コンテニュー演出中
            else
            {
                //ゲーム内時間をすすめ、演出をする
                blinkTimer += Time.deltaTime;
                returnTimer += Time.deltaTime;
            }
        }
    }

    //床の落下処理
    private void FixedUpdate()
    {
        //落下フラグがONなら
        if (isFall)
        {
            //落下速度を代入する＝落下させる
            rb.velocity = fallVelocity;

            //一定時間経過したら元の位置に戻る
            if(fallTimer > fallTime)
            {
                isRetrun = true;
                transform.position = floorDefaultPos;
                rb.velocity = Vector2.zero;
                isFall = false;
                timer = 0.0f;
                fallTimer = 0.0f;
            }
            else
            {
                fallTimer += Time.deltaTime;
                isOn = false;
            }
        }
    }
}
