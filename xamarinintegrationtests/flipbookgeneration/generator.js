const fs = require('fs');
const path = require('path');
const flipbookPath = 'flipbook';

var testData;
var flipbookDirectory = fs.readdirSync(flipbookPath);
var outputHtml = "<html><body>";

for (var idx = 0; idx < flipbookDirectory.length; idx++) {
    testData = fs.readFileSync(`${flipbookPath}/${flipbookDirectory[idx]}/testDetails.txt`, 'utf-8').split("\n");
    outputHtml += `<h1>${flipbookDirectory[idx]}</h1><h2>AppVersion=v${testData[0]} | Model=${testData[1]} | OS Version=${testData[2]} ${testData[3]}</h2>`;

    var screenshotDir = path.resolve(`${flipbookPath}/${flipbookDirectory[idx]}/screenshots`);
    var files = fs.readdirSync(screenshotDir);

    for (var screenshot in files) {
        if (!path.extname(screenshotDir[screenshot]) === ".png") {
            return;
        }

        if (screenshot % 4 == 0) {
            outputHtml += "<p style='margin-top:1em;'></p>";
        }

        outputHtml += `<div style="display:inline-block;margin-left:1em;">
            <span style="padding:10px;font-weight:bold;font-size:16px;">${parseInt(screenshot) + 1}</span><img src="${screenshotDir}/${files[screenshot]}" height="400px" width="200px" style="display:inline;vertical-align:top;"/>
        </div>`; 
    }

    outputHtml += "</body></html>";
}

fs.writeFileSync('output.html', outputHtml);