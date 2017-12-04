/*
  Infineo Communication framework for SyncLED™ (Version 0.9)
  Author(s): Sandeepan Sengupta, Tamojit Saha

  Frame format:

  Inputs are 8 charectors in an UART stream
  Charector 1 is Identifier
  Charector 2~7 are Payload in HEX charectors
  Charector 8 is Termination Indicator

  Registered identifier(s): '~' , '!' , '@' , '#' and '*'
  Registered termination indicator(s): ';'

  '~' is identified as shake back code
  '!' is identified as override request identification code
  '@' is identified as random response identification code (receive)
  '@' is identified as random referance identification code (send)
  '#' is identified as RGB data identification code
  '*' is identified as handshake request identification code

  RGB payload is 6 charectors HEX in 2+2+2 = 6 format (#000000; to #ffffff;)

  Accepted input formats are
  (Single frame)
  eg. *abcdef;
      ~9EF85F;
      @1C8039;
      #ffffff;
  etc...

  (Multiple frames without seperation)
  eg. *63962D;~58F5C;
  @0397DD;#64F5ED;
  etc...

  Test String: *63962D;~58F5C;@300000;#ff0000;@300000;#00ff00;@300000;#0000ff;
  Override Command is !2DFA1067; [ 0771362919 or higher sumDigits() == 9 ]
  @300000;#ff0000;@300000;#00ff04;@300000;#0000ff;!2DFA1067;

  Copyright (c) 2013-2018, IObitZ (https://www.iobitz.com)
*/

#include <avr/interrupt.h>
#include <avr/power.h>
#include <avr/sleep.h>
#include <avr/wdt.h>

#define termChar ';'

#define _idleRecovery_  60000   //MilliSeconds
#define idleTime        10000   //MilliSeconds
#define delayValue      2500    //MicroSeconds
#define pwm_res         8
#define _delay_

#define _return_
//#define _idleLock_    //Disable RGB at the event of inactive blackOut
#define _powerSaver_  //Disable for older AVR (eg. ATmega8)

//#define _initDebug_
//#define _debug_
//#define _blackoutEvent_
//#define _intruderDetect_
//#define _colorDebug_
//#define _fadeDebug_
//#define _TimeStamp_

bool      intruderFlag  = LOW;
boolean   flip          = false;
boolean   rgbEnabled    = false;
boolean   validEvent    = false;
boolean   blackoutState = false;
boolean   sampleWaiting = false;

uint8_t   RGB_value[3]  = {};
uint8_t   RGB_pin[3]    = {9, 10, 11};

uint8_t   index         = NULL;
uint8_t   unitSum       = NULL;
uint8_t   autoValue     = NULL;
uint8_t   current_bit   = NULL;

uint16_t  counter       = NULL;

uint32_t  meta          = NULL;
uint32_t  seed          = NULL;
uint32_t  sample        = NULL;
uint32_t  response      = NULL;
uint32_t  challenge     = NULL;
uint32_t  idleColor     = NULL;
uint32_t  eventStamp    = NULL;

void setup()
{
  wdt_reset();
  wdtSetup();

  Serial.begin(115200);
  {
    while (!Serial);
  }
  for (uint8_t i = 0; i < 3; i++)
  {
    pinMode(RGB_pin[i], OUTPUT);
  }

#ifdef _debug_
  Serial.println("Infineo Communication framework for SyncLED™ v0.9\nAuthor(s): Sandeepan Sengupta, Tamojit Saha\nCopyright (c) 2015-2017, IObitZ (https://www.iobitz.com)\n\n");
#endif

}

