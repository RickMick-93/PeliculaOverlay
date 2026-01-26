using System;
using System.Windows.Forms;
using System.Drawing;

namespace PeliculaOverlay
{
    /// <summary>
    /// Janela que desenha bordas visuais ciano ao redor da tela.
    /// Características:
    /// - Bordas ciano fixas de 4px
    /// - Cliques SEMPRE passam (área central é transparente)
    /// - Apenas visual - não interativa
    /// - Sincronizada com TransparentWindow
    /// </summary>
    public class VG_Interface_Borda : Form
    {
        // Constantes fixas (não configuráveis)
        private const int BORDER_THICKNESS = 4;         // 4 pixels
        private const string WINDOW_TITLE = "Pelicula Overlay - Bordas";
        private static readonly Color BORDER_COLOR = Color.Cyan; // #00FFFF

        // Magenta é a cor de transparência (área que será invisível)
        private static readonly Color TRANSPARENCY_COLOR = Color.Magenta;

        public VG_Interface_Borda()
        {
            InitializeWindow();
        }

        private void InitializeWindow()
        {
            // Configurações básicas
            this.Text = WINDOW_TITLE;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.ShowIcon = false;
            this.DoubleBuffered = true; // Melhora renderização das bordas

            // Cobrir toda a tela primária
            var screen = Screen.PrimaryScreen;
            this.Bounds = screen.Bounds;

            // Cores: fundo magenta (transparente), bordas ciano
            this.BackColor = TRANSPARENCY_COLOR;
            this.TransparencyKey = TRANSPARENCY_COLOR; // Magenta será invisível

            Console.WriteLine($"   Tamanho das bordas: {screen.Bounds.Width}x{screen.Bounds.Height}");
        }

        /// <summary>
        /// Configura os estilos estendidos - idêntico ao TransparentWindow
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                // Mesmos estilos do TransparentWindow:
                cp.ExStyle |= VG_Sistema_Win32.WS_EX_LAYERED;      // Permite transparência
                cp.ExStyle |= VG_Sistema_Win32.WS_EX_TRANSPARENT;  // Cliques passam
                cp.ExStyle |= VG_Sistema_Win32.WS_EX_TOPMOST;      // Sempre no topo
                cp.ExStyle |= VG_Sistema_Win32.WS_EX_TOOLWINDOW;   // Não aparece na taskbar
                cp.ExStyle |= VG_Sistema_Win32.WS_EX_NOACTIVATE;   // Não recebe foco

                return cp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Console.WriteLine("   Configurando transparência magenta...");
            ConfigureTransparency();

            Console.WriteLine("   Criando região das bordas...");
            CreateBorderRegion();

            Console.WriteLine("   Forçando janela para o topo...");
            ForceToTopMost();

            Console.WriteLine($"   ✅ Bordas prontas: {BORDER_THICKNESS}px ciano");
        }

        /// <summary>
        /// Configura a transparência: magenta será invisível
        /// </summary>
        private void ConfigureTransparency()
        {
            if (this.IsDisposed || this.Handle == IntPtr.Zero)
                return;

            try
            {
                // Converter Color.Magenta para COLORREF (0x00FF00FF)
                uint magentaColorRef = 0x00FF00FF;

                VG_Sistema_Win32.SetLayeredWindowAttributes(
                    this.Handle,
                    magentaColorRef,     // Magenta será transparente
                    255,                 // Opacidade total (apenas a cor chave é transparente)
                    VG_Sistema_Win32.LWA_COLORKEY
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ⚠️ Atenção ao configurar transparência: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria a região que define apenas as bordas (área central vazia)
        /// </summary>
        private void CreateBorderRegion()
        {
            try
            {
                int screenWidth = this.Width;
                int screenHeight = this.Height;
                int thickness = BORDER_THICKNESS;

                // Criar região total da tela
                Region totalRegion = new Region(new Rectangle(0, 0, screenWidth, screenHeight));

                // Criar região central (que será removida)
                Rectangle innerRect = new Rectangle(
                    thickness,                      // X
                    thickness,                      // Y
                    screenWidth - (2 * thickness),  // Largura
                    screenHeight - (2 * thickness)  // Altura
                );

                Region innerRegion = new Region(innerRect);

                // Subtrair região central = ficam apenas as bordas
                totalRegion.Exclude(innerRegion);

                // Aplicar região à janela
                this.Region = totalRegion;

                Console.WriteLine($"   Bordas: {thickness}px, Área central: {innerRect.Width}x{innerRect.Height}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ⚠️ Erro ao criar região das bordas: {ex.Message}");
                // Fallback: janela completa (ainda funcionará, mas sem área central transparente)
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
        /// Desenha as bordas ciano (reforço visual)
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Desenhar bordas ciano
            using (Pen borderPen = new Pen(BORDER_COLOR, BORDER_THICKNESS))
            {
                Rectangle borderRect = new Rectangle(
                    BORDER_THICKNESS / 2,
                    BORDER_THICKNESS / 2,
                    this.ClientSize.Width - BORDER_THICKNESS,
                    this.ClientSize.Height - BORDER_THICKNESS
                );

                e.Graphics.DrawRectangle(borderPen, borderRect);
            }
        }

        /// <summary>
        /// Não pinta fundo - completamente transparente (magenta)
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Pintar com magenta (será transparente via COLORKEY)
            e.Graphics.Clear(TRANSPARENCY_COLOR);
        }

        /// <summary>
        /// Sincroniza posição/tamanho com a janela vidro
        /// </summary>
        public void SyncWithParent(Form parentWindow)
        {
            if (parentWindow != null && !parentWindow.IsDisposed)
            {
                this.Location = parentWindow.Location;
                this.Size = parentWindow.Size;
                this.Invalidate(); // Redesenhar bordas
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberar região se existir
                if (this.Region != null)
                {
                    this.Region.Dispose();
                    this.Region = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}