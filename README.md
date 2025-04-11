# Geemu Boy ![](https://github.com/jarkkopa/GeemuBoy/workflows/.NET%20Core/badge.svg?branch=build-action)
Geemu Boy emulator for .NET 9.

Uses [SDL2](https://www.libsdl.org/) with [SDL2-CS Bindings](https://github.com/flibitijibibo/SDL2-CS) for rendering.

Integration tests in `/GeemuBoyRomTests` uses Gekkio's compiled test roms from [Mooneye GB](https://github.com/Gekkio/mooneye-gb)
and [Blargg's test roms](https://github.com/retrio/gb-test-roms). Blargg's roms are included as git submodule.

What should be implemented next:
- Missing STOP opcode
- Audio
