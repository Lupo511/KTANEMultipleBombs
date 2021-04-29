#Build script
#Requires buildconfig.json in the same directory
#Sample config:
#{
#    "msbuildPath": "C:/Program Files (x86)/Microsoft Visual Studio/2019/Community/MSBuild/Current/Bin/MSBuild.exe",
#    "unityPath": "C:/Program Files/Unity_KTaNE/Editor/Unity.exe",
#    "debugModsDirectory": "C:/Program Files (x86)/Steam/steamapps/common/Keep Talking and Nobody Explodes/mods"
#}
#"debugModsDirectory" is optional

import sys
import json
import os
import shutil
import subprocess

def copydir(dir, dest):
    os.makedirs(dest, exist_ok=True)
    with os.scandir(dir) as entries:
        for entry in entries:
            fullpath = os.path.join(dir, entry.name)
            if entry.is_file():
                shutil.copy(fullpath, dest)
            else:
                copydir(fullpath, os.path.join(dest, entry.name))

def build(config):
    print("Building mod...")

    print("Building assembly...")
    result = subprocess.run([config["msbuildPath"], "./MultipleBombsAssembly/MultipleBombsAssembly.sln", "/p:Configuration=Release"])
    print("Assembly build completed")

    print("Building mod bundle...")
    #Apparently projectPath value has to be passed in the same argument while executeMethod value has to be passed in the next argument
    result = subprocess.run([config["unityPath"], "-batchmode", "-quit", "-projectPath ./MultipleBombs/", "-executeMethod", "AssetBundler.BuildAllAssetBundles_WithEditorUtility", "-logFile", "-"])
    print("Mod bundle build completed")

    print("Copying to build directory...")
    copydir("./MultipleBombs/build/MultipleBombs", "./Build/MultipleBombs")
    print("Build completed")

    if("debugModsDirectory" in config):
        print("Copying build to debug directory...")
        copydir("./Build/MultipleBombs", config["debugModsDirectory"] + "/MultipleBombs")
        print("Build copied to debug directory")

def clean():
    shutil.rmtree("./Build/")

config = None
with open("./buildconfig.json", "rb") as configFile:
    config = json.load(configFile)

if("clean" in sys.argv):
    clean()
else:
    build(config)