void loop()
{
  if (sampleWaiting)
  {
    sampleWaiting = false;

    seed = rotl(seed, 1); // Spread randomness around
    seed ^= sample; // XOR preserves randomness

    current_bit++;
    if (current_bit > 31)
    {
      current_bit = NULL;
    }
  }

  uint8_t RGB_Old_Val[3] = {};
  for (uint8_t i = 0; i < 3; i++)
  {
    RGB_Old_Val[i] = RGB_value[i];
  }

  if (validEvent == true && intruderFlag == LOW)
  {
    uint32_t lapseTime = millis() - eventStamp;
    if (lapseTime >= idleTime)
    {
      eventStamp = millis();
      for (uint8_t i = NULL; i < 3; i++)
      {
        if (RGB_value[i] != NULL)
        {
          RGB_value[i]  = NULL;
        }
      }

      if (rgbEnabled == true)
      {

#ifdef  _blackoutEvent_
        Serial.print("Inactivity in milliseconds:\t");
        Serial.println(lapseTime);
        Serial.print("Blackout triggered at:\t");
        Serial.print(millis());
        Serial.println("th milliseconds");
#endif

#ifdef _idleLock_
        rgbEnabled  = false;
#endif

        validEvent  = false;

#ifdef _return_
        WriteStringLn("696e616374697665"); //inactive
#endif

        blackoutState = blackOut(RGB_pin, RGB_Old_Val, delayValue);

#ifdef _return_
        WriteStringLn("426c61636b4f7574\n");  //BlackOut
#endif


      }

      if (blackoutState == true)
      {

#ifdef _powerSaver_
        powerSaver();
#endif

      }
    }
  }

  uint32_t  lockdownTimer = millis();
  if (intruderFlag == HIGH)
  {
    if (lockdownTimer == NULL)  //Timer overflow
    {
      intruderFlag = LOW;

#ifdef _intruderDetect_
      Serial.print("Lockdown disengaged");
#endif

      wdt_enable(WDTO_15MS);    //Schedule a !RESET after 15ms
      block();                  //Block program execution and wait for reset
    }
  }

  if (index > 3)
  {
    index = NULL;
  }

#ifdef _initDebug_
  Serial.print(index);
  Serial.print("\t");
  Serial.print(flip);
  Serial.print("\t");
  Serial.print(RGB_value[0]);
  Serial.print("\t");
  Serial.print(RGB_value[1]);
  Serial.print("\t");
  Serial.print(RGB_value[2]);
  Serial.print("\n");
#endif

  if (blackoutState == false && rgbEnabled == false)
  {
    for (uint8_t i = NULL; i < 3; i++)
    {
      analogWrite(RGB_pin[i], RGB_value[i]);
    }
    delayMicroseconds(delayValue);
    RGB_value[index] = counter;
    if (counter > (pow(2, pwm_res) - 1))
    {
      flip = !flip;
      index++;
    }
    if (!flip)
    {
      counter++;
    }
    else
    {
      counter--;
    }
    if (counter == NULL)
    {
      flip = !flip;
    }
    if (index == 3)
    {
      counter = pow(2, pwm_res); //Artificially creating the exit condition
      flip = false;
    }
  }

#ifdef _idleRecovery_
  if (blackoutState == true && intruderFlag == LOW && ((millis() - eventStamp) >= _idleRecovery_))
  {
    wdt_enable(WDTO_15MS);    //Schedule a !RESET after 15ms
    block();                  //Block program execution and wait for reset
  }
#endif

  Serial.flush();
  while (Serial.available() > NULL)
  {
    String  serialData  = Serial.readStringUntil(termChar);
    serialData.trim();
    char id = serialData.charAt(NULL);
    {
      switch (id)
      {
        case '~':
          if (intruderFlag == HIGH)
          {

#ifdef _intruderDetect_
            Serial.print("Code injection detected.\nSystem lockdown triggered !!!\nTry after:\t");
            Serial.print(- lockdownTimer);
            Serial.println(" MilliSeconds\n");
#endif

#ifdef _return_
            WriteStringLn("4c6f636b446f776e"); //LockDown
#endif

            Serial.flush();
          }
          else  if  (intruderFlag == LOW)
          {
            validEvent = true;
          }
          break;

        case '!':
          if (intruderFlag == HIGH)
          {
            validEvent = true;
          }
          else  if  (intruderFlag == LOW)
          {
            validEvent = false;
          }
          break;

        case '@':
          if (intruderFlag == HIGH)
          {
#ifdef _intruderDetect_
            Serial.print("Code injection detected.\nSystem lockdown triggered !!!\nTry after:\t");
            Serial.print(- lockdownTimer);
            Serial.println(" MilliSeconds\n");
#endif

#ifdef _return_
            WriteStringLn("4c6f636b446f776e"); //LockDown
#endif

            Serial.flush();
          }
          else  if  (intruderFlag == LOW)
          {
            validEvent = true;
          }
          break;

        case '#':
          if (intruderFlag == HIGH)
          {

#ifdef _intruderDetect_
            Serial.print("Code injection detected.\nSystem lockdown triggered !!!\nTry after:\t");
            Serial.print(- lockdownTimer);
            Serial.println(" MilliSeconds\n");
#endif

#ifdef _return_
            WriteStringLn("4c6f636b446f776e"); //LockDown
#endif
            Serial.flush();
          }
          else  if  (intruderFlag == LOW)
          {
            validEvent = true;
          }
          break;

        case '*':
          if (intruderFlag == HIGH)
          {

#ifdef _intruderDetect_
            Serial.print("Code injection detected.\nSystem lockdown triggered !!!\nTry after:\t");
            Serial.print(- lockdownTimer);
            Serial.println(" MilliSeconds\n");
#endif

#ifdef _return_
            WriteStringLn("4c6f636b446f776e"); //LockDown
#endif

            Serial.flush();
          }
          else  if  (intruderFlag == LOW)
          {
            validEvent = true;
          }
          break;

        default:
          exit(NULL);
          break;
      }

      if (validEvent == true)
      {
        Serial.flush();
        eventStamp = millis();

        serialData.remove(NULL, String(id).length());
        uint32_t  code = (uint32_t)strtol( &serialData[NULL], NULL, HEX);

#ifdef _debug_
        Serial.print("Entered string data is\t\t:\t'");
        Serial.print(id);
        Serial.print(serialData);
        Serial.print(termChar);
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

        boolean verifier = false;
        switch (id)
        {
          case '~':
            if (unitSum == sumDigits(code) && code != response && code != challenge)
            {
              syncIndicate(RGB_pin, RGB_value, delayValue);
              for (uint8_t i = NULL; i < 3; i++)
              {
                RGB_Old_Val[i] = RGB_value[i];
              }
              rgbEnabled = true;
              randomSeed(randRes(code));
              WriteStringLn("@" + String(random(), HEX) + String(termChar));

#ifdef _debug_
              Serial.println("RGB Enabled");
              Serial.print("\n\0\r");
#endif

            }
            else
            {
              validEvent    = false;
              unitSum       = NULL;
              rgbEnabled    = false;
              intruderFlag  = HIGH;
              if (blackoutState == false)
              {

#ifdef _return_
                WriteStringLn("696e747275646572"); //intruder
#endif

                blackoutState = blackOut(RGB_pin, RGB_Old_Val, delayValue);

#ifdef _return_
                WriteStringLn("426c61636b4f7574\n");  //BlackOut
#endif

              }
            }
            break;

          case '!':
            if ( code >= 771362919 && sumDigits(code) == sumDigits(9192631770) )
            {

#ifdef _debug_
              Serial.println("Forced RESET");
              Serial.print("\n\0\r");
#endif

              WriteStringLn("6f76657272696465"); //override

              wdt_enable(WDTO_15MS);    //Schedule a !RESET after 15ms
              block();                  //Block program execution and wait for reset
            }
            break;

          case '@':
            meta = code;
            break;

          case '#':
            if (meta != code || code == NULL || code == 0xFFFFFF)
            {
              verifier = true;
            }
            if (sumDigits(meta) == sumDigits(code) && verifier == true)
            {
              if (rgbEnabled == true)
              {
                RGB_value[0]  = code >> 16;
                RGB_value[1]  = code >> 8 & 0xFF;
                RGB_value[2]  = code & 0xFF;

                fadeRGB(RGB_value, RGB_Old_Val, delayValue);

                WriteStringLn("@" + String(randRes(meta), HEX) + String(termChar));
              }
            }
            else
            {
              validEvent    = false;
              intruderFlag  = HIGH;

#ifdef _return_
              WriteStringLn("696e747275646572"); //intruder
#endif

              if (blackoutState == false)
              {
                blackoutState = blackOut(RGB_pin, RGB_Old_Val, delayValue);

#ifdef _return_
                WriteStringLn("426c61636b4f7574\n");  //BlackOut
#endif

              }
            }
            break;

          case '*':
            challenge = code;
            response  = randRes(code);
            unitSum   = sumDigits(code);
            WriteStringLn("~" + String(response, HEX) + String(termChar));
            break;

          default:
            exit(NULL);
            break;
        }

#ifdef _debug_
        if (RGB_value[0] == NULL && RGB_value[1] == NULL && RGB_value[2] == NULL)
        {
          Serial.println("RGB Disabled");
          Serial.print("\n\0\r");
        }
#endif

      }

#ifdef _blackoutEvent_
      if (validEvent == true)
      {
        Serial.print("Valid Event for\t");
        Serial.print(id);
        Serial.print("\thappened at:\t");
        Serial.print(eventStamp);                   //Serial buffer frezes here sometimes
        Serial.println("th MilliSeconds\n");
      }
#endif

    }
  }
}



