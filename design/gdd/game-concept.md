# Game Concept: Pulverize

*Created: 2026-10-02*
*Status: Draft*

---

## Elevator Pitch

> Pulverize là một survivor-like roguelite: bạn di chuyển bằng một ngón tay, vũ khí tự bắn, và mọi enemy, mọi vật thể bị bắn trúng đều vỡ vụn thành hàng trăm hạt. Chính những hạt đó là XP bạn hút về để chọn Core, ghép build và đi từ yếu ớt thành một cỗ máy nghiền nát cả màn hình.

---

## Core Identity

| Aspect | Detail |
| ---- | ---- |
| **Genre** | Action roguelite, survivor-like (auto-shooter, horde) |
| **Platform** | Multi-platform: Mobile (iOS / Android) + PC (Steam). Performance budget đặt theo mobile. |
| **Target Audience** | Mid-core / casual action player thích power fantasy — xem Target Player Profile |
| **Player Count** | Single-player |
| **Session Length** | Một run 10–15 phút; session 1–3 run |
| **Monetization** | Chưa chốt — open question (F2P hybrid trên mobile vs premium trên PC) |
| **Estimated Scope** | Large (15–18 tháng, team size TBD) |
| **Comparable Titles** | Vampire Survivors, Survivor.io, Brotato |

---

## Core Fantasy

Bạn bắt đầu như một thứ nhỏ bé, yếu ớt giữa một đàn quái đông nghịt — và kết thúc run như một lực lượng phá huỷ mà không gì đứng vững được trước mặt. Điểm khác biệt là sức mạnh không chỉ hiện trên con số: nó hiện ra ở việc *màn hình tan rã* — càng mạnh, thế giới quanh bạn vỡ càng nhanh, càng nhiều, càng đẹp, và mỗi tiếng vỡ đều đã tai.

---

## Unique Hook

Giống Vampire Survivors, **và** destruction chính là economy: mọi thứ bị bắn vỡ thành hạt, và hạt là XP. Phá càng đã thì lên cấp càng nhanh — vòng thoả mãn (cú vỡ) và vòng progression (cú thu) là một.

Hook này ảnh hưởng trực tiếp gameplay (vị trí di chuyển để gom hạt, chọn Core theo cách phá), không chỉ là lớp aesthetic, và đọc được trong 3 giây trên video.

---

## Player Experience Analysis (MDA Framework)

### Target Aesthetics (What the player FEELS)

| Aesthetic | Priority | How We Deliver It |
| ---- | ---- | ---- |
| **Sensation** (sensory pleasure) | 1 | Feedback nhiều lớp mỗi hit: hit-stop, flash, crack, shatter particle, impact audio có bass, ASMR collect chain, haptics |
| **Submission** (relaxation, comfort zone) | 2 | Input tối giản (auto-fire, chỉ di chuyển), nhịp horde đều, flow state không cần suy nghĩ cơ học |
| **Fantasy** (make-believe, role-playing) | 3 | Power fantasy "yếu → không thể cản" thể hiện trực quan qua quy mô destruction |
| **Expression** (self-expression, creativity) | 4 | Build variety qua Core Selection và Fusion |
| **Discovery** (exploration, secrets) | 5 | Khám phá công thức Fusion, codex Core |
| **Challenge** (obstacle course, mastery) | 6 | Tier khó cao hơn ở meta; không phải trọng tâm |
| **Narrative** (drama, story arc) | N/A | Chỉ flavor text tối thiểu |
| **Fellowship** (social connection) | N/A | Ngoài leaderboard, không có social |

### Key Dynamics (Emergent player behaviors)

- Người chơi chủ động lướt qua "biển hạt" để gom XP, chấp nhận rủi ro đổi lấy tốc độ lên cấp.
- Người chơi thử nghiệm tổ hợp Core để tìm Fusion, và chia sẻ build mạnh.
- Người chơi nhắm vào prop phá được vì biết bên trong có loot — tạo mục tiêu di chuyển ngoài việc né.
- Người chơi "chơi thêm một run" để hoàn thành một Fusion chưa từng thấy.

### Core Mechanics (Systems we build)

1. **Auto-combat + Movement** — di chuyển một ngón/một stick; vũ khí tự bắn theo targeting rule.
2. **Shatter & Collect** — enemy/prop vỡ thành hạt vật lý nhẹ; hạt bị hút về người chơi trong bán kính magnet và quy đổi thành XP.
3. **Core Selection & Fusion** — lên cấp chọn 1/3 Core hoặc upgrade; hai Core đủ điều kiện fuse thành dạng tiến hoá.
4. **Horde Director** — điều phối wave, Elite, boss theo thời gian run; độ khó tăng bằng số lượng chứ không bằng HP.
5. **Meta Progression** — Shard mang về sau run → permanent upgrade, unlock Core, nhân vật, arena.

