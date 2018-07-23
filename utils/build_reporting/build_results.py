import regex
import requests
import os
import io
import pandas as pd
from xml.etree import ElementTree

host = 'tc.nhschoices.net'
build_type = 'NHSOnline_Build_App_C1bddFullTest'


# start session with TC
s = requests.Session()
s.auth = (os.environ['TC_USERNAME'],os.environ['TC_PASS'])

# get builds from 'NHSOnline_Build_App_C1bddFullTest'
r = s.get("https://{}/httpAuth/feed.html?buildTypeId={}&itemsType=builds&buildStatus=successful&buildStatus=failed&userKey=feed".format(
    host,
    build_type
))

# extract buildIds from builds
feed = ElementTree.fromstring(r.content)
ns = {'atom':'http://www.w3.org/2005/Atom'}
links = feed.findall('atom:entry/atom:link', ns)
build_ids = [regex.search('buildId=(.+)&', link.attrib['href']).group(1) for link in links]

def get_data_from_build(id):
    serenity_results_csv = s.get("https://{}/repository/download/{}/{}:id/serenity_report.zip!/target/site/serenity/results.csv".format(
        host,
        build_type,
        id),
        stream=True)
    csv_text = b''.join(serenity_results_csv)
    csv_text = csv_text.decode('utf-8').replace('\"','')
    data = [line.split(',') for line in csv_text.split('\n')]
    return data

# get first set of test results from list of buildIds
data = get_data_from_build(build_ids[0])
cols = data.pop(0)

# populate rest of results from other buildIds
builds_added = 0
for build in build_ids[1:]:
    build_data = get_data_from_build(build)
    build_data.pop(0)
    for line in build_data:
        data.append(line)
    builds_added += 1
print("Added data for "+str(builds_added)+" builds")

# create dataframe for manipulation
df = pd.DataFrame(data, columns=cols)
# print(df.head())

# clean data
df = df.drop(columns=['Stability'])
cols = df.columns
cols = cols.map(lambda x: x.replace(' (s)',''))
df.columns = cols

# add your code into here for insights