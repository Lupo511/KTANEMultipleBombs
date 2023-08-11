#Build script
#Requires buildconfig.json in the same directory
#Sample config:
#{
#    "msbuildPath": "C:/Program Files (x86)/Microsoft Visual Studio/2019/Community/MSBuild/Current/Bin/MSBuild.exe",
#    "unityPath": "C:/Program Files/Unity/Editor/Unity.exe",
#    "ktanePath": "C:/Program Files (x86)/Steam/steamapps/common/Keep Talking and Nobody Explodes/",
#    "copyToModsFolder": true
#}
#"debugModsDirectory" is optional

import sys
import json
import os
import shutil
import subprocess
import Resources

def copydir(dir, dest):
    os.makedirs(dest, exist_ok=True)
    with os.scandir(dir) as entries:
        for entry in entries:
            fullpath = os.path.join(dir, entry.name)
            if entry.is_file():
                shutil.copy(fullpath, dest)
            else:
                copydir(fullpath, os.path.join(dest, entry.name))

def buildResources():
    print("Building resources...")

    resourceCompiler = Resources.ResourceCompiler()

    with os.scandir("./Resources") as resourceFiles:
        for resourceFile in resourceFiles:
            if resourceFile.is_file():
                if resourceFile.name.endswith(".json") and resourceFile.name.startswith("Resources."):
                    languageCode = None
                    regionCode = None
                    if len(resourceFile.name) != 14:
                        languageCode = resourceFile.name[10:12]
                        if len(resourceFile.name) == 20:
                            regionCode = resourceFile.name[13:15]
                    
                    resourceCompiler.readJsonFile(languageCode, regionCode, resourceFile.path)

    resourceCompiler.writeCompiledBytes(open("./Build/MultipleBombs/Resources.bin", "wb"))
    
    print("Resources build completed")

def buildAssembly(config):
    print("Building assembly...")
    
    result = subprocess.run([config["msbuildPath"], "./MultipleBombsAssembly/MultipleBombsAssembly.sln", "/p:Configuration=Release", "/p:KTANEPath=" + config["ktanePath"]])
    
    if result.returncode != 0:
        raise Exception("MSBuild returned with exit code " + str(result.returncode))
    
    shutil.copy("./MultipleBombsAssembly/MultipleBombsAssembly/bin/Release/MultipleBombsAssembly.dll", "./Build/MultipleBombs/MultipleBombsAssembly.dll")
    shutil.copy("./MultipleBombsAssembly/MultipleBombsAssembly/bin/Release/MultipleBombsAssembly.dll", "./MultipleBombs/Assets/Plugins/Managed/MultipleBombsAssembly.dll")
    
    print("Assembly build completed")
    return 0

def buildBundle(config):
    print("Building mod bundle...")
    
    result = subprocess.run([config["unityPath"], "-batchmode", "-quit", "-projectPath", os.getcwd() + "/MultipleBombs/", "-executeMethod", "AssetBundler.BuildAllAssetBundles_WithEditorUtility", "-logFile", "-"])
    
    if result.returncode != 0:
        raise Exception("Unity returned with exit code " + str(result.returncode))
    
    copydir("./MultipleBombs/build/MultipleBombs", "./Build/MultipleBombs")

    print("Mod bundle build completed")
    return 0

def buildInfo():
    print("Building info...")

    copydir("./InfoFiles", "./Build/MultipleBombs")

    print("Info build completed")

def copy(ktanePath):
    print("Copying build to mods directory...")
    copydir("./Build/MultipleBombs", ktanePath + "/mods/MultipleBombs")
    print("Build copied to mods directory")

def buildAll(config):

    buildResources()

    buildAssembly(config)

    buildBundle(config)

    buildInfo()

    if("copyToModsFolder" in config and config["copyToModsFolder"] == True):
        copy(config["ktanePath"])

def clean():
    print("Cleaning build...")
    shutil.rmtree("./Build/")
    print("Build cleaned")

config = None
with open("./buildconfig.json", "rb") as configFile:
    config = json.load(configFile)

print("Building mod...")
os.makedirs("./Build/MultipleBombs", exist_ok=True)
if len(sys.argv) == 1:
    buildAll(config)
else:
    for target in sys.argv[1:]:
        if target == "all":
            buildAll(config)
        elif target == "resources":
            buildResources()
        elif target == "assembly":
            buildAssembly(config)
        elif target == "bundle":
            buildBundle(config)
        elif target == "info":
            buildInfo()
        elif target == "copy":
            copy(config["ktanePath"])
        elif target == "clean":
            clean()
        else:
            print("Unknown target: " + target)