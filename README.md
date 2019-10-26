# zer0cs_reverse

Platform independent (.NET required), interactive, minimalistic reverse shell written in C#.
Based on old C# reverse shell source from 2007.

- Windows
- Linux
- MacOS
- ... if .NET is available 

[https://github.com/loadenmb/zer0cs_reverse](https://github.com/loadenmb/zer0cs_reverse)

## Usage

On Linux / *nix: get [mono](https://www.mono-project.com/docs/about-mono/languages/csharp/) compiler.

```shell

# download from git
git clone https://github.com/loadenmb/zer0cs_reverse.git

# build (for Windows: replace mcs with path to csc.exe)
mcs zer0cs_reverse.cs -optimize
```

Wait for reverse connection at localhost:
```shell
nc -nvlp 4242
```

## Contribute
Discuss features, report issues, questions -> [here](https://github.com/loadenmb/zer0cs_reverse/issues).

Developer -> fork & pull ;)
