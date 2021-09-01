# Directories.Net

## Introduction

- a tiny library (12kB) with a minimal API
- that provides the platform-specific, user-accessible locations
- for retrieving and storing configuration, cache and other data
- on Linux, Windows (≥ 7), macOS and BSD

The library provides the location of these directories by leveraging the mechanisms defined by
- the [XDG base directory](https://standards.freedesktop.org/basedir-spec/basedir-spec-latest.html) and
  the [XDG user directory](https://www.freedesktop.org/wiki/Software/xdg-user-dirs/) specifications on Linux
- the [Known Folder](https://msdn.microsoft.com/en-us/library/windows/desktop/dd378457.aspx) API on Windows
- the [Standard Directories](https://developer.apple.com/library/content/documentation/FileManagement/Conceptual/FileSystemProgrammingGuide/FileSystemOverview/FileSystemOverview.html#//apple_ref/doc/uid/TP40010672-CH2-SW6)
  guidelines on macOS

## Platforms

This library is written for .NET, and runs on .NET 6.0 and later.

A version of this library implemented in Rust is provided by [directories-rs](https://github.com/dirs-dev/directories-rs).

This version is directly based on [the Java version](https://github.com/dirs-dev/directories-jvm).

## Usage

#### Dependency

Add the library as a dependency to your project:

```xml
<ItemGroup>
  <PackageReference Include="Directories.Net" Version="1.0.0" />
</ItemGroup>
```

#### Example

Library run by user Alice:

```csharp
using Directories.Net;

ProjectDirectories myProjDirs = ProjectDirectories.From("com", "Foo Corp", "Bar App");
myProjDirs.configDir;
// Lin: /home/alice/.config/barapp
// Mac: /Users/Alice/Library/Application Support/com.Foo-Corp.Bar-App
// Win: C:\Users\Alice\AppData\Roaming\Foo Corp\Bar App\config

BaseDirectories baseDirs = new BaseDirectories();
baseDirs.executableDir;
// Lin: /home/alice/.local/bin
// Mac: null
// Win: null

UserDirectories userDirs = new UserDirectories();
userDirs.audioDir;
// Lin: /home/alice/Music
// Mac: /Users/Alice/Music
// Win: C:\Users\Alice\Music
```

## Design Goals

- The _directories_ library is designed to provide an accurate snapshot of the
  system's state at the point of invocation of `new BaseDirectories()`, `new
  UserDirectories()` or `ProjectDirectories.From()`.
- This library does not create directories or check for their existence. The library only provides
  information on what the path to a certain directory _should_ be. How this information is used is
  a decision that developers need to make based on the requirements of each individual application.
- This library is intentionally focused on providing information on user-writable directories only.
  There is no discernible benefit in returning a path that points to a user-level, writable
  directory on one operating system, but a system-level, read-only directory on another, that would
  outweigh the confusion and unexpected failures such an approach would cause.
  - `ExecutableDir` is specified to provide the path to a user-writable directory for binaries.<br/>
    As such a directory only commonly exists on Linux, it returns `null` on macOS and Windows.
  - `FontDir` is specified to provide the path to a user-writable directory for fonts.<br/>
    As such a directory only exists on Linux and macOS, it returns `null` Windows.
  - `RuntimeDir` is specified to provide the path to a directory for non-essential runtime data.
    It is required that this directory is created when the user logs in, is only accessible by the
    user itself, is deleted when the user logs out, and supports all filesystem features of the
    operating system.<br/>
    As such a directory only commonly exists on Linux, it returns `null` on macOS and Windows.

## Features

### `BaseDirectories`

The intended use-case for `BaseDirectories` is to query the paths of user-invisible standard directories
that have been defined according to the conventions of the operating system the library is running on.

If you want to compute the location of cache, config or data folders for your own application or project, use `ProjectDirectories` instead.

| Field name     | Value on Linux / BSD / Solaris                                   | Value on Windows                  | Value on macOS                      |
| -------------- | ---------------------------------------------------------------- | --------------------------------- | ----------------------------------- |
| `HomeDir`      | `$HOME`                                                          | `{FOLDERID_UserProfile}`          | `$HOME`                             |
| `CacheDir`     | `$XDG_CACHE_HOME`  or `$HOME`/.cache                             | `{FOLDERID_LocalApplicationData}` | `$HOME`/Library/Caches              |
| `ConfigDir`    | `$XDG_CONFIG_HOME` or `$HOME`/.config                            | `{FOLDERID_ApplicationData}`      | `$HOME`/Library/Application Support |
| `DataDir`      | `$XDG_DATA_HOME`   or `$HOME`/.local/share                       | `{FOLDERID_ApplicationData}`      | `$HOME`/Library/Application Support |
| `DataLocalDir` | `$XDG_DATA_HOME`   or `$HOME`/.local/share                       | `{FOLDERID_LocalApplicationData}` | `$HOME`/Library/Application Support |
| `ExecutableDir`| `$XDG_BIN_HOME` or `$XDG_DATA_HOME`/../bin or `$HOME`/.local/bin | `null`                            | `null`                              |
| `PreferenceDir`| `$XDG_CONFIG_HOME` or `$HOME`/.config                            | `{FOLDERID_ApplicationData}`      | `$HOME`/Library/Preferences         |
| `RuntimeDir`   | `$XDG_RUNTIME_DIR` or `null`                                     | `null`                            | `null`                              |

### `UserDirectories`

The intended use-case for `UserDirectories` is to query the paths of user-facing standard directories
that have been defined according to the conventions of the operating system the library is running on.

| Field name    | Value on Linux / BSD                                 | Value on Windows         | Value on macOS        |
| ------------- | ---------------------------------------------------- | ------------------------ | --------------------- |
| `HomeDir`     | `$HOME`                                              | `{FOLDERID_UserProfile}` | `$HOME`               |
| `AudioDir`    | `XDG_MUSIC_DIR`                                      | `{FOLDERID_Music}`       | `$HOME`/Music         |
| `DesktopDir`  | `XDG_DESKTOP_DIR`                                    | `{FOLDERID_Desktop}`     | `$HOME`/Desktop       |
| `DocumentDir` | `XDG_DOCUMENTS_DIR`                                  | `{FOLDERID_Documents}`   | `$HOME`/Documents     |
| `DownloadDir` | `XDG_DOWNLOAD_DIR`                                   | `{FOLDERID_Downloads}`   | `$HOME`/Downloads     |
| `FontDir`     | `$XDG_DATA_HOME`/fonts or `$HOME`/.local/share/fonts | `null`                   | `$HOME`/Library/Fonts |
| `PictureDir`  | `XDG_PICTURES_DIR`                                   | `{FOLDERID_Pictures}`    | `$HOME`/Pictures      |
| `PublicDir`   | `XDG_PUBLICSHARE_DIR`                                | `{FOLDERID_Public}`      | `$HOME`/Public        |
| `TemplateDir` | `XDG_TEMPLATES_DIR`                                  | `{FOLDERID_Templates}`   | `null`                |
| `VideoDir`    | `XDG_VIDEOS_DIR`                                     | `{FOLDERID_Videos}`      | `$HOME`/Movies        |

### `ProjectDirectories`

The intended use-case for `ProjectDirectories` is to compute the location of cache, config or data folders for your own application or project,
which are derived from the standard directories.

| Field name      | Value on Linux / BSD                                                       | Value on Windows                                         | Value on macOS                                       |
| --------------- | -------------------------------------------------------------------------- | -------------------------------------------------------- | ---------------------------------------------------- |
| `CacheDir`      | `$XDG_CACHE_HOME`/`<project_path>` or `$HOME`/.cache/`<project_path>`      | `{FOLDERID_LocalApplicationData}`/`<project_path>`/cache | `$HOME`/Library/Caches/`<project_path>`              |
| `ConfigDir`     | `$XDG_CONFIG_HOME`/`<project_path>`  or `$HOME`/.config/`<project_path>`   | `{FOLDERID_ApplicationData}`/`<project_path>`/config     | `$HOME`/Library/Application Support/`<project_path>` |
| `DataDir`       | `$XDG_DATA_HOME`/`<project_path>` or `$HOME`/.local/share/`<project_path>` | `{FOLDERID_ApplicationData}`/`<project_path>`/data       | `$HOME`/Library/Application Support/`<project_path>` |
| `DataLocalDir`  | `$XDG_DATA_HOME`/`<project_path>` or `$HOME`/.local/share/`<project_path>` | `{FOLDERID_LocalApplicationData}`/`<project_path>`/data  | `$HOME`/Library/Application Support/`<project_path>` |
| `PreferenceDir` | `$XDG_CONFIG_HOME`/`<project_path>`  or `$HOME`/.config/`<project_path>`   | `{FOLDERID_ApplicationData}`/`<project_path>`/config     | `$HOME`/Library/Preferences/`<project_path>`         |
| `RuntimeDir`    | `$XDG_RUNTIME_DIR`/`<project_path>`                                        | `null`                                                   | `null`                                               |

The specific value of `<project_path>` is computed by the

    ProjectDirectories.From(string qualifier,
                            string organization,
                            string application)

method and varies across operating systems. As an example, calling

    ProjectDirectories.From("org"         /*qualifier*/,
                            "Baz Corp"    /*organization*/,
                            "Foo Bar-App" /*application*/)

results in the following values:


| Value on Linux | Value on Windows         | Value on macOS               |
| -------------- | ------------------------ | ---------------------------- |
| `"foobar-app"` | `"Baz Corp/Foo Bar-App"` | `"org.Baz-Corp.Foo-Bar-App"` |

The `ProjectDirectories.FromPath` method allows the creation of `ProjectDirectories` instances directly from a project path.
This argument is used verbatim and is not adapted to operating system standards.
The use of `ProjectDirectories.FromPath` is strongly discouraged, as its results will not follow operating system standards on at least two of three platforms.
 Directories
