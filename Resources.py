from io import BufferedWriter
from typing import Dict
import json
from xml.etree import ElementTree

quantityValues = [ "zero", "one", "two", "few", "many", "other" ]

class ResourceCollection:
    def __init__(self) -> None:
        self.strings = {}
        self.pluralStrings = {}

class PluralString:
    def __init__(self) -> None:
        self.strings = [ None, None, None, None, None, None]

class ResourceCompiler:
    def __init__(self) -> None:
        self.resourceCollections = {}
    
    def readJson(self, languageCode: str, regionCode: str, jsonObject) -> None:
        localeCode = "default"
        if(languageCode != None):
            localeCode = languageCode
            if regionCode != None:
                localeCode += regionCode
        
        if localeCode in self.resourceCollections:
            resourceCollection = self.resourceCollections[localeCode]
        else:
            resourceCollection = ResourceCollection()
            self.resourceCollections[localeCode] = resourceCollection

        for resourceId, content in jsonObject.items():
            if isinstance(content, str):
                resourceCollection.strings[resourceId] = content
            else:
                pluralString = PluralString()

                for quantity, value in content.items():
                    try:
                        quantityIndex = quantityValues.index(quantity)
                    except ValueError:
                        raise Exception("Unknown quantity: " + quantity)
                    
                    pluralString.strings[quantityIndex] = value
                
                resourceCollection.pluralStrings[resourceId] = pluralString

    
    def readJsonFile(self, languageCode: str, regionCode: str, path: str) -> None:
        with open(path, encoding="utf-8") as jsonFile:
            self.readJson(languageCode, regionCode, json.load(jsonFile))

    def readXml(self, elementTree: ElementTree.ElementTree) -> None:
        resourceFileElement = elementTree.getroot()
        if resourceFileElement.tag != "ResourceFile":
            raise Exception("Root tag is not ResourceFile.")

        for resourcesElement in resourceFileElement:
            if resourcesElement.tag != "Resources":
                raise Exception("Unexpected tag: " + resourcesElement.tag)
            
            if "Language" not in resourcesElement.attrib:
                localeCode = "default"
            else:
                localeCode = resourcesElement.attrib["Language"]
                if "Region" in resourcesElement.attrib:
                    localeCode += resourcesElement.attrib["Region"]
            
            if localeCode in self.resourceCollections:
                resourceCollection = self.resourceCollections[localeCode]
            else:
                resourceCollection = ResourceCollection()
                self.resourceCollections[localeCode] = resourceCollection

            for resourceElement in resourcesElement:
                if "Id" not in resourceElement.attrib:
                    raise Exception("Resource must have an id.")
                resourceId = resourceElement.attrib["Id"]

                if resourceElement.tag == "String":
                    resourceCollection.strings[resourceId] = resourceElement.text
                elif resourceElement.tag == "PluralString":
                    pluralString = PluralString()

                    for valueElement in resourceElement:
                        if valueElement.tag != "Value":
                            raise Exception("Unexpected tag: " + resourceElement.tag)
                        
                        if "Quantity" not in valueElement.attrib:
                            raise Exception("PluralString Value must have a quantity.")
                        
                        try:
                            quantityIndex = quantityValues.index(valueElement.attrib["Quantity"].lower())
                        except ValueError:
                            raise Exception("Unknown quantity: " + valueElement.attrib["Quantity"])
                        
                        pluralString.strings[quantityIndex] = valueElement.text
                    
                    resourceCollection.pluralStrings[resourceId] = pluralString
                else:
                    raise Exception("Unknown resource tag: " + resourceElement.tag)

    def readXmlFile(self, filePath: str) -> None:
        self.readXml(ElementTree.parse(filePath))
    
    def writeResourceCollectionBytes(self, writer: BufferedWriter, resourceCollection: ResourceCollection):
        resourceCount = len(resourceCollection.strings) + len(resourceCollection.pluralStrings)
        writer.write(resourceCount.to_bytes(4, "little", signed=True))

        for id, string in resourceCollection.strings.items():
            writer.write((0).to_bytes(1, "little", signed=True))

            stringBytes = id.encode("utf-8")
            writer.write(len(stringBytes).to_bytes(4, "little", signed=True))
            writer.write(stringBytes)

            stringBytes = string.encode("utf-8")
            writer.write(len(stringBytes).to_bytes(4, "little", signed=True))
            writer.write(stringBytes)
        
        for id, pluralString in resourceCollection.pluralStrings.items():
            writer.write((1).to_bytes(1, "little", signed=True))

            stringBytes = id.encode("utf-8")
            writer.write(len(stringBytes).to_bytes(4, "little", signed=True))
            writer.write(stringBytes)

            for value in pluralString.strings:
                if value == None:
                    writer.write((0).to_bytes(4, "little", signed=True))
                else:
                    stringBytes = value.encode("utf-8")
                    writer.write(len(stringBytes).to_bytes(4, "little", signed=True))
                    writer.write(stringBytes)
    
    def writeCompiledBytes(self, writer: BufferedWriter) -> None:
        headerSize = 8 + 8 * (len(self.resourceCollections) - 1)

        writer.write(headerSize.to_bytes(4, "little", signed=True))
        writer.write((len(self.resourceCollections) - 1).to_bytes(4, "little", signed=True))
        for i in range(len(self.resourceCollections) - 1):
            writer.write(b"\x00\x00\x00\x00\x00\x00\x00\x00")

        self.writeResourceCollectionBytes(writer, self.resourceCollections["default"])

        collectionAdresses = {}
        for locale, collection in self.resourceCollections.items():
            if locale == "default":
                continue

            collectionAdresses[locale] = writer.tell()

            self.writeResourceCollectionBytes(writer, collection)
            
        writer.seek(8)
        for locale, adress in collectionAdresses.items():
            writer.write(locale.encode("ascii"))
            if len(locale) == 2:
                writer.write(b"\x00\x00")
            
            writer.write(adress.to_bytes(4, "little", signed=True))