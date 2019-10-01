let request = require("request");

main();

function main() {

    let url = constructURL();
    console.log(url);
    request(url, getdata);
}


function constructURL() {
    let prefix = "https://api.mapbox.com/v4/mapbox.mapbox-streets-v8/1/0/0/mvt?";
    // let key = "crashnorun?access_token=pk.eyJ1IjoiY3Jhc2hub3J1biIsImEiOiJjajVpbnEyYXkxZ21zMzBuejIyOGRubWNmIn0.NzvTKcEi2LCV7IGScDLYJw";
    let key = "access_token=pk.eyJ1IjoiY3Jhc2hub3J1biIsImEiOiJjajVpbnEyYXkxZ21zMzBuejIyOGRubWNmIn0.NzvTKcEi2LCV7IGScDLYJw";

    let url = prefix + key;
    return url;
    //@40.7410015,-73.9913487,16.5z
}

function getdata(error, responce, body) {
    let val = JSON.parse(body);
    console.log(val);
}

function long2tile(lon, zoom) { return (Math.floor((lon + 180) / 360 * Math.pow(2, zoom))); }
function lat2tile(lat, zoom) { return (Math.floor((1 - Math.log(Math.tan(lat * Math.PI / 180) + 1 / Math.cos(lat * Math.PI / 180)) / Math.PI) / 2 * Math.pow(2, zoom))); }