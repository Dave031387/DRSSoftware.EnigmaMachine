# Enigma Machine
The next couple of sections go into a lot of detail about the original Enigma machine and the software version named *Enigma V2*. If you aren't
interested in all the details feel free to skip to the section describing the [Enigma V2 class library](#the-enigma-v2-class-library). Or, skip right
to the end where I describe the [Enigma machine application](#the-enigma-machine-application) included in this repo.

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
the 26 letters of the alphabet. Between the two ends of the cylinders wires were used to connect the pins on one end to the pins on the other end. For
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

After the first key was pressed, Rotor #1 would rotate one pin position. When a rotor rotates, what was the "A" pin moves one position and becomes the
"B" pin. What was the "B" pin moves and becomes the "C" pin. Until we get to the "Z" pin which becomes the "A" pin after rotation. If you look at the
chart for Rotor #1 above you see that the "A" pin on the right side of the rotor is connected to the "G" pin on the left side. So when the rotor is
rotated one position, this would become the "B" pin on the right side connected to the "H" pin on the left side. Thus we end up with the following
after rotating Rotor #1 one position:

```
Rotor #1

    ABCDEFGHIJKLMNOPQRSTUVWXYZ  (right side)
    EHOVCJQXALSZGNUBIPWDKRYFMT  (left side)
```

If we were to type "L" on the keyboard again at this point, it would then look like this (assuming no other rotors rotated):

```
  Keyboard(L) > (L)Plugboard(L) > (L)Rotor #1(Z) > (Z)Rotor #2(A) > (A)Rotor #3(L) > (L)Reflector --|
Lamp Board(V) < (V)Plugboard(A) < (A)Rotor #1(G) < (G)Rotor #2(J) < (J)Rotor #3(G) < (G)Reflector <-|
```

The value returned this time is "V" instead of the "Q" which was returned the first time even though we pressed the same key. It was this feature that
made the Enigma machine so hard to crack since the cipher changed with each key press. One thing to note, though, is that the encrypted value can
never be the same as the value that was typed on the keyboard. Typing "L" on the keyboard will never result in the "L" lamp being lit up no matter how
the Enigma machine was configured. This is one of the features that eventually aided the Allies in cracking the Enigma code.

## The Enigma V2 Class Library
The Enigma V2 class library is the core component of the Enigma Machine application. It implements the logic described above for encrypting and
decrypting messages. There are four public classes defined in this library:
- `CipherWheel` - This is an abstract base class that contains the common functionality shared by both the **Rotor** and **Reflector** classes.
- `Rotor` - This class represents a single rotor of the Enigma machine. It contains properties and methods for configuring the rotor wiring, setting
  the cipher index position, rotating the rotor, and processing a character through the rotor.
- `Reflector` - This class represents the reflector of the Enigma machine. It contains properties and methods for configuring the reflector wiring,
  setting the cipher index position, rotating the reflector, and processing a character through the reflector.
- `EnigmaMachine` - This class represents the entire Enigma machine. It contains properties and methods for configuring the machine with multiple
  rotors and a reflector, processing an entire message, and resetting the machine back to its initial state.

The class library also provides four public interfaces that define the contracts for the above classes:
- `ICipherWheel` - This interface defines the common contract shared by both the **Rotor** and **Reflector** classes.
- `IRotor` - This interface defines the contract for the **Rotor** class.
- `IReflector` - This interface defines the contract for the **Reflector** class.
- `IEnigmaMachine` - This interface defines the contract for the **EnigmaMachine** class.

These classes and interfaces will be described more fully in the sections that follow.

### The CipherWheel Class and ICipherWheel Interface
The **Rotor** and **Reflector** classes share a lot in common. This common functionality has been encapsulated in the abstract **CipherWheel** class
which both the **Rotor** and **Reflector** classes inherit from. The **ICipherWheel** interface defines the contract for the **CipherWheel** class.
Note that the **CipherWheel** class is normally only accessed indirectly through the **Rotor** and **Reflector** classes.

The **ICipherWheel** interface defines the following read-only public properties:
- `int CipherIndex` - This property corresponds to what was known as the alphabet ring position on the original Enigma machine. It determines the
  starting rotational position of the rotor or reflector. Valid values are between 0 and 95 inclusive, where 0 corresponds to the space character
  (Unicode 0020) and 95 corresponds to the DEL (delete) character (Unicode 007F).
- `int CycleCount` - This property keeps track of how many character transforms have taken place since the last time the rotor or reflector was
  rotated. The property value is incremented by 1 with each character transform.
- `int CycleSize` - This property determines how many character transforms must take place before the rotor or reflector is rotated one position.
  Valid values are between 0 and 95 inclusive. Once the *CycleCount* reaches the *CycleSize* value, the rotor or reflector is rotated one position and
  the *CycleCount* is reset back to zero. A *CycleSize* of 0 means that the rotor or reflector will never rotate.
- `bool IsInitialized` - This property indicates whether the rotor or reflector has been properly initialized. (Refer to the *Initialize* method
  description below for more details.)

The **ICipherWheel** interface also defines the following public methods:
- `void Initialize(int seed)` - This method initializes the rotor or reflector by setting up the internal wiring based on the given seed value. The
  seed is used to randomize the connections between the input and output pins of the rotor or reflector. This method must be called before the rotor
  or reflector can be used. If this method is called again with a different seed value, it will reinitialize the rotor or reflector with a new random
  wiring based on the new seed value.
- `void SetCipherIndex(int index)` - This method sets the *CipherIndex* property to the given index value. Valid values are between 0 and 95
  inclusive. The *CycleCount* property is also set to an initial value based on the new *CipherIndex* value and the *CycleSize* property.
- `int Transform(int originalValue)` - This method processes the given original value through the rotor or reflector and returns the transformed
  value. The *originalValue* parameter must be between 0 and 95 inclusive, where 0 corresponds to the space character (Unicode 0020) and 95
  corresponds to the DEL (delete) character (Unicode 007F). The method also increments the *CycleCount* property and rotates the rotor or reflector if
  necessary based on the *CycleSize* property.

The *Initialize* and *Transform* methods of the **CipherWheel** class are both abstract methods because their exact implementation differs between the
**Rotor** and **Reflector** classes. However, the *SetCipherIndex* method is implemented in the **CipherWheel** class since it works the same for both
derived classes.

The **CipherWheel** class has a single constructor that takes an integer parameter representing the cycle size for the rotor or reflector. This
constructor must be called by the constructor of any derived classes in order to properly initialize the *CycleSize* property.

```csharp
public CipherWheel(int cycleSize) {}
```

### The Rotor Class and IRotor Interface
The **Rotor** class represents a single rotor of the Enigma machine. It inherits from the abstract **CipherWheel** class and implements the **IRotor**
interface. The **IRotor** interface derives from the **ICipherWheel** interface and adds one property and two methods specific to the **Rotor** class.

> [!NOTE]
> *Rotors have two sides - left and right. Transforms can take place in either direction depending on whether the signal is going inbound towards the
> reflector or outbound coming back from the reflector. Inbound transforms go from right to left through the rotor, while outbound transforms go from
> left to right through the rotor.*

The **IRotor** interface defines the following read-only public property in addition to those inherited from the **ICipherWheel** interface:
- `bool TransformIsInProgress` - This property is set to *true* when the *Transform* method has processed an inbound character transform. It is set
  back to *false* when the *Transform* method has completed processing the oubound character transform. The sole purpose of this property is to enable
  the rotor to keep track of the inbound vs. outbound character transform since the wiring is traversed in opposite directions for each.

The **IRotor** interface also defines the following public methods in addition to those inherited from the **ICipherWheel** interface:
- `void ConnectRightComponent(IRotor rotor)` - This method connects the left side of the given rotor to the right side of this rotor.
- `void ConnectLeftComponent(ICipherWheel cipherWheel)` - This method connects the left side of this rotor to the right side of the given cipher
  wheel. The given cipher wheel will either be another rotor or a reflector.

The **Rotor** class provides the implementation for the *Initialize* and *Transform* methods inherited from the **CipherWheel** class. The
*Initialize* method sets up the internal wiring of the rotor based on the given seed value. The *Transform* method processes the given original value
through the rotor and returns the transformed value. The method also increments the *CycleCount* property and rotates the rotor if necessary based on
the *CycleSize* property.

The **Rotor** class has a single constructor that takes an integer parameter representing the cycle size for the rotor. This constructor calls the base
**CipherWheel** constructor in order to properly initialize the *CycleSize* property.

```csharp
public Rotor(int cycleSize) : base(cycleSize) {}
```

### The Reflector Class and IReflector Interface
The **Reflector** class represents the reflector of the Enigma machine. It inherits from the abstract **CipherWheel** class and implements the
**IReflector** interface. The **IReflector** interface derives from the **ICipherWheel** interface and adds one method specific to the **Reflector**
class.

> [!NOTE]
> *Unlike the rotors, the reflector only has one side (the right side) that connects to the left side of the left-most rotor. Inbound transforms
> always enter the reflector from the rotor on the right side of the reflector. The reflector then sends the signal back out through a different pin
> to the same rotor, thus starting the outbound leg of the transform.*

The **IReflector** interface defines the following public method in addition to those inherited from the **ICipherWheel** interface:
- `void ConnectRightComponent(IRotor rotor)` - This method connects the right side of this reflector to the left side of the given rotor.

The **Reflector** class provides the implementation for the *Initialize* and *Transform* methods inherited from the **CipherWheel** class. The
*Initialize* method sets up the internal wiring of the reflector based on the given seed value. The *Transform* method processes the given original
value through the reflector and returns the transformed value. The method also increments the *CycleCount* property and rotates the reflector if
necessary based on the *CycleSize* property.

The **Reflector** class has a single constructor that takes an integer parameter representing the cycle size for the reflector. This constructor calls
the base **CipherWheel** constructor in order to properly initialize the *CycleSize* property.

```csharp
public Reflector(int cycleSize) : base(cycleSize) {}
```

### The EnigmaMachine Class and IEnigmaMachine Interface
The **EnigmaMachine** class represents the entire Enigma machine. It implements the **IEnigmaMachine** interface.

The **IEnigmaMachine** interface defines the following read-only public properties:
- `bool IsInitialized` - This property indicates whether the Enigma machine has been properly initialized. (Refer to the *Initialize* method
  description below for more details.)
- `IReflector MyReflector` - This property gets the reflector component of the Enigma machine. (See the **Reflector** class description
  [above](#the-reflector-class-and-ireflector-interface) for more details.)
- `int NumberOfRotors` - This property gets the number of rotors currently configured in the Enigma machine.
- `IRotor Rotor1` - This property gets the first rotor component of the Enigma machine (the right-most rotor). (See the **Rotor** class description
  [above](#the-rotor-class-and-irotor-interface) for more details.)
- `IRotor Rotor2` - This property gets the second rotor component of the Enigma machine. The property value will be *null* if the Enigma machine is
  configured to use only one rotor.
- `IRotor Rotor3` - This property gets the third rotor component of the Enigma machine. The property value will be *null* if the Enigma machine is
  configured to use less than three rotors.
- `IRotor Rotor4` - This property gets the fourth rotor component of the Enigma machine. The property value will be *null* if the Enigma machine is
  configured to use less than four rotors.
- `IRotor Rotor5` - This property gets the fifth rotor component of the Enigma machine. The property value will be *null* if the Enigma machine is
  configured to use less than five rotors.
- `IRotor Rotor6` - This property gets the sixth rotor component of the Enigma machine. The property value will be *null* if the Enigma machine is
  configured to use less than six rotors.
- `IRotor Rotor7` - This property gets the seventh rotor component of the Enigma machine. The property value will be *null* if the Enigma machine is
  configured to use less than seven rotors.
- `IRotor Rotor8` - This property gets the eighth rotor component of the Enigma machine. The property value will be *null* if the Enigma machine is
  configured to use less than eight rotors.

The **IEnigmaMachine** interface also defines the following public methods:
- `void Initialize(string seed)` - This method initializes the Enigma machine by calling the *Initialize* method of each rotor and the reflector using
  different seed values derived from the given seed string. This method must be called before the Enigma machine can be used. If this method is called
  again with a different seed value, it will reinitialize the rotors and reflector with new random wiring based on the new seed value. The
  *IsInitialized* property is set to *true* upon successful completion of this method.
- `void ResetCipherIndexes()` - This method resets the *CipherIndex* and *CycleCount* properties of each rotor and the reflector back to their initial
  values. This will be the values set by the last call to the *SetCipherIndexes* method, or zero if the *SetCipherIndexes* method was never called.
- `void SetCipherIndexes(params int[] indexes)` - This method sets the *CipherIndex* property of each rotor and the reflector to the corresponding
  value in the given *indexes* array. The length of the *indexes* array must be equal to the number of rotors plus one (for the reflector). Valid
  values for each index are between 0 and 95 inclusive. The last value in the *indexes* array corresponds to the reflector.
- `string Transform(string text)` - This method processes the given text message through the Enigma machine and returns the transformed message. The
  method handles all ASCII characters between the space character (Unicode 0020) and the tilde (~) character (Unicode 017E). It also handles carriage
  return/line feed (CRLF) sequences, or just single line feeds, converting them to Unicode 017F while being processed and then converting them back
  when done.

The **EnigmaMachine** class has three constructors as shown below:
- The first constructor is the default constructor. It creates an Enigma machine with four rotors and a reflector with pre-defined cycle sizes for
  each.
- The second constructor takes an integer parameter representing the number of rotors to create. It creates an Enigma machine with the given number of
  rotors and a reflector with pre-defined cycle sizes for each.
- The third constructor takes a variable list of objects. The first object must be an object implementing the **IReflector** interface. The remaining
  objects must be objects implementing the **IRotor** interface. This constructor allows the caller to create an Enigma machine with any number of
  rotors. The user also has control over the cycle sizes of each rotor and the reflector since they are set when the rotor and reflector objects are
  created.

```csharp
public EnigmaMachine() {}
public EnigmaMachine(int numberOfRotors) {}
public EnigmaMachine(IReflector reflector, params IRotor[] rotors) {}
```
<br />

> [!NOTE]
> *Although you are able to create an Enigma machine with any number of rotors using the third constructor, keep in mind that you will only be able to
> access the first eight rotors through the public properties of the **EnigmaMachine** class. Normally this shouldn't be an issue unless there is a
> need to display the rotor settings in a user interface.*

### Building an Enigma Machine
This section provides examples of how to build and use an Enigma machine using the **Enigma V2** class library. The examples will have the following
characteristics:
- The Enigma machine will be built with three rotors and one reflector.
- The cycle sizes for the rotors and reflector will be set to 1, 5, 13, and 7 respectively.
- The seed string used for initializing the Enigma machine will be "ThisIsTheSecretSeedString".
- The cipher indexes for the rotors and reflector will be set to 22, 4, 65, and 48 respectively.

Given these characteristics, the following code snippet shows how to build and use the Enigma machine:

```csharp
using DRSSoftware.EnigmaV2;

// Create the rotors and reflector with the desired cycle sizes
IRotor rotor1 = new Rotor(1);
IRotor rotor2 = new Rotor(5);
IRotor rotor3 = new Rotor(13);
IReflector reflector = new Reflector(7);

// Create the Enigma machine with the rotors and reflector
IEnigmaMachine enigmaMachine = new EnigmaMachine(reflector, rotor1, rotor2, rotor3);

// Initialize the Enigma machine with the seed string
enigmaMachine.Initialize("ThisIsTheSecretSeedString");

// Set the cipher indexes for the rotors and reflector
enigmaMachine.SetCipherIndexes(22, 4, 65, 48);

// Define the message to be encrypted
string plainTextMessage = "This is the secret message that will be encrypted.\nIt's rather simple, but it illustrates what we're trying to do.";

// Encrypt the message
string encryptedMessage = enigmaMachine.Transform(plainTextMessage);

// Output the encrypted message
Console.WriteLine("Encrypted Message:");
Console.WriteLine(encryptedMessage);

// Now, let's decrypt the message back to plain text
// Reset the cipher indexes back to their initial values
enigmaMachine.ResetCipherIndexes();

// Decrypt the message
string decryptedMessage = enigmaMachine.Transform(encryptedMessage);

// Output the decrypted message
Console.WriteLine("\nDecrypted Message:");
Console.WriteLine(decryptedMessage);

// Verify that the decrypted message matches the original plain text message
Console.WriteLine($"\nDecryption Successful: {decryptedMessage == plainTextMessage}");
```

<br />

The **EnigmaMachine** class makes it easy to build and use an Enigma machine with just a few lines of code. The example above demonstrates how to
create the rotors and reflector, initialize the machine, set the cipher indexes, encrypt a message, and then decrypt it back. All of this could have
been done with a little more work by using the **Rotor** and **Reflector** classes directly and not using the **EnigmaMachine** class at all. The
following code snippet shows how to do this using nearly the same characterstics that were mentioned above:

```csharp
using DRSSoftware.EnigmaV2;

// Create the rotors and reflector with the desired cycle sizes
IRotor rotor1 = new Rotor(1);
IRotor rotor2 = new Rotor(5);
IRotor rotor3 = new Rotor(13);
IReflector reflector = new Reflector(7);

// Connect the rotors and reflector together
rotor1.ConnectLeftComponent(rotor2);
rotor2.ConnectLeftComponent(rotor3);
rotor3.ConnectLeftComponent(reflector);
reflector.ConnectRightComponent(rotor3);
rotor3.ConnectRightComponent(rotor2);
rotor2.ConnectRightComponent(rotor1);

// Initialize the Enigma machine with the seed strings
rotor1.Initialize("ThisIsTheFirstSeedString");
rotor2.Initialize("ThisIsTheSecondSeedString");
rotor3.Initialize("ThisIsTheThirdSeedString");
reflector.Initialize("ThisIsTheFourthSeedString");

// Set the cipher indexes for the rotors and reflector
rotor1.SetCipherIndex(22);
rotor2.SetCipherIndex(4);
rotor3.SetCipherIndex(65);
reflector.SetCipherIndex(48);

// Define the message to be encrypted
string plainTextMessage = "This is the secret message that will be encrypted.\nIt's rather simple, but it illustrates what we're trying to do.";

// Encrypt the message
string encryptedMessage = rotor1.Transform(plainTextMessage);

// Output the encrypted message
Console.WriteLine("Encrypted Message:");
Console.WriteLine(encryptedMessage);

// Now, let's decrypt the message back to plain text
// Reset the cipher indexes back to their initial values
rotor1.SetCipherIndex(22);
rotor2.SetCipherIndex(4);
rotor3.SetCipherIndex(65);
reflector.SetCipherIndex(48);

// Decrypt the message
string decryptedMessage = rotor1.Transform(encryptedMessage);

// Output the decrypted message
Console.WriteLine("\nDecrypted Message:");
Console.WriteLine(decryptedMessage);

// Verify that the decrypted message matches the original plain text message
Console.WriteLine($"\nDecryption Successful: {decryptedMessage == plainTextMessage}");
```

<br />

This example is logically equivalent to the first example with one exception - a unique seed string must be provided for each rotor and the reflector
when initializing them directly. When using the **EnigmaMachine** class, a single seed string is used to derive unique seed values for each rotor and
the reflector. Other than that, the process of encrypting and decrypting messages is the same. It should be obvious, however, that using the
**EnigmaMachine** class is much simpler and less error-prone than using the **Rotor** and **Reflector** classes directly.

We could also use the **EnigmaMachine** constructor that takes a single integer argument specifying the number of rotors to create. This would look
like the following example:

```csharp
using DRSSoftware.EnigmaV2;

// Create the Enigma machine with the four rotors and a reflector
IEnigmaMachine enigmaMachine = new EnigmaMachine(3);

// Initialize the Enigma machine with the seed string
enigmaMachine.Initialize("ThisIsTheSecretSeedString");

// Set the cipher indexes for the rotors and reflector
enigmaMachine.SetCipherIndexes(22, 4, 65, 48);

// Define the message to be encrypted
string plainTextMessage = "This is the secret message that will be encrypted.\nIt's rather simple, but it illustrates what we're trying to do.";

// Encrypt the message
string encryptedMessage = enigmaMachine.Transform(plainTextMessage);

// Output the encrypted message
Console.WriteLine("Encrypted Message:");
Console.WriteLine(encryptedMessage);

// Now, let's decrypt the message back to plain text
// Reset the cipher indexes back to their initial values
enigmaMachine.ResetCipherIndexes();

// Decrypt the message
string decryptedMessage = enigmaMachine.Transform(encryptedMessage);

// Output the decrypted message
Console.WriteLine("\nDecrypted Message:");
Console.WriteLine(decryptedMessage);

// Verify that the decrypted message matches the original plain text message
Console.WriteLine($"\nDecryption Successful: {decryptedMessage == plainTextMessage}");
```

<br />

The constructor used in this example allows you to create an Enigma machine having between 1 and 8 rotors. However, the cycle sizes assigned to the
rotors and reflector are pre-defined and cannot be changed. Therefore this version doesn't meet the characteristics listed at the start of this
section, but it does provide a somewhat easier way to create an Enigma machine than the first example shown above.

> [!NOTE]
> *The cycle sizes assigned to the rotors and reflector when using this constructor are taken consecutively from the following list: 1, 11, 7, 17, 13,
> 23, 29, 37, and 41. Therefore, the cycle sizes for the rotors in the above example would be 1, 11, and 7, and the cycle size for the reflector would
> be 17.*

Finally, the **EnigmaMachine** class also has a default constructor. The following code snippet shows how to use this constructor to build and use an
Enigma machine:

```csharp
using DRSSoftware.EnigmaV2;

// Create the Enigma machine with the four rotors and a reflector
IEnigmaMachine enigmaMachine = new EnigmaMachine();

// Initialize the Enigma machine with the seed string
enigmaMachine.Initialize("ThisIsTheSecretSeedString");

// Set the cipher indexes for the rotors and reflector
enigmaMachine.SetCipherIndexes(22, 4, 65, 31, 48);

// Define the message to be encrypted
string plainTextMessage = "This is the secret message that will be encrypted.\nIt's rather simple, but it illustrates what we're trying to do.";

// Encrypt the message
string encryptedMessage = enigmaMachine.Transform(plainTextMessage);

// Output the encrypted message
Console.WriteLine("Encrypted Message:");
Console.WriteLine(encryptedMessage);

// Now, let's decrypt the message back to plain text
// Reset the cipher indexes back to their initial values
enigmaMachine.ResetCipherIndexes();

// Decrypt the message
string decryptedMessage = enigmaMachine.Transform(encryptedMessage);

// Output the decrypted message
Console.WriteLine("\nDecrypted Message:");
Console.WriteLine(decryptedMessage);

// Verify that the decrypted message matches the original plain text message
Console.WriteLine($"\nDecryption Successful: {decryptedMessage == plainTextMessage}");
```

<br />

The default constructor takes away more of the flexibility that we had with the constructor used in the first example above. The default constructor
will always create an Enigma machine with four rotors and a reflector with pre-defined cycle sizes (1, 11, 7, 17, and 13 respectively). Therefore this
version fails to meet the characteristics listed at the start of this section even more so than the previous example. However, it does provide the
easiest way to create an Enigma machine of all the examples shown in this section.

## The Enigma Machine Application
This repo also includes an implementation of a working Enigma machine based on the Enigma V2 class library and writen entirely in C# .NET 8 and WPF. A
couple features have been added to this implementation that enhance the cryptographic capability of the Enigma machine. The remainder of this readme
file explain the operation of the application.

### Features
The Enigma machine application implements the following features:
- Text files can be loaded, encrypted/decrypted, and then saved
- Ability to select 3 to 8 rotors to configure the Enigma machine
- The cycle sizes are fixed for the reflector and each rotor
- The index settings of the reflector and each rotor can be automatically generated or manually selected
- The seed value used for initializing the Enigma machine can also be autmatically generated or manually entered
- A "cloaking" transformation can optionally be applied to the text after it has been encrypted, thus making it less likely that someone intercepting
  the text file will be able to successfully decrypt it
- The Enigma machine configuration can optionally be embedded within the encrypted text file, making it easier for the recipient of the text file to
  extract the decrypted text (it is hignly recommended that the cloaking transformation also be used if this option is used)
- All of the options and features of the Enigma machine are accessible through a Windows UI built on WPF and C# .NET 8

### The Enigma Machine Main Window
The main window of the application is comprised of three sections as noted by the headings across the top of the window:
- Input
- Control
- Output

The left side of the UI is devoted to the input functions. In the bottom left hand corner of the window are two buttons - the Load button for loading
text files, and the Decloak button for decloaking the loaded file if it had previously been cloaked. When text is loaded it gets stored in the large
green text box on the left side of the window.

The right side of the UI is devoted to the output functions. Encrypted or decrypted text will appear in the large green text box on the right side of
the window. In the lower right hand corner of the window are two buttons - the Cloak button for cloaking the text, and the Save button for saving the
text to a text file.

The middle portion of the main window is devoted to the control functions of the Enigma mashine. The Enigma configuration (minus the seed value) is
displayed in a column in the center of the window. At the top center of the window is the Apply Transform button used for encrypting or decrypting the
text in the input text box on the left side of the screen. Below the configuration is the Move to Input button used for moving the contents of the
output text box on the right side of the window to the input text box on the left side of the window. At the very bottom center of the window are two
more buttons - the Configure button for specifying the configuration for the Enigma machine, and the Reset button used for resetting the state of the
Enigma machine back to the point immediately after it was last configured.

> [!NOTE]
> Each button in the Enigma Machine application will be enabled for use only when the Enigma machine is in a state where that button's function is
> valid. If the button you wish to click is disabled, that means you haven't completed a necessary step in the process of encrypting or decrypting the
> text file.

### The Enigma Machine Setup Window
When the Configure button is clicked in the main window it causes the Enigma Machine Setup window to be shown. This is where you specify all the details of the Enigma machine. The window is arranged in four sections stacked vertically down the window:
- Number of Rotors
- Rotor Indexes
- Seed Value
- Options

In the first section at the top of the window you can select the number of rotors to be used in the Enigma machine. You are limited to selecting between 3 and 8 rotors.

In the next section down you can select the initial index value for each rotor and the reflector using the provided combo boxes. The last combo box on
the right is labeled "R" and is for the reflector. The reset of the combo boxes are labeled 1 through 8, depending on the number of rotors that were
selected. Each index value can be any integer from 0 through 95.

The next section down is used for entering the seed value that is used for randomizing the "wiring" of the reflector and rotors. The seed value must
be a text string consisting of at least 10 characters but no more than 94 characters.

The last section of the setup window is for other options. Currently there is only a single option available in this section. This is the option that
allows you to embed the Enigma machine configuration into the encrypted text file.

The first three sections in the setup window allow you to choose between manual and automatic configuration. When Manual is selected you are able to
manually change the settings in that section of the setup window. When Automatic is selected then the configuration in that section of the window will
be randomly selected/generated and the user will no longer be able to manually change it until Manual is selected again. You can switch between
Automatic and Manual as often as you like. Every time Automatic is selected the configuration will be randomly selected/generated again.

> [!NOTE]
> A cryptographically secure random number generator is used for generating the automatic configurations, including the random seed value as well as
> the reflector index and rotor index values. The randomly generated seed value is at least 32 characters long.

In the bottom right corner of the setup window are two buttons. The Accept button accepts the user-defined configuration and causes the Enigma machine
to be configured accordingly. The Cancel button exits the window without making any changes. Note that the Accept button will be disabled until a
valid seed value is entered.

### Other Windows
The Enigma Machine application makes use of the standard Windows Open File and Save File dialogs for loading and saving text files.

There is also a third custom window used for entering the text used for decloaking/cloaking the input/output text. This window is shown when either
the Cloak or the Decloak button is clicked in the main window. The window has a green text box for entering the cloaking text. In the lower right hand
corner are two buttons - the Accept button which accepts the text entered by the user, and the Cancel button which discards the text. Both buttons
close the window. The text entered by the user must be at least 10 characters in length before the Accept button will be enabled.

### Basic Steps for Encrypting a Text File
The basic operation of the Enigma Machine application is quite simple. The following steps can be used to encrypt a text file:
1. Click the Load button to show the Open File dialog.
1. Navigate to the text file that is to be encrypted and select it. (Notice that the preview pane is enabled by default so that you can see the
   contents of the file prior to loading it.)
1. Click Open in the Open File dialog.
1. The contents of the text file will be loaded into the green text box on the left side of the main window (this is the input text box).
1. Notice that the status in the middle of the main window shows a red indicator box with "Not Configured" displayed next to it. This is because the
   Enigma machine hasn't yet been fully configured. You won't be able to encrypt the text file until this is taken care of.
1. Click the Configure button in the bottom center of the main window. This will show the Enigma Machine Setup window.
1. Select the number of rotors you want in the Enigma machine.
1. Select an index value for each rotor and the reflector.
1. Enter a seed value that is between 10 and 94 characters in length.
1. At this point you may want to make a record of the settings if you don't think you can remember them. (It's much better, though, and more secure,
   if you pick settings you can easily remember.)
1. Click the Accept button to accept the configuration. You will be returned to the main window and the Enigma machine will be configured as you
   specified. The configuration will be displayed in the middle of the window. (The seed value is never displayed, though.) The status should now show
   a green box with "Configured" displayed next to it.
1. At this point the Apply Transform button in the top center of the main window will become enabled. Click this button to encrypt the text in the
   input text box.
1. The encrypted text will appear in the green text box on the right side of the window. (This is the output text box.)
1. The Save button in the lower right hand corner of the main window will become enabled when any text appears in the output text box. Click this
   button to display the Save File dialog.
1. Enter a name and choose a location to save the file and click Save to complete the operation.

At this point, if you wanted to verify the functioning of the Enigma machine, you could do the following:
1. Click the Move to Input button to move the contents of the output text box over to the input text box. When you do this the output text box will be
   cleared and the state of the Enigma machine will automatically be reset back to the state it was in before the Apply Transform button was pressed.
1. At this point the Apply Transform button will be enabled again. Click it.
1. Now the original text you started with should appear in the output text box. This confirms that the Enigma machine is working as expected.

### Basic Steps for Decrypting a Text File
The steps for decrypting a text file are nearly identical to the steps used for encrypting the file.
1. Click the Load button to show the Open File dialog.
1. Navigate to the text file that is to be decrypted, select it, and click Open.
1. The contents of the text file will be loaded into the input text box on the left side of the window.
1. Click the Configure button in the bottom center of the main window. This will show the Enigma Machine Setup window.
1. Enter the exact same configuration as was used when encrypting the text file. It is important that you get this right. Even a small difference in
   the configuration will prohibit you from successfully decrypting the file.
1. Click the Accept button to accept the configuration. You will be returned to the main window and the Enigma machine will be configured as you
   specified.
1. At this point the Apply Transform button in the top center of the main window will become enabled. Click this button to decrypt the text in the
   input text box.
1. The decrypted text will appear in the output text box on the right side of the window.

### Using the Cloaking Feature
One of the optional features implemented in the Enigma Machine application is the cloaking feature. Cloaking can be used to apply an extra level of
encryption to an already-encrypted text file. It's use is quite simple. Follow the steps given previously for encrypting a file. Before clicking the
Save button, though, click the Cloak button just to the left of the Save button. This will open the Cloak Text window where you can enter a text
string that will be used to apply the cloaking transform to the text.

The cloaking text must be at least 10 characters long. Again, it is important that you pick a word, phrase, or some other arrangement of characters
that you can easily remember. The exact text used for cloaking the encrypted text file must be used for decloaking the text file before it can be
decrypted.

When an encrypted and cloaked text file is loaded into the Enigma machine the Apply Transform button will be disabled even when the Enigma machine is
fully configured. However, the Decloak button will be enabled. You must click the Decloak button to display the Deloak Text window. You must then
enter the exact same cloaking text into this window as was used to cloak the encrypted file.

Click the Accept button once the correct cloaking text is entered. You will be returned to the main window and the cloaking text will be used to
remove the cloak from the encrypted text. The Apply Transform button will now be enabled. Simply click this button to decrypt the text file. The
decrypted text will appear in the output text box on the right side of the main window.

If you wish to test the cloaking feature without actually saving anything you can follow these steps:
1. Load a text file and encrypt it as explained in the [Basic Steps for Encrypting a File](#basic-steps-for-encrypting-a-file). Perform all steps but
   stop at the step where it has you click the Save button.
1. Click the Cloak button, enter your desired cloaking string, and click Accept.
1. Click the Move to Input button to move the encrypted and cloaked text to the input text box.
1. The Decloak button should now be enabled. Click it, enter the cloaking text, and click Accept.
1. The Apply Transform button should now be enabled. Click it.
1. The decrypted text should appear in the output text box. It should be identical to the text you started with.

### Using the Embedded Configuration Feature
The other optional feature implemented in the Enigma Machine application is the embedded configuration feature. With this feature enabled in the setup
window, when the Apply Transform button is clicked to encrypt a text file, the Enigma machine configuration will be embedded into the text appearing
in the output text box. You can tell that this feature is enabled when "== Embedded Config ==" appears under the status indicator in the center of the
main window above the Move to Input button.

When an encrypted text file containing embedded configuration is loaded into the Enigma Machine application, the application strips out the embedded
configuration and uses it to automatically configure the Enigma machine before the user can do anything else. The text that appears in the input text
box is the encrypted text minus the embedded configuration. At this point you only need to click the Apply Transform button to decrypt the text file.

It should be obvious that, by itself, this feature is not very secure. Although it removes the burden of having to remember all the configuration
settings, anyone who has a copy of the Enigma Machine application could easily decrypt any text file that has been encrypted using this feature. For
this reason it is highly advisable that if you intend on using the embedded configuration option you should also use the cloaking feature. If
possible, choose a lengthy cloaking string that is easy to remember. The longer, the better. Using a good cloaking string with the embedded
configuration option should produce an encrypted file that will be very difficult, if not impossible, to decrypt unless you have the correct cloaking
string.

## End of README File