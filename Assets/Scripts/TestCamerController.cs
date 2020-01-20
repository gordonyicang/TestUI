using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCamerController: MonoBehaviour {

    Transform playerTrans;
    Text label;
    SuperButton showHideBtn;
    SuperButton resetBtn;
    SuperButton leftZhuanBtn;
    SuperButton rightZhuanBtn;
    SuperButton upMoveBtn;
    SuperButton downMoveBtn;

    SuperButton shangZhuanBtn;
    SuperButton xiaZhuanBtn;
    SuperButton leftMoveBtn;
    SuperButton rightMoveBtn;

    SuperButton frontMoveBtn;
    SuperButton behideMoveBtn;

    SuperButton fontZhuanBtn;
    SuperButton behideZhuanBtn;

    bool leftZhuanOpen = false;
    bool rightZhuanOpen = false;
    bool shangZhuanOpen = false;
    bool xiaZhuanOpen = false;
    bool frontZhuanBtnOpen = false;
    bool behideZhuanBtnOpen = false;
    bool upMoveOpen = false;
    bool downMoveOpen = false;
    bool leftMoveOpen = false;
    bool rightMoveOpen = false;
    bool frontMoveOpen = false;
    bool behideMoveOpen = false;

    Vector3 cubeOldPos;
    Vector3 cameraOldPos;


	//定义摄像机与Target的偏移位置
    private Vector3 offsetStation;

    private float speed = 5f;

    // Use this for initialization
    void Start () {
        playerTrans = GameObject.Find("target").transform;
        label = GameObject.Find("ParamLabel").GetComponent<Text>();
        showHideBtn = GameObject.Find("showBtn").AddComponent<SuperButton>();
        resetBtn = GameObject.Find("resetBtn").AddComponent<SuperButton>();
        leftZhuanBtn = GameObject.Find("leftZhuanBtn").AddComponent<SuperButton>();
        rightZhuanBtn = GameObject.Find("rightZhuanBtn").AddComponent<SuperButton>();
        shangZhuanBtn = GameObject.Find("shangZhuanBtn").AddComponent<SuperButton>();
        xiaZhuanBtn = GameObject.Find("xiaZhuanBtn").AddComponent<SuperButton>();
        fontZhuanBtn = GameObject.Find("fontZhuanBtn").AddComponent<SuperButton>();
        behideZhuanBtn = GameObject.Find("behideZhuanBtn").AddComponent<SuperButton>();

        upMoveBtn = GameObject.Find("upMoveBtn").AddComponent<SuperButton>();
        downMoveBtn = GameObject.Find("downMoveBtn").AddComponent<SuperButton>();
        leftMoveBtn = GameObject.Find("leftMoveBtn").AddComponent<SuperButton>();
        rightMoveBtn = GameObject.Find("rightMoveBtn").AddComponent<SuperButton>();
        frontMoveBtn = GameObject.Find("frontMoveBtn").AddComponent<SuperButton>();
        behideMoveBtn = GameObject.Find("behideMoveBtn").AddComponent<SuperButton>();

        resetBtn.setTouchEnabled(true);
        showHideBtn.setTouchEnabled(true);
        leftZhuanBtn.setTouchEnabled(true);
        rightZhuanBtn.setTouchEnabled(true);
        shangZhuanBtn.setTouchEnabled(true);
        xiaZhuanBtn.setTouchEnabled(true);
        fontZhuanBtn.setTouchEnabled(true);
        behideZhuanBtn.setTouchEnabled(true);

        upMoveBtn.setTouchEnabled(true);
        downMoveBtn.setTouchEnabled(true);
        leftMoveBtn.setTouchEnabled(true);
        rightMoveBtn.setTouchEnabled(true);
        frontMoveBtn.setTouchEnabled(true);
        behideMoveBtn.setTouchEnabled(true);


        resetBtn.setLuaCallback("resetBtn", clickHandler, downHandler, upHandler);
        showHideBtn.setLuaCallback("showHideBtn", clickHandler, downHandler, upHandler);
        leftZhuanBtn.setLuaCallback("leftZhuan",clickHandler, downHandler, upHandler);
        rightZhuanBtn.setLuaCallback("rightZhuan", clickHandler, downHandler, upHandler);
        shangZhuanBtn.setLuaCallback("shangZhuan", clickHandler, downHandler, upHandler);
        xiaZhuanBtn.setLuaCallback("xiaZhuan", clickHandler, downHandler, upHandler);
        fontZhuanBtn.setLuaCallback("fontZhuanBtn", clickHandler, downHandler, upHandler);
        behideZhuanBtn.setLuaCallback("behideZhuanBtn", clickHandler, downHandler, upHandler);

        upMoveBtn.setLuaCallback("upMove", clickHandler, downHandler, upHandler);
        downMoveBtn.setLuaCallback("downMove", clickHandler, downHandler, upHandler);
        leftMoveBtn.setLuaCallback("leftMove", clickHandler, downHandler, upHandler);
        rightMoveBtn.setLuaCallback("rightMove", clickHandler, downHandler, upHandler);
        frontMoveBtn.setLuaCallback("frontMove", clickHandler, downHandler, upHandler);
        behideMoveBtn.setLuaCallback("behideMove", clickHandler, downHandler, upHandler);

        cubeOldPos = playerTrans.position;
        cameraOldPos = transform.position;
        transform.LookAt(playerTrans.position);
		//得到偏移量
		offsetStation = transform.position - playerTrans.position;
    }

    void clickHandler(string name, float x, float y)
    {
        if (name == "showHideBtn")
        {
            leftZhuanBtn.gameObject.SetActive(!leftZhuanBtn.gameObject.activeSelf);
            label.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
            rightZhuanBtn.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
            shangZhuanBtn.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
            xiaZhuanBtn.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
            fontZhuanBtn.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
            behideZhuanBtn.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
            upMoveBtn.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
            downMoveBtn.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
            leftMoveBtn.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
            rightMoveBtn.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
            frontMoveBtn.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
            behideMoveBtn.gameObject.SetActive(leftZhuanBtn.gameObject.activeSelf);
        }
        else if(name == "resetBtn")
        {
            transform.position = cameraOldPos;
            playerTrans.position = cubeOldPos;
            transform.LookAt(playerTrans.position);
            offsetStation = transform.position - playerTrans.position;
        }
    }

    void upHandler(string name, float x, float y)
    {
        if (name == "leftZhuan")
        {
            leftZhuanOpen = false;
        }
        else if (name == "rightZhuan")
        {
            rightZhuanOpen = false;
        }
        else if (name == "shangZhuan")
        {
            shangZhuanOpen = false;
        }
        else if (name == "xiaZhuan")
        {
            xiaZhuanOpen = false;
        }
        else if (name == "fontZhuanBtn")
        {
            frontZhuanBtnOpen = false;
        }
        else if (name == "behideZhuanBtn")
        {
            behideZhuanBtnOpen = false;
        }
        else if (name == "upMove")
        {
            upMoveOpen = false;
        }
        else if (name == "downMove")
        {
            downMoveOpen = false;
        }
        else if (name == "leftMove")
        {
            leftMoveOpen = false;
        }
        else if (name == "rightMove")
        {
            rightMoveOpen = false;
        }
        else if (name == "frontMove")
        {
            frontMoveOpen = false;
        }
        else if (name == "behideMove")
        {
            behideMoveOpen = false;
        }
    }

    void downHandler(string name, float x, float y)
    {
        if (name == "leftZhuan")
        {
            leftZhuanOpen = true;
        }
        else if (name == "rightZhuan")
        {
            rightZhuanOpen = true;
        }
        else if (name == "fontZhuanBtn")
        {
            frontZhuanBtnOpen = true;
        }
        else if (name == "behideZhuanBtn")
        {
            behideZhuanBtnOpen = true;
        }
        else if (name == "upMove")
        {
            upMoveOpen = true;
        }
        else if (name == "downMove")
        {
            downMoveOpen = true;
        }
        else if (name == "shangZhuan")
        {
            shangZhuanOpen = true;
        }
        else if (name == "xiaZhuan")
        {
            xiaZhuanOpen = true;
        }
        else if (name == "leftMove")
        {
            leftMoveOpen = true;
        }
        else if (name == "rightMove")
        {
            rightMoveOpen = true;
        }
        else if (name == "frontMove")
        {
            frontMoveOpen = true;
        }
        else if (name == "behideMove")
        {
            behideMoveOpen = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        //transform.RotateAround(playerTrans.position, Vector3.up, 5 * Time.deltaTime); //摄像机围绕目标旋转
        //var mouse_x = Input.GetAxis("Mouse X");//获取鼠标X轴移动
        //var mouse_y = -Input.GetAxis("Mouse Y");//获取鼠标Y轴移动
        //if (Input.GetKey(KeyCode.Mouse1))
        //{
        //    transform.Translate(Vector3.left * (mouse_x * 15f) * Time.deltaTime);
        //    transform.Translate(Vector3.up * (mouse_y * 15f) * Time.deltaTime);
        //}
        //if (Input.GetKey(KeyCode.Mouse0))
        //{
        //    //transform.RotateAround(playerTrans.transform.position, Vector3.up, mouse_x * 5);
        //    Debug.Log("值 == "+mouse_y);
        //    transform.RotateAround(playerTrans.transform.position, transform.right, mouse_y * 5);
        //}

        //左转
        if (leftZhuanOpen)
        {
            transform.RotateAround(playerTrans.position, Vector3.up, speed * Time.deltaTime);
            offsetStation = transform.position - playerTrans.position;
            Debug.Log("左转");
        }
        //右转
        else if (rightZhuanOpen)
        {
            transform.RotateAround(playerTrans.position, Vector3.up, -speed * Time.deltaTime);
            offsetStation = transform.position - playerTrans.position;
            Debug.Log("右转");
        }
        //上翻
        else if (shangZhuanOpen)
        {
            transform.RotateAround(playerTrans.transform.position, transform.right, speed * Time.deltaTime);
            offsetStation = transform.position - playerTrans.position;
            Debug.Log("上翻");
        }
        //下翻
        else if (xiaZhuanOpen)
        {
            transform.RotateAround(playerTrans.transform.position, transform.right, -speed * Time.deltaTime);
            offsetStation = transform.position - playerTrans.position;
            Debug.Log("下翻");
        }
        //向前
        else if (frontZhuanBtnOpen)
        {
            Debug.Log("向前");
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            offsetStation = transform.position - playerTrans.position;
        }
        //退后
        else if (behideZhuanBtnOpen)
        {
            Debug.Log("退后");
            transform.Translate(Vector3.forward * -speed * Time.deltaTime);
            offsetStation = transform.position - playerTrans.position;
        }
        //上移
        else if (upMoveOpen)
        {
            playerTrans.Translate(Vector3.up * speed * Time.deltaTime);
        }
        //下移
        else if (downMoveOpen)
        {
            playerTrans.Translate(Vector3.up * -speed * Time.deltaTime);
        }
        //左移
        else if (leftMoveOpen)
        {
            playerTrans.Translate(Vector3.right * speed * Time.deltaTime);
        }
        //右移
        else if (rightMoveOpen)
        {
            playerTrans.Translate(Vector3.right * -speed * Time.deltaTime);
        }
        //左移
        else if (frontMoveOpen)
        {
            playerTrans.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        //右移
        else if (behideMoveOpen)
        {
            playerTrans.Translate(Vector3.forward * -speed * Time.deltaTime);
        }
	       // //让摄像机的位置= 人物行走的位置+与偏移量的相加
        if (leftZhuanOpen == false &&
            rightZhuanOpen == false &&
            shangZhuanOpen == false &&
            xiaZhuanOpen == false &&
            frontZhuanBtnOpen == false &&
            behideZhuanBtnOpen == false)
        {
            transform.position = offsetStation + playerTrans.position;
        }

        label.text = "摄像机位置："+ transform.position.x + "," + transform.position.y + "," + transform.position.z + "\n" + transform.localEulerAngles.x+","+transform.localEulerAngles.y+","+transform.localEulerAngles.z+"\n"+
            "格子位置："+ playerTrans.position.x + "," + playerTrans.position.y + "," + playerTrans.position.z + "\n" + playerTrans.localEulerAngles.x + "," + playerTrans.localEulerAngles.y + "," + playerTrans.localEulerAngles.z;
    }
}
