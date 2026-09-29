using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// [LobbyUIStyler]
//
// Tools > Lobby UI > Apply Layout 을 누르면:
//   씬에 있는 기존 UI 오브젝트(버튼/텍스트/입력창)를 이름으로 찾아서
//   헤더 + 카드 3개(Connection / Lobby / Room) 레이아웃으로 재배치하고 스타일만 입힌다.
//   오브젝트를 새로 만들지 않고 옮기기만 하므로
//   PhotonLobbyManager / CreateRoom 의 참조와 버튼 OnClick 연결은 그대로 유지된다.
//   여러 번 실행해도 같은 결과가 나온다.

public static class LobbyUIStyler
{
    const string UndoName = "Apply Lobby UI";

    static readonly Color BgColor      = Hex("151A24");
    static readonly Color HeaderColor  = Hex("1B2230");
    static readonly Color CardColor    = Hex("1F2633");
    static readonly Color DividerColor = Hex("2E3747");
    static readonly Color InputColor   = Hex("121722");
    static readonly Color BadgeColor   = Hex("2A3344");
    static readonly Color TextMain     = Hex("E6EAF2");
    static readonly Color TextSub      = Hex("8A94A8");
    static readonly Color Blue         = Hex("3B82F6");
    static readonly Color Red          = Hex("E5484D");
    static readonly Color Green        = Hex("22A55E");
    static readonly Color Gray         = Hex("4B5563");

    static Sprite roundedSprite;

    [MenuItem("Tools/Lobby UI/Apply Layout")]
    public static void Apply()
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        if (canvasGo == null)
        {
            Debug.LogError("[LobbyUIStyler] Canvas를 찾을 수 없습니다.");
            return;
        }

        Transform canvas = canvasGo.transform;
        Undo.RegisterFullObjectHierarchyUndo(canvasGo, UndoName);
        roundedSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        // 기존 오브젝트
        Transform panel        = Require(canvas, "Panel");
        Transform statusText   = Require(canvas, "StatusText");
        Transform gameVersion  = Require(canvas, "GameVersion");
        Transform nickName     = Require(canvas, "NickName");
        Transform nickInput    = Require(canvas, "InputField (TMP)");
        Transform connectBtn   = Require(canvas, "ConnectButton");
        Transform disconBtn    = Require(canvas, "DisconnectButton");
        Transform lobbyStats   = Require(canvas, "LobbyStatsText");
        Transform lobbyBtn     = Require(canvas, "LobbyStatsButton");
        Transform roomInfo     = Require(canvas, "RoomInfoText");
        Transform createBtn    = Require(canvas, "CreateRoomButton");
        Transform roomInfoBtn  = Require(canvas, "RoomInfo");
        if (panel == null || statusText == null || gameVersion == null || nickName == null ||
            nickInput == null || connectBtn == null || disconBtn == null || lobbyStats == null ||
            lobbyBtn == null || roomInfo == null || createBtn == null || roomInfoBtn == null)
            return;

