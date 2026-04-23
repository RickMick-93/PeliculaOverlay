using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace VisionGlass
{
    public class VG_Monitor_OCR
    {
        // Correção do erro CS0104: Especificando que é o Timer do Windows Forms
        private System.Windows.Forms.Timer timerSistema;
        private Point ultimaPosicaoMouse;
        private Bitmap? ultimaCapturaTela; // Interrogação corrige o aviso CS8618
        private VG_Motor_Idiomas motorTradutor;
        private bool aguardandoMudancaDeTela = false;
        private string idiomaNativo;
        private VG_Interface_Pelicula? janelaPelicula;
        private VG_Interface_Borda? janelaBorda;

        public VG_Monitor_OCR()
        {
            motorTradutor = new VG_Motor_Idiomas();
            timerSistema = new System.Windows.Forms.Timer();
            timerSistema.Interval = 1000; // 1 segundo
            timerSistema.Tick += GerenciadorCiclo;
            timerSistema.Start();
            
            Console.WriteLine("VG: Sensor OCR ativo (Modo Detecção).");
        }

        public void ConectarInterface(VG_Interface_Pelicula pelicula)
        {
            janelaPelicula = pelicula;
        }

        public void DefinirBorda(VG_Interface_Borda borda)
        {
            janelaBorda = borda;
        }

        private void GerenciadorCiclo(object sender, EventArgs e)
        {
            Bitmap telaAtual = CapturarMiniaturaTela();
            bool telaMudou = VerificarSeTelaMudou(telaAtual);

            if (telaMudou)
            {
                // Se a tela mudou (ex: rolou o menu), redisparamos o OCR imediatamente
                Console.WriteLine("VG [SENSOR]: Tela mudou (Scroll detectado?). Atualizando...");
                aguardandoMudancaDeTela = false;
                ultimaCapturaTela = telaAtual;
                ExecutarVarreduraETraducao();
                return;
            }

            // Se já traduzimos esta tela e nada mudou, ignoramos o mouse
            if (aguardandoMudancaDeTela) return;

            Point posicaoMouseAgora = Cursor.Position;

            // Se o mouse parou por 1 segundo
            if (posicaoMouseAgora == ultimaPosicaoMouse)
            {
                ExecutarVarreduraETraducao();
                aguardandoMudancaDeTela = true;
            }

            ultimaPosicaoMouse = posicaoMouseAgora;
        }

        private async void ExecutarVarreduraETraducao()
        {
            Console.WriteLine("VG [DEBUG]: > Entrando em ExecutarVarreduraETraducao");
            try
            {
                // 1. Captura a tela em alta resolução
                // 1. Oculta overlays para não capturar as próprias traduções (evita loop e jitter)
                if (janelaPelicula != null) janelaPelicula.Invoke(new Action(() => janelaPelicula.Visible = false));
                if (janelaBorda != null) janelaBorda.Invoke(new Action(() => janelaBorda.Visible = false));
                
                // Pausa mínima para o Windows processar a ocultação antes da captura
                System.Threading.Thread.Sleep(60); 

                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                using Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height);
                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                // 1b. Restaura visibilidade prontamente
                if (janelaPelicula != null) janelaPelicula.Invoke(new Action(() => janelaPelicula.Visible = true));
                if (janelaBorda != null) janelaBorda.Invoke(new Action(() => janelaBorda.Visible = true));

                // 2. Converte para o formato que o Windows Media OCR entende
                using var stream = new System.IO.MemoryStream();
                screenshot.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                
                stream.Position = 0; 
                var randomAccessStream = stream.AsRandomAccessStream();
                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(randomAccessStream);
                var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                // 3. Executa o OCR
                var engine = Windows.Media.Ocr.OcrEngine.TryCreateFromUserProfileLanguages();
                if (engine == null) return;

                var result = await engine.RecognizeAsync(softwareBitmap);
                var listaFinal = new System.Collections.Generic.List<VG_Texto_Traduzido>();
                
                // 4. Filtra e Detecta Palavra por Palavra
                foreach (var line in result.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        string textoOriginal = word.Text;

                        // 4. COMPENSAÇÃO DE DPI (CALIBRAÇÃO)
                        // O OCR usa pixels físicos da tela. O WinForms usa pixels lógicos.
                        float scaleX = 1.0f;
                        using (Graphics gDpi = Graphics.FromHwnd(IntPtr.Zero)) 
                        {
                            scaleX = gDpi.DpiX / 96.0f;
                        }

                        // Converte física -> lógica
                        var rectPalavra = new Rectangle(
                            (int)(word.BoundingRect.Left / scaleX), 
                            (int)(word.BoundingRect.Top / scaleX), 
                            (int)(word.BoundingRect.Width / scaleX), 
                            (int)(word.BoundingRect.Height / scaleX)
                        );

                        bool deveUsar80;
                        string traducao = motorTradutor.TraduzirTexto(textoOriginal, out deveUsar80);
                        
                        // REGRA: Apenas se for identificado como estrangeiro (retorno diferente do original)
                        if (traducao != textoOriginal)
                        {
                            // Ajuste Fino: Movemos o grifo 2px para baixo para não "atropelar" a base da letra
                            rectPalavra.Offset(0, 2); 
                            var t = new VG_Texto_Traduzido(textoOriginal, traducao, rectPalavra, deveUsar80);
                            listaFinal.Add(t);
                            Console.WriteLine($"VG [FIXED]: '{textoOriginal}' em ({rectPalavra.X}, {rectPalavra.Y}) Scale:{scaleX:F2}");
                        }
                    }
                }

                // 5. Envia para a Película desenhar (se lista vazia, limpa a tela)
                janelaPelicula?.AtualizarTraducoes(listaFinal);
            }
            catch (Exception ex)
            {
                Console.WriteLine("VG [ERRO OCR]: " + ex.Message);
            }
        }

        private bool VerificarSeTelaMudou(Bitmap novaTela)
        {
            if (ultimaCapturaTela == null) return true;

            for (int x = 0; x < novaTela.Width; x++)
            {
                for (int y = 0; y < novaTela.Height; y++)
                {
                    Color p1 = novaTela.GetPixel(x, y);
                    Color p2 = ultimaCapturaTela.GetPixel(x, y);
                    
                    // Tolerância pequena para variações de compressão/brilho
                    if (Math.Abs(p1.R - p2.R) > 5 || Math.Abs(p1.G - p2.G) > 5 || Math.Abs(p1.B - p2.B) > 5)
                        return true;
                }
            }
            return false;
        }

        private Bitmap CapturarMiniaturaTela()
        {
            // Amostragem de 5 pontos (centro e as 4 áreas ao redor)
            Bitmap bmp = new Bitmap(5, 1); 
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                // Ponto 1: Topo esquerdo
                g.CopyFromScreen(100, 100, 0, 0, new Size(1, 1));
                // Ponto 2: Centro
                g.CopyFromScreen(bounds.Width / 2, bounds.Height / 2, 1, 0, new Size(1, 1));
                // Ponto 3: Base Direita
                g.CopyFromScreen(bounds.Width - 100, bounds.Height - 100, 2, 0, new Size(1, 1));
                // Ponto 4: Área de menu superior
                g.CopyFromScreen(bounds.Width / 2, 100, 3, 0, new Size(1, 1));
                // Ponto 5: Área de chat inferior
                g.CopyFromScreen(bounds.Width / 2, bounds.Height - 100, 4, 0, new Size(1, 1));
            }
            return bmp;
        }
    }
}