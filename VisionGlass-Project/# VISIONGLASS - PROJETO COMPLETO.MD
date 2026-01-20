VG-PROJECT - VISIONGLASS
1. INICIALIZACAO SILENCIOSA E IDENTIDADE VISUAL
O aplicativo atua como um fantasma no sistema. Ao iniciar, identifica o idioma do Windows (ex: pt-BR) e cria uma janela invisivel que cobre toda a tela. A unica evidencia de funcionamento e uma borda ciano de 4px nos limites do monitor e uma leve pelicula (Alpha 15). O sistema e totalmente "click-through", ou seja, os cliques passam direto para o jogo ou app.

2. MONITORAMENTO PASSIVO E DETECCAO DE IDIOMAS
O app observa a tela constantemente. Ele usa o idioma do Windows como "Ancora". Se o Windows esta em Portugues e surge um texto em Ingles, ele marca para traducao. Se o texto ja estiver em Portugues, ele ignora.

3. ZONAS DE EXCLUSAO (CHATS E COMANDOS)
O VisionGlass possui uma inteligencia para identificar e ignorar janelas de chat e comandos tecnicos (como os comandos de barra do Minecraft), evitando traducoes desnecessarias em textos que o usuario ja domina ou que sao de uso interno do jogo.

4. GATILHO DE ATIVACAO GLOBAL (O PEDAL DE 1 SEGUNDO)
O mouse funciona como um interruptor de intencao. O app nao precisa que voce aponte para o texto. Se o cursor do mouse ficar parado por 1 segundo (1000ms) em QUALQUER parte da tela, o VisionGlass entende que o usuario parou para processar a cena e ativa todas as traducoes detectadas na tela de uma so vez.

5. RENDERIZACAO ADAPTATIVA (AS TRES ROUPAS)
O app identifica o tipo de texto e aplica a melhor forma visual:

CASO A (Menus/Botoes): Desenha tarjas cinzas sobre 80% do botao. Se houver scroll, a tarja fica e o texto dentro dela atualiza.

CASO B (Legendas/Dialogos): Sobrepoe uma tarja cinza clara com texto em amarelo, mantendo um tempo de leitura confortavel.

CASO C (Blocos/Paginas): Cria um Box que imita a cor de fundo original e usa a cor da fonte original para uma substituicao natural.

6. HIERARQUIA DE INTELIGENCIA (O CEREBRO)
A traducao segue tres niveis:

Dicionarios Locais (.lang): Prioridade total para termos tecnicos de jogos salvos na pasta Dictionaries.

API de Traducao: Consulta servicos online para dialogos e textos novos.

Cache de Sessao: Memoria temporaria que guarda o que ja foi traduzido para exibicao instantanea se o texto reaparecer.

7. PERSISTENCIA E DESAPARECIMENTO
A traducao permanece ativa enquanto o texto original estiver na tela. Se o texto sumir (fechar menu), a traducao apaga instantaneamente. Existe um limite de 30 segundos (timeout) para textos estaticos para evitar poluir a visao.

8. DELAY DE LEITURA PARA DIALOGOS
Diferente dos menus, nos dialogos de cinema, o app segura a traducao por alguns segundos extras mesmo apos o texto original sumir, garantindo que o usuario termine de ler a frase.

9. DESACOPLAMENTO DO MOUSE
Apos o gatilho de 1 segundo ser acionado, o mouse deixa de ter importancia. O usuario pode mover o cursor livremente para jogar ou lutar; a traducao continuara sendo processada e exibida baseada apenas no que aparece na tela, e nao na posicao do ponteiro.

AMBICAO FUTURA E PORTABILIDADE
O projeto e estruturado para ser global e multiplataforma. A logica desenvolvida para Windows PC deve ser modular o suficiente para ser portada para Android (Tablets) no futuro, mantendo a mesma filosofia de tradutor passivo e invisivel.