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

public class program
{
    public static void Main(string[] args)
    {
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
    }
}
