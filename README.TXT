Example for access to a MAXIM 127 chip
12 bit data aquisitions system with 8 channels communication via IIC

You need an IOWarrior with a MAXIM 127 wired on the breadboard area.

Best get the datasheet for the MAX127 as PDF from http://www.maxim-ic.com

The demo reads all 8 channels with a timer set to 200 msec.
Keep in mind that each read or write to the IO-Warrior can take 8 msec.
Each channel mode is set to measuring +/- 10 V because this is the
most secure and informative setup for experiments.
The value returned for each channel is a 12 bit signed integer.
It is sign extended to 32 bit and then scaled to the +/- 10 V range and displayed.

IOWarrior 56 not supported by the example for lack of test device.