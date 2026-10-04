using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class DewyUI
{
    public static readonly Color Ink = Hex("#11394D");
    public static readonly Color Muted = Hex("#5F7280");
    public static readonly Color Blue = Hex("#58BFE8");
    public static readonly Color BlueDeep = Hex("#238FBD");
    public static readonly Color Sea = Hex("#49ACD2");
    public static readonly Color Sun = Hex("#FFC94F");
    public static readonly Color Green = Hex("#68BF72");
    public static readonly Color GreenDeep = Hex("#378F4F");
    public static readonly Color Earth = Hex("#BD8D63");

    private static readonly Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();
    private static Font font;
    public static Font Font => font != null ? font : (font = Resources.GetBuiltinResource<Font>("Arial.ttf"));

    public static Color Hex(string value)
    {
        ColorUtility.TryParseHtmlString(value, out Color c);
        return c;
    }

    public static RectTransform MakeRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.localScale = Vector3.one;
        return rt;
    }

    public static void Place(RectTransform rt, float x, float y, float w, float h)
    {
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(x, -y);
        rt.sizeDelta = new Vector2(w, h);
    }

    public static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public static Image Panel(string name, Transform parent, Color color, float x, float y, float w, float h, bool rounded = false)
    {
        RectTransform rt = MakeRect(name, parent);
        Place(rt, x, y, w, h);
        Image image = rt.gameObject.AddComponent<Image>();
        image.color = color;
        if (rounded)
        {
            image.sprite = RoundedSprite();
            image.type = Image.Type.Sliced;
        }
        return image;
    }

    public static Image Circle(string name, Transform parent, Color color, float x, float y, float size)
    {
        Image image = Panel(name, parent, color, x, y, size, size);
        image.sprite = CircleSprite();
        image.type = Image.Type.Simple;
        image.preserveAspect = true;
        return image;
    }

    public static Text Label(string name, Transform parent, string value, int size, Color color, float x, float y, float w, float h,
        TextAnchor anchor = TextAnchor.UpperLeft, FontStyle style = FontStyle.Normal)
    {
        RectTransform rt = MakeRect(name, parent);
        Place(rt, x, y, w, h);
        Text t = rt.gameObject.AddComponent<Text>();
        t.font = Font;
        t.fontSize = size;
        t.fontStyle = style;
        t.color = color;
        t.alignment = anchor;
        t.text = value;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        t.raycastTarget = false;
        return t;
    }

    public static Button Button(string name, Transform parent, string label, float x, float y, float w, float h, Color bg, Color fg, int fontSize = 22)
    {
        Image image = Panel(name, parent, bg, x, y, w, h, true);
        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.97f,0.97f,0.97f,1f);
        colors.pressedColor = new Color(0.88f,0.88f,0.88f,1f);
        colors.disabledColor = new Color(1f,1f,1f,0.35f);
        colors.colorMultiplier = 1f;
        button.colors = colors;
        Label("Text", image.transform, label, fontSize, fg, 0, 0, w, h, TextAnchor.MiddleCenter, FontStyle.Bold);
        return button;
    }

    public static Image Stage(Transform parent, Color background)
    {
        Image stage = Panel("Stage", parent, background, 16, 190, 418, 612, true);
        Mask mask = stage.gameObject.AddComponent<Mask>();
        mask.showMaskGraphic = true;
        return stage;
    }

    public static Text Instruction(Transform stage, string instruction)
    {
        Image card = Panel("Instruction", stage, new Color(1,1,1,0.92f), 14, 14, 390, 54, true);
        Shadow shadow = card.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0.13f,0.41f,0.53f,0.12f);
        shadow.effectDistance = new Vector2(0,-3);
        return Label("InstructionText", card.transform, instruction, 12, Hex("#395563"), 12, 8, 366, 40, TextAnchor.MiddleLeft, FontStyle.Normal);
    }

    public static Text LearningLabel(Transform stage, string value, float y = 548f)
    {
        Image card = Panel("LearningLabel", stage, Color.white, 109, y, 200, 42, true);
        card.gameObject.SetActive(false);
        return Label("LearningText", card.transform, value, 13, Ink, 0, 0, 200, 42, TextAnchor.MiddleCenter, FontStyle.Bold);
    }

    public static Image Dewy(string name, Transform parent, float x, float y, float size = 72f)
    {
        Image drop = Circle(name, parent, Hex("#62C9ED"), x, y, size);
        Image shine = Circle("Shine", drop.transform, new Color(1,1,1,0.32f), size * .13f, size * .13f, size * .28f);
        shine.raycastTarget = false;
        RectTransform face = MakeRect("Face", drop.transform);
        Place(face, 0, 0, size, size);
        Circle("EyeL", face, Ink, size*.28f, size*.38f, 6f).raycastTarget = false;
        Circle("EyeR", face, Ink, size*.64f, size*.38f, 6f).raycastTarget = false;
        Image smile = Panel("Smile", face, Ink, size*.35f, size*.62f, size*.30f, 3f, true);
        smile.raycastTarget = false;
        return drop;
    }

    public static RectTransform Cloud(string name, Transform parent, float x, float y, float w, float h, Color color)
    {
        RectTransform root = MakeRect(name, parent);
        Place(root, x, y, w, h);
        Circle("LobeA", root, color, 0, h*.34f, h*.62f);
        Circle("LobeB", root, color, w*.23f, 0, h*.82f);
        Circle("LobeC", root, color, w*.60f, h*.36f, h*.58f);
        Panel("Base", root, color, w*.10f, h*.52f, w*.80f, h*.38f, true);
        return root;
    }

    public static Sprite CircleSprite() => MakeSprite("circle", true);
    public static Sprite RoundedSprite() => MakeSprite("rounded", false);

    private static Sprite MakeSprite(string key, bool circle)
    {
        if (SpriteCache.TryGetValue(key, out Sprite cached)) return cached;
        const int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.name = "Dewy_" + key;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        float radius = circle ? 31.5f : 14f;
        for (int y=0;y<size;y++) for (int x=0;x<size;x++)
        {
            float alpha = 1f;
            if (circle)
            {
                float dx=x-31.5f, dy=y-31.5f;
                alpha = Mathf.Clamp01(32f - Mathf.Sqrt(dx*dx+dy*dy));
            }
            else
            {
                float cx = Mathf.Clamp(x, radius, size-1-radius);
                float cy = Mathf.Clamp(y, radius, size-1-radius);
                float dx=x-cx, dy=y-cy;
                alpha = Mathf.Clamp01(radius + .5f - Mathf.Sqrt(dx*dx+dy*dy));
            }
            tex.SetPixel(x,y,new Color(1,1,1,alpha));
        }
        tex.Apply();
        Vector4 border = circle ? Vector4.zero : new Vector4(14,14,14,14);
        Sprite sprite = Sprite.Create(tex, new Rect(0,0,size,size), new Vector2(.5f,.5f), 100f, 0, SpriteMeshType.FullRect, border);
        sprite.name = "Dewy_" + key;
        SpriteCache[key] = sprite;
        return sprite;
    }
}
