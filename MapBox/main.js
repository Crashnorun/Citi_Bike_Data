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