        // 1. Canvas 스케일러: 해상도에 맞춰 늘어나도록
        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        // 2. 배경
        Stretch((RectTransform)panel, Vector2.zero, Vector2.zero);
        Image panelImage = panel.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.sprite = null;
            panelImage.color = BgColor;
        }

        // 3. 헤더
        RectTransform header = Container(panel, "UI_Header", 0);
        header.anchorMin = new Vector2(0, 1);
        header.anchorMax = new Vector2(1, 1);
        header.pivot = new Vector2(0.5f, 1);
        header.anchoredPosition = Vector2.zero;
        header.sizeDelta = new Vector2(0, 96);
        GetOrAdd<Image>(header).color = HeaderColor;
        HorizontalLayoutGroup headerLayout = GetOrAdd<HorizontalLayoutGroup>(header);
        SetLayout(headerLayout, new RectOffset(48, 48, 0, 0), 20, TextAnchor.MiddleLeft, false, false);

        TMP_Text title = Text(header, "UI_Title", 0, "PHOTON LOBBY", 34, TextMain, TextAlignmentOptions.MidlineLeft, true);
        Layout(title, flexW: 1);

        TMP_Text versionLabel = Text(header, "UI_VersionLabel", 1, "VERSION", 18, TextSub, TextAlignmentOptions.MidlineRight, true);
        Layout(versionLabel);

        Place(gameVersion, header, 2);
        StyleText(gameVersion, "-", 26, TextMain, TextAlignmentOptions.MidlineLeft, false);
        Layout(gameVersion, prefW: 90);

        RectTransform badge = Container(header, "UI_StatusBadge", 3);
        Image badgeImage = GetOrAdd<Image>(badge);
        badgeImage.sprite = roundedSprite;
        badgeImage.type = Image.Type.Sliced;
        badgeImage.color = BadgeColor;
        Layout(badge, prefW: 380, prefH: 52);

        Place(statusText, badge, 0);
        Stretch((RectTransform)statusText, new Vector2(18, 0), new Vector2(-18, 0));
        TMP_Text status = StyleText(statusText, "Offline", 24, TextMain, TextAlignmentOptions.Center, true);
        status.enableAutoSizing = true;
        status.fontSizeMin = 14;
        status.fontSizeMax = 24;
        status.textWrappingMode = TextWrappingModes.NoWrap;
        status.overflowMode = TextOverflowModes.Ellipsis;

        // 4. 본문: 카드 3장
        RectTransform content = Container(panel, "UI_Content", 1);
        Stretch(content, Vector2.zero, new Vector2(0, -96));
        HorizontalLayoutGroup contentLayout = GetOrAdd<HorizontalLayoutGroup>(content);
        SetLayout(contentLayout, new RectOffset(48, 48, 48, 48), 32, TextAnchor.UpperLeft, true, true);

        // Connection 카드
        RectTransform connCard = Card(content, "UI_ConnectionCard", 0, "Connection");
        Text(connCard, "UI_NickLabel", 2, "NICKNAME", 18, TextSub, TextAlignmentOptions.MidlineLeft, true);

        Place(nickInput, connCard, 3);
        StyleInputField(nickInput);
        Layout(nickInput, prefH: 60);

        RectTransform connRow = Container(connCard, "UI_ConnectButtons", 4);
        HorizontalLayoutGroup rowLayout = GetOrAdd<HorizontalLayoutGroup>(connRow);
        SetLayout(rowLayout, new RectOffset(0, 0, 0, 0), 16, TextAnchor.MiddleCenter, true, true);
        Layout(connRow, prefH: 56);
        Place(connectBtn, connRow, 0);
        StyleButton(connectBtn, "Connect", Blue);
        Layout(connectBtn, flexW: 1);
        Place(disconBtn, connRow, 1);
        StyleButton(disconBtn, "Disconnect", Red);
        Layout(disconBtn, flexW: 1);

        Layout(Container(connCard, "UI_Spacer", 5), flexH: 1);

        Text(connCard, "UI_CurrentNickLabel", 6, "CURRENT NICKNAME", 18, TextSub, TextAlignmentOptions.MidlineLeft, true);
        Place(nickName, connCard, 7);
        TMP_Text nick = StyleText(nickName, "-", 30, TextMain, TextAlignmentOptions.MidlineLeft, true);
        nick.overflowMode = TextOverflowModes.Ellipsis;
        Layout(nickName, prefH: 44);

        // Lobby 카드
        RectTransform lobbyCard = Card(content, "UI_LobbyCard", 1, "Lobby");
        Place(lobbyStats, lobbyCard, 2);
        TMP_Text stats = StyleText(lobbyStats, "Press Refresh Stats", 26, TextMain, TextAlignmentOptions.TopLeft, false);
        stats.lineSpacing = 30;
        Layout(lobbyStats, flexH: 1);
        Place(lobbyBtn, lobbyCard, 3);
        StyleButton(lobbyBtn, "Refresh Stats", Gray);
        Layout(lobbyBtn, prefH: 56);

        // Room 카드
        RectTransform roomCard = Card(content, "UI_RoomCard", 2, "Room");
        Place(roomInfo, roomCard, 2);
        StyleText(roomInfo, "Not in a room", 26, TextMain, TextAlignmentOptions.TopLeft, false);
        Layout(roomInfo, flexH: 1);
        Place(createBtn, roomCard, 3);
        StyleButton(createBtn, "Create Room", Green);
        Layout(createBtn, prefH: 56);
        Place(roomInfoBtn, roomCard, 4);
        StyleButton(roomInfoBtn, "Refresh Room", Gray);
        Layout(roomInfoBtn, prefH: 56);

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)panel);
        EditorSceneManager.MarkSceneDirty(canvasGo.scene);
        Debug.Log("[LobbyUIStyler] 레이아웃 적용 완료");
    }

    // ---------- 찾기 / 배치 ----------

    static Transform Require(Transform root, string name)
    {
        Transform found = FindDeep(root, name);
        if (found == null)
            Debug.LogError($"[LobbyUIStyler] '{name}' 오브젝트를 찾을 수 없습니다.");
        return found;
    }

    static Transform FindDeep(Transform root, string name)
    {
        if (root.name == name)
            return root;

        foreach (Transform child in root)
        {
            Transform found = FindDeep(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    static void Place(Transform target, Transform parent, int index)
    {
        if (target.parent != parent)
            Undo.SetTransformParent(target, parent, false, UndoName);

        target.SetSiblingIndex(Mathf.Min(index, parent.childCount - 1));
        target.localScale = Vector3.one;
        target.localRotation = Quaternion.identity;
    }

    static RectTransform Container(Transform parent, string name, int index)
    {
        Transform existing = parent.Find(name);
        GameObject go;
        if (existing != null)
        {
            go = existing.gameObject;
        }
        else
        {
            go = new GameObject(name, typeof(RectTransform));
            go.layer = parent.gameObject.layer;
            Undo.RegisterCreatedObjectUndo(go, UndoName);
        }

        Place(go.transform, parent, index);
        return (RectTransform)go.transform;
    }

    static RectTransform Card(Transform parent, string name, int index, string title)
    {
        RectTransform card = Container(parent, name, index);
        Image image = GetOrAdd<Image>(card);
        image.sprite = roundedSprite;
        image.type = Image.Type.Sliced;
        image.color = CardColor;
        VerticalLayoutGroup layout = GetOrAdd<VerticalLayoutGroup>(card);
        SetLayout(layout, new RectOffset(32, 32, 28, 32), 16, TextAnchor.UpperLeft, true, false);
        Layout(card, flexW: 1);

        Text(card, "UI_CardTitle", 0, title, 30, TextMain, TextAlignmentOptions.MidlineLeft, true);
        RectTransform divider = Container(card, "UI_Divider", 1);
        GetOrAdd<Image>(divider).color = DividerColor;
        Layout(divider, prefH: 2);

        return card;
    }

    static void Stretch(RectTransform rt, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
    }

    // ---------- 레이아웃 ----------

    static void SetLayout(HorizontalOrVerticalLayoutGroup layout, RectOffset padding, float spacing,
                          TextAnchor alignment, bool expandWidth, bool expandHeight)
    {
        layout.padding = padding;
        layout.spacing = spacing;
        layout.childAlignment = alignment;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = expandWidth;
        layout.childForceExpandHeight = expandHeight;
    }

    static void Layout(Component target, float prefW = -1, float prefH = -1, float flexW = -1, float flexH = -1)
    {
        LayoutElement le = GetOrAdd<LayoutElement>(target);
        le.preferredWidth = prefW;
        le.preferredHeight = prefH;
        le.minHeight = prefH;
        le.flexibleWidth = flexW;
        le.flexibleHeight = flexH;
    }

    // ---------- 스타일 ----------

    static TMP_Text Text(Transform parent, string name, int index, string text, float size, Color color,
                         TextAlignmentOptions align, bool bold)
    {
        RectTransform rt = Container(parent, name, index);
        return StyleText(rt, text, size, color, align, bold);
    }

    static TMP_Text StyleText(Component target, string text, float size, Color color,
                              TextAlignmentOptions align, bool bold)
    {
        TMP_Text t = target.GetComponent<TMP_Text>();
        if (t == null)
            t = Undo.AddComponent<TextMeshProUGUI>(target.gameObject);
        if (t.font == null)
            t.font = TMP_Settings.defaultFontAsset;

        t.text = text;
        t.fontSize = size;
        t.enableAutoSizing = false;
        t.color = color;
        t.alignment = align;
        t.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
        t.textWrappingMode = TextWrappingModes.Normal;
        t.overflowMode = TextOverflowModes.Overflow;
        t.lineSpacing = 0;
        t.raycastTarget = false;
        return t;
    }

    static void StyleButton(Transform target, string label, Color color)
    {
        Image image = target.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = roundedSprite;
            image.type = Image.Type.Sliced;
            image.color = color;
        }

        Button button = target.GetComponent<Button>();
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.88f, 0.88f, 0.88f, 1f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            colors.selectedColor = Color.white;
            colors.disabledColor = new Color(1f, 1f, 1f, 0.25f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            button.colors = colors;
        }

        TMP_Text text = target.GetComponentInChildren<TMP_Text>(true);
        if (text != null)
        {
            StyleText(text, label, 24, Color.white, TextAlignmentOptions.Center, true);
            Stretch(text.rectTransform, Vector2.zero, Vector2.zero);
        }
    }

    static void StyleInputField(Transform target)
    {
        Image image = target.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = roundedSprite;
            image.type = Image.Type.Sliced;
            image.color = InputColor;
        }

        TMP_InputField input = target.GetComponent<TMP_InputField>();
        if (input == null)
            return;

        if (input.textViewport != null)
            Stretch(input.textViewport, new Vector2(18, 6), new Vector2(-18, -6));

        if (input.textComponent != null)
        {
            input.textComponent.fontSize = 24;
            input.textComponent.color = TextMain;
            input.textComponent.alignment = TextAlignmentOptions.MidlineLeft;
        }

        if (input.placeholder is TMP_Text placeholder)
        {
            placeholder.text = "Enter nickname...";
            placeholder.fontSize = 24;
            placeholder.fontStyle = FontStyles.Italic;
            placeholder.color = new Color(TextSub.r, TextSub.g, TextSub.b, 0.7f);
            placeholder.alignment = TextAlignmentOptions.MidlineLeft;
        }

        input.customCaretColor = true;
        input.caretColor = TextMain;
        input.caretWidth = 2;
        input.selectionColor = new Color(Blue.r, Blue.g, Blue.b, 0.4f);

        ColorBlock colors = input.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.92f, 0.92f, 0.92f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(1f, 1f, 1f, 0.4f);
        input.colors = colors;
    }

    // ---------- 유틸 ----------

    static T GetOrAdd<T>(Component target) where T : Component
    {
        T comp = target.GetComponent<T>();
        return comp != null ? comp : Undo.AddComponent<T>(target.gameObject);
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color color);
        return color;
    }
}
