// P-gelados - Jackeline e Matheus
// Menu "P-Gelados" no topo do Unity: organiza a cena, monta HUD, gera itens e valida a entrega.
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;

public static class PGeladosSetup
{
    const string AudioFolder = "Assets/Audio";
    const string ArtFolder = "Assets/Art";
    const string PenguinPrefab = "Assets/Hosh/Stylized Penguin (Free)/Prefabs/URP/Penguen_Emperor_Mesh_Lite.prefab";
    const string FishFolder = "Assets/Alstra Infinite/Fish - PolyPack/Prefabs/";

    // =====================================================================
    // 0) TUDO DE UMA VEZ
    // =====================================================================
    [MenuItem("P-Gelados/0) FAZER TUDO (recomendado)", false, 0)]
    static void FazerTudo()
    {
        if (!Organizar()) return;
        Visual();
        Gerar();
        Validar();
    }

    // =====================================================================
    // 1) ORGANIZAR CENA + HUD + SONS
    // =====================================================================
    [MenuItem("P-Gelados/1) Organizar cena, HUD e sons", false, 1)]
    static void OrganizarMenu()
    {
        Organizar();
    }

    static bool Organizar()
    {
        if (TMP_Settings.instance == null)
        {
            Debug.LogError("Importe o TMP Essentials: Window > TextMeshPro > Import TMP Essential Resources, e rode de novo.");
            return false;
        }

        // Nomes exigidos pela atividade
        PlayerSettings.productName = "P-gelados";
        PlayerSettings.companyName = "Jackeline e Matheus";

        // ---- Jogador ----
        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO == null)
        {
            playerGO = GameObject.Find("Penguen_Emperor_Mesh_Lite");
            if (playerGO != null) playerGO.tag = "Player";
        }
        if (playerGO == null)
        {
            Debug.LogError("Nao achei o pinguim. Coloque o prefab do pinguim na cena e marque a Tag como Player.");
            return false;
        }

        if (playerGO.GetComponent<Rigidbody>() == null) Undo.AddComponent<Rigidbody>(playerGO);
        if (playerGO.GetComponent<Collider>() == null)
        {
            SphereCollider sc = Undo.AddComponent<SphereCollider>(playerGO);
            sc.radius = 0.8f;
            sc.center = new Vector3(0f, 0.7f, 0f);
        }
        if (playerGO.GetComponent<PlayerController>() == null) Undo.AddComponent<PlayerController>(playerGO);

        Rigidbody rb = playerGO.GetComponent<Rigidbody>();
        Undo.RecordObject(rb, "Config Rigidbody");
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // ---- Grupos (hierarquia organizada) ----
        Transform gCenario = GetGroup("Cenario");
        Transform gColet = GetGroup("Coletaveis");
        Transform gInim = GetGroup("Inimigos");
        Transform gObst = GetGroup("Obstaculos");

        string[] cenarioNomes = { "GeloInicio", "GeloFinal", "Plataforma_1", "FundoDoMar", "Agua", "ParedesInvisiveis" };
        foreach (string n in cenarioNomes) MoveTo(n, gCenario);

        // ---- Peixes ----
        Fish[] fishes = Object.FindObjectsByType<Fish>(FindObjectsSortMode.None);
        foreach (Fish f in fishes)
        {
            SetTrigger(f.gameObject);
            if (f.transform.parent == null) Undo.SetTransformParent(f.transform, gColet, "Organizar");
        }

        // ---- Inimigos e obstaculos ----
        Damager[] dmgs = Object.FindObjectsByType<Damager>(FindObjectsSortMode.None);
        foreach (Damager d in dmgs)
        {
            SetTrigger(d.gameObject);
            bool inimigo = d.GetComponent<Patrol>() != null;
            if (inimigo && d.GetComponent<Rigidbody>() == null)
            {
                Rigidbody drb = Undo.AddComponent<Rigidbody>(d.gameObject);
                drb.isKinematic = true;
                drb.useGravity = false;
            }
            if (d.transform.parent == null)
                Undo.SetTransformParent(d.transform, inimigo ? gInim : gObst, "Organizar");
        }

