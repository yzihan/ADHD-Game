using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnValueChangeEvent();

public class PatternRenderer
{
    public float ActivationZone = 20.0f;

    private bool editable;
    private GameObject canvas;
    private List<(int a, int b)> background;
    private GameObject bgLineTemplate;
    private GameObject fgLineTemplate;
    private GameObject pointTemplate;

    public List<(int a, int b)> Value { get; private set; }

    private List<((int a, int b) id, GameObject obj)> lines;
    private List<((int a, int b) id, GameObject obj)> fglines;
    private List<(int id, GameObject obj)> points;
    private RectTransform CanvasRect;
    private GameObject DrawingLine;
    private bool Editing;
    private int DrawingStartPoint;
    public OnValueChangeEvent OnValueChange;

    public PatternRenderer(GameObject canvas, GameObject bgLineTemplate, List<(int a, int b)> background, bool editable = false, GameObject pointTemplate = null, GameObject fgLineTemplate = null) {
        this.editable = editable;
        this.canvas = canvas;
        this.background = background;
        this.bgLineTemplate = bgLineTemplate;
        this.fgLineTemplate = fgLineTemplate;
        this.pointTemplate = pointTemplate;
        this.Value = new List<(int, int)>();
        this.lines = new List<((int, int), GameObject)>();
        this.points = new List<(int, GameObject)>();
        this.CanvasRect = canvas.GetComponent<RectTransform>();
        this.Editing = false;
        this.fglines = new List<((int, int), GameObject)>();

        bgLineTemplate.SetActive(false);
        if(fgLineTemplate) {
            fgLineTemplate.SetActive(false);
        }
        if(pointTemplate) {
            pointTemplate.SetActive(false);
        }

        var size = this.CanvasRect.rect.size;
        var placementGeometry = this.CalculateGeometry(size);

        {
            float z = bgLineTemplate.transform.localPosition.z;
            foreach(var line in background) {
                var lineObj = UnityEngine.Object.Instantiate(bgLineTemplate);
                lineObj.transform.SetParent(canvas.transform);
                var rect = lineObj.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(-size.x / 2, -size.y / 2, z);
                rect.localScale = new Vector3(1, 1, 1);
                var renderer = lineObj.GetComponent<LineRenderer>();

                Vector2 pos;

                pos = this.LocatePoint(line.a, placementGeometry);
                renderer.SetPosition(0, new Vector3(pos.x, pos.y, 0));
                pos = this.LocatePoint(line.b, placementGeometry);
                renderer.SetPosition(1, new Vector3(pos.x, pos.y, 0));

                lineObj.SetActive(true);
                this.lines.Add((line, lineObj));
            }
        }

        if(editable) {
            this.DrawingLine = UnityEngine.Object.Instantiate(fgLineTemplate);
            this.DrawingLine.transform.SetParent(canvas.transform);
            {
                float fg_z = fgLineTemplate.transform.localPosition.z;
                var rect = this.DrawingLine.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(-size.x / 2, -size.y / 2, fg_z);
                rect.localScale = new Vector3(1, 1, 1);
            }
            float z = pointTemplate.transform.localPosition.z;
            placementGeometry.offset = -placementGeometry.content_half_size;
            for(var i = 1; i <= 9; i++) {
                var pointObj = UnityEngine.Object.Instantiate(pointTemplate);
                pointObj.transform.SetParent(canvas.transform);
                var rect = pointObj.GetComponent<RectTransform>();
                var pos = this.LocatePoint(i, placementGeometry);
                rect.localPosition = new Vector3(pos.x, pos.y, z);
                rect.localScale = new Vector3(1, 1, 1);

                pointObj.SetActive(true);
                this.points.Add((i, pointObj));
            }
        }
    }

    public void ClearGraph() {
        if(this.Editing) {
            this.Editing = false;
            this.DrawingLine.SetActive(false);
        }
        foreach(var item in this.fglines) {
            UnityEngine.Object.Destroy(item.obj);
        }
        this.fglines.Clear();
        this.Value.Clear();
        Debug.Log("Cleared");
    }

    public void OnResize() {
        Debug.Log("Unimplemented");
    }

    private (Vector2 offset, Vector2 content_half_size) CalculateGeometry(Vector2 size) {
        float max_padding = Mathf.Min(size.x, size.y) * 0.3f;
        float padding = Mathf.Min(max_padding, 30);
        Vector2 offset = new Vector2(padding, padding);
        Vector2 content_half_size = size / 2 - offset;
        return (offset, content_half_size);
    }