---

## Player Motivation Profile

### Primary Psychological Needs Served

| Need | How This Game Satisfies It | Strength |
| ---- | ---- | ---- |
| **Autonomy** (freedom, meaningful choice) | Chọn Core/Fusion định hình build; chọn nơi di chuyển, gom hạt, phá prop | Core |
| **Competence** (mastery, skill growth) | Hiểu synergy, sống lâu hơn, vượt tier khó; power growth rõ ràng trong run | Core |
| **Relatedness** (connection, belonging) | Codex/collection, leaderboard | Minimal |

### Player Type Appeal (Bartle Taxonomy)

- [x] **Achievers** (goal completion, collection, progression) — How: power growth trong run, meta unlock, hoàn thành codex Fusion
- [x] **Explorers** (discovery, understanding systems, finding secrets) — How: khám phá công thức Fusion và tương tác giữa Core
- [ ] **Socializers** (relationships, cooperation, community) — How: không phục vụ
- [ ] **Killers/Competitors** (domination, PvP, leaderboards) — How: chỉ leaderboard nhẹ, không PvP

Theo Quantic Foundry: motivation chính là *Destruction* và *Power*, phụ là *Completion* và *Discovery*.

### Flow State Design

- **Onboarding curve**: 60 giây đầu run đầu tiên chỉ có di chuyển và một vũ khí; level-up đầu tiên đến trong ~15 giây để dạy Core choice ngay. Không có tutorial text — feedback tự dạy.
- **Difficulty scaling**: Mật độ enemy và loại wave tăng theo thời gian run; tier khó ở meta mở sau khi clear. Không dùng HP-sponge.
- **Feedback clarity**: Quy mô destruction và tốc độ màn hình "tan" là thước đo trực quan của sức mạnh; số DPS chỉ là phụ.
- **Recovery from failure**: Thua → màn kết quả với Shard nhận được → run mới trong dưới 10 giây. Thất bại vẫn cho tiến triển meta.

---

## Core Loop

### Moment-to-Moment (30 seconds)

Di chuyển, vũ khí tự bắn. Enemy trúng đạn: flash → nứt → vỡ thành hàng trăm hạt (hit-stop vài frame, screen shake nhẹ, impact có bass). Hạt bị hút về người chơi với tiếng "tink" tăng pitch theo chuỗi (ASMR). Mỗi kill cho hai lần thoả mãn liền nhau: cú vỡ (sensation) và cú thu (gain) — căng–xả ngắn và liên tục. Action phải thoả mãn ngay cả khi tắt progression.

### Short-Term (5-15 minutes)

Đầu run lên cấp mỗi ~20–40 giây → chọn 1/3 Core hoặc upgrade. Mỗi ~2 phút có Elite rơi Core chest. Hai Core đủ điều kiện → Fusion. "Một chút nữa" nằm ở khoảnh khắc chọn Core và ở Fusion chỉ còn thiếu một bước. Run kết thúc bằng boss **Monolith** — một khối khổng lồ bị bóc vỡ từng lớp.

### Session-Level (30-120 minutes)

Session trên mobile là 1–3 run (15–45 phút); PC có thể dài hơn với endless mode. Điểm dừng tự nhiên là cuối mỗi run. Lý do quay lại: công thức Fusion chưa thử, upgrade meta vừa đủ Shard.

### Long-Term Progression

Shard → permanent upgrade (stat nền, magnet, reroll), unlock Core mới vào pool, nhân vật/vũ khí khởi đầu mới, arena mới với loại prop và enemy riêng. Game "xong" khi khám phá hết Fusion và vượt tier khó cao nhất.

### Retention Hooks

- **Curiosity**: Fusion chưa khám phá (hiện dạng silhouette trong codex), arena chưa mở
- **Investment**: Meta upgrade tích luỹ, bộ sưu tập Core
- **Social**: Leaderboard theo arena/tier (nhẹ)
- **Mastery**: Tier khó cao hơn, thời gian clear boss

---

## Game Pillars

### Pillar 1: Every Hit Lands
Mọi action đều có feedback nhiều lớp — audio, VFX, hit-stop, hit reaction, haptics.

*Design test*: Nếu phân vân giữa polish feedback cho vũ khí hiện có và thêm vũ khí mới, chọn polish.

### Pillar 2: Destruction Is the Reward
Phá vỡ trực tiếp nuôi progression — thứ bị phá là nguồn reward chính.

*Design test*: Nếu phân vân giữa reward qua UI/chest và reward rơi ra từ thứ người chơi vừa phá, chọn thứ bị phá.

### Pillar 3: Weak to Unstoppable
Power curve phải thấy được bằng mắt ngay trong một run.

*Design test*: Nếu phân vân giữa cân bằng chặt và cảm giác mạnh, cuối run nghiêng về OP; độ khó tăng bằng số lượng, không bằng HP.

