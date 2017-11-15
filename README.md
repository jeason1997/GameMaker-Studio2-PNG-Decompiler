gms-assets
==========
`gms-assets` is **GameMaker Studio** *PNG* asset extractor/decompiler/dumper.

## Usage
```bash
gms_assets [-z zip name*] [-f filename*] [-t trim image]
```
 - zip name can be `gamename.zip` or `"my game.zip"`
 - file name `data.win` or `"my data.win"`
 - zip usage tested, but file usage not tested yet.
 - trim image will remove edge transparent, but in some cases there is image in erased part.
 - if you use trim image, original image still saved.
 
##### Tested in GameMaker Studio 2
[Download binary executable](https://github.com/VicoErv/gms-assets/releases)