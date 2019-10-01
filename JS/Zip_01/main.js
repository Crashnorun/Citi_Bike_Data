var fs = require("fs");
var JSZip = require("jszip");
var http = require("http");
var url = require("url");
var zip = new JSZip();

console.log("Hi");

console.log("DONE");

function main() {

    var new_zip = new JSZip();
    // more files !
    zip.loadAsync(content)
        .then(function (zip2) {
            // you now have every files contained in the loaded zip
            zip.file("hello.txt").async("string"); // a promise of "Hello World\n"
        });

}

function CreateFile() {
    console.log("Creating File");
    let z = zip.file("C:\\Users\\Charlie\\Documents\\GitHub\\Citi_Bike_Data\\JS\\Zip_01\\Charlie.txt", "Something \n");
    console.log(z.file);
    let zz = zip.file("C:\\Users\\Charlie\\Documents\\GitHub\\Citi_Bike_Data\\JS\\Zip_01\\Charlie.txt", "something else");
    console.log(zz.file);
}

function CreateFile2() {
    fs.appendFile("C:\\Users\\Charlie\\Documents\\GitHub\\Citi_Bike_Data\\JS\\Zip_01\\Charlie.txt", "Something againa", function (err) {
        if (err) throw err;
        console.log("saved");
    })

    var adr = 'https://s3.amazonaws.com/tripdata/201307-citibike-tripdata.zip';
    var q = url.parse(adr, true);
    console.log(q);

    http.createServer(function (req, res) {
        let q = url.parse(adr, true);
        let filename = "." + q.pathname;
        fs.readFile(filename, function (err, data) {
            if (err) {
                res.writeHead(404, { 'Content-Type': 'text/html' });
                console.log("not found")
                return res.end("404 Not Found");
            }
            res.writeHead(200, { 'Content-Type': 'text/html' });
            console.log(data);
            res.write(data);
            return res.end();
        });
    }).listen(8080);
}