const fs = require('fs')
var showdown  = require('showdown')

converter = new showdown.Converter()
text      = fs.readFileSync('../README.md',   { encoding: 'utf8', flag: 'r' })
html      = converter.makeHtml(text)

fs.writeFile("./index.html", html, function(err) {
    if(err) {
        return console.log(err);
    }
    console.log("The file was saved!");
})

// Or
//fs.writeFileSync('/tmp/test-sync', 'Hey there!');