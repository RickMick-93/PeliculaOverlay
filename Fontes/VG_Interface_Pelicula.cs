using System;
using System.Windows.Forms;
using System.Drawing;

namespace VisionGlass
{
    /// <summary>
    /// Janela "vidro" transparente que serve como base para o overlay.
    /// Características:
    /// - Alpha fixo: 15 (6% visível)
    /// - Cliques SEMPRE passam (WS_EX_TRANSPARENT permanente)
    /// - Sem controles de transparência
    /// - Base para futuras tarjas de tradução
    /// </summary>
    public class VG_Interface_Pelicula : Form
    {
        // Constantes fixas (não configuráveis)
        private const byte FIXED_ALPHA = 38;       // ~15% visibilidade (vidro fantasma original)
        private const string WINDOW_TITLE = "Pelicula Overlay - Vidro Base";

        // Propriedade apenas para leitura (valor fixo)
        public int CurrentAlpha { get; } = FIXED_ALPHA;

        private System.Collections.Generic.List<VG_Texto_Traduzido> listaTraducoes = new System.Collections.Generic.List<VG_Texto_Traduzido>();
        private object lockTraducoes = new object();

        public VG_Interface_Pelicula()
        {
            InitializeWindow();
        }

        private void InitializeWindow()
        {
            // Configurações básicas da janela
            this.Text = WINDOW_TITLE;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.ShowIcon = false;
            this.DoubleBuffered = true; // Melhora renderização

            // Cobrir toda a tela primária
            var screen = Screen.PrimaryScreen;
            this.Bounds = screen.Bounds;
            Console.WriteLine($"   Tamanho do vidro: {screen.Bounds.Width}x{screen.Bounds.Height}");

            // Configurações de cor/transparência
            this.BackColor = Color.Magenta; // Magenta será nossa cor de "vidro" invisível
            this.TransparencyKey = Color.Magenta;
            this.Opacity = 1.0; 
        }

        private Action? callbackPanico;
        public void EscutarTeclaPanico(Action acao)
        {
            callbackPanico = acao;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == VG_Sistema_Win32.WM_HOTKEY && (int)m.WParam == 9010) // 9010 = HOTKEY_ID
            {
                callbackPanico?.Invoke();
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Configura os estilos estendidos da janela para ser totalmente não-interativa
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                // WS_EX_LAYERED: Permite transparência alpha
                cp.ExStyle |= VG_Sistema_Win32.WS_EX_LAYERED;

                // WS_EX_TRANSPARENT: Cliques passam através da janela (ESSENCIAL)
                cp.ExStyle |= VG_Sistema_Win32.WS_EX_TRANSPARENT;

                // WS_EX_TOPMOST: Sempre no topo da Z-order
                cp.ExStyle |= VG_Sistema_Win32.WS_EX_TOPMOST;

                // WS_EX_TOOLWINDOW: Não aparece na taskbar/Alt+Tab
                cp.ExStyle |= VG_Sistema_Win32.WS_EX_TOOLWINDOW;

                // WS_EX_NOACTIVATE: Não recebe foco
                cp.ExStyle |= VG_Sistema_Win32.WS_EX_NOACTIVATE;

                return cp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Console.WriteLine("   Aplicando transparência fixa...");
            ApplyFixedTransparency();

            Console.WriteLine("   Forçando janela para o topo...");
            ForceToTopMost();

            Console.WriteLine($"   ✅ Vidro pronto: Alpha={FIXED_ALPHA}/255 (6% visível)");
        }

