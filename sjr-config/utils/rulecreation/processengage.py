import csv
import os
from collections import defaultdict


ENGAGE_CSV_FILENAME = 'engage.csv'
ENGAGE_PATH = '../../configurations/Journeys/engage/'
ENGAGE_YAML_FILENAME = 'engage.yaml'
YAML_SUFFIX = '.yaml'
    
def removeExistingFile():
    existingFilesPath = os.path.join(os.path.dirname(__file__), ENGAGE_PATH)
    existingFiles = os.listdir(existingFilesPath)

    for file in existingFiles:
        if file.endswith(YAML_SUFFIX):
            os.remove(existingFilesPath + file )
            print(file + " deleted")

inputFile = csv.DictReader(open(os.path.join(os.path.dirname(__file__), ENGAGE_CSV_FILENAME)))

fileTemplate = '''$schema: "Schemas/Journeys/configuration_schema.json"
target:
  odsCodes:
    - {odsList}
journeys:
  silverIntegrations:
    consultations:
      - engage
    consultationsAdmin:
      - engage
    messages:
      - engage
'''

removeExistingFile()

odsCodes = []
for row in inputFile:
    odsCodes.append('"' + row['odsCode'].upper() + '"')
    print(row['odsCode'] + " added")

with open(os.path.join(os.path.dirname(__file__), ENGAGE_PATH + ENGAGE_YAML_FILENAME), "wb") as ymlFile:
    ymlFile.write(fileTemplate.format(odsList='\n    - '.join(odsCodes)))