/*User derived Functions*/

void block()
{
  for (;;)
  {
    ;
  }
}

void syncIndicate(uint8_t pinRGB[3], uint8_t RGB_PWM[3], uint32_t Interval)
{
  for (uint8_t i = NULL; i < 3; i++)
  {
    while (RGB_PWM[i] != (pow(2, pwm_res) - 1))
    {
      RGB_PWM[i]++;

#ifdef _colorDebug_
      for (uint8_t i = NULL; i < 3; i++)
      {
        Serial.flush();
        Serial.print(i);
        Serial.print(":\t");
        Serial.println(RGB_PWM[i]);
      }
#endif

      analogWrite(pinRGB[i], RGB_PWM[i]);

#ifdef _delay_
      delayMicroseconds(Interval);
#endif

    }
  }
}

boolean blackOut(uint8_t pinRGB[3], uint8_t RGB_PWM[3], uint32_t Interval)
{
  for (uint8_t i = NULL; i < 3; i++)
  {
    while (RGB_PWM[i] != NULL)
    {
      RGB_PWM[i]--;

#ifdef _colorDebug_
      for (uint8_t i = NULL; i < 3; i++)
      {
        Serial.flush();
        Serial.print(i);
        Serial.print(":\t");
        Serial.println(RGB_PWM[i]);
      }
#endif

      analogWrite(pinRGB[i], RGB_PWM[i]);

#ifdef _delay_
      delayMicroseconds(Interval);
#endif

    }
  }
  return (true);
}

