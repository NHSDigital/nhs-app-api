const fs = require('fs');
const path = require('path');
const flipbookPath = 'flipbook';
const testDetails = JSON.parse(fs.readFileSync(`${flipbookPath}/testDetails.json`));

formatName = (name) => name.toLowerCase().split(' ').join('-');
formatDate = (date) => `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()} ${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}`;

// We want the tests with no parent journey to appear at the top
testDetails.sort((a,b) => a.ParentJourney.localeCompare(b.ParentJourney));

var outputHtml = `<html>
    <head>
        <style>
            .break:not(:last-of-type){ break-after: page;}
            body {
                font-family: Arial;
                font-size: 15px;
            }
        </style>
    </head>
<body>`;

outputHtml += `<h1>NHS App Flipbook - ${formatDate(new Date())}</h1><hr><h2>Contents</h2><div class='break'>`;

// Produce contents table
for (var idx = 0; idx < testDetails.length; idx++) {
    const { TestName, TestOutcome } = testDetails[idx];
    // 2 = Passed
    if (TestOutcome === 2) {
        // Table of contents should start from 1
        outputHtml += `<p><h3>${idx + 1} ......... ${TestName}</h3></p>`;
    }
}

outputHtml += `</div>`;

for (var idx = 0; idx < testDetails.length; idx++) {
    const { AppVersion, Device, OSVersion, ParentJourney, TestName, Folder, TestOutcome } = testDetails[idx];
    const contentIndex = idx + 1;
    
    // 2 = Passed
    if (TestOutcome === 2) {
        let ParentJourneyTestId;

        if (!ParentJourney) {
            let TestNameId = formatName(TestName);
            outputHtml += `<div class="break"><h1 id="${TestNameId}" name="${TestNameId}">${contentIndex} - ${TestName}</h1>`;
        } else {
            ParentJourneyTestId = formatName(ParentJourney);
            outputHtml += `<div class="break"><h1>${contentIndex} - ${TestName}</h1>` 
        }

        outputHtml += `<h2>AppVersion=v${AppVersion} | Device=${Device} | OS Version=${OSVersion}</h2>`;

        // If not the login journey we need to print the link to it
         if (ParentJourney) {
            outputHtml += `<p style="margin:0;padding:20px;background:blue;color:white;display:inline-block;">
            <a href="#${ParentJourneyTestId}" style="color:white;">${ParentJourney}</a></p>
            <p style="margin:0;margin-top:10px;margin-bottom:10px;font-weight:bold;font-size:30px;">&darr;</p>`;
        }

        var screenshotDir = path.resolve(`${flipbookPath}/${Folder}/screenshots`);
        var files = fs.readdirSync(screenshotDir);

        for (var screenshot in files) {
            if (!path.extname(screenshotDir[screenshot]) === ".png") {
                throw new Error('Detected non PNG format in screenshots directory. Screenshots must be in PNG format.')
            }

            outputHtml += `<div class="break" style="display:inline-block;margin-left:1em;">`;

            // Don't render arrows before first screenshot
            if (screenshot > 0 && !files[screenshot].includes('_error') && !files[screenshot].includes('_scrolled')) {
                outputHtml += `<span style="padding:10px;font-weight:bold;font-size:30px;">&rarr;</span>`;
            }
                
            outputHtml +=`<img src="${screenshotDir}/${files[screenshot]}" height="500px" width="250px" style="display:inline;vertical-align:middle;margin-bottom:1em;"/></div>`; 
        }

        outputHtml += '</div>'
    }
}

outputHtml += "</body></html>";

fs.writeFileSync('output.html', outputHtml);
