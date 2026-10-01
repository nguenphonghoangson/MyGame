// PROTOTYPE - NOT FOR PRODUCTION
// Question: Is "shoot -> enemy shatters into shards -> shards magnetize back as XP" satisfying at scale?
// Date: 2026-10-02
using System;
using UnityEngine;

public class Pulverize : MonoBehaviour
{
    // ---- Tuning (all hardcoded on purpose) ----
    const float ArenaHalf = 30f;
    const float PlayerSpeed = 7f;
    const float PlayerMaxHp = 100f;
    const float RunSeconds = 300f;
    const int MaxEnemies = 900, MaxBullets = 700, MaxShards = 5000;
    const int ShardsPerKill = 10;
    const float HitStopTime = 0.03f, HitStopCooldown = 0.12f;
    const float ShakeAdd = 0.035f, ShakeMax = 0.5f;
    const float ChainWindow = 0.35f, TinkMinGap = 0.025f;
    static readonly int[] Scale = { 0, 2, 4, 7, 9, 12, 14, 16, 19 };

    enum State { Playing, LevelUp, Dead, Won }
    State state;

    // player
    Vector2 pPos; float pHp; int level; float xp; float need;
    float fireTimer; Transform playerVis;
    // stats
    float fireInterval, dmg, magnet, splash; int barrels, pierce;
    // enemies
    int en; float[] ex, ez, ehp, espd, eflash; bool[] edead;
    // bullets
    int bn; float[] bx, bz, bvx, bvz, blife; int[] bpierce, blast;
    // shards
    int sn; float[] sx, sy, sz, svx, svy, svz, sage; bool[] smag;
    // grid
    const int GridN = 32; const float Cell = 2f * ArenaHalf / GridN;
    int[] cellStart = new int[GridN * GridN + 1], cellFill = new int[GridN * GridN], cellList = new int[MaxEnemies];
    // timing / feel
    float runTime, spawnAcc, trauma, hitStopLeft, nextHitStop, chainT, lastTink; int chain; int killsThisFrame;
    // render
    Mesh cube; Material enemyMat, flashMat, shardMat, bulletMat, groundMat;
    Matrix4x4[] buf = new Matrix4x4[1023];
    Camera cam;
    // audio
    AudioSource[] pool; int poolI; AudioClip cTink, cHit, cKill, cShoot, cLevel, cHurt;
    // ui
    string[] coreNames = { "Rapid Fire  (+30% fire rate)", "Twin Barrel  (+1 projectile)", "Piercing  (+1 pierce)", "Heavy Rounds  (+50% damage)", "Magnet  (+60% pickup radius)", "Blast  (hits splash nearby)" };
    int[] offer = new int[3];
    float fps; GUIStyle big, small;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot() { new GameObject("Pulverize").AddComponent<Pulverize>(); }