uint8_t sumDigits(uint64_t num)
{
  uint8_t sumOne = NULL;
  while (num > NULL)
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
    randomSeed(micros() + seed);
    uint32_t response = random(NULL, pow(2, sizeof(uint32_t) * String(information, HEX).length()));
    if (information != response && sumDigits(information) == sumDigits(response))
    {
      match = true;
      return response;
    }
  }
}

void WriteString(String stringData)
{
  for (uint8_t i = NULL; i < stringData.length(); i++)
  {
    Serial.write(stringData[i]);
  }
}

void WriteStringLn(String stringData)
{
  for (uint8_t i = NULL; i < stringData.length(); i++)
  {
    Serial.write(stringData[i]);
  }
  Serial.write("\n\0\r");
}

void fadeRGB (uint8_t RGB_currVal[3], uint8_t RGB_preVal[3], uint32_t Interval)
{

#ifdef _TimeStamp_
  uint32_t startPoint = millis();
#endif

  for (uint8_t i = NULL; i < 3; i++)
  {
    RGB_currVal[i] = RGB_value[i];
  }

  for (uint8_t i = NULL; i < 3; i++)
  {
    if (RGB_preVal[i] > RGB_currVal[i])
    {
      while (RGB_preVal[i] > RGB_currVal[i])
      {
        {
          RGB_preVal[i]--;
          analogWrite(RGB_pin[i], RGB_preVal[i]);

#ifdef _fadeDebug_
          Serial.println("Decrementing...");
          Serial.print("RED is\t\t\t\t:\t");
          Serial.println(RGB_preVal[0]);
          Serial.print("GREEN is\t\t\t:\t");
          Serial.println(RGB_preVal[1]);
          Serial.print("BLUE is\t\t\t\t:\t");
          Serial.println(RGB_preVal[2]);
          Serial.print("RGB color value set is\t\t:\t(");
          Serial.print(RGB_preVal[0], HEX);
          Serial.print(",");
          Serial.print(RGB_preVal[1], HEX);
          Serial.print(",");
          Serial.print(RGB_preVal[2], HEX);
          Serial.println(")");
          Serial.print("\n");
#endif

        }

#ifdef  _delay_
        delayMicroseconds(Interval);
#endif

      }
    }
    else if (RGB_preVal[i] < RGB_currVal[i])
    {
      while (RGB_preVal[i] < RGB_currVal[i])
      {
        {
          RGB_preVal[i]++;
          analogWrite(RGB_pin[i], RGB_preVal[i]);

#ifdef _fadeDebug_
          Serial.println("Incrementing...");
          Serial.print("RED is\t\t\t\t:\t");
          Serial.println(RGB_preVal[0]);
          Serial.print("GREEN is\t\t\t:\t");
          Serial.println(RGB_preVal[1]);
          Serial.print("BLUE is\t\t\t\t:\t");
          Serial.println(RGB_preVal[2]);
          Serial.print("RGB color value set is\t\t:\t(");
          Serial.print(RGB_preVal[0], HEX);
          Serial.print(",");
          Serial.print(RGB_preVal[1], HEX);
          Serial.print(",");
          Serial.print(RGB_preVal[2], HEX);
          Serial.println(")");
          Serial.print("\n");
#endif

#ifdef  _delay_
          delayMicroseconds(Interval);
#endif

        }
      }
    }
    else
    {
      {
        analogWrite(RGB_pin[i], RGB_preVal[i]);

#ifdef _fadeDebug_
        Serial.println("Color Refresh");
        Serial.print("RED is\t\t\t\t:\t");
        Serial.println(RGB_preVal[0]);
        Serial.print("GREEN is\t\t\t:\t");
        Serial.println(RGB_preVal[1]);
        Serial.print("BLUE is\t\t\t\t:\t");
        Serial.println(RGB_preVal[2]);
        Serial.print("RGB color value set is\t\t:\t(");
        Serial.print(RGB_preVal[0], HEX);
        Serial.print(",");
        Serial.print(RGB_preVal[1], HEX);
        Serial.print(",");
        Serial.print(RGB_preVal[2], HEX);
        Serial.println(")");
        Serial.print("\n");
#endif

#ifdef  _delay_
        delayMicroseconds(Interval);
#endif

      }
    }
  }

#ifdef _TimeStamp_
  Serial.print("RGB transition time in milliseconds:\t");
  Serial.println(millis() - startPoint);
#endif

}

