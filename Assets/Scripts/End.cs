﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    [SerializeField]
    private Image bg;
    [SerializeField]
    private Sprite[] comicSprites;
    [SerializeField]
    private Button next;
    [SerializeField]
    private Button yes;
    [SerializeField]
    private Button no;

    [SerializeField]
    private float[] waitTime;

    //stats
    [SerializeField]
    private Image[] boyStatIcons;
    [SerializeField]
    private Image[] girlStatIcons;
    [SerializeField]
    private Text[] boyStatText;
    [SerializeField]
    private Text[] girlStatText; 
    [SerializeField]
    private GameObject[] stats;

    //endings
    [SerializeField]
    private Sprite[] endYesGood;
    [SerializeField]
    private float[] endWaitYesGood;

    [SerializeField]
    private Sprite[] endYesBad;
    [SerializeField]
    private float[] endWaitYesBad;

    [SerializeField]
    private Sprite[] endNoGood;
    [SerializeField]
    private float[] endWaitNoGood;

    [SerializeField]
    private Sprite credits; 


    private GameController gController; 
	// Use this for initialization
	void Start ()
    {
        gController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        next.onClick.AddListener(EnableStats);
        yes.onClick.AddListener(Yes);
        no.onClick.AddListener(No);

        StartCoroutine(ChangePanel()); 
    }

    private IEnumerator ChangePanel()
    {
        for (int i = 0; i < comicSprites.Length; i++)
        {
            if (i == comicSprites.Length - 1)
                next.gameObject.SetActive(true);

            bg.sprite = comicSprites[i];
            yield return new WaitForSeconds(waitTime[i]);
        }
    }

    private void EnableStats()
    {
        bg.gameObject.SetActive(false);
        next.gameObject.SetActive(false);

        for (int i = 0; i < boyStatIcons.Length; i++)
        {
            boyStatIcons[i].color = gController.boyIconValues[i];
            girlStatIcons[i].color = gController.girlIconValues[i];
            boyStatText[i].text = gController.boyStatValues[i].ToString();
            girlStatText[i].text = gController.girlStatValues[i].ToString();
        }

        for (int i = 0; i < stats.Length; i++)
            stats[i].SetActive(true);
    }

    private void Yes()
    {
        for (int i = 0; i < stats.Length; i++)
            stats[i].SetActive(false);

        StartCoroutine(Stay());
        Camera main = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        main.backgroundColor = Color.black;
    }

    private void No()
    {
        for (int i = 0; i < stats.Length; i++)
            stats[i].SetActive(false);

        StartCoroutine(Leave());
        Camera main = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        main.backgroundColor = Color.black; 
    }

    private IEnumerator Stay()
    {
        int totalStat = gController.CheckSobriety(true) + gController.CheckSocial(true) + gController.CheckLove(true) + gController.CheckSobriety(false) + gController.CheckSocial(false) + gController.CheckLove(false);
        float initialTime = 0;
        float totalTime = 5;
        Color startColor = bg.color;

        //fadeout audio
        StartCoroutine(gController.FadeAudio(false, totalTime));
        //Yes Bad
        if (gController.CheckSobriety(true) <= -3 || gController.CheckSobriety(false) <= -3 || 
            gController.CheckSocial(true) <= -3 || gController.CheckSocial(false) <= -3 ||
            gController.CheckLove(true) <= -3 || gController.CheckLove(false) <= -3 || 
            totalStat <= 0)
        {
            bg.gameObject.SetActive(true);
            for (int i = 0; i < endYesBad.Length; i++)
            {
                bg.sprite = endYesBad[i];
                yield return new WaitForSeconds(endWaitYesBad[i]);
            }

            while (initialTime < totalTime)
            {
                bg.color = Color.Lerp(startColor,new Color(bg.color.r, bg.color.g, bg.color.b, 0),initialTime/totalTime);
                initialTime += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(1);
            StartCoroutine(Restart());
        }
        //Yes Good
        else
        {
            bg.gameObject.SetActive(true);
            for (int i = 0; i < endYesGood.Length; i++)
            {
                bg.sprite = endYesGood[i];
                yield return new WaitForSeconds(endWaitYesGood[i]);
            }
            //fadeout
            StartCoroutine(gController.FadeAudio(false, totalTime));
            while (initialTime < totalTime)
            {
                bg.color = Color.Lerp(startColor, new Color(bg.color.r, bg.color.g, bg.color.b, 0), initialTime / totalTime);
                initialTime += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(1);
            StartCoroutine(Restart());
        }
    }

    private IEnumerator Leave()
    {
        float initialTime = 0;
        float totalTime = 5;
        Color startColor = bg.color;
        bg.gameObject.SetActive(true);

        //fadeout audio
        StartCoroutine(gController.FadeAudio(false, totalTime));

        for (int i = 0; i < endNoGood.Length; i++)
        {
            bg.sprite = endNoGood[i];
            yield return new WaitForSeconds(endWaitNoGood[i]);
        }
        //fadeout
        while (initialTime < totalTime)
        {
            bg.color = Color.Lerp(startColor, new Color(bg.color.r, bg.color.g, bg.color.b, 0), initialTime / totalTime);
            initialTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(Restart());

    }

    private IEnumerator Restart()
    {
        float initialTime = 0;
        float totalTime = 3;
        Color startColor = bg.color;
        bg.sprite = credits;

        while (initialTime < totalTime)
        {
            bg.color = Color.Lerp(startColor, new Color(bg.color.r, bg.color.g, bg.color.b, 1), initialTime / totalTime);
            initialTime += Time.deltaTime;
            yield return null;
        }

        initialTime = 0;
        totalTime = 3;
        startColor = bg.color;
        yield return new WaitForSeconds(3);       

        while (initialTime < totalTime)
        {
            bg.color = Color.Lerp(startColor, new Color(bg.color.r, bg.color.g, bg.color.b, 0), initialTime / totalTime);
            initialTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(2);

        Destroy(gController.gameObject);
        SceneManager.LoadScene("Start");
    }
}