        // ---- Agua ----
        GameObject agua = GameObject.Find("Agua");
        if (agua != null)
        {
            if (agua.GetComponent<Water>() == null) Undo.AddComponent<Water>(agua);
            SetTrigger(agua);
        }
        else Debug.LogWarning("Objeto 'Agua' nao encontrado.");

        // ---- Iglu ----
        GameObject iglu = GameObject.Find("Igloo");
        if (iglu != null)
        {
            if (iglu.GetComponent<Igloo>() == null) Undo.AddComponent<Igloo>(iglu);
            SetTrigger(iglu);
            SphereCollider isc = iglu.GetComponent<SphereCollider>();
            if (isc != null)
            {
                Undo.RecordObject(isc, "Igloo collider");
                isc.radius = Mathf.Max(isc.radius, 0.7f);
            }

            GameObject fim = GameObject.Find("GeloFinal");
            if (fim != null)
            {
                Collider fc = fim.GetComponent<Collider>();
                float dist = Vector3.Distance(iglu.transform.position, fim.transform.position);
                if (fc != null && dist > 8f)
                {
                    bool mover = EditorUtility.DisplayDialog("Iglu",
                        "O iglu esta longe do gelo final (fim da fase). Mover o iglu para o GeloFinal?\n\n(A vitoria e chegar nele com os peixes.)",
                        "Mover para o GeloFinal", "Deixar onde esta");
                    if (mover)
                    {
                        Bounds fb = fc.bounds;
                        Undo.RecordObject(iglu.transform, "Mover iglu");
                        iglu.transform.position = new Vector3(fb.center.x, fb.max.y - 0.1f, fb.center.z + fb.extents.z * 0.45f);
                    }
                }
            }
        }
        else Debug.LogWarning("Objeto 'Igloo' nao encontrado.");