        /// <summary>
        /// Aplica a transparência fixa (alpha=15) uma única vez
        /// </summary>
        private void ApplyFixedTransparency()
        {
            if (this.IsDisposed || this.Handle == IntPtr.Zero)
                return;

            try
            {
                VG_Sistema_Win32.SetLayeredWindowAttributes(
                    this.Handle,
                    (uint)ColorTranslator.ToWin32(Color.Magenta), 
                    FIXED_ALPHA,
                    VG_Sistema_Win32.LWA_COLORKEY | VG_Sistema_Win32.LWA_ALPHA
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ⚠️ Atenção ao aplicar transparência: {ex.Message}");
                // Não é crítico - a janela ainda funcionará
            }
        }

        /// <summary>
        /// Garante que a janela permaneça sempre no topo
        /// </summary>
        private void ForceToTopMost()
        {
            try
            {
                VG_Sistema_Win32.SetWindowPos(
                    this.Handle,
                    (IntPtr)VG_Sistema_Win32.HWND_TOPMOST,
                    0, 0, 0, 0,
                    VG_Sistema_Win32.SWP_NOMOVE | VG_Sistema_Win32.SWP_NOSIZE | VG_Sistema_Win32.SWP_NOACTIVATE
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ⚠️ Atenção ao forçar top-most: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza a lista de textos a serem desenhados e força o redesenho.
        /// </summary>
        public void AtualizarTraducoes(System.Collections.Generic.List<VG_Texto_Traduzido> novasTraducoes)
        {
            Console.WriteLine($"VG [DEBUG]: Recebidas {novasTraducoes.Count} traduções para desenhar.");
            lock (lockTraducoes)
            {
                listaTraducoes = novasTraducoes;
            }
            
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => this.Invalidate()));
            }
            else
            {
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            
            lock (lockTraducoes)
            {
                // OCR DESENHO: No modo Alpha Global (15-38), magenta fica 100% invisível
                // Não desenhamos fundo manual para não gerar blend roxo.
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                foreach (var item in listaTraducoes)
                {
                    DesenharGrifoAmarelo(g, item);
                }
            }
        }

        private void DesenharGrifoAmarelo(Graphics g, VG_Texto_Traduzido item)
        {
            // O grifo deve ser uma linha amarela abaixo da palavra
            // Usamos a base do retângulo da região detectada
            using (Pen penAmarela = new Pen(Color.Yellow, 2)) // Linha de 2px de espessura
            {
                int yBase = item.Regiao.Bottom - 1; 
                g.DrawLine(penAmarela, item.Regiao.Left, yBase, item.Regiao.Right, yBase);
            }
        }

        /// <summary>
        /// Garante que o fundo permaneça transparente
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Pinta de Magenta para que a TransparencyKey torne isso invisível
            // Mas o que desenharmos no OnPaint será 100% visível
            e.Graphics.Clear(Color.Magenta);
        }

        // MÉTODOS OBSOLETOS - REMOVER NO FUTURO
        #region Métodos Obsoletos (para compatibilidade transitória)

        [Obsolete("Transparência é fixa em alpha=15. Use a propriedade CurrentAlpha.")]
        public void SetTransparency(byte alpha)
        {
            Console.WriteLine("⚠️ Método obsoleto: Transparência é FIXA em alpha=15");
            Console.WriteLine("   Ignorando comando de ajuste...");
        }

        [Obsolete("Transparência é fixa. Use ApplyFixedTransparency() internamente.")]
        public void ForceUpdateTransparency(byte alpha)
        {
            Console.WriteLine("⚠️ Método obsoleto: Transparência configurada no carregamento");
            Console.WriteLine("   Valor fixo: alpha=15 (não alterável)");
        }

        [Obsolete("Testes de transparência removidos - valor fixo.")]
        public void TestarTransparencias()
        {
            Console.WriteLine("\nℹ️ TRANSPARÊNCIA FIXA DO VIDRO:");
            Console.WriteLine("   Alpha = 15/255 (6% visível)");
            Console.WriteLine("   Cliques SEMPRE passam para o aplicativo abaixo");
            Console.WriteLine("   Não configurável - otimizado para sistema OCR");
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberar recursos gerenciados se necessário
            }
            base.Dispose(disposing);
        }
    }
}