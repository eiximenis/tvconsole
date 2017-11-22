# TVconsole - The dotnet Console revamped!

One of the most used dotnet APIs is `System.Console`. Unfortunately the dotnet `System.Console` do not expose all the power of the Windows (& Linux) console subsystems.

The goal of this project is to **expose all the Win32 Console API** in a familiar and easy-to-use way. It is not just a simple wrapper of Win32 console API, but a full new console API.

Goals:

* Built entirely from sratch. No dependencies on any method of `System.Console`
* Support por **peeking** console events
* **Mouse support**
* Allow creation of new consoles and attaching to existing ones

## TVConsole class

The `TVConsole` class is the main entrypoint of the TVConsole API. Its public API mimics the `System.Console` with the addition of new methods / properties.

### Currently System.Console implemented API (more or less, this is heavy wip)

- `In` and `Out` properties as `TextWriter` instances
- `Read`, `ReadLine`, `ReadKey` methods
- `IsKeyAvailable` property
- `Write`, `WriteLine` methods

### New API (again more or less implemented...)

- `ReadEvents` method to read any kind of console event (not only keyboard but also mouse ones).


**Project is WIP currently. Once a minumum version is ready a NuGet package will be released** 