# TvConsole - The dotnet Console revamped!

> **Note**: This project is in heavy WIP. Not everything is done... but it will (hopefully) be! :D

# What is this project?

One of the most used dotnet APIs is `System.Console`. Unfortunately the dotnet `System.Console` do not expose all the power of the Windows (& probably Linux) console subsystems.

The goal of this project is to create a **new console API** for dotnet, supporting all major features provided by the OS:

* Support for **multiple secreen buffers**
* Support for **mouse events** and other event types
* Support for attaching another process console

## Compatibility with System.Console

`TvConsole.Instance` is the singleton containing the process' console, in a very similar way that the static class `System.Console` does. The public API of `TvConsole` class tries to mimic the public API of `System.Console`. You can open your project, and replace `Console` for `TvConsole.Instance` and everything should compile and work.

Of course, `TvConsole` offers a new set of APIs.

## Color management with using pattern

Despite of using the `ForegroundColor` and `BackgroundColor` properties, four methods have been added to `TvConsole` API. This all for methods returns a `TvConsoleColor` which is a **disposable** object. When a `TvConsoleColor` object is disposed it restores the foreground and background colors at the same state they were when the object was created. This allows changing colors in a far more easy way:

```
TvConsole.Instance.ForegroundColor = ConsoleColor.Yellow;
TvConsole.Instance.BackgroundColor = ConsoleColor.Blue;
TvConsole.Instance.WriteLine("This is in yellow over blue");
using (TvConsole.Instance.ForeColor(ConsoleColor.Red))
{
    TvConsole.Instance.WriteLine("This is written in red (over blue)");
}
TvConsole.Instance.WriteLine("This is writeen in the red over blue again");

```

The four methods are:

1. `ForeColor`: Foreground color is set to the new specified color, until the `using` scope ends
2. `BackColor`: Background color is set to the new specified color, until the `using` scope ends
3. `CharacterColor`: Foreground & background color are set to the new specified color, until the `using` scope ends
4. `ColorScope`: This is a property that opens a new color scope: no color is changed, but any change performed inside the `using` scope is discarded when the `using` block ends.

## Read console events

Method `ReadEvents()` reads all pending console events in a non-blocking way and returns all them in a single instance of `TvConsoleEvents` object:

```
var events = TvConsole.Instance.ReadEvents();
if (events.HasEvents) {
     foreach (var ke in events.KeyboardEvents) {
         // Process keyboard events (keyup and keydown)
     }
     foreach (var me in events.MouseEvents) {
         // Process mouse events (mouse move, click)
     }
 }
```

`ReadEvents()` do not wait: if no event is on the event queue of the console it returns an empty `TvConsoleEvents` object.

## Clear screen

Just use `TvConsole.Instance.Cls()` to clear the console screen :)

## Multiple screen buffers

You can have multiple screen buffers and switch to anyone of them at any time. The only two rules are:

1. The default screen buffer can't be deleted
2. Only one screen buffer can be active at same time

Use `TvConsole.Instance.CreateNewScreenBuffer()` to create a new screen buffer. This returns a `ISecondaryScreenBuffer` instance, which is a **disposable** object. When you finish using your screen buffer just call `Dispose()` or `Close()`. To make one specific screen buffer the active one, just call:

```
TvConsole.Instance.ActivateScreenBuffer(screenBufferToActivate);
```

Methods `Write` and `WriteLine` of the `TvConsole.Instance` always write to the active screen buffer (regardless if this is the default or not). To write to a specific screen buffer, use the `Write` or `WriteLine` methods of the screen buffer object directly. To access the default screen buffer use `TvConsole.Instance.DefaultBuffer` property.

To activate the default screen buffer you can use the helper method `TvConsole.Instance.ActivateDefaultScreenBuffer()` which is equivalent to call `TvConsole.Instance.ActivateScreenBuffer(TvConsole.Instance.DefaultBuffer)`.

> Every screen buffer has the `Out` property to access its `TextWriter`. The `TvConsole.Instance.Out` property returns the `Out` property of the active screen buffer.

## Cursor management

The property `TvConsole.Instance.Cursor` returns the `TvCursor` object to interact with the cursor. You can get the current cursor position using `GetPosition()` method and use the `MoveTo()` method to move the cursor to a new position.

## System.Console currently implemented API (more coming soon)

- `In` and `Out` properties as `TextWriter` instances
- `Read`, `ReadLine`, `ReadKey` methods
- `IsKeyAvailable` property
- `Write`, `WriteLine` methods
- `BackgroundColor`, `ForegroundColor` properties

> **Project is WIP currently. Once a minumum version is ready a NuGet package will be released** 

# Project usage & contribute

Currently there is not a nuget package (it will be one in near future). If you use/test this project and find a bug, fill free to create a issue (same for feature requests) and/or a pull request. Just have in mind that this project is in heavy development so... expect to find bugs and/or unfinished/unpolished features, right now.

# A note about netcore and linux

The project is a netstandard2 library developed under netcore2. But it only supports windows at it relies heavily in the native win32 console api. **I would like to make it compatible with Linux too**, but right now (unfortunately) my knwoledge about how the console works in Linux is very poor... Just learning, but any idea will be welcome ;-)

Thanks!