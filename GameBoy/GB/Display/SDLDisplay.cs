using SDL2;
using System;
using System.Runtime.InteropServices;

namespace GameBoy.GB
{
    public class SDLDisplay : IDisplay, IDisposable
    {
        private const float SCALE = 2;
        private const int WIDTH = 160;
        private const int HEIGHT = 144;

        private readonly IntPtr window = IntPtr.Zero;
        private readonly IntPtr renderer = IntPtr.Zero;
        private readonly IntPtr texture = IntPtr.Zero;

        private IntPtr pixels;

        public SDLDisplay()
        {
            pixels = Marshal.AllocHGlobal(WIDTH * HEIGHT * sizeof(uint));

            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                throw new Exception($"Unable to initialize SDL {SDL.SDL_GetError()}");
            }

            window = SDL.SDL_CreateWindow(
                "Emulator",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                Convert.ToInt32(WIDTH * SCALE),
                Convert.ToInt32(HEIGHT * SCALE),
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

            renderer = SDL.SDL_CreateRenderer(
               window,
               -1,
               SDL.SDL_RendererFlags.SDL_RENDERER_SOFTWARE);

            texture = SDL.SDL_CreateTexture(
                renderer,
                SDL.SDL_PIXELFORMAT_ARGB8888,
                (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                WIDTH,
                HEIGHT);

            ClearTexture();

        }

        public void RenderLine(int y, uint[] line)
        {
            unsafe
            {
                uint* data = (uint*)this.pixels;
                int offset = y * WIDTH;
                for (int i = 0; i < line.Length; i++)
                {
                    data[i + offset] = line[i];
                }
            }
        }

        public void Render()
        {
            SDL.SDL_UpdateTexture(
                texture,
                IntPtr.Zero,
                pixels,
                WIDTH * sizeof(uint));

            SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);
            SDL.SDL_RenderClear(renderer);

            SDL.SDL_RenderCopy(
                renderer,
                texture,
                IntPtr.Zero,
                IntPtr.Zero);
            SDL.SDL_RenderPresent(renderer);
        }

        private void ClearTexture()
        {
            unsafe
            {
                uint* data = (uint*)pixels;
                for (int i = 0; i < WIDTH * HEIGHT; i++)
                {
                    data[i] = 0xFFFFFFFF;
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                SDL.SDL_DestroyWindow(window);
                SDL.SDL_DestroyRenderer(renderer);
                SDL.SDL_DestroyTexture(texture);
                Marshal.FreeHGlobal(pixels);
                SDL.SDL_Quit();

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~SDLDisplay()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
