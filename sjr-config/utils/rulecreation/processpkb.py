import csv
import os
from collections import defaultdict

OLC_PATH = '../../configurations/Journeys/patientsKnowBest/'
YAML_SUFFIX = '.yaml'
 
def removeExistingFiles():
    existingFilesPath = os.path.join(os.path.dirname(__file__), OLC_PATH)
    existingFiles = os.listdir(existingFilesPath)

    for file in existingFiles:
        if file.endswith(YAML_SUFFIX):
            os.remove(existingFilesPath + file )

inputFile = csv.DictReader(open(os.path.join(os.path.dirname(__file__), "pkb.csv")))

fileTemplate = '''$schema: "Schemas/Journeys/configuration_schema.json"
target:
  odsCodes:
    - {odsList}
journeys:
  silverIntegrations:
    carePlans:
      - pkb
    healthTrackers:
      - pkb
    libraries:
      - pkb
    medicines:
      - pkb
    messages:
      - pkb
    recordSharing:
      - pkb
    secondaryAppointments:
      - pkb
    testResults:
      - pkb
'''

allGroupings = defaultdict(list)

for row in inputFile:
    allGroupings[row['grouping']].append('"' + row['odsCode'].upper() + '"')

print allGroupings

removeExistingFiles()

for key, value in allGroupings.iteritems():
    with open(os.path.join(os.path.dirname(__file__), OLC_PATH + key + YAML_SUFFIX), "wb") as ymlFile:
        ymlFile.write(fileTemplate.format(odsList='\n    - '.join(value)))

   