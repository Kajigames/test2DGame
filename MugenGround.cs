using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MugenGround : MonoBehaviour
{
    [Header("ゲームオブジェクト")] public GameObject groundObj;
    [Header("床の移動速度")] public float speed = 20.0f;
    [Header("床の消失点x座標")] public float disappear = -10.0f;
    [Header("床の再出現x座標")] public float respawn = 30.0f;

    //ゲームオブジェクトを10個生成し、配列に格納
    GameObject[] step = new GameObject[10];



    // Start is called before the first frame update
    void Start()
    {
        //床の1つ1つに名前をつけて配列に格納する(step[0],step[1]・・・)
        for (int i = 0; i < step.Length; i++)
        {
            //step[0]~step[9]までの床をそれぞれ(0,0,0),(1,0,0)・・・(36,0,0)の位置に出現させる
            step[i] = Instantiate(groundObj, new Vector3(4 * i, 0, 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < step.Length; i++)
        {
            step[i].gameObject.transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);

            if (step[i].gameObject.transform.position.x < disappear)
            {
                ChangeScale(i);
                step[i].gameObject.transform.position = new Vector3(respawn, 0, 0);
            }
        }
    }

    //床の高さを自動変更
    private void ChangeScale(int i)
    {
        //step[i]の1つ前の床をstep[x]とする
        //step[0]の時に配列外参照とならないようにi+9を10で割った余りをstep[x]とする
        int x = (i + 9) % 10;

        //step[x]の床の高さが0.5だったら
        if (step[x].transform.localScale.y == 0.5)
        {
            //step[i]をstep[x]と同じか、それより高い高さにする
            step[i].transform.localScale = step[x].transform.localScale + new Vector3(0, Random.Range((float)0.0, (float)1.0), 0);
        }
        //床の高さが2.0より大きければ
        else if (step[x].transform.localScale.y > 2.0)
        {
            //step[i]をstep[x]と同じか、それより低い高さにする
            step[i].transform.localScale = step[x].transform.localScale + new Vector3(0, Random.Range((float)-1.0, (float)0.0), 0);
        }
        else
        {
            //step[i]の高さをランダムに変更(step[x]より高い場合もあれば、低い場合もある)
            step[i].transform.localScale = step[x].transform.localScale + new Vector3(0, Random.Range((float)-0.5, (float)1.0), 0);
        }
    }
}
