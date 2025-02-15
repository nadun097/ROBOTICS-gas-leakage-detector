#include <Wire.h>                 
#include <LiquidCrystal_I2C.h>    
#include <SoftwareSerial.h>       
#include <Servo.h>

#define gasSensor A0              
#define irSensor A1              
#define flameSensor A2           
#define BUZZER_PIN 9  

LiquidCrystal_I2C lcd(0x27, 16, 4);  
SoftwareSerial gsm(7, 8);    
Servo motor;

// const char phoneNumber[] = "+94760755522"; // Replace with the recipient's number
const char phoneNumber[] = "+94765512595"; 

String message = "";

int solenoid = 2;                 
int servo = 4;                    
int gasSensorValue;               
int flameSensorValue;             
String command;                   
String msgSent = "false";
String windowMode = "auto";
String timePassed = "false";
bool alarmTriggered = false;      

void setup() {
  Serial.begin(9600);             
  gsm.begin(9600);                
  motor.attach(servo);
  motor.write(0);
  pinMode(solenoid, OUTPUT);      
  pinMode(gasSensor, INPUT);      
  pinMode(irSensor, INPUT);
  pinMode(flameSensor, INPUT);
  pinMode(BUZZER_PIN, OUTPUT);
  lcd.begin(16, 4);               
  lcd.backlight();                
  gsmSetup();                     
}

void loop() {
  SensorValues();                 
  handleCommand();                

 
  if (alarmTriggered) {
    tone(BUZZER_PIN, 100); 
    delay(600);
    noTone(BUZZER_PIN);
    delay(300);
  }
}

void SensorValues() {
  delay(500);
  gasSensorValue = analogRead(gasSensor);
  flameSensorValue = analogRead(flameSensor); 

  Serial.print(gasSensorValue);
  Serial.print(",");
  Serial.println(flameSensorValue);

  lcd.setCursor(0, 0);            
  lcd.print("Gas Value: ");       
  lcd.print(gasSensorValue);      


// Gas Condition
  if (gasSensorValue >= 350 || flameSensorValue <= 500) {  
    digitalWrite(solenoid, HIGH);     
    motor.write(150);

    alarmTriggered = true;

    if (analogRead(irSensor) >= 500 && msgSent == "false") {
      message="WARNING: Gas leak detected! Please take immediate action.";
      gsmMsg();
      msgSent = "true";
      
    }
  } 
  else {
    if (windowMode == "auto") {
      motor.write(0);
    }
   
  }



}

void handleCommand() {
  if (Serial.available()) {       
    command = Serial.readString();
    command.trim();               

    if (command == "OPEN_VALVE") {
      digitalWrite(solenoid, LOW);   
    } else if (command == "CLOSE_VALVE") {
      digitalWrite(solenoid, HIGH);    
    } else if (command == "OPEN_WINDOW") {
      motor.write(150);
    } else if (command == "CLOSE_WINDOW") {
      motor.write(0);
    } else if (command == "ManualWindow") {
      windowMode = "manual";
    } else if (command == "AutoWindow") {
      windowMode = "auto";
    } else if (command == "timePassed") {
      if (analogRead(irSensor) >= 500 && timePassed == "false") {
        message="WARNING: Valve is still not Closed Take Immediate Action !!!";
        gsmMsg();
        timePassed = "true";
      }
    } else if (command == "normal") {
      timePassed = "false";
      msgSent = "false";
    } else if (command == "STOP_ALARM") { 
      alarmTriggered = false;  
      noTone(BUZZER_PIN);
    }
  }
}

void gsmSetup() {
  Serial.println("Initializing GSM Module...");
  gsm.println("AT"); 
  delay(1000);

  if (gsm.available()) {
    Serial.println("GSM Module is responding.");
    while (gsm.available()) {
      String response = gsm.readString();
      Serial.println(response);
    }
  } else {
    Serial.println("No response from GSM Module.");
  }

  gsm.println("AT+CPIN?");
  delay(1000);
  if (gsm.available()) {
    while (gsm.available()) {
      String response = gsm.readString();
      Serial.println(response);
    }
  } else {
    Serial.println("Failed to check SIM status.");
  }

  gsm.println("AT+CREG?");
  delay(1000);
  if (gsm.available()) {
    while (gsm.available()) {
      String response = gsm.readString();
      Serial.println(response);
    }
  } else {
    Serial.println("Failed to check network registration.");
  }

  Serial.println("Initialization complete.");
}

void gsmMsg() {
  gsm.println("AT+CMGF=1"); 
  delay(1000);

  gsm.print("AT+CMGS=\"");
  gsm.print(phoneNumber);
  gsm.println("\"");
  delay(1000);

  gsm.print(message);
  delay(1000);

  gsm.write(26);
  delay(3000);
}
