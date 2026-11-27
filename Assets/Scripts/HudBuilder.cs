// P-gelados - Jackeline e Matheus
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Monta o HUD completo por codigo. Usado pelo menu P-Gelados (fica salvo na cena)
// e tambem como reserva automatica se o GameManager estiver sem HUD.
public static class HudBuilder
{
    static readonly Color PanelColor = new Color(0.04f, 0.14f, 0.28f, 0.72f);
    static readonly Color Gold = new Color(1f, 0.85f, 0.35f, 1f);

    public static GameObject Build(GameManager gm)
    {
        GameObject canvasGO = new GameObject("Canvas_HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        Transform root = canvasGO.transform;

        // ---- Titulo + creditos (topo, centro)
        RectTransform title = Panel("Painel_Titulo", root, new Vector2(0.5f, 1f), new Vector2(0f, -30f), new Vector2(520f, 120f), PanelColor);
        Label("Titulo", title, "P-GELADOS", 64f, TextAlignmentOptions.Center, new Vector2(0.5f, 1f), new Vector2(0f, -8f), new Vector2(500f, 80f), Color.white, true);
        Label("Creditos", title, "Jackeline & Matheus", 28f, TextAlignmentOptions.Center, new Vector2(0.5f, 0f), new Vector2(0f, 10f), new Vector2(500f, 40f), new Color(0.8f, 0.92f, 1f, 1f), false);

        // ---- Peixes + barra de progresso (topo, esquerda)
        RectTransform fishPanel = Panel("Painel_Peixes", root, new Vector2(0f, 1f), new Vector2(30f, -30f), new Vector2(540f, 150f), PanelColor);
        gm.scoreText = Label("ScoreText", fishPanel, "Peixes: 0 / " + gm.fishNeeded, 50f, TextAlignmentOptions.TopLeft, new Vector2(0f, 1f), new Vector2(24f, -14f), new Vector2(500f, 70f), Color.white, true);
        RectTransform barBg = Panel("Barra_Fundo", fishPanel, new Vector2(0f, 0f), new Vector2(24f, 24f), new Vector2(492f, 28f), new Color(0f, 0f, 0f, 0.55f));
        RectTransform fill = Panel("Barra_Preenchimento", barBg, new Vector2(0f, 0f), Vector2.zero, Vector2.zero, new Color(1f, 0.62f, 0.12f, 1f));
        fill.anchorMin = new Vector2(0f, 0f);
        fill.anchorMax = new Vector2(0f, 1f);
        fill.pivot = new Vector2(0f, 0.5f);
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
        gm.fishBarFill = fill;

        // ---- Vidas (topo, direita)
        RectTransform livesPanel = Panel("Painel_Vidas", root, new Vector2(1f, 1f), new Vector2(-30f, -30f), new Vector2(400f, 150f), PanelColor);
        gm.livesText = Label("LivesText", livesPanel, "Vidas: " + gm.lives, 50f, TextAlignmentOptions.TopRight, new Vector2(1f, 1f), new Vector2(-24f, -14f), new Vector2(360f, 70f), Color.white, true);
        int n = Mathf.Max(1, gm.lives);
        Image[] icons = new Image[n];
        for (int i = 0; i < n; i++)
        {
            RectTransform ic = Panel("Vida_" + (i + 1), livesPanel, new Vector2(1f, 0f), Vector2.zero, new Vector2(34f, 34f), new Color(1f, 0.35f, 0.4f, 1f));
            ic.pivot = new Vector2(0.5f, 0.5f);
            ic.anchoredPosition = new Vector2(-45f - i * 56f, 42f);
            ic.localRotation = Quaternion.Euler(0f, 0f, 45f);
            icons[i] = ic.GetComponent<Image>();
        }
        gm.lifeIcons = icons;

        // ---- Mensagens (centro-baixo)
        RectTransform msgPanel = Panel("Painel_Mensagem", root, new Vector2(0.5f, 0.2f), Vector2.zero, new Vector2(1500f, 130f), new Color(0f, 0f, 0f, 0.6f));
        msgPanel.pivot = new Vector2(0.5f, 0.5f);
        gm.messageText = Label("MessageText", msgPanel, "", 52f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1460f, 110f), Gold, true);
        gm.messagePanel = msgPanel.gameObject;
        msgPanel.gameObject.SetActive(false);

        // ---- Controles (baixo, esquerda)
        RectTransform hint = Panel("Painel_Controles", root, new Vector2(0f, 0f), new Vector2(30f, 24f), new Vector2(1330f, 56f), new Color(0f, 0f, 0f, 0.5f));
        Label("Controles", hint, "WASD: mover   |   ESPACO: pular / subir   |   SHIFT: descer   |   Q/E: girar camera   |   R: reiniciar", 28f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1300f, 50f), Color.white, false);

        // ---- Flash vermelho ao levar dano
        GameObject flash = new GameObject("DamageFlash", typeof(RectTransform), typeof(Image));
        flash.transform.SetParent(root, false);
        Stretch(flash.GetComponent<RectTransform>());
        Image flashImg = flash.GetComponent<Image>();
        flashImg.color = new Color(0.9f, 0.1f, 0.1f, 0f);
        flashImg.raycastTarget = false;
        gm.damageFlash = flashImg;

        // ---- Tela de fim (vitoria / derrota)
        GameObject end = new GameObject("Painel_Fim", typeof(RectTransform), typeof(Image));
        end.transform.SetParent(root, false);
        Stretch(end.GetComponent<RectTransform>());
        Image endImg = end.GetComponent<Image>();
        endImg.color = new Color(0f, 0.05f, 0.12f, 0.82f);
        endImg.raycastTarget = false;
        gm.endTitleText = Label("FimTitulo", end.transform, "", 130f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0f, 170f), new Vector2(1700f, 180f), Gold, true);
        gm.endDetailText = Label("FimDetalhes", end.transform, "", 48f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), new Vector2(0f, -40f), new Vector2(1700f, 380f), Color.white, false);
        gm.endPanel = end;
        end.SetActive(false);

        return canvasGO;
    }

    static RectTransform Panel(string name, Transform parent, Vector2 anchor, Vector2 pos, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Image img = go.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return rt;
    }

    static TMP_Text Label(string name, Transform parent, string text, float size, TextAlignmentOptions align, Vector2 anchor, Vector2 pos, Vector2 box, Color color, bool bold)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = box;
        TextMeshProUGUI t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.alignment = align;
        t.color = color;
        t.raycastTarget = false;
        if (bold) t.fontStyle = FontStyles.Bold;
        return t;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
