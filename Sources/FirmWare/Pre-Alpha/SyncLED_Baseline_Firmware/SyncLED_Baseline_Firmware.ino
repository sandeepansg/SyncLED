/*
  Communication framework for SyncLED v0.1
  Author(s): Sandeepan Sengupta, Tamojit Saha
  Special thanks to: Sumant Banerjee, B.Tech (Electrical Engineering), West Bengal University of Technology


  Frame format:

  Inputs are 8 charectors in an UART stream
  Charector 1 is Identifier
  Charector 2~7 are Payload in HEX charectors
  Charector 8 is Termination Indicator

  Registered identifier(s): '~' , '#' and '*'
  Registered termination indicator(s): ';'

  '~' is identified as shake back code
  '#' is identified as RGB data
  '*' is identified as information code

  RGB payload (both address and data) is 6 charectors HEX in 2+2+2 = 6 format (#000000; to #ffffff;)

  Configuration payload is converted into 24 charectors BIN from 6 charector HEX
  24 charectors BIN in in 1+8+(5+5+5) = 24 format
  1st bit (BIT_0) used to change LED State
  2nd to 9th bit (BIT_1 ~ BIT_8) is used to produce PWM code for intensity adjustment
  Rest of the bits(all 15 of them in 5+5+5 format) are used to feed shift registers secondary configuration data to drive 5x5x5 LED cube

  Accepted input formats are
  (Single frame)
  *abcdef;
  ~9EF85F;
  @1C8039;
  #ffffff;
  etc...

  (Multiple frames without seperation)
  c
  etc...

  Copyright (c) 2015-2016, Intelectron India (https://www.intelectron.in)
*/

#define termChar ';'

#define _rndRsp_

#define _debug_
//#define _$_

uint8_t RGB_pin[3]    = {9, 10, 11};
uint8_t RGB_value[3]  = {};

uint8_t unitSum       = NULL;
boolean rgbEnabled    = false;

uint32_t  code        = NULL;
uint32_t  meta        = NULL;

#ifdef  _$_
uint32_t  testNum     = NULL;
String    testStr     = "";
#endif

void setup()
{
  Serial.begin(115200);
  {
    while (!Serial);
  }
  for (uint8_t j = 0; j < 3; j++)
  {
    pinMode(RGB_pin[j], OUTPUT);
  }
}

