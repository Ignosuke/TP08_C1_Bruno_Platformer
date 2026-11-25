# Corrección, Trabajo Práctico N°08, Platformer 2D

- **Alumno/a:** Bruno (usuario de GitHub `Ignosuke`)
- **Entrega:** `TP08_C1_Bruno_Platformer`
- **Materia:** Programación con Motores de Videojuegos I
- **Nota TP08:** 9
- Observación: Al momento de corregir el repositorio era privado. Hacelo público, es parte de las pautas.

## 1. Checklist de requisitos

### General

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| G1 | Repositorio `TP08_C1_Apellido_Titulo` | ✓ | `.git/config` apunta a `https://github.com/Ignosuke/TP08_C1_Bruno_Platformer` |
| G2 | Proyecto de Unity con el mismo nombre | ✓ | La raíz del repo es el proyecto (`Assets/`, `ProjectSettings/`). `productName: The Old Blade` en `ProjectSettings/ProjectSettings.asset:16` es el nombre del build |
| G3 | Al menos 2 escenas: MainMenu y Gameplay | ✓ | `Assets/Scenes/MainMenu.unity` y `Assets/Scenes/Gameplay.unity` (más `Splash.unity`), las tres en `ProjectSettings/EditorBuildSettings.asset` |
| V1 | Publicado en Itch.io y presentable | ✓ | Publicado. Lo revisé en itch.io el 2026-11-16 |
| V2 | Itch.io jugable en web y descargable para Windows | ✓ | Carga la build web y tiene descarga para Windows |

### Jugador

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| J1 | Se mueve a derecha e izquierda | ✓ | `Assets/Scripts/Player/PlayerMover.cs:60-75`, con la velocidad aplicada en `FixedUpdate` |
| J2 | Salta | ✓ | `Assets/Scripts/Player/PlayerJumper.cs:111-131`, y el doble salto en `AirJump` (`PlayerJumper.cs:133-141`) |
| J3 | Puede dañar enemigos | ✓ | `PlayerAttacker.cs:206-212` llama a `MeleeHitbox.Hit` (`Assets/Scripts/Shared/MeleeHitbox.cs:33-57`); el shuriken en `Shuriken.cs:58-75` |
| J4 | Barra de vida | ✓ | La fila de corazones `IconRow` está conectada en `Assets/Scripts/UI/PlayerHud.cs:12-29` a `PlayerHealth.OnHealthChanged` (corazones y medios corazones, como las máscaras de Hollow Knight) |
| J5 | Puede curarse | ✓ | `PlayerHealer.cs:86-105` gasta una carga y cura `healAmount` al terminar; el pickup `Heal` está en `Pickup.cs:37-39`, y los checkpoints llenan la vida (`Checkpoint.cs:21`) |
| J6 | Todas sus acciones animadas y con sonido | Parcial | Animás Idle, Run, JumpStart/Rise/Apex/Fall, Dash, Attack1-3, AirAttack, Defend, Hurt, Death, Healing y Throw (`Player.controller`). Tienen sonido (`PlayerAudio.cs:42-83`, clips en `PlayerSo.asset`) el salto, el doble salto, el aterrizaje, el dash, los ataques, el impacto, la defensa, el parry, el daño, la curación y el lanzamiento. Correr no tiene sonido (no hay clip de pasos en `PlayerSo` ni Animation Event en `PlayerRun.anim`) y la muerte solo reproduce el clip de daño del golpe letal |

### Enemigos

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| E1 | Dañan al jugador | ✓ | `MeleeAttack.cs:20-27` (Oni, Executioner y jefe), flechas en `RangedAttack.cs:28-36` y `Arrow.cs:88-117`, daño de contacto en `EnemyContactDamage.cs` |
| E2 | Poseen barra de vida | ✓ | `EnemyHealthBar.cs` en los prefabs `Oni`, `Executioner` y `Archer`; el jefe usa `BossHealthBar.cs` en el HUD (`Image` con `fillAmount`) |
| E3 | Feedback visual al morir (partículas + animación) | ✓ | `EnemyDeathEffect.cs:19-22` reproduce el `ParticleSystem` en `OnDied`; estado `Death` en `EnemyAnimator.cs:49`; además sueltan monedas (`EnemyCoinDrop.cs`) y tienen destello (`EnemyHitFlash.cs`) |
| E4 | Todas sus acciones animadas y con sonido | Parcial | Animás Idle, Walk, Attack, Hurt y Death (y el Dash del arquero, el Shout y la fase Flaming del jefe). `EnemyAudio.cs:24-45` suena en ataque, daño y muerte, y `BossAudio.cs` en el grito. Caminar no tiene sonido en ningún enemigo, y el dash del arquero tampoco (`EnemySo` solo tiene `attackClip`, `hurtClip` y `deathClip`) |