        // ---- Camera ----
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraFollow cf = mainCam.GetComponent<CameraFollow>();
            if (cf == null) cf = Undo.AddComponent<CameraFollow>(mainCam.gameObject);
            Undo.RecordObject(cf, "Camera");
            cf.target = playerGO.transform;
        }

        // ---- GameManager + HUD ----
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm == null)
        {
            GameObject gmGO = new GameObject("GameManager");
            Undo.RegisterCreatedObjectUndo(gmGO, "Criar GameManager");
            gm = gmGO.AddComponent<GameManager>();
        }
        Undo.RecordObject(gm, "Config GameManager");
        gm.player = playerGO.transform;

        bool temHud = gm.scoreText != null && gm.livesText != null && gm.messageText != null;
        if (temHud && gm.endPanel == null)
        {
            bool trocar = EditorUtility.DisplayDialog("HUD",
                "Existe um HUD antigo na cena. Substituir pelo novo (com barra de peixes, vidas, flash de dano e tela de vitoria/derrota)?\n\nO Canvas antigo sera apagado.",
                "Substituir", "Manter o antigo");
            if (trocar)
            {
                Canvas oldCanvas = gm.scoreText.GetComponentInParent<Canvas>();
                if (oldCanvas != null) Undo.DestroyObjectImmediate(oldCanvas.rootCanvas.gameObject);
                gm.scoreText = null;
                gm.livesText = null;
                gm.messageText = null;
                temHud = false;
            }
        }
        if (!temHud)
        {
            GameObject hud = HudBuilder.Build(gm);
            Undo.RegisterCreatedObjectUndo(hud, "Criar HUD");
        }

        // ---- Sons (se a pasta Assets/Audio existir) ----
        AssignClip(ref gm.collectSound, "sfx_collect");
        AssignClip(ref gm.hurtSound, "sfx_hurt");
        AssignClip(ref gm.jumpSound, "sfx_jump");
        AssignClip(ref gm.splashSound, "sfx_splash");
        AssignClip(ref gm.winSound, "sfx_win");
        AssignClip(ref gm.loseSound, "sfx_lose");
        AssignClip(ref gm.music, "music_loop");
        EditorUtility.SetDirty(gm);

        // ---- Build Settings: a cena do jogo precisa ser a primeira ----
        Scene cena = EditorSceneManager.GetActiveScene();
        if (!string.IsNullOrEmpty(cena.path))
        {
            EditorBuildSettingsScene[] atuais = EditorBuildSettings.scenes;
            if (atuais.Length == 0 || atuais[0].path != cena.path)
                EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(cena.path, true) };
        }

        EditorSceneManager.MarkSceneDirty(cena);
        if (!string.IsNullOrEmpty(cena.path)) EditorSceneManager.SaveScene(cena);
        Debug.Log("P-Gelados: cena organizada, HUD e GameManager prontos.");
        return true;
    }

    // =====================================================================
    // 2) VISUAL: AGUA, GELO E FUNDO
    // =====================================================================
    [MenuItem("P-Gelados/2) Visual: agua, gelo e fundo", false, 2)]
    static void VisualMenu()
    {
        Visual();
    }

    static void Visual()
    {
        EnsureFolder(ArtFolder);

        // textura de ondas gerada por codigo (sem baixar nada)
        string texPath = ArtFolder + "/PG_AguaOndas.png";
        if (!File.Exists(texPath)) CriarTexturaAgua(texPath);
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);

        Material matAgua = CriarMaterialAgua(tex);
        GameObject agua = GameObject.Find("Agua");
        if (agua != null)
        {
            Renderer r = agua.GetComponent<Renderer>();
            if (r != null)
            {
                Undo.RecordObject(r, "Material da agua");
                r.sharedMaterial = matAgua;
                r.shadowCastingMode = ShadowCastingMode.Off;
                r.receiveShadows = false;
            }
            if (agua.GetComponent<WaterScroll>() == null) Undo.AddComponent<WaterScroll>(agua);
        }

        Material matGelo = MakeLit("PG_Gelo", new Color(0.72f, 0.9f, 1f, 1f), 0.85f);
        ApplyMat("GeloInicio", matGelo);
        ApplyMat("GeloFinal", matGelo);
        ApplyMat("Plataforma_1", matGelo);

        Material matFundo = MakeLit("PG_Fundo", new Color(0.35f, 0.42f, 0.5f, 1f), 0.1f);
        ApplyMat("FundoDoMar", matFundo);

        // neblina leve (mantem as variantes de neblina no build; debaixo d'agua a camera aumenta)
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = new Color(0.78f, 0.88f, 0.95f, 1f);
        RenderSettings.fogDensity = 0.004f;

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("P-Gelados: visual aplicado (agua animada, gelo, fundo e neblina).");
    }

    static void CriarTexturaAgua(string path)
    {
        int size = 256;
        Texture2D t = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float twoPi = 2f * Mathf.PI;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float u = x / (float)size;
                float v = y / (float)size;
                float a = 0f;
                a += Mathf.Sin((u * 3f + v * 2f) * twoPi + 0.3f);
                a += Mathf.Sin((u * -2f + v * 5f) * twoPi + 1.7f);
                a += Mathf.Sin((u * 4f - v * 3f) * twoPi + 4.1f);
                a += Mathf.Sin((u * 7f + v * 1f) * twoPi + 2.2f) * 0.5f;
                a = a / 3.5f;
                float ridge = Mathf.Pow(Mathf.Clamp01(1f - Mathf.Abs(a)), 8f);
                Color c = new Color(0.80f + 0.20f * ridge, 0.92f + 0.08f * ridge, 1f, 0.70f + 0.30f * ridge);
                t.SetPixel(x, y, c);
            }
        }
        t.Apply();
        File.WriteAllBytes(path, t.EncodeToPNG());
        Object.DestroyImmediate(t);
        AssetDatabase.ImportAsset(path);
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null)
        {
            ti.wrapMode = TextureWrapMode.Repeat;
            ti.alphaIsTransparency = true;
            ti.SaveAndReimport();
        }
    }

    static Material CriarMaterialAgua(Texture2D tex)
    {
        string path = ArtFolder + "/PG_Agua.mat";
        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            m = new Material(sh);
            AssetDatabase.CreateAsset(m, path);
        }
        m.shader = sh;
        m.SetColor("_BaseColor", new Color(0.10f, 0.50f, 0.85f, 0.70f));
        if (tex != null)
        {
            m.SetTexture("_BaseMap", tex);
            m.SetTextureScale("_BaseMap", new Vector2(10f, 22f));
        }
        m.SetFloat("_Smoothness", 0.9f);
        m.SetFloat("_Surface", 1f);
        m.SetFloat("_Blend", 0f);
        m.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
        m.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        m.SetFloat("_ZWrite", 0f);
        m.SetFloat("_Cull", 0f);
        m.SetOverrideTag("RenderType", "Transparent");
        m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        m.renderQueue = (int)RenderQueue.Transparent;
        m.SetShaderPassEnabled("ShadowCaster", false);
        EditorUtility.SetDirty(m);
        return m;
    }

    static Material MakeLit(string name, Color color, float smooth)
    {
        EnsureFolder(ArtFolder);
        string path = ArtFolder + "/" + name + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            Shader sh = Shader.Find("Universal Render Pipeline/Lit");
            if (sh == null) sh = Shader.Find("Standard");
            m = new Material(sh);
            AssetDatabase.CreateAsset(m, path);
        }
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
        if (m.HasProperty("_Color")) m.SetColor("_Color", color);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smooth);
        EditorUtility.SetDirty(m);
        return m;
    }

    static void ApplyMat(string objName, Material m)
    {
        GameObject go = GameObject.Find(objName);
        if (go == null) return;
        Renderer r = go.GetComponent<Renderer>();
        if (r == null) return;
        Undo.RecordObject(r, "Material");
        r.sharedMaterial = m;
    }

    // =====================================================================
    // 3) GERAR PEIXES, TUBAROES, OBSTACULOS, BLOCOS DE GELO E FAMILIA
    // =====================================================================
    [MenuItem("P-Gelados/3) Gerar peixes, tubaroes e obstaculos", false, 3)]
    static void GerarMenu()
    {
        Gerar();
    }

    static void Gerar()
    {
        GameObject agua = GameObject.Find("Agua");
        Collider ac = agua != null ? agua.GetComponent<Collider>() : null;
        Bounds wb = ac != null ? ac.bounds : new Bounds(new Vector3(0f, -5f, 30f), new Vector3(30f, 10f, 70f));
        float surface = wb.max.y;
        float half = Mathf.Max(4f, wb.extents.x - 5f);
        System.Random rnd = new System.Random(2026);

        Material matGelo = MakeLit("PG_Gelo", new Color(0.72f, 0.9f, 1f, 1f), 0.85f);
        Material matCristal = MakeLit("PG_Cristal", new Color(0.45f, 0.9f, 1f, 1f), 0.95f);

        // ---------- Peixes ----------
        Transform gFish = ResetGroup("Coletaveis_Gerados");
        List<Vector3> usados = new List<Vector3>();
        foreach (Fish f in Object.FindObjectsByType<Fish>(FindObjectsSortMode.None)) usados.Add(f.transform.position);

        string[] fishNames = { "FishV1", "FishV2", "FishV3", "FishV4" };
        int nPeixes = 15;
        for (int i = 0; i < nPeixes; i++)
        {
            Vector3 p = PontoAleatorio(rnd, wb, usados, 3.2f);
            usados.Add(p);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(FishFolder + fishNames[i % fishNames.Length] + ".prefab");
            GameObject root = new GameObject("Peixe_" + (i + 1));
            Undo.RegisterCreatedObjectUndo(root, "Gerar peixe");
            root.transform.SetParent(gFish, false);
            root.transform.position = p;
            GameObject model = CriarModelo(prefab, PrimitiveType.Capsule, root.transform);
            FitAndCenter(model.transform, root.transform, 0.9f);
            root.transform.rotation = Quaternion.Euler(0f, rnd.Next(0, 360), 0f);
            SphereCollider sc = root.AddComponent<SphereCollider>();
            sc.isTrigger = true;
            sc.radius = 0.9f;
            root.AddComponent<Fish>();
        }

        // ---------- Tubaroes (inimigos) ----------
        Transform gEnemy = ResetGroup("Inimigos_Gerados");
        GameObject sharkPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(FishFolder + "SharkV1.prefab");
        int nTub = 4;
        for (int i = 0; i < nTub; i++)
        {
            float t = (i + 1f) / (nTub + 1f);
            float z = Mathf.Lerp(wb.min.z, wb.max.z, t);
            float y = Mathf.Lerp(wb.min.y + 3f, surface - 2f, (i % 2 == 0) ? 0.25f : 0.7f);
            bool fromLeft = (i % 2 == 0);
            Vector3 start = new Vector3(wb.center.x + (fromLeft ? -half : half), y, z);

            GameObject root = new GameObject("Tubarao_" + (i + 1));
            Undo.RegisterCreatedObjectUndo(root, "Gerar tubarao");
            root.transform.SetParent(gEnemy, false);
            root.transform.position = start;

            GameObject model = CriarModelo(sharkPrefab, PrimitiveType.Capsule, root.transform);
            FitAndCenter(model.transform, root.transform, 3.2f);
            Bounds mb = GetBounds(model);
            if (mb.size.x > mb.size.z * 1.3f)
            {
                model.transform.RotateAround(root.transform.position, Vector3.up, 90f);
                mb = GetBounds(model);
                model.transform.position += root.transform.position - mb.center;
            }

            CapsuleCollider cc = root.AddComponent<CapsuleCollider>();
            cc.direction = 2;
            cc.radius = 0.7f;
            cc.height = 3.2f;
            cc.isTrigger = true;

            Rigidbody rb = root.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            root.AddComponent<Damager>();
            Patrol pat = root.AddComponent<Patrol>();
            pat.direction = fromLeft ? Vector3.right : Vector3.left;
            pat.distance = half * 2f;
            pat.speed = 2.2f + 0.4f * (i % 3);
            pat.chaseSpeed = pat.speed + 1.2f;
        }

        // ---------- Obstaculos (dano) ----------
        Transform gObst = ResetGroup("Obstaculos_Gerados");
        int nFundo = 6;
        for (int i = 0; i < nFundo; i++)
        {
            float z = Mathf.Lerp(wb.min.z + 5f, wb.max.z - 5f, (i + 0.5f) / nFundo);
            float x = Mathf.Lerp(-half, half, (float)rnd.NextDouble());
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Undo.RegisterCreatedObjectUndo(s, "Gerar obstaculo");
            s.name = "Cristal_Fundo_" + (i + 1);
            s.transform.SetParent(gObst, false);
            s.transform.localScale = new Vector3(1.1f, 3.5f, 1.1f);
            s.transform.position = new Vector3(wb.center.x + x, wb.min.y + 1.8f, z);
            s.transform.rotation = Quaternion.Euler(0f, rnd.Next(0, 360), rnd.Next(-10, 10));
            s.GetComponent<Renderer>().sharedMaterial = matCristal;
            s.GetComponent<Collider>().isTrigger = true;
            s.AddComponent<Damager>();
        }
        int nTopo = 4;
        for (int i = 0; i < nTopo; i++)
        {
            float z = Mathf.Lerp(wb.min.z + 10f, wb.max.z - 8f, (i + 0.5f) / nTopo);
            float x = Mathf.Lerp(-half + 2f, half - 2f, (float)rnd.NextDouble());
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Undo.RegisterCreatedObjectUndo(s, "Gerar obstaculo");
            s.name = "Iceberg_Espinhoso_" + (i + 1);
            s.transform.SetParent(gObst, false);
            s.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
            s.transform.position = new Vector3(wb.center.x + x, surface - 0.3f, z);
            s.transform.rotation = Quaternion.Euler(45f, rnd.Next(0, 360), 45f);
            s.GetComponent<Renderer>().sharedMaterial = matCristal;
            s.GetComponent<Collider>().isTrigger = true;
            s.AddComponent<Damager>();
        }

        // ---------- Blocos de gelo e rampas (solidos, sem dano) ----------
        Transform gBlocos = ResetGroup("Blocos_Gerados");
        int nBlocos = 5;
        for (int i = 0; i < nBlocos; i++)
        {
            float z = Mathf.Lerp(wb.min.z + 6f, wb.max.z - 6f, (i + 0.5f) / nBlocos);
            float x = ((i % 2 == 0) ? -1f : 1f) * (4f + (float)rnd.NextDouble() * 4f);
            float sx = 3.5f + (float)rnd.NextDouble() * 1.5f;
            float sz = 3.5f + (float)rnd.NextDouble() * 1.5f;
            GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Undo.RegisterCreatedObjectUndo(b, "Gerar bloco");
            b.name = "BlocoGelo_" + (i + 1);
            b.transform.SetParent(gBlocos, false);
            b.transform.localScale = new Vector3(sx, 1f, sz);
            b.transform.position = new Vector3(wb.center.x + x, surface - 0.5f, z);
            b.GetComponent<Renderer>().sharedMaterial = matGelo;

            if (i == 1 || i == 3)
            {
                GameObject r = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Undo.RegisterCreatedObjectUndo(r, "Gerar rampa");
                r.name = "Rampa_" + i;
                r.transform.SetParent(gBlocos, false);
                r.transform.localScale = new Vector3(3f, 0.4f, 6f);
                r.transform.position = new Vector3(wb.center.x + x, surface + 0.5f, z - 0.2f);
                r.transform.rotation = Quaternion.Euler(-14f, 0f, 0f);
                r.GetComponent<Renderer>().sharedMaterial = matGelo;
            }
        }

        // ---------- Familia de pinguins perto do iglu ----------
        Transform gFam = ResetGroup("Familia_Gerada");
        GameObject iglu = GameObject.Find("Igloo");
        GameObject penPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PenguinPrefab);
        if (iglu != null && penPrefab != null)
        {
            Vector3 c = iglu.transform.position;
            GameObject fim = GameObject.Find("GeloFinal");
            Collider fc = fim != null ? fim.GetComponent<Collider>() : null;
            float groundY = fc != null ? fc.bounds.max.y + 0.02f : c.y + 0.1f;
            Vector3[] offs = { new Vector3(-3.2f, 0f, -3.8f), new Vector3(3.0f, 0f, -3.4f), new Vector3(0.6f, 0f, -4.8f) };
            float[] esc = { 0.55f, 0.5f, 0.4f };
            for (int i = 0; i < offs.Length; i++)
            {
                GameObject p = (GameObject)PrefabUtility.InstantiatePrefab(penPrefab);
                Undo.RegisterCreatedObjectUndo(p, "Gerar familia");
                p.name = "Pinguim_Familia_" + (i + 1);
                p.transform.SetParent(gFam, false);
                p.transform.position = new Vector3(c.x + offs[i].x, groundY, c.z + offs[i].z);
                p.transform.localScale = Vector3.one * esc[i];
                p.transform.rotation = Quaternion.Euler(0f, 180f + i * 15f, 0f);
                p.tag = "Untagged";
            }
        }
        else Debug.LogWarning("Familia nao gerada (iglu ou prefab do pinguim nao encontrados).");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("P-Gelados: gerados " + nPeixes + " peixes, " + nTub + " tubaroes, " + (nFundo + nTopo) + " obstaculos, " + nBlocos + " blocos de gelo.");
    }

    static GameObject CriarModelo(GameObject prefab, PrimitiveType fallback, Transform parent)
    {
        GameObject model;
        if (prefab != null) model = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        else model = GameObject.CreatePrimitive(fallback);
        model.name = "Modelo";
        model.transform.SetParent(parent, false);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        Collider[] cols = model.GetComponentsInChildren<Collider>();
        for (int i = 0; i < cols.Length; i++) Object.DestroyImmediate(cols[i]);
        return model;
    }

    static Vector3 PontoAleatorio(System.Random rnd, Bounds wb, List<Vector3> usados, float minDist)
    {
        Vector3 p = wb.center;
        for (int tentativa = 0; tentativa < 40; tentativa++)
        {
            float x = Mathf.Lerp(wb.min.x + 3f, wb.max.x - 3f, (float)rnd.NextDouble());
            float z = Mathf.Lerp(wb.min.z + 8f, wb.max.z - 8f, (float)rnd.NextDouble());
            float y = Mathf.Lerp(wb.min.y + 2.5f, wb.max.y - 1.8f, (float)rnd.NextDouble());
            p = new Vector3(x, y, z);
            bool ok = true;
            for (int i = 0; i < usados.Count; i++)
            {
                if ((usados[i] - p).sqrMagnitude < minDist * minDist) { ok = false; break; }
            }
            if (ok) return p;
        }
        return p;
    }

    static void FitAndCenter(Transform model, Transform root, float targetSize)
    {
        Bounds b = GetBounds(model.gameObject);
        float maxDim = Mathf.Max(b.size.x, Mathf.Max(b.size.y, b.size.z));
        if (maxDim > 0.0001f) model.localScale = model.localScale * (targetSize / maxDim);
        b = GetBounds(model.gameObject);
        model.position += root.position - b.center;
    }

    static Bounds GetBounds(GameObject go)
    {
        Renderer[] rs = go.GetComponentsInChildren<Renderer>();
        if (rs.Length == 0) return new Bounds(go.transform.position, Vector3.one);
        Bounds b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        return b;
    }

    // =====================================================================
    // 4) VALIDAR (CHECKLIST DE ENTREGA)
    // =====================================================================
    [MenuItem("P-Gelados/4) Validar projeto (checklist)", false, 4)]
    static void ValidarMenu()
    {
        Validar();
    }

    static void Validar()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int ok = 0;
        int falta = 0;

        Check(sb, ref ok, ref falta, PlayerSettings.companyName.Contains("Jackeline") && PlayerSettings.companyName.Contains("Matheus"),
            "Nomes Jackeline e Matheus nas configuracoes do projeto");
        Check(sb, ref ok, ref falta, PlayerSettings.productName.ToLower().Contains("gelados"), "Nome do jogo: P-gelados");

        GameObject player = GameObject.FindWithTag("Player");
        Check(sb, ref ok, ref falta, player != null, "Personagem principal com tag Player");
        if (player != null)
        {
            Check(sb, ref ok, ref falta, player.GetComponent<PlayerController>() != null, "Personagem com PlayerController");
            Check(sb, ref ok, ref falta, player.GetComponent<Rigidbody>() != null, "Personagem com Rigidbody");
            Collider pc = player.GetComponent<Collider>();
            Check(sb, ref ok, ref falta, pc != null && !pc.isTrigger, "Personagem com Collider (nao trigger)");
        }

        CameraFollow cf = Object.FindFirstObjectByType<CameraFollow>();
        Check(sb, ref ok, ref falta, cf != null && cf.target != null, "Camera seguindo o personagem (3a pessoa)");

        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        Check(sb, ref ok, ref falta, gm != null, "GameManager na cena");
        int totalPeixes = Object.FindObjectsByType<Fish>(FindObjectsSortMode.None).Length;
        if (gm != null)
        {
            Check(sb, ref ok, ref falta, gm.scoreText != null && gm.livesText != null && gm.messageText != null, "HUD ligado ao GameManager (peixes, vidas, mensagens)");
            Check(sb, ref ok, ref falta, totalPeixes >= gm.fishNeeded, "Peixes na cena (" + totalPeixes + ") >= peixes necessarios (" + gm.fishNeeded + ")");
        }

        int semTrigger = 0;
        foreach (Fish f in Object.FindObjectsByType<Fish>(FindObjectsSortMode.None))
        {
            Collider c = f.GetComponent<Collider>();
            if (c == null || !c.isTrigger) semTrigger++;
        }
        Check(sb, ref ok, ref falta, totalPeixes > 0 && semTrigger == 0, "Todos os peixes tem Collider em modo Trigger");

        int inimigos = Object.FindObjectsByType<Patrol>(FindObjectsSortMode.None).Length;
        int hazards = 0;
        int dmgSemTrigger = 0;
        foreach (Damager d in Object.FindObjectsByType<Damager>(FindObjectsSortMode.None))
        {
            if (d.GetComponent<Patrol>() == null) hazards++;
            Collider c = d.GetComponent<Collider>();
            if (c == null || !c.isTrigger) dmgSemTrigger++;
        }
        Check(sb, ref ok, ref falta, inimigos > 0, "Inimigos que se movem (" + inimigos + ")");
        Check(sb, ref ok, ref falta, hazards > 0, "Obstaculos que causam dano (" + hazards + ")");
        Check(sb, ref ok, ref falta, dmgSemTrigger == 0, "Inimigos/obstaculos com Collider em modo Trigger");

        GameObject agua = GameObject.Find("Agua");
        Check(sb, ref ok, ref falta, agua != null && agua.GetComponent<Water>() != null, "Agua com script Water");

        GameObject iglu = GameObject.Find("Igloo");
        bool igluOk = iglu != null && iglu.GetComponent<Igloo>() != null;
        Collider ic = iglu != null ? iglu.GetComponent<Collider>() : null;
        Check(sb, ref ok, ref falta, igluOk && ic != null && ic.isTrigger, "Iglu com script Igloo e Collider Trigger");

        Scene cena = EditorSceneManager.GetActiveScene();
        bool naBuild = false;
        foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
        {
            if (s.path == cena.path && s.enabled) naBuild = true;
        }
        Check(sb, ref ok, ref falta, !string.IsNullOrEmpty(cena.path) && naBuild, "Cena salva e na Build Settings");
        Check(sb, ref ok, ref falta, gm != null && gm.collectSound != null && gm.hurtSound != null && gm.winSound != null, "Sons ligados no GameManager");

        string resumo = "OK: " + ok + "   |   FALTA: " + falta + "\n\n" + sb.ToString();
        Debug.Log("P-Gelados - CHECKLIST\n" + resumo);
        EditorUtility.DisplayDialog("P-Gelados - Checklist", resumo, "OK");
    }

    static void Check(System.Text.StringBuilder sb, ref int ok, ref int falta, bool condicao, string texto)
    {
        if (condicao) { ok++; sb.AppendLine("[OK]    " + texto); }
        else { falta++; sb.AppendLine("[FALTA] " + texto); }
    }

    // =====================================================================
    // 5) PAREDES INVISIVEIS (ligar/desligar o desenho dos cubos)
    // =====================================================================
    [MenuItem("P-Gelados/5) Alternar visual das paredes invisiveis", false, 5)]
    static void Paredes()
    {
        GameObject p = GameObject.Find("ParedesInvisiveis");
        if (p == null)
        {
            Debug.LogWarning("Objeto 'ParedesInvisiveis' nao encontrado.");
            return;
        }
        Renderer[] rs = p.GetComponentsInChildren<Renderer>(true);
        bool algumVisivel = false;
        for (int i = 0; i < rs.Length; i++) if (rs[i].enabled) algumVisivel = true;
        for (int i = 0; i < rs.Length; i++)
        {
            Undo.RecordObject(rs[i], "Paredes");
            rs[i].enabled = !algumVisivel;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log(algumVisivel ? "Paredes escondidas (colisao continua)." : "Paredes visiveis.");
    }

    // =====================================================================
    // Helpers
    // =====================================================================
    static Transform GetGroup(string name)
    {
        GameObject g = GameObject.Find(name);
        if (g == null)
        {
            g = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(g, "Criar grupo");
        }
        return g.transform;
    }

    static Transform ResetGroup(string name)
    {
        Transform g = GetGroup(name);
        for (int i = g.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(g.GetChild(i).gameObject);
        return g;
    }

    static void MoveTo(string objName, Transform parent)
    {
        GameObject go = GameObject.Find(objName);
        if (go != null && go.transform.parent == null)
            Undo.SetTransformParent(go.transform, parent, "Organizar");
    }

    static void SetTrigger(GameObject go)
    {
        foreach (Collider c in go.GetComponents<Collider>())
        {
            Undo.RecordObject(c, "Trigger");
            c.isTrigger = true;
        }
    }

    static void AssignClip(ref AudioClip field, string file)
    {
        if (field != null) return;
        AudioClip c = AssetDatabase.LoadAssetAtPath<AudioClip>(AudioFolder + "/" + file + ".wav");
        if (c != null) field = c;
    }

    static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            int slash = path.LastIndexOf('/');
            AssetDatabase.CreateFolder(path.Substring(0, slash), path.Substring(slash + 1));
        }
    }
}
