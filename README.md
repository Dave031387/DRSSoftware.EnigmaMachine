# Enigma Machine

> [!IMPORTANT]
> *This project is a work in progress. The EnigmaV2 class library is pretty much done, but the UI portion isn't completed yet. This README file will
> be updated when the project nears completion.*

The next couple of sections go into a lot of detail about the original Enigma machine and the software version named *Enigma V2*. If you aren't
interested in all the details feel free to skip to the section describing the [Enigma V2 class library](#enigma-v2-class-library).

## Overview
According to **Wikipedia**: *"The Enigma machine is a cipher device developed and used in the early- to mid-20th century to protect commercial
diplomatic, and military communication. It was employed extensively by Nazi Germany during World War II, in all branches of the German military. The
Enigma machine was considered so secure that it was used to encipher the most top-secret messages."*

This project implements a software version of the original Enigma machine called Enigma V2. This software version has a few differences from the
original:
- The original Enigma machine was only able to work with capital letters A through Z. Special sequences of characters had to be used to represent
  punctuation. The Enigma V2 software version works with all ASCII characters between the space character (Unicode 0020) and the tilde (~) character
  (Unicode 017E). It also handles carriage return/line feed (CRLF) sequences, or just single line feeds, converting them to Unicode 017F while being
  processed and then converting them back when done.
- The original Enigma machine had several pre-wired rotors used in enciphering messages. The rotors could be swapped in and out and placed in
  different order within the machine, but the wiring of individual rotors was fixed. With the Enigma V2 software version each rotor can be dynamically
  reconfigured based on a given seed value which is used to randomize the connections, thus allowing for much more variability in the cipher process.
- The original Enigma machine had a plug board that could be manually wired to add an additional level of encryption. For example, if the E and N keys
  were wired together, then pressing the E key would send current through the N connection of the first rotor, and pressing the N key would send
  current to the E connection of the rotor. The signal coming back from the rotors would also be scrambled by the same plug board wiring. Logically
  this works the same as if another rotor was added between the keyboard and the first rotor, so the decision was made not to include this feature in
  the Enigma V2 since it doesn't really add any benefit.
- The rotors of the original Enigma machine were either stationary or rotated on a fixed cycle. The first rotor would always rotate one position after
  each letter in the message was encrypted. Other rotors would rotate after a certain number of letters were encrypted (every 10 letters, every 13
  letters, etc.). This "cycle size" was fixed for each rotor and couldn't be changed. In the Enigma V2 machine each rotor can be assigned any cycle
  size desired.
- The original Enigma machine required an operator to enter the message one character at a time using the keyboard. As each key was pressed a light
  would shine under one of the letters in the lamp board, thus showing the encrypted value of the letter that was typed on the keyboard. The operator
  would then have to record this value before pressing the key for the next letter in the message. This was a very laborious process. With the Enigma
  V2 software version an entire text file can be passed in all at once and a new text file will be created containing the encrypted text almost
  instantly.

## Inner Workings of the Enigma Machine
What follows is an attempt at explaining the inner workings of the Enigma machine without resorting to diagrams or pictures, which may be a bit hard
for some to understand. It might be useful to refer to the Wikipedia article on the [Enigma machine](https://en.wikipedia.org/wiki/Enigma_machine) to
get a more thorough understanding of what the Enigma machine is and how it operates.

### Basic Components
Although there were some variations over time, most Enigma machines had the following components in common:
- A keyboard containing 26 keys - one key for each letter in the alphabet.
- A plugboard that sat between the keyboard and the entry wheel.
- The entry wheel.
- Three or more rotors.
- A reflector.
- A lamp board containing 26 bulbs corresponding to the 26 letters of the alphabet.
- A battery used to provide current to light one of the 26 bulbs of the lamp board when one of the keys was pressed.

Each of these components are described in more detail below.

### The Keyboard
The keyboard was the means for entering text into the Enigma machine. The text that the operator typed in could either be plain text that was to be
encrypted, or it could be encrypted text that needed to be converted back into plain text. There was no SHIFT key, so all messages were done entirely
in uppercase letters. There wasn't a space key or any punctuation key or even number keys. Special combinations of letters were used to represent
these.

### The Plugboard
The plugboard contained 26 sockets - one for each letter of the alphabet. Two sockets could be patched together by plugging the ends of a cable into
the sockets. The effect of doing this was that the value of the keys corresponding to the two sockets would be swapped. For example, if the "B" socket was
patched to the "F" socket, then pressing "B" on the keyboard would send current to the "F" pin of the [entry wheel](#the-entry-wheel). Similarly, if the
"F" key was pressed then current would be sent to the "B" pin of the entry wheel.

Due to the way the Enigma machine was wired, the plugboard also affected the indicator lights in the lamp board. Once the signal made its way from
the keyboard, through the plugboard, entry wheel, the rotors, reflector, back through the rotors, and out through the entry wheel, it would pass again
through the plugboard before reaching the lamp board. If no sockets were patched together on the plugboard, then a signal coming out of a pin on
the entry wheel would light up the corresponding indicator light in the lamp board. For example, current flowing out of the "S" pin on the entry
wheel would light up "S" in the lamp board. If, however, the "S" and "D" sockets of the plugboard are patched together, then current flowing out of
the "S" pin of the entry wheel would light up "D" in the lamp board.

Typically the Germans would patch 10 pairs of plugboard sockets together when configuring the Enigma machine to encrypt a message. However, there were
some versions of the Enigma machine that didn't have a plugboard.

### The Entry Wheel
The entry wheel simply provided the means of connecting keyboard/plugboard to the first [rotor](#the-rotor). The entry wheel was stationary. It had 26
pins spaced evenly around its circumference. On one side of the wheel connections were made between each of the 26 keys of the keyboard/plugboard and
the corresponding pin of the entry wheel. For example, the "A" key would be wired to the "A" pin of the entry wheel, and the "B" key would be wired to
the "B" pin, and so on. (Remember that the plugboard can alter this, though.) The other side of the entry wheel made contact with the first rotor.
Each pin on the entry wheel came into contact with exactly one pin on the first rotor. As the rotor would rotate from one position to the next, the
pins on the entry wheel would come into contact with different pins on the rotor.

### The Rotor
The rotor was a short cylinder that had 26 pins spaced evenly around the circumference of each end of the cylinder. Again, the pins corresponded to
the 26 letters of the alphabet. Between the two ends of the cylinders wires were used connect the pins on one end to the pins on the other end. For
example, the "A" pin on one side might be connected to the "T" pin on the other side, while the "B" pin on one side might be connected to the "J" pin
on the other side, and so on. The connections between the two ends of the cylinder thus implemented a simple substitution cipher.

The Enigma machine came with a set of several rotors, each wired differently from the others. The machine itself had room for three or more rotors to
be installed for operation (depending on the particular model of Enigma machine). The operator would pick the desired rotors from the set and install
them into the Enigma machine in a predetermined order.

By convention, the right-most rotor was the first rotor. The pins on the right side of this rotor made contact with the pins on the [entry
wheel](#the-entry-wheel), while the pins on the left side of the first rotor made contact with the pins on the right side of the second rotor. The
pins on the right side of the second rotor made contact with the pins on the left side of the first rotor, and the pins on the left side of the second
rotor made contact with the pins on the right side of the third rotor, and so on until you got to the left-most rotor. The pins on the left side of
the left-most rotor made contact with the pins on the [reflector](#the-reflector).

Each rotor had a ratchet and pawl mechanism that was used to rotate the rotor one pin position. The right-most rotor would turn one pin position with
each key that was pressed on the keyboard. For the second and subsequent rotors, the rotor would turn one pin position every 13 or 26 key presses,
depending on the particular rotor. The first rotation of the second and subsequent rotors would depend on the starting position of each rotor. For
example, the second rotor might rotate on the fifth key press and then rotate ever 26 key presses after that, while the third rotor might rotate on
the second key press and every 13 key presses thereafter.

The rotor also included an alphabet ring around the rotor with the letters A through Z embossed on the ring. The ring could be rotated around the
rotor so that the letters aligned with the pins of the rotor. Thus there were a total of 26 possible positions that the ring could be set to on each
rotor. This came into play when the Enigma machine was being configured by the operator to prepare it for encrypting or decrypting a message. The
position of the alphabet ring determined at what point the next rotor to the left of the current rotor would rotate.

### The Reflector
The reflector was a wheel with 26 pins spaced evenly around its circumference. These pins came into contact with the pins on the left side of the
left-most [rotor](#the-rotor). The pins of the reflector were wired in pairs. The reflector took the current coming in from one pin of the left-most
rotor and sent it back out through a different pin in that rotor. In most versions of the Enigma machine the rotor was stationary, although some
models allowed the rotor to be manually turned to a specific position before typing on the keyboard.

### The Lamp Board
The lamp board provided the output mechanism for the Enigma machine. When a key was pressed on the keyboard one of the lamps of the lamp board would
light up under a letter of the alphabet. If the operator was keying in the plain text message, the letter that was lit up would be the encrypted value
for the key that was pressed. If the operator was keying in an encrypted message, then the plain text value would be lit up. Thus the same Enigma
machine could be used to encrypt and decrypt a message. The only requirement was that the machine had to start out in exactly the same configuration
that it was in at the start when the plain text message was being encrypted.

### The Battery
The Enigma machine used a battery to supply power and to light up the lamps on the lamp board. If no key was pressed there was no closed circuit and
none of the lamps would light up. Pressing a key created a closed circuit and caused one of the lamps to light.

## Operation of the Enigma Machine
Before encrypting a message the operator had to configure the Enigma machine in a predetermined manner. The German military used code books that
specified what configuration to use on any given day of the month. The code books specified the following details of the configuration of the Enigma
machine:
- Which rotors to use and the order to place them in
- Where to set the alphabet ring on each rotor
- Which sockets to patch together on the plugboard
- The starting position of the rotors (i.e., which letter on the alphabet ring should show for each rotor through the windows above the lamp board on
  top of the Enigma)

The operator would then start typing the plain text message on the keyboard. Each key pressed would light up single lamp on the lamp board showing a
letter of the alphabet. This letter was the encrypted form of the character that was just typed on the keyboard. The operator would write down that
value and then type the next character from the plain text. This process would continue until the entire plain text was typed and the corresponding
encrypted letters were recorded.

The encrypted message would then be sent off to the intended party. The Enigma machine at the receiving end would be configured according to the
directions given in their copy of the code book. The configuration had to be identical to what was used at the beginning of the encryption process in
order for this to work. Even one misconfigured setting would result in garbage being returned during the decryption process. The operator would then
repeat the process that was used during encryption, only they would be typing in the encrypted message on the keyboard. The lamps on the lamp board
would then reveal the characters of the original plain text message, one character at a time.

To illustrate what typing in one letter on the keyboard might return, let's assume that the Enigma machine is configured as follows:

```
Plugboard

    ABCDEFGHIJKLMNOPQRSTUVWXYZ  (sockets)
    V QPHZ EJIR YTWDCK N AO MF  (patch)

Rotor #1

    ABCDEFGHIJKLMNOPQRSTUVWXYZ  (right side)
    GNUBIPWZKRYFMTAHOVCJQXELSD  (left side)

Rotor #2

    ABCDEFGHIJKLMNOPQRSTUVWXYZ  (right side)
    FKPUZEJOTYDINSXCHMRWBGLQVA  (left side)

Rotor #3

    ABCDEFGHIJKLMNOPQRSTUVWXYZ  (right side)
    LWHSDOZKVGRCNYJAFQBMXITEPU  (left side)

Reflector

    ABCDEFGHIJKLMNOPQRSTUVWXYZ
    RCBWJULONETGYIHXSAQKFZDPMV
```

Let's say that the operator presses the "L" key. Here's a step-by-step description of the path that the current would follow to light up a lamp on the
lamp board:
1. The "L" key on the keyboard would send current to the "L" socket on the plugboard.
1. We see that the "L" socket isn't patched to any other socket, therefore the plugboard would send current to the "L" pin on the right side of Rotor
   #1.
1. The "L" pin on the right side of Rotor #1 is wired to the "F" pin on the left side of Rotor #1. Therefore, current will be sent to the "F" pin on
   the right side of Rotor #2.
1. The "F" pin on the right side of Rotor #2 is wired to the "E" pin on the left side of Rotor #2. Therefore, current will be sent to the "E" pin on
   the right side of Rotor #3.
1. The "E" pin on the right side of Rotor #3 is wired to the "D" pin on the left side of Rotor #3. Therefore, current will be sent to the "D" pin of
   the Reflector.
1. The "D" pin of the reflector is wired to the "W" pin. Therefore, current will be sent back to the "W" pin on the left side of Rotor #3.
1. The "W" pin on the left side of Rotor #3 is wired to the "B" pin on the right side of Rotor #3. Therefore, current will be sent to the "B" pin on
   the left side of Rotor #2.
1. The "B" pin on the left side of Rotor #2 is wired to the "U" pin on the right side of Rotor #2. Therefore, current will be sent to the "U" pin on
   the left side of Rotor #1.
1. The "U" pin on the left side of Rotor #1 is wired to the "C" pin on the right side of Rotor #1. Therefore, current will be sent to the "C" socket
   of the plugboard.
1. The "C" socket of the plugboard is patched to the "Q" socket. Therefore, current will be sent to the "Q" lamp on the lamp board.
1. The operator would then write down "Q" before typing in the next letter of the message.

This whole process could be written more concisely like so (note the second line is read right-to-left):

```
  Keyboard(L) > (L)Plugboard(L) > (L)Rotor #1(F) > (F)Rotor #2(E) > (E)Rotor #3(D) > (D)Reflector --|
Lamp Board(Q) < (Q)Plugboard(C) < (C)Rotor #1(U) < (U)Rotor #2(B) < (B)Rotor #3(W) < (W)Reflector <-|
```

After the first key was pressed, Rotor #1 would rotate one pin position. So, effectively its wiring would now look like this:

```
Rotor #1

    ABCDEFGHIJKLMNOPQRSTUVWXYZ  (right side)
    EHOVCJQXALSZGNUBIPWDKRYFMT  (left side)
```

The way to think of this is that the pin positions of the rotors and reflector are always taken in relation to the stationary entry wheel. So, the pin
on Rotor #1 that is in contact with the "A" pin of the entry wheel would be in contact with the "B" pin of the entry wheel after it was rotated one
position. Likewise, before Rotor #1 was rotated, that pin would have been wired to the "G" pin on the other side of the rotor. That pin also rotates
one position, becoming the "H" pin. And so now we have the "B" pin wired to the "H" pin. The wiring of the remaining pins all shift in the same
manner.

If we were to type "L" on the keyboard again at this point, it would then look like this (assuming no other rotors rotated):

```
  Keyboard(L) > (L)Plugboard(L) > (L)Rotor #1(Z) > (Z)Rotor #2(A) > (A)Rotor #3(L) > (L)Reflector --|
Lamp Board(V) < (V)Plugboard(A) < (A)Rotor #1(G) < (G)Rotor #2(J) < (J)Rotor #3(G) < (G)Reflector <-|
```

The value returned this time is "V" instead of the "Q" which was returned the first time. It was this feature that made the Enigma machine so hard to
crack since the cipher changed with each key press. One thing to note, though, is that the encrypted value can never be the same as the value that was
typed on the keyboard. Typing "L" on the keyboard will never result in the "L" lamp being lit up no matter how the Enigma machine was configured. This
is one of the features that eventually aided the Allies in cracking the Enigma code.

## Enigma V2 Class Library