
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCopParser {

    // Takes everything in IFFFileMananger and converts it to higher parsed data.
    // Object also manages sections and level layout
    public class FCopLevel {

        public int width;
        public int height;

        public List<FCopLevelSection> sections = new();
        public FCopAudioParser audio;
        public List<FCopTexture> textures = new();
        public List<TextureSnippet> textureSnippets = new();
        public List<FCopNavMesh> navMeshes = new();
        public List<FCopObject> objects = new();
        public FCopScriptingProject scripting;
        public FCopSceneActors sceneActors;

        IFFFileManager fileManager;

        public FCopLevel(IFFFileManager fileManager) {

            this.fileManager = fileManager;

            InitSectionData();

            InitData();

        }

        public FCopLevel(int width, int height, IFFFileManager fileManager) {

            this.fileManager = fileManager;
            this.width = width + 8;
            this.height = height + 8;

            // + 8s are there for the out of bounds padding, 4 sections on either side.
            foreach (var y in Enumerable.Range(0, height + 8)) {

                foreach (var x in Enumerable.Range(0, width + 8)) {

                    if (x >= 4 && x < width + 4 && y >= 4 && y < height + 4) {
                        sections.Add(FCopLevelSection.CreateEmpty(-120, -100, -80));
                    }
                    else {
                        sections.Add(FCopLevelSection.CreateEmpty(20, -128, -128));
                    }

                }

            }

            InitData();

        }

        public FCopLevel(byte[] nonCompressedFCopFile) {
            // Data is inited in this read func
            ReadNCFCFile(nonCompressedFCopFile.ToList());
        }

        public FCopLevel(int width, int height, byte[] nonCompressedFCopFile) {

            ReadNCFCFile(nonCompressedFCopFile.ToList());

            ClearLevelData(width, height);

        }

        void InitData() {

            List<IFFDataFile> GetFiles(string fourCC) {

                return fileManager.files.Where(file => {

                    return file.dataFourCC == fourCC;

                }).ToList();

            }

            IFFDataFile GetFile(string fourCC) {

                return fileManager.files.First(file => {

                    return file.dataFourCC == fourCC;

                });

            }

            var rpns = new FCopRPNS(GetFile("RPNS"));

            var cfun = new FCopFunctionParser(GetFile("Cfun"));

            scripting = new FCopScriptingProject(rpns, cfun);

            var rawCwavs = GetFiles("Cwav");
            var rawCshd = GetFile("Cshd");

            var rawBitmapFiles = GetFiles("Cbmp");

            try {

                var cdcs = GetFile("Cdcs");

                textureSnippets = TextureSnippet.Parse(cdcs.data);

            }
            catch { }

            var rawNavMeshFiles = GetFiles("Cnet");

            var rawObjectFiles = GetFiles("Cobj");

            var rawActorFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Cact" || file.dataFourCC == "Csac";

            }).ToList();

            foreach (var rawFile in rawBitmapFiles) {
                textures.Add(new FCopTexture(rawFile));
            }

            foreach (var rawFile in rawNavMeshFiles) {
                navMeshes.Add(new FCopNavMesh(rawFile));
            }

            foreach (var rawFile in rawObjectFiles) {
                objects.Add(new FCopObject(rawFile));
            }

            List<FCopActor> actors = new();
            foreach (var rawFile in rawActorFiles) {
                actors.Add(new FCopActor(rawFile));
            }
            sceneActors = new FCopSceneActors(actors, this);

            audio = new FCopAudioParser(rawCwavs, rawCshd, fileManager.subFiles, fileManager.music);


        }

        void InitSectionData() {

            var layout = FCopLevelLayoutParser.Parse(fileManager.files.First(file => {

                return file.dataFourCC == "Cptc";

            }));

            width = layout[0].Count - 1;
            height = layout.Count - 1;

            var rawCtilFiles = fileManager.files.Where(file => {

                return file.dataFourCC == "Ctil";

            }).ToList();

            var parsers = new List<FCopLevelSectionParser>();

            foreach (var rawFile in rawCtilFiles) {
                parsers.Add(new FCopLevelSectionParser(rawFile));
            }

            var itx = 0;
            var ity = 0;

            foreach (var row in layout) {

                foreach (var column in row) {

                    if (column == 0) {
                        itx++;
                        continue;
                    }

                    var grabbedParser = parsers.FirstOrDefault(parser => { return parser.rawFile.dataID == column; });

                    if (grabbedParser == null) {
                        throw new Exception("Layout has id for a non existant section?");
                    }

                    sections.Add(new FCopLevelSection(grabbedParser, this));

                    itx++;

                }

                itx = 0;
                ity++;

            }
        


        }

        public void DeleteAsset(AssetType assetType, int id) {

            switch (assetType) {
                case AssetType.WavSound:
                    audio.RemoveWave(id);
                    break;

                case AssetType.Object:

                    var obj = objects.First(o => o.DataID == id);

                    objects.Remove(obj);

                    break;
                case AssetType.NavMesh:

                    var navMesh = navMeshes.First(n => n.DataID == id);

                    navMeshes.Remove(navMesh);

                    break;
                case AssetType.TextureSnippet:
                    
                    var textureSnippet = textureSnippets.First(n => n.id == id);

                    textureSnippets.Remove(textureSnippet);

                    break;

                case AssetType.SndsSound:
                    // TODO
                    break;

            }

        }

        public void ImportAsset(AssetType assetType, int id, byte[] newData) {

            switch (assetType) {
                case AssetType.WavSound:

                    audio.ImportWave(id, newData);

                    break;
                case AssetType.Texture:

                    var texture = textures.First(t => t.DataID == id);

                    texture.ImportCbmp(newData);

                    break;
                case AssetType.Object:

                    var obj = objects.First(o => o.DataID == id);

                    obj.Import(newData);

                    break;
                case AssetType.SndsSound:
                    // TODO
                    break;
                case AssetType.Music:
                    
                    audio.music.rawFile.data = newData.ToList();

                    break;
                case AssetType.MiniAnimation:
                    // ...TODO..?
                    break;
                case AssetType.Mixed:
                    // ...TODO..?
                    break;
            }

        }

        public FCopAsset AddAsset(AssetType assetType, byte[] newData) {

            switch (assetType) {
                case AssetType.WavSound:

                    return audio.AddWave(newData, scripting.emptyOffset);

                case AssetType.Object:

                    var maxID = objects.Max(o => o.DataID);

                    var rawFile = new IFFDataFile(2, newData.ToList(), "Cobj", maxID + 1, scripting.emptyOffset);

                    var obj = new FCopObject(rawFile);

                    objects.Add(obj);

                    return obj;

                case AssetType.SndsSound:
                    return null;


            }

            return null;

        }

        public void AddAsset(AssetType assetType, FCopAsset asset) {

            switch (assetType) {
                case AssetType.WavSound:
                    break;

                case AssetType.Object:

                    objects.Add((FCopObject)asset);

                    break;

                case AssetType.SndsSound:
                    break;

            }

        }

        public void AddTextureSnippet(int x, int y, int width, int height, int texturePaletteID) {

            int newID = -1;

            if (textureSnippets.Count != 0) {

                var snippetsByAscending = textureSnippets.OrderBy(t => t.id).ToList();

                var previousID = snippetsByAscending[0].id;
                foreach (var snippet in snippetsByAscending) {

                    if (snippet.id == previousID + 1 || snippet.id == previousID) {
                        previousID = snippet.id;
                        continue;
                    }
                    else {
                        newID = previousID + 1;
                        break;
                    }

                }

                if (newID == -1) {

                    newID = snippetsByAscending.Last().id + 1;

                }

            }
            else {

                newID = 1;

            }

            textureSnippets.Add(new TextureSnippet(x, y, width, height, texturePaletteID, newID));

        }

        public IFFDataFile CreateEmptyAssetFile(AssetType assetType) {

            int maxID;
            IFFDataFile rawFile;

            switch (assetType) {
                case AssetType.WavSound:
                    return null;

                case AssetType.Object:

                    maxID = objects.Max(o => o.DataID);

                    rawFile = new IFFDataFile(2, new(), "Cobj", maxID + 1, scripting.emptyOffset);

                    return rawFile;

                case AssetType.NavMesh:

                    maxID = navMeshes.Max(n => n.DataID);

                    rawFile = new IFFDataFile(2, new(), "Cnet", maxID + 1, scripting.emptyOffset);

                    return rawFile;

                    return null;
                case AssetType.SndsSound:
                    return null;

            }

            return null;

        }

        public void ClearLevelData(int width, int height) {

            sections.Clear();

            this.width = width + 8;
            this.height = height + 8;

            // + 8s are there for the out of bounds padding, 4 sections on either side.
            foreach (var y in Enumerable.Range(0, height + 8)) {

                foreach (var x in Enumerable.Range(0, width + 8)) {

                    if (x >= 4 && x < width + 4 && y >= 4 && y < height + 4) {
                        sections.Add(FCopLevelSection.CreateEmpty(-120, -100, -80));
                    }
                    else {
                        sections.Add(FCopLevelSection.CreateEmpty(20, -128, -128));
                    }

                }

            }

        }

        List<string> dataAccountedFor = new List<string>() { "RPNS", "Cshd", "Cwav", "Ctos", "Cptc", "Ctil", "Cfun", "Cnet", "Cbmp", "Cdcs", "Cobj", "Cact", "Csac", "Cctr" };

        void UpdateRPNSOffsets(IFFDataFile rawFile) {

            var preCompileRefs = rawFile.rpnsReferences;

            rawFile.rpnsReferences = new();

            // Remember that the actual compiled offset is stored on the script object
            foreach (var rpnsRef in preCompileRefs) {
                if (rpnsRef == -1) {
                    rawFile.rpnsReferences.Add(-1);
                }
                else {

                    if (scripting.rpns.codeByOffset.TryGetValue(rpnsRef, out var script)) {
                        rawFile.rpnsReferences.Add(script.offset);
                    }
                    else {
                        rawFile.rpnsReferences.Add(scripting.rpns.codeByOffset[scripting.emptyOffset].offset);
                    }

                }

            }

        }

        public IFFFileManager Compile() {

            IFFFileManager newFileManager = new();

            newFileManager.files.AddRange(scripting.Compile());

            newFileManager.files.Add(audio.CompileSoundHeader());

            foreach (var wav in audio.soundEffects) {
                newFileManager.files.Add(wav.rawFile);
            }

            newFileManager.files.Add(audio.CreateCtos(scripting.emptyOffset));

            foreach (var navMesh in navMeshes) {
                newFileManager.files.Add(navMesh.Compile());
            }

            foreach (var texture in textures) {
                newFileManager.files.Add(texture.Compile());
            }

            newFileManager.files.Add(TextureSnippet.Compile(textureSnippets, scripting.emptyOffset));

            foreach (var obj in objects) {
                newFileManager.files.Add(obj.Compile());
            }

            newFileManager.files.AddRange(sceneActors.Compile());

            List<List<int>> layout = new() { new() };

            Dictionary<int, FCopLevelSection> groupedSections = new();

            var row = 0;
            var id = 1;
            foreach (var section in sections) {

                if (groupedSections.Count == 0) {
                    groupedSections[id] = section;
                    layout[row].Add(id);
                    id++;
                    
                }
                else {

                    var foundGroup = false;
                    foreach (var group in groupedSections) {

                        if (section.Compare(group.Value)) {
                            layout[row].Add(group.Key);
                            foundGroup = true;
                            break;
                        }

                    }

                    if (!foundGroup) {
                        groupedSections[id] = section;
                        layout[row].Add(id);
                        id++;
                    }

                }

                if (layout[row].Count == width) {
                    layout[row].Add(0);
                    row++;
                    layout.Add(new());
                }

            }

            foreach (var i in Enumerable.Range(0, width + 1)) {
                layout[row].Add(0);
            }

            var createdCptcRawFile = new IFFDataFile(2, new(), "Cptc", 1, scripting.emptyOffset);
            FCopLevelLayoutParser.Compile(layout, createdCptcRawFile);

            newFileManager.files.Add(createdCptcRawFile);

            foreach (var group in groupedSections) {
                var bytes = group.Value.Compile().Compile();
                var rawFile = new IFFDataFile(2, bytes, "Ctil", group.Key, scripting.emptyOffset);
                newFileManager.files.Add(rawFile);
            }

            var floatingFiles = fileManager.files.Where(file => !dataAccountedFor.Contains(file.dataFourCC));

            newFileManager.files.AddRange(floatingFiles);

            // Cctr
            // I'm not really sure what ctr does, however, all the data is the exact same (other than GlblData) throughout the mission files.
            // So I guess I don't really need to care, I'll just throw this in here.
            newFileManager.files.Add(new IFFDataFile(1, 
                new() { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2B, 0x01, 0x64, 0x1B, 0x38, 0x01, 0x2B, 0x6F, 0xE6 }, 
                "Cctr", 1, scripting.emptyOffset));

            // SWVRs
            newFileManager.subFiles = audio.CompileStreams();
            newFileManager.music = audio.CompileMusic();

            foreach (var rawFile in newFileManager.files) {
                UpdateRPNSOffsets(rawFile);
            }

            newFileManager.Sort();
            fileManager = newFileManager;

            scripting.ResetIDAndOffsets();

            return newFileManager;

        }

        // Ctos and Cctr is not saved into the file because it contains no useful data and is boilerplate for Future Cop.
        // Both will be made when the level compiles.
        public List<byte> CompileToNCFCFile() {

            var total = new List<byte>();

            List<byte> CreateHeader(IFFDataFile file, string eightCC, string name, int dataSize) {

                var totalHeader = new List<byte>();
                var header = new List<byte>();

                header.AddRange(BitConverter.GetBytes(file.startNumber));
                header.AddRange(BitConverter.GetBytes(file.dataID));
                header.AddRange(BitConverter.GetBytes(file.rpnsReferences[0]));
                header.AddRange(BitConverter.GetBytes(file.rpnsReferences[1]));
                header.AddRange(BitConverter.GetBytes(file.rpnsReferences[2]));
                header.AddRange(BitConverter.GetBytes(file.headerCodeData[0]));
                header.AddRange(BitConverter.GetBytes(file.headerCodeData[1]));
                header.AddRange(BitConverter.GetBytes(file.headerCode.Count));
                header.AddRange(file.headerCode);
                header.AddRange(BitConverter.GetBytes(name.Count()));
                header.AddRange(Encoding.ASCII.GetBytes(name));

                totalHeader.AddRange(Encoding.ASCII.GetBytes(eightCC));
                totalHeader.AddRange(BitConverter.GetBytes(header.Count + 12 + dataSize));
                totalHeader.AddRange(header);

                return totalHeader;

            }

            void CreateHeaderWithFile(IFFDataFile file, string eightCC, string name) {

                total.AddRange(CreateHeader(file, eightCC, name, file.data.Count));
                total.AddRange(file.data);

            }

            total.AddRange(scripting.CompileNCFC());
            UpdateRPNSOffsets(scripting.rpns.rawFile);
            UpdateRPNSOffsets(scripting.functionParser.rawFile);

            var compiledEmptyOffset = scripting.rpns.codeByOffset[scripting.emptyOffset].offset;


            var soundHeader = audio.CompileSoundHeader();
            UpdateRPNSOffsets(soundHeader);
            CreateHeaderWithFile(soundHeader, "FCopCshd", "");

            foreach (var wav in audio.soundEffects) {
                UpdateRPNSOffsets(wav.rawFile);
                CreateHeaderWithFile(wav.rawFile, "FCopCwav", wav.name);
            }

            foreach (var navMesh in navMeshes) {
                UpdateRPNSOffsets(navMesh.rawFile);
                CreateHeaderWithFile(navMesh.Compile(), "FCopCnet", navMesh.name);
            }

            foreach (var texture in textures) {
                UpdateRPNSOffsets(texture.rawFile);
                CreateHeaderWithFile(texture.Compile(), "FCopCbmp", texture.name);
            }

            CreateHeaderWithFile(TextureSnippet.Compile(textureSnippets, compiledEmptyOffset), "FCopCdcs", "");

            foreach (var obj in objects) {
                UpdateRPNSOffsets(obj.rawFile);
                CreateHeaderWithFile(obj.Compile(), "FCopCobj", obj.name);
            }

            foreach (var section in sections) {

                var sectionData = new List<byte>();

                sectionData.AddRange(BitConverter.GetBytes(width));
                sectionData.AddRange(BitConverter.GetBytes(height));

                sectionData.AddRange(BitConverter.GetBytes(section.heightMap.Count));

                foreach (var height in section.heightMap) {

                    sectionData.Add((byte)height.GetTruePoint(1));
                    sectionData.Add((byte)height.GetTruePoint(2));
                    sectionData.Add((byte)height.GetTruePoint(3));

                }

                sectionData.AddRange(BitConverter.GetBytes(section.tileColumns.Count));

                foreach (var tileColumn in section.tileColumns) {
                    sectionData.AddRange(BitConverter.GetBytes(tileColumn.tiles.Count));

                    foreach (var tile in tileColumn.tiles) {

                        var meshType = MeshType.IDFromVerticies(tile.verticies);

                        if (meshType == null) {
                            throw new MeshIDException();
                        }

                        sectionData.AddRange(BitConverter.GetBytes((int)meshType));
                        sectionData.AddRange(BitConverter.GetBytes(tile.culling));
                        sectionData.AddRange(BitConverter.GetBytes(tile.effectIndex));
                        sectionData.AddRange(BitConverter.GetBytes(tile.uvs.Count));

                        foreach (var uv in tile.uvs) {
                            sectionData.AddRange(BitConverter.GetBytes(uv));
                        }

                        sectionData.AddRange(BitConverter.GetBytes(tile.texturePalette));
                        sectionData.AddRange(BitConverter.GetBytes(tile.isSemiTransparent ? 1 : 0));
                        sectionData.AddRange(BitConverter.GetBytes(tile.isVectorAnimated ? 1 : 0));

                        //var newShader = tile.shaders.VerifyCorrectShader();

                        //if (newShader != null) {

                        //    tile.shaders = tile.shaders.VerifyCorrectShader();

                        //}

                        sectionData.AddRange(BitConverter.GetBytes((int)tile.shaders.type));
                        sectionData.AddRange(BitConverter.GetBytes(tile.shaders.isQuad ? 1 : 0));

                        switch (tile.shaders.type) {
                            case VertexColorType.MonoChrome:
                                var monoShader = (MonoChromeShader)tile.shaders;

                                sectionData.AddRange(BitConverter.GetBytes((int)monoShader.value));

                                break;
                            case VertexColorType.DynamicMonoChrome:

                                var dynamicMonoShader = (DynamicMonoChromeShader)tile.shaders;

                                foreach (var value in dynamicMonoShader.values) {
                                    sectionData.AddRange(BitConverter.GetBytes(value));
                                }

                                break;
                            case VertexColorType.Color:

                                var colorShader = (ColorShader)tile.shaders;

                                sectionData.AddRange(BitConverter.GetBytes(colorShader.values.Count()));

                                foreach (var value in colorShader.values) {
                                    sectionData.AddRange(BitConverter.GetBytes(value.ToUShort()));
                                }

                                break;
                            case VertexColorType.ColorAnimated:
                                break;
                        }

                        sectionData.AddRange(BitConverter.GetBytes(tile.animationSpeed));
                        sectionData.AddRange(BitConverter.GetBytes(tile.animatedUVs.Count));

                        foreach (var uv in tile.animatedUVs) {
                            sectionData.AddRange(BitConverter.GetBytes(uv));
                        }


                    }

                }

                sectionData.AddRange(BitConverter.GetBytes(section.animationVector.x));
                sectionData.AddRange(BitConverter.GetBytes(section.animationVector.y));
                sectionData.AddRange(section.tileEffects);

                if (section.slfxData != null) {
                    sectionData.AddRange(BitConverter.GetBytes(section.slfxData.Count));
                    sectionData.AddRange(section.slfxData);
                }
                else {
                    sectionData.AddRange(BitConverter.GetBytes(0));

                }

                total.AddRange(
                    CreateHeader(new IFFDataFile(2, new(), "Ctil", 0, compiledEmptyOffset),
                    "SECTION ", "", sectionData.Count));
                total.AddRange(sectionData);

            }

            sceneActors.Compile();

            foreach (var actor in sceneActors.actors) {
                UpdateRPNSOffsets(actor.rawFile);
                CreateHeaderWithFile(actor.rawFile, "FCop" + actor.rawFile.dataFourCC, actor.name);

            }

            #region Actor Groups
            var actorGroupData = new List<byte>();

            foreach (var group in sceneActors.positionalGroupedActors) {

                actorGroupData.AddRange(BitConverter.GetBytes(group.name.Count()));
                actorGroupData.AddRange(Encoding.ASCII.GetBytes(group.name));
                actorGroupData.AddRange(BitConverter.GetBytes(group.nestedActors.Count));

                foreach (var nestAct in group.nestedActors) {
                    actorGroupData.AddRange(BitConverter.GetBytes(nestAct.DataID));
                }

            }

            total.AddRange(Encoding.ASCII.GetBytes("ACTORGRP"));
            total.AddRange(BitConverter.GetBytes(actorGroupData.Count + 16));
            total.AddRange(BitConverter.GetBytes(sceneActors.positionalGroupedActors.Count));
            total.AddRange(actorGroupData);
            #endregion

            #region Teams

            var teamsData = new List<byte>();

            foreach (var team in sceneActors.teams) {

                teamsData.AddRange(BitConverter.GetBytes(team.Value.Count()));
                teamsData.AddRange(Encoding.ASCII.GetBytes(team.Value));
                teamsData.AddRange(BitConverter.GetBytes(team.Key));

            }

            total.AddRange(Encoding.ASCII.GetBytes("ACTTEAMS"));
            total.AddRange(BitConverter.GetBytes(teamsData.Count + 16));
            total.AddRange(BitConverter.GetBytes(sceneActors.teams.Count()));
            total.AddRange(teamsData);

            #endregion

            #region Texture Snippets

            var tSnippetData = new List<byte>();

            foreach (var snippet in textureSnippets) {

                tSnippetData.AddRange(BitConverter.GetBytes(snippet.name.Count()));
                tSnippetData.AddRange(Encoding.ASCII.GetBytes(snippet.name));
                tSnippetData.AddRange(BitConverter.GetBytes(snippet.x));
                tSnippetData.AddRange(BitConverter.GetBytes(snippet.y));
                tSnippetData.AddRange(BitConverter.GetBytes(snippet.width));
                tSnippetData.AddRange(BitConverter.GetBytes(snippet.height));
                tSnippetData.AddRange(BitConverter.GetBytes(snippet.texturePaletteID));
                tSnippetData.AddRange(BitConverter.GetBytes(snippet.id));

            }

            total.AddRange(Encoding.ASCII.GetBytes("TEXTSNIP"));
            total.AddRange(BitConverter.GetBytes(tSnippetData.Count + 16));
            total.AddRange(BitConverter.GetBytes(textureSnippets.Count()));
            total.AddRange(tSnippetData);

            #endregion

            #region Everything Else
            foreach (var file in fileManager.files) {

                if (dataAccountedFor.Contains(file.dataFourCC)) {
                    continue;
                }

                UpdateRPNSOffsets(file);

                CreateHeaderWithFile(file, "FCop" + file.dataFourCC, "");

            }

            var subFiles = audio.CompileStreams();

            foreach (var subFile in subFiles) {

                var fileData = new List<byte>();

                foreach (var file in subFile.files) {
                    fileData.AddRange(CreateHeader(file, "FCop" + file.dataFourCC, file.name, file.data.Count));
                    fileData.AddRange(file.data);
                }

                total.AddRange(Encoding.ASCII.GetBytes("SubFile "));
                total.AddRange(BitConverter.GetBytes(fileData.Count + 32));
                total.AddRange(BitConverter.GetBytes(subFile.files.Count));
                total.AddRange(subFile.CompileName());
                total.AddRange(fileData);

            }

            var musicFile = audio.CompileMusic();

            total.AddRange(Encoding.ASCII.GetBytes("Music   "));
            total.AddRange(BitConverter.GetBytes(musicFile.data.Count + 28));
            total.AddRange(musicFile.CompileName());
            total.AddRange(musicFile.data);
            #endregion

            scripting.ResetIDAndOffsets();

            // Presets

            var presetData = Encoding.ASCII.GetBytes(Presets.Save());

            total.AddRange(Encoding.ASCII.GetBytes("Presets "));
            total.AddRange(BitConverter.GetBytes(presetData.Count()));
            total.AddRange(presetData);

            return total;

        }

        public void ReadNCFCFile(List<byte> data) {

            var dataArray = data.ToArray();

            var newFileManager = new IFFFileManager();

            // key data
            List<FCopActor> actors = new();
            List<FCopNavMesh> navMeshes = new();
            List<FCopObject> objects = new();
            List<FCopTexture> textures = new();
            List<TextureSnippet> texturesSnippets = new();
            List<IFFDataFile> rawCwavs = new();
            IFFDataFile rawCshd = null;
            List<ActorGroup> actorGroups = new();
            Dictionary<int, string> teams = new();
            FCopRPNS rpns = null;
            FCopFunctionParser funParser = null;

            var i = 0;

            IFFDataFile CreateDataFile(int iBeforeHeader) {

                var eightCC = Encoding.Default.GetString(dataArray, i, 8);
                i += 8;
                var totalSize = BitConverter.ToInt32(dataArray, i);
                i += 4;
                var startingNumber = BitConverter.ToInt32(dataArray, i);
                i += 4;
                var dataID = BitConverter.ToInt32(dataArray, i);
                i += 4;
                var rpnsRef1 = BitConverter.ToInt32(dataArray, i);
                i += 4;
                var rpnsRef2 = BitConverter.ToInt32(dataArray, i);
                i += 4;
                var rpnsRef3 = BitConverter.ToInt32(dataArray, i);
                i += 4;
                var headerCodeData1 = BitConverter.ToInt32(dataArray, i);
                i += 4;
                var headerCodeData2 = BitConverter.ToInt32(dataArray, i);
                i += 4;
                var headerCodeSize = BitConverter.ToInt32(dataArray, i);
                i += 4;
                var headerCode = data.GetRange(i, headerCodeSize);
                i += headerCodeSize;
                var nameSize = BitConverter.ToInt32(dataArray, i);
                i += 4;
                var name = Encoding.Default.GetString(dataArray, i, nameSize);
                i += nameSize;

                var headeSize = i - iBeforeHeader;

                if (eightCC == "SECTION ") {
                    sections.Add(new FCopLevelSection(data.GetRange(i, totalSize - headeSize), this));
                    i += totalSize - headeSize;
                    return null;
                }
                else if (eightCC[..4] == "FCop") {
                    var file = new IFFDataFile(
                            startingNumber,
                            data.GetRange(i, totalSize - headeSize),
                            eightCC.Substring(4, 4),
                            dataID,
                            new() { rpnsRef1, rpnsRef2, rpnsRef3 },
                            new() { headerCodeData1, headerCodeData2 },
                            headerCode
                            );

                    file.name = name;

                    i += totalSize - headeSize;

                    if (eightCC.Substring(4,4) == "Cact" || eightCC.Substring(4, 4) == "Csac") {

                        var actor = new FCopActor(file);

                        if (name != "") {
                            actor.name = name;
                        }

                        actors.Add(actor);

                    }
                    else if (eightCC.Substring(4, 4) == "Cnet") {

                        var navMesh = new FCopNavMesh(file);

                        if (name != "") {
                            navMesh.name = name;
                        }

                        navMeshes.Add(navMesh);

                    }
                    else if (eightCC.Substring(4, 4) == "Cobj") {

                        var obj = new FCopObject(file);

                        if (name != "") {
                            obj.name = name;
                        }

                        objects.Add(obj);

                    }
                    else if (eightCC.Substring(4, 4) == "Cbmp") {

                        var texture = new FCopTexture(file);

                        if (name != "") {
                            texture.name = name;
                        }

                        textures.Add(texture);

                    }
                    else if (eightCC.Substring(4, 4) == "Cwav") {
                        file.name = name;
                        rawCwavs.Add(file);
                    }
                    else if (eightCC.Substring(4, 4) == "Cshd") {
                        rawCshd = file;
                    }
                    else if (eightCC.Substring(4, 4) == "RPNS") {
                        rpns = new FCopRPNS(file);
                    }
                    else if (eightCC.Substring(4, 4) == "Cfun") {
                        funParser = new FCopFunctionParser(file);
                    }
                    
                    return file;

                }
                else {
                    throw new Exception("Encountered unkown file");
                }

            } 

            while (i < data.Count()) {

                var iBeforeHeader = i;

                // Offset is not moved because it's just to peak what kind of data it is
                var eightCC = Encoding.Default.GetString(dataArray, i, 8);

                if (eightCC == "SubFile ") {

                    i += 8;
                    var totalSize = BitConverter.ToInt32(dataArray, i);
                    i += 4;
                    var fileCount = BitConverter.ToInt32(dataArray, i);
                    i += 4;
                    var name = data.GetRange(i, 16).ToArray();
                    i += 16;

                    newFileManager.subFiles.Add(new SubFile(name));

                    foreach (var _f in Enumerable.Range(0, fileCount)) {

                        // The index is updated because the subfile header has nothing to do with a data file.
                        iBeforeHeader = i;

                        var file = CreateDataFile(iBeforeHeader);

                        if (file == null) {
                            throw new Exception("Specialized data found in subfile");
                        }

                        newFileManager.subFiles.Last().files.Add(file);

                    }

                }
                else if (eightCC == "Music   ") {

                    i += 8;
                    var totalSize = BitConverter.ToInt32(dataArray, i);
                    i += 4;
                    var name = data.GetRange(i, 16).ToArray();
                    i += 16;

                    var headeSize = i - iBeforeHeader;

                    newFileManager.music = new MusicFile(name, data.GetRange(i, totalSize - headeSize));

                    i += totalSize - headeSize;
                    

                }
                else if (eightCC == "Presets ") {

                    i += 8;
                    var totalSize = BitConverter.ToInt32(dataArray, i);
                    i += 4;

                    Presets.ReadString(Encoding.ASCII.GetString(data.GetRange(i, totalSize).ToArray()));
                    i += totalSize;
                    break;

                }
                else if (eightCC == "ACTORGRP") {

                    i += 8;
                    var totalSize = BitConverter.ToInt32(dataArray, i);
                    i += 4;
                    var groupCount = BitConverter.ToInt32(dataArray, i);
                    i += 4;

                    foreach (var g in Enumerable.Range(0, groupCount)) {

                        var nameSize = BitConverter.ToInt32(dataArray, i);
                        i += 4;
                        var name = Encoding.ASCII.GetString(data.GetRange(i, nameSize).ToArray());
                        i += nameSize;
                        var actorIDCount = BitConverter.ToInt32(dataArray, i);
                        i += 4;

                        var groupStruct = new ActorGroup(name, ActorGroupType.Position, new());

                        foreach (var a in Enumerable.Range(0, actorIDCount)) {
                            var id = BitConverter.ToInt32(dataArray, i);
                            i += 4;
                            groupStruct.actorIDs.Add(id);
                        }

                        actorGroups.Add(groupStruct);

                    }

                }
                else if (eightCC == "ACTTEAMS") {

                    i += 8;
                    var totalSize = BitConverter.ToInt32(dataArray, i);
                    i += 4;
                    var teamCount = BitConverter.ToInt32(dataArray, i);
                    i += 4;

                    foreach (var t in Enumerable.Range(0, teamCount)) {

                        var nameSize = BitConverter.ToInt32(dataArray, i);
                        i += 4;
                        var name = Encoding.ASCII.GetString(data.GetRange(i, nameSize).ToArray());
                        i += nameSize;
                        var id = BitConverter.ToInt32(dataArray, i);
                        i += 4;

                        teams[id] = name;

                    }

                }
                else if (eightCC == "TEXTSNIP") {

                    i += 8;
                    var totalSize = BitConverter.ToInt32(dataArray, i);
                    i += 4;
                    var snippetCount = BitConverter.ToInt32(dataArray, i);
                    i += 4;

                    foreach (var s in Enumerable.Range(0, snippetCount)) {

                        var nameSize = BitConverter.ToInt32(dataArray, i);
                        i += 4;
                        var name = Encoding.ASCII.GetString(data.GetRange(i, nameSize).ToArray());
                        i += nameSize;
                        var x = BitConverter.ToInt32(dataArray, i);
                        i += 4;
                        var y = BitConverter.ToInt32(dataArray, i);
                        i += 4;
                        var width = BitConverter.ToInt32(dataArray, i);
                        i += 4;
                        var height = BitConverter.ToInt32(dataArray, i);
                        i += 4;
                        var textureID = BitConverter.ToInt32(dataArray, i);
                        i += 4;
                        var id = BitConverter.ToInt32(dataArray, i);
                        i += 4;

                        texturesSnippets.Add(new TextureSnippet(name, id, x, y, width, height, textureID));

                    }

                }
                else if (eightCC == "SCPTRPNS") {

                    i += 8;
                    var totalSize = BitConverter.ToInt32(dataArray, i);
                    i += 4;
                    var scriptCount = BitConverter.ToInt32(dataArray, i);
                    i += 4;

                    rpns = new FCopRPNS(data.GetRange(i, totalSize - 16));

                    i += totalSize - 16;

                }
                else if (eightCC == "FUNCTION") {

                    i += 8;
                    var totalSize = BitConverter.ToInt32(dataArray, i);
                    i += 4;
                    var funcCount = BitConverter.ToInt32(dataArray, i);
                    i += 4;

                    funParser = new FCopFunctionParser(data.GetRange(i, totalSize - 16), rpns.code.Last().offset);

                    i += totalSize - 16;

                }
                else {

                    var file = CreateDataFile(iBeforeHeader);

                    if (file != null) {
                        newFileManager.files.Add(file);
                    }

                }

            }

            fileManager = newFileManager;
            sceneActors = new FCopSceneActors(actors, this);
            if (actorGroups.Count != 0) {
                sceneActors.SetPositionalGroup(actorGroups);
            }
            if (teams.Count != 0) {
                sceneActors.FindTeams();
            }
            else {
                sceneActors.teams = teams;
            }
            this.navMeshes = navMeshes;
            this.objects = objects;
            this.textures = textures;
            this.textureSnippets = texturesSnippets;

            scripting = new FCopScriptingProject(rpns, funParser);

            audio = new FCopAudioParser(rawCwavs, rawCshd, fileManager.subFiles, newFileManager.music);

        }

    }

    public class FCopLevelSection {

        public static FCopLevelSection CreateEmpty(int height1, int hieght2, int height3) {

            var emptySection = new FCopLevelSection();

            foreach (var hy in Enumerable.Range(0, 17)) {

                foreach (var hx in Enumerable.Range(0, 17)) {
                    emptySection.heightMap.Add(new HeightPoints(height1, hieght2, height3));
                }

            }

            var x = 0;
            var y = 0;
            foreach (var i in Enumerable.Range(0, 16 * 16)) {

                var newTiles = new List<Tile>();

                var heights = new List<HeightPoints> {
                    emptySection.GetHeightPoint(x, y),
                    emptySection.GetHeightPoint(x + 1, y),
                    emptySection.GetHeightPoint(x, y + 1),
                    emptySection.GetHeightPoint(x + 1, y + 1)
                };

                var column = new TileColumn(x, y, newTiles, heights);
                newTiles.Add(new Tile(column, 68, 0));

                emptySection.tileColumns.Add(column);

                x++;
                if (x == 16) {
                    y++;
                    x = 0;
                }

            }

            emptySection.tileEffects = new() { 0, 0, 0, 0 };

            emptySection.animationVector = new AnimationVector(0, 0);

            emptySection.culling = new LevelCulling();

            emptySection.culling.CalculateCulling(emptySection);

            return emptySection;

        }

        public FCopLevel parent;

        public const int heightMapWdith = 17;

        public const int tileColumnsWidth = 16;

        public List<HeightPoints> heightMap = new List<HeightPoints>();
        public List<TileColumn> tileColumns = new List<TileColumn>();
        // This might be unused.
        List<XRGB555> colors = new List<XRGB555>();
        public AnimationVector animationVector;
        public List<byte> tileEffects;

        public LevelCulling culling;
        public List<byte> slfxData;

        public FCopLevelSection(FCopLevelSectionParser parser, FCopLevel parent) {

            this.colors = parser.colors;
            this.culling = parser.culling;
            this.slfxData = parser.slfxData;

            animationVector = new AnimationVector(parser.animationVector);
            tileEffects = parser.tileEffects;

            foreach (var parsePoint in parser.heightPoints) {
                heightMap.Add(new HeightPoints(parsePoint));
            }

            var count = 0;
            var x = 0;
            var y = 0;
            foreach (var parseColumn in parser.thirdSectionBitfields) {

                // Grabs the tiles for the column in the tiles array. Number 2 is the index of the tiles and number 1 is the count.
                var parsedTiles = parser.tiles.GetRange(parseColumn.number2, parseColumn.number1);

                // Makes the parsed bitfield into a Tile object.
                var tiles = new List<Tile>();

                // Grabs the heights. The heights have already been added so it uses the local height array.
                var heights = new List<HeightPoints>();

                heights.Add(GetHeightPoint(x, y));
                heights.Add(GetHeightPoint(x + 1, y));
                heights.Add(GetHeightPoint(x, y + 1));
                heights.Add(GetHeightPoint(x + 1, y + 1));

                var column = new TileColumn(x, y, tiles, heights);

                foreach (var parsedTile in parsedTiles) {
                    tiles.Add(new Tile(parsedTile, column, parser));
                }

                tileColumns.Add(column);

                x++;
                if (x == 16) {
                    y++;
                    x = 0;
                }

                count++;

            }

            this.parent = parent;

        }

        public FCopLevelSection(List<byte> ncfcSectionData, FCopLevel level) {

            var ncfcSectionDataArray = ncfcSectionData.ToArray();

            var i = 0;

            var mapWidth = BitConverter.ToInt32(ncfcSectionDataArray, i);
            i += 4;
            var mapHeight = BitConverter.ToInt32(ncfcSectionDataArray, i);
            i += 4;
            var heightMapCount = BitConverter.ToInt32(ncfcSectionDataArray, i);
            i += 4;

            if (heightMapCount != 289) {
                throw new Exception("Incorrect vertex count");
            }

            var heightMapData = ncfcSectionData.GetRange(i, heightMapCount * 3);

            foreach (var heightI in Enumerable.Range(0, heightMapCount)) {
                heightMap.Add(new HeightPoints((sbyte)heightMapData[heightI * 3], (sbyte)heightMapData[heightI * 3 + 1], (sbyte)heightMapData[heightI * 3 + 2]));
            }

            i += heightMapCount * 3;

            var tileColumnsCount = BitConverter.ToInt32(ncfcSectionDataArray, i);
            i += 4;

            if (tileColumnsCount != 256) {
                throw new Exception("Incorrect tile column count");
            }

            var x = 0;
            var y = 0;

            foreach (var _tc in Enumerable.Range(0, tileColumnsCount)) {

                var heights = new List<HeightPoints> {
                    GetHeightPoint(x, y),
                    GetHeightPoint(x + 1, y),
                    GetHeightPoint(x, y + 1),
                    GetHeightPoint(x + 1, y + 1)
                };

                var column = new TileColumn(x, y, new(), heights);

                var tileCount = BitConverter.ToInt32(ncfcSectionDataArray, i);
                i += 4;

                foreach (var _t in Enumerable.Range(0, tileCount)) {

                    var meshID = BitConverter.ToInt32(ncfcSectionDataArray, i);
                    i += 4;
                    var culling = BitConverter.ToInt32(ncfcSectionDataArray, i);
                    i += 4;
                    var effectIndex = BitConverter.ToInt32(ncfcSectionDataArray, i);
                    i += 4;
                    var uvCount = BitConverter.ToInt32(ncfcSectionDataArray, i);
                    i += 4;
                    var uvs = new List<int>();
                    foreach (var _ in Enumerable.Range(0, uvCount)) {
                        uvs.Add(BitConverter.ToInt32(ncfcSectionDataArray, i));
                        i += 4;
                    }
                    var texturePalette = BitConverter.ToInt32(ncfcSectionDataArray, i);
                    i += 4;
                    var isSemiTransparent = BitConverter.ToInt32(ncfcSectionDataArray, i);
                    i += 4;
                    var isVectorAnimation = BitConverter.ToInt32(ncfcSectionDataArray, i);
                    i += 4;
                    var shaderType = BitConverter.ToInt32(ncfcSectionDataArray, i);
                    i += 4;
                    var isQuad = BitConverter.ToInt32(ncfcSectionDataArray, i);
                    i += 4;

                    TileShaders shader = null;

                    switch ((VertexColorType)shaderType) {
                        case VertexColorType.MonoChrome:

                            var monoValue = BitConverter.ToInt32(ncfcSectionDataArray, i);
                            i += 4;

                            shader = new MonoChromeShader((byte)monoValue, isQuad == 1);

                            break;
                        case VertexColorType.DynamicMonoChrome:

                            var dynamicMono = new List<int>();
                            foreach (var _ in Enumerable.Range(0, 4)) {
                                dynamicMono.Add(BitConverter.ToInt32(ncfcSectionDataArray, i));
                                i += 4;
                            }

                            shader = new DynamicMonoChromeShader(dynamicMono, isQuad == 1);

                            break;
                        case VertexColorType.Color:

                            var colors = new List<XRGB555>();
                            var colorCount = BitConverter.ToInt32(ncfcSectionDataArray, i);
                            i += 4;

                            foreach (var _c in Enumerable.Range(0, colorCount)) {
                                colors.Add(new XRGB555(ncfcSectionData.GetRange(i, 2)));
                                i += 2;
                            }

                            shader = new ColorShader(colors.ToArray(), isQuad == 1);

                            break;
                        case VertexColorType.ColorAnimated:
                            shader = new AnimatedShader(isQuad == 1);
                            break;
                    }

                    var animationSpeed = BitConverter.ToInt32(ncfcSectionDataArray, i);
                    i += 4;
                    var animatedUVCount = BitConverter.ToInt32(ncfcSectionDataArray, i);
                    i += 4;

                    var animatedUVs = new List<int>();
                    foreach (var _a in Enumerable.Range(0, animatedUVCount)) {
                        animatedUVs.Add(BitConverter.ToInt32(ncfcSectionDataArray, i));
                        i += 4;
                    }

                    var tile = new Tile(column, meshID, culling, effectIndex, uvs, texturePalette, isSemiTransparent == 1, isVectorAnimation == 1, shader, animationSpeed, animatedUVs);

                    column.tiles.Add(tile);

                }

                tileColumns.Add(column);

                x++;

                if (x == tileColumnsWidth) {
                    y++;
                    x = 0;
                }

            }

            animationVector = new AnimationVector(BitConverter.ToInt32(ncfcSectionDataArray, i), BitConverter.ToInt32(ncfcSectionDataArray, i + 4));
            i += 8;

            tileEffects = ncfcSectionData.GetRange(i, 4);
            i += 4;

            var slfxDataCount = BitConverter.ToInt32(ncfcSectionDataArray, i);
            i += 4;

            if (slfxDataCount != 0) {
                slfxData = ncfcSectionData.GetRange(i, slfxDataCount);
                i += slfxDataCount;
            }

            level.width = mapWidth; 
            level.height = mapHeight;
            this.parent = level;

        }

        FCopLevelSection() {
            
        }

        public HeightPoints GetHeightPoint(int x, int y) {
            return heightMap[(y * heightMapWdith) + x];
        }

        public TileColumn GetTileColumn(int x, int y) {
            return tileColumns[(y * tileColumnsWidth) + x];
        }

        class Chunk {

            public int x;
            public int y;

            public List<TileColumn> tileColumns = new List<TileColumn>();

            public Chunk(int x, int y) {
                this.x = x;
                this.y = y;
            }

            public int Count() {

                var total = 0;

                foreach (var column in tileColumns) {

                    total += column.tiles.Count;

                }

                return total;

            }

        }

        // Takes all the higher parsed data and puts them back into their basic data form found in Ctil.
        // This method does all the indexing and compression to allow for FCopLevelParser to convert the data back into binary.
        public FCopLevelSectionParser Compile() {

            culling ??= new LevelCulling();

            culling.CalculateCulling(this);

            var parser = new FCopLevelSectionParser(null);

            List<HeightPoint3> heightPoints = new List<HeightPoint3>();
            List<ThirdSectionBitfield> thirdSectionBitfields = new List<ThirdSectionBitfield>();
            List<TileBitfield> tiles = new List<TileBitfield>();

            var textureCoordinates = new List<int>();
            var tileGraphics = new List<TileGraphicsItem>();
            var tileUVAnimationMetaData = new List<TileUVAnimationMetaData>();
            var animatedTextureCoordinates = new List<int>();

            var existingColors = new Dictionary<ushort, (int, XRGB555)>();
            var colorIndex = 0;

            List<Chunk> chunks = new List<Chunk>() { new Chunk(0,0) };
            List<(TileColumn column, int tileIndex)> columnWithIndex = new();

            foreach (var point in heightMap) {
                heightPoints.Add(point.Compile());
            }

            // IMPORTANT: The tile column array inside the Ctil is sorted from left to right, HOWEVER the tile array is not.
            // The tile array stores tiles inside a 4x4 tile chunk. The tiles inside this chunk move from left to right,
            // and chunks move from left to right as well. What needs to be done is take the sorted tile columns and move 
            // them to the 4x4 chunk pattern. This needs to be done for the tile array alone.
            var x = 0;
            var y = 0;
            var chunkX = 0;
            var chunkY = 0;
            foreach (var i in Enumerable.Range(0,256)) {

                var offsetX = ((chunks.Count - 1) % 4) * 4;
                var offsetY = ((chunks.Count - 1) / 4) * 4;

                var index = ((y + offsetY) * 16) + (x + offsetX);

                chunks.Last().tileColumns.Add(tileColumns[index]);

                x++;

                if (x == 4) {
                    y++;
                    x = 0;
                    if (y == 4) {
                        y = 0;
                        chunkX++;

                        if (chunkX == 4) {
                            chunkY++;

                            if (chunkY == 4) {
                                break;
                            }

                            chunkX = 0;
                        }

                        chunks.Add(new Chunk(chunkX, chunkY));

                    }

                }

            }

            // Compresses all the tiles and adds them to the tile array
            // Data is tracked for the third bitfield (tile count, tiles index)
            foreach (var chunk in chunks) {

                foreach (var column in chunk.tileColumns) {

                    var sortedTiles = new List<TileBitfield>();

                    // Now that the tile columns are sorted to fit the 4x4 chunk pattern in the tile array, we can simple add the tiles after they're sorted.
                    foreach (var tile in column.tiles) {

                        // TODO: Maybe these compressions would be better with dictonaries?

                        // Compresses both the uv mapping and tile graphics
                        int textureIndex = -1;
                        int graphicsIndex = -1;

                        // First the uvs... (Includeding animated UVs)
                        #region CompileTextures

                        if (textureCoordinates.Count != 0) {

                            foreach (var i in Enumerable.Range(0, textureCoordinates.Count - tile.uvs.Count + 1)) {

                                if (textureCoordinates.GetRange(i, tile.uvs.Count).SequenceEqual(tile.uvs)) {

                                    // If no UV animations exist no need to test for them
                                    if (tileUVAnimationMetaData.Count == 0) {

                                        // Index was found, but no uv animation data has been collected so this is a static texture.
                                        // This index is invalid.
                                        if (tile.IsFrameAnimated()) {
                                            break;
                                        }

                                        textureIndex = i;
                                        break;
                                    }

                                    var isTileAnimated = tile.IsFrameAnimated();

                                    var found = false;

                                    // Tests to see if the index is overwritten by animated UVs
                                    foreach (var metaData in tileUVAnimationMetaData) {

                                        if (metaData.textureReplaceOffset / 2 == i) {

                                            // If this tile is not animated but this index is overwritten,
                                            // this index is not valid.
                                            if (!isTileAnimated) {
                                                found = true;
                                                break;
                                            }

                                            if (metaData.frames == tile.GetFrameCount() && metaData.frameDuration == tile.animationSpeed) {

                                                // Tests to see if that animated UVs are the same
                                                if (animatedTextureCoordinates.GetRange(metaData.animationOffset / 2, metaData.frames * 4).SequenceEqual(tile.animatedUVs)) {

                                                    textureIndex = i;
                                                    found = true;
                                                    break;

                                                }

                                            }

                                        }

                                    }

                                    // Index was found, so the loop can stop
                                    if (found && isTileAnimated) { break; }

                                    // If this index isn't overwritten and the tile isn't animated this is a valid index
                                    if (!found && !isTileAnimated) {
                                        textureIndex = i; 
                                        break;
                                    }


                                }

                            }

                        }

                        // No index was found, so new data needs to be added
                        if (textureIndex == -1) {

                            textureIndex = textureCoordinates.Count;

                            if (tile.IsFrameAnimated()) {

                                tileUVAnimationMetaData.Add(tile.CompileFrameAnimation(animatedTextureCoordinates.Count * 2, textureCoordinates.Count * 2));
                                animatedTextureCoordinates.AddRange(tile.animatedUVs);
                                // Re-reverses so it remains in the correct order after compile
                                tile.ReverseAnimatedFrames();

                            }

                            textureCoordinates.AddRange(tile.uvs);


                            if (textureCoordinates.Count > 1024) {
                                throw new TextureArrayMaxExceeded();
                            }

                        }

                        #endregion

                        // Next the colors... (these are for colored vertices)
                        #region Compile Colors

                        if (tile.shaders.type == VertexColorType.Color) {
                            var shader = (ColorShader)tile.shaders;

                            foreach (var color in shader.values) {

                                if (!existingColors.ContainsKey(color.ToUShort())) {
                                    existingColors.Add(color.ToUShort(), (colorIndex, color));
                                    colorIndex++;
                                }

                            }

                        }

                        if (existingColors.Count > 255) {
                            throw new ColorArrayMaxExceeded();
                        }

                        

                        #endregion

                        // Finally the graphics
                        #region CompileGraphics

                        // TEMPORARY
                        // If tile has animated shaders add SLFX data

                        //if (tile.shaders.type == VertexColorType.ColorAnimated) {

                        //    parser.slfxData = new List<byte> { 16, 128, 50, 20 };

                        //}

                        var compiledGraphics = tile.CompileGraphics(existingColors);

                        if (tileGraphics.Count != 0) {

                            foreach (var i in Enumerable.Range(0, tileGraphics.Count)) {

                                var graphicsItem = tileGraphics[i];

                                if (graphicsItem is TileGraphics) {

                                    var graphics = (TileGraphics)graphicsItem;

                                    if ((TileGraphics)compiledGraphics[0] == graphics) {

                                        if (compiledGraphics.Count == 1) {
                                            graphicsIndex = i;
                                            break;
                                        } else {

                                            var same = true;
                                            var i2 = 0;
                                            foreach (var compiledGraphicsItem in compiledGraphics) {

                                                if (i2 == 0) {
                                                    i2++;
                                                    continue;
                                                }

                                                if (i + i2 > tileGraphics.Count) {
                                                    same = false;
                                                    break;
                                                }

                                                var compiledGraphicsMetaData = (TileGraphicsMetaData)compiledGraphicsItem;

                                                if (!(tileGraphics[i + i2] is TileGraphicsMetaData)) {
                                                    same = false;
                                                    break;
                                                }

                                                var graphicsMetaData = (TileGraphicsMetaData)tileGraphics[i + i2];

                                                if (!compiledGraphicsMetaData.data.SequenceEqual(graphicsMetaData.data)) {
                                                    same = false;
                                                    break;
                                                }

                                                i2++;

                                            }

                                            if (same) {
                                                graphicsIndex = i;
                                                break;
                                            }

                                        }

                                    }

                                }

                            }

                        }

                        if (graphicsIndex == -1) {

                            graphicsIndex = tileGraphics.Count;

                            tileGraphics.AddRange(compiledGraphics);

                            if (tileGraphics.Count > 1023) {
                                throw new GraphicsArrayMaxExceeded();
                            }

                        }

                        #endregion

                        var compiledTile = tile.Compile(textureIndex, graphicsIndex);

                        // Tiles are sorted within a tile column, the order is not completely known but what is know is walls cannot be first
                        if (compiledTile.meshID < 71) {
                            sortedTiles.Insert(0, compiledTile);
                        } else {
                            sortedTiles.Add(compiledTile);
                        }

                    }

                    // Makes sure the last tile value is correct

                    foreach (var i in Enumerable.Range(0, sortedTiles.Count)) {
                        var cTile = sortedTiles[i];
                        cTile.isEndInColumnArray = 0;
                        sortedTiles[i] = cTile;
                    }

                    var lastCTile = sortedTiles.Last();
                    lastCTile.isEndInColumnArray = 1;
                    sortedTiles[^1] = lastCTile;

                    columnWithIndex.Add((column, tiles.Count));

                    tiles.AddRange(sortedTiles);

                }

            }

            if (tiles.Count > 1024) {
                throw new MaxTilesExceeded();
            }

            var sortedColumnsWithIndex = columnWithIndex.OrderBy(item => item.column.y).ThenBy(item => item.column.x).ToList();

            foreach (var item in sortedColumnsWithIndex) {

                var bitField = new ThirdSectionBitfield(item.column.tiles.Count, item.tileIndex);
                thirdSectionBitfields.Add(bitField);

            }

            parser.heightPoints = heightPoints;
            parser.culling = culling;
            parser.thirdSectionBitfields = thirdSectionBitfields;
            parser.tiles = tiles;
            parser.textureCoordinates = textureCoordinates;

            colors.Clear();
            foreach (var existingColor in existingColors) {
                colors.Add(existingColor.Value.Item2);
            }
            parser.colors = colors;

            parser.tileGraphics = tileGraphics;
            parser.tileUVAnimationMetaData = tileUVAnimationMetaData;
            parser.animatedTextureCoordinates = animatedTextureCoordinates;

            parser.animationVector = animationVector.Compile();
            parser.tileEffects = new List<byte>(tileEffects);
            parser.slfxData = slfxData;

            return parser;

        }

        public void MirrorDiagonally() {

            MirrorVertically();
            MirrorHorizontally();

        }

        public void MirrorHorizontally() {

            var newHeightOrder = new List<HeightPoints>();

            foreach (var hy in Enumerable.Range(0, 17)) {

                foreach (var hx in Enumerable.Range(0, 17)) {
                    newHeightOrder.Add(GetHeightPoint(16 - hx, hy));
                }

            }

            heightMap = newHeightOrder;

            var newTileColum = new List<TileColumn>();

            foreach (var ty in Enumerable.Range(0, 16)) {

                foreach (var tx in Enumerable.Range(0, 16)) {
                    var column = tileColumns[(ty * 16) + (15 - tx)];

                    var heights = new List<HeightPoints>();

                    heights.Add(GetHeightPoint(tx, ty));
                    heights.Add(GetHeightPoint(tx + 1, ty));
                    heights.Add(GetHeightPoint(tx, ty + 1));
                    heights.Add(GetHeightPoint(tx + 1, ty + 1));

                    column.x = tx;
                    column.y = ty;
                    column.heights = heights;

                    newTileColum.Add(column);
                }

            }

            tileColumns = newTileColum;

            var movedTiles = new List<Tile>();

            foreach (var column in tileColumns) {

                var validTiles = new List<Tile>();

                foreach (var tile in column.tiles) {

                    if (movedTiles.Contains(tile)) {
                        validTiles.Add(tile);
                        continue;
                    }

                    var result = tile.MirrorVerticesHorizontally();

                    if (result == Tile.TransformResult.Success) {

                        tile.MirrorUVsHorizontally();
                        tile.MirrorShadersHorizontally();

                        validTiles.Add(tile);

                    }
                    else if (result == Tile.TransformResult.MoveColumnPosX) {

                        if (column.x < 15) {

                            var nextColumn = tileColumns[(column.y * 16) + (column.x + 1)];

                            tile.column = nextColumn;

                            nextColumn.tiles.Add(tile);
                            movedTiles.Add(tile);

                        }

                    }

                }

                column.tiles = validTiles;
            }


        }

        public void MirrorVertically() {

            var newHeightOrder = new List<HeightPoints>();

            foreach (var hy in Enumerable.Range(0, 17)) {

                foreach (var hx in Enumerable.Range(0, 17)) {
                    newHeightOrder.Add(GetHeightPoint(hx, 16 - hy));
                }

            }

            heightMap = newHeightOrder;

            var newTileColum = new List<TileColumn>();

            foreach (var ty in Enumerable.Range(0, 16)) {

                foreach (var tx in Enumerable.Range(0, 16)) {
                    var column = tileColumns[((15 - ty) * 16) + tx];

                    var heights = new List<HeightPoints>();

                    heights.Add(GetHeightPoint(tx, ty));
                    heights.Add(GetHeightPoint(tx + 1, ty));
                    heights.Add(GetHeightPoint(tx, ty + 1));
                    heights.Add(GetHeightPoint(tx + 1, ty + 1));

                    column.x = tx;
                    column.y = ty;
                    column.heights = heights;

                    newTileColum.Add(column);
                }

            }

            tileColumns = newTileColum;

            var movedTiles = new List<Tile>();

            foreach (var column in tileColumns) {

                var validTiles = new List<Tile>();

                foreach (var tile in column.tiles) {

                    if (movedTiles.Contains(tile)) {
                        validTiles.Add(tile);
                        continue;
                    }

                    var result = tile.MirrorVerticesVertically();

                    if (result == Tile.TransformResult.Success) {

                        tile.MirrorUVsVertically();
                        tile.MirrorShadersVertically();

                        validTiles.Add(tile);

                    }
                    else if (result == Tile.TransformResult.MoveColumnPosY) {

                        if (column.y < 15) {

                            var nextColumn = tileColumns[((column.y + 1) * 16) + (column.x)];

                            tile.column = nextColumn;

                            nextColumn.tiles.Add(tile);
                            movedTiles.Add(tile);

                        }

                    }

                }

                column.tiles = validTiles;
            }


        }

        public void RotateClockwise() {

            var newHeightOrder = new List<HeightPoints>();

            foreach (var hy in Enumerable.Range(0, 17)) {

                foreach (var hx in Enumerable.Range(0, 17)) {

                    newHeightOrder.Add(GetHeightPoint(hy, 16 - hx));

                }

            }

            heightMap = newHeightOrder;

            var newTileColum = new List<TileColumn>();

            foreach (var ty in Enumerable.Range(0, 16)) {

                foreach (var tx in Enumerable.Range(0, 16)) {
                    var column = tileColumns[((15 - tx) * 16) + ty];

                    var heights = new List<HeightPoints>();

                    heights.Add(GetHeightPoint(tx, ty));
                    heights.Add(GetHeightPoint(tx + 1, ty));
                    heights.Add(GetHeightPoint(tx, ty + 1));
                    heights.Add(GetHeightPoint(tx + 1, ty + 1));

                    column.x = tx;
                    column.y = ty;
                    column.heights = heights;

                    newTileColum.Add(column);
                }

            }

            tileColumns = newTileColum;

            var movedTiles = new List<Tile>();

            foreach (var column in tileColumns) {

                var validTiles = new List<Tile>();

                foreach (var tile in column.tiles) {

                    if (movedTiles.Contains(tile)) {
                        validTiles.Add(tile);
                        continue;
                    }

                    var result = tile.RotateVerticesClockwise();

                    if (result == Tile.TransformResult.Success) {

                        tile.RotateUVsClockwise();
                        tile.RotateShadersClockwise();

                        validTiles.Add(tile);

                    }
                    else if (result == Tile.TransformResult.MoveColumnPosX) {

                        if (column.x < 15) {

                            var nextColumn = tileColumns[(column.y * 16) + (column.x + 1)];

                            tile.column = nextColumn;

                            nextColumn.tiles.Add(tile);
                            movedTiles.Add(tile);

                        }

                    }

                }

                column.tiles = validTiles;

            }

        }

        public void RotateCounterClockwise() {

            var newHeightOrder = new List<HeightPoints>();

            foreach (var hy in Enumerable.Range(0, 17)) {

                foreach (var hx in Enumerable.Range(0, 17)) {

                    newHeightOrder.Add(GetHeightPoint(16 - hy, hx));

                }

            }

            heightMap = newHeightOrder;

            var newTileColum = new List<TileColumn>();

            foreach (var ty in Enumerable.Range(0, 16)) {

                foreach (var tx in Enumerable.Range(0, 16)) {
                    var column = tileColumns[(tx * 16) + (15 - ty)];

                    var heights = new List<HeightPoints>();

                    heights.Add(GetHeightPoint(tx, ty));
                    heights.Add(GetHeightPoint(tx + 1, ty));
                    heights.Add(GetHeightPoint(tx, ty + 1));
                    heights.Add(GetHeightPoint(tx + 1, ty + 1));

                    column.x = tx;
                    column.y = ty;
                    column.heights = heights;

                    newTileColum.Add(column);
                }

            }

            tileColumns = newTileColum;

            var movedTiles = new List<Tile>();

            foreach (var column in tileColumns) {

                var validTiles = new List<Tile>();

                foreach (var tile in column.tiles) {

                    if (movedTiles.Contains(tile)) {
                        validTiles.Add(tile);
                        continue;
                    }

                    var result = tile.RotateVerticesCounterClockwise();

                    if (result == Tile.TransformResult.Success) {

                        tile.RotateUVsCounterClockwise();
                        tile.RotateShadersCounterClockwise();

                        validTiles.Add(tile);

                    }
                    else if (result == Tile.TransformResult.MoveColumnPosY) {

                        if (column.y < 15) {

                            var nextColumn = tileColumns[((column.y + 1) * 16) + (column.x)];

                            tile.column = nextColumn;

                            nextColumn.tiles.Add(tile);
                            movedTiles.Add(tile);

                        }

                    }

                }

                column.tiles = validTiles;

            }

        }

        public void DiscardUnusedHeights() {

            foreach (var y in Enumerable.Range(0, 17)) {

                foreach (var x in Enumerable.Range(0, 17)) {

                    var usedChannels = new HashSet<int>();

                    var heightPoint = GetHeightPoint(x, y);

                    TileColumn topLeft = null;
                    TileColumn topRight = null;
                    TileColumn bottomLeft = null;
                    TileColumn bottomRight = null;

                    if (x - 1 >= 0 && y - 1 >= 0) {
                        topLeft = GetTileColumn(x - 1, y - 1);
                    }

                    if (x < 16 && y - 1 >= 0) {
                        topRight = GetTileColumn(x, y - 1);
                    }

                    if (x - 1 >= 0 && y < 16) {
                        bottomLeft = GetTileColumn(x - 1, y);
                    }

                    if (x < 16 && y < 16) {
                        bottomRight = GetTileColumn(x, y);
                    }

                    if (topLeft != null) {
                        
                        foreach (var tile in topLeft.tiles) {

                            foreach (var vert in tile.verticies) {

                                if (vert.vertexPosition == VertexPosition.BottomRight) {
                                    usedChannels.Add(vert.heightChannel); 
                                }

                            }

                        }

                    }
                    if (topRight != null) {

                        foreach (var tile in topRight.tiles) {

                            foreach (var vert in tile.verticies) {

                                if (vert.vertexPosition == VertexPosition.BottomLeft) {
                                    usedChannels.Add(vert.heightChannel);
                                }

                            }

                        }

                    }
                    if (bottomLeft != null) {

                        foreach (var tile in bottomLeft.tiles) {

                            foreach (var vert in tile.verticies) {

                                if (vert.vertexPosition == VertexPosition.TopRight) {
                                    usedChannels.Add(vert.heightChannel);
                                }

                            }

                        }

                    }
                    if (bottomRight != null) {

                        foreach (var tile in bottomRight.tiles) {

                            foreach (var vert in tile.verticies) {

                                if (vert.vertexPosition == VertexPosition.TopLeft) {
                                    usedChannels.Add(vert.heightChannel);
                                }

                            }

                        }

                    }

                    if (!usedChannels.Contains(1)) {
                        heightPoint.SetPoint(HeightPoints.invalid, 1);
                    }
                    if (!usedChannels.Contains(2)) {
                        heightPoint.SetPoint(HeightPoints.invalid, 2);
                    }
                    if (!usedChannels.Contains(3)) {
                        heightPoint.SetPoint(HeightPoints.invalid, 3);
                    }

                }

            }

        }

        public void Overwrite(FCopLevelSection section) {

            heightMap.Clear();
            foreach (var newHeight in section.heightMap) {
                heightMap.Add(new HeightPoints(newHeight.height1, newHeight.height2, newHeight.height3));
            }

            tileColumns.Clear();
            var x = 0;
            var y = 0;
            foreach (var newColumn in section.tileColumns) {

                var newTiles = new List<Tile>();

                var heights = new List<HeightPoints>();

                heights.Add(GetHeightPoint(x, y));
                heights.Add(GetHeightPoint(x + 1, y));
                heights.Add(GetHeightPoint(x, y + 1));
                heights.Add(GetHeightPoint(x + 1, y + 1));

                var column = new TileColumn(x, y, newTiles, heights);

                foreach (var newTile in newColumn.tiles) {
                    newTiles.Add(new Tile(newTile, column, section));
                }

                tileColumns.Add(column);

                x++;
                if (x == 16) {
                    y++;
                    x = 0;
                }

            }

            colors.Clear();

            animationVector = new AnimationVector(section.animationVector.x, section.animationVector.y);

            foreach (var newColor in section.colors) {

                colors.Add(new XRGB555(newColor.x, newColor.r, newColor.g, newColor.b));

            }

            tileEffects = new List<byte>(section.tileEffects);
            if (section.slfxData != null) {
                slfxData = new List<byte>(section.slfxData);
            }

        }

        public void OverwriteHeights(FCopLevelSection section) {

            heightMap.Clear();
            foreach (var newHeight in section.heightMap) {
                heightMap.Add(new HeightPoints(newHeight.height1, newHeight.height2, newHeight.height3));
            }

            var oldTileColumns = new List<TileColumn>(tileColumns);

            tileColumns.Clear();
            var x = 0;
            var y = 0;
            var i = 0;
            foreach (var newColumn in section.tileColumns) {

                var newTiles = new List<Tile>();

                var heights = new List<HeightPoints>();

                heights.Add(GetHeightPoint(x, y));
                heights.Add(GetHeightPoint(x + 1, y));
                heights.Add(GetHeightPoint(x, y + 1));
                heights.Add(GetHeightPoint(x + 1, y + 1));

                var column = new TileColumn(x, y, newTiles, heights);

                foreach (var newTile in oldTileColumns[i].tiles) {
                    newTiles.Add(newTile);
                }

                tileColumns.Add(column);

                x++;
                if (x == 16) {
                    y++;
                    x = 0;
                }
                i++;

            }

            colors.Clear();

            animationVector = new AnimationVector(section.animationVector.x, section.animationVector.y);

            foreach (var newColor in section.colors) {

                colors.Add(new XRGB555(newColor.x, newColor.r, newColor.g, newColor.b));

            }

        }

        public void OverwriteTiles(FCopLevelSection section) {

            tileColumns.Clear();
            var x = 0;
            var y = 0;
            foreach (var newColumn in section.tileColumns) {

                var newTiles = new List<Tile>();

                var heights = new List<HeightPoints>();

                heights.Add(GetHeightPoint(x, y));
                heights.Add(GetHeightPoint(x + 1, y));
                heights.Add(GetHeightPoint(x, y + 1));
                heights.Add(GetHeightPoint(x + 1, y + 1));

                var column = new TileColumn(x, y, newTiles, heights);

                foreach (var newTile in newColumn.tiles) {
                    newTiles.Add(new Tile(newTile, column, section));
                }

                tileColumns.Add(column);

                x++;
                if (x == 16) {
                    y++;
                    x = 0;
                }

            }

            colors.Clear();

            animationVector = new AnimationVector(section.animationVector.x, section.animationVector.y);

            foreach (var newColor in section.colors) {

                colors.Add(new XRGB555(newColor.x, newColor.r, newColor.g, newColor.b));

            }

            tileEffects = new List<byte>(section.tileEffects);
            if (section.slfxData != null) {

                slfxData = new List<byte>(section.slfxData);

            }

        }

        public FCopLevelSection Clone() {

            var clone = new FCopLevelSection();

            clone.Overwrite(this);

            return clone;

        }

        public bool Compare(FCopLevelSection section) {

            var hi = 0;
            foreach (var height in heightMap) {

                if (!height.Compare(section.heightMap[hi])) {
                    return false;
                }

                hi++;
            }

            var ci = 0;
            foreach (var column in tileColumns) {

                var otherColumn = section.tileColumns[ci];

                if (column.tiles.Count != otherColumn.tiles.Count) {
                    return false;
                }

                // Tiles aren't sorted yet so we need to compare against all.
                foreach (var tile in column.tiles) {

                    var foundMatching = false;
                    foreach (var otherTile in otherColumn.tiles) {

                        if (tile.Compare(otherTile)) {
                            foundMatching = true;
                        }
                        
                    }

                    if (!foundMatching) {
                        return false;
                    }

                }

                ci++;
            }

            if (!animationVector.Compile().SequenceEqual(section.animationVector.Compile())) {
                return false;
            }

            if (!tileEffects.SequenceEqual(section.tileEffects)) {
                return false;
            }

            return true;

        }

    }

    public class HeightPoints {

        public const float multiplyer = 32f;
        public const float maxValue = SByte.MaxValue / multiplyer;
        public const float minValue = SByte.MinValue / multiplyer;
        public const int invalid = -128;

        // Man why did I make these floats way back when :(
        public float height1;
        public float height2;
        public float height3;

        public HeightPoints(float height1, float height2, float height3) {
            this.height1 = height1;
            this.height2 = height2;
            this.height3 = height3;
        }

        public HeightPoints(HeightPoint3 parsedHeightPoint3) {
            this.height1 = parsedHeightPoint3.height1 / multiplyer;
            this.height2 = parsedHeightPoint3.height2 / multiplyer;
            this.height3 = parsedHeightPoint3.height3 / multiplyer;
        }

        public HeightPoints(int height1, int height2, int height3) {
            this.height1 = height1 / multiplyer;
            this.height2 = height2 / multiplyer;
            this.height3 = height3 / multiplyer;
        }

        public float GetPoint(int channel) {

            switch(channel) {
                case 1: return height1;
                case 2: return height2;
                case 3: return height3;
                default: return 0;
            }

        }

        public int GetTruePoint(int index) {

            switch (index) {
                case 1: return (int)Math.Round(height1 * multiplyer);
                case 2: return (int)Math.Round(height2 * multiplyer);
                case 3: return (int)Math.Round(height3 * multiplyer);
                default: return 0;
            }

        }

        public void AddToPoint(float amount, int channel) {

            switch (channel) {
                case 1:
                    height1 += amount;

                    if (height1 > maxValue) {
                        height1 = maxValue;
                    } else if (height1 < minValue) {
                        height1 = minValue;
                    }

                    height1 = (float)Math.Round(height1 * multiplyer) / multiplyer;

                    break;
                case 2:
                    height2 += amount;

                    if (height2 > maxValue) {
                        height2 = maxValue;
                    } else if (height2 < minValue) {
                        height2 = minValue;
                    }

                    height2 = (float)Math.Round(height2 * multiplyer) / multiplyer;

                    break;
                case 3:
                    height3 += amount;

                    if (height3 > maxValue) {
                        height3 = maxValue;
                    } else if (height3 < minValue) {
                        height3 = minValue;
                    }

                    height3 = (float)Math.Round(height3 * multiplyer) / multiplyer;

                    break;
                default: break; 
            }

        }

        public void SetPoint(int value, int channel) {

            switch (channel) {
                case 1:

                    height1 = value / multiplyer;

                    if (height1 > maxValue) {
                        height1 = maxValue;
                    } else if (height1 < minValue) {
                        height1 = minValue;
                    }

                    break;
                case 2:

                    height2 = value / multiplyer;

                    if (height2 > maxValue) {
                        height2 = maxValue;
                    } else if (height2 < minValue) {
                        height2 = minValue;
                    }

                    break;
                case 3:

                    height3 = value / multiplyer;

                    if (height3 > maxValue) {
                        height3 = maxValue;
                    } else if (height3 < minValue) {
                        height3 = minValue;
                    }

                    break;
                default: break;
            }


        }

        public HeightPoint3 Compile() {
            return new HeightPoint3(
                (sbyte)Math.Round(height1 * multiplyer),
                (sbyte)Math.Round(height2 * multiplyer),
                (sbyte)Math.Round(height3 * multiplyer));

        }

        public HeightPoints Clone() {

            return new HeightPoints(height1, height2, height3);

        }

        public void ReceiveData(HeightPoints heights) {

            height1 = heights.height1;
            height2 = heights.height2;
            height3 = heights.height3;

        }

        public bool Compare(HeightPoints height) {

            return this.GetTruePoint(1) == height.GetTruePoint(1) &&
                    this.GetTruePoint(2) == height.GetTruePoint(2) && 
                    this.GetTruePoint(3) == height.GetTruePoint(3);

        }

    }

    // Columns form form left to right
    public class TileColumn {

        public int x;
        public int y;

        public List<Tile> tiles;

        public List<HeightPoints> heights;

        public TileColumn(int x, int y, List<Tile> tiles, List<HeightPoints> heights) {
            this.x = x;
            this.y = y;
            this.tiles = tiles;
            this.heights = heights;
        }

        // Note that tiles are NOT cloned
        public TileColumn CloneWithHeights() {

            var newHeights = new List<HeightPoints>();

            foreach (var height in heights) {
                newHeights.Add(height.Clone());
            }

            return new TileColumn(x, y, new(tiles), newHeights);

        }

    }

    // Tiles are sorted into 4x4 chunks
    public class Tile {

        public TileColumn column;

        public bool isEndInColumnArray;
        // tile vertex ordering Top-Left, Top-Right, Bottom-Left, Bottom-Right
        public List<TileVertex> verticies;

        // Something important to note, uvs order is different than tile vertices.
        // It goes from Top-Left, Top-Right, Bottom-Right, Bottom-Left
        public List<int> uvs = new();
        public TileShaders shaders;
        public List<int> animatedUVs = new();
        public int animationSpeed = -1;

        public int texturePalette;
        public bool isVectorAnimated;
        public bool isSemiTransparent;
        public int culling;
        public int effectIndex;

        public bool isQuad {
            get { return verticies.Count == 4; }
        }

        // Original parsed data from file
        TileGraphics graphics;
        List<TileGraphicsMetaData> graphicsMetaData = new();
        TileUVAnimationMetaData? uvAnimationData = null;
        TileBitfield parsedTile;

        public Tile(TileBitfield parsedTile, TileColumn column, FCopLevelSectionParser section) {


            this.column = column;

            isEndInColumnArray = parsedTile.isEndInColumnArray == 1;

            verticies = MeshType.VerticiesFromID(parsedTile.meshID);

            culling = parsedTile.culling;
            effectIndex = parsedTile.number4;

            var textureIndex = parsedTile.textureIndex;
            var graphicsIndex = parsedTile.graphicIndex;

            graphics = (TileGraphics)section.tileGraphics[graphicsIndex];

            if (graphics.graphicsType == 1) {
                graphicsMetaData.Add((TileGraphicsMetaData)section.tileGraphics[graphicsIndex + 1]);
            }
            else if (graphics.graphicsType == 2) {

                if (graphics.isRect == 1) {
                    graphicsMetaData.Add((TileGraphicsMetaData)section.tileGraphics[graphicsIndex + 1]);
                    graphicsMetaData.Add((TileGraphicsMetaData)section.tileGraphics[graphicsIndex + 2]);
                }
                else {
                    graphicsMetaData.Add((TileGraphicsMetaData)section.tileGraphics[graphicsIndex + 1]);
                }

            }

            var shaderData = new List<byte>();

            foreach (var meta in graphicsMetaData) {
                shaderData.AddRange(meta.data);
            }

            shaderData.Add((byte)graphics.lightingInfo);

            switch ((VertexColorType)graphics.graphicsType) {
                case VertexColorType.MonoChrome:
                    shaders = new MonoChromeShader(shaderData[0], graphics.isRect == 1);
                    break;
                case VertexColorType.DynamicMonoChrome:
                    shaders = new DynamicMonoChromeShader(shaderData, graphics.isRect == 1);
                    break;
                case VertexColorType.Color:
                    shaders = new ColorShader(shaderData, section, graphics.isRect == 1);
                    break;
                case VertexColorType.ColorAnimated:
                    shaders = new AnimatedShader(graphics.isRect == 1);
                    break;
            }


            foreach (var i in Enumerable.Range(textureIndex, verticies.Count)) {

                uvs.Add(section.textureCoordinates[i]);

            }

            texturePalette = graphics.cbmpID;
            isVectorAnimated = graphics.isAnimated == 1;
            isSemiTransparent = graphics.isSemiTransparent == 1;

            #region ParseAnimationData

            if (section.tileUVAnimationMetaData.Count == 0) {
                return;
            }

            foreach (var metaData in section.tileUVAnimationMetaData) {

                if (metaData.textureReplaceOffset / 2 == textureIndex) {
                    uvAnimationData = metaData;

                    var frameUVs = section.animatedTextureCoordinates.GetRange(metaData.animationOffset / 2, metaData.frames * 4);

                    animatedUVs.AddRange(frameUVs);

                    ReverseAnimatedFrames();

                    break;
                }

            }

            if (uvAnimationData != null) {
                animationSpeed = uvAnimationData.Value.frameDuration;
            }

            #endregion

            this.parsedTile = parsedTile;

        }

        public Tile(Tile tile, TileColumn column, FCopLevelSection section) {

            this.column = column;

            isEndInColumnArray = tile.isEndInColumnArray;

            verticies = new List<TileVertex>(tile.verticies);
            uvs = new List<int>(tile.uvs);
            shaders = tile.shaders.Clone(verticies.Count == 4);
            animatedUVs = new List<int>(tile.animatedUVs);
            animationSpeed = tile.animationSpeed;

            culling = tile.culling;
            texturePalette = tile.texturePalette;
            isVectorAnimated = tile.isVectorAnimated;
            isSemiTransparent = tile.isSemiTransparent;
            effectIndex = tile.effectIndex;

            graphics = tile.graphics;
            graphicsMetaData = new(tile.graphicsMetaData);
            uvAnimationData = tile.uvAnimationData;
            this.parsedTile = tile.parsedTile;

        }

        // This is only used for flattening a level.
        public Tile(TileColumn column, List<TileVertex> vertices, int culling, List<int> uvs, TileGraphics graphics) {
            this.column = column;
            isEndInColumnArray = true;
            this.verticies = vertices;
            this.culling = culling;
            this.uvs = uvs;
            this.texturePalette = graphics.cbmpID;

            isVectorAnimated = false;
            isSemiTransparent = false;
            effectIndex = 0;

            shaders = new MonoChromeShader(116, vertices.Count == 4);

        }

        public Tile(TileColumn column, int meshID, int culling) {

            this.column = column;
            verticies = MeshType.VerticiesFromID(meshID);
            this.culling = culling;

            if (verticies.Count == 4) {
                this.uvs = new() { 57200, 57228, 50060, 50032 };
            }
            else {
                this.uvs = new() { 57200, 57228, 50060 };
            }

            this.texturePalette = 6;

            isVectorAnimated = false;
            isSemiTransparent = false;
            effectIndex = 0;

            shaders = new MonoChromeShader(116, verticies.Count == 4);

        }

        public Tile(TileColumn column, int meshID, int culling, int effectIndex, 
            List<int> uvs, int texturePalette, bool isSemiTransparent, bool isVectorAnimated, 
            TileShaders shaders, int animationSpeed, List<int> animatedUVs) {

            this.column = column;
            verticies = MeshType.VerticiesFromID(meshID);
            this.culling = culling;
            this.effectIndex = effectIndex;
            this.uvs = new(uvs);
            this.texturePalette = texturePalette;
            this.isSemiTransparent = isSemiTransparent;
            this.isVectorAnimated = isVectorAnimated;
            this.shaders = shaders;
            this.animationSpeed = animationSpeed;
            this.animatedUVs = new(animatedUVs);

        }

        public void ReceiveData(Tile tile, bool updateColumn = true) {

            if (updateColumn) {
                this.column = tile.column;
            }

            isEndInColumnArray = tile.isEndInColumnArray;

            verticies = new List<TileVertex>(tile.verticies);
            uvs = new List<int>(tile.uvs);
            shaders = tile.shaders.Clone(verticies.Count == 4);
            animatedUVs = new List<int>(tile.animatedUVs);
            animationSpeed = tile.animationSpeed;

            culling = tile.culling;
            texturePalette = tile.texturePalette;
            isVectorAnimated = tile.isVectorAnimated;
            isSemiTransparent = tile.isSemiTransparent;
            effectIndex = tile.effectIndex;

            graphics = tile.graphics;
            graphicsMetaData = new(tile.graphicsMetaData);
            uvAnimationData = tile.uvAnimationData;
            this.parsedTile = tile.parsedTile;

        }

        public Tile Clone() {
            return new Tile(this, column, null);
        }

        public int GetMaxHeight() {

            int minHeight = 128;
            int maxHeight = -128;

            foreach (var vert in verticies) {

                var height = column.heights[((int)vert.vertexPosition) - 1];

                var value = height.GetTruePoint(vert.heightChannel);

                if (value < minHeight) {
                    minHeight = value;
                }
                if (value > maxHeight) {
                    maxHeight = value;
                }

            }

            return maxHeight;

        }

        public int GetFrameCount() {
            return animatedUVs.Count / 4;
        }

        public bool IsFrameAnimated() {
            return animatedUVs.Count > 0;
        }

        public void ReverseAnimatedFrames() {

            var newOrderedFrames = new List<int>();

            var i = GetFrameCount() - 1;
            foreach (var _ in Enumerable.Range(0, GetFrameCount())) {
                newOrderedFrames.AddRange(animatedUVs.GetRange(i * 4, 4));
                i--;
            }

            animatedUVs = newOrderedFrames;

        }

        public void ChangeShader(VertexColorType type) {

            switch (type) {

                case VertexColorType.MonoChrome:
                    shaders = new MonoChromeShader(verticies.Count == 4);
                    break;
                case VertexColorType.DynamicMonoChrome:
                    shaders = new DynamicMonoChromeShader(shaders);
                    break;
                case VertexColorType.Color:
                    shaders = new ColorShader(shaders);
                    break;
                case VertexColorType.ColorAnimated:
                    shaders = new AnimatedShader(verticies.Count == 4);
                    break;

            }

        }

        public TileBitfield Compile(int textureIndex, int graphicsIndex) {

            var id = MeshType.IDFromVerticies(verticies);

            if (id == null) {
                throw new MeshIDException();
            }

            parsedTile.isEndInColumnArray = isEndInColumnArray ? 1 : 0;
            parsedTile.meshID = (int)id;
            parsedTile.textureIndex = textureIndex;
            parsedTile.culling = culling;
            parsedTile.number4 = effectIndex;
            parsedTile.graphicIndex = graphicsIndex;

            return parsedTile;

        }

        public List<TileGraphicsItem> CompileGraphics(Dictionary<ushort, (int, XRGB555)> existingColors) {

            var isRect = verticies.Count == 4;

            var potentialNewShaders = shaders.VerifyCorrectShader();

            if (potentialNewShaders != null) {
                shaders = potentialNewShaders;
            }

            var shaderData = new List<byte>();

            if (shaders.type == VertexColorType.Color) {
                shaderData = ((ColorShader)shaders).ColorCompile(existingColors);
            } else {
                shaderData = shaders.Compile();
            }

            var graphicItems = new List<TileGraphicsItem>();
            var graphic = new TileGraphics(graphics.lightingInfo, texturePalette, isVectorAnimated ? 1 : 0, isSemiTransparent ? 1 : 0, isRect ? 1 : 0, (int)shaders.type);

            // If no shader data just uses the existing data
            if (shaderData.Count == 0) {

                graphicItems.Add(graphic);
                foreach (var metaData in graphicsMetaData) {
                    graphicItems.Add(metaData);
                }

            } else {

                graphic.lightingInfo = shaderData.Last();

                graphicItems.Add(graphic);

                if (shaderData.Count > 1) {

                    foreach (var i in Enumerable.Range(0, shaderData.Count / 2)) {
                        graphicItems.Add(new TileGraphicsMetaData(shaderData.GetRange(i * 2, 2)));
                    }

                }

            }

            return graphicItems;

        }

        public TileUVAnimationMetaData CompileFrameAnimation(int animationOffset, int textureReplaceOffset) {
            ReverseAnimatedFrames();
            var metaData = new TileUVAnimationMetaData(GetFrameCount(), 9, animationSpeed, animationOffset, textureReplaceOffset);
            uvAnimationData = metaData;
            return metaData;
        }

        // Data NOT compared:
        // animationSpeed, culling, isSemiTransparent
        public bool Compare(Tile tile) {

            var thisID = MeshType.IDFromVerticies(verticies);
            var otherID = MeshType.IDFromVerticies(tile.verticies);

            if (thisID == null || otherID == null) {
                throw new MeshIDException();
            }

            if (thisID != otherID) {
                return false;
            }

            if (!this.uvs.SequenceEqual(tile.uvs)) {
                return false;
            }

            if (this.shaders.type != tile.shaders.type) {
                return false;
            }

            if (this.animatedUVs.Count != tile.animatedUVs.Count) {
                return false;
            }

            if (this.animatedUVs.Count != 0) {
                if (!this.animatedUVs.SequenceEqual(tile.animatedUVs)) {
                    return false;
                }
            }

            if (this.texturePalette != tile.texturePalette) {
                return false;
            }

            if (this.isVectorAnimated != tile.isVectorAnimated) {
                return false;
            }

            if (this.effectIndex != tile.effectIndex) {
                return false;
            }

            return true;

        }

        #region Transforming

        // Call the UV and shader transforming methods AFTER the vert transforming methods

        public enum TransformResult {
            Success,
            Invalid,
            MoveColumnPosX,
            MoveColumnPosY,
            MoveColumnNegX,
            MoveColumnNegY

        }

        public TransformResult MoveHeightChannelsToNextChannel() {

            var previousVerticies = new List<TileVertex>(verticies);
            var newVerticies = new HashSet<TileVertex>();

            foreach (var index in Enumerable.Range(0, verticies.Count)) {

                var vertex = verticies[index];

                if (vertex.heightChannel < 3) {

                    vertex.heightChannel += 1;

                    newVerticies.Add(vertex);

                }

            }

            if (newVerticies.Count == previousVerticies.Count) {
                verticies = newVerticies.ToList();
                return TransformResult.Success;
            }
            else {
                return TransformResult.Invalid;

            }

        }

        Tile MakeTriTile(List<int> vertOrder, List<int> UVOrder) {

            var meshID = MeshType.IDFromVerticies(new() {
                    verticies[vertOrder[0]],
                    verticies[vertOrder[1]],
                    verticies[vertOrder[2]],
                });

            Tile tile = null;

            if (meshID != null) {
                tile = new Tile(column, (int)meshID, culling);
            }

            if (tile != null) {

                tile.uvs = new List<int> { uvs[UVOrder[0]], uvs[UVOrder[1]], uvs[UVOrder[2]] };
                tile.texturePalette = texturePalette;
                tile.isVectorAnimated = isVectorAnimated;
                tile.isSemiTransparent = isSemiTransparent;
                tile.effectIndex = effectIndex;

                switch (shaders.type) {
                    case VertexColorType.MonoChrome:

                        var thisSolidMono = (MonoChromeShader)shaders;

                        tile.shaders = new MonoChromeShader(false);
                        var solidMono = (MonoChromeShader)tile.shaders;

                        solidMono.value = thisSolidMono.value;
                        solidMono.Apply();

                        break;
                    case VertexColorType.DynamicMonoChrome:

                        var thisMono = (DynamicMonoChromeShader)shaders;

                        tile.shaders = new DynamicMonoChromeShader(false);
                        var mono = (DynamicMonoChromeShader)tile.shaders;

                        // Remember that dynamic mono stores 4 regardless of shape
                        mono.values = new int[] {
                                thisMono.values[UVOrder[0]],
                                thisMono.values[UVOrder[1]],
                                thisMono.values[UVOrder[2]],
                                thisMono.values[UVOrder[0]]
                            };
                        mono.Apply();

                        break;
                    case VertexColorType.Color:

                        var thisColor = (ColorShader)shaders;

                        tile.shaders = new ColorShader(false);
                        var color = (ColorShader)tile.shaders;

                        color.values[ColorShader.uvOrderedTriIndexes[0]] = thisColor.values[ColorShader.uvOrderedQuadIndexes[UVOrder[0]]].Clone();
                        color.values[ColorShader.uvOrderedTriIndexes[1]] = thisColor.values[ColorShader.uvOrderedQuadIndexes[UVOrder[1]]].Clone();
                        color.values[ColorShader.uvOrderedTriIndexes[2]] = thisColor.values[ColorShader.uvOrderedQuadIndexes[UVOrder[2]]].Clone();

                        color.Apply();

                        break;
                    case VertexColorType.ColorAnimated:
                        tile.shaders = shaders.Clone(false);
                        tile.shaders.isQuad = false;
                        break;
                }


            }

            return tile;

        }

        public List<Tile> BreakApartQuadTileBottomTop() {

            var total = new List<Tile>();

            if (verticies.Count == 3) {
                return null;
            }

            int originalMeshID = (int)MeshType.IDFromVerticies(verticies);

            Tile tile1;
            Tile tile2;

            if (MeshType.topWallMeshes.Contains(originalMeshID) || MeshType.diagonalTLeftBRightQuadWallMeshes.Contains(originalMeshID)) {

                tile1 = MakeTriTile(
                    new() { 0, 1, 3 },
                    new() { 0, 1, 2 }
                );

                tile2 = MakeTriTile(
                    new() { 0, 3, 2 },
                    new() { 0, 2, 3 }
                );

            }
            else if (MeshType.diagonalBLeftTRightQuadWallMeshes.Contains(originalMeshID)) {

                tile1 = MakeTriTile(
                    new() { 0, 1, 3 },
                    new() { 3, 1, 2 }
                );

                tile2 = MakeTriTile(
                    new() { 0, 3, 2 },
                    new() { 3, 0, 1 }
                );

            }
            else {

                tile1 = MakeTriTile(
                    new() { (int)VertexPosition.TopLeft - 1, (int)VertexPosition.TopRight - 1, (int)VertexPosition.BottomLeft - 1 },
                    new() { 0, 1, 3 }
                );

                tile2 = MakeTriTile(
                    new() { (int)VertexPosition.TopRight - 1, (int)VertexPosition.BottomRight - 1, (int)VertexPosition.BottomLeft - 1 },
                    new() { 1, 2, 3 }
                );

            }

            if (tile1 != null) {
                total.Add(tile1);
            }

            if (tile2 != null) {
                total.Add(tile2);
            }

            return total;

        }

        public List<Tile> BreakApartQuadTileTopBottom() {

            var total = new List<Tile>();

            if (verticies.Count == 3) {
                return null;
            }

            int originalMeshID = (int)MeshType.IDFromVerticies(verticies);

            Tile tile1 = null;
            Tile tile2 = null;

            if (MeshType.topWallMeshes.Contains(originalMeshID) || MeshType.diagonalTLeftBRightQuadWallMeshes.Contains(originalMeshID)) {

                tile1 = MakeTriTile(
                    new() { 2, 3, 1 },
                    new() { 1, 2, 3 }
                );

                tile2 = MakeTriTile(
                    new() { 2, 0, 1 },
                    new() { 0, 1, 3 }
                );

            }
            else if (MeshType.diagonalBLeftTRightQuadWallMeshes.Contains(originalMeshID)) {

                tile1 = MakeTriTile(
                    new() { 2, 3, 1 },
                    new() { 1, 3, 2 }
                );

                tile2 = MakeTriTile(
                    new() { 2, 0, 1 },
                    new() { 0, 3, 1 }
                );

            }
            else {

                tile1 = MakeTriTile(
                    new() { (int)VertexPosition.TopLeft - 1, (int)VertexPosition.TopRight - 1, (int)VertexPosition.BottomRight - 1 },
                    new() { 0, 1, 2 }
                );

                tile2 = MakeTriTile(
                    new() { (int)VertexPosition.TopLeft - 1, (int)VertexPosition.BottomRight - 1, (int)VertexPosition.BottomLeft - 1 },
                    new() { 0, 2, 3 }
                );

            }

            if (tile1 != null) {
                total.Add(tile1);
            }

            if (tile2 != null) {
                total.Add(tile2);
            }

            return total;

        }

        // - Mirror Vertically -

        public TransformResult MirrorVerticesVertically() {

            int ogMeshID = (int)MeshType.IDFromVerticies(verticies);

            var mirrorVertices = new List<TileVertex>();

            foreach (var vertex in verticies) {

                switch (vertex.vertexPosition) {

                    case VertexPosition.TopLeft:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomLeft));
                        break;
                    case VertexPosition.TopRight:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomRight));
                        break;
                    case VertexPosition.BottomLeft:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                        break;
                    case VertexPosition.BottomRight:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopRight));
                        break;

                }

            }

            var mirorVID = MeshType.IDFromVerticies(mirrorVertices);

            if (mirorVID != null) {

                verticies = MeshType.VerticiesFromID((int)mirorVID);

                return TransformResult.Success;

            }
            else {

                if (MeshType.topWallMeshes.Contains(ogMeshID)) {

                    return TransformResult.MoveColumnPosY;

                }

                return TransformResult.Invalid;

            }

        }

        public void MirrorUVsVertically() {

            // Oddly enough if the order of the UVs are just reversed the textures mirror perfectly for most tiles

            int meshID = (int)MeshType.IDFromVerticies(verticies);

            // Walls
            if (MeshType.wallMeshes.Contains(meshID)) {

                if (MeshType.leftWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                }
                else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                    RotateUVOrderCounterClockwise();
                }
                else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                    RotateUVOrderCounterClockwise();
                }

                return;

            }
            
            FlipUVOrderVertically();

        }

        public void MirrorShadersVertically() {

            int meshID = (int)MeshType.IDFromVerticies(verticies);

            if (shaders is DynamicMonoChromeShader) {

                if (MeshType.wallMeshes.Contains(meshID)) {

                    if (MeshType.leftWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                    }
                    else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                        RotateMonoShaderOrderCounterClockwise();
                    }
                    else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                        RotateMonoShaderOrderCounterClockwise();
                    }

                    return;

                }

                FlipDynamicMonoShaderOrderVertically();

            }
            if (shaders is ColorShader) {

                if (MeshType.wallMeshes.Contains(meshID)) {

                    if (MeshType.leftWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                    }
                    else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                        RotateColorShaderOrderCounterClockwise();
                    }
                    else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                        RotateColorShaderOrderCounterClockwise();
                    }

                    return;

                }

                FlipColorShaderOrderVertically();

            }

        }

        // - Mirror Horizontally -

        public TransformResult MirrorVerticesHorizontally() {

            int ogMeshID = (int)MeshType.IDFromVerticies(verticies);

            var mirrorVertices = new List<TileVertex>();

            foreach (var vertex in verticies) {

                switch (vertex.vertexPosition) {

                    case VertexPosition.TopLeft:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopRight));
                        break;
                    case VertexPosition.TopRight:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                        break;
                    case VertexPosition.BottomLeft:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomRight));
                        break;
                    case VertexPosition.BottomRight:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomLeft));
                        break;

                }

            }

            var mirorVID = MeshType.IDFromVerticies(mirrorVertices);

            if (mirorVID != null) {

                verticies = MeshType.VerticiesFromID((int)mirorVID);

                return TransformResult.Success;

            }
            else {

                if (MeshType.leftWallMeshes.Contains(ogMeshID)) {

                    return TransformResult.MoveColumnPosX;

                }

                return TransformResult.Invalid; 

            }

        }

        public void MirrorUVsHorizontally() {

            int meshID = (int)MeshType.IDFromVerticies(verticies);

            // Walls
            if (MeshType.wallMeshes.Contains(meshID)) {

                if (MeshType.topWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                }
                else if (MeshType.diagonalBLeftTRightQuadWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                }
                else if (MeshType.diagonalTLeftBRightQuadWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                }
                else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                    RotateUVOrderClockwise();
                }
                else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                    RotateUVOrderCounterClockwise();
                }

                return;

            }

            if (MeshType.bottomLeftTriangles.Contains(meshID)) {

                FlipUVOrderVertically();
                RotateUVOrderCounterClockwise();

                return;

            }
            if (MeshType.bottomRightTriangles.Contains(meshID)) {

                FlipUVOrderVertically();
                RotateUVOrderCounterClockwise();

                return;

            }

            FlipUVOrderHorizontally();

        }

        public void MirrorShadersHorizontally() {

            int meshID = (int)MeshType.IDFromVerticies(verticies);

            if (shaders is DynamicMonoChromeShader) {

                if (MeshType.wallMeshes.Contains(meshID)) {

                    if (MeshType.topWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                    }
                    else if (MeshType.diagonalBLeftTRightQuadWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                    }
                    else if (MeshType.diagonalTLeftBRightQuadWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                    }
                    else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                        RotateMonoShaderOrderClockwise();
                    }
                    else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                        RotateMonoShaderOrderCounterClockwise();
                    }

                    return;

                }

                if (MeshType.bottomLeftTriangles.Contains(meshID)) {

                    FlipDynamicMonoShaderOrderVertically();
                    RotateMonoShaderOrderCounterClockwise();

                    return;

                }
                if (MeshType.bottomRightTriangles.Contains(meshID)) {

                    FlipDynamicMonoShaderOrderVertically();
                    RotateMonoShaderOrderCounterClockwise();

                    return;

                }

                FlipDynamicMonoShaderOrderHorizontally();

            }
            if (shaders is ColorShader) {

                if (MeshType.wallMeshes.Contains(meshID)) {

                    if (MeshType.topWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                    }
                    else if (MeshType.diagonalBLeftTRightQuadWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                    }
                    else if (MeshType.diagonalTLeftBRightQuadWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                    }
                    else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                        RotateColorShaderOrderClockwise();
                    }
                    else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                        RotateColorShaderOrderCounterClockwise();
                    }

                    return;

                }

                if (MeshType.bottomLeftTriangles.Contains(meshID)) {

                    FlipColorShaderOrderVertically();
                    RotateColorShaderOrderCounterClockwise();

                    return;

                }
                if (MeshType.bottomRightTriangles.Contains(meshID)) {

                    FlipColorShaderOrderVertically();
                    RotateColorShaderOrderCounterClockwise();

                    return;

                }

                FlipColorShaderOrderHorizontally();

            }

        }

        // - Rotate Clockwise -

        public TransformResult RotateVerticesClockwise() {

            int ogMeshID = (int)MeshType.IDFromVerticies(verticies);

            var mirrorVertices = new List<TileVertex>();

            foreach (var vertex in verticies) {

                switch (vertex.vertexPosition) {

                    case VertexPosition.TopLeft:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopRight));
                        break;
                    case VertexPosition.TopRight:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomRight));
                        break;
                    case VertexPosition.BottomLeft:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                        break;
                    case VertexPosition.BottomRight:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomLeft));
                        break;

                }

            }

            var mirrorVID = MeshType.IDFromVerticies(mirrorVertices);

            if (mirrorVID != null) {

                verticies = MeshType.VerticiesFromID((int)mirrorVID);

                return TransformResult.Success;

            }
            else {

                // Because right walls don't exist it needs to see if making the right wall a left wall is possible
                if (MeshType.topWallMeshes.Contains(ogMeshID)) {

                    var wallMirrorVertices = new List<TileVertex>();

                    foreach (var vertex in mirrorVertices) {

                        switch (vertex.vertexPosition) {

                            case VertexPosition.TopLeft:
                                wallMirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                                break;
                            case VertexPosition.TopRight:
                                wallMirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                                break;
                            case VertexPosition.BottomLeft:
                                wallMirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomLeft));
                                break;
                            case VertexPosition.BottomRight:
                                wallMirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomLeft));
                                break;

                        }

                    }

                    mirrorVID = MeshType.IDFromVerticies(wallMirrorVertices);

                    if (mirrorVID != null) {

                        verticies = MeshType.VerticiesFromID((int)mirrorVID);

                        return TransformResult.MoveColumnPosX;

                    }


                }

                return TransformResult.Invalid;

            }

        }

        public void RotateUVsClockwise() {

            int meshID = (int)MeshType.IDFromVerticies(verticies);

            // Walls
            if (MeshType.wallMeshes.Contains(meshID)) {

                if (MeshType.topWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                }
                else if (MeshType.diagonalBLeftTRightQuadWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                }
                else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                    RotateUVOrderCounterClockwise();
                }
                else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                    RotateUVOrderCounterClockwise();
                }

                return;

            }

            // I guess top right and bottom right triangles use the same UV order?
            if (MeshType.bottomRightTriangles.Contains(meshID)) {

                return;

            }

            RotateUVOrderCounterClockwise();

        }

        public void RotateShadersClockwise() {

            int meshID = (int)MeshType.IDFromVerticies(verticies);

            if (shaders is DynamicMonoChromeShader) {

                // Walls
                if (MeshType.wallMeshes.Contains(meshID)) {

                    if (MeshType.topWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                    }
                    else if (MeshType.diagonalBLeftTRightQuadWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                    }
                    else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                        RotateMonoShaderOrderCounterClockwise();
                    }
                    else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                        RotateMonoShaderOrderCounterClockwise();
                    }

                    return;

                }

                // I guess top right and bottom right triangles use the same UV order?
                if (MeshType.bottomRightTriangles.Contains(meshID)) {

                    return;

                }

                RotateMonoShaderOrderCounterClockwise();

            }
            if (shaders is ColorShader) {

                // Walls
                if (MeshType.wallMeshes.Contains(meshID)) {

                    if (MeshType.topWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                    }
                    else if (MeshType.diagonalBLeftTRightQuadWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                    }
                    else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                        RotateColorShaderOrderCounterClockwise();
                    }
                    else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                        RotateColorShaderOrderCounterClockwise();
                    }

                    return;

                }

                // I guess top right and bottom right triangles use the same UV order?
                if (MeshType.bottomRightTriangles.Contains(meshID)) {

                    return;

                }

                RotateColorShaderOrderCounterClockwise();

            }

        }

        // - Rotate Counter-Clockwise -

        public TransformResult RotateVerticesCounterClockwise() {

            int ogMeshID = (int)MeshType.IDFromVerticies(verticies);

            var mirrorVertices = new List<TileVertex>();

            foreach (var vertex in verticies) {

                switch (vertex.vertexPosition) {

                    case VertexPosition.TopLeft:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomLeft));
                        break;
                    case VertexPosition.TopRight:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                        break;
                    case VertexPosition.BottomLeft:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.BottomRight));
                        break;
                    case VertexPosition.BottomRight:
                        mirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopRight));
                        break;

                }

            }

            var mirrorVID = MeshType.IDFromVerticies(mirrorVertices);

            if (mirrorVID != null) {

                verticies = MeshType.VerticiesFromID((int)mirrorVID);

                return TransformResult.Success;

            }
            else {

                // Because bottom walls don't exist it needs to see if making the bottom wall a top wall is possible
                if (MeshType.leftWallMeshes.Contains(ogMeshID)) {

                    var wallMirrorVertices = new List<TileVertex>();

                    foreach (var vertex in mirrorVertices) {

                        switch (vertex.vertexPosition) {

                            case VertexPosition.TopLeft:
                                wallMirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                                break;
                            case VertexPosition.TopRight:
                                wallMirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopRight));
                                break;
                            case VertexPosition.BottomLeft:
                                wallMirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopLeft));
                                break;
                            case VertexPosition.BottomRight:
                                wallMirrorVertices.Add(new TileVertex(vertex.heightChannel, VertexPosition.TopRight));
                                break;

                        }

                    }

                    mirrorVID = MeshType.IDFromVerticies(wallMirrorVertices);

                    if (mirrorVID != null) {

                        verticies = MeshType.VerticiesFromID((int)mirrorVID);

                        return TransformResult.MoveColumnPosY;

                    }


                }

                return TransformResult.Invalid;

            }

        }

        public void RotateUVsCounterClockwise() {

            int meshID = (int)MeshType.IDFromVerticies(verticies);

            // Walls
            if (MeshType.wallMeshes.Contains(meshID)) {

                if (MeshType.leftWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                }
                else if (MeshType.diagonalTLeftBRightQuadWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                }
                else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                    RotateUVOrderClockwise();
                }
                else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                    FlipUVOrderVertically();
                    RotateUVOrderCounterClockwise();
                }

                return;

            }

            // Order stays the same
            if (MeshType.topRightTriangles.Contains(meshID)) {

                return;

            }

            RotateUVOrderClockwise();

        }

        public void RotateShadersCounterClockwise() {

            int meshID = (int)MeshType.IDFromVerticies(verticies);

            if (shaders is DynamicMonoChromeShader) {

                // Walls
                if (MeshType.wallMeshes.Contains(meshID)) {

                    if (MeshType.leftWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                    }
                    else if (MeshType.diagonalTLeftBRightQuadWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                    }
                    else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                        RotateMonoShaderOrderClockwise();
                    }
                    else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                        FlipDynamicMonoShaderOrderVertically();
                        RotateMonoShaderOrderCounterClockwise();
                    }

                    return;

                }

                // Order stays the same
                if (MeshType.topRightTriangles.Contains(meshID)) {

                    return;

                }

                RotateMonoShaderOrderClockwise();

            }
            if (shaders is ColorShader) {

                // Walls
                if (MeshType.wallMeshes.Contains(meshID)) {

                    if (MeshType.leftWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                    }
                    else if (MeshType.diagonalTLeftBRightQuadWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                    }
                    else if (MeshType.diagonalTLeftBRightTriWallMeshes.Contains(meshID)) {
                        RotateColorShaderOrderClockwise();
                    }
                    else if (MeshType.diagonalBLeftTRightTriWallMeshes.Contains(meshID)) {
                        FlipColorShaderOrderVertically();
                        RotateColorShaderOrderCounterClockwise();
                    }

                    return;

                }

                // Order stays the same
                if (MeshType.topRightTriangles.Contains(meshID)) {

                    return;

                }

                RotateColorShaderOrderClockwise();

            }

        }

        // - Util -

        // - UVs -
        void FlipUVPositionHorizontally() {

            var uvVectors = new List<int[]>();

            foreach (var uv in uvs) {
                uvVectors.Add(TextureCoordinate.GetVector(uv));
            }

            float minX = uvVectors.Min(v => v[0]);
            float maxX = uvVectors.Max(v => v[0]);

            float width = maxX - minX;
            var center = width / 2f;

            foreach (var uv in uvVectors) {

                var localX = uv[0] - minX;

                var distanceFromCenter = localX - center;

                var vFlippedX = center - distanceFromCenter;

                uv[0] = (int)(minX + vFlippedX);

            }

            foreach (var i in Enumerable.Range(0, uvs.Count)) {
                uvs[i] = TextureCoordinate.SetPixel(uvVectors[i][0], uvVectors[i][1]);
            }

        }

        void FlipUVOrderVertically() {

            var uvVectors = new List<int[]>();

            foreach (var uv in uvs) {
                uvVectors.Add(TextureCoordinate.GetVector(uv));
            }

            var newUVs = new List<int[]>();

            if (uvs.Count == 4) {

                newUVs.Add(uvVectors[3]);
                newUVs.Add(uvVectors[2]);
                newUVs.Add(uvVectors[1]);
                newUVs.Add(uvVectors[0]);

            }
            else {

                newUVs.Add(uvVectors[2]);
                newUVs.Add(uvVectors[1]);
                newUVs.Add(uvVectors[0]);

            }

            foreach (var i in Enumerable.Range(0, uvs.Count)) {
                uvs[i] = TextureCoordinate.SetPixel(newUVs[i][0], newUVs[i][1]);
            }

        }

        void FlipUVOrderHorizontally() {

            var uvVectors = new List<int[]>();

            foreach (var uv in uvs) {
                uvVectors.Add(TextureCoordinate.GetVector(uv));
            }

            var newUVs = new List<int[]>();

            if (uvs.Count == 4) {

                newUVs.Add(uvVectors[1]);
                newUVs.Add(uvVectors[0]);
                newUVs.Add(uvVectors[3]);
                newUVs.Add(uvVectors[2]);

            }
            else {

                newUVs.Add(uvVectors[1]);
                newUVs.Add(uvVectors[0]);
                newUVs.Add(uvVectors[2]);

            }

            foreach (var i in Enumerable.Range(0, uvs.Count)) {
                uvs[i] = TextureCoordinate.SetPixel(newUVs[i][0], newUVs[i][1]);
            }

        }

        void RotateUVOrderCounterClockwise() {

            var uvVectors = new List<int[]>();

            foreach (var uv in uvs) {
                uvVectors.Add(TextureCoordinate.GetVector(uv));
            }

            var newUVs = new List<int[]>();

            if (uvs.Count == 4) {

                newUVs.Add(uvVectors[3]);
                newUVs.Add(uvVectors[0]);
                newUVs.Add(uvVectors[1]);
                newUVs.Add(uvVectors[2]);

            }
            else {

                newUVs.Add(uvVectors[2]);
                newUVs.Add(uvVectors[0]);
                newUVs.Add(uvVectors[1]);

            }

            foreach (var i in Enumerable.Range(0, uvs.Count)) {
                uvs[i] = TextureCoordinate.SetPixel(newUVs[i][0], newUVs[i][1]);
            }

        }

        void RotateUVOrderClockwise() {

            var uvVectors = new List<int[]>();

            foreach (var uv in uvs) {
                uvVectors.Add(TextureCoordinate.GetVector(uv));
            }

            var newUVs = new List<int[]>();

            if (uvs.Count == 4) {

                newUVs.Add(uvVectors[1]);
                newUVs.Add(uvVectors[2]);
                newUVs.Add(uvVectors[3]);
                newUVs.Add(uvVectors[0]);

            }
            else {

                newUVs.Add(uvVectors[1]);
                newUVs.Add(uvVectors[2]);
                newUVs.Add(uvVectors[0]);

            }

            foreach (var i in Enumerable.Range(0, uvs.Count)) {
                uvs[i] = TextureCoordinate.SetPixel(newUVs[i][0], newUVs[i][1]);
            }

        }

        // - Shaders -

        // Well here's some confusing stuff...
        // Vertices, UVs and shaders all have their own order. To avoid redoing a lot of work,
        // I'm using the UV array order on shaders. Which means there's some funky indexing.
        // Unless it's dynamic monochrome which has the same order as UVs

        // - Mono -
        void FlipDynamicMonoShaderOrderVertically() {

            var monoShader = (DynamicMonoChromeShader)shaders;

            var newValues = new List<int>();

            if (monoShader.isQuad) {

                newValues.Add(monoShader.values[3]);
                newValues.Add(monoShader.values[2]);
                newValues.Add(monoShader.values[1]);
                newValues.Add(monoShader.values[0]);

            }
            else {

                newValues.Add(monoShader.values[2]);
                newValues.Add(monoShader.values[1]);
                newValues.Add(monoShader.values[0]);
                // Even if the tile is a triangle it stores 4 vertex colors
                // Not adding the last one will cause crashes!
                newValues.Add(monoShader.values[3]);

            }

            monoShader.values = newValues.ToArray();
            monoShader.Apply();

        }

        void FlipDynamicMonoShaderOrderHorizontally() {

            var monoShader = (DynamicMonoChromeShader)shaders;

            var newValues = new List<int>();

            if (monoShader.isQuad) {

                newValues.Add(monoShader.values[1]);
                newValues.Add(monoShader.values[0]);
                newValues.Add(monoShader.values[3]);
                newValues.Add(monoShader.values[2]);

            }
            else {

                newValues.Add(monoShader.values[1]);
                newValues.Add(monoShader.values[0]);
                newValues.Add(monoShader.values[2]);
                // Even if the tile is a triangle it stores 4 vertex colors
                // Not adding the last one will cause crashes!
                newValues.Add(monoShader.values[3]);

            }

            monoShader.values = newValues.ToArray();
            monoShader.Apply();

        }

        void RotateMonoShaderOrderCounterClockwise() {

            var monoShader = (DynamicMonoChromeShader)shaders;

            var newValues = new List<int>();

            if (monoShader.isQuad) {

                newValues.Add(monoShader.values[3]);
                newValues.Add(monoShader.values[0]);
                newValues.Add(monoShader.values[1]);
                newValues.Add(monoShader.values[2]);

            }
            else {

                newValues.Add(monoShader.values[2]);
                newValues.Add(monoShader.values[0]);
                newValues.Add(monoShader.values[1]);
                // Even if the tile is a triangle it stores 4 vertex colors
                // Not adding the last one will cause crashes!
                newValues.Add(monoShader.values[3]);

            }

            monoShader.values = newValues.ToArray();
            monoShader.Apply();

        }

        void RotateMonoShaderOrderClockwise() {

            var monoShader = (DynamicMonoChromeShader)shaders;

            var newValues = new List<int>();

            if (monoShader.isQuad) {

                newValues.Add(monoShader.values[1]);
                newValues.Add(monoShader.values[2]);
                newValues.Add(monoShader.values[3]);
                newValues.Add(monoShader.values[0]);

            }
            else {

                newValues.Add(monoShader.values[1]);
                newValues.Add(monoShader.values[2]);
                newValues.Add(monoShader.values[0]);
                // Even if the tile is a triangle it stores 4 vertex colors
                // Not adding the last one will cause crashes!
                newValues.Add(monoShader.values[3]);

            }

            monoShader.values = newValues.ToArray();
            monoShader.Apply();

        }

        // - Color -
        void FlipColorShaderOrderVertically() {

            var colorShader = (ColorShader)shaders;

            var newValues = new List<XRGB555>(colorShader.values);

            if (colorShader.isQuad) {

                newValues[ColorShader.uvOrderedQuadIndexes[0]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[3]];
                newValues[ColorShader.uvOrderedQuadIndexes[1]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[2]];
                newValues[ColorShader.uvOrderedQuadIndexes[2]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[1]];
                newValues[ColorShader.uvOrderedQuadIndexes[3]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[0]];

            }
            else {

                newValues[ColorShader.uvOrderedTriIndexes[0]] = colorShader.values[ColorShader.uvOrderedTriIndexes[2]];
                newValues[ColorShader.uvOrderedTriIndexes[1]] = colorShader.values[ColorShader.uvOrderedTriIndexes[1]];
                newValues[ColorShader.uvOrderedTriIndexes[2]] = colorShader.values[ColorShader.uvOrderedTriIndexes[0]];

            }

            colorShader.values = newValues.ToArray();
            colorShader.Apply();

        }

        void FlipColorShaderOrderHorizontally() {

            var colorShader = (ColorShader)shaders;

            var newValues = new List<XRGB555>(colorShader.values);

            if (colorShader.isQuad) {

                newValues[ColorShader.uvOrderedQuadIndexes[0]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[1]];
                newValues[ColorShader.uvOrderedQuadIndexes[1]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[0]];
                newValues[ColorShader.uvOrderedQuadIndexes[2]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[3]];
                newValues[ColorShader.uvOrderedQuadIndexes[3]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[2]];

            }
            else {

                newValues[ColorShader.uvOrderedTriIndexes[0]] = colorShader.values[ColorShader.uvOrderedTriIndexes[1]];
                newValues[ColorShader.uvOrderedTriIndexes[1]] = colorShader.values[ColorShader.uvOrderedTriIndexes[0]];
                newValues[ColorShader.uvOrderedTriIndexes[2]] = colorShader.values[ColorShader.uvOrderedTriIndexes[2]];

            }

            colorShader.values = newValues.ToArray();
            colorShader.Apply();

        }

        void RotateColorShaderOrderCounterClockwise() {

            var colorShader = (ColorShader)shaders;

            var newValues = new List<XRGB555>(colorShader.values);

            if (colorShader.isQuad) {

                newValues[ColorShader.uvOrderedQuadIndexes[0]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[3]];
                newValues[ColorShader.uvOrderedQuadIndexes[1]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[0]];
                newValues[ColorShader.uvOrderedQuadIndexes[2]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[1]];
                newValues[ColorShader.uvOrderedQuadIndexes[3]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[2]];

            }
            else {

                newValues[ColorShader.uvOrderedTriIndexes[0]] = colorShader.values[ColorShader.uvOrderedTriIndexes[2]];
                newValues[ColorShader.uvOrderedTriIndexes[1]] = colorShader.values[ColorShader.uvOrderedTriIndexes[0]];
                newValues[ColorShader.uvOrderedTriIndexes[2]] = colorShader.values[ColorShader.uvOrderedTriIndexes[1]];

            }

            colorShader.values = newValues.ToArray();
            colorShader.Apply();

        }

        void RotateColorShaderOrderClockwise() {

            var colorShader = (ColorShader)shaders;

            var newValues = new List<XRGB555>(colorShader.values);

            if (colorShader.isQuad) {

                newValues[ColorShader.uvOrderedQuadIndexes[0]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[1]];
                newValues[ColorShader.uvOrderedQuadIndexes[1]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[2]];
                newValues[ColorShader.uvOrderedQuadIndexes[2]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[3]];
                newValues[ColorShader.uvOrderedQuadIndexes[3]] = colorShader.values[ColorShader.uvOrderedQuadIndexes[0]];

            }
            else {

                newValues[ColorShader.uvOrderedTriIndexes[0]] = colorShader.values[ColorShader.uvOrderedTriIndexes[1]];
                newValues[ColorShader.uvOrderedTriIndexes[1]] = colorShader.values[ColorShader.uvOrderedTriIndexes[2]];
                newValues[ColorShader.uvOrderedTriIndexes[2]] = colorShader.values[ColorShader.uvOrderedTriIndexes[0]];

            }

            colorShader.values = newValues.ToArray();
            colorShader.Apply();

        }


        #endregion

    }

    public interface TileShaders {

        public float[][] colors { get; set; }

        public bool isQuad { get; set; }

        public VertexColorType type { get; set; }

        public void Apply();

        public List<byte> Compile();

        public TileShaders VerifyCorrectShader();

        public TileShaders Clone(bool isQuad);

        public bool Compare(TileShaders shaders);

    }

    public class MonoChromeShader : TileShaders {
        public float[][] colors { get; set; }

        public bool isQuad { get; set; }

        public VertexColorType type { get; set; }

        public byte value;

        public const float white = 116f;

        public MonoChromeShader(byte lightingInfo, bool isQuad) {
            this.value = lightingInfo;
            this.isQuad = isQuad;
            type = VertexColorType.MonoChrome;
            Apply();

        }

        public MonoChromeShader(bool isQuad) {

            this.value = (byte)white;
            this.isQuad = isQuad;
            type = VertexColorType.MonoChrome;
            Apply();

        }

        public MonoChromeShader() {
            type = VertexColorType.MonoChrome;
        }

        public void Apply() {

            var color = value / white;

            var fColors = new float[] { color, color, color };

            if (isQuad) {

                colors = new float[][] {
                    fColors, fColors, fColors, fColors
                };

            } else {

                colors = new float[][] {
                    fColors, fColors, fColors
                };

            }

        }

        public List<byte> Compile() {
            return new List<byte> { value };
        }

        public TileShaders VerifyCorrectShader() {
            return null;
        }

        public TileShaders Clone(bool isQuad) {

            return new MonoChromeShader(value, isQuad);

        }

        public bool Compare(TileShaders shaders) {
            
            if (shaders is MonoChromeShader monoShader) {

                return this.value == monoShader.value;

            }
            else {
                return false;
            }

        }

    }

    // Note that regardless of shape, this will always store 4 vertex colors
    // These values are ordered the same as UVs
    public class DynamicMonoChromeShader : TileShaders {

        // Ordered by tile vertex positions (Top-Left, Top-Right, Bottom-Left, Bottom-Right)
        public static readonly int[] vertexOrderedQuadIndexes = new int[] { 0, 1, 3, 2 };
        public static readonly int[] vertexOrderedTriIndexes = new int[] { 0, 2, 1 };

        public float[][] colors { get; set; }
        public bool isQuad { get; set; }

        public VertexColorType type { get; set; }

        public int[] values;

        public const float white = 29f;

        public DynamicMonoChromeShader(List<byte> data, bool isQuad) {

            this.isQuad = isQuad;
            type = VertexColorType.DynamicMonoChrome;
            var bitField = new BitArray(data.ToArray());

            values = new int[] {

                Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 18, 24)),
                Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 12, 18)),
                Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 6, 12)),
                Utils.BitsToInt(Utils.CopyBitsOfRange(bitField, 0, 6))

            };


            Apply();

        }

        public DynamicMonoChromeShader(List<int> values, bool isQuad) {

            this.isQuad = isQuad;
            this.values = values.ToArray();
            type = VertexColorType.DynamicMonoChrome;

            Apply();

        }

        public DynamicMonoChromeShader(bool isQuad) {

            this.isQuad = isQuad;
            type = VertexColorType.DynamicMonoChrome;

            values = new int[] {
                (int)white,
                (int)white,
                (int)white,
                (int)white
            };

            Apply();

        }

        public DynamicMonoChromeShader(TileShaders previousShader) {

            if (previousShader.type == VertexColorType.MonoChrome) {

                var monoChrome = (MonoChromeShader)previousShader;

                var whitePercentage = monoChrome.value / MonoChromeShader.white;

                var valueConversion = (int)MathF.Round(white * whitePercentage);

                values = new int[] {
                    valueConversion,
                    valueConversion,
                    valueConversion,
                    valueConversion
                };

            }
            else {

                values = new int[] {
                    (int)white,
                    (int)white,
                    (int)white,
                    (int)white
                };

            }

            this.isQuad = previousShader.isQuad;
            type = VertexColorType.DynamicMonoChrome;

            Apply();

        }

        public DynamicMonoChromeShader() {
            type = VertexColorType.DynamicMonoChrome;
            values = new int[0];
        }

        public void Apply() {

            var total = new List<float[]>();

            if (isQuad) {

                total.Add(new float[] { values[0] / white, values[0] / white, values[0] / white, values[0] / white });
                total.Add(new float[] { values[1] / white, values[1] / white, values[1] / white, values[1] / white });
                total.Add(new float[] { values[3] / white, values[3] / white, values[3] / white, values[3] / white });
                total.Add(new float[] { values[2] / white, values[2] / white, values[2] / white, values[2] / white });


            }
            else {

                total.Add(new float[] { values[0] / white, values[0] / white, values[0] / white, values[0] / white });
                total.Add(new float[] { values[2] / white, values[2] / white, values[2] / white, values[2] / white });
                total.Add(new float[] { values[1] / white, values[1] / white, values[1] / white, values[1] / white });

            }

            colors = total.ToArray();

        }

        public List<byte> Compile() {

            var bitfield = new BitField(24, new() {
                        new BitNumber(6, values[3]),
                        new BitNumber(6, values[2]),
                        new BitNumber(6, values[1]),
                        new BitNumber(6, values[0])
                    });

            return Utils.BitArrayToByteArray(bitfield.Compile()).ToList();

        }

        public TileShaders VerifyCorrectShader() {

            if (!isQuad) {
                values[3] = 0;
            }

            var first = values[0];

            if (values.All(value => value == first)) {

                var whitePercentage = first / white;

                var newShader = new MonoChromeShader((byte)MathF.Round(MonoChromeShader.white * whitePercentage), isQuad);

                return newShader;

            }
            else {

                return null;

            }

        }

        public TileShaders Clone(bool isQuad) {

            return new DynamicMonoChromeShader(this.Compile(), isQuad);

        }

        public bool Compare(TileShaders shaders) {

            if (shaders is DynamicMonoChromeShader monoShader) {

                return this.values.SequenceEqual(monoShader.values);

            }
            else {
                return false;
            }

        }

    }

    // This doesn't apply to color shader, triangles will store 3
    public class ColorShader : TileShaders {

        // Ordered by tile vertex positions (Top-Left, Top-Right, Bottom-Left, Bottom-Right)
        public static readonly int[] vertexOrderedQuadIndexes = new int[] { 3, 1, 2, 0 };
        public static readonly int[] vertexOrderedTriIndexes = new int[] { 2, 0, 1 };

        // Ordered by tile uv positions (Top-Left, Top-Right, Bottom-Right, Bottom-Left)
        public static readonly int[] uvOrderedQuadIndexes = new int[] { 3, 1, 0, 2 };
        public static readonly int[] uvOrderedTriIndexes = new int[] { 2, 1, 0 };

        public float[][] colors { get; set; }
        public bool isQuad { get; set; }

        public VertexColorType type { get; set; }

        public XRGB555[] values;

        public ColorShader(List<byte> data, FCopLevelSectionParser section, bool isQuad) {

            this.isQuad = isQuad;
            type = VertexColorType.Color;
            var colors = new List<XRGB555>();

            var i = 0;
            foreach (var colorIndex in data) {

                if (isQuad && i == 2) {
                    i++;
                    continue;
                }

                colors.Add(section.colors[colorIndex].Clone());

                i++;
            }

            values = colors.ToArray();

            Apply();

        }

        public ColorShader(XRGB555[] values, bool isQuad) {

            this.isQuad = isQuad;
            type = VertexColorType.Color;

            this.values = values;

            Apply();

        }

        public ColorShader(bool isQuad) {

            this.isQuad = isQuad;
            type = VertexColorType.Color;

            values = new XRGB555[] { 
                new XRGB555(false, 31, 31, 31), 
                new XRGB555(false, 31, 31, 31), 
                new XRGB555(false, 31, 31, 31), 
                new XRGB555(false, 31, 31, 31) 
            };

            Apply();

        }

        public ColorShader(TileShaders previousShader) {

            if (previousShader.type == VertexColorType.MonoChrome) {

                var monoChrome = (MonoChromeShader)previousShader;

                var whitePercentage = monoChrome.value / MonoChromeShader.white;

                var valueConversion = (int)MathF.Round(XRGB555.maxChannelValue * whitePercentage);

                if (valueConversion > XRGB555.maxChannelValue) {
                    valueConversion = (int)XRGB555.maxChannelValue;
                }

                values = new XRGB555[] {
                    new XRGB555(false, valueConversion, valueConversion, valueConversion),
                    new XRGB555(false, valueConversion, valueConversion, valueConversion),
                    new XRGB555(false, valueConversion, valueConversion, valueConversion),
                };

                if (previousShader.isQuad) {
                    values = values.Append(new XRGB555(false, valueConversion, valueConversion, valueConversion)).ToArray();
                }

            }
            else if (previousShader.type == VertexColorType.DynamicMonoChrome) {

                // Filler Data
                var valuesList = new List<XRGB555>() {
                    new XRGB555(false, 31, 31, 31),
                    new XRGB555(false, 31, 31, 31),
                    new XRGB555(false, 31, 31, 31),
                    new XRGB555(false, 31, 31, 31)
                };

                var dyanmicMonoChrome = (DynamicMonoChromeShader)previousShader;

                var quadMonoPosToColor = new int[] { 3, 1, 0, 2 };
                var triangleMonoPosToColor = new int[] { 2, 1, 0 };

                var i = 0;
                foreach (var value in dyanmicMonoChrome.values) {

                    var whitePercentage = value / DynamicMonoChromeShader.white;

                    var valueConversion = (int)MathF.Round(XRGB555.maxChannelValue * whitePercentage);

                    if (valueConversion > XRGB555.maxChannelValue) {
                        valueConversion = (int)XRGB555.maxChannelValue;
                    }

                    if (previousShader.isQuad) {
                        valuesList[quadMonoPosToColor[i]] = new XRGB555(false, valueConversion, valueConversion, valueConversion);
                    } else {

                        if (i < 3) {

                            valuesList[triangleMonoPosToColor[i]] = new XRGB555(false, valueConversion, valueConversion, valueConversion);

                        }

                    }

                    i++;

                }

                if (!previousShader.isQuad) {
                    valuesList.RemoveAt(valuesList.Count - 1);
                }

                values = valuesList.ToArray();

            }
            else {

                if (previousShader.isQuad) {

                    values = new XRGB555[] {
                        new XRGB555(false, 31, 31, 31),
                        new XRGB555(false, 31, 31, 31),
                        new XRGB555(false, 31, 31, 31),
                        new XRGB555(false, 31, 31, 31)
                    };

                }
                else {

                    values = new XRGB555[] {
                        new XRGB555(false, 31, 31, 31),
                        new XRGB555(false, 31, 31, 31),
                        new XRGB555(false, 31, 31, 31),
                    };

                }

            }

            this.isQuad = previousShader.isQuad;
            type = VertexColorType.Color;

            Apply();

        }

        public ColorShader() {

            type = VertexColorType.Color;
            values = new XRGB555[0];

        }

        public void Apply() {

            var total = new List<float[]>();

            if (isQuad) {

                total.Add(values[3].ToColors());
                total.Add(values[1].ToColors());
                total.Add(values[2].ToColors());
                total.Add(values[0].ToColors());


            } else {

                total.Add(values[2].ToColors());
                total.Add(values[0].ToColors());
                total.Add(values[1].ToColors());

            }

            colors = total.ToArray();

        }

        public List<byte> Compile() {
            return new();
        }

        public List<byte> ColorCompile(Dictionary<ushort, (int, XRGB555)> existingColors) {
            var corner1 = (byte)existingColors[values[0].ToUShort()].Item1;
            var corner2 = (byte)existingColors[values[1].ToUShort()].Item1;
            var corner3 = (byte)existingColors[values[2].ToUShort()].Item1;

            if (isQuad) {
                var corner4 = (byte)existingColors[values[3].ToUShort()].Item1;

                return new() { corner1, corner2, 0, corner3, corner4 };

            } else {
                return new() { corner1, corner2, corner3 };
            }


        }

        public TileShaders VerifyCorrectShader() {

            if ((isQuad && values.Length != 4) || (!isQuad && values.Length != 3)) {

                if (!isQuad && values.Length != 3) {

                    values = values.Take(values.Length - 1).ToArray();

                }
                else if (isQuad && values.Length != 4) {

                    values = values.Append(new XRGB555(false, 31, 31, 31)).ToArray();

                }
                else {
                    
                    throw new Exception("Incorrect shader format");

                }

            }

            var first = values[0];

            var sameColor = values.All(color => first.ToUShort() == color.ToUShort());

            if (sameColor) {

                var firstChannel = first.r;

                var isGrey = first.g == firstChannel && first.b == firstChannel;

                if (isGrey) {

                    var whitePercentage = first.r / XRGB555.maxChannelValue;

                    return new MonoChromeShader((byte)MathF.Round(MonoChromeShader.white * whitePercentage), isQuad);

                }
                else {

                    return null;

                }


            }
            else {

                var monoValues = new List<int>();
                foreach (var color in values) {

                    var firstChannel = color.r;

                    var isGrey = color.g == firstChannel && color.b == firstChannel;

                    if (!isGrey) {
                        return null;
                    }

                    var whitePercentage = color.r / XRGB555.maxChannelValue;
                    monoValues.Add((int)MathF.Round(DynamicMonoChromeShader.white * whitePercentage));

                }

                var newShader = new DynamicMonoChromeShader();
                newShader.isQuad = isQuad;

                // Remember that regardless of shape, monochrome will always store 4 vertex colors
                if (!isQuad) {
                    monoValues.Add(0);
                }

                var quadMonoPosToColor = new int[] { 3, 1, 0, 2 };
                var triangleMonoPosToColor = new int[] { 2, 1, 0, 3 };

                var orderedMonoValues = new List<int>();

                foreach (var i in Enumerable.Range(0, monoValues.Count)) {

                    if (isQuad) {
                        //orderedMonoValues[quadMonoPosToColor[i]] = monoValues[i];
                        orderedMonoValues.Add(monoValues[quadMonoPosToColor[i]]);
                    }
                    else {
                        //orderedMonoValues[triangleMonoPosToColor[i]] = monoValues[i];

                        orderedMonoValues.Add(monoValues[triangleMonoPosToColor[i]]);
                    }

                }

                newShader.values = orderedMonoValues.ToArray();
                newShader.Apply();

                return newShader;

            }

        }

        public TileShaders Clone(bool isQuad) {

            var colors = new List<XRGB555>();

            foreach (var color in values) {
                colors.Add(color.Clone());
            }

            if (!isQuad && colors.Count == 4) {
                colors.RemoveAt(colors.Count - 1);
            }
            else if (isQuad && colors.Count != 4) {
                colors.Add(new XRGB555(false, 31, 31, 31));
            }

            return new ColorShader(colors.ToArray(), isQuad);

        }

        public bool Compare(TileShaders shaders) {

            if (shaders is ColorShader colorShader) {

                var i = 0;
                foreach (var col in values) {

                    if (!col.Compile().SequenceEqual(colorShader.values[i].Compile())) {
                        return false;
                    }

                    i++;
                }

                return true;

            }
            else {
                return false;
            }

        }

    }

    // Same with this
    public class AnimatedShader : TileShaders {

        public float[][] colors { get; set; }
        public bool isQuad { get; set; }

        public VertexColorType type { get; set; }

        public AnimatedShader(bool isQuad) {
            this.isQuad = isQuad;
            type = VertexColorType.ColorAnimated;
            Apply();
        }

        public AnimatedShader() {
            type = VertexColorType.ColorAnimated;
        }

        public void Apply() {

            var dummyColors = new float[] { 1f, 1f, 1f };

            if (isQuad) {

                colors = new float[][] {
                    dummyColors, dummyColors, dummyColors, dummyColors
                };

            }
            else {

                colors = new float[][] {
                    dummyColors, dummyColors, dummyColors
                };

            }

        }

        public List<byte> Compile() {
            return new List<byte> { 0 };
        }

        public TileShaders VerifyCorrectShader() {
            return null;
        }

        public TileShaders Clone(bool isQuad) {
            return new AnimatedShader(isQuad);
        }

        public bool Compare(TileShaders shaders) {

            return this.isQuad && shaders.isQuad;

        }

    }

    public class AnimationVector {

        public const int maxDistance = 27;
        public const float frameTime = 5.95f / 27f;

        public int x;
        public int y;

        public AnimationVector(List<byte> bytes) {
            x = (sbyte)bytes[0];
            y = (sbyte)bytes[1];
        }

        public AnimationVector(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public List<byte> Compile() {
            return new List<byte>() { (byte)x, (byte)y };
        }
    }

    public struct TileVertex {

        public int heightChannel;

        public VertexPosition vertexPosition;

        public TileVertex(int heightChannel, VertexPosition vertexPosition) {
            this.heightChannel = heightChannel;
            this.vertexPosition = vertexPosition;
        }

    }

    public enum VertexColorType {
        MonoChrome = 0,
        DynamicMonoChrome = 1,
        Color = 2,
        ColorAnimated = 3
    }

    public enum VertexPosition {
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 3,
        BottomRight = 4
    }

    public enum TileEffectType {

        Normal = 0,
        Liquid = 1,
        InstantKill = 2,
        Slipper0 = 3,
        Slipper1 = 4,
        Slipper2 = 5,
        Damage_Both_Medium = 6,
        Damage_Both_High = 7,
        Damage_Walker_Medium_Hover_Low = 8,
        Damage_Walker_High_Hover_Medium  = 9,
        Damage_Walker_Instant_Hover_Low = 10,
        Damage_Walker_Instant_Hover_Medium = 11,
        Damage_Walker_Low_Hover_None = 12,
        Damage_Walker_Medium_Hover_None = 13,
        Damage_Red = 14,
        Damage_Blue = 15,
        Move_PosX_Medium = 32,
        Move_PosX_High = 33,
        Move_NegX_Medium = 34,
        Move_NegX_High = 35,
        Move_PosY_Medium = 36,
        Move_PosY_High = 37,
        Move_NegY_Medium = 38,
        Move_NegY_High = 39,
        Move_PosX_Low = 40,
        Move_NegX_Low = 41,
        Move_PosY_Low = 42,
        Move_NegY_Low = 43,
        Dupe_Move_PosX_Low = 44,
        Dupe_Move_NegX_Low = 45,
        Dupe_Move_PosY_Low = 46,
        Dupe_Move_NegY_Low = 47,
        No_Collision = 64,
        Other = 255

    }

}