using GameBoy.GB;
using SDL2;
using System;
using System.IO;
namespace GameBoy
{
    class EmulatorSDL
    {
        private readonly CPU cpu;
        private readonly Memory memory;
        private readonly PPU ppu;

        private IntPtr window = IntPtr.Zero;
        private IntPtr renderer = IntPtr.Zero;

        public EmulatorSDL(string? cartridgePath, string? bootRomPath)
        {
            byte[]? cartridge = null;
            if (!string.IsNullOrEmpty(cartridgePath))
            {
                cartridge = File.ReadAllBytes(cartridgePath);
            }

            byte[]? bootRom = null;
            if (!string.IsNullOrEmpty(bootRomPath))
            {
                bootRom = File.ReadAllBytes(bootRomPath);
            }

            memory = new Memory(cartridge, bootRom);
            ppu = new PPU(memory);
            cpu = new CPU(memory, ppu);

            InitializeSDL();
            Run();
        }

        private const int width = 160;
        private const int height = 144;
        private void InitializeSDL()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                throw new Exception($"Unable to initialize SDL {SDL.SDL_GetError()}");
            }

            window = SDL.SDL_CreateWindow(
                "Emulator",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                width,
                height,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

            renderer = SDL.SDL_CreateRenderer(
               window,
               0,
               SDL.SDL_RendererFlags.SDL_RENDERER_SOFTWARE);
        }

        private void Run()
        {
            bool quit = false;
            SDL.SDL_Event evt;

            while (!quit)
            {
                while (SDL.SDL_PollEvent(out evt) != 0)
                {
                    switch (evt.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            quit = true;
                            break;
                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            if (evt.key.keysym.sym == SDL.SDL_Keycode.SDLK_q)
                            {
                                quit = true;
                            }
                            else if (evt.key.keysym.sym == SDL.SDL_Keycode.SDLK_p)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    for (int y = 0; y < height; y++)
                                    {
                                        SDL.SDL_SetRenderDrawColor(renderer, 256%8, 200, 0, 1);
                                        SDL.SDL_RenderDrawPoint(renderer, x, y);

                                    }
                                }
                                SDL.SDL_RenderPresent(renderer);

                            }
                            break;
                    }
                }
            }
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
}
