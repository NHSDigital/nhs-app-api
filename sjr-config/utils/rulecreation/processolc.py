import csv
import os
from collections import defaultdict

OLC_PATH = '../../configurations/Journeys/olc/'
CCG_FILE_PREFIX = 'ccg_'
YAML_SUFFIX = '.yaml'

def camelCaseCcgName(name):
    return ''.join(x for x in name.title() if not x.isspace()).replace('&', 'And')
    
def removeExistingFiles():
    existingFilesPath = os.path.join(os.path.dirname(__file__), OLC_PATH)
    existingFiles = os.listdir(existingFilesPath)

    for file in existingFiles:
        if file.startswith(CCG_FILE_PREFIX) and file.endswith(YAML_SUFFIX):
            os.remove(existingFilesPath + file )

inputFile = csv.DictReader(open(os.path.join(os.path.dirname(__file__), "econsult.csv")))

fileTemplate = '''$schema: "Schemas/Journeys/configuration_schema.json"
target:
  odsCodes:
    - {odsList}
journeys:
  cdssAdvice:
    provider: eConsult
    serviceDefinition: GEC_GEN
    conditionsServiceDefinition: CONDITION_LIST
  cdssAdmin:
    provider: eConsult
    serviceDefinition: CONDITION_LIST_ADMIN
'''

allCCGs = defaultdict(list)

for row in inputFile:
    allCCGs[row['ccg']].append(row['odsCode'].upper())

print allCCGs

removeExistingFiles()

for key, value in allCCGs.iteritems():
    with open(os.path.join(os.path.dirname(__file__), OLC_PATH + CCG_FILE_PREFIX + camelCaseCcgName(key) + YAML_SUFFIX), "wb") as ymlFile:
        ymlFile.write(fileTemplate.format(odsList='\n    - '.join(value)))

   