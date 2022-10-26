using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;

public class StageTileInformation : MonoBehaviour
{
    //kgw sheet
    //const string URL = "https://script.google.com/macros/s/AKfycbxcyMz1T42QlZqBzBTiPHMYs-SJJlBFeqpGw6TN7Ws1_7EzS4SzGrL_lPuc0s8E0uBKUA/exec";

    const string URL = "https://script.google.com/macros/s/AKfycbxzoHfRTzsaIR5un1lkT9sm0Tiwhm0iIhEWepodlykJAy0db_Oaj4BAlDPt_aSslDFN/exec";




    private int stageNum;

    [System.Serializable]
    public class GoogleData
    {
        public string order;
        public string result;
        public string deta;
        public string value;

    }
    public GoogleData GD;

    public int[][] StageTileData = new int[5][];

    [HideInInspector]
    public int StarCount;   //클리어 후 받은 별 갯수


    private void Start()
    {
        StarCount = 0;

        LoadingControl.GetInstance.LoadingCompleteAction += SetTileData;

        //클릭한 스테이지 번호 가져오기
        stageNum = PlayerPrefs.GetInt("StageNum");
        Debug.Log("stageNum: " + stageNum);
        WWWForm form = new WWWForm();

        //각각의 정보 이름과 넣을정보를 넣어줌
        form.AddField("order", "getStageTile");
        form.AddField("num", stageNum);

        //보내기
        StartCoroutine(Post(form));



    }


    IEnumerator Post(WWWForm form)
    {

        //using을 안쓰면 통신을 안할 때가 있음
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            //보내는 동안 대기
            yield return www.SendWebRequest();

            //다 보냈으면
            if (www.isDone)
            {
                //값 저장 및 msg출력
                Response(www.downloadHandler.text);

                switch (GD.order)
                {
                    case "getStageTile":
                        GetTileDataSave();

                        // 로딩 완료

                        LoadingControl.GetInstance.GameSceneLoadingComplete();

                        break;

                    case "setValue":
                        break;

                }


            }
            else
            {
                Debug.Log("Google Apps Script 응답없음.");
            }


        }

    }


    //값 저장 및 msg출력
    public void Response(string json)
    {
        //받아온 값이 비어있다면
        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("빈값받음");
            return;
        }

        //값 저장
        GD = JsonUtility.FromJson<GoogleData>(json);

        //에러일때
        if (GD.result == "ERROR")
        {
            Debug.Log("(" + GD.order + "): " + GD.result + " [deta: " + GD.deta + "]");
            return;
        }


        Debug.Log("(" + GD.order + "): " + GD.result + " [deta: " + GD.deta + "]");
    }

    public void GetTileDataSave()
    {
        string[] data = GD.deta.Split(',');
        int count = 0;

        //Debug.Log("data.Length: " + data.Length);

        for (int i = 0; i < 5; i++)
        {
            StageTileData[i] = new int[5];

            for (int j = 0; j < 5; j++)
            {
                StageTileData[i][j] = int.Parse(data[count]);
                count++;
            }
        }

    }



    public void SetTileData()
    {
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (StageTileData[y][x] == -1)
                {
                    continue;
                }

                TileManager.GetInstance.SetTile(
                    Define.Tilemap.Building,
                    new Vector3Int(
                        Managers.Game.Setting.StartPosition.x + x + 1,
                        Managers.Game.Setting.StartPosition.y + y + 1, 0),
                        (Define.Building)StageTileData[y][x]);
            }
        }

    }

    //클리어시 별 저장 후 다음챕터 오픈
    public void GameClearSetStarCount()
    {
        //클리어를 못한게 아니라면
        if (StarCount != 0)
        {
            WWWForm form = new WWWForm();

            //각각의 정보 이름과 넣을정보를 넣어줌
            form.AddField("order", "setValue");
            form.AddField("star", StarCount);

            //보내기
            StartCoroutine(Post(form));
        }
    }

    //뒤로가기
    public void BackStartScene()
    {
        Time.timeScale = 1f;
        //시작화면으로 이동
        Managers.Clear();
        SceneManager.LoadScene(0);
    }

    //다시하기
    public void NowStageAgain()
    {
        Time.timeScale = 1f;
        //Game씬(1번)다시 시작
        Managers.Clear();
        SceneManager.LoadScene(1);
    }

    //다음챕터
    public void NextStageNumSet()
    {
        Time.timeScale = 1f;

        int stage = PlayerPrefs.GetInt("StageCount");
        if (stage != stageNum)
        {
            //다음 스테이지 번호로 변경저장
            PlayerPrefs.SetInt("Num", stageNum + 1);

            //Game씬(1번)다시 시작
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log("모든 스테이지 클리어!!!");
            //Start씬(0번) 시작
            Managers.Clear();
            SceneManager.LoadScene(0);
        }

    }





}