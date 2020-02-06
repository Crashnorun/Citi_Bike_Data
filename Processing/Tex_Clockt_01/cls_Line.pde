
class cls_Line {

  PVector stpt;
  PVector endpt;
  PVector delta;
  float duration;
  float lineLength;
  float currentLength;
  float step;
  int count;
  boolean debug;

  cls_Line(PVector StPt, PVector EndPt, float Duration) {
    stpt = StPt;
    endpt = EndPt;
    duration = Duration;

    debug = false;

    currentLength = 0;
    FindLength();

    count = 1;

    if (debug) {
      println("Stpt : " + stpt);
      println("Endpt : " + endpt);
      println("Trip Length: " + lineLength);
      println("Trip Step: " + step);
    }
  }

  void FindLength() {
    delta = PVector.sub( endpt, stpt);            // subtract start from end

    lineLength = delta.mag();           // get magnitude
    step = lineLength / duration;       // get the distance per second
    delta.setMag(step);                 // set vector to equal the length of the step
  }

  void render() {
    PVector temp = new PVector(0, 0, 0);

    if (delta.mag() < lineLength) {
      float num = step * count;          // multiplier

      delta.setMag(num);                 // set vetor equal to the step * counter
      temp = PVector.add(stpt, delta);            // add lengtht to the stpt

      stroke(0, 0, 255);
      strokeWeight(5);

      line(stpt.x, stpt.y, temp.x, temp.y);
      count++;
    } else {
      // trip is done
    }
  }
}