using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UniRx;
using DG.Tweening;

public class AddButton : MonoBehaviour
{
    [SerializeField] public Transform btnField = default;
    [SerializeField] public GameObject btnFalse = default;
    [SerializeField] public GameObject btnTrue = default;
    [SerializeField] public int falseNum = default;
    [SerializeField] public int trueNum = default;
    [SerializeField] public Vector2 panelSize = new Vector2(200, 200);
    [SerializeField] public Vector2 offset = new Vector2(200, -600);
    List<Vector2> positions = new List<Vector2>();
    List<GameObject> targets = new List<GameObject>();
    List<GameObject> buttons = new List<GameObject>();
    List<GameObject> flashBtn = new List<GameObject>();

    void Start()
    {
        // 全パネル配列
        GameObject[] arr = new GameObject[trueNum + falseNum];
        int j = 0;

        // 不正解パネル生成
        for(int i = 0; i < falseNum ; i++)
        {
            GameObject button = Instantiate(btnFalse);
            button.transform.SetParent(btnField, false);
            button.GetComponentInChildren<Text>().text = "False" + (i + 1);
            arr[j++] = button;
        }

        // 正解パネル生成
        for(int i = 0; i < trueNum ; i++)
        {
            GameObject button = Instantiate(btnTrue);
            button.transform.SetParent(btnField, false);
            button.GetComponentInChildren<Text>().text = "True" + (i + 1);
            arr[j++] = button;
            flashBtn.Add(button);
        }

        // Observable.FromCoroutine(SetColor, publishEveryYield: false)
        //     .Subscribe(
        //         _ => Debug.Log("OnNext"),
        //         () => Debug.Log("OnCompleted")
        // ).AddTo(this);

        // パネルをランダムで並べる
        arr.Shuffle();

        // 順番に光らせる
        StartCoroutine("SetColor");

        float w = panelSize.x, h = panelSize.y;

        for(int i = 0; i < arr.Length; i++) {
            var go = arr[i];
            var t = go.GetComponent<RectTransform>();

            // パネルのサイズ
            t.sizeDelta = new Vector2(w, h);
            // パネルを中央に並べる
            var x = (i % 3) * w + offset.x;
            var y = Mathf.Floor(i / 3) * h + offset.y;

            go.name = string.Format("button{0}", i + 1);

            // パネルを入れ替え処理
            go.GetComponent<Button>().OnClickAsObservable().Select(_ => go).Subscribe(target => {
                if (targets.Count < 2) {
                    Debug.Log(target);
                    targets.Add(target);
                }
                if (targets.Count == 2) {
                    Debug.LogFormat("{0} : {1}", targets[0], targets[1]);
                    int id1 = findIndex(targets[0]);
                    int id2 = findIndex(targets[1]);
                    Debug.LogFormat("index: {0}, {1}", id1, id2);

                    var pos1 = positions[id1];
                    var pos2 = positions[id2];

                    targets[0].GetComponent<RectTransform>().DOAnchorPos(pos2, .5f);
                    targets[1].GetComponent<RectTransform>().DOAnchorPos(pos1, .5f);

                    buttons[id1] = targets[1];
                    buttons[id2] = targets[0];

                    var btnText1 = targets[0].transform.Find("Text").GetComponent<Text>().text;
                    var btnText2 = targets[1].transform.Find("Text").GetComponent<Text>().text;

                    if(btnText1.Contains("True1") && btnText2.Contains("True2"))
                    {
                        targets.Clear();
                    }
                    else if(btnText1.Contains("True3") && btnText2.Contains("True4"))
                    {
                        targets.Clear();
                    }
                    else
                    {
                        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
            });
            t.anchoredPosition = new Vector2(x, y);
            positions.Add(new Vector2 (x, y));
            buttons.Add(go);
        }
    }

    int findIndex(GameObject obj) {
        for(int i = 0; i < buttons.Count; i++) {
            if (buttons[i] == obj) {
                return i;
            }
        }
        return -1;
    }
    public void FlashBtn(int num)
    {
        flashBtn[num].transform.DORotate(new Vector3(0f, 360f, 0f), 1f, RotateMode.FastBeyond360);
        flashBtn[num].GetComponent<Image>().material.SetColor("_Emission", Color.red);
    }
    IEnumerator SetColor()
    {
        for(int i = 0; i < trueNum; i++)
        {
            FlashBtn(i);
            yield return new WaitForSeconds(2.0f);
            yield return null;
        }
    }
}

public static class Extensions
{
    //配列の要素をシャッフルする
    public static void Shuffle<T>(this T[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }
    }
}
