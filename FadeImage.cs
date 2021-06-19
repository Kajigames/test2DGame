using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour
{
    //インスペクタでパラメータ調整
    [Header("最初からフェードインが完了しているか")] public bool firstFadeInComp;

    //private変数
    private Image img = null;//Image
    private int frameCount = 0;//経過時間(フレーム数)カウント用
    private float timer = 0.0f;//フェード時間計測用
    private bool fadeIn = false;//フェードイン判定
    private bool fadeOut = false;
    private bool compFadeIn = false;//フェードイン完了判定
    private bool compFadeOut = false;//フェードアウト完了判定

    #region//フェードイン開始メソッド呼び出し
    /// <summary>
    /// フェードイン開始処理を他のスクリプトから呼べるようにする
    /// </summary>
    public void StartFadeIn()
    {
        //フェード中は新たにフェードを開始しない
        if ( fadeIn || fadeOut)
        {
            return;
        }
        //フェードインの初期設定
        fadeIn = true;
        compFadeIn = false;
        timer = 0.0f;
        img.color = new Color(1, 1, 1, 1);
        img.fillAmount = 1;//当たり判定を有効にし、ボタンを押せないようにする
        img.raycastTarget = true;
    }
    #endregion

    #region//フェードイン完了メソッド呼び出し
    /// <summary>
    /// フェードイン完了フラグを他のスクリプトから呼べるようにする
    /// </summary>
    /// <returns>compFadeIn</returns>
    public bool isFadeInComplete()
    {
        return compFadeIn;
    }
    #endregion

    #region//フェードアウト開始メソッド呼び出し
    /// <summary>
    /// フェードアウト開始処理を他のスクリプトから呼べるようにする
    /// </summary>
    public void StartFadeOut()
    {
        //フェード中は新たにフェードを開始しない
        if (fadeIn || fadeOut)
        {
            return;
        }
        //フェードアウトの初期設定
        fadeOut = true;
        compFadeOut = false;
        timer = 0.0f;
        img.color = new Color(1, 1, 1, 0);
        img.fillAmount = 0;
        img.raycastTarget = true;
    }
    #endregion

    #region//フェードアウト完了メソッド呼び出し
    /// <summary>
    /// フェードアウト完了フラグを他のスクリプトから呼べるようにする
    /// </summary>
    /// <returns>compFadeOut</returns>
    public bool isFadeOutComplete()
    {
        return compFadeOut;
    }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        //コンポーネントのインスタンスを捕まえる
        img = GetComponent<Image>();

        //最初からフェードが完了した状態にするかの切り分け
        //インスペクタのfirstFadeCompにチェックが入っていたら
        if (firstFadeInComp)
        {
            FadeInComplete();//FadeInCompleteメソッドを呼び出す
        }
        else
        {
            StartFadeIn();//フェードイン開始メソッドを呼び出す
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (frameCount > 2)
        {
            if (fadeIn)
            {
                FadeInUpdate();//フェードインメソッドを呼び出し  
            }
            else if (fadeOut)
            {
                FadeOutUpdate();//フェードアウトメソッドを呼び出し
            }
        }
        ++frameCount;
    }

    #region//フェードインメソッド
    /// <summary>
    /// フェードインの毎フレーム処理をメソッド化したもの
    /// </summary>
    private void FadeInUpdate()
    {
        //1秒でフェードインする場合
        if (timer < 1f) //フェード中
        {
            img.color = new Color(1, 1, 1, 1 - timer);//1秒かけて画像を透明にする
            img.fillAmount = 1 - timer;//インスペクタのFillAmountと同じ

        }
        else //フェード完了後
        {
            FadeInComplete();//フェードイン完了メソッドの呼び出し
        }

        //timerを作動させる(時間を加算)
        timer += Time.deltaTime;
    }
    #endregion

    #region//フェードイン完了メソッド
    /// <summary>
    /// フェードイン完了後の処理をメソッド化したもの
    /// </summary>
    private void FadeInComplete()
    {
        //フェードが終わったらそれぞれの値を指定
        img.color = new Color(1, 1, 1, 0);//透明
        img.fillAmount = 0;
        img.raycastTarget = false;//ボタンを押せるようにする
        timer = 0.0f;//timerをリセット
        fadeIn = false;
        compFadeIn = true;
    }
    #endregion

    #region//フェードアウトメソッド
    /// <summary>
    /// フェードアウトの毎フレーム処理をメソッド化したもの
    /// </summary>
    private void FadeOutUpdate()
    {
        //1秒でフェードインする場合
        if (timer < 1f) //フェード中
        {
            img.color = new Color(1, 1, 1, timer);//1秒かけて画像を元の色にする
            img.fillAmount = timer;//インスペクタのFillAmountと同じ

        }
        else //フェード完了後
        {
            FadeOutComplete();//フェードアウト完了メソッドの呼び出し
        }

        //timerを作動させる(時間を加算)
        timer += Time.deltaTime;
    }
    #endregion

    #region//フェードアウト完了メソッド
    /// <summary>
    /// フェードアウト完了後の処理をメソッド化したもの
    /// </summary>
    private void FadeOutComplete()
    {
        //フェードが終わったらそれぞれの値を指定
        img.color = new Color(1, 1, 1, 1);//元の色
        img.fillAmount = 1;
        img.raycastTarget = true;//ボタンを押せないようにする
        timer = 0.0f;//timerをリセット
        fadeOut = false;
        compFadeOut = true;
    }
    #endregion
}
