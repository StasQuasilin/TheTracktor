#include <ESP8266WiFi.h>
#include <WiFiManager.h>
#include <GyverMotor.h>
#include <ArduinoJson.h>

#define AC_SSID "TheTractor"
#define IN_1  5
#define IN_2  4
#define IN_3  0
#define IN_4  2
#define MIN_DUTY 60

GMotor motorR(DRIVER2WIRE, IN_1, IN_2, HIGH);
GMotor motorL(DRIVER2WIRE, IN_3, IN_4, HIGH);

const char* ssid = AC_SSID;

WiFiManager wifiManager;
WiFiServer wifiServer(80);

void setup() {
  pinMode(IN_1, OUTPUT);
  pinMode(IN_2, OUTPUT);
  pinMode(IN_3, OUTPUT);
  pinMode(IN_4, OUTPUT);
  digitalWrite(IN_1, LOW);
  digitalWrite(IN_2, LOW);
  digitalWrite(IN_3, LOW);
  digitalWrite(IN_4, LOW);
  
  motorR.setMode(AUTO);
  motorL.setMode(AUTO);

  //motorR.setDirection(REVERSE);
  //motorR.setDirection(NORMAL);
  
  motorR.setMinDuty(MIN_DUTY);
  motorL.setMinDuty(MIN_DUTY);
  Serial.begin(9600);
  wifiManager.autoConnect(ssid);
  wifiServer.begin();
}

WiFiClient client;
StaticJsonDocument<48> doc;

void loop() {
  client = wifiServer.available();
  if(client){
    Serial.println("Client connected");
    while(client.connected()){
      delay(100);
      client.write("--<!!>--");
      while(client.available() > 0){
        DeserializationError error = deserializeJson(doc, client);
          if (error){
           Serial.print(F("deserializeJson() failed: "));
            Serial.println(error.f_str());
            continue;  
          }
          
          drive((int)doc["dr"], (int)doc["dl"]);
          String output;
          serializeJson(doc, output);
          Serial.println(output);
        } 
      }      
      Serial.println("Client disconnected");
    }
  }

void drive(int dutyL, int dutyR){
  dutyL = constrain(dutyL, -255, 255);
  dutyR = constrain(dutyR, -255, 255);
  
  motorL.smoothTick(dutyL);
  motorR.smoothTick(dutyR);
}
