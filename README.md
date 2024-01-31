Design patterns are frequently used methods in software development. They allow for optimizing, clarifying, and, most importantly, making the computer code more robust.

State Pattern
A design pattern provides a solution to a frequently encountered problem in object-oriented programming. In the 1990s, the « Gang Of Four » highlighted three categories in their work « Design Patterns: Elements of Reusable Object-Oriented Software »:

Creational patterns
Behavioral patterns
Structural patterns
The State design pattern, or State Design Pattern, is part of the behavioral patterns family. These patterns describe a class structure for the behavior of the application (e.g., a response to an event). Thus, changing the object’s state allows modifying its behavior.

Problem Statement
Imagine that you need to handle a state change. Depending on the state of your objects, your actions will have different behaviors. This is a scenario that is often encountered in specifications.

The first idea you might have to solve this problem is to implement a finite state machine, based on a switch statement, as follows:

switch (state)
{
    case 1:
        A();
        break;
    case 2:
        B();
        break;
    case 3:
        C();
        break;
}
Solution with the State Design Pattern
The main idea of the state pattern is to create new classes for all possible states of an object and extract the state-related behaviors into these classes.

Thus, the original object, referred to as the context, stores a reference to one of the objects (states) that represents its current state. Everything related to state manipulation is delegated to this object.



All classes must implement the same interface so that the context can transition from one state to another by replacing the state object with the one that represents its new state.

Theoretical Case: Example of Implementation
We are going to create a class that will take on the role of the context:

using System;
public class Context
{
    // Reference to the current state of the Context
    private State _state = null;

    public Context(State state)
    {
        this.MoveTo(state);
    }

    public void MoveTo(State state)
    {
        Console.WriteLine($"Context: Move to {state.GetType().Name}");
        this._state = state;
        this._state.SetContext(this);
    }

    public void Request()
    {
        this._state.DoSomething(this);    
    }
}

We declare the state that will encapsulate the behavior of states as an « abstract class »:

public abstract class State
{
    protected Context _context;

    public void SetContext(Context context)
    {
        this._context = context;
    }

    public abstract void DoSomething(); 
}

For each state, we create a class that derives from this abstract class, containing the code that concerns that state:

public class ConcreteStateA : State
{
    public override void DoSomething()
    {
        Console.WriteLine("ConcreteStateA wants to change the state of the context.");
        this._context.MoveTo(new ConcreteStateB());
    }
}

public class ConcreteStateB : State
{
    public override void DoSomething()
    {
        Console.WriteLine("ConcreteStateB wants to change the state of       the context.");
        this._context.MoveTo(new ConcreteStateC());
    }
}

public class ConcreteStateC : State
{
    public override void DoSomething()
    {
        Console.WriteLine("ConcreteStateC wants to change the state of the context.");
        this._context.MoveTo(new ConcreteStateA());
    } 
}

Finally, to change the state of the context, we create an instance of one of the state classes and pass it to the context:

public class program 
{
        public static void Main(string[] args)
        {
            var context = new Context(new ConcreteStateA());

            context.Request();
            context.Request();
            context.Request();
        }     
}
Here is the result upon execution:

Context: Move to ConcreteStateA
ConcreteStateA wants to change the state of the context.
Context: Move to ConcreteStateB
ConcreteStateB wants to change the state of the context.
Context: Move to ConcreteStateC
ConcreteStateC wants to change the state of the context.
Context: Move to ConcreteStateA
Concrete Case: The Audio Player
Let’s assume that we have an audio player with one button corresponding to one possible action for the user: – Play: « P »

However, the behavior of these actions will be different depending on the state of the player. Let’s start with two states: – Playing – Paused


Let’s revisit the previous exercise. We begin by creating our context (here named Reader) for our playback, which will include our action:

using System;
public class Reader
{
    // Reference to the current state of the Reader
    private ReaderState _state = null;

    public Reader(ReaderState state)
    {
         this._state = state;
    }

    public void PressPlay()
    {
        this._state.PressPlay(this);
    }

    public ReaderState CurrentState
    {
        get { return _state; }
        set { _state = value; }
    } 
}

Then our abstract state class along with our two concrete classes that correspond to the playing and paused states:

public abstract class ReaderState
{
    public abstract void PressPlay(Reader reader);
}

public class ReaderPlayingState : ReaderState
{
    public ReaderPlayingState()
    {
        Console.WriteLine("Reader playing");
    }

    public override void PressPlay(Reader reader)
    {
        reader.CurrentState = new ReaderPausedState();
    }
}

public class ReaderPausedState : ReaderState
{
    public ReaderPausedState()
    {
        Console.WriteLine("Reader paused");
    }

    public override void PressPlay(Reader reader)
    {
        reader.CurrentState = new ReaderPlayingState();     
    } 
}
To be able to test our implementation in a playful way, we will add the Play/Pause action in the program.cs using a console.readkey that will wait for the P key:

  var reader = new Reader(new ReaderPausedState());
  ConsoleKeyInfo cki;
  Console.WriteLine($"Press P to launch : ");
  do
  {
      cki = Console.ReadKey();
      if(cki.Key.ToString().Equals("P"))
      {
          Console.WriteLine($"{Environment.NewLine}");
          reader.PressPlay();
      }

  } while (cki.Key != ConsoleKey.Escape);
Thus, by pressing several times, you get the following state changes:

Reader paused
Press P to launch:
P
Reader playing
P
Reader paused
P
Reader playing
If you want to add new states, such as fast forward, for example, you just need to add the corresponding concrete class for that state. Then, you’ll need to update the different actions according to your desires, while adhering to SOLID principles.

Conclusion
If the behavior of one of your objects is expected to vary based on its state, this design pattern can be a solution to adhere to the SOLID principles of Single Responsibility and Open/Closed. To achieve this, organize the code related to different states into separate classes while adding new states without modifying existing state classes or the context. Code maintenance will be simplified by avoiding large conditional blocks in the state machine.
