using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GManager : MonoBehaviour
{
    //GameManagerインスタンス保存用変数
    public static GManager instance = null;
    
    //管理変数定義
    [Header("スコア管理")] public int score;
    [Header("ステージ管理")] public int stageNum;
    [Header("現在のコンティニュー位置")] public int continueNum;
    [Header("現在の残機")] public int heartNum;
    [Header("デフォルトの残機")] public int defaultHeartNum;
    [Header("GameOver時のSE")] public AudioClip gameOverSE;
    [HideInInspector] public bool isGameOver = false;//ゲームオーバーフラグ

    private AudioSource audioSource = null;

    private void Awake()
    {
        //もしインスタンスの中身が空ならインスタンスにこのオブジェクトのアドレスを代入
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);//すでにインスタンスがあればオブジェクトごと破棄する
        }
    }

    private void Start()
    {
        //AudioSourceのコンポーネントを捕まえる
        audioSource = GetComponent<AudioSource>();
    }

    #region//残機加算処理
    /// <summary>
    /// 残機を加算するメソッド
    /// </summary>
    public void AddHeartNum()
    {
        //残機が99未満なら
        if (heartNum < 99)
        {
            ++heartNum;//残機を1増やす
        }
    }
    #endregion

    #region//残機減算処理
    /// <summary>
    /// 残機を減算し、0ならゲームオーバーにするメソッド
    /// </summary>
    public void SubHeartNum()
    {
        // 残機が0以外なら
        if (heartNum > 0)
        {
            --heartNum; //残機数を減らす
        }
        else
        {
            isGameOver = true;//残機0でゲームオーバーフラグをtrueにする
            //GameOverSE再生
            PlaySE(gameOverSE);
        }
    }
    #endregion

    #region//ReSTART処理
    /// <summary>
    /// ReSTART時の処理
    /// </summary>
    public void RetryGame()
    {
        //フラグ、スコア、残機等をリセットする
        isGameOver = false;
        heartNum = defaultHeartNum;
        score = 0;
        stageNum = 1;
        continueNum = 0;
    }
    #endregion

    #region//SE再生用メソッド
    /// <summary>
    /// SE1回のみ再生する処理(重複OK)
    /// </summary>
    /// <param name="clip">音源の名前</param>
    public void PlaySE(AudioClip clip)
    {
        if (audioSource != null)
        {
            //引数に指定した音源を1回再生する(別の音源と重複可能)
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("オーディオが設定されていません");
        }
    }
    #endregion
}
