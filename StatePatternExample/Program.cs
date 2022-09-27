using System;


public class Context
{
    // Reference to the current state of the Context
    protected State _state;

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
        this._state.DoSomething();
    }
}

public abstract class State
{
    protected Context _context;

    public void SetContext(Context context)
    {
        this._context = context;
    }

    public abstract void DoSomething();
}

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
        Console.WriteLine("ConcreteStateB wants to change the state of the context.");
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

public class program
{
    public static void Main(string[] args)
    {
        var context = new Context(new ConcreteStateA());

        context.Request();
        context.Request();
        context.Request();

        Console.ReadKey();
    }
}