### Pillar 4: One Thumb, Deep Build
Input tối giản; chiều sâu nằm ở lựa chọn build.

*Design test*: Nếu phân vân giữa thêm nút skill chủ động và thêm một tầng Core choice, chọn Core choice.

*Tension có chủ đích*: Every Hit Lands ↔ khả năng đọc màn hình khi hỗn loạn; Weak to Unstoppable ↔ challenge; Deep Build ↔ pick-up đơn giản.

### Anti-Pillars (What This Game Is NOT)

- **NOT manual aiming / control phức tạp**: phá One Thumb, Deep Build và flow.
- **NOT HP-sponge enemy**: enemy phải bắn rất lâu mới chết phá Every Hit Lands và Weak to Unstoppable.
- **NOT narrative nặng / cutscene**: cắt flow; story chỉ là flavor.
- **NOT PvP / realtime multiplayer**: scope và phá flow single-player.

---

## Visual Identity Anchor

*Đề xuất trong brainstorm (AD-CONCEPT-VISUAL skipped — Lean mode); `/art-bible` sẽ hoàn thiện.*

**Direction: Crystal Shatter**

**Visual rule**: Chỉ thứ phá được mới được phát sáng.

**Supporting principles**:
1. **Breakables pop, background recedes** — arena dùng value thấp, saturation thấp; enemy và prop phá được dùng vật liệu tinh thể/chunk sáng, rõ silhouette.
   *Design test*: Nếu một asset không phá được mà tranh sự chú ý, làm tối nó đi.
2. **Shard color is information** — màu hạt luôn mã hoá giá trị (XP thường / XP lớn / Shard / loot).
   *Design test*: Nếu một màu hạt chỉ để trang trí và không truyền thông tin, bỏ nó.
3. **Power is visible** — tier vũ khí và Fusion càng cao thì quy mô và cường độ VFX càng lớn, nhưng không che enemy.
   *Design test*: Nếu VFX mới che mất mối nguy, giảm alpha/scale trước khi giảm feedback audio.

**Color philosophy**: Nền lạnh, trầm, desaturated; hit, vỡ và hạt dùng màu ấm, bão hoà. Tương phản value giữa "thứ phá được" và "thứ không phá được" là ưu tiên số một cho readability khi màn hình hỗn loạn.

---

## Inspiration and References

| Reference | What We Take From It | What We Do Differently | Why It Matters |
| ---- | ---- | ---- | ---- |
| Vampire Survivors | Auto-fire, horde, level-up choice, evolution | Destruction là economy; feedback dày thay vì lo-fi | Chứng minh core loop survivor-like giữ chân người chơi |
| Survivor.io | Survivor-like trên mobile, run ngắn, one-thumb | Hook trực quan mạnh hơn cho creative; giới hạn rõ scope meta | Chứng minh genre chạy trên mobile |
| Brotato | Build synergy sâu, run ngắn, item tương tác | Synergy hiện qua Fusion trực quan thay vì bảng stat | Chứng minh chiều sâu build có thể nằm trong run ngắn |

**Non-game inspirations**: Video ASMR "satisfying" (kinetic sand cắt, kính vỡ, hydraulic press) — tham chiếu cho cảm giác vỡ và âm thanh; slow-motion destruction footage cho timing hit-stop.

---

## Target Player Profile

| Attribute | Detail |
| ---- | ---- |
| **Age range** | 18–40 |
| **Gaming experience** | Casual đến mid-core |
| **Time availability** | Run 10–15 phút lúc rảnh trên mobile; session dài hơn trên PC buổi tối |
| **Platform preference** | Mobile chính; PC (Steam) cho người chơi survivor-like |
| **Current games they play** | Vampire Survivors, Survivor.io, Brotato, Archero |
| **What they're looking for** | Cảm giác xả stress đã tay, power fantasy rõ ràng, build để thử mà không cần học control |
| **What would turn them away** | Control phức tạp, grind meta quá chậm, màn hình hỗn loạn đến mức không đọc được, enemy "trâu" |

---

## Technical Considerations

| Consideration | Assessment |
| ---- | ---- |
| **Recommended Engine** | Unity — user chọn; phù hợp multi-platform mobile + PC. Cân nhắc DOTS/Entities hoặc GPU instancing cho horde và hạt (quyết định ở ADR) |
| **Key Technical Challenges** | Hàng trăm enemy + hàng nghìn hạt cùng lúc trong budget mobile; shatter procedural hay pre-fractured; magnet/collect hiệu quả; haptics đa nền tảng; audio voice limit khi hàng trăm impact/giây |
| **Art Style** | 3D stylized (top-down/isometric camera), vật liệu tinh thể/chunk |
| **Art Pipeline Complexity** | Medium — enemy/prop low-poly cần pre-fracture hoặc shatter shader; VFX-heavy |
| **Audio Needs** | Adaptive — impact layering, pitch-chain collect, voice management; audio là pillar chứ không phải polish |
| **Networking** | None (leaderboard qua backend nhẹ) |
| **Content Volume** | Full vision ước tính: 4–5 arena, 40–60 Core, 15–20 Fusion, 6–8 nhân vật, 1 boss/arena |
| **Procedural Systems** | Wave/horde director theo thời gian; spawn prop theo rule trong arena hand-crafted |

