﻿using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

public class RewardBoxOpenScript : MonoBehaviour
{
    public GameObject box0;
    public GameObject box1;
    public GameObject box2;

    private List<GameObject> _boxes;
    private Camera _cam;
    private Text _coinsAmountText;
    private const int RewardMin = 5;
    private const int RewardMax = 15; 
    private float _smokeOffsetZ = -1.5f;
    [SerializeField] private float _nextLevelDelay = 3.0f; //todo: actually instead of this should be toss coins animation length;
    private bool _controlLocked;

    private void Start()
    {
        if (!box0) box0 = GameObject.Find("Box_0");
        if (!box1) box1 = GameObject.Find("Box_1");
        if (!box2) box2 = GameObject.Find("Box_2");
        
        _cam = Camera.main;
        _boxes = new List<GameObject> {box0, box1, box2};
        _coinsAmountText = GameObject.Find("CoinsAmountText").GetComponent<Text>();
        _coinsAmountText.text = "" + SaveController.Instance.Save.CoinsCount;
    }
    
    private void Update()
    {
        if (_controlLocked) return;
        
        // if (Input.touchCount > 0) //TODO: replace on release
        // {
        //  var touch = Input.GetTouch(0);
        var touches = InputHelper.GetTouches();
        if (touches.Count > 0)
        {
            var touch = touches[0];
            var touchNear = _cam.ScreenToWorldPoint(
                new Vector3(touch.position.x, touch.position.y, _cam.nearClipPlane));
            var touchFar = _cam.ScreenToWorldPoint(
                new Vector3(touch.position.x, touch.position.y, _cam.farClipPlane));

            if (touch.phase == TouchPhase.Ended)
            {
                if (Physics.Raycast(touchNear, touchFar - touchNear, out var hitInfo))
                {
                    foreach (var box in _boxes)
                    {
                        if (box == hitInfo.collider.gameObject)
                        {
                            var newCamPos = box.transform.position;
                            var effectSpawnPos = newCamPos;

                            newCamPos.z = -4;
                            effectSpawnPos.z = _smokeOffsetZ;
                            box.GetComponent<Rotator>().enabled = false;
                            this.GetComponent<SmoothCameraTranslator>().MoveTowards(newCamPos);
                            this.GetComponent<SmoothObjectRotationResetor>().RestoreBoxRotation(box);

                            //TODO: instead of control lock there could be tap-and-earn mechanic
                            _controlLocked = true;
                            StartCoroutine(nameof(RunGiftReceivingSequence), box);
                        }
                    }
                }
            }
        }
    }
    
    private IEnumerator RunGiftReceivingSequence(GameObject box)
    {
        yield return new WaitForSecondsRealtime(this.GetComponent<SmoothObjectRotationResetor>().timeToRestore);

        var effectObj = Resources.Load<GameObject>("Prefabs/SmokeScreenAlter");
        var effectPos = box.transform.position;

        effectPos.z = _smokeOffsetZ;
        GameObject.Instantiate(effectObj, effectPos, Quaternion.identity);
        box.GetComponent<Animator>().enabled = true;
        box.GetComponent<Animator>().Play("DestroyBox");
    }

    public void TossCoinsAnimation()
    {
        //TODO: ILYA WILL MAKE AN ANIMATION HERE!
        int coinsAmount = Random.Range(RewardMin, RewardMax);
        // GameController.Instance.AddCoins(coinsAmount, _coinsAmountText);
        GameObject canvas = GameObject.Find("UI");
        GameObject coin = Resources.Load<GameObject>("Prefabs/Coin2d");
        for (int i = 0; i < coinsAmount; i++)
        {
            //TODO монетки должны вылетать из метеорита, а не из центра экрана
            Vector3 pos = new Vector3(Random.Range(transform.position.x - 100.0f, transform.position.x + 100.0f), 
                Random.Range(transform.position.y - 100.0f, transform.position.y + 100.0f), transform.position.z);
            coin = Instantiate(coin, pos, Quaternion.identity);
            coin.transform.SetParent(canvas.transform, false);
        }
        // Debug.Log("Coins tossed!");
        StartCoroutine(nameof(GoToNextLevel));
    }

    private IEnumerator GoToNextLevel()
    {
        yield return new WaitForSecondsRealtime(_nextLevelDelay);

        SceneManager.sceneLoaded += GameController.Instance.SetUpLoadedScene;
        SceneManager.LoadScene(1);
    }
}
