# Geemu Boy ![](https://github.com/jarkkopa/GeemuBoy/workflows/.NET%20Core/badge.svg?branch=build-action)
Geemu Boy emulator for .NET 9.

Uses [SDL3](https://www.libsdl.org/) with [SDL3-CS Bindings](https://www.nuget.org/packages/SDL3-CS) for rendering.
SDL3 is not included. See [SDL README](https://wiki.libsdl.org/SDL3/README) how to build it.

Integration tests in `/GeemuBoyRomTests` uses Gekkio's compiled test roms from [Mooneye GB](https://github.com/Gekkio/mooneye-gb)
and [Blargg's test roms](https://github.com/retrio/gb-test-roms). Blargg's roms are included as git submodule.

What should be implemented next:
- Missing STOP opcode
- Audio
