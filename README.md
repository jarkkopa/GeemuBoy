# Geemu Boy [![Build Status](https://travis-ci.org/jarkkopa/GeemuBoy.svg?branch=master)](https://travis-ci.org/jarkkopa/GeemuBoy)
Geemu Boy emulator for .NET Core 3.1.

Uses [SDL2](https://www.libsdl.org/) with [SDL2-CS Bindings](https://github.com/flibitijibibo/SDL2-CS) for rendering.

Integration tests in `/GeemuBoyRomTests` uses Gekkio's compiled test roms from [Mooneye GB](https://github.com/Gekkio/mooneye-gb)

What should be implemented next:
- Missing opcodes (STOP and HALT)
- More precise clock handling
- Audio

Blargg's individual CPU instruction test results
- 01: Passed
- 02: Failed
- 03: Passed
- 04: Passed
- 05: Passed
- 06: Passed
- 07: Passed
- 08: Passed
- 09: Passed
- 10: Passed
- 11: Passed
