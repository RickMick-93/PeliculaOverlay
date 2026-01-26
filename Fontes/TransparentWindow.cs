using System;
using System.Windows.Forms;
using System.Drawing;

namespace PeliculaOverlay
{
    /// <summary>
    /// Janela "vidro" transparente que serve como base para o overlay.
    /// Características:
    /// - Alpha fixo: 15 (6% visível)
    /// - Cliques SEMPRE passam (WS_EX_TRANSPARENT permanente)
    /// - Sem controles de transparência
    /// - Base para futuras tarjas de tradução
    /// </summary>
    public class TransparentWindow : Form
    {
        // Constantes fixas (não configuráveis)
        private const byte FIXED_ALPHA = 15;        // 6% visível
        private const string WINDOW_TITLE = "Pelicula Overlay - Vidro Base";

        // Propriedade apenas para leitura (valor fixo)
        public int CurrentAlpha { get; } = FIXED_ALPHA;

        public TransparentWindow()
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
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.Opacity = 0.01; // Valor mínimo para existir
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
                cp.ExStyle |= Win32API.WS_EX_LAYERED;

                // WS_EX_TRANSPARENT: Cliques passam através da janela (ESSENCIAL)
                cp.ExStyle |= Win32API.WS_EX_TRANSPARENT;

                // WS_EX_TOPMOST: Sempre no topo da Z-order
                cp.ExStyle |= Win32API.WS_EX_TOPMOST;

                // WS_EX_TOOLWINDOW: Não aparece na taskbar/Alt+Tab
                cp.ExStyle |= Win32API.WS_EX_TOOLWINDOW;

                // WS_EX_NOACTIVATE: Não recebe foco
                cp.ExStyle |= Win32API.WS_EX_NOACTIVATE;

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
                // Aplica alpha fixo via Windows API
                Win32API.SetLayeredWindowAttributes(
                    this.Handle,
                    0,              // ColorKey (0 = não usamos)
                    FIXED_ALPHA,    // Alpha fixo: 15
                    Win32API.LWA_ALPHA
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
                Win32API.SetWindowPos(
                    this.Handle,
                    (IntPtr)Win32API.HWND_TOPMOST,
                    0, 0, 0, 0,
                    Win32API.SWP_NOMOVE | Win32API.SWP_NOSIZE | Win32API.SWP_NOACTIVATE
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ⚠️ Atenção ao forçar top-most: {ex.Message}");
            }
        }

        /// <summary>
        /// Não pinta nada - a janela é completamente transparente
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Base transparente - não desenha nada
            // Futuramente: aqui desenharemos tarjas de tradução
            base.OnPaint(e);
        }

        /// <summary>
        /// Garante que o fundo permaneça transparente
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Não pinta fundo - completamente transparente
            e.Graphics.Clear(Color.Transparent);
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