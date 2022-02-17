using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject StandardCanvas;
    public GameObject StandardCanvasLineTemplate;
    public GameObject MainCanvas;
    public GameObject MainCanvasLineTemplate;
    public GameObject MainCanvasFgLineTemplate;
    public GameObject MainCanvasPointTemplate;
    public PatternRenderer standard;
    public PatternRenderer main;
    public GameObject RetryButton;
    private RectTransform RetryButtonRect;
    public GameObject BackButton;
    private RectTransform BackButtonRect;
    public GameObject SettingsButton;
    private RectTransform SettingsButtonRect;

    void Start()
    {
        var stdpat_val = new List<(int a, int b)> {
                (2,4),
                (2,5),
                (4,5),
                (5,8),
                (7,8),
                (8,9),
            };
        var stdpat = new PatternMatcher(stdpat_val);
        standard = new PatternRenderer(
            StandardCanvas,
            StandardCanvasLineTemplate,
            stdpat_val);
        main = new PatternRenderer(
            MainCanvas,
            MainCanvasLineTemplate,
            new List<(int a, int b)> {
                (1,2),
                (2,3),
                (4,5),
                (5,6),
                (7,8),
                (8,9),
                (1,4),
                (4,7),
                (2,5),
                (5,8),
                (3,6),
                (6,9),
                (3,5),
                (6,8),
            },
            editable: true,
            pointTemplate: MainCanvasPointTemplate,
            fgLineTemplate: MainCanvasFgLineTemplate);
        main.OnValueChange = delegate () {
            var newpat = new PatternMatcher(main.Value);
            if(stdpat.IsSame(newpat)) {
                // UnityEditor.EditorUtility.DisplayDialog("标题", "成功啦！", "确认", "取消");
                // UnityEditor.EditorUtility.DisplayDialog("demo", "成功啦！\n这只是一个demo所以后续行为并没有写……", "确认");
                main.ClearGraph();
            }
        };
        this.RetryButtonRect = this.RetryButton.GetComponent<RectTransform>();
        this.BackButtonRect = this.BackButton.GetComponent<RectTransform>();
        this.SettingsButtonRect = this.SettingsButton.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        standard.Update();
        main.Update();

        if(Input.GetMouseButtonUp(0)) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3[] corners = new Vector3[4];
            this.RetryButtonRect.GetWorldCorners(corners);
            Rect rectRetry = new Rect(corners[0], corners[2]-corners[0]);
            this.BackButtonRect.GetWorldCorners(corners);
            Rect rectBack = new Rect(corners[0], corners[2]-corners[0]);
            this.SettingsButtonRect.GetWorldCorners(corners);
            Rect rectSettings = new Rect(corners[0], corners[2]-corners[0]);

            if(rectRetry.Contains(pos)) {
                this.main.ClearGraph();
            }
            // if(rectBack.Contains(pos)) {
            //     UnityEditor.EditorUtility.DisplayDialog("demo", "你点击了返回\n这只是一个demo所以后续行为并没有写……", "确认");
            // }
            // if(rectSettings.Contains(pos)) {
            //     UnityEditor.EditorUtility.DisplayDialog("demo", "你点击了设置\n这只是一个demo所以后续行为并没有写……", "确认");
            // }

        }
    }

    // // Meaningless
    // void OnDestroy() {
    //     standard = null;
    //     main = null;
    // }
}
