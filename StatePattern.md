# Design Pattern d'état #

 Le modèle de conception d'état est de type comportemental.  
 Le changement d'état de l'objet permet de modifier son comportement.

## Enoncé du problème ##

 Le client A a besoin un outil de workflow pour la gestion de ses dossiers. Le dossier doit naviguer dans les différents services en fonction de son état.  
 À chaque état, une action lui sera associé de la façon suivante : 
Etat | Action associé
------------- | -------------
1 | A
2  |  B
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

Un nombre important de d'état va venir augmenter la compléxité cyclomatique de la solution, c'est à dire le nombre de chemins possibles dans une méthode.  
Aussi la maintenance va devenir lourde lorsqu'il va falloir ajouter de nouveau état et va a l'encontre du deuxième principe SOLID. A savoir celui de ouvert/fermé (O.C.P) qui précise ceci :

> Les objets ou entités devraient être ouverts à l’extension mais fermés à la modification.

## Solution ##


