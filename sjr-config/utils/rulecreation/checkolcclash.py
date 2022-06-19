import csv
import os
from collections import defaultdict

ACCURX_CSV_FILENAME = 'accurx.csv'
ECONSULT_CSV_FILENAME = 'econsult.csv'
OVERRIDES_FILENAME = 'olcoverrides.csv'

accurx = csv.DictReader(open(os.path.join(os.path.dirname(__file__), ACCURX_CSV_FILENAME)))
econsult = csv.DictReader(open(os.path.join(os.path.dirname(__file__), ECONSULT_CSV_FILENAME)))
overrides = list(csv.DictReader(open(os.path.join(os.path.dirname(__file__), OVERRIDES_FILENAME))))

accurxOdsCodes = []
econsultOdsCodes = []

for row in accurx:
    accurxOdsCodes.append(row['odsCode'].upper()) 
for row in econsult:
    econsultOdsCodes.append(row['odsCode'].upper())   

clashes = [element for element in accurxOdsCodes if element in econsultOdsCodes]

if len(clashes) == 0:
    print('No OLC clashes detected')
else:  
    print('Clash/es detected - please check the following and amend as necessary:')
    for element in clashes:
        reason = 'unknown'
        winner = 'unknown'
        for row in overrides:
            if row['odsCode'].upper() == element:
                reason = row['reason']
                winner = row['overridingSupplier']

        print('ODS code: ' + element + ' Winner: ' + winner + ' Reason: ' + reason)