#ifdef _powerSaver_
void powerSaver()
{
  set_sleep_mode(SLEEP_MODE_IDLE);

  sleep_enable();
  power_adc_disable();
  power_spi_disable();
  power_timer0_disable();
  power_timer1_disable();
  power_timer2_disable();
  power_twi_disable();

  sleep_mode();

  sleep_disable();
  power_all_enable();
}
#endif


// Rotate bits to the left
// https://en.wikipedia.org/wiki/Circular_shift#Implementing_circular_shifts
uint32_t rotl(const uint32_t value, uint32_t shift)
{
  if ((shift &= sizeof(value) * 8 - 1) == NULL)
    return value;
  return (value << shift) | (value >> (sizeof(value) * 8 - shift));
}

// Setup of the watchdog timer.
void wdtSetup()
{
  cli();
  MCUSR = NULL;

  /* Start timed sequence */
  WDTCSR |= _BV(WDCE) | _BV(WDE);

  /* Put WDT uint32_to uint32_terrupt mode */
  /* Set shortest prescaler(time-out) value = 2048 cycles (~16 ms) */
  WDTCSR = _BV(WDIE);

  sei();
}

// Watchdog Timer Interrupt Service Routine
ISR(WDT_vect)
{
  sample = TCNT1L; // Ignore higher bits
  sampleWaiting = true;
}
