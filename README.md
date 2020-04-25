# Geemu Boy [![Build Status](https://travis-ci.org/jarkkopa/GeemuBoy.svg?branch=master)](https://travis-ci.org/jarkkopa/GeemuBoy)
Geemu Boy emulator for .NET Core 3.1.

Uses [SDL2](https://www.libsdl.org/) with [SDL2-CS Bindings](https://github.com/flibitijibibo/SDL2-CS) for rendering.

What should be implemented next:
- Missing opcodes (STOP, HALT and DAA)
- Memory banking
- Sprite rendering
- Input
- Timers
- Audio

Blargg's individual CPU instruction test results
- 01: Failed
- 02: Failed
- 03: Passed
- 04: Passed
- 05: Passed
- 06: Passed
- 07: Passed
- 08: Passed
- 09: Failed
- 10: Passed
- 11: Failed
