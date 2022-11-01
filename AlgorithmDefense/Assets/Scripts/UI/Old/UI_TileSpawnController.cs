using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TileSpawnController : MonoBehaviour
{


    //해당 버튼이 가지는 정보
    [System.Serializable]
    public class BuildButtonsInformation
    {
        public Sprite ButtonSprite; //버튼 이미지
        public int _cost;           //가격
        [HideInInspector]
        public TextMeshProUGUI CostText;       //가격 텍스트
        [HideInInspector]
        public Button ButtonObj;   //생성된 버튼 오브젝트 정보
    }

    public BuildButtonsInformation[] BuildButtons;


    [SerializeField]
    private Button TileButton;
    [SerializeField]
    private RectTransform buildButtonContainer;
    private CallSkill _CallSkill;
    private GameObject _buildTileMenu;


    private void Start()
    {

        //해당 오브젝트의 부모를 찾아서 찾기
        _buildTileMenu = GameObject.Find("BuildSpawnButtons");
        _CallSkill = FindObjectOfType<CallSkill>();
        CreateButton(buildButtonContainer, TileButton, BuildButtons);

    }



    public void CreateButton(RectTransform container, Button btn_Obj, BuildButtonsInformation[] btn_Slot)
    {
   
        for (int i = 0; i < btn_Slot.Length; i++)
        {
            int index = i;


            //버튼 생성
            Button newButton = Instantiate(btn_Obj, container.transform);
            btn_Slot[i].ButtonObj = newButton;


            //생성된 버튼의 좌표와 이미지 변경
            newButton.transform.localPosition = new Vector3(newButton.transform.localPosition.x + 250f * i, newButton.transform.localPosition.y, newButton.transform.localPosition.z);
            newButton.GetComponent<Image>().sprite = btn_Slot[i].ButtonSprite;


            //Debug.Log(newButton.transform.Find("Text").name);

            //현재 버튼의 하위 항목 찾기(obj.text)
            GameObject buttonText = newButton.transform.Find("Text").gameObject;
            btn_Slot[i].CostText = buttonText.GetComponent<TextMeshProUGUI>();

            //찾은 text에 cost를 형변환해서 변경함
            string conversion = btn_Slot[i]._cost.ToString();
            btn_Slot[i].CostText.text = conversion;


            //해당 버튼 클릭시 정보전달
            btn_Slot[index].ButtonObj.onClick.AddListener(() =>
            {

                //TileButtonOnClick(index);

                int firstGoid = Managers.Game.Gold;

                GoldAnimation.GetInstance.GoldExpenditure(btn_Slot[index]._cost);

                if(firstGoid>= btn_Slot[index]._cost)
                {
                    //Debug.Log("스킬실행");
                    _CallSkill.skill(index);
                }


            });

        }


    }







}