---

## Risks and Open Questions

### Design Risks
- Màn hình tan rã quá nhiều làm mất readability của mối nguy — xung đột Every Hit Lands với flow.
- Survivor-like dễ "solved" khi người chơi tìm ra một build tối ưu — Fusion cần đủ đa dạng.
- Meta progression trên mobile có thể kéo về grind, phá cảm giác thoả mãn.

### Technical Risks
- Performance horde + particle trên thiết bị Android tầm thấp (feasibility chưa được đánh giá — TD-FEASIBILITY skipped, Lean mode).
- Audio: hàng trăm impact/giây vượt voice limit, cần hệ thống priority/ducking.
- Shatter chất lượng cao nhưng rẻ: pre-fractured mesh vs shader-based dissolve-to-particles.

### Market Risks
- Survivor-like đã bão hoà; hook destruction phải thể hiện rõ trên creative trong 3 giây.
- Định hướng monetization khác nhau giữa mobile (F2P) và PC (premium) có thể kéo thiết kế meta về hai hướng.

### Scope Risks
- Content Core/Fusion tăng tổ hợp test theo cấp số nhân.
- VFX và audio per-Core là chi phí sản xuất lớn nhất (scope chưa được producer đánh giá — PR-SCOPE skipped, Lean mode).

### Open Questions
- Shatter pipeline nào đạt cảm giác mong muốn trong budget mobile? → `/prototype shatter-and-collect` với stress test 300 enemy.
- Run 10–15 phút có phải độ dài đúng cho mobile không? → playtest prototype với timer khác nhau.
- Monetization model? → quyết định trước khi thiết kế meta progression GDD.
- Camera: top-down hay isometric, portrait hay landscape trên mobile? → quyết định cùng `/art-bible` và prototype.

---

## MVP Definition

**Core hypothesis**: Vòng "bắn vỡ → hút hạt → chọn Core" đủ thoả mãn để người chơi muốn chơi lại ngay sau một run 10 phút, chỉ với một arena.

**Required for MVP**:
1. Movement + auto-fire với 1 nhân vật
2. Shatter & Collect: enemy vỡ thành hạt, hạt = XP, magnet
3. Feedback stack đầy đủ cho hit/shatter/collect (hit-stop, VFX, audio, haptics)
4. Level-up Core Selection với ~8 Core và 2 Fusion
5. Horde director cho một run 10 phút, 1 loại Elite, 1 boss Monolith đơn giản
6. 1 arena với prop phá được

**Explicitly NOT in MVP** (defer to later):
- Meta progression, Shard, unlock
- Nhiều nhân vật, nhiều arena
- Leaderboard, monetization, IAP/IAA
- Endless mode PC

### Scope Tiers (if budget/time shrinks)

| Tier | Content | Features | Timeline |
| ---- | ---- | ---- | ---- |
| **MVP** | 1 arena, 1 nhân vật, ~8 Core, 2 Fusion | Core loop + feedback stack | 6–8 tuần |
| **Vertical Slice** | 1 arena hoàn chỉnh, 2 nhân vật, ~16 Core, 5 Fusion | Core + meta progression cơ bản, art/audio final quality | ~12 tuần (tháng 3–5) |
| **Alpha** | 4 arena placeholder, 40 Core | Toàn bộ feature, chưa polish | ~6 tháng (tháng 6–11) |
| **Full Vision** | 4–5 arena, 40–60 Core, 15–20 Fusion, 6–8 nhân vật | Polish, live tuning, multi-platform | 15–18 tháng tổng |

---

## Next Steps

- [ ] Get concept approval from creative-director
- [ ] Fill in CLAUDE.md technology stack based on engine choice (`/setup-engine`)
- [ ] Create game pillars document (`/design-review` to validate)
- [ ] **Prototype core idea** (`/prototype shatter-and-collect`) — before writing GDDs, validate the concept is worth designing
- [ ] If prototype PROCEEDS: Decompose concept into systems (`/map-systems`)
- [ ] Design each system (`/design-system [system-name]`) — use prototype learnings in Tuning Knobs and Formulas sections
- [ ] Build vertical slice in Pre-Production (`/vertical-slice`) — validate full game loop before committing to Production
- [ ] Validate core loop with playtest (`/playtest-report`)
- [ ] Plan first milestone (`/sprint-plan new`)
