# Design Pattern d'état #

Un patron de conception apporte une solution à un problème fréquemment rencontrer en programmation orienté objet. Dans les années 90 le "gang of four" dans son ouvrage "Design Patterns: Elements of Reusable Object-Oriented Software" en a fait ressortir 3 types :
- Patrons de Création
- Patrons Comportemental
- Patrons Structurel

 Le modèle de conception d'état est de type comportemental. 

 Le changement d'état de l'objet permet de modifier son comportement.

## Enoncé du problème ##

Vous vous retrouvé a devoir gerer un changement d'état. Selon l'état de vos objets vos actions auront un comportement différent. Ceci est un cas de figure que l'on retrouve fréquement dans nos spécification.

La première idée qui pourrait venir en tête pour résoudre ce problème serait l'implémentation d'un automate fini, en se basant sur un switch, de la façon suivante :


```c#
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
```

Un nombre important d'état va venir augmenter la complexité cyclomatique de la solution, c'est-à-dire le nombre de chemins possibles dans une méthode.  
Aussi la maintenance va devenir lourde lorsqu'il va falloir ajouter de nouveau état et va à l'encontre du deuxième principe SOLID. À savoir celui d'ouvert/fermé (O.C.P) qui précise ceci :

> Les objets ou entités devraient être ouverts à l’extension mais fermés à la modification.  



## Solution avec le patron de conception d'état ##

L'idée principale du state pattern est de créer de nouvelles classes pour l'ensemble des états possible d'un objet et d'extraire les comportements liés aux états dans ces classes.

Ainsi l'objet original, qui sera nommé *contexte*, stocke une référence vers un des objets, état qui représente son état actuel. Tout ce qui concerne la manipulation des états est donc délégué à cet objet.

Toutes les classes doivent implémenter la même interface de façon à pouvoir faire passer le contexte d'un état à un autre, en remplaçant l'objet état par celui qui représente son nouvel état.

##  Cas théorique : exemple d'implémentation ##

Nous allons créer une classe qui va prendre le rôle du contexte : 


```c#
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
```

Nous déclarons classe abstraite état qui va encapsuler le comportement des états.

```csharp
public abstract class State
{
    protected Context _context;

    public void SetContext(Context context)
    {
        this._context = context;
    }

    public abstract void DoSomething();
}
```

Pour chaque état, nous créons une classe qui dérive de cette classe abstraite. Contenant le code qui concerne cet état.
```csharp
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
```


Enfin pour changer l’état du contexte, nous créons  une instance de l’une des classes état et on la passe au contexte.

```csharp
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
```
Voici le resultat à l'éxécution : 

```console
Context: Move to ConcreteStateA
ConcreteStateA wants to change the state of the context.
Context: Move to ConcreteStateB
ConcreteStateB wants to change the state of the context.
Context: Move to ConcreteStateC
ConcreteStateC wants to change the state of the context.
Context: Move to ConcreteStateA
```

## Cas concret : Le lecteur audio ##

Partons du principe que nous avons un lecteur audio avec 1 boutons qui correspondent a 1 actions possible pour l'utilisateur :
- Play : "P"


Néamoins le comportement de ses actions sera différente selon l'état du lecteur. Nous allons commencer par 2 etats :
- Lecture
- Pause


Reprennons l'exercice précédent. Nous commençons par creer notre context (ici nommé Reader) de notre lecture, qui reprendra notre actions.

```c#
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
```
Ensuite notre classe abstaire d'état ainsi que nos deux classe concréte concernant l'état de lecture et de pause.

```csharp
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



```

Afin de pouvoir tester notre implementation de façon ludique nous allons rajouter l'action Play/Pause dans le programe.cs à l'aide d'un console.readkey qui va attendre la touche P.

```csharp
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
```

Ainsi en appuyant plusieur on obtient le changement d'état suivant : 

```console
Reader paused
Press P to launch : 
P

Reader playing
P

Reader paused
P

Reader playing
```

Si vous souhaitez ajouter de nouveaux état comme une avance rapide il vous suffira d'ajouter la classe concrète correspondant à cette état puis mettre à jours les différentes action selon vos souhait. Tout en respectant facilement les principes SOLID.


## Conclusion ##

Si le comportement d'un de vos objets est amené a varié en fonction de son état, ce patron de conception peu être un solution afin de respecter les principes SOLID de responsabilité unique et ouvert/fermé.

En organisant le code lié aux différents états dans des classes séparées tout en ajoutant de nouveaux états sans modifier les classes état ou le contexte existants.

La maintenance du code sera simplifié en évitant les gros blocs de conditionnels de l'autonmate.