### Pickables

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| K1 | Heal | ✓ | `Pickup.cs:37-39`, `Heal.prefab` (2 en la escena) |
| K2 | Damage (aumenta daño) | ✓ | `Pickup.cs:40-42` llama a `PlayerDamageBuff.Apply` (+1 de daño durante 8 s, en `PickupSo.asset`) |
| K3 | Invulnerability por x segundos | ✓ | `Pickup.cs:46-48` llama a `PlayerInvulnerability.Apply`, que usa `Health.SetInvulnerable` durante 4 s con parpadeo |
| K4 | Reward: monedas para el shop | ✓ | `Coin` (1) y `Pouch` (5) en `Pickup.cs:31-36`, monedas que sueltan los enemigos (`EnemyCoinDrop.cs`), y se gastan en `Shop.TryBuy` (`Shop.cs:30-46`) |

### Sonido

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| S1 | AudioMixer con Master, Music, FX y UI | Parcial | `Assets/Art/Audio/MainMixer.mixer` tiene Master, Music y SFX, con `MasterVolume`, `MusicVolume` y `SfxVolume` expuestos. Falta el grupo UI: el `AudioSource` del click de UI en `MainMenu.unity` sale por SFX |
| S2 | Al menos 3 grupos modificables en Settings | ✓ | `UiSettingsConfig.cs` con tres `UiVolumeStepper` (Master, Music, SFX) en `UiSettings.prefab`, aplicados en `VolumeSettings.SetVolume`. Son controles de menos y más con una barra de 10 pasos en lugar de `Slider`, pensados para jugar solo con teclado |
| S3 | Toda acción del juego se refleja en sonido | ✓ | Música de menú, nivel y jefe (`MusicPlayer.cs`), click de UI (`UiButtonSound`, `UiClickPlayer`), pickups (clips en `PickupSo`), compra y error en la tienda (`ShopSo`), gong en checkpoints y al ganar (`GongPlayer.cs`) y grito del jefe. Los pasos que faltan están marcados en J6 y E4 |

### Datos

| #  | Requisito | Estado | Evidencia |
|----|-----------|--------|-----------|
| D1 | Configuraciones del juego en ScriptableObject | ✓ | 10 clases de SO (`PlayerSo`, `EnemySo`, `BossSo`, `PickupSo`, `ShopSo`, `ScoreSo`, `CameraSo`, `AreaSo`, `ParallaxSo`, `PromptSo`) y 3 canales (`HitStopChannel`, `CameraShakeChannel`, `CameraFocusChannel`), con 20 `.asset` en `Assets/Data/` |
| D2 | Datos de partida en PlayerPrefs | ✓ | Mejor puntaje en `RunStats.cs:64-71`, mejor cantidad de monedas en `PlayerWallet.cs:33-39` y volúmenes en `VolumeSettings.cs:10-18` |

### Extras

| #  | Extra | Estado | Evidencia |
|----|-------|--------|-----------|
| X1 | Doble salto | ✓ | `PlayerJumper.cs:133-141`, `airJumpCount: 1` en `PlayerSo.asset` |
| X2 | Triple salto por power up | ✗ | Ningún pickup cambia `airJumpCount` |
| X3 | Tienda con monedas | ✓ | `Merchant.cs`, `Shop.cs`, `ShopPanel.cs` y `ShopSo.asset`: vida máxima, afilado, cargas de curación y shurikens para la pelea con el jefe (las compras duran la partida) |
| X4 | Integración PlayFab | ✗ | No hay referencias a PlayFab |
| X5 | Multijugador local | ✗ | Un solo `Player` en la escena |

### Win, lose y volver a jugar

- Win condition: vencer al jefe. `BossBrain.OnDefeated` (`BossBrain.cs:68-71`) dispara `RunStats.Score` (`RunStats.cs:52-75`), y `VictoryBanner` muestra el cartel y abre `UiVictory` con el puntaje.
- Lose condition: morir. `PlayerHealth.Die` (`PlayerHealth.cs:46-49`) llama a `PlayerRespawner.StartRespawn`, que apaga los controles y, cuando termina la animación, te hace reaparecer en el último checkpoint tocado o en el inicio (`PlayerRespawner.cs:69-81`). Es respawn en checkpoint sin Game Over, y vale como lose condition.
- Volver a jugar después de perder: sí. `Respawn` llena las cargas (`PlayerHealer.RefillCharges`) y la vida (`PlayerHealth.Revive`, que llama a `Health.Refill` y también pone en 0 la invulnerabilidad y el hurt), y `OnRespawned` devuelve a su lugar a todos los enemigos (`EnemyBrain.ResetToSpawn`, `EnemyBrain.cs:110-123`), reinicia la pelea del jefe y abre el muro de la arena (`BossBrain.ResetToSpawn`, `ArenaGate.cs`). La muerte no toca `Time.timeScale`. Después de ganar, `UiVictory.Replay` recarga `Gameplay`, y "Main Menu" desde la pausa hace `gameFlow.Resume()` antes de cargar la escena (`UiGameCanvas.cs:100-104`), así el tiempo vuelve a 1.