    void Awake()
    {
        Application.targetFrameRate = 120;
        ex = new float[MaxEnemies]; ez = new float[MaxEnemies]; ehp = new float[MaxEnemies]; espd = new float[MaxEnemies]; eflash = new float[MaxEnemies]; edead = new bool[MaxEnemies];
        bx = new float[MaxBullets]; bz = new float[MaxBullets]; bvx = new float[MaxBullets]; bvz = new float[MaxBullets]; blife = new float[MaxBullets]; bpierce = new int[MaxBullets]; blast = new int[MaxBullets];
        sx = new float[MaxShards]; sy = new float[MaxShards]; sz = new float[MaxShards]; svx = new float[MaxShards]; svy = new float[MaxShards]; svz = new float[MaxShards]; sage = new float[MaxShards]; smag = new bool[MaxShards];

        var tmp = GameObject.CreatePrimitive(PrimitiveType.Cube); cube = tmp.GetComponent<MeshFilter>().sharedMesh; Destroy(tmp);
        enemyMat = Mat(new Color(0.15f, 0.9f, 1f), 1.4f);
        flashMat = Mat(Color.white, 3f);
        shardMat = Mat(new Color(1f, 0.6f, 0.1f), 2.2f);
        bulletMat = Mat(new Color(1f, 1f, 0.7f), 3f);
        groundMat = Mat(new Color(0.07f, 0.08f, 0.12f), 0f);

        var g = GameObject.CreatePrimitive(PrimitiveType.Plane); Destroy(g.GetComponent<Collider>());
        g.transform.localScale = Vector3.one * (ArenaHalf * 2f / 10f * 1.6f); g.GetComponent<Renderer>().sharedMaterial = groundMat;
        var p = GameObject.CreatePrimitive(PrimitiveType.Sphere); Destroy(p.GetComponent<Collider>());
        playerVis = p.transform; playerVis.localScale = Vector3.one * 1.1f; p.GetComponent<Renderer>().sharedMaterial = Mat(new Color(1f, 0.25f, 0.45f), 1.2f);

        cam = Camera.main;
        if (cam == null) { var c = new GameObject("Cam"); cam = c.AddComponent<Camera>(); c.tag = "MainCamera"; c.AddComponent<AudioListener>(); }
        cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = new Color(0.02f, 0.02f, 0.04f); cam.fieldOfView = 50f;
        RenderSettings.ambientLight = new Color(0.35f, 0.38f, 0.5f);
        var lightObj = FindObjectOfType<Light>();
        if (lightObj == null) { var l = new GameObject("Light").AddComponent<Light>(); l.type = LightType.Directional; l.transform.rotation = Quaternion.Euler(60, 20, 0); }

        pool = new AudioSource[16];
        for (int i = 0; i < pool.Length; i++) { pool[i] = gameObject.AddComponent<AudioSource>(); pool[i].spatialBlend = 0f; pool[i].playOnAwake = false; }
        BuildAudio();
        Restart();
    }

    Material Mat(Color c, float emit)
    {
        var m = new Material(Shader.Find("Standard")); m.enableInstancing = true; m.color = c; m.SetFloat("_Glossiness", 0.2f);
        if (emit > 0f) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", c * emit); }
        return m;
    }

    void Restart()
    {
        state = State.Playing; pPos = Vector2.zero; pHp = PlayerMaxHp; level = 1; xp = 0f; need = NeedFor(1);
        en = bn = sn = 0; runTime = 0f; spawnAcc = 0f; trauma = 0f; hitStopLeft = 0f; chain = 0; fireTimer = 0.2f;
        fireInterval = 0.45f; dmg = 2f; magnet = 4f; splash = 0f; barrels = 1; pierce = 0;
        Time.timeScale = 1f;
    }

    float NeedFor(int lv) { return 60f + 25f * lv; }

