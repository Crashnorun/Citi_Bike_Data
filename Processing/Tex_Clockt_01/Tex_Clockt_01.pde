
PVector stpt;
PVector endpt;
float duration = 20;
cls_Line line;

PFont font;
int fontHeight = 20;
int padding = 5;

int prevMinute, currentMinute;


void setup() {
  size (400, 400);

  font = loadFont("ArialMT-48.vlw");
  textFont(font, fontHeight);

  prevMinute = second();
  currentMinute = prevMinute +1;

  stpt =  new PVector(10, height/2, 0);
  endpt = new PVector(width - 10, height/2, 0);
  line = new cls_Line(stpt, endpt, duration);
  // find length of line
  // give line a duraction
 
   
}


void draw() {
  drawTime();
 
}


void drawTime() {

  currentMinute = second();
  if (prevMinute != currentMinute) {
    background(0);
    if (currentMinute < 10) {
      text(hour() + " : 0" + second(), padding, height - padding);
    } else {
      text(hour() + " : " + second(), padding, height - padding);
    }

    line.render();
    prevMinute = currentMinute;
  }
}