## 2. Cumplimiento de Pautas de Cursada

| ID | Pauta | Estado | Evidencia |
|----|-------|--------|-----------|
| P1 | Unity 6000.3.21f1 | ✓ | `ProjectSettings/ProjectVersion.txt` dice `6000.3.21f1` |
| P2 | Repo público en GitHub | ✗ | El repositorio es privado: GitHub devuelve 404 sin iniciar sesión (revisado el 2026-11-16) |
| P3 | Nombre del repo | ✓ | `TP08_C1_Bruno_Platformer` |
| P4 | Código en inglés | ✓ | Clases, métodos, campos, enums (`PickupType`, `ShopItemType`, `ParallaxZone`), capas y escenas en inglés en los 129 scripts. Los comentarios están en español, que está permitido |
| P5 | Un repo por TP, sin branches | ✓ | Trabajaste solo en `main` (38 commits), sin carpetas de otros TPs |

## 3. Estructura de carpetas

| Carpeta esperada | Encontrada | Observación |
|------------------|------------|-------------|
| `Art/Animations/` | ✓ | `Player/`, `Enemies/` (4), `Pickups/`, `World/` |
| `Art/Audio/` | ✓ | `SFX/`, `Tracks/` y `MainMixer.mixer` |
| `Art/Fonts/` | ✓ | |
| `Art/Materials/` | ✓ | `EnemyParticle.mat` |
| `Art/Sprites/` | ✓ | Backgrounds, Enemies, FX, Items, Player, Props, Tilesets, UI |
| `Data/` | ✓ | Subcarpetas por dominio (`Camera`, `Channels`, `Enemies`, `Game`, `Parallax`, `Pickups`, `Player`, `UI`, `World`) |
| `Prefabs/` | ✓ | `Enemies/`, `Pickups/`, `Player/`, `UI/`, `World/` |
| `Scenes/` | ✓ | |
| `Scripts/` | ✓ | Por sistema, con `Data/`, `Game/` y `UI/` |

Archivos mal ubicados:

- `Assets/PhysicsMaterials/PlayerNoFriction.physicsMaterial2D` va en `Assets/Art/Materials/`.
- `Assets/InputSystem_Actions.inputactions` está suelto en la raíz de `Assets/`. Es el asset de input que usa el proyecto (`EditorBuildSettings.asset`, `PlayerSo`, `ShopSo`); ponelo en `Assets/Data/` o `Assets/Settings/`.
- `Assets/Art/UI/IconFlash.mat` va en `Assets/Art/Materials/`.
- `Assets/Art/UI/Spritesheet_UI_Flat.png` y `Assets/Art/UI/tilemap_packed.png` van en `Assets/Art/Sprites/UI/`.

Carpetas extra: `Assets/Art/Tiles/` (tiles y paletas), `Assets/Art/UI/` y `Assets/PhysicsMaterials/`. `Assets/Settings/`, `DefaultVolumeProfile.asset` y `UniversalRenderPipelineGlobalSettings.asset` son de la plantilla URP, y `Assets/TextMesh Pro/` es un paquete importado. Fuera de los archivos de arriba, no penalizan.

El `.gitignore` de Unity está, y no versionaste `Library/` ni `Temp/`.

## 4. Qué le falta

- El grupo UI en el `AudioMixer`, con su parámetro expuesto y su control en Settings (S1).
- Sonido de pasos al correr para el jugador y al caminar para los enemigos, sonido para el dash del arquero y un sonido de muerte propio del jugador (J6, E4).
- Devolver los 6 controles del jugador cuando un golpe con empujón cierra la tienda (`Merchant.cs:85`).
- Coordinar quién prende y apaga los controles del jugador (`PlayerKnockback.cs:81`, `Merchant.cs`, `BossPhaseCutscene.cs`, `PlayerRespawner.cs`).
- El triple salto por power up (extra X2).
- Borrar el código muerto (`TrainingDummy`, `PromptSo.AttackOldText`, los frames del "builder" en `PlayerSo`) y reubicar los 5 archivos de la sección 3.

## 5. Hallazgos de código

### Error (0)

### Bug (1)

