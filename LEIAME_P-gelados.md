# P-gelados - Jackeline e Matheus
Atualizacao do projeto (scripts melhorados + menu automatico + sons).

## Como aplicar (5 minutos)
1. Deixe o Unity aberto. No Finder, descompacte este zip.
2. Copie a pasta `Assets` de dentro do zip para dentro da pasta do projeto `P-Gelados`
   e escolha **Mesclar / Substituir** quando o Mac perguntar.
   (Os scripts tem o MESMO nome dos seus, entao a cena continua ligada neles.)
3. Volte ao Unity e espere compilar (barrinha no canto inferior direito).
4. Abra a cena `Assets/main.unity`.
5. Menu no topo: **P-Gelados > 0) FAZER TUDO (recomendado)**
   - Se perguntar sobre o iglu: "Mover para o GeloFinal". Se perguntar sobre o HUD: "Substituir".
6. Apareceu um quadro "Checklist". Tudo [OK]? Aperte **Play**.
7. Salve (Cmd+S).

Cada item tambem pode ser rodado separado (1 organizar, 2 visual, 3 gerar, 4 validar, 5 paredes).
O item 3 pode ser rodado de novo para sortear outra posicao (ele apaga so o que gerou antes).

## Controles
WASD/setas mover | Espaco pular (na agua: subir; na superficie: pula para o gelo) |
Shift descer (nadando) | Q/E girar a camera | R reiniciar

## O que mudou
- GameManager: vidas, invulnerabilidade, empurrao e piscar ao levar dano, flash vermelho,
  respawn, tela de vitoria/derrota com peixes/vidas/tempo, sons, musica, HUD automatico.
- Jogador: animacoes do pinguim (Idle2/Walk/Swim/Jump/Dead), nado com superficie, pulo para sair da agua.
- Inimigos (Patrol): patrulham e PERSEGUEM o jogador quando ele esta nadando perto.
- Camera: suave, Q/E giram, neblina azul debaixo d'agua.
- Agua com textura de ondas animada (gerada por codigo), gelo, fundo do mar.
- Menu gera peixes (FishV1-4), tubaroes (SharkV1), obstaculos, blocos de gelo, rampas e familia no iglu.

## Requisitos da atividade -> onde esta
| Requisito | Onde |
|---|---|
| Personagem principal | Penguen_Emperor_Mesh_Lite + PlayerController |
| Inimigos | Tubarao_* e Focas (Patrol + Damager) |
| Game Manager | GameManager |
| Controle de danos | GameManager.TakeDamage (vidas, invulnTime) + Damager |
| Coleta / pontuacao | Fish -> GameManager.AddFish (Peixes X / Y) |
| Vitoria | Chegar ao Igloo com peixes suficientes |
| Derrota | Vidas chegam a 0 |
| Visual | Neve, blocos de gelo, rampas, agua |

## Problemas comuns
- Erro vermelho ao compilar: copie a mensagem inteira e mande para o Claude.
- Tubarao nada "de lado" ou "de costas": selecione o tubarao > no Patrol mude **Yaw Offset** (90, -90 ou 180).
- Paredes cinzas feias: menu P-Gelados > 5) Alternar visual das paredes invisiveis.
- Agua opaca/rosa: Assets/Art/PG_Agua.mat > confira Surface Type = Transparent.
- Texto com quadradinhos: nao use acentos nos textos do HUD (a fonte padrao do TMP nao tem todos).
- Para ficar mais facil/dificil: GameManager > Fish Needed, Lives, Invuln Time.

## Antes de entregar
- Menu P-Gelados > 4) Validar (tudo [OK]).
- Jogue uma vez ganhando e outra perdendo.
- Git: `git add -A && git commit -m "P-gelados final" && git push`
