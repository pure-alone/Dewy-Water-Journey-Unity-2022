using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class DewyPages
{
    public const float SUN_COMPLETION_Y = 125f;
    public static readonly Vector2[] GROUNDWATER_CHECKPOINTS = { new Vector2(188, 300), new Vector2(121, 455), new Vector2(242, 455) };
    public static readonly Vector2[] STREAM_CHECKPOINTS = { new Vector2(125, 250), new Vector2(163, 335), new Vector2(238, 415), new Vector2(340, 505) };
    public static readonly Vector2[] RIVER_CHECKPOINTS = { new Vector2(160, 230), new Vector2(126, 365), new Vector2(262, 405), new Vector2(346, 505) };

    public static void Build(DewyApp app, DewyApp.Page page)
    {
        switch (page)
        {
            case DewyApp.Page.Home: BuildHome(app); break;
            case DewyApp.Page.Scene1: BuildScene1(app); break;
            case DewyApp.Page.Scene2: BuildScene2(app); break;
            case DewyApp.Page.Scene3: BuildScene3(app); break;
            case DewyApp.Page.Scene4: BuildScene4(app); break;
            case DewyApp.Page.Scene5: BuildScene5(app); break;
            case DewyApp.Page.Scene6: BuildScene6(app); break;
            case DewyApp.Page.Credits: BuildCredits(app); break;
        }
    }

    private static void BuildHome(DewyApp app)
    {
        RectTransform stage = app.Stage;
        stage.GetComponent<Image>().color = DewyUI.Hex("#BFEFFF");
        DewyUI.Panel("Ocean", stage, DewyUI.Hex("#64C1DF"), 0, 315, 418, 297);
        for (int i=0;i<7;i++) DewyUI.Circle("Wave"+i, stage, new Color(1,1,1,.75f), -8+i*68, 298, 52).raycastTarget = false;
        AddSun(stage, 300, 65, 82, true);
        DewyUI.Label("HeroTitle", stage, DewyContent.HomeHero, 20, DewyUI.Ink, 26, 150, 366, 62, TextAnchor.MiddleCenter, FontStyle.Bold);
        DewyUI.Label("Tagline", stage, DewyContent.HomeTagline, 13, DewyUI.Hex("#2E6074"), 24, 220, 370, 34, TextAnchor.MiddleCenter, FontStyle.Bold);
        Image dewy = DewyUI.Dewy("HomeDewy", stage, 173, 280, 72);
        dewy.gameObject.AddComponent<DewyBob>();

        Button start = DewyUI.Button("StartJourney", stage, "Start Journey", 79, 490, 260, 64, DewyUI.Sun, DewyUI.Hex("#173C4B"), 17);
        start.onClick.AddListener(() => { app.Audio.PlaySfx("drop", .46f); app.Toast("Let the journey begin!"); app.ShowPage(DewyApp.Page.Scene1); });
        Button credits = DewyUI.Button("CreditsLink", stage, "Credits", 320, 566, 78, 30, new Color(1,1,1,0f), DewyUI.Hex("#1E6C8D"), 12);
        credits.onClick.AddListener(() => { app.Audio.PlaySfx("ui", .28f); app.ShowPage(DewyApp.Page.Credits); });
    }

    private static void BuildScene1(DewyApp app)
    {
        RectTransform stage = app.Stage;
        stage.GetComponent<Image>().color = DewyUI.Hex("#BCEcff");
        DewyUI.Panel("Ocean", stage, DewyUI.Hex("#54B8D8"), 0, 355, 418, 257);
        AddInstructionLearning(app, DewyContent.Scene1);

        Image sun = AddSun(stage, 163, 278, 92, false);
        DewyUI.Label("SunDragHint", sun.transform, "↕", 28, DewyUI.Hex("#9B6811"), 0, 0, 92, 92, TextAnchor.MiddleCenter, FontStyle.Bold);
        Image dewy = DewyUI.Dewy("Scene1Dewy", stage, 173, 430, 72);

        List<GameObject> vapours = new List<GameObject>();
        for (int i=0;i<3;i++)
        {
            Text v = DewyUI.Label("Vapour"+i, stage, "≈", 38, new Color(1,1,1,.82f), 125+i*55, 335, 40, 90, TextAnchor.MiddleCenter, FontStyle.Bold);
            v.gameObject.SetActive(false); vapours.Add(v.gameObject);
        }

        bool done = false;
        DewyDragHandler drag = sun.gameObject.AddComponent<DewyDragHandler>();
        drag.Dragged = e =>
        {
            DragTo(stage, sun.rectTransform, e, 135, 82, 4, 330);
            float y = Top(sun.rectTransform);
            if (!done && y < SUN_COMPLETION_Y)
            {
                done = true;
                foreach (GameObject v in vapours) v.SetActive(true);
                dewy.rectTransform.anchoredPosition = new Vector2(173, -278);
                app.Audio.PlaySfx("evaporation", .5f);
                app.SetSceneComplete(true);
                app.Toast("Evaporation unlocked");
            }
        };
        drag.Ended = _ => { if (!done) app.Toast("Try dragging the sun higher"); };
    }

    private static void BuildScene2(DewyApp app)
    {
        RectTransform stage = app.Stage;
        stage.GetComponent<Image>().color = DewyUI.Hex("#D9F5FF");
        AddInstructionLearning(app, DewyContent.Scene2);
        RectTransform cloud = DewyUI.Cloud("CloudTarget", stage, 74, 130, 270, 150, Color.white);
        DewyUI.Label("GatherHint", cloud, "Gather here", 13, DewyUI.Hex("#7F9FAB"), 40, 48, 190, 38, TextAnchor.MiddleCenter, FontStyle.Bold);
        Text progress = DewyUI.Label("CloudProgress", stage, "0 / 4 droplets gathered", 13, DewyUI.Hex("#46829A"), 100, 335, 218, 30, TextAnchor.MiddleCenter, FontStyle.Bold);

        Vector2[] starts = { new Vector2(45,365), new Vector2(297,375), new Vector2(105,475), new Vector2(255,485) };
        Vector2[] snaps = { new Vector2(145,190), new Vector2(190,160), new Vector2(225,195), new Vector2(175,210) };
        bool[] gathered = new bool[4];
        int dropletsGathered = 0;
        for (int i=0;i<4;i++)
        {
            int index = i;
            Image drop = MiniDrop(stage, "Droplet" + i, starts[i].x, starts[i].y, 54);
            DewyDragHandler drag = drop.gameObject.AddComponent<DewyDragHandler>();
            drag.Dragged = e => { if (!gathered[index]) DragTo(stage, drop.rectTransform, e, 54, 54, 75, 540); };
            drag.Ended = _ =>
            {
                if (gathered[index]) return;
                Vector2 c = Center(drop.rectTransform);
                if (c.x >= 74 && c.x <= 344 && c.y >= 130 && c.y <= 280)
                {
                    gathered[index] = true;
                    dropletsGathered++;
                    drop.rectTransform.anchoredPosition = new Vector2(snaps[index].x, -snaps[index].y);
                    drop.rectTransform.localScale = Vector3.one * .58f;
                    app.Audio.PlaySfx("bubble", .44f);
                    progress.text = dropletsGathered + " / 4 droplets gathered";
                    if (dropletsGathered == 4)
                    {
                        cloud.localScale = Vector3.one * 1.06f;
                        app.Audio.PlaySfx("sparkle", .46f);
                        app.SetSceneComplete(true);
                        app.Toast("Condensation complete");
                    }
                }
                else app.Toast("Drop it inside the cloud");
            };
        }
    }

    private static void BuildScene3(DewyApp app)
    {
        RectTransform stage = app.Stage;
        stage.GetComponent<Image>().color = DewyUI.Hex("#A6D9E7");
        Image ground = DewyUI.Panel("ForestGround", stage, DewyUI.Hex("#CAA276"), 0, 343, 418, 269);
        AddInstructionLearning(app, DewyContent.Scene3);
        RectTransform cloud = DewyUI.Cloud("RainCloud", stage, 86, 110, 245, 118, DewyUI.Hex("#657885"));
        Image hit = cloud.gameObject.AddComponent<Image>(); hit.color = new Color(1,1,1,0f);
        Button cloudButton = cloud.gameObject.AddComponent<Button>(); cloudButton.targetGraphic = hit;
        Text counter = DewyUI.Label("TapCounter", stage, "4 taps to help the forest", 12, DewyUI.Hex("#4B6470"), 102, 265, 214, 34, TextAnchor.MiddleCenter, FontStyle.Bold);

        List<Image> crowns = new List<Image>();
        crowns.Add(AddTree(stage, 42, 400, 1f));
        crowns.Add(AddTree(stage, 165, 378, 1.15f));
        crowns.Add(AddTree(stage, 310, 400, 1f));
        Image plant1 = AddPlant(stage, 108, 514); Image plant2 = AddPlant(stage, 255, 514);

        int rainTaps = 0;
        cloudButton.onClick.AddListener(() =>
        {
            if (rainTaps >= 4) return;
            rainTaps++;
            app.Audio.PlaySfx("rain", .35f);
            SpawnRain(stage, 24);
            float t = rainTaps / 4f;
            Color healed = Color.Lerp(DewyUI.Hex("#9D9676"), DewyUI.Hex("#3EA75B"), t);
            foreach (Image crown in crowns) crown.color = healed;
            plant1.color = healed; plant2.color = healed;
            ground.color = Color.Lerp(DewyUI.Hex("#CAA276"), DewyUI.Hex("#79BE69"), t);
            int remaining = 4 - rainTaps;
            counter.text = remaining > 0 ? remaining + " more tap" + (remaining == 1 ? "" : "s") + " to help the forest" : "The forest is refreshed!";
            if (rainTaps >= 4)
            {
                app.Audio.PlaySfx("forest", .44f);
                app.SetSceneComplete(true);
                app.Toast("Precipitation complete");
            }
        });
    }

    private static void BuildScene4(DewyApp app)
    {
        RectTransform stage = app.Stage;
        stage.GetComponent<Image>().color = DewyUI.Hex("#BFEAFF");
        DewyUI.Panel("Grass", stage, DewyUI.Hex("#8CCC7C"), 0, 110, 418, 44);
        DewyUI.Panel("Soil", stage, DewyUI.Earth, 0, 154, 418, 275);
        DewyUI.Panel("WaterTable", stage, DewyUI.Hex("#8CC8DC"), 0, 429, 418, 183);
        AddInstructionLearning(app, DewyContent.Scene4);
        AddRock(stage, 68, 235, 82, 52, 11); AddRock(stage, 288, 335, 82, 52, -18); AddRock(stage, 138, 412, 82, 52, 8);
        Text status = Status(stage, "Move down through the topsoil");
        string[] labels = { "Move down through the topsoil", "Travel around the rocks", "Reach the groundwater layer" };
        Image[] points = MakeCheckpoints(stage, GROUNDWATER_CHECKPOINTS);
        Image dewy = DewyUI.Dewy("GuideDewy", stage, 188, 126, 72);
        int checkpointIndex = 0;
        DewyDragHandler drag = dewy.gameObject.AddComponent<DewyDragHandler>();
        drag.Dragged = e =>
        {
            DragTo(stage, dewy.rectTransform, e, 72, 72, 82, 525);
            if (checkpointIndex < GROUNDWATER_CHECKPOINTS.Length && DistanceTo(dewy.rectTransform, GROUNDWATER_CHECKPOINTS[checkpointIndex]) < 72f)
            {
                points[checkpointIndex].color = DewyUI.Hex("#66CF7A");
                app.Audio.PlaySfx("drip", .38f);
                checkpointIndex++;
                status.text = checkpointIndex < labels.Length ? labels[checkpointIndex] : "Groundwater reached!";
                app.Toast(checkpointIndex < points.Length ? "Good route!" : "Groundwater reached!");
                if (checkpointIndex == GROUNDWATER_CHECKPOINTS.Length)
                {
                    app.Audio.PlaySfx("sparkle", .4f);
                    app.SetSceneComplete(true);
                    app.Toast("Infiltration complete");
                }
            }
        };
        drag.Ended = _ => { if (checkpointIndex < labels.Length) app.Toast(labels[checkpointIndex]); };
    }

    private static void BuildScene5(DewyApp app)
    {
        RectTransform stage = app.Stage;
        stage.GetComponent<Image>().color = DewyUI.Hex("#BDEAFD");
        DewyUI.Circle("HillA", stage, DewyUI.Hex("#7FBA6D"), -80, 118, 330);
        DewyUI.Circle("HillB", stage, DewyUI.Hex("#68A95F"), 170, 150, 360);
        Image stream = DewyUI.Panel("StreamMain", stage, DewyUI.Hex("#55BCE0"), 115, 170, 72, 420, true); stream.rectTransform.localEulerAngles = new Vector3(0,0,-18);
        Image trA = DewyUI.Panel("TributaryA", stage, new Color(.42f,.77f,.89f,.32f), 22, 330, 205, 20, true); trA.rectTransform.localEulerAngles = new Vector3(0,0,-16);
        Image trB = DewyUI.Panel("TributaryB", stage, new Color(.42f,.77f,.89f,.32f), 230, 355, 175, 20, true); trB.rectTransform.localEulerAngles = new Vector3(0,0,20);
        DewyUI.Circle("SpringPool", stage, DewyUI.Hex("#79CCE7"), 52, 145, 86);
        AddInstructionLearning(app, DewyContent.Scene5);
        AddSmallTree(stage, 32, 505); AddSmallTree(stage, 340, 470); AddSmallTree(stage, 285, 245);
        Text status = Status(stage, "Leave the hillside spring");
        string[] labels = { "Leave the hillside spring", "Follow the downhill stream", "Collect the joining runoff", "Reach the valley river" };
        Image[] points = MakeCheckpoints(stage, STREAM_CHECKPOINTS);
        Image dewy = DewyUI.Dewy("StreamGuide", stage, 60, 132, 72);
        int checkpointIndex = 0;
        DewyDragHandler drag = dewy.gameObject.AddComponent<DewyDragHandler>();
        drag.Dragged = e =>
        {
            DragTo(stage, dewy.rectTransform, e, 72, 72, 82, 525);
            if (checkpointIndex < STREAM_CHECKPOINTS.Length && DistanceTo(dewy.rectTransform, STREAM_CHECKPOINTS[checkpointIndex]) < 76f)
            {
                points[checkpointIndex].color = DewyUI.Hex("#66CF7A");
                app.Audio.PlaySfx("splash", .36f);
                checkpointIndex++;
                if (checkpointIndex >= 2) trA.color = DewyUI.Hex("#6BC5E4");
                if (checkpointIndex >= 3) trB.color = DewyUI.Hex("#6BC5E4");
                status.text = checkpointIndex < labels.Length ? labels[checkpointIndex] : "Runoff collected into a larger stream!";
                app.Toast(checkpointIndex < points.Length ? "The stream is growing!" : "Runoff & collection complete");
                if (checkpointIndex == STREAM_CHECKPOINTS.Length)
                {
                    stream.rectTransform.localScale = new Vector3(1.08f,1.02f,1f);
                    app.Audio.PlaySfx("sparkle", .4f);
                    app.SetSceneComplete(true);
                    app.Toast("Runoff & Collection unlocked");
                }
            }
        };
        drag.Ended = _ => { if (checkpointIndex < labels.Length) app.Toast(labels[checkpointIndex]); };
    }

    private static void BuildScene6(DewyApp app)
    {
        RectTransform stage = app.Stage;
        stage.GetComponent<Image>().color = DewyUI.Hex("#C9EFFD");
        DewyUI.Panel("Land", stage, DewyUI.Hex("#9FD77D"), 0, 146, 418, 466);
        Image river = DewyUI.Panel("River", stage, DewyUI.Hex("#59BFE4"), 86, 145, 126, 410, true); river.rectTransform.localEulerAngles = new Vector3(0,0,-14);
        Image waterfall = DewyUI.Panel("Waterfall", stage, DewyUI.Hex("#9DE0F6"), 180, 245, 58, 116, true);
        DewyUI.Circle("Lake", stage, DewyUI.Hex("#76CBE7"), 120, 428, 180);
        Image ocean = DewyUI.Panel("OceanArea", stage, DewyUI.Hex("#3FA8D0"), 275, 468, 170, 160, true);
        DewyUI.Label("OceanLabel", ocean.transform, "OCEAN", 13, Color.white, 0, 42, 145, 38, TextAnchor.MiddleCenter, FontStyle.Bold);
        AddInstructionLearning(app, DewyContent.Scene6);
        Text status = Status(stage, "Follow the river bend");
        string[] labels = { "Follow the river bend", "Pass the waterfall", "Cross the lake", "Return to the ocean" };
        Image[] points = MakeCheckpoints(stage, RIVER_CHECKPOINTS);
        Image dewy = DewyUI.Dewy("RiverGuide", stage, 52, 132, 72);
        GameObject ending = BuildEndingCard(stage);
        ending.SetActive(false);
        int checkpointIndex = 0;
        DewyDragHandler drag = dewy.gameObject.AddComponent<DewyDragHandler>();
        drag.Dragged = e =>
        {
            DragTo(stage, dewy.rectTransform, e, 72, 72, 82, 525);
            if (checkpointIndex < RIVER_CHECKPOINTS.Length && DistanceTo(dewy.rectTransform, RIVER_CHECKPOINTS[checkpointIndex]) < 74f)
            {
                points[checkpointIndex].color = DewyUI.Hex("#66CF7A");
                app.Audio.PlaySfx("splash", .36f);
                checkpointIndex++;
                status.text = checkpointIndex < labels.Length ? labels[checkpointIndex] : "Journey complete!";
                app.Toast(checkpointIndex < points.Length ? "Keep following the water!" : "Back to the ocean!");
                if (checkpointIndex == RIVER_CHECKPOINTS.Length)
                {
                    app.Audio.PlaySfx("complete", .5f);
                    ending.SetActive(true);
                    app.SetSceneComplete(true);
                }
            }
        };
        drag.Ended = _ => { if (checkpointIndex < labels.Length) app.Toast(labels[checkpointIndex]); };
    }

    private static void BuildCredits(DewyApp app)
    {
        RectTransform stage = app.Stage;
        stage.GetComponent<Image>().color = DewyUI.Hex("#BFEFFF");
        DewyUI.Panel("Ocean", stage, DewyUI.Hex("#61BDDC"), 0, 318, 418, 294);
        Image dewy = DewyUI.Dewy("CreditsDewy", stage, 173, 34, 72);
        dewy.gameObject.AddComponent<DewyBob>();
        Image card = DewyUI.Panel("CreditsCard", stage, new Color(1,1,1,.94f), 25, 115, 368, 430, true);
        DewyUI.Label("CreditsTitle", card.transform, DewyContent.CreditsTitle, 20, DewyUI.Ink, 18, 18, 332, 54, TextAnchor.MiddleCenter, FontStyle.Bold);
        DewyUI.Label("Assessment", card.transform, DewyContent.CreditsAssessment, 13, DewyUI.Muted, 22, 76, 324, 58, TextAnchor.MiddleCenter);
        DewyUI.Label("Creator", card.transform, DewyContent.CreditsCreator, 14, DewyUI.Ink, 22, 139, 324, 28, TextAnchor.MiddleCenter, FontStyle.Bold);
        DewyUI.Label("Message", card.transform, DewyContent.CreditsQuote, 14, DewyUI.GreenDeep, 22, 174, 324, 60, TextAnchor.MiddleCenter, FontStyle.Bold);
        DewyUI.Label("Detail", card.transform, DewyContent.CreditsDetail, 12, DewyUI.Muted, 22, 242, 324, 88, TextAnchor.MiddleCenter);
        Button restart = DewyUI.Button("RestartJourney", card.transform, "Restart Journey", 64, 347, 240, 56, DewyUI.Sun, DewyUI.Hex("#173C4B"), 16);
        restart.onClick.AddListener(() => { app.Audio.PlaySfx("drop", .42f); app.ShowPage(DewyApp.Page.Home); });
    }

    private static void AddInstructionLearning(DewyApp app, DewyContent.PageCopy copy)
    {
        DewyUI.Instruction(app.Stage, copy.Instruction);
        Text learning = DewyUI.LearningLabel(app.Stage, copy.Learning);
        app.RegisterLearningCard(learning.transform.parent.gameObject);
    }

    private static Image AddSun(Transform parent, float x, float y, float size, bool rays)
    {
        if (rays)
        {
            for (int i=0;i<12;i++)
            {
                Image ray = DewyUI.Panel("Ray"+i, parent, DewyUI.Hex("#F7AD27"), x+size/2-3, y-13, 6, 22, true);
                ray.rectTransform.pivot = new Vector2(.5f, 3.1f);
                ray.rectTransform.localEulerAngles = new Vector3(0,0,i*30f);
                ray.raycastTarget = false;
            }
        }
        Image sun = DewyUI.Circle("Sun", parent, DewyUI.Sun, x, y, size);
        sun.raycastTarget = true;
        return sun;
    }

    private static Image MiniDrop(Transform parent, string name, float x, float y, float size)
    {
        Image d = DewyUI.Panel(name, parent, DewyUI.Hex("#55C4EB"), x, y, size, size, true);
        d.rectTransform.localEulerAngles = new Vector3(0,0,45f);
        return d;
    }

    private static Image AddTree(Transform stage, float x, float y, float scale)
    {
        RectTransform root = DewyUI.MakeRect("Tree", stage); DewyUI.Place(root, x, y, 72*scale, 180*scale);
        DewyUI.Panel("Trunk", root, DewyUI.Hex("#8B6548"), 29*scale, 85*scale, 17*scale, 85*scale, true).raycastTarget = false;
        Image crown = DewyUI.Circle("Crown", root, DewyUI.Hex("#9D9676"), 0, 0, 72*scale); crown.rectTransform.sizeDelta = new Vector2(72*scale,96*scale); crown.raycastTarget = false;
        return crown;
    }

    private static Image AddPlant(Transform stage, float x, float y)
    {
        Image a = DewyUI.Circle("Plant", stage, DewyUI.Hex("#938D70"), x, y, 34); a.rectTransform.sizeDelta = new Vector2(28,48); a.rectTransform.localEulerAngles = new Vector3(0,0,-24);
        return a;
    }

    private static void SpawnRain(RectTransform stage, int count)
    {
        for (int i=0;i<count;i++)
        {
            float x = 30 + UnityEngine.Random.value * 358f;
            Image drop = DewyUI.Panel("RainDrop", stage, DewyUI.Hex("#3BA8DB"), x, 215-UnityEngine.Random.value*80f, 4, 22, true);
            DewyRainDrop mover = drop.gameObject.AddComponent<DewyRainDrop>(); mover.Speed = 340f + UnityEngine.Random.value*100f;
        }
    }

    private static void AddRock(Transform stage, float x, float y, float w, float h, float angle)
    {
        Image r = DewyUI.Panel("Rock", stage, DewyUI.Hex("#765F54"), x, y, w, h, true); r.rectTransform.localEulerAngles = new Vector3(0,0,angle); r.raycastTarget = false;
    }

    private static void AddSmallTree(Transform stage, float x, float y)
    {
        DewyUI.Panel("SmallTrunk", stage, DewyUI.Hex("#7B654A"), x+18, y+40, 8, 44, true).raycastTarget = false;
        DewyUI.Circle("SmallCrown", stage, DewyUI.Hex("#55A95C"), x, y, 46).raycastTarget = false;
    }

    private static Text Status(Transform stage, string value)
    {
        Image bg = DewyUI.Panel("Status", stage, new Color(1,1,1,.92f), 18, 552, 382, 42, true);
        return DewyUI.Label("StatusText", bg.transform, value, 12, DewyUI.Hex("#47606B"), 8, 0, 366, 42, TextAnchor.MiddleCenter, FontStyle.Bold);
    }

    private static Image[] MakeCheckpoints(Transform stage, Vector2[] points)
    {
        Image[] result = new Image[points.Length];
        for (int i=0;i<points.Length;i++)
        {
            result[i] = DewyUI.Circle("Checkpoint"+i, stage, new Color(.14f,.56f,.74f,.65f), points[i].x, points[i].y, 28);
            Outline outline = result[i].gameObject.AddComponent<Outline>(); outline.effectColor = Color.white; outline.effectDistance = new Vector2(2,-2);
            result[i].raycastTarget = false;
        }
        return result;
    }

    private static GameObject BuildEndingCard(Transform stage)
    {
        Image card = DewyUI.Panel("EndingCard", stage, new Color(1,1,1,.96f), 24, 205, 370, 205, true);
        DewyUI.Label("EndTitle", card.transform, DewyContent.EndingTitle, 18, DewyUI.Ink, 20, 18, 330, 58, TextAnchor.MiddleCenter, FontStyle.Bold);
        DewyUI.Label("EndBody", card.transform, DewyContent.EndingBody, 13, DewyUI.Muted, 20, 80, 330, 55, TextAnchor.MiddleCenter);
        DewyUI.Label("EndMessage", card.transform, DewyContent.ConservationMessage, 14, DewyUI.GreenDeep, 20, 143, 330, 38, TextAnchor.MiddleCenter, FontStyle.Bold);
        return card.gameObject;
    }

    private static void DragTo(RectTransform stage, RectTransform target, PointerEventData e, float w, float h, float minY, float maxY)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(stage, e.position, e.pressEventCamera, out Vector2 local)) return;
        float x = Mathf.Clamp(local.x - w/2f, 2f, stage.rect.width - w - 2f);
        float y = Mathf.Clamp(-local.y - h/2f, minY, Mathf.Min(maxY, stage.rect.height - h - 8f));
        target.anchoredPosition = new Vector2(x, -y);
    }

    private static float Top(RectTransform rt) => -rt.anchoredPosition.y;
    private static Vector2 Center(RectTransform rt) => new Vector2(rt.anchoredPosition.x + rt.rect.width/2f, -rt.anchoredPosition.y + rt.rect.height/2f);
    private static float DistanceTo(RectTransform rt, Vector2 checkpoint) => Vector2.Distance(Center(rt), checkpoint + new Vector2(14,14));
}

public sealed class DewyRainDrop : MonoBehaviour
{
    public float Speed = 360f;
    private RectTransform rt;
    private void Awake() => rt = transform as RectTransform;
    private void Update()
    {
        if (rt == null) return;
        rt.anchoredPosition += new Vector2(0, -Speed * Time.unscaledDeltaTime);
        if (-rt.anchoredPosition.y > 610f) Destroy(gameObject);
    }
}

public sealed class DewyBob : MonoBehaviour
{
    private RectTransform rt;
    private Vector2 origin;
    private void Start() { rt = transform as RectTransform; if (rt != null) origin = rt.anchoredPosition; }
    private void Update() { if (rt != null) rt.anchoredPosition = origin + new Vector2(0, Mathf.Sin(Time.unscaledTime * 2.4f) * 6f); }
}
