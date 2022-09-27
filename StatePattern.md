# Design Pattern d'état #

 Le modèle de conception d'état est de type comportemental.  
 Le changement d'état de l'objet permet de modifier son comportement.

## Enoncé du problème ##

 Le client A a besoin un outil de workflow pour la gestion de ses dossiers. Le dossier doit naviguer dans les différents services en fonction de son état.  
 À chaque état, une action lui sera associée de la façon suivante : 
État | Action associées
------------- | -------------
1 | A
2  |  B
3 | C

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

Un nombre important de d'état va venir augmenter la complexité cyclomatique de la solution, c'est-à-dire le nombre de chemins possibles dans une méthode.  
Aussi la maintenance va devenir lourde lorsqu'il va falloir ajouter de nouveau état et va à l'encontre du deuxième principe SOLID. À savoir celui d'ouvert/fermé (O.C.P) qui précise ceci :

> Les objets ou entités devraient être ouverts à l’extension mais fermés à la modification.  



## Solution avec le patron de conception d'état ##

L'idée principale du state pattern est de créer de nouvelles classes pour l'ensemble des états possible d'un objet et d'extraire les comportements liés aux états dans ces classes.

Ainsi l'objet original, qui sera nommé *contexte*, stocke une référence vers un des objets, état qui représente son état actuel. Tout ce qui concerne la manipulation des états est donc délégué à cet objet.

Toutes les classes doivent implémenter la même interface de façon à pouvoir faire passer le contexte d'un état à un autre, en remplaçant l'objet état par celui qui représente son nouvel état.

## Mise en place et implémentation ##

Nous allons créeer un classe qui va prendre le rôle du contexte : 


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

Pour chaque état, nous créons une classe qui dérive de l’interface état. Contenant le code qui concerne cet état.
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

## Conclusion ##

Si le comportement d'un de vos objets est amené a varié en fonction de son état, ce patron de conception peu être un solution afin de respecter les principe SOLID de responsabilité unique et ouvert/fermé.

En organisant le code lié aux différents états dans des classes séparées tout en ajoutant de nouveaux états sans modifier les classes état ou le contexte existants.

La maintenance du code sera simplifié en évitant les gros blocs de conditionnels de l'autonmate.


