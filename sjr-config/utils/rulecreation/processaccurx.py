import csv
import os
from collections import defaultdict


ACCURX_CSV_FILENAME = 'accurx.csv'
ACCURX_PATH = '../../configurations/Journeys/accurx/'
ACCURX_YAML_FILENAME = 'accurx.yaml'
YAML_SUFFIX = '.yaml'
    
def removeExistingFile():
    existingFilesPath = os.path.join(os.path.dirname(__file__), ACCURX_PATH)
    existingFiles = os.listdir(existingFilesPath)

    for file in existingFiles:
        if file.endswith(YAML_SUFFIX):
            os.remove(existingFilesPath + file )
            print(file + " deleted")

inputFile = csv.DictReader(open(os.path.join(os.path.dirname(__file__), ACCURX_CSV_FILENAME)))

fileTemplate = '''$schema: "Schemas/Journeys/configuration_schema.json"
target:
  odsCodes:
    - {odsList}
journeys:
  silverIntegrations:
    consultations:
      - accurx
    messages:
      - accurx
'''

removeExistingFile()

odsCodes = []
for row in inputFile:
    odsCodes.append('"' + row['odsCode'].upper() + '"')
    print(row['odsCode'] + " added")

with open(os.path.join(os.path.dirname(__file__), ACCURX_PATH + ACCURX_YAML_FILENAME), "wb") as ymlFile:
    ymlFile.write(fileTemplate.format(odsList='\n    - '.join(odsCodes)))