void loop()
{
  Serial.flush();
  while (Serial.available() > 0)
  {
    String  serialData  = Serial.readStringUntil(termChar);
    serialData.trim();
    char id = serialData.charAt(NULL);
    {
      boolean change  = false;
      switch (id)
      {
        case '~':
          change = true;
          break;

        case '@':
          change = true;
          break;

        case '#':
          change = true;
          break;

#ifdef _$_
        case '$':
          change = true;
          break;
#endif

        case '*':
          change = true;
          break;

        default:
          change = false;
          break;
      }

      if (change == true)
      {
        serialData.remove(NULL, String(id).length());
        code = (uint32_t)strtol( &serialData[NULL], NULL, HEX);

#ifdef _debug_
        Serial.print("Entered string data is\t\t:\t'");
        Serial.print(serialData);
        Serial.println("'");

        Serial.print("Rectified code (in HEX) is\t:\t");
        Serial.print(code, HEX);
        Serial.println("\n");

        Serial.print("Configuration code in DEC\t:\t");
        Serial.println(code, DEC);
        Serial.print("Configuration code in BIN\t:\t");
        Serial.println(code, BIN);
        Serial.print("\n");
#endif

        uint32_t rsp = NULL;
        switch (id)
        {
          case '~':
            if (unitSum == sumDigits(code))
            {
              rgbEnabled = true;
            }
            else
            {
              rgbEnabled = false;
              unitSum = NULL;
            }
            break;

          case '@':
            meta = code;
            break;

          case '#':
            if (sumDigits(meta) == sumDigits(code) && rgbEnabled == true)
            {
#ifdef _debug_
              Serial.println("RGB Enabled");
              Serial.print("\n");
#endif
              RGB_value[0]  = code >> 16;
              RGB_value[1]  = code >> 8 & 0xFF;
              RGB_value[2]  = code & 0xFF;
            }
#ifdef _debug_
            Serial.print("RED is\t\t\t\t:\t");
            Serial.println(RGB_value[0]);
            Serial.print("GREEN is\t\t\t:\t");
            Serial.println(RGB_value[1]);
            Serial.print("BLUE is\t\t\t\t:\t");
            Serial.println(RGB_value[2]);
            Serial.print("RGB color value set is\t\t:\t(");
            Serial.print(RGB_value[0], HEX);
            Serial.print(",");
            Serial.print(RGB_value[1], HEX);
            Serial.print(",");
            Serial.print(RGB_value[2], HEX);
            Serial.println(")");
            Serial.print("\n");
#endif
            for (uint8_t j = NULL; j < 3; j++)
            {
              analogWrite(RGB_pin[j], RGB_value[j]);
            }
            break;

#ifdef _$_
          case '$':
            Serial.print(sumDigits(code));
            Serial.print("\tis the sumDigits(code) for the original code\t0x");
            Serial.println(code, HEX);
            testNum = randRes(code);
            Serial.print(sumDigits(testNum));
            Serial.print("\tis the sumDigits(code) for the randResp(code)\t0x");
            Serial.println(testNum, HEX);
            testStr = randRspStr(code);
            Serial.print("\tis the sumDigits(code) for the randRspStr(code)\t0x");
            Serial.println(testStr);
            Serial.print("\n");
            break;
#endif

          case '*':
#ifdef _rndRsp_
            unitSum = sumDigits(code);
            rsp = randRes(code);
#endif
#ifndef _rndRsp_
            rsp = sumDigits(code);
            unitSum = rsp;
#endif
            writeString("~" + String(rsp, HEX) + ";");
            Serial.write("\n\r\0");
            break;

          default:
            exit(NULL);
            break;
        }
      }
    }
  }
}


/*User derived Functions*/

uint8_t sumDigits(uint32_t num)
{
  uint8_t sumOne = 0;
  while (num > 0)
  {
    sumOne += num % 10;
    num /= 10;
  }
  if (sumOne > 9)
  {
    sumOne = sumDigits(sumOne);
  }
  return sumOne;
}

uint32_t randRes(uint32_t information)
{
  boolean match;
  while (match == false)
  {
    uint32_t response = random(NULL, pow(2, sizeof(uint32_t) * String(information, HEX).length()));
    if (information != response && sumDigits(information) == sumDigits(response))
    {
      match = true;
      return response;
    }
  }
}

#ifdef _$_
String randRspStr(uint32_t information)
{
  boolean match;
  while (match == false)
  {
    uint32_t response = random(NULL, pow(2, sizeof(uint32_t) * String(information, HEX).length()));
    if (information != response && sumDigits(information) == sumDigits(response))
    {
#ifdef _$_
      Serial.print(sumDigits(response));
#endif
      if (String(information, HEX).length() == String(response, HEX).length())
      {
        match = true;
        return String(response, HEX);
      }
      else
      {
        String rspStr = String(response, HEX);
        while (rspStr.length() < String(information, HEX).length())
        {
          rspStr = "0" + rspStr;
        }
        return rspStr;
      }
    }
  }
}
#endif

void writeString(String stringData)
{
  for (uint8_t i = 0; i < stringData.length(); i++)
  {
    Serial.write(stringData[i]);
  }
}

/*
  uint32_t RAM_left ()
  {
  extern uint32_t __heap_start, *__brkval;
  uint32_t val;
  uint32_t left = (int) &val - (__brkval == 0 ? (int) &__heap_start : (int) __brkval);
  return left;
  }
*/
