let request = require("request");
let access = require('./Access');
let map = require('https://api.tiles.mapbox.com/mapbox-gl-js/v0.53.0/mapbox-gl.js');

console.log(map);
//main();

function main() {

    let url = constructURL();
    console.log(url);
    request(url, getdata);
}


function constructURL() {
    let prefix = "https://api.mapbox.com/v4/mapbox.mapbox-streets-v8/1/0/0/mvt?";
    // let key = "crashnorun?access_token=pk.eyJ1IjoiY3Jhc2hub3J1biIsImEiOiJjajVpbnEyYXkxZ21zMzBuejIyOGRubWNmIn0.NzvTKcEi2LCV7IGScDLYJw";
    let key = access.key;

    let url = prefix + key;

   // "https://api.mapbox.com/v4/crashnorun.id??access_token=pk.eyJ1IjoiY3Jhc2hub3J1biIsImEiOiJjajVpbnEyYXkxZ21zMzBuejIyOGRubWNmIn0.NzvTKcEi2LCV7IGScDLYJw/16.5/40.7/-73.9/.png";

    return url;
    //@40.7410015,-73.9913487,16.5z
}

function getdata(error, responce, body) {
    let val = JSON.parse(body);
    console.log(val);

}