- En `Assets/Scripts/World/Merchant.cs:85`, si un golpe con empujón cierra la tienda, `Release` sale sin devolver los controles, y `PlayerKnockback` solo prende Mover, Jumper, Dasher y Attacker. `PlayerDefender` y `PlayerHealer` quedan apagados hasta la próxima muerte.

### Warning (6)

- En `Assets/Scripts/Player/PlayerKnockback.cs:81` `RestoreControl` prende los controles aunque `BossPhaseCutscene` o `Merchant` los hayan apagado. Tenés cuatro clases tocando los mismos `enabled` sin coordinarse.
- En `Assets/Scripts/Player/PlayerAudio.cs:93` cambiás `audioSource.pitch` antes de cada `PlayOneShot`, y eso cambia el tono de los clips que todavía están sonando.
- En `Assets/Scripts/Data/Player/PlayerSo.cs:169` `GetComboDamage` (y los otros getters de arrays) devuelven el array del asset, que cualquier clase puede modificar.
- En `Assets/Scripts/Shared/HitStop.cs:31` comparás floats con `==` y `!=` sobre `Time.timeScale` (también en `Restore`).
- En `Assets/Scripts/Player/PlayerKnockback.cs:64` comparás floats con `==` en `info.Direction.x == 0f`.
- En `Assets/Scripts/Parallax/ParallaxSwitcher.cs:43` comparás floats con `==` en `autumnAlpha == targetAlpha`.

### Suggestion (10)

- `Assets/Scripts/Data/Player/PlayerSo.cs:55` ningún script lee `comboHitFrames`, `airAttackHitFrame` ni `throwHitFrame` (ni sus getters), y los tooltips hablan de un "builder" que no está en el repo.
- `Assets/Scripts/Data/UI/PromptSo.cs:10` `AttackOldText` no se usa, y hay un comentario sobre un "menu 15" que no existe.
- `Assets/Scripts/Shared/TrainingDummy.cs:6` el componente no está en ninguna escena ni prefab.
- `Assets/Scripts/Player/PlayerJumper.cs:57` `if (groundChecker == null)` siempre da verdadero en `Awake`.
- `Assets/Scripts/Pickups/Pickup.cs:46` la invulnerabilidad cae en el `default` del `switch`.
- `Assets/Scripts/Enemies/EnemyHealthBar.cs:27` la barra escala el transform en lugar de usar `Image.fillAmount`.
- `Assets/Scripts/World/Shop.cs:59` el afilado mejora solo el golpe N del combo, pero el README dice "+1 damage on every hit".
- `Assets/Scripts/Shared/StateMachine/AnimState.cs:29` llamás dos veces a `GetCurrentAnimatorStateInfo(0)`.
- `Assets/Scripts/Player/PlayerWallet.cs:47` guardás `BestRunCoins` pero el menú no lo muestra.
- `Assets/Scripts/UI/UiVictory.cs:16` los nombres de escena están repetidos como string en 4 componentes.

Esto no lo cuento como hallazgo: los `Debug.LogError` en los `Awake` de `EnemyBrain`, `EnemyAttack`, `EnemyAudio`, `EnemyHealth`, `EnemyKnockback`, `EnemyAnimator`, `BossAttack` y `BossMover`, y el `Debug.LogWarning` en `PromptSo.GetText`, solo saltan ante un error de configuración. `TrainingDummy.cs:33` tiene un `Debug.Log` por golpe, pero el componente no está en ninguna escena.

## 6. Recomendaciones

1. Agregá el grupo UI al `MainMixer`, exponé `UiVolume`, mandá `UiClickPlayer` a ese grupo, sumá el parámetro a `VolumeLoader` y un cuarto `UiVolumeStepper` en `UiSettings.prefab` (S1).
2. Agregá sonido de pasos al jugador (`PlayerAudio` con un temporizador mientras corre en el piso) y a los enemigos al caminar, sonido al dash del arquero y un clip de muerte para el jugador (J6, E4).
3. En `Merchant`, cuando el golpe cierra la tienda con empujón, esperá a que termine el knockback y devolvé los 6 controles (`Merchant.cs:85`).
4. Centralizá el bloqueo de controles del jugador en un solo componente con contador, para que `PlayerKnockback`, `Merchant`, `BossPhaseCutscene` y `PlayerRespawner` no se pisen.
5. Agregá un pickup de triple salto que sume un salto aéreo (extra X2).
6. Devolvé colecciones de solo lectura desde `PlayerSo`, usá una `AudioSource` por tipo de sonido en `PlayerAudio` y reemplazá los `==` de float.
7. Borrá `TrainingDummy`, `AttackOldText` y los campos del builder, y mové los 5 archivos de la sección 3.
