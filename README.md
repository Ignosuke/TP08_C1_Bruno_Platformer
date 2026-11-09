# THE OLD BLADE

**[English](#english) · [Español](#español)**

**[Play it on itch.io](https://ignosuke.itch.io/the-old-blade)** · **[Repository](https://github.com/Ignosuke/TP08_C1_Bruno_Platformer)**

---

## English

A 2D action platformer built in Unity, in the spirit of Hollow Knight. An old samurai walks from a spring forest into a demon's cave with a sword, a handful of shurikens and whatever the blacksmith will sell him on the way. At the bottom waits the Demon Samurai.

### How to play

| | Keyboard | Gamepad |
|---|---|---|
| **Move** | `A` `D` / arrows | Left stick |
| **Jump / double jump** | `Space` | South button |
| **Attack** (3-hit combo, air attack) | `J` | West button |
| **Throw shuriken** | `S` + `J` | Right trigger |
| **Dash** | `K` | |
| **Defend / parry** | `L` | |
| **Heal** | `Q` | |
| **Talk to the blacksmith** | `E` | |
| **Pause** | `Esc` / `P` | Start |

Menus use the arrows to choose, `J` to confirm and `K` or `Esc` to go back.

### The journey

The level crosses five areas: the Spring Entrance, the Spring Forest, the Autumn Forest, the Torii Camp and the Demon Cave. Each one has its own lighting and its own parallax background, and they swap as you walk between them.

**Torii gates are checkpoints.** Touching one saves your spot and refills your health, with a gong to let you know. Dying sends you back to the last one you touched, with the coins and shurikens you had.

**Teaching by doing.** Small prompts with the real key icons appear the first time you need a move: walking and jumping at the start, attacking before the first enemy, parrying before the first archer, the double jump before the first gap too high to clear with one.

### Fighting

**The sword** strikes in a three-hit combo. The third hit is heavier and knocks the enemy further back. In the air you get a single slash instead.

**Shurikens** fly straight and spin until they hit something. They only do 1 damage: they are for finishing off an enemy or softening up an archer from a distance, not for killing the boss. You start with 3.

**Defending** halves the damage of a hit. Pressing it right before the hit lands turns it into a **parry**: no damage at all, a short freeze on impact and extra points on the final score.

**Healing** spends one of your charges (you start with 3) and restores a heart, but you have to stand still while it happens. Every enemy you kill gives one charge back.

### The cast

**Oni** patrol their ground and chase you once they see you. Plain, honest melee.

**Archers** keep their distance and shoot. Close the gap or bring shurikens.

**Executioners** are slow and tough, and their swing hits for four half hearts. Do not trade blows with them.

**The Demon Samurai** guards the arena at the end of the cave. A stone wall closes behind you when the fight starts. He has two phases with different attack chains, and a cutscene with a roar between them. Dying resets the fight and opens the wall again.

### Coins, pickups and the blacksmith

Enemies burst into coins the first time you kill them. Respawned enemies drop nothing, so coins cannot be farmed. Silver coins are worth 1 and gold coins 5, and the gold ones sit in hidden or risky spots.

| Pickup | Effect |
|---|---|
| **Heal potion** | Restores a heart on the spot |
| **Red potion** | +1 damage on every hit for 8 seconds. The samurai glows red |
| **Blue potion** | Invulnerable for 4 seconds. The samurai glows light blue and blinks as it runs out |

The blacksmith waits right before the boss arena, so you spend everything in one go. There are never enough coins for all of it, so you have to choose.

| Item | Effect | Cap |
|---|---|---|
| **Max health** | +1 heart | 2 |
| **Sharpen** | +1 damage on every hit | 3 |
| **Heal charge** | +1 healing charge | 2 |
| **Shurikens** | +3 shurikens | 3 |

### Score

Beating the boss opens a score screen where each line pops in on its own:

| | Points |
|---|---|
| **Time** | 10000, minus 20 per second, never below 1000 |
| **Coins** | 25 for each coin you were carrying |
| **Parries** | 100 for each parry |
| **Deaths** | A multiplier on the whole score: ×1.2 with no deaths, ×1.0 with one, then −0.1 for each extra death, never below ×0.5 |

The best score is saved and shown on the main menu.

### Game feel

**Coyote time and jump buffer.** You can still jump a moment after leaving a ledge, and a jump pressed just before landing fires as you touch down. Attacks are buffered the same way, so combos do not drop inputs.

**Hit stop and camera shake.** Every hit freezes the game for a few hundredths of a second, longer on the heavy third hit and on a parry. Big hits shake the camera.

**Knockback both ways.** Enemies get pushed back by your hits and you get pushed back by theirs. Blocking halves the push.

**Readable feedback.** Enemies flash when hit, get stunned on a parry and burst into particles when they die. Every enemy has its own health bar, and the boss has a big one at the top of the screen.

### Settings

Master, music and SFX each have their own bus on the audio mixer and their own volume control, reachable from the main menu and from the pause screen. The values are saved between sessions.

### Built with

Unity 6 · C# · 2D physics · Input System · Tilemap · URP 2D lights · TextMeshPro · Audio Mixer

### How it's put together

**ScriptableObjects for every tunable value.** `PlayerSo`, one `EnemySo` per enemy, `BossSo` with its attack patterns, `PickupSo`, `ShopSo`, `ScoreSo`, `CameraSo`, one `AreaSo` per area, `PromptSo` for every on-screen text. Components hold references and nothing else.

**Small components with one job each.** The player is a mover, a jumper, a dasher, an attacker, a thrower, a defender, a healer, a wallet and so on. None of them knows about the others more than it has to.

**Events everywhere.** Every system raises its own events and never learns who listens. The animator, the audio and the whole UI subscribe to them. That is why the sound could be added at the end without touching any gameplay code.

**A state machine for the game flow.** Playing and Paused are states on a stack. Pausing pushes a state that stops time and disables the player's input, and resuming pops it.

**Damage belongs to whoever takes it.** Attackers implement `IDamageSource` and only expose a number. Whatever implements `IDamageable` decides what the hit means: block, parry, knockback, death.

**Pooling.** Arrows and shurikens come from a shared pool instead of being created and destroyed.

### Running it locally

```
git clone https://github.com/Ignosuke/TP08_C1_Bruno_Platformer.git
```

Open the project in Unity 6000.3, load `Splash` (or `MainMenu`) and press Play.

---

## Español

Un plataformero de acción 2D hecho en Unity, al estilo Hollow Knight. Un samurai viejo va desde un bosque de primavera hasta la cueva de un demonio con una espada, un puñado de shurikens y lo que el herrero le quiera vender en el camino. Al fondo lo espera el Samurai Demonio.

### Cómo se juega

| | Teclado | Joystick |
|---|---|---|
| **Moverse** | `A` `D` / flechas | Stick izquierdo |
| **Saltar / doble salto** | `Espacio` | Botón de abajo |
| **Atacar** (combo de 3, ataque aéreo) | `J` | Botón de la izquierda |
| **Lanzar shuriken** | `S` + `J` | Gatillo derecho |
| **Dash** | `K` | |
| **Defender / parry** | `L` | |
| **Curarse** | `Q` | |
| **Hablar con el herrero** | `E` | |
| **Pausa** | `Esc` / `P` | Start |

En los menús se elige con las flechas, se confirma con `J` y se vuelve con `K` o `Esc`.

### El recorrido

El nivel cruza cinco zonas: la Entrada de Primavera, el Bosque de Primavera, el Bosque de Otoño, el Campamento Torii y la Cueva del Demonio. Cada una tiene su iluminación y su fondo con parallax, y van cambiando a medida que caminás.

**Los torii son checkpoints.** Tocar uno guarda tu lugar y te llena la vida, con un gong que te avisa. Si morís volvés al último que tocaste, con las monedas y los shurikens que tenías.

**Se aprende jugando.** La primera vez que necesitás un movimiento aparecen carteles chiquitos con los íconos de las teclas: caminar y saltar al principio, atacar antes del primer enemigo, el parry antes del primer arquero, el doble salto antes del primer hueco que no se salta con uno solo.

### El combate

**La espada** pega en un combo de tres golpes. El tercero es más pesado y empuja más lejos al enemigo. En el aire hay un solo corte.

**Los shurikens** van derecho y giran hasta pegarle a algo. Hacen 1 de daño nomás: sirven para rematar a un enemigo o ablandar a un arquero de lejos, no para matar al jefe. Arrancás con 3.

**Defender** reduce el daño de un golpe a la mitad. Apretarlo justo antes de que llegue el golpe lo convierte en **parry**: nada de daño, un congelamiento corto en el impacto y puntos extra en el puntaje final.

**Curarse** gasta una de tus cargas (arrancás con 3) y recupera un corazón, pero te tenés que quedar quieto mientras dura. Cada enemigo que matás te devuelve una carga.

### El elenco

**Los Oni** patrullan su zona y te persiguen cuando te ven. Cuerpo a cuerpo, sin vueltas.

**Los arqueros** mantienen la distancia y disparan. Acortá la distancia o llevá shurikens.

**Los verdugos** son lentos y duros, y su golpe saca cuatro medios corazones. No intercambies golpes con ellos.

**El Samurai Demonio** custodia la arena al final de la cueva. Cuando empieza la pelea se cierra un muro de piedra detrás tuyo. Tiene dos fases con cadenas de ataques distintas, y entre una y otra una escena con un rugido. Morir reinicia la pelea y vuelve a abrir el muro.

### Monedas, pickups y el herrero

Los enemigos sueltan monedas la primera vez que los matás. Los que reaparecen no sueltan nada, así que no se puede farmear. Las monedas de plata valen 1 y las de oro 5, y las de oro están en lugares escondidos o arriesgados.

| Pickup | Efecto |
|---|---|
| **Poción de vida** | Recupera un corazón en el momento |
| **Poción roja** | +1 de daño en todos los golpes durante 8 segundos. El samurai brilla rojo |
| **Poción azul** | Invulnerable durante 4 segundos. El samurai brilla celeste y parpadea cuando se está por terminar |

El herrero está justo antes de la arena del jefe, así que gastás todo de una. Nunca alcanzan las monedas para comprar todo, así que hay que elegir.

| Objeto | Efecto | Máximo |
|---|---|---|
| **Vida máxima** | +1 corazón | 2 |
| **Afilar** | +1 de daño en todos los golpes | 3 |
| **Carga de curación** | +1 carga para curarse | 2 |
| **Shurikens** | +3 shurikens | 3 |

### Puntaje

Al vencer al jefe se abre una pantalla de puntaje donde cada línea aparece de a una:

| | Puntos |
|---|---|
| **Tiempo** | 10000, menos 20 por segundo, nunca menos de 1000 |
| **Monedas** | 25 por cada moneda que tenías encima |
| **Parries** | 100 por cada parry |
| **Muertes** | Un multiplicador sobre todo el puntaje: ×1.2 sin morir, ×1.0 con una muerte, después −0.1 por cada muerte de más, nunca menos de ×0.5 |

El mejor puntaje se guarda y se muestra en el menú principal.

### Game feel

**Coyote time y jump buffer.** Podés saltar un instante después de pasarte del borde, y un salto apretado justo antes de aterrizar sale apenas tocás el piso. Los ataques también se guardan así, para que el combo no pierda golpes.

**Hit stop y sacudida de cámara.** Cada golpe congela el juego unas centésimas de segundo, más en el tercer golpe del combo y en el parry. Los golpes fuertes sacuden la cámara.

**Knockback para los dos lados.** Tus golpes empujan a los enemigos y los suyos te empujan a vos. Defender reduce el empujón a la mitad.

**Feedback claro.** Los enemigos titilan al recibir un golpe, quedan aturdidos con un parry y estallan en partículas al morir. Cada enemigo tiene su barra de vida, y el jefe una grande arriba de la pantalla.

### Configuración

Master, música y efectos tienen cada uno su bus en el mixer y su propio control de volumen, accesible desde el menú principal y desde la pausa. Los valores se guardan entre sesiones.

### Hecho con

Unity 6 · C# · físicas 2D · Input System · Tilemap · luces 2D de URP · TextMeshPro · Audio Mixer

### Cómo está armado

**ScriptableObjects para cada valor configurable.** `PlayerSo`, un `EnemySo` por enemigo, `BossSo` con sus patrones de ataque, `PickupSo`, `ShopSo`, `ScoreSo`, `CameraSo`, un `AreaSo` por zona y `PromptSo` para cada texto en pantalla. Los componentes guardan referencias y nada más.

**Componentes chicos con una sola tarea.** El jugador es un mover, un jumper, un dasher, un attacker, un thrower, un defender, un healer, una wallet y así. Ninguno sabe de los otros más de lo necesario.

**Eventos en todos lados.** Cada sistema emite sus propios eventos y nunca se entera de quién escucha. El animator, el audio y toda la UI se suscriben a ellos. Por eso el sonido se pudo agregar al final sin tocar código de gameplay.

**Una máquina de estados para el flujo del juego.** Jugando y Pausado son estados en una pila. Pausar apila un estado que frena el tiempo y desactiva el input del jugador, y reanudar lo saca.

**El daño es de quien lo recibe.** Los atacantes implementan `IDamageSource` y solo exponen un número. Quien implementa `IDamageable` decide qué significa el golpe: bloqueo, parry, knockback, muerte.

**Pooling.** Las flechas y los shurikens salen de un pool compartido en lugar de crearse y destruirse.

### Correrlo localmente

```
git clone https://github.com/Ignosuke/TP08_C1_Bruno_Platformer.git
```

Abrí el proyecto en Unity 6000.3, cargá `Splash` (o `MainMenu`) y dale Play.

---

## Credits · Créditos

- **Sprites:** [xzany.itch.io](https://xzany.itch.io)
- **VFX:** [cs5475.itch.io](https://cs5475.itch.io)
- **SFX:** [ci.itch.io](https://ci.itch.io) & [kronbits.itch.io](https://kronbits.itch.io)
- **Music:** [junyua.itch.io](https://junyua.itch.io)
- **Font:** Monogram by datagoblin

Developed by **Ignosuke** for a game development course.
