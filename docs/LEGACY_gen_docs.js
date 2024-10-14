const fs = require('fs')
var showdown  = require('showdown')

const css = '<link rel="stylesheet" href="https://unpkg.com/terminal.css@0.7.4/dist/terminal.min.css" />'
converter = new showdown.Converter()
text      = fs.readFileSync('../README.md',   { encoding: 'utf8', flag: 'r' })
html      = css + converter.makeHtml(text)
fs.writeFileSync('./index.html', html)
console.log('Docs created.')