# Engineer documentation
Engineer is a windows program used to create text-based games, and the program is written by `mforoud86/minecraftpublisher/martia`.<br>
The program itself is available to download for free and has unlimited uses, and no in-app purchases, or any purchases at all, but donation is optional.<br>
I appericiate any donations, suggestions or even improvements made to the forks that you make from the github repo!<br>
<br>
## The program
The main program contains some pretty complicated stuff, so i am going to explain them.<br>
Engineer has 2 forms, one is the Main page or the editor, and the another has the job to compile the created game.<br>
<br>
### Main page
<br>
The main page contains a menu strip at the top, a space for writing code, a release notes, a Compile button, and a log box for the compilation logs.<br>
The menu strip contains 2 main options: File and Documentation.<br>
Documentation brings you to where you are right now, and contains 2 options: The code and The IDE option.<br>
The file option, contains 3 options: Open, Save and Save As. These are all notepad options and you should know how they work.<br>
The release notes are displayed from the news.txt file from the repo.<br>
Clicking the compile button opens the compilation menu, which i am going to explain in the next section.<br>
<br>

### Compile menu

<br>
The compile menu contains 3 text boxes for the game info and the author, a debug mode option for quickly testing the game, A choose icon option for adding an icon to your game ( this nearly made me go crazy ), a status strip at the bottom and a Compile button.<br>
When you click the Choose icon button, you can choose a '.ico' file to attach to your game assembly.<br>
If you click the compile button, you can choose a directory to save the game and it's resources there, i am planning to compress everything in one file, so there isn't any issues.<br>
That's it for the compile menu and the program itself!<br>
<br>

## The engineer programming language

<br>
Well the language used to code games in Engineer doesn't have a name, so we will call it 'Engineer programming language'.<br>
The syntax is very straight forward, it contains two parts: Entities and Attributes.<br>
<br>

### Entities

<br>
The entities are the main parts of the game. defining one is very easy, and could be done like this:<br>

```
(Entity type)[Entity id ( must be unique and a number )]:
   ATTRIBUTES GO HERE
END [Entity id]
```

The current available entity types are at the end of the documentation, you can navigate there using the menu bar on the left.<br>
<br>

### Attributes

<br>
Attributes define what each Entity does. Attributes are also very to define and learn.<br>

```
( at least 3 spaces or pressing the tab button at least once ) <Attribute type>:Attribute data
```

The data formats are also unique for each attribute, so they are at the end of the documentation.<br>
<br>

## The engineer language specifications

There are 3 current entity types:

`section`, `dialog` and `choice`.<br>

# Tutorial

Sections are the main parts, they order and execute entities. The usable attribute in a section is `ref`. It's used like this:

```
(section)[1]:
   <ref>:[2]
END [1]
```

This code refers to the entity with ID 2. in this case, we can define a dialog:

```
(section)[1]:
   <ref>:[2]
END [2]

(dialog)[2]:

END [2]
```

But we still don't know how a dialog works, well dialogs are text typed on the screen by the game, and they only accept `text`, so now we can complete our code:

```
(section)[1]:
   <ref>:[2]
END [2]

(dialog)[2]:
   <text>:'Hello world!'
END [2]
```

Now we need to show another text on the screen, but do we really make another dialog? No!

```
(section)[1]:
   <ref>:[2]
END [2]

(dialog)[2]:
   <text>:'Hello world!'
   <text>:'Hello again world!'
END [2]
```

So now, i want to make a choice for the player, so they can continue the story in a different way, but how?
First, i'm going to define the dialogs.

```
(section)[1]:
   <ref>:[2]
END [2]

(dialog)[2]:
   <text>:'Hello world!'
   <text>:'Hello again world!'
END [2]

(dialog)[3]:
   <text>:'You chose yes!'
END [3]

(dialog)[4]:
   <text>:'You chose no!'
END [2]
```

Now that is done, but how do we... make the choice?
Let's see, choices give a text and options for the player to choose from. They accept a `text` at the beginning and multiple `option` attributes.

```
(section)[1]:
   <ref>:[2]
END [2]

(dialog)[2]:
   <text>:'Hello world!'
   <text>:'Hello again world!'
END [2]

(dialog)[3]:
   <text>:'You chose yes!'
END [3]

(dialog)[4]:
   <text>:'You chose no!'
END [2]

(choice)[5]:
   <text>:'Yes or no?'
   <option>:['Yes :)',3]
   <option>:['No... :(',4]
END [5]
```

Now you should pay attention to NOT put any spaces between the attributes in the OPTION.

**INCORRECT CODE**: `<option>:['Yes', 2]`
**CORRECT CODE**:   `<option>:['Yes',2]`

I've decided that i need the player's name and talk to them to make them feel the game a bit more.
What do i do?
Correct!
Use responsives!
Responsives are included in the v0.5 update, and they accept `text` and `name` only once.
So i could update my code like this, first i'm going to add the dialoges for the responsives:

```
(section)[1]:
   <ref>:[2]
   <ref>:[6]
   <ref>:[7]
END [2]

(dialog)[2]:
   <text>:'Hello world!'
   <text>:'Hello again world!'
END [2]

(dialog)[3]:
   <text>:'You chose yes!'
END [3]

(dialog)[4]:
   <text>:'You chose no!'
END [2]

(choice)[5]:
   <text>:'Yes or no?'
   <option>:['Yes :)',3]
   <option>:['No... :(',4]
END [5]

(responsive)[6]:
   
END [6]

(dialog)[7]:
   <text>:'Nice to meet you, {name}!'
END [7]

```

Now i can include the responsive:

```
(section)[1]:
   <ref>:[2]
   <ref>:[6]
   <ref>:[7]
END [2]

(dialog)[2]:
   <text>:'Hello world!'
   <text>:'Hello again world!'
END [2]

(dialog)[3]:
   <text>:'You chose yes!'
END [3]

(dialog)[4]:
   <text>:'You chose no!'
END [2]

(choice)[5]:
   <text>:'Yes or no?'
   <option>:['Yes :)',3]
   <option>:['No... :(',4]
END [5]

(responsive)[6]:
   <text>:'What is your name?'
   <name>:'name'
END [6]

(dialog)[7]:
   <text>:'Nice to meet you, {name}!'
END [7]

```

Enjoy!
I would update this tutorial when there is a major change.
Cya!

# The finish line

We are done!<br>
I mean for now, because the updates will be pushed frequently and this will update ASAP!

Thanks for reading :3
