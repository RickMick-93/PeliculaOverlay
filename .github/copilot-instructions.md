# Copilot Instructions

## General Guidelines
- Use imperative mood for instructions (e.g., "Use X" instead of "You should use X").
- Keep instructions concise and actionable.
- Avoid redundant phrases and ensure clarity.

## Project-Specific Rules
- VisionGlass is a passive and invisible translation app that overlays translations on the screen.
- Architecture consists of the following files with fixed responsibilities:
  - `VG_App_Entrada.cs`: Initialization
  - `VG_Gerenciador_Geral.cs`: Coordination
  - `VG_Interface_Pelicula.cs`: Invisible window with alpha 15
  - `VG_Interface_Borda.cs`: Cyan border of 4px
  - `VG_Monitor_OCR.cs`: Passive sensor with a mouse trigger of 1s, bitmap capture, and Timer
  - `VG_Motor_Idiomas.cs`: Anchor language with local dictionaries prioritized, API fallback, and session cache
  - `VG_Sistema_Win32.cs`: Isolated Win32 bridges
- Rules for development:
  - Ensure click-through functionality is always enabled.
  - Use `Windows.Media.Ocr` for OCR tasks.
  - Maintain compatibility with a minimum of Windows 10.0.19041.0.
  - Local dictionaries should be in `.lang` or `.json` format with priority.
  - Work one step at a time and never generate code without explanation and confirmation.
- Repository location: `C:\Users\rickm\Meu Drive\PeliculaOverlay` (branch main).
- Target projects: .NET 10.
- Immediate task: Program `VG_Monitor_OCR.cs` for a trigger of 1000ms, set the anchor language via `CultureInfo.CurrentCulture`, implement the logic for the three clothing items, and decouple OCR from the mouse position after the trigger.