    // ---------------- Update ----------------
    void Update()
    {
        fps = Mathf.Lerp(fps, 1f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f), 0.05f);
        if ((state == State.Dead || state == State.Won) && Input.GetKeyDown(KeyCode.R)) Restart();
        if (state == State.LevelUp)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) PickCore(0); else if (Input.GetKeyDown(KeyCode.Alpha2)) PickCore(1); else if (Input.GetKeyDown(KeyCode.Alpha3)) PickCore(2);
        }
        if (state == State.Playing)
        {
            if (Input.GetKeyDown(KeyCode.F)) for (int i = 0; i < 100; i++) SpawnEnemy();
            Step(Time.deltaTime);
        }
        Render();
        Feel();
    }

    Vector2 mouseStart;
    Vector2 ReadMove()
    {
        Vector2 m = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetMouseButtonDown(0)) mouseStart = Input.mousePosition;
        if (Input.GetMouseButton(0)) { Vector2 d = (Vector2)Input.mousePosition - mouseStart; if (d.magnitude > 12f) m = d.normalized; }
        return m.sqrMagnitude > 1f ? m.normalized : m;
    }

    void Step(float dt)
    {
        runTime += dt; killsThisFrame = 0;
        if (runTime >= RunSeconds) { state = State.Won; Time.timeScale = 1f; return; }

        pPos += ReadMove() * PlayerSpeed * dt;
        pPos.x = Mathf.Clamp(pPos.x, -ArenaHalf, ArenaHalf); pPos.y = Mathf.Clamp(pPos.y, -ArenaHalf, ArenaHalf);

        // spawn: target count ramps with time
        float target = Mathf.Min(40f + runTime * 1.4f, 420f);
        spawnAcc += dt * (8f + runTime * 0.05f);
        while (spawnAcc >= 1f) { spawnAcc -= 1f; if (en < target) SpawnEnemy(); }

        // enemies
        float speedMul = 1f + runTime / 240f;
        float hurt = 0f;
        for (int i = 0; i < en; i++)
        {
            float dx = pPos.x - ex[i], dz = pPos.y - ez[i]; float d = Mathf.Sqrt(dx * dx + dz * dz) + 0.0001f;
            ex[i] += dx / d * espd[i] * speedMul * dt; ez[i] += dz / d * espd[i] * speedMul * dt;
            if (eflash[i] > 0f) eflash[i] -= dt;
            if (d < 0.95f) hurt += 9f;
        }
        if (hurt > 0f)
        {
            pHp -= hurt * dt; trauma = Mathf.Min(trauma + 0.6f * dt, ShakeMax);
            if (pHp <= 0f) { pHp = 0f; state = State.Dead; Time.timeScale = 1f; Play(cHurt, 1f, 0.8f); return; }
        }
        BuildGrid();

        // fire
        fireTimer -= dt;
        if (fireTimer <= 0f && en > 0)
        {
            int t = Nearest();
            if (t >= 0)
            {
                fireTimer = fireInterval;
                float ang = Mathf.Atan2(ez[t] - pPos.y, ex[t] - pPos.x);
                for (int k = 0; k < barrels; k++)
                {
                    float a = ang + (barrels == 1 ? 0f : (k - (barrels - 1) * 0.5f) * 0.18f);
                    SpawnBullet(a);
                }
                Play(cShoot, 0.35f, UnityEngine.Random.Range(0.92f, 1.08f));
            }
        }

        // bullets
        for (int i = bn - 1; i >= 0; i--)
        {
            bx[i] += bvx[i] * dt; bz[i] += bvz[i] * dt; blife[i] -= dt;
            bool die = blife[i] <= 0f || Mathf.Abs(bx[i]) > ArenaHalf + 4f || Mathf.Abs(bz[i]) > ArenaHalf + 4f;
            if (!die)
            {
                int hit = HitEnemy(bx[i], bz[i], 0.75f);
                if (hit >= 0)
                {
                    Damage(hit, dmg);
                    if (splash > 0f) Splash(ex[hit], ez[hit], splash, dmg * 0.6f, hit);
                    Play(cHit, 0.45f, UnityEngine.Random.Range(0.9f, 1.15f));
                    if (bpierce[i] > 0) { bpierce[i]--; bx[i] += bvx[i] * 0.02f; bz[i] += bvz[i] * 0.02f; } else die = true;
                }
            }
            if (die) { bn--; Copy(i, bn); }
        }

        // reap dead enemies (spawn shards)
        for (int i = en - 1; i >= 0; i--)
            if (edead[i]) { en--; ex[i] = ex[en]; ez[i] = ez[en]; ehp[i] = ehp[en]; espd[i] = espd[en]; eflash[i] = eflash[en]; edead[i] = edead[en]; edead[en] = false; }

        StepShards(dt);

        if (killsThisFrame > 0)
        {
            trauma = Mathf.Min(trauma + ShakeAdd * Mathf.Min(killsThisFrame, 4), ShakeMax);
            if (Time.unscaledTime >= nextHitStop) { hitStopLeft = HitStopTime; nextHitStop = Time.unscaledTime + HitStopCooldown; }
            Play(cKill, Mathf.Min(0.5f + 0.15f * killsThisFrame, 1f), UnityEngine.Random.Range(0.85f, 1.1f));
        }

        if (xp >= need)
        {
            xp -= need; level++; need = NeedFor(level);
            for (int i = 0; i < 3; i++) { int c; bool dup; do { c = UnityEngine.Random.Range(0, coreNames.Length); dup = false; for (int j = 0; j < i; j++) if (offer[j] == c) dup = true; } while (dup); offer[i] = c; }
            state = State.LevelUp; Time.timeScale = 0f; Play(cLevel, 0.8f, 1f);
        }
    }

    void Copy(int dst, int src) { bx[dst] = bx[src]; bz[dst] = bz[src]; bvx[dst] = bvx[src]; bvz[dst] = bvz[src]; blife[dst] = blife[src]; bpierce[dst] = bpierce[src]; blast[dst] = blast[src]; }

    void PickCore(int slot)
    {
        switch (offer[slot])
        {
            case 0: fireInterval *= 0.7f; break;
            case 1: barrels++; break;
            case 2: pierce++; break;
            case 3: dmg *= 1.5f; break;
            case 4: magnet *= 1.6f; break;
            case 5: splash += 1.6f; break;
        }
        state = State.Playing; Time.timeScale = 1f;
    }

    // ---------------- Enemies / bullets ----------------
    void SpawnEnemy()
    {
        if (en >= MaxEnemies) return;
        float a = UnityEngine.Random.value * Mathf.PI * 2f, r = 20f;
        ex[en] = Mathf.Clamp(pPos.x + Mathf.Cos(a) * r, -ArenaHalf, ArenaHalf); ez[en] = Mathf.Clamp(pPos.y + Mathf.Sin(a) * r, -ArenaHalf, ArenaHalf);
        ehp[en] = 3f + runTime / 40f; espd[en] = UnityEngine.Random.Range(1.8f, 3.2f); eflash[en] = 0f; edead[en] = false; en++;
    }

    void SpawnBullet(float a)
    {
        if (bn >= MaxBullets) return;
        bx[bn] = pPos.x; bz[bn] = pPos.y; bvx[bn] = Mathf.Cos(a) * 26f; bvz[bn] = Mathf.Sin(a) * 26f; blife[bn] = 1.1f; bpierce[bn] = pierce; bn++;
    }

    int Nearest()
    {
        int best = -1; float bd = 1e9f;
        for (int i = 0; i < en; i++) { if (edead[i]) continue; float dx = ex[i] - pPos.x, dz = ez[i] - pPos.y, d = dx * dx + dz * dz; if (d < bd) { bd = d; best = i; } }
        return best;
    }

    void BuildGrid()
    {
        Array.Clear(cellFill, 0, cellFill.Length); Array.Clear(cellStart, 0, cellStart.Length);
        for (int i = 0; i < en; i++) cellStart[CellOf(ex[i], ez[i]) + 1]++;
        for (int c = 0; c < GridN * GridN; c++) cellStart[c + 1] += cellStart[c];
        for (int i = 0; i < en; i++) { int c = CellOf(ex[i], ez[i]); cellList[cellStart[c] + cellFill[c]++] = i; }
    }

    int CellOf(float x, float z)
    {
        int cx = Mathf.Clamp((int)((x + ArenaHalf) / Cell), 0, GridN - 1), cz = Mathf.Clamp((int)((z + ArenaHalf) / Cell), 0, GridN - 1);
        return cz * GridN + cx;
    }

    int HitEnemy(float x, float z, float r)
    {
        int cx = Mathf.Clamp((int)((x + ArenaHalf) / Cell), 0, GridN - 1), cz = Mathf.Clamp((int)((z + ArenaHalf) / Cell), 0, GridN - 1);
        float r2 = r * r;
        for (int oz = -1; oz <= 1; oz++) for (int ox = -1; ox <= 1; ox++)
        {
            int gx = cx + ox, gz = cz + oz; if (gx < 0 || gz < 0 || gx >= GridN || gz >= GridN) continue;
            int c = gz * GridN + gx;
            for (int k = cellStart[c]; k < cellStart[c + 1]; k++)
            {
                int e = cellList[k]; if (edead[e]) continue;
                float dx = ex[e] - x, dz = ez[e] - z; if (dx * dx + dz * dz < r2) return e;
            }
        }
        return -1;
    }

    void Splash(float x, float z, float radius, float d, int skip)
    {
        int cx = Mathf.Clamp((int)((x + ArenaHalf) / Cell), 0, GridN - 1), cz = Mathf.Clamp((int)((z + ArenaHalf) / Cell), 0, GridN - 1);
        int span = Mathf.CeilToInt(radius / Cell); float r2 = radius * radius;
        for (int oz = -span; oz <= span; oz++) for (int ox = -span; ox <= span; ox++)
        {
            int gx = cx + ox, gz = cz + oz; if (gx < 0 || gz < 0 || gx >= GridN || gz >= GridN) continue;
            int c = gz * GridN + gx;
            for (int k = cellStart[c]; k < cellStart[c + 1]; k++)
            {
                int e = cellList[k]; if (e == skip || edead[e]) continue;
                float dx = ex[e] - x, dz = ez[e] - z; if (dx * dx + dz * dz < r2) Damage(e, d);
            }
        }
    }

    void Damage(int e, float d)
    {
        if (edead[e]) return;
        ehp[e] -= d; eflash[e] = 0.08f;
        if (ehp[e] <= 0f) { edead[e] = true; killsThisFrame++; Shatter(ex[e], ez[e]); }
    }

    // ---------------- Shards ----------------
    void Shatter(float x, float z)
    {
        for (int k = 0; k < ShardsPerKill; k++)
        {
            if (sn >= MaxShards) { xp += 1f; continue; } // full: pay out directly, no visual
            float a = UnityEngine.Random.value * Mathf.PI * 2f, sp = UnityEngine.Random.Range(2f, 8f);
            sx[sn] = x; sy[sn] = 0.6f; sz[sn] = z; svx[sn] = Mathf.Cos(a) * sp; svz[sn] = Mathf.Sin(a) * sp; svy[sn] = UnityEngine.Random.Range(3f, 7f); sage[sn] = 0f; smag[sn] = false; sn++;
        }
    }

    void StepShards(float dt)
    {
        float m2 = magnet * magnet;
        for (int i = sn - 1; i >= 0; i--)
        {
            sage[i] += dt;
            float dx = pPos.x - sx[i], dz = pPos.y - sz[i], d2 = dx * dx + dz * dz;
            if (!smag[i] && sage[i] > 0.35f && (d2 < m2 || sage[i] > 4f)) smag[i] = true;
            if (smag[i])
            {
                float d = Mathf.Sqrt(d2) + 0.0001f, speed = 12f + (sage[i] - 0.35f) * 14f; speed = Mathf.Min(speed, 38f);
                float k = 1f - Mathf.Exp(-14f * dt);
                svx[i] = Mathf.Lerp(svx[i], dx / d * speed, k); svz[i] = Mathf.Lerp(svz[i], dz / d * speed, k); svy[i] = Mathf.Lerp(svy[i], (0.6f - sy[i]) * 6f, k);
                if (d < 0.6f) { Collect(); sn--; CopyShard(i, sn); continue; }
            }
            else
            {
                svy[i] -= 22f * dt;
                svx[i] *= 1f - 3f * dt; svz[i] *= 1f - 3f * dt;
            }
            sx[i] += svx[i] * dt; sy[i] += svy[i] * dt; sz[i] += svz[i] * dt;
            if (sy[i] < 0.12f) { sy[i] = 0.12f; svy[i] = Mathf.Abs(svy[i]) > 1.5f ? -svy[i] * 0.35f : 0f; }
        }
    }

    void CopyShard(int dst, int src) { sx[dst] = sx[src]; sy[dst] = sy[src]; sz[dst] = sz[src]; svx[dst] = svx[src]; svy[dst] = svy[src]; svz[dst] = svz[src]; sage[dst] = sage[src]; smag[dst] = smag[src]; }

    void Collect()
    {
        xp += 1f;
        if (Time.unscaledTime - chainT > ChainWindow) chain = 0;
        chainT = Time.unscaledTime; chain++;
        if (Time.unscaledTime - lastTink < TinkMinGap) return;
        lastTink = Time.unscaledTime;
        int step = Scale[Mathf.Min(chain / 3, Scale.Length - 1)];
        Play(cTink, 0.35f, Mathf.Pow(2f, step / 12f));
    }

    // ---------------- Feel ----------------
    void Feel()
    {
        if (state == State.Playing)
        {
            if (hitStopLeft > 0f) { hitStopLeft -= Time.unscaledDeltaTime; Time.timeScale = 0.04f; }
            else Time.timeScale = 1f;
        }
        trauma = Mathf.Max(0f, trauma - 1.8f * Time.unscaledDeltaTime);
        float s = trauma * trauma;
        Vector3 shake = new Vector3(UnityEngine.Random.value - 0.5f, 0f, UnityEngine.Random.value - 0.5f) * 2f * s * 1.4f;
        Vector3 focus = new Vector3(pPos.x, 0f, pPos.y);
        cam.transform.position = focus + new Vector3(0f, 22f, -13f) + shake;
        cam.transform.rotation = Quaternion.LookRotation(focus - cam.transform.position + shake, Vector3.up);
        playerVis.position = new Vector3(pPos.x, 0.6f, pPos.y);
    }

    void Play(AudioClip c, float vol, float pitch)
    {
        var a = pool[poolI]; poolI = (poolI + 1) % pool.Length;
        a.clip = c; a.volume = vol; a.pitch = pitch; a.Play();
    }

    // ---------------- Render ----------------
    void Render()
    {
        int n = 0;
        for (int i = 0; i < en; i++) { if (eflash[i] > 0f) continue; buf[n++] = Matrix4x4.TRS(new Vector3(ex[i], 0.5f, ez[i]), Quaternion.identity, Vector3.one * 0.9f); if (n == 1023) { Graphics.DrawMeshInstanced(cube, 0, enemyMat, buf, n); n = 0; } }
        if (n > 0) Graphics.DrawMeshInstanced(cube, 0, enemyMat, buf, n);
        n = 0;
        for (int i = 0; i < en; i++) { if (eflash[i] <= 0f) continue; buf[n++] = Matrix4x4.TRS(new Vector3(ex[i], 0.5f, ez[i]), Quaternion.identity, Vector3.one * 1.05f); if (n == 1023) { Graphics.DrawMeshInstanced(cube, 0, flashMat, buf, n); n = 0; } }
        if (n > 0) Graphics.DrawMeshInstanced(cube, 0, flashMat, buf, n);
        n = 0;
        for (int i = 0; i < sn; i++) { float sc = 0.16f + (i % 5) * 0.03f; buf[n++] = Matrix4x4.TRS(new Vector3(sx[i], sy[i], sz[i]), Quaternion.Euler(sage[i] * 180f, i * 37f, 0f), Vector3.one * sc); if (n == 1023) { Graphics.DrawMeshInstanced(cube, 0, shardMat, buf, n); n = 0; } }
        if (n > 0) Graphics.DrawMeshInstanced(cube, 0, shardMat, buf, n);
        n = 0;
        for (int i = 0; i < bn; i++) { buf[n++] = Matrix4x4.TRS(new Vector3(bx[i], 0.6f, bz[i]), Quaternion.LookRotation(new Vector3(bvx[i], 0f, bvz[i])), new Vector3(0.14f, 0.14f, 0.7f)); if (n == 1023) { Graphics.DrawMeshInstanced(cube, 0, bulletMat, buf, n); n = 0; } }
        if (n > 0) Graphics.DrawMeshInstanced(cube, 0, bulletMat, buf, n);
    }

    // ---------------- UI ----------------
    void OnGUI()
    {
        if (big == null) { big = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(Screen.height * 0.045f), fontStyle = FontStyle.Bold }; small = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(Screen.height * 0.024f) }; big.normal.textColor = Color.white; small.normal.textColor = new Color(0.85f, 0.9f, 1f); }
        float w = Screen.width, h = Screen.height;
        GUI.Label(new Rect(12, 8, w, h * 0.04f), "FPS " + Mathf.RoundToInt(fps) + "   enemies " + en + "   shards " + sn + "   chain " + chain + "   [F] +100 enemies", small);
        GUI.Label(new Rect(12, h * 0.04f, w, h * 0.05f), "Lv " + level + "   HP " + Mathf.CeilToInt(pHp) + "   " + FormatTime(runTime) + " / " + FormatTime(RunSeconds), big);
        GUI.color = new Color(0.1f, 0.1f, 0.15f, 0.9f); GUI.DrawTexture(new Rect(0, h - 14, w, 14), Texture2D.whiteTexture);
        GUI.color = new Color(1f, 0.7f, 0.15f); GUI.DrawTexture(new Rect(0, h - 14, w * Mathf.Clamp01(xp / need), 14), Texture2D.whiteTexture); GUI.color = Color.white;

        if (state == State.LevelUp)
        {
            GUI.Label(new Rect(w * 0.5f - 170, h * 0.22f, 600, 60), "LEVEL UP - choose a Core (1/2/3)", big);
            for (int i = 0; i < 3; i++)
                if (GUI.Button(new Rect(w * 0.5f - 230, h * 0.34f + i * h * 0.12f, 460, h * 0.1f), (i + 1) + ".  " + coreNames[offer[i]], big)) PickCore(i);
        }
        else if (state == State.Dead) GUI.Label(new Rect(w * 0.5f - 260, h * 0.4f, 700, 80), "DESTROYED at " + FormatTime(runTime) + "   -   press R", big);
        else if (state == State.Won) GUI.Label(new Rect(w * 0.5f - 240, h * 0.4f, 700, 80), "SURVIVED 5:00   -   press R", big);
    }

    static string FormatTime(float t) { int s = Mathf.FloorToInt(t); return (s / 60) + ":" + (s % 60).ToString("00"); }

    // ---------------- Procedural audio ----------------
    delegate float Gen(float t);

    AudioClip Clip(string name, float len, Gen g)
    {
        const int sr = 22050; int n = Mathf.CeilToInt(len * sr); var data = new float[n];
        for (int i = 0; i < n; i++) data[i] = Mathf.Clamp(g(i / (float)sr), -1f, 1f);
        var c = AudioClip.Create(name, n, 1, sr, false); c.SetData(data, 0); return c;
    }

    void BuildAudio()
    {
        var rng = new System.Random(7);
        Func<float> noise = () => (float)(rng.NextDouble() * 2.0 - 1.0);
        const float TwoPi = Mathf.PI * 2f;
        cTink = Clip("tink", 0.14f, t => (Mathf.Sin(TwoPi * 660f * t) * 0.8f + Mathf.Sin(TwoPi * 1320f * t) * 0.35f) * Mathf.Exp(-t * 34f));
        cHit = Clip("hit", 0.07f, t => (noise() * 0.5f * Mathf.Exp(-t * 90f)) + Mathf.Sin(TwoPi * 240f * t) * Mathf.Exp(-t * 55f) * 0.7f);
        float ph = 0f, g1 = 3100f, g2 = 4700f, g3 = 5900f;
        cKill = Clip("kill", 0.28f, t =>
        {
            ph += TwoPi * (60f + 110f * Mathf.Exp(-t * 14f)) / 22050f;
            float thump = Mathf.Sin(ph) * Mathf.Exp(-t * 11f) * 0.9f;
            float crack = noise() * Mathf.Exp(-t * 26f) * 0.55f;
            float glass = (Mathf.Sin(TwoPi * g1 * t) + Mathf.Sin(TwoPi * g2 * t) * 0.7f + Mathf.Sin(TwoPi * g3 * t) * 0.5f) * Mathf.Exp(-t * 22f) * 0.22f;
            return thump + crack + glass;
        });
        float ph2 = 0f;
        cShoot = Clip("shoot", 0.09f, t => { ph2 += TwoPi * (950f - 7000f * t) / 22050f; return Mathf.Sin(ph2) * Mathf.Exp(-t * 28f) * 0.6f; });
        cLevel = Clip("level", 0.6f, t =>
        {
            float[] f = { 523f, 659f, 784f, 1046f }; int k = Mathf.Min((int)(t / 0.1f), 3); float lt = t - k * 0.1f;
            return Mathf.Sin(TwoPi * f[k] * t) * Mathf.Exp(-lt * 8f) * 0.6f * Mathf.Exp(-t * 2f);
        });
        float ph3 = 0f;
        cHurt = Clip("hurt", 0.4f, t => { ph3 += TwoPi * (90f - 60f * t) / 22050f; return Mathf.Sin(ph3) * Mathf.Exp(-t * 7f) * 0.9f + noise() * Mathf.Exp(-t * 30f) * 0.3f; });
    }
}
