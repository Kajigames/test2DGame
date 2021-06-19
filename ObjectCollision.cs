using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollision : MonoBehaviour
{
    //インスペクタで衝突時のパラメータ調整
    [Header("踏んだときにplayerが跳ねる高さ")] public float boundHeight;
    //playerが乗ったかどうかの判定(HideInInspectorでインスペクタに表示しない設定)
    [HideInInspector] public bool playerOn;
}
