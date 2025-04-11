using SDL3;
using System;
using System.Runtime.InteropServices;

namespace GeemuBoy.GB;

public class SDLDisplay : IDisplay, IDisposable
{
    private const float Scale = 2;
    private const int Width = 160;
    private const int Height = 144;

    private readonly IntPtr window;
    private readonly IntPtr renderer;
    private readonly IntPtr texture;
    private readonly IntPtr pixels;

    public SDLDisplay()
    {
        pixels = Marshal.AllocHGlobal(Width * Height * sizeof(uint));

        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio))
        {
            throw new Exception($"Unable to initialize SDL {SDL.GetError()}");
        }

        SDL.CreateWindowAndRenderer(
            //     "Emulator",
            //     SDL.SDL_WINDOWPOS_CENTERED,
            //     SDL.SDL_WINDOWPOS_CENTERED,
            //     Convert.ToInt32(WIDTH * SCALE),
            //     Convert.ToInt32(HEIGHT * SCALE),
            //     SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            "GeemuBoy",
            Convert.ToInt32(Width * Scale),
            Convert.ToInt32(Height * Scale),
            0,
            out window,
            out renderer
        );

        // renderer = SDL.SDL_CreateRenderer(
        //     window,
        //     -1,
        //     SDL.SDL_RendererFlags.SDL_RENDERER_SOFTWARE);

        texture = SDL.CreateTexture(
            renderer,
            // SDL.SDL_PIXELFORMAT_ARGB8888,
            SDL.PixelFormat.ARGB8888,
            SDL.TextureAccess.Streaming,
            Width,
            Height);

        ClearTexture();

    }

    public void RenderLine(int y, uint[] line)
    {
        unsafe
        {
            uint* data = (uint*)this.pixels;
            int offset = y * Width;
            for (int i = 0; i < line.Length; i++)
            {
                data[i + offset] = line[i];
            }
        }
    }

    public void Render()
    {
        SDL.UpdateTexture(
            texture,
            IntPtr.Zero,
            pixels,
            Width * sizeof(uint));

        SDL.SetRenderDrawColor(renderer, 0xFF, 0xFF, 0xFF, 0xFF);
        SDL.RenderClear(renderer);

        SDL.RenderTexture(
            renderer,
            texture,
            IntPtr.Zero,
            IntPtr.Zero);
        SDL.RenderPresent(renderer);
    }

    private void ClearTexture()
    {
        unsafe
        {
            uint* data = (uint*)pixels;
            for (int i = 0; i < Width * Height; i++)
            {
                data[i] = 0xFFFFFFFF;
            }
        }
    }

    #region IDisposable Support
    private bool disposedValue; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects).
            }

            // Free unmanaged resources (unmanaged objects) and override a finalizer below.
            // Set large fields to null.
            SDL.DestroyWindow(window);
            SDL.DestroyRenderer(renderer);
            SDL.DestroyTexture(texture);
            Marshal.FreeHGlobal(pixels);
            SDL.Quit();

            disposedValue = true;
        }
    }

    // Override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
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
        // Uncomment the following line if the finalizer is overridden above.
        GC.SuppressFinalize(this);
    }
    #endregion
}