import csv
import os
from collections import defaultdict


WAYFINDER_CSV_FILENAME = 'wayfinder.csv'
WAYFINDER_PATH = '../../configurations/Journeys/wayfinder/'
WAYFINDER_YAML_FILENAME = 'wayfinder.yaml'
YAML_SUFFIX = '.yaml'
    
def removeExistingFile():
    existingFilesPath = os.path.join(os.path.dirname(__file__), WAYFINDER_PATH)
    existingFiles = os.listdir(existingFilesPath)

    for file in existingFiles:
        if file.endswith(YAML_SUFFIX):
            os.remove(existingFilesPath + file )
            print(file + " deleted")

inputFile = csv.DictReader(open(os.path.join(os.path.dirname(__file__), WAYFINDER_CSV_FILENAME)))

fileTemplate = '''$schema: "Schemas/Journeys/configuration_schema.json"
target:
  odsCodes:
    - {odsList}
journeys:
  wayfinder:
    isEnabled: true
'''

removeExistingFile()

odsCodes = []
for row in inputFile:
    odsCodes.append('"' + row['odsCode'].upper() + '"')
    print(row['odsCode'] + " added")

with open(os.path.join(os.path.dirname(__file__), WAYFINDER_PATH + WAYFINDER_YAML_FILENAME), "wb") as ymlFile:
    ymlFile.write(fileTemplate.format(odsList='\n    - '.join(odsCodes)))
