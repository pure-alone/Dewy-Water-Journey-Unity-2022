using UnityEngine;
using UnityEngine.UI;

public sealed class DewyVisualPolish : MonoBehaviour
{
    private DewyApp app;
    private DewyApp.Page lastPage = (DewyApp.Page)(-1);
    private RectTransform lastStage;

    private Image[] scene5Checkpoints;
    private Image[] scene5Segments;
    private float[] scene5BaseWidths;
    private GameObject[] scene5Ripples;
    private Image scene5TributaryLeft;
    private Image scene5TributaryRight;

    private Image[] scene6Checkpoints;
    private Image[] scene6Segments;
    private float[] scene6BaseWidths;
    private Image scene6Waterfall;
    private Image scene6Lake;
    private Image scene6Ocean;

    private static Sprite upArrowSprite;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Object.FindObjectOfType<DewyVisualPolish>() != null) return;
        new GameObject("DewyVisualPolish").AddComponent<DewyVisualPolish>();
    }

    private void Update()
    {
        if (app == null) app = Object.FindObjectOfType<DewyApp>();
        if (app == null || app.Stage == null) return;

        if (lastStage != app.Stage || lastPage != app.CurrentPage)
        {
            lastStage = app.Stage;
            lastPage = app.CurrentPage;
            ResetPageReferences();
            ApplyPagePolish();
        }

        if (app.CurrentPage == DewyApp.Page.Scene5) UpdateScene5Progress();
        else if (app.CurrentPage == DewyApp.Page.Scene6) UpdateScene6Progress();
    }

    private void ResetPageReferences()
    {
        scene5Checkpoints = null;
        scene5Segments = null;
        scene5BaseWidths = null;
        scene5Ripples = null;
        scene5TributaryLeft = null;
        scene5TributaryRight = null;
        scene6Checkpoints = null;
        scene6Segments = null;
        scene6BaseWidths = null;
        scene6Waterfall = null;
        scene6Lake = null;
        scene6Ocean = null;
    }

    private void ApplyPagePolish()
    {
        switch (app.CurrentPage)
        {
            case DewyApp.Page.Home:
                FixHomeSunRays();
                break;
            case DewyApp.Page.Scene1:
                PolishScene1Sun();
                break;
            case DewyApp.Page.Scene4:
                AddGuidedPath("GroundwaterPath", new Vector2(224f, 162f), DewyPages.GROUNDWATER_CHECKPOINTS);
                break;
            case DewyApp.Page.Scene5:
                BuildMountainStreamScene();
                break;
            case DewyApp.Page.Scene6:
                BuildRiverToOceanScene();
                break;
        }
    }

    private void FixHomeSunRays()
    {
        RectTransform sun = app.Stage.Find("Sun") as RectTransform;
        if (sun == null) return;

        float sunWidth = sun.rect.width > 0f ? sun.rect.width : 92f;
        float sunHeight = sun.rect.height > 0f ? sun.rect.height : 92f;
        float centerX = sun.anchoredPosition.x + sunWidth * .5f;
        float centerY = -sun.anchoredPosition.y + sunHeight * .5f;
        float radius = Mathf.Max(sunWidth, sunHeight) * .72f;

        for (int i = 0; i < 12; i++)
        {
            RectTransform ray = app.Stage.Find("Ray" + i) as RectTransform;
            if (ray == null) continue;
            float angle = -90f + i * 30f;
            float radians = angle * Mathf.Deg2Rad;
            ray.anchorMin = ray.anchorMax = new Vector2(0f, 1f);
            ray.pivot = new Vector2(.5f, .5f);
            ray.sizeDelta = new Vector2(6f, 20f);
            ray.anchoredPosition = new Vector2(
                centerX + Mathf.Cos(radians) * radius,
                -(centerY + Mathf.Sin(radians) * radius));
            ray.localEulerAngles = new Vector3(0f, 0f, angle + 90f);
            Image image = ray.GetComponent<Image>();
            if (image != null) image.raycastTarget = false;
        }
    }

    private void PolishScene1Sun()
    {
        RectTransform sun = app.Stage.Find("Sun") as RectTransform;
        if (sun == null) return;

        DewyUI.Place(sun, 163f, 235f, 92f, 92f);

        Transform oldHint = sun.Find("SunDragHint");
        if (oldHint != null) oldHint.gameObject.SetActive(false);
        Transform oldGraphic = sun.Find("SunArrowGraphic");
        if (oldGraphic != null) oldGraphic.gameObject.SetActive(false);

        Image sunImage = sun.GetComponent<Image>();
        if (sunImage != null) sunImage.color = DewyUI.Hex("#FFC94F");

        Image innerGlow = DewyUI.Circle("SunInnerGlow", sun, new Color(1f, .92f, .62f, .34f), 10f, 10f, 72f);
        innerGlow.raycastTarget = false;

        Image arrow = DewyUI.Panel("SunArrow", sun, DewyUI.Hex("#94600B"), 25f, 18f, 42f, 54f);
        arrow.sprite = GetUpArrowSprite();
        arrow.type = Image.Type.Simple;
        arrow.preserveAspect = true;
        arrow.raycastTarget = false;
    }

    private static Sprite GetUpArrowSprite()
    {
        if (upArrowSprite != null) return upArrowSprite;
        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.name = "Dewy_UpArrow";
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool shaft = x >= 28 && x <= 35 && y >= 7 && y <= 37;
                float halfWidth = Mathf.Max(0f, (56f - y) * .72f);
                bool head = y >= 28 && y <= 56 && Mathf.Abs(x - 31.5f) <= halfWidth;
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, shaft || head ? 1f : 0f));
            }
        }

        texture.Apply();
        upArrowSprite = Sprite.Create(texture, new Rect(0,0,size,size), new Vector2(.5f,.5f), 100f, 0, SpriteMeshType.FullRect);
        upArrowSprite.name = "Dewy_UpArrow";
        return upArrowSprite;
    }

    private void BuildMountainStreamScene()
    {
        HideDirect("HillA", "HillB", "StreamMain", "TributaryA", "TributaryB", "SpringPool");
        HideEveryDirectNamed("SmallTrunk");
        HideEveryDirectNamed("SmallCrown");

        RectTransform root = CreateLayer("Scene5MountainStreamVisuals");
        root.SetAsFirstSibling();

        Image mountainBackdrop = DewyUI.Circle("MountainBackdrop", root, DewyUI.Hex("#A7D3A0"), -90, 85, 330);
        mountainBackdrop.rectTransform.sizeDelta = new Vector2(390, 240);
        Image mountainMid = DewyUI.Circle("MountainMid", root, DewyUI.Hex("#82BC78"), 125, 112, 360);
        mountainMid.rectTransform.sizeDelta = new Vector2(410, 280);
        DewyUI.Panel("ValleyFloor", root, DewyUI.Hex("#70B46A"), 0, 388, 418, 224, true).raycastTarget = false;

        Image springGlow = DewyUI.Circle("HillsideSpringGlow", root, new Color(.72f,.94f,1f,.55f), 42, 142, 104);
        springGlow.raycastTarget = false;
        Image spring = DewyUI.Circle("HillsideSpring", root, DewyUI.Hex("#73CCE9"), 54, 154, 80);
        spring.rectTransform.sizeDelta = new Vector2(92, 62);
        spring.raycastTarget = false;
        DewyUI.Label("SpringLabel", root, "HILLSIDE SPRING", 9, Color.white, 42, 211, 126, 22, TextAnchor.MiddleCenter, FontStyle.Bold);

        Vector2 start = new Vector2(96f, 186f);
        Vector2[] route = BuildRoute(start, DewyPages.STREAM_CHECKPOINTS);
        Color[] water = {
            DewyUI.Hex("#7BD4EF"), DewyUI.Hex("#6AC8E9"),
            DewyUI.Hex("#59BCE0"), DewyUI.Hex("#48AED5")
        };
        scene5BaseWidths = new [] { 24f, 31f, 39f, 50f };
        scene5Segments = new Image[4];
        for (int i=0; i<4; i++)
        {
            scene5Segments[i] = AddWaterSegment(root, "StreamSegment" + i, route[i], route[i+1], scene5BaseWidths[i], water[i]);
            scene5Segments[i].raycastTarget = false;
        }

        scene5TributaryLeft = AddWaterSegment(root, "TributaryLeft", new Vector2(26, 326), route[2], 14f, new Color(.38f,.75f,.89f,.30f));
        scene5TributaryRight = AddWaterSegment(root, "TributaryRight", new Vector2(392, 365), route[3], 16f, new Color(.38f,.75f,.89f,.30f));
        scene5TributaryLeft.raycastTarget = scene5TributaryRight.raycastTarget = false;

        AddSmallTree(root, 28, 255, .76f); AddSmallTree(root, 334, 242, .72f);
        AddSmallTree(root, 55, 468, .72f); AddSmallTree(root, 342, 474, .70f);
        DewyUI.Circle("ShrubA", root, DewyUI.Hex("#4F9D58"), 210, 292, 28).raycastTarget = false;
        DewyUI.Circle("ShrubB", root, DewyUI.Hex("#5CA864"), 300, 430, 34).raycastTarget = false;

        scene5Ripples = new GameObject[4];
        for (int i=0; i<4; i++)
        {
            Vector2 p = route[i+1];
            Image ripple = DewyUI.Panel("StreamRipple" + i, root, new Color(1,1,1,.72f), p.x-22, p.y+22, 44, 4, true);
            ripple.raycastTarget = false;
            scene5Ripples[i] = ripple.gameObject;
            scene5Ripples[i].SetActive(false);
        }

        AddDottedPath(root, "StreamGuidePath", route, new Color(1f,1f,1f,.72f));
        scene5Checkpoints = FindCheckpoints(DewyPages.STREAM_CHECKPOINTS.Length);
        UpdateScene5Progress();
    }

    private void UpdateScene5Progress()
    {
        if (scene5Checkpoints == null) return;
        int completed = CountCompleted(scene5Checkpoints);
        HighlightNext(scene5Checkpoints, completed);

        for (int i=0; i<scene5Segments.Length; i++)
        {
            float width = scene5BaseWidths[i] * (i < completed ? 1.18f : 1f);
            RectTransform rt = scene5Segments[i].rectTransform;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, width);
            scene5Ripples[i].SetActive(i < completed);
        }

        if (scene5TributaryLeft != null)
            scene5TributaryLeft.color = completed >= 2 ? DewyUI.Hex("#67C5E5") : new Color(.38f,.75f,.89f,.30f);
        if (scene5TributaryRight != null)
            scene5TributaryRight.color = completed >= 3 ? DewyUI.Hex("#67C5E5") : new Color(.38f,.75f,.89f,.30f);
    }

    private void BuildRiverToOceanScene()
    {
        HideDirect("Land", "River", "Waterfall", "Lake", "OceanArea");

        RectTransform root = CreateLayer("Scene6RiverToOceanVisuals");
        root.SetAsFirstSibling();

        DewyUI.Panel("RiverLand", root, DewyUI.Hex("#A7D785"), 0, 112, 418, 500, true).raycastTarget = false;
        Image hillA = DewyUI.Circle("RiverDistantHill", root, DewyUI.Hex("#86BF76"), -70, 105, 260);
        hillA.rectTransform.sizeDelta = new Vector2(315, 190);
        Image hillB = DewyUI.Circle("RiverDistantHill2", root, DewyUI.Hex("#74AF68"), 235, 116, 250);
        hillB.rectTransform.sizeDelta = new Vector2(300, 180);
        hillA.raycastTarget = hillB.raycastTarget = false;

        Vector2 start = new Vector2(88f, 170f);
        Vector2[] route = BuildRoute(start, DewyPages.RIVER_CHECKPOINTS);
        scene6BaseWidths = new [] { 38f, 46f, 58f, 72f };
        scene6Segments = new Image[4];
        scene6Segments[0] = AddWaterSegment(root, "RiverBend0", route[0], route[1], scene6BaseWidths[0], DewyUI.Hex("#65C6E7"));
        scene6Segments[1] = AddWaterSegment(root, "RiverBend1", route[1], new Vector2(148,300), scene6BaseWidths[1], DewyUI.Hex("#59BDE0"));
        scene6Segments[2] = AddWaterSegment(root, "RiverBend2", route[2], route[3], scene6BaseWidths[2], DewyUI.Hex("#55B8DC"));
        scene6Segments[3] = AddWaterSegment(root, "RiverOutlet", route[3], route[4], scene6BaseWidths[3], DewyUI.Hex("#49AED5"));
        foreach (Image segment in scene6Segments) segment.raycastTarget = false;

        scene6Waterfall = DewyUI.Panel("WaterfallCurtain", root, DewyUI.Hex("#9DE6FA"), 119, 286, 58, 102, true);
        scene6Waterfall.raycastTarget = false;
        for (int i=0; i<4; i++)
        {
            Image foam = DewyUI.Circle("WaterfallFoam" + i, root, new Color(1,1,1,.84f), 105+i*22, 368+(i%2)*6, 30);
            foam.raycastTarget = false;
        }

        scene6Lake = DewyUI.Circle("CalmLake", root, DewyUI.Hex("#76CBE7"), 170, 366, 170);
        scene6Lake.rectTransform.sizeDelta = new Vector2(196,108);
        scene6Lake.raycastTarget = false;
        DewyUI.Panel("LakeRippleA", root, new Color(1,1,1,.55f), 208, 405, 82, 4, true).raycastTarget = false;
        DewyUI.Panel("LakeRippleB", root, new Color(1,1,1,.42f), 230, 424, 70, 4, true).raycastTarget = false;

        scene6Ocean = DewyUI.Panel("OceanHorizon", root, DewyUI.Hex("#3FA8D0"), 258, 458, 190, 154, true);
        scene6Ocean.raycastTarget = false;
        for (int i=0; i<4; i++)
            DewyUI.Circle("OceanWave" + i, root, new Color(1,1,1,.48f), 258+i*48, 445, 42).raycastTarget = false;
        DewyUI.Label("OceanLabelPolished", root, "OCEAN", 12, Color.white, 320, 535, 82, 28, TextAnchor.MiddleCenter, FontStyle.Bold);

        AddSmallTree(root, 32, 455, .72f); AddSmallTree(root, 330, 240, .72f); AddSmallTree(root, 282, 318, .68f);
        AddDottedPath(root, "RiverGuidePath", route, new Color(1f,1f,1f,.72f));

        scene6Checkpoints = FindCheckpoints(DewyPages.RIVER_CHECKPOINTS.Length);
        UpdateScene6Progress();
    }

    private void UpdateScene6Progress()
    {
        if (scene6Checkpoints == null) return;
        int completed = CountCompleted(scene6Checkpoints);
        HighlightNext(scene6Checkpoints, completed);

        for (int i=0; i<scene6Segments.Length; i++)
        {
            float width = scene6BaseWidths[i] * (i < completed ? 1.12f : 1f);
            RectTransform rt = scene6Segments[i].rectTransform;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, width);
        }

        if (scene6Waterfall != null)
            scene6Waterfall.color = completed >= 2 ? DewyUI.Hex("#B8EEFC") : DewyUI.Hex("#8DD8EF");
        if (scene6Lake != null)
            scene6Lake.color = completed >= 3 ? DewyUI.Hex("#83D4ED") : DewyUI.Hex("#70C2DE");
        if (scene6Ocean != null)
            scene6Ocean.color = completed >= 4 ? DewyUI.Hex("#2F9FCB") : DewyUI.Hex("#3FA8D0");
    }

    private Image[] FindCheckpoints(int count)
    {
        Image[] points = new Image[count];
        for (int i=0; i<count; i++)
        {
            Transform t = app.Stage.Find("Checkpoint" + i);
            points[i] = t == null ? null : t.GetComponent<Image>();
        }
        return points;
    }

    private static int CountCompleted(Image[] points)
    {
        Color green = DewyUI.Hex("#66CF7A");
        int completed = 0;
        for (int i=0; i<points.Length; i++)
        {
            if (points[i] == null) break;
            if (ColorDistance(points[i].color, green) < .08f) completed++;
            else break;
        }
        return completed;
    }

    private static void HighlightNext(Image[] points, int completed)
    {
        for (int i=0; i<points.Length; i++)
        {
            if (points[i] == null) continue;
            if (i < completed) points[i].color = DewyUI.Hex("#66CF7A");
            else if (i == completed) points[i].color = DewyUI.Hex("#FFC94F");
            else points[i].color = new Color(.14f,.56f,.74f,.48f);
        }
    }

    private static float ColorDistance(Color a, Color b)
    {
        return Mathf.Abs(a.r-b.r) + Mathf.Abs(a.g-b.g) + Mathf.Abs(a.b-b.b);
    }

    private RectTransform CreateLayer(string name)
    {
        RectTransform root = DewyUI.MakeRect(name, app.Stage);
        DewyUI.Stretch(root);
        return root;
    }

    private void HideDirect(params string[] names)
    {
        foreach (string name in names)
        {
            Transform t = app.Stage.Find(name);
            if (t != null) t.gameObject.SetActive(false);
        }
    }

    private void HideEveryDirectNamed(string targetName)
    {
        for (int i=0; i<app.Stage.childCount; i++)
        {
            Transform child = app.Stage.GetChild(i);
            if (child.name == targetName) child.gameObject.SetActive(false);
        }
    }

    private static Vector2[] BuildRoute(Vector2 start, Vector2[] checkpoints)
    {
        Vector2[] route = new Vector2[checkpoints.Length + 1];
        route[0] = start;
        for (int i=0; i<checkpoints.Length; i++) route[i+1] = checkpoints[i] + new Vector2(14f,14f);
        return route;
    }

    private static Image AddWaterSegment(Transform parent, string name, Vector2 from, Vector2 to, float width, Color color)
    {
        Vector2 localDelta = new Vector2(to.x-from.x, -(to.y-from.y));
        float length = localDelta.magnitude;
        Vector2 mid = (from + to) * .5f;
        RectTransform rt = DewyUI.MakeRect(name, parent);
        rt.anchorMin = rt.anchorMax = new Vector2(0f,1f);
        rt.pivot = new Vector2(.5f,.5f);
        rt.anchoredPosition = new Vector2(mid.x, -mid.y);
        rt.sizeDelta = new Vector2(length, width);
        rt.localEulerAngles = new Vector3(0,0,Mathf.Atan2(localDelta.y,localDelta.x)*Mathf.Rad2Deg);
        Image image = rt.gameObject.AddComponent<Image>();
        image.color = color;
        image.sprite = DewyUI.RoundedSprite();
        image.type = Image.Type.Sliced;
        return image;
    }

    private static void AddDottedPath(Transform parent, string name, Vector2[] route, Color color)
    {
        RectTransform root = DewyUI.MakeRect(name, parent);
        DewyUI.Stretch(root);
        const float spacing = 18f;
        const float dotSize = 7f;
        for (int segment=0; segment<route.Length-1; segment++)
        {
            Vector2 a = route[segment];
            Vector2 b = route[segment+1];
            float distance = Vector2.Distance(a,b);
            int steps = Mathf.Max(2, Mathf.FloorToInt(distance/spacing));
            for (int step=1; step<steps; step++)
            {
                Vector2 p = Vector2.Lerp(a,b,step/(float)steps);
                Image dot = DewyUI.Circle("PathDot_"+segment+"_"+step, root, color, p.x-dotSize*.5f, p.y-dotSize*.5f, dotSize);
                dot.raycastTarget = false;
            }
        }
    }

    private void AddGuidedPath(string name, Vector2 startCenter, Vector2[] checkpoints)
    {
        if (app.Stage.Find(name) != null || checkpoints == null || checkpoints.Length == 0) return;
        Vector2[] route = BuildRoute(startCenter, checkpoints);
        RectTransform root = CreateLayer(name);
        Transform firstCheckpoint = app.Stage.Find("Checkpoint0");
        if (firstCheckpoint != null) root.SetSiblingIndex(firstCheckpoint.GetSiblingIndex());
        AddDottedPath(root, "GroundwaterGuideDots", route, new Color(1f,1f,1f,.68f));
    }

    private static void AddSmallTree(Transform parent, float x, float y, float scale)
    {
        DewyUI.Panel("TreeTrunk", parent, DewyUI.Hex("#7B654A"), x+18*scale, y+40*scale, 8*scale, 44*scale, true).raycastTarget = false;
        DewyUI.Circle("TreeCrown", parent, DewyUI.Hex("#55A95C"), x, y, 46*scale).raycastTarget = false;
    }
}