    private Vector2 LocatePoint(int id, (Vector2 offset, Vector2 content_half_size) placementGeometry) {
        (int x, int y)[] mapping = {
            (0,2),(1,2),(2,2),
            (0,1),(1,1),(2,1),
            (0,0),(1,0),(2,0),
        };
        var (x, y) = mapping[id - 1];
        return placementGeometry.offset + new Vector2(x, y) * placementGeometry.content_half_size;
    }

    private int GetLineStatus(int a, int b) {
        bool found = false;
        foreach(var bgelem in this.lines) {
            if((bgelem.id.a == a && bgelem.id.b == b) || (bgelem.id.a == b && bgelem.id.b == a)) {
                found = true;
                break;
            }
        }
        if(!found) {
            return -1;
        }
        found = false;
        foreach(var bgelem in this.fglines) {
            if((bgelem.id.a == a && bgelem.id.b == b) || (bgelem.id.a == b && bgelem.id.b == a)) {
                found = true;
                break;
            }
        }
        return !found ? 0 : 1;
    }

    private void AddLine(int a, int b, Vector2 coord_b, Vector2 coord_a) {
        float z = bgLineTemplate.transform.localPosition.z;

        var size = this.CanvasRect.rect.size;

        var lineObj = UnityEngine.Object.Instantiate(fgLineTemplate);
        lineObj.transform.SetParent(canvas.transform);
        var rect = lineObj.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(-size.x / 2, -size.y / 2, z);
        rect.localScale = new Vector3(1, 1, 1);
        var renderer = lineObj.GetComponent<LineRenderer>();

        renderer.SetPosition(0, new Vector3(coord_a.x, coord_a.y, 0));
        renderer.SetPosition(1, new Vector3(coord_b.x, coord_b.y, 0));

        lineObj.SetActive(true);
        this.fglines.Add(((a, b), lineObj));
        if(a > b) {
            this.Value.Add((b, a));
            Debug.Log((b, a));
        } else {
            this.Value.Add((a, b));
            Debug.Log((a, b));
        }
    }

    public void Update()
    {
        if(this.editable) {
            bool MouseDown = Input.GetMouseButtonDown(0);
            bool MouseUp = Input.GetMouseButtonUp(0);
            if(this.Editing || MouseDown || MouseUp) {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);;

                Vector3[] corners = new Vector3[4];
                this.CanvasRect.GetWorldCorners(corners);
                Rect rect = new Rect(corners[0], corners[2]-corners[0]);
                var relpos = (pos - corners[0]) * this.CanvasRect.rect.size / (corners[2] - corners[0]);

                int point_id = 0;
                Vector2 realpos = new Vector2();
                if(rect.Contains(pos)) {
                    var geom = CalculateGeometry(this.CanvasRect.rect.size);
                    for(var i = 1; i <= 9; i++) {
                        var ppos = this.LocatePoint(i, geom);
                        if(Vector2.Distance(ppos, relpos) <= this.ActivationZone) {
                            realpos = ppos;
                            point_id = i;
                        }
                    }
                }

                if(MouseDown) {
                    if(rect.Contains(pos)) {
                        if(!this.Editing) {
                            var renderer = this.DrawingLine.GetComponent<LineRenderer>();
                            renderer.SetPosition(0, (point_id == 0) ? relpos : realpos);

                            this.Editing = true;
                            this.DrawingStartPoint = point_id;
                        }
                    }
                } else if(MouseUp) {
                    if(this.Editing) {
                        if(rect.Contains(pos)) {
                            bool ok = point_id != 0 && this.DrawingStartPoint != 0;
                            if(ok) {
                                ok = this.GetLineStatus(point_id, this.DrawingStartPoint) == 0;
                            }
                            if(ok) {
                                var renderer = this.DrawingLine.GetComponent<LineRenderer>();
                                this.AddLine(point_id, this.DrawingStartPoint, realpos, renderer.GetPosition(0));
                                this.OnValueChange();
                            }
                        }
                        this.Editing = false;
                        this.DrawingLine.SetActive(false);
                    }
                } else if(this.Editing) {
                    if(rect.Contains(pos)) {
                        var renderer = this.DrawingLine.GetComponent<LineRenderer>();
                        bool ok = point_id != 0 && this.DrawingStartPoint != 0;
                        Color color = (ok && this.GetLineStatus(point_id, this.DrawingStartPoint) == 0) ? new Color(0.6f, 0.6f, 1.0f) : new Color(1.0f, 0.4f, 0.4f);
                        Gradient gradient = new Gradient();
                        gradient.SetKeys(
                            new GradientColorKey[] { new GradientColorKey(color, 0.0f) },
                            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f) }
                        );
                        renderer.colorGradient = gradient;
                        renderer.SetPosition(1, ok ? realpos : relpos);
                        this.DrawingLine.SetActive(true);
                    }
                }
            }
        }
    }
}
